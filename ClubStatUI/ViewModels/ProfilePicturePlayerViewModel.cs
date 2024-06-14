using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;

using ClubStatUI.Infrastructure;

namespace ClubStatUI.ViewModels
{
    public partial class ProfilePicturePlayerViewModel : ObservableObject, ILoadAsync
    {
        [ObservableProperty]
        private ImageSource? _profileImage;
        private readonly IProfilePictureFactory _pictureFactory;
        private readonly ILoginFactory _loginFactory;

        public ProfilePicturePlayerViewModel(IProfilePictureFactory pictureFactory, ILoginFactory loginFactory)
        {
            _pictureFactory = pictureFactory;
            _loginFactory = loginFactory;
        }




        async Task ILoadAsync.ExecuteAsync()
        {
            if (_loginFactory.CurrentUser is null)
            {
                return;
            }

            var bytes = await _pictureFactory.GetProfilePictureForUserAsync(_loginFactory.CurrentUser.UserId);
            if (bytes.Length > 0)
            {
                var source = ImageSource.FromStream(() => new MemoryStream(bytes));
                ProfileImage = source;
            }
        }

        // Method to upload a new profile picture
        public async Task UploadProfilePictureAsync(Guid userId,FileResult fileResult)
        {
            if (fileResult == null)
                throw new ArgumentNullException(nameof(fileResult));

            using var stream = await fileResult.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();

            await _pictureFactory.UploadPictureForUserAsync(userId,imageBytes);

            
            // Update the profile image after uploading
            var source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            ImageHelper.ReplaceImage(imageBytes,source);
            ProfileImage = source;
        }


    }
}
