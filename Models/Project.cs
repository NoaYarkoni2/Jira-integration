namespace JiraIntegration.Models
{
    public class Project
    {
        public string? Expand { get; set; }
        public string? Self { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string? ProjectTypeKey { get; set; }
        public bool? Simplified { get; set; }
        public string? Style { get; set; }
        public bool? IsPrivate { get; set; }
        public Dictionary<string, object>? Properties { get; set; }
        public string? EntityId { get; set; }
        public string? Uuid { get; set; }
    }
}
