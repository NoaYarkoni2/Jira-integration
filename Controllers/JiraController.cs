using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using JiraIntegration.Models;
using JiraIntegration.Services;
using JiraIntegration.Interface;

namespace JiraIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JiraController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IJiraService _jiraService;

        public JiraController(HttpClient httpClient, IJiraService jiraService)
        {
            _httpClient = httpClient;
            _jiraService = jiraService;
        }

        [HttpPost("create-issue")]
        public async Task<IActionResult> CreateIssue([FromBody] CreateIssueRequest request, [FromHeader] string username, [FromHeader] string email, [FromHeader] string apiToken)
        {
            try
            {
                var jiraUrl = $"https://{username}.atlassian.net/rest/api/3/issue/";             
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{email}:{apiToken}"));

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

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
                        issuetype = new { name = request.IssueTypeName }
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(issueData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(jiraUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Ok(result);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, error);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getAllProjects")]
        public async Task<ActionResult<List<Project>>> GetAllProjects([FromHeader] string username, [FromHeader] string email, [FromHeader] string apiToken)
        {
            try
            {
                var projects = await _jiraService.GetAllProjectsAsync(username,email, apiToken);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
