using Microsoft.Extensions.Caching.Memory;
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

        Task UploadPictureForUserAsync(Guid userId, byte[] pictureData);
    }
    internal class ProfilePictureFactory : ApiBasedFactory, IProfilePictureFactory
    {
          private readonly IMemoryCache _memory;



        public ProfilePictureFactory( IConfiguration configuration
            , IHttpClientFactory clientFactory, IMemoryCache memory)
        : base(configuration, clientFactory)
        {
            _memory = memory;

        }


        public async Task<byte[]> GetProfilePictureForUserAsync(Guid userId)
        {
            var url = MagicStrings.PlayerProfileUrl(userId);
            if (_memory.TryGetValue<byte[]>(url, out var bytes) && bytes is not null && bytes.Length > 0)
                return bytes;
            try
            {
                bytes = await base.GetBytesAsync(url).ConfigureAwait(true);
                if (bytes.Length > 0)
                {
                    _memory.Set(url, bytes);
                }
                //should be more than 0 bytes
                return bytes;
            }
            catch (Exception ex) 
            {
                Walter.Inverse.GetLogger("ProfilePictureFactory")?.LogException(ex);
            }
            return [];
        }

        public async Task UploadPictureForUserAsync(Guid userId, byte[] pictureData)
        {
            //remember the picture
            var cashKey = MagicStrings.PlayerProfileUrl(userId);
            _memory.Set(cashKey, pictureData);

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
