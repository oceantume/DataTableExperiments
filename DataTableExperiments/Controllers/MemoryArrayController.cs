using DataTableExperiments.Extensions;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataTableExperiments.Controllers
{
    public class MemoryArrayController : Controller
    {
        private class DataEntry
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Value { get; set; }
        }

        private static readonly IList<object[]> s_data =
            Enumerable.Range(1, 100000)
                .Select((n) => new object[]
                {
                    n,
                    "test " + n,
                    new decimal(new Random(n).NextDouble())
                }).ToList();

        // GET: MemoryArray
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(IDataTablesRequest request)
        {
            var totalCount = s_data.Count;
            var query = s_data.AsEnumerable();

            var filtered = query
                .DataTableSearchArray(e => e[1].ToString(), request.Search)
                .DataTableSearchColumnsArray(request.Columns);

            var result = filtered
                .DataTableSortArray(request.Columns)
                .Skip(request.Start)
                .Take(request.Length);
            
            var data = result;

            var response = DataTablesResponse.Create(request, totalCount, filtered.Count(), data);

            return new DataTablesJsonResult(response, JsonRequestBehavior.AllowGet);
        }
    }
}