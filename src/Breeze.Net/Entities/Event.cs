namespace Breeze.Net.Entities
{
    public class Event
    {

        public string id { get; set; }
        public string oid { get; set; }
        public string event_id { get; set; }
        public string name { get; set; }
        public string category_id { get; set; }
        public string settings_id { get; set; }
        public string start_datetime { get; set; }
        public string end_datetime { get; set; }
        public string is_modified { get; set; }
        public string created_on { get; set; }
    }
}
