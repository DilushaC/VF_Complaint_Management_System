using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.ConncetionHandler
{
    public class _ConnectionService
    {
        private readonly DapperContext _context;

        public _ConnectionService(DapperContext context)
        {
            _context = context;
        }
        public int InsertAndGetId(string query, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();
            return connection.QuerySingle<int>(query, parameters);
        }

        public List<T> Query<T>(string query)
        {
            using var connection = _context.CreateConnection();
            return connection.Query<T>(query).ToList();
        }

        public async Task<PaginationResultsModel<T>> QueryMultipleForPaginationAsync<T>(string query, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(query, parameters);

            var items = (await multi.ReadAsync<T>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new PaginationResultsModel<T>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public DataTable Return(string query)
        {
            using var connection = _context.CreateConnection();
            using var reader = connection.ExecuteReader(query, commandTimeout: int.MaxValue);
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }

        public DataTable ReturnWithPara(string query, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();
            using var reader = connection.ExecuteReader(query, parameters, commandTimeout: int.MaxValue);
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }
        

        public object ExecuteScalar(string query)
        {
            using var connection = _context.CreateConnection();
            var result = connection.ExecuteScalar(query);
            return result == DBNull.Value ? null : result;
        }

        public DataTable ExecuteQuery(string query)
        {
            using var connection = _context.CreateConnection();
            using var reader = connection.ExecuteReader(query);
            var dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }

        public void ExecuteCommand(string query)
        {
            using var connection = _context.CreateConnection();
            connection.Execute(query);
        }

        public DataSet ExecuteMultipleTbl(string query)
        {
            using var connection = _context.CreateConnection();
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            var adapter = new SqlDataAdapter((SqlCommand)command);
            var dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet;
        }
    }
}
