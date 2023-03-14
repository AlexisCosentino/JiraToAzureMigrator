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
    public class Patch_Relation_To_PBI
    {
        string PAT;
        string ID_PBI;
        string ID_of_Relation;

        public Patch_Relation_To_PBI(string pbi_id, string relation_id)
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.PAT = (string?)data["azureToken"];
            this.ID_PBI = pbi_id;
            this.ID_of_Relation = relation_id;
        }

        public void patchRelationToAzure()
        {
            HttpClient client = new HttpClient();

            // Set Media Type of Response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Generate base64 encoded authorization header.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", PAT))));

            // Build the URI for creating Work Item.
            string uri = $"https://dev.azure.com/IRIUMSOFTWARE/_apis/wit/workitems/{ID_PBI}?api-version=7.0";
            string json = "[{\"op\": \"add\", \"path\": \"/relations/-\", \"value\": {\"rel\": \"System.LinkTypes.Related\",  \"url\": \"https://dev.azure.com/IRIUMSOFTWARE/_apis/wit/workItems/"+ this.ID_of_Relation +"\"}}]";

            Console.WriteLine(json);

            // Create Request body in JSON format.
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

            // Call CreateWIT method.
            string result = Update(client, uri, content).Result;

            // Pretty print the JSON if result not empty or null.
            if (!String.IsNullOrEmpty(result))
            {
                dynamic wit = JsonConvert.DeserializeObject<object>(result);
                Console.WriteLine(JsonConvert.SerializeObject(wit, Formatting.Indented));
            }
            client.Dispose();
        }


        public async Task<string> Update(HttpClient client, string uri, HttpContent content)
        {
            try
            {
                // Send asynchronous POST request.
                using (HttpResponseMessage response = await client.PatchAsync(uri, content).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return (await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }
    }
}
