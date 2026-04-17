using Microsoft.AspNetCore.Mvc;

namespace AI_Agent.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AiAgentController : ControllerBase
    {
        private readonly AiAgentService _agent;

        private readonly ILogger<AiAgentController> _logger;

        public AiAgentController(AiAgentService aiAgent)
        {
            _agent = aiAgent;
        }
       

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var result = await _agent.Handle(request.Message);
            return Ok(result);
        }


    }
    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
