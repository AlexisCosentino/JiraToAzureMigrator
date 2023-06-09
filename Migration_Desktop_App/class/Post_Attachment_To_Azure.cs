﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Security.Policy;

namespace Migration_Desktop_App
{
    public class Post_Attachment_To_Azure
    {
        public const string BASE = "https://dev.azure.com";
        string PAT;
        string ID;
        public string ORG;
        public const string API = "api-version=6.0";
        public string PROJECT;


        public Post_Attachment_To_Azure(string ID, string org, string project)
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.PAT = (string?)data["azureToken"];
            this.ID = ID;
            this.ORG = org;
            this.PROJECT = project;
        }


        public string PatchAttachmentToAzureServer(string linkToPost, string filename)
        {
            //var url = $"https://dev.azure.com/IRIUMSOFTWARE/TEST_ALEXIS/_apis/wit/attachments?fileName=logo.png&api-version=6.0";

            string url = String.Join("?", String.Join("/", BASE, ORG, PROJECT, "_apis/wit/attachments"),"fileName="+ filename + "&" + API);

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", get_credentials("azureToken")))));

            WebClient wc = new WebClient();

            wc.Headers.Add("Authorization", "Basic " + GetEncodedCredentials());

            try
            {
                byte[] byteData = wc.DownloadData(linkToPost);
                dynamic att_url = postImgToAzure(byteData, client, url).Result;
                att_url = JsonConvert.DeserializeObject<object>(att_url);
                return att_url["url"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR IMAGE------------------------------------------------- {ex.ToString()} ------------------");
                return "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f7/Generic_error_message.png/220px-Generic_error_message.png";
            }

        }

        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", get_credentials("jira_username"), get_credentials("jira_pwd"));
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }

        private string get_credentials(string key)
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            return (string?)data[key];
        }

        public async Task<string> postImgToAzure(byte[] byteData, HttpClient client, string url)
        {
            try
            {
                // Send asynchronous POST request.
                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    HttpResponseMessage response =  client.PostAsync(url, content).Result;
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
