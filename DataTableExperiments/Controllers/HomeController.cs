using DataTableExperiments.Extensions;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DataTableExperiments.Controllers
{
    public class HomeController : Controller
    {
        private class DataEntry
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Value { get; set; }
        }

        private static readonly IList<DataEntry> s_data =
            Enumerable.Range(1, 100000)
                .Select((n) => new DataEntry
                {
                    Id = n,
                    Name = "test " + n,
                    Value = new decimal(new Random(n).NextDouble())
                }).ToList();



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(IDataTablesRequest request)
        {
            
            var totalCount = s_data.Count;
            var query = s_data.AsEnumerable();

            var filtered = query
                .DataTableSearch(e => e.Name, request.Search, request.Columns);

            var result = filtered
                .DataTableSort(request.Columns)
                .Skip(request.Start)
                .Take(request.Length);

            var data = result.ToList();
            
            var response = DataTablesResponse.Create(request, totalCount, filtered.Count(), data);

            return new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
        }
    }
}