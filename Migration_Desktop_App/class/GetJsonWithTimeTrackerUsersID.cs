using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Migration_Desktop_App
{
    class GetJsonWithTimeTrackerUsersID
    {
        string token;
        Dictionary<string, string> dictOfNameAndID = new Dictionary<string, string>();

        public GetJsonWithTimeTrackerUsersID()
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.token = (string?)data["timeTrackerToken"];
        }

        public Dictionary<string, string> getRequest()
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", this.token))));

            string uri = "https://iriumsoftware.timehub.7pace.com/api/rest/users/roles?api-version=3.2&$expand=user.displayName";

            string result = getJsonOfUser(client, uri).Result;

            // Pretty print the JSON if result not empty or null.
            if (!String.IsNullOrEmpty(result))
            {
                dynamic json = JsonConvert.DeserializeObject<object>(result);
                foreach (dynamic item in json["data"])
                {
                    dictOfNameAndID.Add(item["uniqueName"].ToString(), item["id"].ToString());
                }
            }
            client.Dispose();
            return dictOfNameAndID;
/*            foreach(KeyValuePair<string, string> kvp in dictOfNameAndID)
            {
                Console.WriteLine(kvp.Key + "=" + kvp.Value);  
            }
*/

        }

        public async Task<string> getJsonOfUser(HttpClient client, string uri)
        {
            try
            {
                // Send asynchronous POST request.
                using (HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false))
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
