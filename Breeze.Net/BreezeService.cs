using System;
using System.Collections.Generic;
using System.Linq;
using Breeze.Net.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using static Breeze.Net.Entities.Enums;

namespace Breeze.Net
{
    public class BreezeService
    {
        string apiUrl;
        string apiKey;
        int recordsPerPage = 500;
        const string DATE_FORMAT = "yyyy-MM-dd";
        static List<Profile> profileFields;
        public BreezeService(string subdomain, string apiKey)
        {
            apiUrl = $"https://{subdomain}.breezechms.com/api/";
            this.apiKey = apiKey;
            if (profileFields == null)
            {
                ListProfileFields();
            }
        }

        private string SendRequest(string endpoint, Method method, Dictionary<string, string> queryStrings)
        {
            string url = $"{apiUrl}{endpoint}";
            var client = new RestClient(url);

            var request = new RestRequest(method);
            if (queryStrings != null && queryStrings.Count > 0)
            {
                foreach (var item in queryStrings)
                {
                    request.AddQueryParameter(item.Key, item.Value);
                }
            }

            request.AddHeader("api-key", apiKey);

            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        public List<Person> ListPeople(bool includeDetail, string filter = "")
        {
            int offset = 0;
            bool queryMore = true;
            List<Person> output = new List<Person>();
            Dictionary<string, string> queryStrings = new Dictionary<string, string>();
            queryStrings.Add("details", (includeDetail ? "1" : "0"));
            queryStrings.Add("limit", recordsPerPage.ToString());
            queryStrings.Add("offset", offset.ToString());
            if (!string.IsNullOrEmpty(filter))
            {
                try
                {
                    var tmpObj = JToken.Parse(filter);
                    queryStrings.Add("filter_json", filter);
                }
                catch
                {
                }
            }

            while (queryMore)
            {
                var responseText = SendRequest("people", Method.GET, queryStrings);

                List<Person> peoples = JsonConvert.DeserializeObject<List<Person>>(responseText);

                output.AddRange(peoples);

                if (peoples == null || peoples.Count < recordsPerPage)
                {
                    queryMore = false;
                }
                else
                {
                    offset = offset + recordsPerPage;
                    queryStrings["offset"] = offset.ToString();

                    if (offset < output.Count)
                    {
                        queryMore = false;
                        output = output.GroupBy(x => x.id).Select(group => group.First()).ToList();
                    }
                }
            }

            if (includeDetail)
            {
                foreach (var people in output)
                {
                    people.FetchDetailToList(profileFields);
                }
            }

            return output;
        }

        public Person ShowPerson(string id, bool includeDetail = true)
        {
            Dictionary<string, string> queryStrings = new Dictionary<string, string>();
            queryStrings.Add("details", (includeDetail ? "1" : "0"));
            var responseText = SendRequest($"people/{id}", Method.GET, queryStrings);

            Person p = JsonConvert.DeserializeObject<Person>(responseText);
            if (includeDetail)
            {
                p.FetchDetailToList(profileFields);
            }
            return p;
        }

        public List<Profile> ListProfileFields()
        {
            var responseText = SendRequest("profile", Method.GET, null);

            List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(responseText);

            profileFields = profiles;

            return profiles;
        }
        public List<Folder> ListFolders()
        {
            var responseText = SendRequest("tags/list_folders", Method.GET, null);

            List<Folder> folders = JsonConvert.DeserializeObject<List<Folder>>(responseText);

            return folders;
        }

        public List<Tag> ListTags(string folderId = "")
        {
            Dictionary<string, string> queryStrings = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(folderId))
            {
                queryStrings.Add("folder_id", folderId);
            }
            var responseText = SendRequest("tags/list_tags", Method.GET, queryStrings);

            List<Tag> tags = JsonConvert.DeserializeObject<List<Tag>>(responseText);

            return tags;
        }

        public List<Person> ListPeopleWithTags(bool includeDetail, List<Tag> tags)
        {
            string filter = BuildTagFilter(tags);
            return ListPeople(includeDetail, filter);
        }

        public List<Person> ListPeopleWithTag(bool includeDetail, string tagId)
        {
            return ListPeopleWithTags(includeDetail, new List<Tag> { new Tag() { id = tagId } });
        }

        private string BuildTagFilter(List<Tag> tags)
        {
            if (tags == null || !tags.Any())
            {
                return "";
            }
            dynamic filter = new JObject();
            filter.tag_contains = string.Join("-", tags.Select(x => $"y_{x.id}"));
            return JsonConvert.SerializeObject(filter);
        }

        public List<Event> ListEvents(DateTime? start = null, DateTime? end = null, string categoryId = "",
            bool includeDetail = false, bool isEligible = false, int MaxNumberOfEvents = 500)
        {
            Dictionary<string, string> queryStrings = new Dictionary<string, string>();
            if (start.HasValue && start.Value != DateTime.MinValue)
            {
                queryStrings.Add("start", start.Value.ToString(DATE_FORMAT));
            }

            if (end.HasValue && end.Value != DateTime.MinValue)
            {
                queryStrings.Add("end", end.Value.ToString(DATE_FORMAT));
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                queryStrings.Add("category_id", categoryId);
            }

            if (isEligible)
            {
                queryStrings.Add("eligible", "1");
            }
            else
            {
                queryStrings.Add("eligible", "0");
            }

            if (includeDetail)
            {
                queryStrings.Add("details", "1");
            }
            else
            {
                queryStrings.Add("details", "0");
            }
            if (MaxNumberOfEvents > 1000) MaxNumberOfEvents = 1000;
            if (MaxNumberOfEvents <= 0) MaxNumberOfEvents = 500;

            queryStrings.Add("limit", MaxNumberOfEvents.ToString());
            var responseText = SendRequest("events", Method.GET, queryStrings);

            List<Event> events = JsonConvert.DeserializeObject<List<Event>>(responseText);

            return events;
        }

        public List<Event> ListEvent(string instanceId, bool isScheduled = false,
            ScheduleDirection direction = ScheduleDirection.Before, int scheduleLimit = 100,
            bool isEligible = false, bool includeDetail = false)
        {
            Dictionary<string, string> queryStrings = new Dictionary<string, string>();
            queryStrings.Add("instance_id", instanceId);

            queryStrings.Add("schedule", isScheduled ? "1" : "0");

            queryStrings.Add("schedule_direction", direction.ToString().ToLower());

            if (isEligible)
            {
                queryStrings.Add("eligible", "1");
            }
            else
            {
                queryStrings.Add("eligible", "0");
            }

            if (includeDetail)
            {
                queryStrings.Add("details", "1");
            }
            else
            {
                queryStrings.Add("details", "0");
            }
            if (scheduleLimit > 100) scheduleLimit = 100;
            if (scheduleLimit <= 0) scheduleLimit = 10;

            queryStrings.Add("limit", scheduleLimit.ToString());
            var responseText = SendRequest("events/list_event", Method.GET, queryStrings);
            if (responseText.StartsWith("["))
            {
                List<Event> events = JsonConvert.DeserializeObject<List<Event>>(responseText);
                return events;
            }
            else
            {
                return new List<Event> { JsonConvert.DeserializeObject<Event>(responseText) };
            }


        }

        public List<Calendar> ListCalendars()
        {
            var responseText = SendRequest("events/calendars/list", Method.GET, null);

            List<Calendar> calendars = JsonConvert.DeserializeObject<List<Calendar>>(responseText);

            return calendars;
        }


        public Person AddPerson(string firstname, string lastname, string email, string mobile, string gender)
        {
            Dictionary<string, string> queryStrings = new Dictionary<string, string>();
            queryStrings.Add("first", firstname);
            queryStrings.Add("last", lastname);


            var fields = profileFields.SelectMany(x => x.fields).ToList();
            var requestFields = new List<JObject>();

            if (!string.IsNullOrEmpty(email))
            {
                var emailField = fields.Find(x => x.name == "Email" && x.field_type == "email");
                if (emailField != null)
                {
                    dynamic emailRequest = new JObject();
                    emailRequest.field_id = emailField.field_id;
                    emailRequest.field_type = emailField.field_type;
                    emailRequest.response = true;
                    emailRequest.details = new JObject();
                    emailRequest.details.address = email;
                    requestFields.Add(emailRequest);
                }
            }

            if (!string.IsNullOrEmpty(mobile))
            {
                var mobileField = fields.Find(x => x.name == "Phone" && x.field_type == "phone");
                if (mobileField != null)
                {
                    dynamic mobileRequest = new JObject();
                    mobileRequest.field_id = mobileField.field_id;
                    mobileRequest.field_type = mobileField.field_type;
                    mobileRequest.response = true;
                    mobileRequest.details = new JObject();
                    mobileRequest.details.phone_mobile = mobile;
                    requestFields.Add(mobileRequest);
                }
            }

            if (!string.IsNullOrEmpty(gender))
            {
                var genderField = fields.Find(x => x.name == "Gender" && x.field_type == "multiple_choice");
                if (genderField != null)
                {
                    var optionId = genderField.options?.Where(x => x.name.Equals(gender, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (optionId != null)
                    {
                        dynamic genderRequest = new JObject();
                        genderRequest.field_id = genderField.field_id;
                        genderRequest.field_type = genderField.field_type;
                        genderRequest.response = optionId.option_id;
                        requestFields.Add(genderRequest);
                    }
                }
            }


            queryStrings.Add("fields_json", JsonConvert.SerializeObject(requestFields)) ;

            var responseText = SendRequest("people/add", Method.GET, queryStrings);

            return JsonConvert.DeserializeObject<Person>(responseText);
        }
    }

}
