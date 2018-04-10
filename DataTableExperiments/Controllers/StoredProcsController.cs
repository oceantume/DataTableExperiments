using DataTableExperiments.Services;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataTableExperiments.Controllers
{
    public class StoredProcsController : Controller
    {
        private ExperimentDbService Db { get; }
            = ExperimentDbService.Create();

        // GET: StoredProcs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(IDataTablesRequest request)
        {
            var data = Db.GetSimpleData();

            var response = DataTablesResponse.Create(request, data.Count(), data.Count(), data);

            return new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
        }
    }
}