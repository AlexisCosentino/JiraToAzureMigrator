using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Migration_Desktop_App
{
    class Post_Worklog_To_TimeTracker
    {
        string token;
        string uri = "https://iriumsoftware.timehub.7pace.com/api/rest/workLogs?api-version=3.2";
        public Post_Worklog_To_TimeTracker()
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.token = (string?)data["timeTrackerToken"];
        }

        public void PostJsonToTimeTracker(string json)
        {
            HttpClient client = new HttpClient();

            // Set Media Type of Response.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Generate base64 encoded authorization header.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", token))));

            // Create Request body in JSON format.
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            // Call CreateWIT method.
            string result = Post(client, this.uri, content).Result;


            // Pretty print the JSON if result not empty or null.
            if (!String.IsNullOrEmpty(result))
            {
                try
                {
                    dynamic wit = JsonConvert.DeserializeObject<object>(result);
                    Console.WriteLine(JsonConvert.SerializeObject(wit, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            client.Dispose();
        }

        public async Task<string> Post(HttpClient client, string uri, HttpContent content)
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
