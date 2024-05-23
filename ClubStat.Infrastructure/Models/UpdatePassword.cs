// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sun 12-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="UpdatePassword.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary>
// Task 10: Have the abbility to reset password
// </summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{

    [JsonSerializable(typeof(UpdatePassword))]
    [JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
        )]
    public partial class UpdatePasswordJsonContext : JsonSerializerContext
    {
    }

    public partial class UpdatePassword : ObservableValidator, IAsJson
    {

        [Required(AllowEmptyStrings = false)]
        [ObservableProperty]
        string _currentPassword = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [ObservableProperty]
        string _newPassword = string.Empty;

        [Required(AllowEmptyStrings = false)]
        [ObservableProperty]
        string? _userName = string.Empty;

        [Required]
        [ObservableProperty]
        Guid? _userId;

        [JsonConstructor]
        public UpdatePassword()
        {
            
        }
        
        public UpdatePassword(IAuthenticatedUser user)
        {
            _userId = user.UserId;
            _userName = user.FullName;
        }

        public string AsJson()
        {
          return System.Text.Json.JsonSerializer.Serialize(this, UpdatePasswordJsonContext.Default.UpdatePassword);   
        }
    }
}
