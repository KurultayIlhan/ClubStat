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

using System.Diagnostics;
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

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class ProfileImage : IAsJson, IEquatable<ProfileImage?>
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

    override public string ToString()
    {
        return $"Id: {Id}, Image: {Image?.Length??0} bytes";
    }
    public string AsJson()
    {
       return System.Text.Json.JsonSerializer.Serialize(this, ProfileImageJsonContext.Default.ProfileImage);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ProfileImage);
    }

    public bool Equals(ProfileImage? other)
    {
        return other is not null &&
               Id.Equals(other.Id) &&
               Image == other.Image;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Image);
    }

    /// <summary>
    /// Gets the image bytes.
    /// </summary>
    /// <returns>byte[].</returns>
    public byte[] GetImageBytes()
    {
        if (Image == null) return [];

        return Convert.FromBase64String(Image);
    }

    public static bool operator ==(ProfileImage? left, ProfileImage? right)
    {
        return EqualityComparer<ProfileImage>.Default.Equals(left, right);
    }

    public static bool operator !=(ProfileImage? left, ProfileImage? right)
    {
        return !(left == right);
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
