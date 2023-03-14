using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration_Desktop_App
{
    public class Post_Query_To_Azure
    {
        public const string BASE = "https://dev.azure.com";
        string PAT;
        public string jira_code;
        public string ID_of_PBI { set; get; }

        public Post_Query_To_Azure(string jira_code)
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.PAT = (string?)data["azureToken"];
            this.jira_code = jira_code;
        }

        public void patchWithRelation()
        {
            string json = "{\"query\": \"Select [System.Id] From WorkItems Where [Custom.JiraLink] = 'https://worklog.vega-systems.com/browse/"+ this.jira_code +"'\"}";
            Console.WriteLine(json);
            PostQuery(json);


        }

        public void PostQuery(string jsonToPost)
        {
            HttpClient client = new HttpClient();

            // Set Media Type of Response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Generate base64 encoded authorization header.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", PAT))));

            string uri = "https://dev.azure.com/IRIUMSOFTWARE/_apis/wit/wiql?api-version=7.1-preview.2";

            // Create Request body in JSON format.
            HttpContent content = new StringContent(jsonToPost, Encoding.UTF8, "application/json");

            // Call CreateWIT method.
            string result = CreateWIT(client, uri, content).Result;


            // Pretty print the JSON if result not empty or null.
            if (!String.IsNullOrEmpty(result))
            {
                try
                {
                    dynamic id = JsonConvert.DeserializeObject<object>(result);
                    Console.WriteLine(JsonConvert.SerializeObject(id, Formatting.Indented));
                    this.ID_of_PBI = id["workItems"][0]["id"].ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            client.Dispose();
        }

        public async Task<string> CreateWIT(HttpClient client, string uri, HttpContent content)
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
        }
    }
}
