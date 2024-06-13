using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClubStat.Infrastructure.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStat.Infrastructure.Models.Tests
{
    [TestClass()]
    public class ProfileImageTests
    {
        [TestMethod()]
        public void ProfileImageTest()
        {
            var id = Guid.Parse("93e14140-d603-ef11-868d-c8e26574d4f6");
            var img = new FileInfo("Resources\\NoProfileImage.png");

            var bytes= img.Exists? File.ReadAllBytes(img.FullName):"Fake Image data".ToBytes();
            var profile = new ProfileImage(userId: id, imageBytes: bytes);
            var json= profile.AsJson();
            Assert.IsFalse(string.IsNullOrEmpty(json));
            var copy = json.IsValidJson(ProfileImageJsonContext.Default.ProfileImage, out var clone);

            Assert.IsNotNull(clone);
            Assert.AreEqual(id, clone.Id);
            Assert.IsTrue(bytes.SequenceEqual(clone.GetImageBytes()));
            Assert.IsTrue(profile.Equals(clone));
        }

    }
}