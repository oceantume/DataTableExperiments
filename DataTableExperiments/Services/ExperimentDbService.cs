using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace DataTableExperiments.Services
{
    public class ExperimentDbService
    {
        private string ConnectionString { get; }


        private ExperimentDbService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static ExperimentDbService Create()
        {
            var setting = ConfigurationManager.ConnectionStrings["DefaultConnection"];

            var connectionString = setting?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("No connection string!");

            return new ExperimentDbService(connectionString);
        }



        public IEnumerable<SimpleDataModel> GetSimpleData()
        {
            using (var connection = new SqlConnection(ConnectionString))
                return connection.Query<SimpleDataModel>("pGetSimpleData", commandType: CommandType.StoredProcedure);
        }

        public class SimpleDataModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Value { get; set; }
        }
    }
}