using System;
using System.Collections.Generic;
using System.Text;

namespace BreezeAPI.Entities
{
    public class Calendar
    {
        public object id { get; set; }
        public string oid { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string address { get; set; }
        public string embed_key { get; set; }
        public string created_on { get; set; }
    }

}
