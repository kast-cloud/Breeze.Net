using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Breeze.Net.Entities
{
    public class Person
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string force_first_name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string nick_name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string middle_name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string maiden_name { get; set; }

        public string last_name { get; set; }
        public string thumb_path { get; set; }
        public string path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string street_address { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zip { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Details details { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Family[] family { get; set; }

        public void FetchDetailToList(List<Profile> profileFields)
        {
            if (details?.AdditionalData == null) return;
            var customValues = new List<CustomValue>();

            var fields = profileFields.SelectMany(x => x.fields);
            CustomValue pairValue = new CustomValue()
            {
                FieldType = "",
                FieldName = "",
                FieldId = "",
                FieldValues = new Dictionary<string, string>()
            };

            customValues.Add(pairValue);

            foreach (var item in details.AdditionalData)
            {
                var fieldId = item.Key;
                var fieldvalue = item.Value;

                var field = fields.Where(x => x.field_id == fieldId).FirstOrDefault();
                if (field != null)
                {
                    CustomValue value = new CustomValue()
                    {
                        FieldType = field.field_type,
                        FieldName = field.name,
                        FieldId = fieldId
                    };
                    try
                    {
                        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldvalue.ToString());
                        if (values.ContainsKey("name"))
                        {
                            value.FieldValue = values["name"];
                        }
                        value.FieldValues = values;
                    }
                    catch
                    {
                        value.FieldValue = fieldvalue.ToString();
                    }
                    customValues.Add(value);

                }
                else
                {
                    if (fieldId == "details")
                    {
                        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldvalue.ToString());
                        foreach (var key in values.Keys)
                        {
                            pairValue.FieldValues[key] = values[key];
                        }
                    }
                    else
                    {
                        pairValue.FieldValues.Add(fieldId, fieldvalue.ToString());
                    }
                    
                }
            }

            details.CustomValues = customValues;
        }

    }

    public class Details
    {
        public string person_id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string birthdate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string grade { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string single_line { get; set; }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext ctx)
        {
            if (this != null)
            {
                var ser = (JValue)JsonConvert.SerializeObject(this);
                AdditionalData.Merge(ser);
            }
        }

        [JsonExtensionData]
        [JsonIgnore]
        internal JObject AdditionalData { get; set; }

        [JsonIgnore]
        public List<CustomValue> CustomValues { get; set; }
    }

    public class Family
    {
        public string id { get; set; }
        public string oid { get; set; }
        public string person_id { get; set; }
        public string family_id { get; set; }
        public string family_role_id { get; set; }
        public string created_on { get; set; }
        public string role_name { get; set; }
        public string role_id { get; set; }
        public string order { get; set; }
        public FamilyDetail details { get; set; }
    }

    public class FamilyDetail
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string force_first_name { get; set; }
        public string last_name { get; set; }
        public string thumb_path { get; set; }
        public string path { get; set; }
    }

    public class CustomValue
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }

        public string FieldValue { get; set; }

        public Dictionary<string, string> FieldValues { get; set; }
    }

}
