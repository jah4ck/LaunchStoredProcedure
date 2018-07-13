using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LaunchStoredProcedure.entities
{
    public class StoredProcedureParameters 
    {
        public string DB_NAME { get; set; }
        public string PS_NAME { get; set; }
        public string PARAMETER_NAME { get; set; }
        public string DATA_TYPE { get; set; }
        public int MAX_LENGTH { get; set; }
        public bool IS_OUTPUT { get; set; }
        public int PRECISION { get; set; }
        public string DEFAULT_VALUE { get; set; }

        public string valSaisie { get; set; }

        
    }
}
