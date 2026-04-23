namespace AI_Agent.Dtos
{
    public class IssueAircraftDto
    {
        // Required fields
        public int weight { get; set; }
        public int numberOfEngines { get; set; }
        // Optional field
        public int? buildnumber { get; set; } 
        // Example: add another optional field
        public string? notes { get; set; } 
    }
}
