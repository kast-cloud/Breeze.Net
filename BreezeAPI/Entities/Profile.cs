using System;
using System.Collections.Generic;
using System.Text;

namespace BreezeAPI.Entities
{
    public class Profile
    {
        public string id { get; set; }
        public string oid { get; set; }
        public string section_id { get; set; }
        public string name { get; set; }
        public string column_id { get; set; }
        public string position { get; set; }
        public string profile_id { get; set; }
        public string created_on { get; set; }
        public Field[] fields { get; set; }
    }

    public class Field
    {
        public string id { get; set; }
        public string oid { get; set; }
        public string field_id { get; set; }
        public string profile_section_id { get; set; }
        public string field_type { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public string profile_id { get; set; }
        public string created_on { get; set; }
        public Option[] options { get; set; }
    }

    public class Option
    {
        public string id { get; set; }
        public string oid { get; set; }
        public string option_id { get; set; }
        public string profile_field_id { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public string profile_id { get; set; }
        public string created_on { get; set; }
    }

}
