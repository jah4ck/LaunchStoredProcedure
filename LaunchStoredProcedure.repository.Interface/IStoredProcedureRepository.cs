using LaunchStoredProcedure.entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaunchStoredProcedure.repository.Interface
{
    public interface IStoredProcedureRepository
    {
        Task<TaskResult> GetLstStoredProcedure(string id_collaborateur);

        Task<TaskResult> GetLstParamStoredProcedureByName(StoredProcedure sp);

        Task<TaskResult> ExecStoredProcedureById(string name, IList<StoredProcedureParameters> param);

        Task<TaskResult> SetRedisKey(string key, string value);

        Task<TaskResult> GetRedisKey(string key);
    }
}
