// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-20-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Thu 23-May-2024
// ***********************************************************************
// <copyright file="ProfileImage.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary>User profile indicator</summary>
// ***********************************************************************

using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models;

[JsonSerializable(typeof(ProfileImage))]
[JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
public partial class ProfileImageJsonContext : JsonSerializerContext
{
}

/// <summary>
/// Class ProfileImage is used to upload images to the rest api.
/// </summary>
public class ProfileImage : IAsJson
{
    public ProfileImage(Guid userId, byte[] imageBytes)
    {
        Image = Convert.ToBase64String(imageBytes);
        Id = userId;
    }

    [JsonConstructor]
    public ProfileImage(Guid id, string? image )
    {
        Id= id;
        Image= image;
    }

    public Guid Id { get; set; }

    public string? Image { get; set; }

    public string AsJson()
    {
       return System.Text.Json.JsonSerializer.Serialize(this, ProfileImageJsonContext.Default.ProfileImage);
    }

    /// <summary>
    /// Gets the image bytes.
    /// </summary>
    /// <returns>byte[].</returns>
    public byte[] GetImageBytes()=> Image?.ToBytes() ?? [];
}
