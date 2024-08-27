using JiraIntegration.Interface;
using JiraIntegration.Models;
using Newtonsoft.Json;
using System.Text;

namespace JiraIntegration.Services
{
    public class JiraService : IJiraService
    {
        private readonly HttpClient _httpClient;

        public JiraService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> CreateJiraIssueAsync(string username, string email, string apiToken, CreateIssueRequest request)
        {
            var client = new HttpClient();
            var jiraUrl = $"https://{username}.atlassian.net/rest/api/3/issue/";         
            var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiToken}"));

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var issueData = new
            {
                fields = new
                {
                    project = new { id = request.ProjectId },
                    summary = request.Summary,
                    description = new
                    {
                        type = "doc",
                        version = 1,
                        content = new[]
                        {
                        new
                        {
                            type = "paragraph",
                            content = new[]
                            {
                                new { type = "text", text = request.Description }
                            }
                        }
                    }
                    },
                    issuetype = new { id = request.IssueTypeName }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(issueData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(jiraUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }


        public async Task<List<Project>> GetAllProjectsAsync(string username, string email, string apiToken)
        {
            var jiraUrl = $"https://{username}.atlassian.net/rest/api/3/project";         
            var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiToken}"));

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(jiraUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var projects = JsonConvert.DeserializeObject<List<Project>>(jsonResponse);
                return projects;
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
