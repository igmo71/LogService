using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Dynamic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LogService.Controllers
{
    [Route("/")]
    [ApiController]
    public class LogController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] JsonElement inputJsonObject)
        {
            var inputDynamic = JsonSerializer.Deserialize<ExpandoObject>(inputJsonObject);

            string Message = inputJsonObject.ToString();
            string Solution = string.Empty;
            string Application = string.Empty;
            string Environment = string.Empty;
            string Level = string.Empty;

            if (inputDynamic != null)
            {
                if ((inputDynamic as IDictionary<string, object>).ContainsKey("Solution"))
                    Solution = (inputDynamic as dynamic).Solution.ToString();

                if ((inputDynamic as IDictionary<string, object>).ContainsKey("Application"))
                    Application = (inputDynamic as dynamic).Application.ToString();

                if ((inputDynamic as IDictionary<string, object>).ContainsKey("Environment"))
                    Environment = (inputDynamic as dynamic).Environment.ToString();

                if ((inputDynamic as IDictionary<string, object>).ContainsKey("Level"))
                    Level = (inputDynamic as dynamic).Level.ToString();

                if ((inputDynamic as IDictionary<string, object>).ContainsKey("Message"))
                {
                    var options = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };
                    Message = JsonSerializer.Serialize((inputDynamic as dynamic).Message, options);
                }
            }

            switch (Level)
            {
                case "Error":
                    Log.Error("{Solution} {Application} {Environment} {Message}", Solution, Application, Environment, Message);
                    break;
                case "Warning":
                    Log.Warning("{Solution} {Application} {Environment} {Message}", Solution, Application, Environment, Message);
                    break;
                case "Information":
                    Log.Information("{Solution} {Application} {Environment} {Message}", Solution, Application, Environment, Message);
                    break;
                case "Debug":
                    Log.Debug("{Solution} {Application} {Environment} {Message}", Solution, Application, Environment, Message);
                    break;
                default:
                    Log.Information("{Solution} {Application} {Environment} {Message}", Solution, Application, Environment, Message);
                    break;
            }

            return Ok();
        }
    }
}
