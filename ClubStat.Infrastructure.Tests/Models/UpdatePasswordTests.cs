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
    public class UpdatePasswordTests
    {
        [TestMethod()]
        public void UpdatePasswordTest()
        {
            var upd = new UpdatePassword() { CurrentPassword = "current", NewPassword = "new1234", UserName = "userName" , UserId=Guid.NewGuid()};

            var json= upd.AsJson();
            Assert.IsFalse(String.IsNullOrEmpty(json));
            var copy = json.IsValidJson(UpdatePasswordJsonContext.Default.UpdatePassword, out var clone);
            Assert.IsTrue(copy);
            Assert.IsNotNull(clone);
            Assert.AreEqual(upd.UserName,clone.UserName);
            Assert.AreEqual(upd.NewPassword,clone.NewPassword);
            Assert.AreEqual(upd.CurrentPassword,clone.CurrentPassword);
            Assert.AreEqual(upd.UserId,clone.UserId);
            Assert.IsTrue(upd.Equals(clone));
        }


    }
}