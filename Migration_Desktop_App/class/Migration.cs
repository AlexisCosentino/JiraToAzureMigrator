using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Migration_Desktop_App;
using System.Web;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection.Metadata;
using System.Security.Policy;

namespace Migration_Desktop_App
{
    public class Migration
    {
        DB_Connection connection;
        Post_PBI_To_Azure Post_PBI_To_Azure;
        Post_Comment_To_Azure_PBI Post_Comment_To_Azure_PBI;
        Translate_Jira_To_Azure Translate_Jira_To_Azure;
        Post_Attachment_To_Azure Post_Attachment_To_Azure;
        Patch_PBI_To_Azure Patch_PBI_To_Azure;
        Post_Worklog_To_TimeTracker Post_Worklog_To_TimeTracker = new Post_Worklog_To_TimeTracker();
        string organisation_azure = "IRIUMSOFTWARE";
        string project_azure;

        public Migration()
        {
        }

        public void launchMigration(string query, TextBox result, Label counter, ProgressBar pb1)
        {
            //GET EXECUTION TIME !!
            Stopwatch stwatch = new Stopwatch();
            stwatch.Start();
            connection = new DB_Connection();
            connection.query = get_query(query);
            var dict_of_pbi = connection.getDictOfPBI();


            var total = dict_of_pbi.Count;
            pb1.Maximum = total;
            int i = 0;
            result.Text += $"There is {total} PBI to migrate {Environment.NewLine}{Environment.NewLine}";
            foreach (var dict in dict_of_pbi.Values)
                {
                result.Text += $"L'issue N° {i +1}, ";
                counter.Text = $"{i + 1} / {total}";
                dict["ListOfLabels"] = getLabelsSprint(dict);

                dict["issueStatus"] = translateStatusToAzure(dict["issueStatus"]);

                GetTypeOfPBI(dict);

                GetProjectAndArea(dict);

                //break if status is unknown -> means to complete, Demande, A valider
                if (dict["issueStatus"] == "Unknown")
                {
                    i++;
                    pb1.Value = i;
                    pb1.ForeColor = Color.FromArgb(112, 191, 155);
                    result.Text += $" Non pris en compte {Environment.NewLine}";
                    continue;
                }

                //init project name
                this.project_azure= dict["azureProject"];


                //
                Translate_Jira_To_Azure = new Translate_Jira_To_Azure(dict);
                 var json = Translate_Jira_To_Azure.createJsonWithPBIToPost();
                 Post_PBI_To_Azure = new Post_PBI_To_Azure(this.organisation_azure, this.project_azure, dict["issueType"]);
                 Post_PBI_To_Azure.PostPBIToAzure(json, result);

                 string PBI_ID = Post_PBI_To_Azure.ID_of_PBI;

                    //Get every Attachments related of PBI
                connection.query = $"SELECT id ,mimetype, filename FROM fileattachment Where issueid = {dict["issueNb"]};";
                var list_of_attachments = connection.getListOfAttachments();

                foreach (var attachment in list_of_attachments)
                {

                    // first we need post jira link to azure server, dont forget use webclient to connect to jira account because data is secured
                    // return azure link and use it to make json
                    // then use this json to patch on azure PBI
                    Post_Attachment_To_Azure = new Post_Attachment_To_Azure(PBI_ID, this.organisation_azure, this.project_azure);
                    var filename = attachment.Split('/').Last();
                    filename = HttpUtility.UrlEncode(filename);
                    var azure_link = Post_Attachment_To_Azure.PatchAttachmentToAzureServer(attachment, filename);
                    Translate_Jira_To_Azure.attachment = azure_link;
                    var attachment_json_to_post = Translate_Jira_To_Azure.createJsonToPatchPBIWithAttachment();
                    Patch_PBI_To_Azure = new Patch_PBI_To_Azure(PBI_ID, this.organisation_azure, this.project_azure);
                    Patch_PBI_To_Azure.patchPBIToAzure(attachment_json_to_post);
                }
                 var descriptionJson = Translate_Jira_To_Azure.getDescriptionJson();
                if (descriptionJson != "false")
                {
                    Patch_PBI_To_Azure.patchPBIToAzure(descriptionJson);
                }


                //PATCH PBI with relation parent and child
                patchRelationToPBI(dict["linkParent"], PBI_ID);
                patchRelationToPBI(dict["linkChild"], PBI_ID);


                    //Get every comments related of PBI
                connection.query = $"SELECT jiraaction.issueid, username = (select  lower_user_name from app_user where jiraaction.author=user_key), jiraaction.actionbody, jiraaction.CREATED,  jiraaction.id FROM jiraissue, project, jiraaction WHERE jiraaction.issueid = jiraissue.id  and project.id = jiraissue.project and issueid = {dict["issueNb"]} and project = {dict["project"]} ORDER BY jiraissue.CREATED DESC;";
                var dict_of_comments = connection.getDictOfComments();
                foreach (var comment in dict_of_comments.Values)
                {
                    Translate_Jira_To_Azure.comment_dict = comment;
                    var comment_json_to_post = Translate_Jira_To_Azure.createJsonWithCommentToPost();
                    Post_Comment_To_Azure_PBI = new Post_Comment_To_Azure_PBI(PBI_ID, this.organisation_azure, this.project_azure);
                    Post_Comment_To_Azure_PBI.postCommentToAzurePBI(comment_json_to_post);
                }

                // POST WORKLOG TO TIMETRACKER
                if (!String.IsNullOrEmpty(dict["worklog2"]))
                {
                    foreach (var log in dict["worklog2"].Split(';'))
                    {
                        var jsonWorklogToPost = Translate_Jira_To_Azure.jsonForTimeTrackerAPI(log, PBI_ID);
                        Post_Worklog_To_TimeTracker.PostJsonToTimeTracker(jsonWorklogToPost);
                    }
                }
                i++;
                pb1.Value = i;
                pb1.ForeColor = Color.FromArgb(112, 191, 155);
            }
            stwatch.Stop();
            var exec_time = String.Format("{0:00}:{1:00}.{2:00}", stwatch.Elapsed.Minutes, stwatch.Elapsed.Seconds,
            stwatch.Elapsed.Milliseconds / 10);
            result.Text += $"{Environment.NewLine}END...  {total} Issues has been migrated. {Environment.NewLine}Execution time is {exec_time}";
        }

        private string get_query(string query_name)
        {
            JObject data = JObject.Parse(File.ReadAllText("queries.json"));
            return (string?)data[query_name];

        }

        private string getLabelsSprint(Dictionary<string, string> dict)
        {
            string labelsString = "";
            foreach(string label in dict["labelsList"].Split(','))
            {
                if (!string.IsNullOrEmpty(label))
                {
                    labelsString += $"{label}; ";
                }
            }
            return labelsString;
        }

        public string translateStatusToAzure(string status)
        {
            switch (status)
            {
                case "Acceptée":
                    status = "New";
                    return status;
                case "A Compléter":
                    status = "Unknown";
                    return status;
                case "Attente test":
                    status = "Done";
                    return status;
                case "Cloturée":
                    status = "Done";
                    return status;
                case "Demande":
                    status = "Unknown";
                    return status;
                case "EN ATTENTE":
                    status = "Approved";
                    return status;
                case "En cours":
                    status = "Approved";
                    return status;
                case "Rejetée":
                    status = "Removed";
                    return status;
                case "Terminée":
                    status = "DevDone";
                    return status;
                case "Test KO":
                    status = "Approved";
                    return status;
                case "A tester":
                    status = "To test";
                    return status;
                case "A Valider":
                    status = "Unknown";
                    return status;
                default:
                    status = "Unknown";
                    return status;
            }
        }

        public void GetTypeOfPBI(Dictionary<string, string> dict)
        {
            if (dict["issueType"] == "Evolution " && dict["ProjectName"] == "VEGA9008-Dev pour Maint Niv2")
            {
                dict["issueType"] = "Evolution Support";
            }
            else if (dict["issueType"] == "Evolution " && dict["projectCategoryType"] == "Projet client (DPS)" && string.IsNullOrEmpty(dict["componentList"]))
            {
                dict["issueTye"] = "Evolution Consultants";
            }
            else if (dict["issueType"] == "Evolution " && dict["projectCategoryType"] == "Projet interne (DDP)" && dict["ProjectName"] != "VEGA9008-Dev pour Maint Niv2")
            {
                dict["issueType"] = "Roadmap";
            }
            else if (dict["issueType"] == "Evolution " && dict["projectCategoryType"] == "Projet interne (DID)")
            {
                dict["issueType"] = "Roadmap";
            }
            else if (dict["projectCategoryType"] == "Projet client (DPS)" && !string.IsNullOrEmpty(dict["componentList"]) && !dict["componentList"].Contains("VEGAMAINT"))
            {
                dict["issueType"] = "Contrat";
            }
            else if (dict["ProjectName"] == "VEGA0000 - Interne VEGA")
            {
                dict["issueType"] = "Interne";
            }
            else if ( (dict["projectCategoryType"] == "Projet client (DPS)" || dict["projectCategoryType"] == "Projet interne (DDP)" || dict["projectCategoryType"] == "Projet interne (DID)") && ( dict["componentList"].Contains("VEGAMAINT") || (dict["issueType"] == "Bug") ) )
            {
                dict["issueType"] = "Bug";
            }
            else if (dict["ProjectName"] == "VEGA9008-Dev pour Maint Niv2" && dict["issueType"] == "Bug")
            {
                dict["issueType"] = "Bug";
            }
            else
            {
                dict["issueType"] = "Bug";
            }

        }

        public void GetProjectAndArea(Dictionary<string, string> dict)
        {
            if (dict["projectCategoryType"] == "Projet interne (DID)" || dict["projectCategoryType"] == "Projet interne (DDP)")
            {
                if (dict["ProjectName"] == "VEGA9010 - Locpro Windows" || dict["ProjectName"] == "VEGA9008-Dev pour Maint Niv2" || dict["ProjectName"] == "VEGA9009 - Ergonomie / Design" || dict["description"].IndexOf("locpro win", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\LpWindows");
                }
                else if (dict["ProjectName"] == "VEGA901W - Locpro Web" || dict["description"].IndexOf("locpro web", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\LpWeb");
                }
                else if (dict["ProjectName"] == "VEGA901D - DevisLoc" || dict["description"].IndexOf("devisloc", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\DevisLoc");
                }
                else if (dict["ProjectName"] == "VEGA9010 - Locpro Windows" && (dict["labelsList"].Contains("LPservice") || dict["labelsList"].Contains("LPService")))
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\LpService");
                }
                else if (dict["ProjectName"] == "VEGA9010 - Locpro Windows" && dict["labelsList"].Contains("LPSMS"))
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\LpSMS");
                }
                else if (dict["ProjectName"] == "VEGA8001 - Master" || dict["ProjectName"] == "VEGA8008 - Master AKANEA")
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\Master");
                }
                else if (dict["ProjectName"] == "VEGA8005 - LpReportServer")
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\LpReportServer");
                }
                else if (dict["ProjectName"] == "VEGA9006 - Tests Irium" || dict["ProjectName"] == "VEGA9002 - Qualité / Tests")
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro\\\\Tests");
                }
                else if (dict["ProjectName"] == "GYZMO10000 - Mobility" || dict["description"].IndexOf("Application (", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\_Global");
                }
                else if (dict["ProjectName"] == "GYZMO10001 - Interventions")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\Interventions");
                }
                else if (dict["ProjectName"] == "GYZMO10002 - iMob Check +")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobCheckPlus");
                }
                else if (dict["ProjectName"] == "GYZMO10003 - Entrées / Sorties")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\EntréesSorties");
                }
                else if (dict["ProjectName"] == "GYZMO10004 - Little Move")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\LittleMove");
                }
                else if (dict["ProjectName"] == "GYZMO10005 - Gestion RH")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\GestionRH");
                }
                else if (dict["ProjectName"] == "GYZMO10007 - iMob Rent 24/7")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobRent247");
                }
                else if (dict["ProjectName"] == "GYZMO10008 - iMob Delivery")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobDelivery");
                }
                else if (dict["ProjectName"] == "GYZMO10009 - H24 BTP")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobRentBTP");
                }
                else if (dict["ProjectName"] == "GYZMO10010 - iMob Clock")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobClock");
                }
                else if (dict["ProjectName"] == "GYZMO10011 - iMob Stock")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobStock");
                }
                else if (dict["ProjectName"] == "GYZMO10012 - iMobService")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobService");
                }
                else if (dict["ProjectName"] == "GYZMO10013 - IMobContact")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobContact");
                }
                else if (dict["ProjectName"] == "GYZMO10014 - IMobcheck")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobCheck");
                }
                else if (dict["ProjectName"] == "GYZMO10015 - iMob Expertises")
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility\\\\iMobExpertise");
                }
                else if (dict["ProjectName"] == "VEGA7001 - SAAS LP TRACKER")
                {
                    dict.Add("azureProject", "Digital");
                    dict.Add("areaPath", "Digital\\\\iTracker");
                }
                else if (dict["ProjectName"] == "VEGA8002 - Site Web" || dict["description"].IndexOf("site internet", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Digital");
                    dict.Add("areaPath", "Digital\\\\iWebRent");
                }
                else if (dict["ProjectName"] == "VEGA8003 - LP3K" || dict["description"].IndexOf("locpro 3k", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Digital");
                    dict.Add("areaPath", "Digital\\\\Lp3K");
                }
                else if (dict["ProjectName"] == "VEGA8004 - LpAPI Back End")
                {
                    dict.Add("azureProject", "Digital");
                    dict.Add("areaPath", "Digital\\\\LpBackEnd");
                }
                else if (dict["ProjectName"] == "VEGA8006 - BI Noyau")
                {
                    dict.Add("azureProject", "Digital");
                    dict.Add("areaPath", "Digital\\\\BI");
                }
                else if (dict["ProjectName"] == "VEGA8007 - Locpro Fleet")
                {
                    dict.Add("azureProject", "Digital");
                    dict.Add("areaPath", "Digital\\\\iWebFleet");
                }
                else if (dict["ProjectName"] == "VEGA0000 - Interne VEGA")
                {
                    dict.Add("azureProject", "Interne Service");
                    dict.Add("areaPath", "Interne Service");
                    dict["issueType"] = "Issue";
                }
                else if (dict["ProjectName"] == "VEGA9999 - Recherche")
                {
                    dict.Add("azureProject", "Interne Service");
                    dict.Add("areaPath", "Interne Service\\\\Recherche");
                    dict["issueType"] = "Issue";
                }
                else if (dict["ProjectName"] == "INEOS")
                {
                    dict.Add("azureProject", "Interne Service");
                    dict.Add("areaPath", "Interne Service\\\\Neos");
                    dict["issueType"] = "Issue";
                }
                else
                {
                    dict.Add("azureProject", "TEST_ALEXIS");
                    dict.Add("areaPath", "TEST_ALEXIS");
                }
            }
            else if (dict["projectCategoryType"] == "Projet client (DPS)")
            {
                if (dict["service"] == "DevAgilité" || dict["service"] == "Développement" || dict["description"].IndexOf("locpro win", StringComparison.CurrentCultureIgnoreCase) >= 0 || dict["description"].IndexOf("locpro web", StringComparison.CurrentCultureIgnoreCase) >= 0 || dict["description"].IndexOf("devisloc", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Locpro");
                    dict.Add("areaPath", "Locpro");
                }
                else if (dict["service"] == "Mobilité" || dict["description"].IndexOf("Application (", StringComparison.CurrentCultureIgnoreCase) >= 0 || dict["description"].IndexOf("site internet", StringComparison.CurrentCultureIgnoreCase) >= 0 || dict["description"].IndexOf("locpro 3k", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    dict.Add("azureProject", "Mobility");
                    dict.Add("areaPath", "Mobility");
                }
                else if (dict["service"] == "Digital")
                {
                    dict.Add("azureProject", "Digital");
                    dict.Add("areaPath", "Digital");
                }
                else
                {
                    dict.Add("azureProject", "TEST_ALEXIS");
                    dict.Add("areaPath", "TEST_ALEXIS");
                }

            }
            else
            {
                dict.Add("azureProject", "TEST_ALEXIS");
                dict.Add("areaPath", "TEST_ALEXIS");
            }

        }

        public void patchRelationToPBI(string relations, string id)
        {
            foreach (string r in relations.Split(';'))
            {
                if(!String.IsNullOrEmpty(r))
                {
                    Post_Query_To_Azure pq = new Post_Query_To_Azure(r);
                    try
                    {
                        pq.patchWithRelation();
                        string id_of_rel = pq.ID_of_PBI;
                        Console.WriteLine($"l'ID du pbi relation est {id_of_rel}");
                        Patch_Relation_To_PBI pr = new Patch_Relation_To_PBI(id, id_of_rel);
                        pr.patchRelationToAzure();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}

/**  
 * 
 * 
 * 
 * 
 * 
[ Clonage de tache from PBI, comment surcharger ? ]
Oui c'est possible de mettre des conditions,    si parent = Contrat, Bug...
                                                si parent tag = "SprintRevew" ...
                                                si parent state = New, Approved...
Je peux créer unu ou plusieurs taches en fonction de ses conditions la. Avec des valeurs perso dans chaque champs de la tache.
Voici le Json à rentrer dans la descritpion du template:
{"applywhen": { "System.State": "New", "System.WorkItemType": "Contrat"} }

Réfléhir au différents cas et ensuite écrire le json puis attribuer les valeurs aux champs.





[ Regarder Effort ]


[ Comment PBI ou Tache par le dev ]
Le PBI est créé par une personne non technique donc elle doit être écrite et commenter dans un langage client.
La tache cependant peut être écrite de manière très technique par le développeur.

[ Réaliser un graphique workflow Azure ]
Fait sur Lucid Chart
 *
 */