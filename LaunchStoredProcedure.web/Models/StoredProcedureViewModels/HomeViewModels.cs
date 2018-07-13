using LaunchStoredProcedure.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaunchStoredProcedure.web.Models.StoredProcedureViewModels
{
    public class HomeViewModels
    {
        public string SpName { get; set; }
        public StoredProcedure MyStoredProcedure { get; set; }
        public IList<StoredProcedure> LstStoredProcedure { get; set; }

        public IList<StoredProcedureParameters> MyLstStoredProcedureParameters { get; set; }
        public IList<StoredProcedureParameters> LstStoredProcedureParameters { get; set; }

        public string stringparam { get; set; }

        public dynamic result { get; set; }
    }
}
