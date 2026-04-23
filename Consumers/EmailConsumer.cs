using AI_Agent.Events;
using MassTransit;

namespace AI_Agent.Consumers
{
    public class EmailConsumer : IConsumer<EmailRequestedEvent>
    {
        public async Task Consume(ConsumeContext<EmailRequestedEvent> context)
        {
            var evt = context.Message;

            Console.WriteLine("📧 Sending Email...");
            Console.WriteLine($"To: {evt.To}");
            Console.WriteLine($"Subject: {evt.Subject}");
            Console.WriteLine($"Body: {evt.Body}");

            // هنا تحط SMTP / SendGrid
            await Task.CompletedTask;
        }
    }
}
