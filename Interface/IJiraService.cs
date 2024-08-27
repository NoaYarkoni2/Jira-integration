using JiraIntegration.Models;

namespace JiraIntegration.Interface
{
    public interface IJiraService
    {
        Task<string> CreateJiraIssueAsync(string username, string email, string apiToken, CreateIssueRequest request);
        Task<List<Project>> GetAllProjectsAsync(string username, string email, string apiToken);
    }
}
