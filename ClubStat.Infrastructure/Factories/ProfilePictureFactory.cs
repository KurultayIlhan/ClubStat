using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStat.Infrastructure.Factories
{
    public interface IProfilePictureFactory
    {
        Task<byte[]> GetProfilePictureForUserAsync(Guid userId);
         Task<byte[]> GetProfilePictureForCurrentUserAsync();
        Task UploadPictureForUser(Guid userId, byte[] pictureData);
    }
    internal class ProfilePictureFactory:ApiBasedFactory, IProfilePictureFactory
    {
        private readonly ILoginFactory _loginFactory;

        public ProfilePictureFactory(ILoginFactory loginFactory, IConfiguration configuration, IHttpClientFactory clientFactory)
        : base(configuration, clientFactory)
        {
            _loginFactory = loginFactory;
        }

        public Task<byte[]> GetProfilePictureForCurrentUserAsync()
        {
            return GetProfilePictureForUserAsync(_loginFactory.CurrentUser?.UserId??Guid.Empty);
        
        }
        public async Task<byte[]> GetProfilePictureForUserAsync(Guid userId)
        {
            var bytes=  await base.GetBytesAsync(MagicStrings.PlayerProfileUrl(userId)).ConfigureAwait(false);
            //should be more than 0 bytes
            return bytes;
        }

        public async Task UploadPictureForUser(Guid userId, byte[] pictureData)
        {
           var model = new ProfileImage(userId, pictureData);
           await base.WriteDataAsync(MagicStrings.PlayerPostPicture, model);
        }

        byte[]? LoadPictureFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return System.IO.File.ReadAllBytes(filePath);
            }
            return null;
        }

    }
}
