using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using static System.Net.Mime.MediaTypeNames;


namespace Migration_Desktop_App
{

    public class Translate_Jira_To_Azure
    {
        Dictionary<string, string> ticketData;
        public string comment { get; set; }
        public string attachment { get; set; }
        public List<string> attachment_url { get; set; }= new List<string>();
        public Dictionary<string, string> comment_dict { get; set; }



        public Translate_Jira_To_Azure(Dictionary<string, string> dict)
        {
            ticketData = dict;
        }


        public string createJsonWithPBIToPost()
        {
            var parsedDate = DateTime.Parse(ticketData["created"]);
            if (parsedDate.Date == DateTime.Today)
            {
                // WARNING = if date of creation is less than 2hours, an error gonna occurs, thats why i substract 2h in case of ticket from today.
                parsedDate = parsedDate.AddHours(-2);
            }
            //"2022-11-29T12:26:05.707"
            var createdDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc).ToString("s") + ".000Z";

            string dueDate = translateDateTimeToAzure(ticketData["dueDate"]);
            string startDate = translateDateTimeToAzure(ticketData["startDate"]);
            string endDate = translateDateTimeToAzure(ticketData["endDate"]);
            string worklog = getWorkLog(ticketData["workLog"], ticketData["totalWorkTime"]);
            string originalEstimate = getOriginalEstimateInHour(ticketData["originalEstimate"]);
            string creator = mappingUsers(ticketData["creator"]);
            string assignee = mappingUsers(ticketData["assignee"]);
            string reporter = mappingUsers(ticketData["reporter"]);

            if (ticketData["projectCategoryType"] == "Projet interne (DDP)" || ticketData["projectCategoryType"] == "Projet interne (DID)")
            {
                ticketData["ProjectName"] = "";
            }


            foreach (var item in ticketData.Keys)
            {
                if (item == "areaPath")
                {
                    continue;
                }
                ticketData[item] = cleanJson(ticketData[item]);
            }

            var component = getOneComponent(ticketData["componentList"]);
    

            string jsonToPost = "[{ \"op\": \"add\", \"path\": \"/fields/System.Title\", \"from\": null, \"value\": \"" + ticketData["summary"] + "\"}";
            jsonToPost += ", { \"op\": \"add\", \"path\": \"/fields/System.Description\", \"from\": null, \"value\": \"" + ticketData["description"] +"\"} ";
            jsonToPost += ", { \"op\": \"add\", \"path\": \"/fields/System.State\", \"from\": null, \"value\": \""+ ticketData["issueStatus"] +"\"}";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.CreatedBy\", \"value\": \"" + creator + "\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.AssignedTo\", \"value\": \"" + assignee + "\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.CreatedDate\", \"value\": \""+ createdDate +"\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.AreaPath\", \"value\": \""+ ticketData["areaPath"] +"\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.Tags\", \"value\": \""+ ticketData["ListOfLabels"] +" \" }";
//           jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.Type\", \"value\": \"" + ticketData["issueType"] + "\" }";
//           jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.PriorityField\", \"value\": \"" + ticketData["priority"] + "\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Microsoft.VSTS.Scheduling.DueDate\", \"value\": \"" + dueDate + "\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Microsoft.VSTS.Scheduling.StartDate\", \"value\": \"" + startDate + "\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Microsoft.VSTS.Scheduling.OriginalEstimate\", \"value\": \""+ originalEstimate +"\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Microsoft.VSTS.Scheduling.Effort\", \"value\": \"" + originalEstimate + "\" }";
            //            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.Enddate\", \"value\": \"" + endDate + "\" }";
            //            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.WorkLog\", \"value\": \"" + worklog + "\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.JiraLink\", \"value\": \"https://worklog.vega-systems.com/browse/"+ ticketData["linkToJira"] +"\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.Version\", \"value\": \""+ ticketData["fixedVersionList"] +"\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.Order\", \"value\": \""+ component +"\" }";
            jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/Custom.Customer\", \"value\": \"" + ticketData["ProjectName"] + "\" }";
            jsonToPost = check4sprint(jsonToPost, ticketData);
            jsonToPost += "]";
            Console.WriteLine(jsonToPost);
            return jsonToPost;
        }

        public string createJsonWithCommentToPost()
        {
            comment = cleanJson(comment_dict["comment"]);
            foreach (var a in this.attachment_url)
            {
                comment = editHTMLTagsDependsAttachment(a, comment);
            }
            comment = String.Join("<br><br>", $"<h2><strong>Ecrit par {comment_dict["author"]}</strong></h2> <h4>Le {comment_dict["created_date"]}</h4>", comment);
            string jsonToPost = "{ \"text\": \"" + comment + "\"}";
            return jsonToPost;
        }

        public string createJsonToPatchPBIWithAttachment()
        {
            string json = "[{\"op\": \"add\", \"path\": \"/relations/-\", \"value\": { \"rel\": \"AttachedFile\", \"url\": \""+ attachment +"\", \"attributes\": {\"comment\": \"Spec for the work\"}}}";
            json += "]";
            this.attachment_url.Add(attachment);
            return json;
        }

        public string getDescriptionJson()
        {
            if (attachment_url.Count > 0)
            {
                foreach (var a in this.attachment_url)
                {
                    ticketData["description"] = editHTMLTagsDependsAttachment(a, ticketData["description"]);

                }
                string json = "[{ \"op\": \"add\", \"path\": \"/fields/System.Description\", \"from\": null, \"value\": \"" + ticketData["description"] + "\"}]";

                return json;
            }
            else
            {
              return "false";
            }
        }

        public string cleanJson(string toformat)
        {
            toformat = toformat.Replace("{code:java}", "<code>");
            toformat = toformat.Replace("{code:java}", "<code>");
            
            toformat = toformat.Replace("{code}", "</code>");
            toformat = toformat.Replace("\r\n *****", "<br>&emsp;&emsp;&emsp;&emsp;&emsp;\t■");
            toformat = toformat.Replace("\r\n ****", "<br>&emsp;&emsp;&emsp;&emsp;\t■");
            toformat = toformat.Replace("\r\n ***", "<br>&emsp;&emsp;&emsp;\t■");
            toformat = toformat.Replace("\r\n **", "<br>&emsp;&emsp;\t■");
            toformat = toformat.Replace("\r\n *", "<br>&emsp;\t■");
            toformat = toformat.Replace("\r\n", "<br>"); //Transate line breaker
            toformat = toformat.Replace("\"", " "); // Remove every double quote of the text
            toformat = toformat.Replace("\\", "");  // Remove every backslash of the text
            toformat = toformat.Replace("*[", "<strong>[");
            toformat = toformat.Replace("]*", "]</strong>");
            return toformat;
        }

        public string editHTMLTagsDependsAttachment(string a, string desc)
        {
            string formated;
            string file = HttpUtility.UrlDecode(a.Split("fileName=").Last());
            if (file.Split('.').Last() == "png" || file.Split('.').Last() == "jpeg" || file.Split('.').Last() == "jpg" || file.Split('.').Last() == "gif")
            {
                formated = desc.Replace(file, $"<img alt='img_url' src='{a}' >"); ;
            }
            else
            {
                formated = desc.Replace(file, $"<a href='{a}' target='_blank'>{file}</a>");
            }
            return formated;
        }

        public string translateDateTimeToAzure(string date)
        {
            if (!String.IsNullOrEmpty(date))
            {
                date = DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Utc).ToString("s") + ".000Z";
            }
            return date;
        }
        
        public string getWorkLog(string wl, string total)
        {
            string workLogString = "";
            if (!String.IsNullOrEmpty(wl))
            {
                foreach (string log in wl.Split(';'))
                {
                    workLogString += $"{log} <br> ";
                }
                workLogString += $"Soit un total de <strong>{total} heures </strong>";
            }
            return workLogString;
        }

        public string jsonForTimeTrackerAPI(string log, string id)
        {
            var listOfLog = log.Split('~');
            string date = DateTime.SpecifyKind(DateTime.Parse(listOfLog[0], new CultureInfo("fr-FR")), DateTimeKind.Utc).ToString("s") + ".000Z";
            string user = getAzureUser(listOfLog[1]);
            string second = listOfLog[2];
            return "{\"timeStamp\": \""+ date +"\", \"length\": \""+ second +"\", \"billableLength\": 0, \"workItemId\": \""+ id +"\", \"comment\": \"Sent by API for user  "+ listOfLog[1] +" \", \"userId\": \""+ user +"\"}";
        }

        public string getAzureUser(string username)
        {
            GetJsonWithTimeTrackerUsersID tracker = new GetJsonWithTimeTrackerUsersID();
            var dict_of_user_id = tracker.getRequest();
            foreach(string user in dict_of_user_id.Keys)
            {
                if (user == username + "@irium-software.com")
                {
                    return dict_of_user_id[user];
                }
            }
            return "2509f065-156d-4dcc-bd12-bc29a10657b8";
        }

        public string getOneComponent(string c_list)
        {

            string[] subs = c_list.Split(';');
            return subs[0];
        }

        public string getOriginalEstimateInHour(string time)
        {
            if (!String.IsNullOrEmpty(time))
            {
                int timeInt = Int32.Parse(time);
                timeInt = (timeInt / 60) / 60;
                return timeInt.ToString();
            }
            return "";

        }

        public string mappingUsers(string user)
        {
            if (!String.IsNullOrEmpty(user))
            {
                var temp = user.Split('@');
                if (temp[0] == "yvinee")
                {
                    return "y.vinee@irium-software.com";
                }
                if (temp[0] == "lpatissier")
                {
                    return "l.patissier@irium-software.com";
                }
                if (temp[0] == "cdavid")
                {
                    return "chdavid@irium-software.com";
                }
                return temp[0] + "@irium-software.com";
            }
            return "";
        }

        public string check4sprint(string jsonToPost, Dictionary<string, string> ticketData)
        {
            if (!String.IsNullOrEmpty(ticketData["sprintList"]))
            {
                var sprints = ticketData["sprintList"].Split(",");
                string sprint = sprints.Last();
                var name = sprint.Split("_");
                if (name[0] == "MOB" && ticketData["azureProject"] == "Mobilité")
                {
                    return jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.IterationPath\", \"value\": \"\\\\Mobility\\\\" + sprint + "\" }";
                }
                else if (name[0] == "INNOV" && ticketData["azureProject"] == "Digital")
                {
                    return jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.IterationPath\", \"value\": \"\\\\Digital\\\\" + sprint + "\" }";
                }
                else if (name[0] == "DIG" && ticketData["azureProject"] == "Digital")
                {
                    return jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.IterationPath\", \"value\": \"\\\\Digital\\\\" + sprint + "\" }";
                }
                else if (name[0] == "DEV" && ticketData["azureProject"] == "Locpro")
                {
                    return jsonToPost += ", {\"op\": \"add\", \"path\": \"/fields/System.IterationPath\", \"value\": \"\\\\Locpro\\\\" + sprint + "\" }";

                }
                else
                {
                    foreach (string sp in ticketData["sprintList"].Split(','))
                    {
                        if (!string.IsNullOrEmpty(sp))
                        {
                            ticketData["ListOfLabels"] += $"sprint : {sp};";
                        }
                    }
                }
            }

            return jsonToPost;
        }
    }
}