// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-11-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : 02-11-2024
// ***********************************************************************
// <copyright file="Player.cs" company="ClubStat.Infrastructure">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// Task 9: Add player endpoint and allow it to retrieve player data. update the Payer class to reflect figma property
// </summary>
// ***********************************************************************
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models;
/// <summary>
/// Class ClubMember contain the image for the member.
/// Implements the <see cref="ClubStat.Infrastructure.Models.LoggedInUser" />
/// </summary>
/// <remarks>Can be a player,coach or any other person type to use the system</remarks>
/// <seealso cref="ClubStat.Infrastructure.Models.LoggedInUser" />
public abstract class ClubMember : LoggedInUser
{
    public static byte[] _noProfileImage = [];

    byte[] _profileImageBytes = _noProfileImage;
    int _clubId;
    static ClubMember()
    {
        var assembly = typeof(ClubMember).Assembly;
        var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First(f => f.EndsWith("NoProfileImage.png", StringComparison.Ordinal)));
        if (stream is not null)
        {
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            _noProfileImage = memory.ToArray();
        }
    }
    [JsonIgnore]
    public byte[] ProfileImageBytes
    {
        get => _profileImageBytes;
        set => SetProperty(ref  _profileImageBytes , value);
    }


    public int ClubId { get => _clubId; set => SetProperty(ref _clubId, value); }

}
