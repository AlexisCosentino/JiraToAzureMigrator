﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Migration_Desktop_App
{
    public class DB_Connection  
    {
        string username;
        string dbname;
        string hostname;
        string password;
        public string query { get; set; }


        public Dictionary<string, Dictionary<string, string>> getDictOfPBI()
        {
            get_credentials();
            var queryAnswerDict = new Dictionary<string, Dictionary<string, string>>();
            SqlConnection conn = new SqlConnection($"Server={hostname};Database={dbname};User Id={username};Password={password};");
            try
            {
                SqlCommand command = new SqlCommand(this.query, conn);

                conn.Open();
                Console.WriteLine("We are in bro");

                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        var value_dict = new Dictionary<string, string>();
                        var ID = reader[0].ToString();
                        value_dict.Add("issueNb", reader[0].ToString());
                        value_dict.Add("project", reader[1].ToString());
                        value_dict.Add("reporter", reader[2].ToString());
                        value_dict.Add("assignee", reader[3].ToString());
                        value_dict.Add("creator", reader[4].ToString());
                        value_dict.Add("summary", reader[5].ToString());
                        value_dict.Add("description", reader[6].ToString());
                        value_dict.Add("created", reader[7].ToString());
                        value_dict.Add("updated", reader[8].ToString());
                        value_dict.Add("dueDate", reader[9].ToString());
                        value_dict.Add("ProjectName", reader[10].ToString());
                        value_dict.Add("issueType", reader[11].ToString());
                        value_dict.Add("issueStatus", reader[12].ToString());
                        value_dict.Add("priority", reader[13].ToString());
                        value_dict.Add("componentList", reader[15].ToString());
                        value_dict.Add("fixedVersionList", reader[16].ToString());
                        value_dict.Add("labelsList", reader[17].ToString());
                        value_dict.Add("sprintList", reader[18].ToString());
                        value_dict.Add("startDate", reader[19].ToString());
                        value_dict.Add("endDate", reader[20].ToString());
                        value_dict.Add("workLog", reader[21].ToString());
                        value_dict.Add("totalWorkTime", reader[22].ToString());
                        value_dict.Add("linkToJira", reader[23].ToString());
                        value_dict.Add("worklog2", reader[24].ToString());
                        value_dict.Add("projectCategoryType", reader[25].ToString());
                        value_dict.Add("service", reader[26].ToString());
                        value_dict.Add("originalEstimate", reader[27].ToString());
                        value_dict.Add("linkChild", reader[28].ToString());
                        value_dict.Add("linkParent", reader[29].ToString());


                        queryAnswerDict.Add(ID, value_dict);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    reader.Close();
                    Console.WriteLine(JsonConvert.SerializeObject(queryAnswerDict).ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return queryAnswerDict;
        }

        public Dictionary<string, Dictionary<string, string>> getDictOfComments()
        {
            get_credentials();
            var queryAnswerDict = new Dictionary<string, Dictionary<string, string>>();
            SqlConnection conn = new SqlConnection($"Server={hostname};Database={dbname};User Id={username};Password={password};");
            try
            {
                SqlCommand command = new SqlCommand(this.query, conn);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        var value_dict = new Dictionary<string, string>();
                        var ID = reader[4].ToString();
                        value_dict.Add("issueID", reader[0].ToString());
                        value_dict.Add("author", reader[1].ToString());
                        value_dict.Add("comment", reader[2].ToString());
                        value_dict.Add("created_date", reader[3].ToString());
                        queryAnswerDict.Add(ID, value_dict);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close();
                Console.WriteLine(JsonConvert.SerializeObject(queryAnswerDict).ToString());

            }
            return queryAnswerDict;
        }

        public List<string> getListOfAttachments()
        {
            get_credentials();
            List<string> queryAnswerList = new List<string>();
            SqlConnection conn = new SqlConnection($"Server={hostname};Database={dbname};User Id={username};Password={password};");
            try
            {
                SqlCommand command = new SqlCommand(this.query, conn);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        var attachment_link = $"https://worklog.vega-systems.com/secure/attachment/{reader[0]}/{reader[2]}";
                        Console.WriteLine(attachment_link);
                        queryAnswerList.Add(attachment_link);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return queryAnswerList;
        }

        public List<List<string>> getListOfSprint()
        {
            get_credentials();
            List<List<string>> sprintList = new List<List<string>>();
            SqlConnection conn = new SqlConnection($"Server={hostname};Database={dbname};User Id={username};Password={password};");
            try
            {
                SqlCommand command = new SqlCommand(this.query, conn);

                conn.Open();
                SqlDataReader reader = command.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        List<string> sprint = new List<string>();
                        sprint.Add(reader[0].ToString());
                        sprint.Add(reader[1].ToString());
                        sprint.Add(reader[2].ToString());
                        sprintList.Add(sprint);
                        Console.WriteLine(sprintList);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close();
            }
            return sprintList;
        }

        private void get_credentials()
        {
            JObject data = JObject.Parse(File.ReadAllText("data.json"));
            this.password = (string?)data["db_pwd"];
            this.username = (string?)data["db_username"];
            this.dbname = (string?)data["db_name"];
            this.hostname = (string?)data["db_hostname"];
        }
    }
}
