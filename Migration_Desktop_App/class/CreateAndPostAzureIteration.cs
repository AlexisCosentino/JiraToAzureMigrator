using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration_Desktop_App
{
    public class CreateAndPostAzureIteration
    {
        public const string BASE = "https://dev.azure.com";
        string PAT;
        public string ORG;
        public const string API = "api-version=6.0";
        public string PROJECT;
        public string WIT_TYPE;
        public string[] sprint_ext = { "DEV", "INNOV", "MOB", "DIG" };

        public CreateAndPostAzureIteration()
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.PAT = (string?)data["azureToken"];
            this.ORG = "IRIUMSOFTWARE";
        }

        public void migrateIteration(TextBox res)
        {
            var connect = new DB_Connection();
            connect.query = "select NAME, dateadd(s, convert(bigint, START_DATE) / 1000, convert(datetime, '1-1-1970 00:00:00')), dateadd(s, convert(bigint, END_DATE) / 1000, convert(datetime, '1-1-1970 00:00:00')) from AO_60DB71_SPRINT";
            var sprintList = connect.getListOfSprint();
            foreach(var sprint in sprintList)
            {
                foreach(string ext in this.sprint_ext)
                {
                    if (sprint[0].StartsWith(ext))
                    {
                        string start = translateDate(sprint[1]);
                        string end = translateDate(sprint[2]);
                        string sprintJson = "{ \"name\": \" " + sprint[0] + "\", \"attributes\": { \"startDate\": \"" + start + "\", \"finishDate\": \"" + end + "\" }}";
                        if ( ext == "DEV")
                        {
                            this.PROJECT = "Locpro";
                        } else if ( ext == "INNOV" || ext == "DIG")
                        {
                            this.PROJECT = "Digital"; 
                        } else if ( ext == "MOB")
                        {
                            this.PROJECT = "Mobility";
                        } else
                        {
                            this.PROJECT = "TEST_ALEXIS";
                        }

                        Console.WriteLine($"--------------Projet = {this.PROJECT} & sprint name = {sprint[0]} for the date {start} and {end} --------------");

                        PostIterationToAzure(sprintJson, res);

                    }
                }
            }
        }

        public void PostIterationToAzure(string jsonToPost, TextBox res)
        {
            HttpClient client = new HttpClient();

            // Set Media Type of Response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Generate base64 encoded authorization header.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", PAT))));

            // Build the URI for creating Work Item.
            string uri = String.Join("?", String.Join("/", BASE, ORG, PROJECT, "_apis/wit/classificationnodes/Iterations"), API);




            // Create Request body in JSON format.
            HttpContent content = new StringContent(jsonToPost, Encoding.UTF8, "application/json");

            // Call CreateWIT method.
            string result = PostAsyncIteration(client, uri, content).Result;


            // Pretty print the JSON if result not empty or null.
            if (!String.IsNullOrEmpty(result))
            {
                try
                {
                    dynamic sprint = JsonConvert.DeserializeObject<object>(result);
                    Console.WriteLine(JsonConvert.SerializeObject(sprint, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            client.Dispose();
        }

        public async Task<string> PostAsyncIteration(HttpClient client, string uri, HttpContent content)
        {
            Console.WriteLine(uri);
            try
            {
                // Send asynchronous POST request.
                using (HttpResponseMessage response = await client.PostAsync(uri, content).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return (await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ex.ToString();
            }
        } // End of CreateWIT method

        public string translateDate(string date)
        {

            var parsedDate = DateTime.Parse(date);
            return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc).ToString("s") + "Z";
        }
    }
}
