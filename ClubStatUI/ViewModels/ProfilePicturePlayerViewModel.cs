using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;

namespace ClubStatUI.ViewModels
{
    public partial class ProfilePicturePlayerViewModel : ObservableObject, ILoadAsync
    {
        [ObservableProperty]
        private ImageSource? _profileImage;
        private readonly IProfilePictureFactory _pictureFactory;

        public ProfilePicturePlayerViewModel(IProfilePictureFactory pictureFactory)
        {
            _pictureFactory = pictureFactory;
        }




        async Task ILoadAsync.ExecuteAsync()
        {
            var bytes = await _pictureFactory.GetProfilePictureForCurrentUserAsync();
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

            await _pictureFactory.UploadPictureForUser(userId,imageBytes);

            // Update the profile image after uploading
            var source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            ProfileImage = source;
        }


    }
}
