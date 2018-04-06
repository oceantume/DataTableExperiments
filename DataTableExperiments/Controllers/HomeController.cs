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
    public static class Extensions
    {
        public static IEnumerable<TSource> DataTableSort<TSource>(this IEnumerable<TSource> source, IEnumerable<IColumn> columns)
            where TSource : HomeController.DataEntry
        {
            if (columns == null)
                return source;

            var props = typeof(TSource).GetProperties();

            var orderedColumns = columns
                .Where(c => c.IsSortable && c.Sort != null)
                .OrderBy(c => c.Sort.Order);

            var result = source;
            foreach (var c in orderedColumns)
            {
                var prop = props.Single(p => p.Name.ToLowerInvariant() == c.Field.ToLowerInvariant());
                result = result.OrderByThenBy(e => prop.GetValue(e), c.Sort.Direction);
            }

            return result;
        }

        private static IOrderedEnumerable<TSource> OrderByThenBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction)
        {
            var ordered = source as IOrderedEnumerable<TSource>;

            if (ordered != null)
            {
                return direction == SortDirection.Ascending
                    ? ordered.ThenBy(keySelector)
                    : ordered.ThenByDescending(keySelector);
            }
            else
            {
                return direction == SortDirection.Ascending
                    ? source.OrderBy(keySelector)
                    : source.OrderByDescending(keySelector);
            }
        }



        public static IEnumerable<TSource> DataTableSearch<TSource>(this IEnumerable<TSource> source, ISearch search, IEnumerable<IColumn> columns)
            where TSource : HomeController.DataEntry
        {
            var result = source;

            if (search != null)
            {
                result = result.DataTableSearchValue(e => e.Name, search);
            }

            if (columns != null)
            {
                var props = typeof(TSource).GetProperties();

                foreach (var c in columns.Where(c => c.IsSearchable && c.Search != null))
                {
                    var prop = props.Single(p => p.Name.ToLowerInvariant() == c.Field.ToLowerInvariant());
                    result = result.DataTableSearchValue(e => prop.GetValue(e).ToString(), c.Search);
                }
            }

            return result;
        }

        private static IEnumerable<TSource> DataTableSearchValue<TSource>(this IEnumerable<TSource> source, Func<TSource, string> valueSelector, ISearch search)
        {
            if (search == null)
                return source;

            return search.IsRegex
                    ? source.Where(e => Regex.IsMatch(valueSelector(e), search.Value))
                    : source.Where(e => valueSelector(e).IndexOf(search.Value, StringComparison.InvariantCultureIgnoreCase) > -1);
        }
    }

    public class HomeController : Controller
    {
        public class DataEntry
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       

        public ActionResult GetData(IDataTablesRequest request)
        {
            
            var totalCount = s_data.Count;
            var query = s_data.AsEnumerable();

            var filtered = query
                .DataTableSearch(request.Search, request.Columns);

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