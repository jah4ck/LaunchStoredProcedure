using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchStoredProcedure.web.Helpers
{
    public static class ConvertHelper
    {
        public static byte[] CsvBytesWriter(ref DataTable dTable)
        {

            //--------Columns Name---------------------------------------------------------------------------

            StringBuilder sb = new StringBuilder();
            int intClmn = dTable.Columns.Count;

            int i = 0;
            for (i = 0; i <= intClmn - 1; i += 1)
            {
                sb.Append(@"""" + dTable.Columns[i].ColumnName.ToString() + @"""");
                if (i == intClmn - 1)
                {
                    sb.Append(" ");
                }
                else
                {
                    sb.Append(";");
                }
            }
            sb.Append(Environment.NewLine);

            //--------Data By  Columns---------------------------------------------------------------------------


            foreach (DataRow row in dTable.Rows)
            {
                int ir = 0;
                for (ir = 0; ir <= intClmn - 1; ir += 1)
                {
                    sb.Append(@"""" + row[ir].ToString().Replace(@"""", @"""""") + @"""");
                    if (ir == intClmn - 1)
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        sb.Append(";");
                    }

                }
                sb.Append(Environment.NewLine);
            }

            return System.Text.Encoding.Unicode.GetBytes(sb.ToString());

        }

        public static DataTable JsonArrayToDataTable(List<dynamic> JsonList)
        {
            DataTable table = new DataTable();

            ExpandoObject keyValueFirstRow = JsonConvert.DeserializeObject<ExpandoObject>(JsonList[0].ToString());
            foreach (var item in keyValueFirstRow)
            {
                table.Columns.Add(item.Key);
            }
            foreach (var item in JsonList)
            {
                var data = table.NewRow();
                ExpandoObject keyValue = JsonConvert.DeserializeObject<ExpandoObject>(item.ToString());
                foreach (var prop in keyValue)
                {
                    data[prop.Key] = prop.Value;
                }
                table.Rows.Add(data);
            }
            return table;
        }
    }
}
