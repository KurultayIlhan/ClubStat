
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(LogInUser))]
    [JsonSourceGenerationOptions(
                    GenerationMode = JsonSourceGenerationMode.Metadata,
                    Converters = [typeof(GDPRObfuscatedStringConverter)],
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
        )]
    public partial class LogInUserJsonContext : JsonSerializerContext
    {
}



    public class LogInUser:IAsJson
    {
        [JsonConverter(typeof(GDPRObfuscatedStringConverter))]
        public required string Username { get; set; }

        [JsonConverter(typeof(GDPRObfuscatedStringConverter))]
        // Alleen versleutelde passwoorden 
        public required string Password { get; set; }

        public string AsJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, LogInUserJsonContext.Default.LogInUser);
        }
    }


    [JsonSerializable(typeof(LogInResult))]
    [JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
        )]
    public partial class LogInResultJsonContext : JsonSerializerContext
    {
    }

    public class LogInResult
    {
        public Guid UserId { get; set; }
        public UserType UserType { get; set; } = UserType.None;
    }
}
