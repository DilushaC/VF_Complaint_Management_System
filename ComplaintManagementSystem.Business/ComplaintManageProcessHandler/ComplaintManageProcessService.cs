using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using Dapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComplaintManagementSystem.Business.ConncetionHandler._ConnectionService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComplaintManagementSystem.Business.ComplaintManageProcessHandler
{
    public class ComplaintManageProcessService : IComplaintManageProcessService
    {
        private readonly _ConnectionService _connection;

        public ComplaintManageProcessService(_ConnectionService connection)
        {
            _connection = connection;
        }

        public async Task<List<Complaint_Method_MasterModel>> getMethodList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Method_Master WHERE Active=1";
                var Data = _connection.Return(Query);
                var Row = Data.Rows[0];

                List<Complaint_Method_MasterModel> mthopdList = new List<Complaint_Method_MasterModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    Complaint_Method_MasterModel bModel = new Complaint_Method_MasterModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Method = BRow["Method"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    mthopdList.Add(bModel);
                }
                return mthopdList.ToList();

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
                string Query = $"SELECT * FROM Complaint_Department_Master WHERE Active=1";
                var Data = _connection.Return(Query);
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

        public List<Complaint_Nature_MasterModel> GetNaturesByDepartment(int DepId)
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Nature_Master WHERE Active=1 AND Dep_Id={DepId}";
                var Data = _connection.Return(Query);
                var Row = Data.Rows[0];

                List<Complaint_Nature_MasterModel> natureList = new List<Complaint_Nature_MasterModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    Complaint_Nature_MasterModel bModel = new Complaint_Nature_MasterModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Dep_Id = Convert.ToInt32(BRow["Dep_Id"]),
                        Nature = BRow["Nature"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    natureList.Add(bModel);
                }
                return natureList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateComplaint(IFormCollection collection, IFormFile file)
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_ManageProcess WHERE Active=1";
                var Data = _connection.Return(Query);
                var newCode = Data.Rows.Count + 1;
                var Refference = "CMAF" + newCode.ToString("D5");
                var ComplaintMethod_Id = collection["ComplaintMethod_Id"].ToString();
                var Cu_Name = collection["Cus_Name"].ToString();
                var Cus_Nic = collection["Cus_Nic"].ToString();
                var Cus_Refference = collection["Cus_Refference"].ToString(); ;
                var Cus_MobileNumber = collection["Cus_MobileNumber"].ToString();
                var Dep_Id = collection["Dep_Id"].ToString();
                var Nature_Id = collection["Nature_Id"].ToString();
                var Priority = collection["Priority"].ToString();
                var Compaint = collection["Compaint"].ToString();

                var query = "INSERT INTO Complaint_ManageProcess (ComplaintMethod_Id, Refference, Complaint, Cus_Name, Cus_Nic, Cus_Refference, Cus_MobileNumber, Dep_Id, Nature_Id, Priority, IsSentCentral, IsSentDep, IsSentDepDateTime, Status, Active, CreatedUser, CreatedDate) " +
                    "VALUES (@comMethodId, @reff, @complaint, @cusName , @cus_Nic, @cusReff, @cusMob, @depId, @natId, @priority, @isCentral, @isDep, @isSentDepDate, @status, @active, @createdUser, @createdDate)" +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new DynamicParameters();
                parameters.Add("comMethodId", Convert.ToInt64(ComplaintMethod_Id), DbType.Int64);
                parameters.Add("reff", Refference, DbType.String);
                parameters.Add("complaint", Compaint, DbType.String);
                parameters.Add("cusName", Cu_Name, DbType.String);
                parameters.Add("cus_Nic", Cus_Nic, DbType.String);
                parameters.Add("cusReff", Cus_Refference, DbType.String);
                parameters.Add("cusMob", Cus_MobileNumber, DbType.String);
                parameters.Add("depId", Convert.ToInt64(Dep_Id), DbType.Int64);
                parameters.Add("natId", Convert.ToInt64(Nature_Id), DbType.Int64);
                parameters.Add("priority", Priority, DbType.String);
                parameters.Add("isCentral", 0, DbType.Int32);
                parameters.Add("isDep", 1, DbType.Int32);
                parameters.Add("isSentDepDate", System.DateTime.Now, DbType.DateTime);
                parameters.Add("status", 1, DbType.Int32);
                parameters.Add("active", 1, DbType.Int32);
                parameters.Add("createdUser", "Kasunp", DbType.String);
                parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);

                var ProcessId = _connection.InsertAndGetId(query, parameters);

                var depQuery = "INSERT INTO Complaint_Send_Departments(ComplaintMngProcess_Id, Dep_Id, EscalatiomMatrix, Active, Status, ForwardUser, CreatedDate)" +
                    "VALUES (@comProccessId, @depId, @esMatrix, @active, @status, @forUser, @createdDate)";

                var depParameters = new DynamicParameters();
                depParameters.Add("comProccessId", Convert.ToInt64(ProcessId), DbType.Int64);
                depParameters.Add("depId", Convert.ToInt64(Dep_Id), DbType.Int64);
                depParameters.Add("esMatrix", 1, DbType.Int64);
                depParameters.Add("active", 1, DbType.Int32);
                depParameters.Add("status", 1, DbType.Int32);
                depParameters.Add("forUser", "Kasunp", DbType.String);
                depParameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);
                _connection.ReturnWithPara(depQuery, depParameters);
                //Handle file upload
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Attachments");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var fileName = "_" + ProcessId + ".pdf";
                    var filePath = Path.Combine(uploadsFolder, Path.GetFileName(fileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaginationResultsModel<Complaint_ManageProcessModel>> getComplaintList(int pageNumber, int pageSize, string searchString)
        {
            try
            {
                string whereClause = "WHERE Active = 1";
                if (!string.IsNullOrEmpty(searchString))
                {
                    whereClause += " AND (Cus_Name LIKE @SearchPattern OR Refference LIKE @SearchPattern)";
                    // Add more columns to search here
                }
                int offset = (pageNumber - 1) * pageSize;
                string query = $@" SELECT * FROM Complaint_ManageProcess {whereClause} ORDER BY CreatedDate DESC
                                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;        
                                SELECT COUNT(Id) FROM Complaint_ManageProcess {whereClause};";

                var parameters = new DynamicParameters();
                parameters.Add("@Offset", offset);
                parameters.Add("@PageSize", pageSize);
                if (!string.IsNullOrEmpty(searchString))
                {
                    parameters.Add("@SearchPattern", "%" + searchString + "%");
                }

                return await _connection.QueryMultipleForPaginationAsync<Complaint_ManageProcessModel>(query, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        

    }
}
