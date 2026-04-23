using AI_Agent.Dtos;
using AI_Agent.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AI_Agent.Controllers
{
    [ApiController]
    [Route("mcp/tools")]
    public class McpToolsController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        private readonly ILogger<McpToolsController> _logger;

        public McpToolsController(ILogger<McpToolsController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] ToolExecutionRequest request)
        {
            switch (request.ToolName)
            {
                case "issue_aircraft":
                    var dto = JsonConvert.DeserializeObject<IssueAircraftDto>(request.Arguments.ToString());
                    //var result = await _aircraftService.IssueAircraft(dto);
                    // simulate success
                    var emailEvent = new EmailRequestedEvent
                    {
                        To = "user@email.com",
                        Subject = "Aircraft Issued ✈️",
                        Body = $"Aircraft issued with weight {dto.weight}"
                    };

                    await _publishEndpoint.Publish(new EmailRequestedEvent
                    {
                        To = "user@email.com",
                        Subject = "Aircraft Issued ✈️",
                        Body = "Aircraft issued successfully"
                    });
                    return Ok("aircraftool");

                case "get_customer_profile":
                    //var profile = await _customerService.GetMyProfile();
                    return Ok("customerprofiletool");

                case "create_ticket":
                    //var profile = await _customerService.GetMyProfile();
                    return Ok("tickettool");

                default:
                    return BadRequest("Unknown tool");
            }
        }

        public class ToolExecutionRequest
        {
            public string ToolName { get; set; }
            public object Arguments { get; set; }
        }

        // Use shared DTO from Dtos/IssueAircraftDto.cs

        
    }
}
