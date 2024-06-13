using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models;

[JsonSerializable(typeof(GetTeamPlayersRequest))]
[JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
public partial class GetTeamPlayersRequestContext : JsonSerializerContext
{
}
public class GetTeamPlayersRequest:IAsJson
{
    [Required]
    public int ClubId { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
    public char Level { get; set; } = 'X';

    public string AsJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, GetTeamPlayersRequestContext.Default.GetTeamPlayersRequest);
    }
}
