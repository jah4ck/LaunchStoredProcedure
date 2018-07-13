using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaunchStoredProcedure.web.Helpers
{
    public static class ListHelper
    {
        public static List<dynamic> Top(List<dynamic> input, int nb)
        {
            return input.Take(nb).ToList();
        }
    }
}
