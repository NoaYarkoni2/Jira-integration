namespace JiraIntegration.Models
{
    public class CreateIssueRequest
    {
        public string ProjectId { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string IssueTypeName { get; set; }
    }
}
