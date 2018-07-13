using LaunchStoredProcedure.business.Interface;
using LaunchStoredProcedure.entities;
using LaunchStoredProcedure.web.Helpers;
using LaunchStoredProcedure.web.Models;
using LaunchStoredProcedure.web.Models.StoredProcedureViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace LaunchStoredProcedure.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string id_collaborateur = "FCE3F558-7BCD-4CB9-805F-52F23937FBBA";
        private readonly IStoredProcedureManager _StoredProcedureManager;
        public HomeController(IStoredProcedureManager storedProcedureManager)
        {
            _StoredProcedureManager = storedProcedureManager;
        }

        public async Task<IActionResult> Index()
        {
            
            TaskResult LstStoredProcedure = await _StoredProcedureManager.GetLstStoredProcedure(id_collaborateur);

            HomeViewModels homeViewModels = new HomeViewModels
            {
                LstStoredProcedure = LstStoredProcedure.Result,
            };

            return View(homeViewModels);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<JsonResult> GetStoredProcedureParam(string id)
        {
            TaskResult LstParamStoredProcedure = await _StoredProcedureManager.GetLstParamStoredProcedureByName(id, id_collaborateur);
            return Json(new { LstParamStoredProcedure });
        }

        

        [HttpPost]
        public async Task<JsonResult> CalculStoredProcedure(HomeViewModels vm)
        {

            string nameProc = vm.SpName;
            IList<StoredProcedureParameters> paramProc = vm.MyLstStoredProcedureParameters; 

            TaskResult result = new TaskResult();

            if (!ModelState.IsValid)
            {
                result.Succeeded = false;
                result.Message = "erreur de modèle";
                return Json(new { result });
            }
                
            result = await _StoredProcedureManager.ExecStoredProcedureById(nameProc, paramProc, id_collaborateur);
            //TempData.Put("Tableau", result);

            //sélection des 10 premières lignes pour chaque resultset
            TaskResult resultAffichage = new TaskResult();
            resultAffichage.LstResultSet = new List<dynamic>();
            foreach (var item in result.LstResultSet)
            {
                dynamic toto = ListHelper.Top(item, 10);
                resultAffichage.LstResultSet.Add(toto);
            }

            //stockage du résultat total sous redis 
            string resultJson = JsonConvert.SerializeObject(result.LstResultSet);
            TaskResult WriteData = await _StoredProcedureManager.SetRedisKey($"{nameProc}_{id_collaborateur}", resultJson);


            return Json(resultAffichage);
        }


        [HttpGet]
        public async Task<IActionResult> SaveResultCsv(int id, string sp_name)
        {
            TaskResult tab = await _StoredProcedureManager.GetRedisKey($"{sp_name}_{id_collaborateur}");
            tab.LstResultSet = JsonConvert.DeserializeObject<ICollection<dynamic>>((string)tab.Result);

            // converti en list dynamic pour pouvoir sélection le bon résultset 
            List<dynamic> myList = new List<dynamic>(tab.LstResultSet);//liste contenant les différent resultset au format json

            //on récupère uniquement le dataset qui a été sélectionné 
            List<dynamic> tabDeserialize = myList[id].ToObject<List<dynamic>>();//jusque là tout est ok la liste contien chaque ligne au format JSON

            //création de la datatable 
            DataTable tableFinale = ConvertHelper.JsonArrayToDataTable(tabDeserialize);

            //création du stream pour enregistrement du fichier
            byte[] byteArray = ConvertHelper.CsvBytesWriter(ref tableFinale);
            var mimeType = "application/vnd.ms-excel";
            var memory = new MemoryStream();
            using (var stream = new MemoryStream(byteArray))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, mimeType, $"{sp_name}_{id}.csv");
            
        }
    }
}
