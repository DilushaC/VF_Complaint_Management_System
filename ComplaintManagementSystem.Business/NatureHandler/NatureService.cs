using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplaintManagementSystem.Business.NatureHandler
{
    public class NatureService : INatureService
    {
        private readonly _ConnectionService _connectionService;

        public NatureService(_ConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public List<NatureModel> getAllList()
        {

            try
            {
                string Query = @"
                                SELECT CN.Id AS Id, 
                                       CD.Name AS dep_name, 
                                       CD.Id AS dep_id, 
                                       CN.Nature AS nature, 
                                       CN.Code AS code,
                                       CN.Active AS active,
                                       CN.CreatedDate AS createdDate
                                FROM Complaint_Nature_Master AS CN
                                INNER JOIN Complaint_Department_Master AS CD ON CD.Id = CN.Dep_Id
                                WHERE CN.Active = 1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<NatureModel> depList = new List<NatureModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    NatureModel bModel = new NatureModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Dep_Name = BRow["dep_name"].ToString(),
                        Dep_Id = Convert.ToInt32(BRow["dep_id"]),
                        Nature = BRow["nature"].ToString(),
                        Code = BRow["code"].ToString(),
                        Active = Convert.ToBoolean(BRow["active"]),
                        CreatedDate = Convert.ToDateTime(BRow["createdDate"]),
                    };
                    depList.Add(bModel);
                }
                return depList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Complaint_Department_MasterModel>> getDepList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Department_Master WHERE Active=1 AND Status = 1";
                var Data = _connectionService.Return(Query);
                var Row = Data.Rows[0];

                List<Complaint_Department_MasterModel> depList = new List<Complaint_Department_MasterModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    Complaint_Department_MasterModel bModel = new Complaint_Department_MasterModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Name = BRow["Name"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    depList.Add(bModel);
                }
                return depList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateNature(IFormCollection collection)
        {

            var Dep_Id = collection["Dep_Id"].ToString();
            var Nature = collection["Nature"].ToString();
            var Code = collection["Code"].ToString();

            var query = "INSERT INTO Complaint_Nature_Master (Dep_Id, Nature, Code, Active, CreatedDate) " +
                    "VALUES (@depid, @nature, @code, @active , @createdDate);";

            var parameters = new DynamicParameters();

            parameters.Add("depid", Convert.ToInt64(Dep_Id), DbType.Int64);
            parameters.Add("nature", Nature, DbType.String);
            parameters.Add("code", Code, DbType.String);
            parameters.Add("active", 1, DbType.Int32);
            parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);

            _connectionService.ReturnWithPara(query, parameters);

            return;
        }

        public NatureModel getNatureListId(int Id)
        {
            try
            {
                string query = $@"
                                SELECT CN.Id AS Id, 
                                       CD.Name AS dep_name, 
                                       CD.Id AS dep_id, 
                                       CN.Nature AS nature, 
                                       CN.Code AS code,
                                       CN.Active AS active,
                                       CN.CreatedDate AS createdDate
                                FROM Complaint_Nature_Master AS CN
                                INNER JOIN Complaint_Department_Master AS CD ON CD.Id = CN.Dep_Id
                                WHERE CN.Id={Id}";
                //string query = $@" SELECT * FROM Complaint_Nature_Master WHERE Id={Id} ";
                var data = _connectionService.Return(query);
                var row = data.Rows[0];
                NatureModel NatList = new NatureModel()
                {
                    Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                    Dep_Name = row["dep_name"] != DBNull.Value ? row["dep_name"].ToString() : string.Empty,
                    Dep_Id = row["dep_id"] != DBNull.Value ? Convert.ToInt32(row["dep_id"]) : 0,
                    Nature = row["Code"] != DBNull.Value ? row["nature"].ToString() : string.Empty,
                    Code = row["Code"] != DBNull.Value ? row["code"].ToString() : string.Empty,
                    Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["active"]),
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["createdDate"]) : DateTime.MinValue
                };
                return NatList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateNature(IFormCollection collection)
        {
            try
            {
                var id = collection["Id"].ToString();
                var Dep_Id = collection["Dep_Ids"].ToString();
                var Nature = collection["Natures"].ToString();
                var Code = collection["Codes"].ToString();

                string query = @"
                                UPDATE Complaint_Nature_Master
                                SET 
                                    Dep_Id = @dep,
                                    Nature = @nature,
                                    Code = @code
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);
                parameters.Add("@dep", Convert.ToInt64(Dep_Id), DbType.Int64);
                parameters.Add("@nature", Nature, DbType.String);
                parameters.Add("@code", Code, DbType.String);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteNature(int id)
        {
            try
            {
                string query = @"
                                UPDATE Complaint_Nature_Master
                                SET Active = 0
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);

                _connectionService.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
