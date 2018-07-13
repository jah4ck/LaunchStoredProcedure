using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchStoredProcedure.entities
{
    public class StoredProcedure
    {
        public int PS_ID { get; set; }
        public string PS_NAME { get; set; }
        public string DATA_SOURCE { get; set; }
        public string TITRE { get; set; }
        public string COMMENTAIRES { get; set; }
        public int? TIMEOUT { get; set; }
        public string stringparam { get; set; }
    }
}
