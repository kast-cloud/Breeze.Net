using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Breeze.Net.Tests
{
    [TestClass()]
    public class ServiceTests
    {
        
        BreezeService service = new BreezeService("", "");

        [TestMethod()]
        public void ListPeopleTest()
        {
            var output = service.ListPeople(true);
            Assert.IsTrue(output != null && output.Any());
        }

        [TestMethod()]
        public void ListMalePeopleTest()
        {
            var output = service.ListPeople(true, "{\"445063509\":1}");
            Assert.IsTrue(output != null && output.Any());
        }


        [TestMethod()]
        public void ListProfileFieldsTest()
        {
            var output = service.ListProfileFields();

            Assert.IsTrue(output != null && output.Any());
        }

        [TestMethod()]
        public void ShowPersonTest()
        {
            var output = service.ShowPerson("34193296");

            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void ListFoldersTest()
        {
            var output = service.ListFolders();
            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void ListTagsTest()
        {
            var output = service.ListTags();
            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void ListTagsFromAFolderTest()
        {
            var output = service.ListTags("1657116");
            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void ListPeopleWithTagTest()
        {
            var output = service.ListPeopleWithTag(true, "4025664");
            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void ListEventsTest()
        {
            var output = service.ListEvents(DateTime.Now.AddDays(6));
            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void ListEventTest()
        {
            var output = service.ListEvent("220628184");
            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void ListCalendarsTest()
        {
            var output = service.ListCalendars();
            Assert.IsTrue(output != null);
        }

        [TestMethod()]
        public void AddPersonTest()
        {
            var output = service.AddPerson("Scott", "Tiger", "scott@tiger.com", "0412345678", "Male");
            Assert.IsTrue(output != null);
        }
    }
}