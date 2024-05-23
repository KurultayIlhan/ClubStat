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
    public class ClubIconTests
    {
        [TestMethod()]
        public void ClubIconTest()
        {
            var clubIcon = new ClubIcon("Kvzuun B", "path://...;application/");
            Assert.IsNotNull(clubIcon);
            Assert.AreNotEqual(string.Empty, clubIcon.ClubName);
        }
    }
}