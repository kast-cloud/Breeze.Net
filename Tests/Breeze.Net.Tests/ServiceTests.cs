using System;
using System.Linq;
using Xunit;

namespace Breeze.Net.Tests
{
    public class ServiceTests : BreezeTest
    {
        [Fact]
        public void ListPeopleTest()
        {
            var output = service.ListPeople(true, "{\"added_after\":\"2022-03-01\"}");
            Assert.NotEmpty(output);
        }

        [Fact]
        public void ListMalePeopleTest()
        {
            var output = service.ListPeople(false, "{\"445063509\":1}");
            Assert.NotEmpty(output);
        }


        [Fact]
        public void ListProfileFieldsTest()
        {
            var output = service.ListProfileFields();

            Assert.NotEmpty(output);
        }

        [Fact]
        public void ShowPersonTest()
        {
            var people = service.ListPeople(false);
            var output = service.ShowPerson(people.FirstOrDefault().id);

            Assert.NotNull(output);
        }

        [Fact]
        public void ListFoldersTest()
        {
            var output = service.ListFolders();
            Assert.NotNull(output);
        }

        [Fact]
        public void ListTagsTest()
        {
            var output = service.ListTags();
            Assert.NotNull(output);
        }

        [Fact]
        public void ListTagsFromAFolderTest()
        {
            var output = service.ListTags("1657116");
            Assert.NotNull(output);
        }

        [Fact]
        public void ListPeopleWithTagTest()
        {
            var output = service.ListPeopleWithTag(true, "4025664");
            Assert.NotNull(output);
        }

        [Fact]
        public void ListEventsTest()
        {
            var output = service.ListEvents(DateTime.Now.AddDays(6));
            Assert.NotNull(output);
        }

        [Fact]
        public void ListEventTest()
        {
            var output = service.ListEvent("220628184");
            Assert.NotNull(output);
        }

        [Fact]
        public void ListCalendarsTest()
        {
            var output = service.ListCalendars();
            Assert.NotNull(output);
        }

        [Fact]
        public void AddPersonTest()
        {
            var output = service.AddPerson("Scott", "Tiger", "scott@tiger.com", "0412345678", "Male");
            Assert.NotNull(output);
        }
    }
}