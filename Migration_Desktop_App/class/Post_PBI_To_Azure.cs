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
    public class Post_PBI_To_Azure
    {
        public const string BASE = "https://dev.azure.com";
        string PAT;
        public string ORG;
        public const string API = "bypassRules=true&api-version=6.0";
        public string PROJECT;
        public string WIT_TYPE;
        public string ID_of_PBI { get; set; }

        public Post_PBI_To_Azure(string org, string project, string wit_type)
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.PAT = (string?)data["azureToken"];
            this.ORG = org;
            this.PROJECT = project;
            this.WIT_TYPE = wit_type;
        }

        public void PostPBIToAzure(string jsonToPost, TextBox res)
        {
            HttpClient client = new HttpClient();

            // Set Media Type of Response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Generate base64 encoded authorization header.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", PAT))));

            // Build the URI for creating Work Item.
            string uri = String.Join("?", String.Join("/", BASE, ORG, PROJECT, "_apis/wit/workitems", ("$" + WIT_TYPE)), API);




            // Create Request body in JSON format.
            HttpContent content = new StringContent(jsonToPost, Encoding.UTF8, "application/json-patch+json");

            // Call CreateWIT method.
            string result = CreateWIT(client, uri, content).Result;


            // Pretty print the JSON if result not empty or null.
            if (!String.IsNullOrEmpty(result))
            {
                try
                {
                    dynamic wit = JsonConvert.DeserializeObject<object>(result);
                    Console.WriteLine(JsonConvert.SerializeObject(wit, Formatting.Indented));
                    this.ID_of_PBI = wit["id"].ToString();
                    res.Text += $" PBI migrated ---> OK   {Environment.NewLine}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    res.Text += $" PBI migrated ---> NOPE    Error message : {Environment.NewLine} {result} {Environment.NewLine}";
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
        } // End of CreateWIT method
    }
}
