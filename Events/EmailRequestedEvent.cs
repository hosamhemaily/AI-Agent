namespace AI_Agent.Events
{
    public class EmailRequestedEvent
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
