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

    public sealed class UpdatePassword : ObservableValidator, IAsJson, IEquatable<UpdatePassword?>
    {

        [Required(AllowEmptyStrings = false)]        
        string _currentPassword = string.Empty;

        [Required(AllowEmptyStrings = false)]        
        string _newPassword = string.Empty;

        [Required(AllowEmptyStrings = false)]        
        string? _userName = string.Empty;

        [Required]        
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
        
        [Required(AllowEmptyStrings =false)]
        public string CurrentPassword { get => _currentPassword; set => SetProperty(ref _currentPassword , value); }
        
        [Required(AllowEmptyStrings =false)]
        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword , value); }
        
        [Required(AllowEmptyStrings =false)]
        public string? UserName { get => _userName; set => SetProperty(ref _userName , value); }
        
        [Required()]
        public Guid? UserId { get => _userId; set => SetProperty(ref _userId , value); }

        public override string ToString()
        {
            return $"new password for {UserName}";
        }

        public string AsJson()
        {
          return System.Text.Json.JsonSerializer.Serialize(this, UpdatePasswordJsonContext.Default.UpdatePassword);   
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as UpdatePassword);
        }

        public bool Equals(UpdatePassword? other)
        {
            return other is not null &&
                   _currentPassword == other._currentPassword &&
                   _newPassword == other._newPassword &&
                   _userName == other._userName &&
                   EqualityComparer<Guid?>.Default.Equals(_userId, other._userId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_currentPassword, _newPassword, _userName, _userId);
        }

        public static bool operator ==(UpdatePassword? left, UpdatePassword? right)
        {
            return EqualityComparer<UpdatePassword>.Default.Equals(left, right);
        }

        public static bool operator !=(UpdatePassword? left, UpdatePassword? right)
        {
            return !(left == right);
        }
    }
}
