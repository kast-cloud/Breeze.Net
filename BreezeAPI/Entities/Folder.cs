﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BreezeAPI.Entities
{
    public class Folder
    {
        public string id { get; set; }
        public string parent_id { get; set; }
        public string name { get; set; }
        public string created_on { get; set; }
    }

}
