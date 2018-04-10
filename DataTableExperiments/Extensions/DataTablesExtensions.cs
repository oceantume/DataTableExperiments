using DataTables.AspNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DataTableExperiments.Extensions
{
    public static class DataTablesExtensions
    {
        public static IEnumerable<TSource[]> DataTableSortArray<TSource>(this IEnumerable<TSource[]> source, IEnumerable<IColumn> columns)
        {
            var result = source;

            if (columns != null)
            {
                var columnSorts = columns
                    .Where(c => c.IsSortable && c.Sort != null)
                    .OrderBy(c => c.Sort.Order)
                    .Select(c => new
                    {
                        SortDirection = c.Sort.Direction,
                        Index = int.Parse(c.Field)
                    });

                foreach (var c in columnSorts)
                    result = result.OrderByThenBy(e => e[c.Index], c.SortDirection);
            }

            return result;
        }

        public static IEnumerable<TSource> DataTableSort<TSource>(this IEnumerable<TSource> source, IEnumerable<IColumn> columns)
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



        public static IEnumerable<TSource[]> DataTableSearchArray<TSource>(this IEnumerable<TSource[]> source, Func<TSource[], string> defaultSearchValueSelector, ISearch search)
        {
            var result = source;

            if (search != null && !string.IsNullOrEmpty(search.Value))
                result = result.DataTableSearchValue(e => defaultSearchValueSelector(e), search);

            return result;
        }

        public static IEnumerable<TSource[]> DataTableSearchColumnsArray<TSource>(this IEnumerable<TSource[]> source, IEnumerable<IColumn> columns)
        {
            var result = source;

            if (columns != null)
            {
                var columnSearches = columns
                    .Where(c => c.IsSearchable && c.Search != null && !string.IsNullOrEmpty(c.Search.Value))
                    .Select(c => new
                    {
                        Search = c.Search,
                        Index = int.Parse(c.Field)
                    });

                foreach (var c in columnSearches)
                    result = result.DataTableSearchValue(e => e[c.Index].ToString(), c.Search);
            }

            return result;
        }

        public static IEnumerable<TSource> DataTableSearch<TSource>(this IEnumerable<TSource> source, Func<TSource, string> defaultSearchValueSelector, ISearch search, IEnumerable<IColumn> columns)
        {
            var result = source;

            if (search != null)
                result = result.DataTableSearchValue(e => defaultSearchValueSelector(e), search);

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
}