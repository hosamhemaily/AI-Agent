using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using AI_Agent.Dtos;
using System.Reflection;

namespace AI_Agent
{
    public class AiAgentService
    {
        private readonly HttpClient _http;

        public AiAgentService(HttpClient http)
        {
            _http = http;
        }

        public async Task<object> Handle(string userInput)
        {
            var aiResponse = await CallOllama(userInput);

            // 🧠 لو فيه tool call
            if (aiResponse.ToolName != null)
            {
                // Only handle issue_aircraft for now
                
                var toolResult = await ExecuteTool(aiResponse);
                return new { reply = $"✅ Done: {toolResult}" };
            }

            return new { reply = aiResponse.Text };
        }
        private async Task<string> ExecuteTool(AiResponse ai)
        {
            var response = await _http.PostAsJsonAsync(
                "https://localhost:7033/mcp/tools/execute",
                new
                {
                    toolName = ai.ToolName,
                    arguments = ai.Arguments
                });

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        private async Task<AiResponse> CallOllama(string input)
        {
            var requestBody = new
            {
                model = "llama3.1:8b",
                prompt = BuildPrompt(input),
                stream = false
            };

            var response = await _http.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                requestBody
            );

            var json = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            var cleaned = ExtractJson(json.response);

            return ParseAiResponse(cleaned);
        }
        private AiResponse ParseAiResponse(string text)
        {
            try
            {
                // نحاول نفهم JSON
                var json = JsonSerializer.Deserialize<AiResponse>(text,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return json;
            }
            catch
            {
                // fallback لو رجع نص عادي
                return new AiResponse
                {
                    Text = text
                };
            }
        }

        private string ExtractJson(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // remove ```json ``` or ``` blocks
            var start = text.IndexOf('{');
            var end = text.LastIndexOf('}');

            if (start >= 0 && end > start)
            {
                return text.Substring(start, end - start + 1);
            }

            return text;
        }
        private string BuildPrompt(string userInput)
        {
            // Dynamically determine required and optional fields from IssueAircraftDto
            var requiredFields = new List<string>();
            var optionalFields = new List<string>();
            var props = typeof(IssueAircraftDto).GetProperties();
            foreach (var prop in props)
            {
                if (!IsNullable(prop))
                    requiredFields.Add(prop.Name);
                else
                    optionalFields.Add(prop.Name);
            }
            var requiredFieldsStr = string.Join(", ", requiredFields);
            var optionalFieldsStr = string.Join(", ", optionalFields);
            var allFieldsStr = string.Join(", ", props.Select(p => p.Name));

            return $@"
You are an AI agent.

You can decide to call a tool.

Available tools:
1. issue_aircraft
2. get_customer_profile

-------------------------------------

Tool: issue_aircraft

Fields:
- NumberOfEngines (can be described as: engines count, number of engines, engines)
- Weight (can be described as: weight, mass)

Required fields: [{{requiredFieldsStr}}]

-------------------------------------

Tool: get_customer_profile

Description:
- Retrieves current logged-in customer profile information.

Fields:
- No input parameters required.

-------------------------------------

Rules:

- NEVER assume or guess values that the user did not explicitly provide.
- NEVER invent default values.
- NEVER return null for a field if the user provided a value.
- Map user words to field names (e.g. ""2 engines"" → NumberOfEngines = 2)

- If user asks about:
  - their profile
  - their data
  - their info
  → call get_customer_profile

- If user wants to issue aircraft:
  - check required fields first
  - If any required field is missing → DO NOT call tool, return text asking for missing fields
  - If all required fields are present → call issue_aircraft

-------------------------------------

Response format:

- If calling issue_aircraft:
{{
  ""toolName"": ""issue_aircraft"",
  ""arguments"": {{ ... }}
}}

- If calling get_customer_profile:
{{
  ""toolName"": ""get_customer_profile"",
  ""arguments"": {{}}
}}

- Otherwise:
{{
  ""text"": ""your response""
}}

-------------------------------------

User request:
{userInput}
";
        }

        // Helper to check if property is nullable
        private static bool IsNullable(PropertyInfo prop)
        {
            if (!prop.PropertyType.IsValueType) return true; // reference type
            if (Nullable.GetUnderlyingType(prop.PropertyType) != null) return true; // Nullable<T>
            return false;
        }
    }

    public class AiResponse
    {
        public string? ToolName { get; set; }
        public object? Arguments { get; set; }
        public string? Text { get; set; }
    }
    public class OllamaResponse
    {
        public string response { get; set; }
    }
}
