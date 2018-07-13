using Dapper;
using LaunchStoredProcedure.entities;
using LaunchStoredProcedure.repository.Interface;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaunchStoredProcedure.repository
{
    public class StoredProcedureRepository : BaseRepository, IStoredProcedureRepository
    {
        CultureInfo provider = new CultureInfo("fr-FR");
        public StoredProcedureRepository(IConfiguration config) : base(config)
        {
            config.GetConnectionString("DefaultConnection");
        }

        public async Task<TaskResult> GetLstStoredProcedure(string id_collaborateur)
        {
            TaskResult taskResult = new TaskResult();
            try
            {
                await WithConnection(async c =>
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("collaborateur", id_collaborateur);
                    taskResult.Result = await c.QueryAsync<StoredProcedure>("SRP.dbo.ps_extra_stored_procedure_access_by_collaborateur", parameters, null, 6000, commandType: CommandType.StoredProcedure);
                    taskResult.Succeeded = taskResult.Result != null ? true : false;
                    return taskResult;
                });
                return taskResult;
            }
            catch (Exception ex)
            {
                taskResult.Exception = ex;
                taskResult.Message = "Erreur lors de la récupération des données";
                taskResult.Succeeded = false;
                return taskResult;
            }
        }

        public async Task<TaskResult> GetLstStoredProcedureByName(string id_collaborateur)
        {
            TaskResult taskResult = new TaskResult();
            try
            {
                await WithConnection(async c =>
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("collaborateur", id_collaborateur);
                    taskResult.Result = await c.QueryAsync<StoredProcedure>("SRP.dbo.ps_extra_stored_procedure_access_by_collaborateur", parameters, null, 6000, commandType: CommandType.StoredProcedure);
                    taskResult.Succeeded = taskResult.Result != null ? true : false;
                    return taskResult;
                });
                return taskResult;
            }
            catch (Exception ex)
            {
                taskResult.Exception = ex;
                taskResult.Message = "Erreur lors de la récupération des données";
                taskResult.Succeeded = false;
                return taskResult;
            }
        }


        public async Task<TaskResult> GetLstParamStoredProcedureByName(StoredProcedure sp)
        {
            TaskResult taskResult = new TaskResult();
            try
            {
                await WithConnection(async c =>
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("ps_name", sp.PS_NAME);
                    string query = $"{sp.DATA_SOURCE}.dbo.ps_extra_stored_procedure_parameters_get_v2";
                    //taskResult.Result = await c.QueryAsync<StoredProcedureParameters>("SELECT * FROM SRP.dbo.PROCEDURE_STOCKEE_PARAMETERS WITH(NOLOCK) WHERE ENABLED = 1 AND PS_ID = @id", parameters, null, 6000, CommandType.Text);
                    //OK == > taskResult.Result = await c.QueryAsync<StoredProcedureParameters>("SRP.dbo.ps_extra_stored_procedure_parameters_get_v2", parameters, null, 6000, commandType: CommandType.StoredProcedure);
                    taskResult.Result = await c.QueryAsync<StoredProcedureParameters>(query, parameters, null, 6000, commandType: CommandType.StoredProcedure);
                    taskResult.Succeeded = taskResult.Result != null ? true : false;
                    return taskResult;
                });
                return taskResult;
            }
            catch (Exception ex)
            {
                taskResult.Exception = ex;
                taskResult.Message = "Erreur lors de la récupération des données";
                taskResult.Succeeded = false;
                return taskResult;
            }
        }


        public async Task<TaskResult> ExecStoredProcedureById(string name, IList<StoredProcedureParameters> param)
        {
            TaskResult taskResult = new TaskResult();
            try
            {
                await WithConnection(async c =>
                {
                    DynamicParameters parameters = new DynamicParameters();
                    string query = $"{param.First().DB_NAME}.dbo.{name}";
                    foreach (var item in param)
                    {
                        if (!string.IsNullOrEmpty(item.DATA_TYPE))
                        {
                            switch (item.DATA_TYPE)
                            {
                                case "bit":
                                    parameters.Add(item.PARAMETER_NAME, Boolean.Parse(item.valSaisie));
                                    break;
                                case "decimal":
                                    parameters.Add(item.PARAMETER_NAME, Decimal.Parse(item.valSaisie));
                                    break;
                                case "uniqueidentifier":
                                    parameters.Add(item.PARAMETER_NAME, new Guid(item.valSaisie));
                                    break;
                                case "date":
                                    parameters.Add(item.PARAMETER_NAME, DateTime.ParseExact(item.valSaisie,"dd/MM/yyyy",provider));
                                    break;
                                case "datetime":
                                    parameters.Add(item.PARAMETER_NAME,
                                        item.valSaisie.Length == 10 ? DateTime.ParseExact(item.valSaisie, "dd/MM/yyyy", provider) :
                                        item.valSaisie.Length == 13 ? DateTime.ParseExact(item.valSaisie, "dd/MM/yyyy HH", provider) :
                                        item.valSaisie.Length == 16 ? DateTime.ParseExact(item.valSaisie, "dd/MM/yyyy HH:mm", provider) :
                                        DateTime.ParseExact(item.valSaisie, "dd/MM/yyyy HH:mm:ss", provider)
                                        );
                                    break;
                                default:
                                    parameters.Add(item.PARAMETER_NAME, item.valSaisie);
                                    break;
                            }
                        }
                        else
                        {
                            parameters.Add(item.PARAMETER_NAME, item.valSaisie);
                        }
                        
                    }
                    var grid = await c.QueryMultipleAsync(query, parameters, commandType: CommandType.StoredProcedure);
                    taskResult.LstResultSet = new List<dynamic>();
                    while (!grid.IsConsumed)
                    {
                        taskResult.LstResultSet.Add(grid.Read<dynamic>().ToList());
                    }


                    taskResult.Succeeded = taskResult.Result != null ? true : false;
                    return taskResult;
                });
                return taskResult;
            }
            catch (Exception ex)
            {
                taskResult.Exception = ex;
                taskResult.Message = "Erreur lors de la récupération des données";
                taskResult.Succeeded = false;
                return taskResult;
            }
        }


        public async Task<TaskResult> SetRedisKey(string key, string value)
        {
            TaskResult taskResult = new TaskResult();
            try
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("10.0.75.1:6379");
                int databaseNumber = 0;
                object asyncState = taskResult;
                IDatabase db = redis.GetDatabase(databaseNumber, asyncState);
                taskResult.Succeeded = await db.StringSetAsync(key, value);
                await db.KeyExpireAsync(key, DateTime.Now.AddMinutes(10));
                return taskResult;
            }
            catch (Exception ex)
            {
                taskResult.Exception = ex;
                taskResult.Message = "Erreur lors de l'enregitrement des données";
                taskResult.Succeeded = false;
                return taskResult;
            }
            
        }

        public async Task<TaskResult> GetRedisKey(string key)
        {
            TaskResult taskResult = new TaskResult();
            try
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("10.0.75.1:6379");
                int databaseNumber = 0;
                object asyncState = taskResult;
                IDatabase db = redis.GetDatabase(databaseNumber, asyncState);
                taskResult.Result = await db.StringGetAsync(key);
                taskResult.Succeeded = true ? taskResult.Result != null : false;
                return taskResult;
            }
            catch (Exception ex)
            {
                taskResult.Exception = ex;
                taskResult.Message = $"Erreur lors de la récupération des données de la key {key} sur Redis";
                taskResult.Succeeded = false;
                return taskResult;
            }

        }




    }
}
