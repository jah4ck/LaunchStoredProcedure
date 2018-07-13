using LaunchStoredProcedure.business.Interface;
using LaunchStoredProcedure.entities;
using LaunchStoredProcedure.repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LauchStoredProcedure.business
{
    public class StoredProcedureManager : IStoredProcedureManager
    {
        readonly IStoredProcedureRepository _storedProcedureRepository;
        public StoredProcedureManager(IStoredProcedureRepository storedProcedureRepository)
        {
            _storedProcedureRepository = storedProcedureRepository;
        }


        public async Task<TaskResult> GetLstStoredProcedure(string id_collaborateur)
        {
            return await _storedProcedureRepository.GetLstStoredProcedure(id_collaborateur);
        }

        public async Task<TaskResult> GetLstParamStoredProcedureByName(string name, string id_collaborateur)
        {
            TaskResult tk= await _storedProcedureRepository.GetLstStoredProcedure(id_collaborateur);
            IList<StoredProcedure> lstSP = tk.Result;
            StoredProcedure sp = lstSP.Where(c => c.PS_NAME == name).First();
            return await _storedProcedureRepository.GetLstParamStoredProcedureByName(sp);
        }


        public async Task<TaskResult> ExecStoredProcedureById(string name, IList<StoredProcedureParameters> param, string id_collaborateur)
        {
            TaskResult tk = await GetLstParamStoredProcedureByName(name, id_collaborateur);
            IList<StoredProcedureParameters> paramBdd = tk.Result;

            foreach (var item in param)
            {
                StoredProcedureParameters spp = item;
                StoredProcedureParameters sppBDD = paramBdd.Where(c=>c.PARAMETER_NAME == spp.PARAMETER_NAME).First();
                spp.DATA_TYPE = sppBDD.DATA_TYPE;
                spp.DB_NAME = sppBDD.DB_NAME;

            }
            return await _storedProcedureRepository.ExecStoredProcedureById(name,param);
        }

        public async Task<TaskResult> SetRedisKey(string key, string value)
        {
            return await _storedProcedureRepository.SetRedisKey(key, value);
        }

        public async Task<TaskResult> GetRedisKey(string key)
        {
            return await _storedProcedureRepository.GetRedisKey(key);
        }
    }
}
