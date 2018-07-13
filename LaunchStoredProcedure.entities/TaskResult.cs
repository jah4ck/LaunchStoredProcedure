using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchStoredProcedure.entities
{
    public class TaskResult
    {
        public bool Succeeded { get; set; }
        public Exception Exception { get; set; }
        public dynamic Result { get; set; }
        public string Message { get; set; }
        public bool Authorize { get; set; }
        //public IList<dynamic> LstResultSet { get; set; }
        public ICollection<dynamic> LstResultSet { get; set; }
    }
}
