using Microsoft.AspNetCore.Mvc;
using AI_Agent.Dtos;
using Newtonsoft.Json;

namespace AI_Agent.Controllers
{
    [ApiController]
    [Route("mcp/tools")]
    public class McpToolsController : ControllerBase
    {
        private readonly ILogger<McpToolsController> _logger;

        public McpToolsController(ILogger<McpToolsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] ToolExecutionRequest request)
        {
            switch (request.ToolName)
            {
                case "issue_aircraft":
                    var dto = JsonConvert.DeserializeObject<IssueAircraftDto>(request.Arguments.ToString());
                    //var result = await _aircraftService.IssueAircraft(dto);
                    return Ok("aircraftool");

                case "get_customer_profile":
                    //var profile = await _customerService.GetMyProfile();
                    return Ok("customerprofiletool");

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
