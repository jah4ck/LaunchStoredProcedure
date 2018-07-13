using LaunchStoredProcedure.entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaunchStoredProcedure.business.Interface
{
    public interface IStoredProcedureManager
    {
        Task<TaskResult> GetLstStoredProcedure(string id_collaborateur);
        Task<TaskResult> GetLstParamStoredProcedureByName(string name, string id_collaborateur);
        Task<TaskResult> ExecStoredProcedureById(string name, IList<StoredProcedureParameters> param, string id_collaborateur);
        Task<TaskResult> SetRedisKey(string key, string value);
        Task<TaskResult> GetRedisKey(string key);
    }
}
