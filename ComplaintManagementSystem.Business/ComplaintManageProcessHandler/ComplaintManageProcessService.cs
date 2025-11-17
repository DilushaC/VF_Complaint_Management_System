using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static ComplaintManagementSystem.Business.ConncetionHandler._ConnectionService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComplaintManagementSystem.Business.ComplaintManageProcessHandler
{
    public class ComplaintManageProcessService : IComplaintManageProcessService
    {
        private readonly _ConnectionService _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ComplaintManageProcessService(_ConnectionService connection, IHttpContextAccessor httpContextAccessor)
        {
            _connection = connection;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<BranchModel>> getBranchList()
        {
            try
            {
                string Query = $"SELECT * FROM Complaint_Branch_Master WHERE Active=1";
                var Data = _connection.Return(Query);
                var Row = Data.Rows[0];

                List<BranchModel> List = new List<BranchModel>();
                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    BranchModel bModel = new BranchModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Branch = BRow["Branch"].ToString(),
                        Code = BRow["Code"].ToString(),
                        Active = Convert.ToBoolean(BRow["Active"]),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    List.Add(bModel);
                }
                return List.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                string Query = $"SELECT * FROM Complaint_Department_Master WHERE Active=1 AND Status=1";
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
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");

                string Query = $"SELECT * FROM Complaint_ManageProcess";
                var Data = _connection.Return(Query);
                var newCode = Data.Rows.Count + 1;
                var Refference = "CMAF" + newCode.ToString("D5");
                var ComplaintMethod_Id = collection["ComplaintMethod_Id"].ToString();
                var Cu_Name = collection["Cus_Name"].ToString();
                var Cus_Nic = collection["Cus_Nic"].ToString();
                var Cus_Refference = collection["Cus_Refference"].ToString(); ;
                var Cus_MobileNumber = collection["Cus_MobileNumber"].ToString();
                var Dep_Id = collection["Dep_Id"].ToString();
                var Branch_Id = collection["Branch_Id"].ToString();
                var Nature_Id = collection["Nature_Id"].ToString();
                var Priority = collection["Priority"].ToString();
                var Compaint = collection["Complaint"].ToString();
                var ResolvedStatus = collection["ResolvedStatus"].ToString();
                var ResolvedRemark = collection["ResolvedRemark"].ToString();

                var query = "INSERT INTO Complaint_ManageProcess (ComplaintMethod_Id, Refference, Complaint, Cus_Name, Cus_Nic, Cus_Refference, Cus_MobileNumber, Dep_Id, Nature_Id, Branch_Id, Priority, IsSentCentral, IsSentDep, IsSentDepDateTime, Status, Active, CreatedUser, CreatedDate) " +
                    "VALUES (@comMethodId, @reff, @complaint, @cusName , @cus_Nic, @cusReff, @cusMob, @depId, @natId, @branchId, @priority, @isCentral, @isDep, @isSentDepDate, @status, @active, @createdUser, @createdDate)" +
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
                parameters.Add("branchId", Convert.ToInt64(Branch_Id), DbType.Int64);
                parameters.Add("priority", Priority, DbType.String);
                parameters.Add("isCentral", 0, DbType.Int32);
                parameters.Add("isDep", 0, DbType.Int32);
                parameters.Add("isSentDepDate", System.DateTime.Now, DbType.DateTime);
                parameters.Add("status", 1, DbType.Int32);
                parameters.Add("active", 1, DbType.Int32);
                parameters.Add("createdUser", UserName, DbType.String);
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
                depParameters.Add("forUser", UserName, DbType.String);
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
                if (ResolvedStatus == "Yes")
                {
                    ComplainResolve(ProcessId, ResolvedRemark);
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateAndSendComplaint(IFormCollection collection, IFormFile file)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");

                string Query = $"SELECT * FROM Complaint_ManageProcess";
                var Data = _connection.Return(Query);
                var newCode = Data.Rows.Count + 1;
                var Refference = "CMAF" + newCode.ToString("D5");
                var ComplaintMethod_Id = collection["ComplaintMethod_Id"].ToString();
                var Cu_Name = collection["Cus_Name"].ToString();
                var Cus_Nic = collection["Cus_Nic"].ToString();
                var Cus_Refference = collection["Cus_Refference"].ToString(); ;
                var Cus_MobileNumber = collection["Cus_MobileNumber"].ToString();
                var Dep_Id = collection["Dep_Id"].ToString();
                var Branch_Id = collection["Branch_Id"].ToString();
                var Nature_Id = collection["Nature_Id"].ToString();
                var Priority = collection["Priority"].ToString();
                var Compaint = collection["Complaint"].ToString();
                var ResolvedStatus = collection["ResolvedStatus"].ToString();
                var ResolvedRemark = collection["ResolvedRemark"].ToString();

                var query = "INSERT INTO Complaint_ManageProcess (ComplaintMethod_Id, Refference, Complaint, Cus_Name, Cus_Nic, Cus_Refference, Cus_MobileNumber, Dep_Id, Nature_Id, Branch_Id, Priority, IsSentCentral, IsSentDep, IsSentDepDateTime, Status, Active, CreatedUser, CreatedDate) " +
                    "VALUES (@comMethodId, @reff, @complaint, @cusName , @cus_Nic, @cusReff, @cusMob, @depId, @natId, @branchId, @priority, @isCentral, @isDep, @isSentDepDate, @status, @active, @createdUser, @createdDate)" +
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
                parameters.Add("branchId", Convert.ToInt64(Branch_Id), DbType.Int64);
                parameters.Add("priority", Priority, DbType.String);
                parameters.Add("isCentral", 0, DbType.Int32);
                parameters.Add("isDep", 1, DbType.Int32);
                parameters.Add("isSentDepDate", System.DateTime.Now, DbType.DateTime);

                if (ResolvedStatus == "No")
                {
                    parameters.Add("status", 2, DbType.Int32);
                }
                    
                parameters.Add("active", 1, DbType.Int32);
                parameters.Add("createdUser", UserName, DbType.String);
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
                depParameters.Add("forUser", UserName, DbType.String);
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
                if (ResolvedStatus == "Yes")
                {
                    ComplainResolve(ProcessId, ResolvedRemark);
                }

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaginationResultsModel<ComplaintMaster>> getComplaintList(int pageNumber, int pageSize, string searchString, string ComplaintMethod_Id, string priority)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");

                string whereClause = $"WHERE cmp.Active = 1 AND CreatedUser = '{UserName}' AND (cmp.IsResolved IS NULL OR cmp.IsResolved <> 1)";
                if (!string.IsNullOrEmpty(searchString))
                {
                    whereClause += " AND (cmp.Cus_Name LIKE @SearchPattern OR cmp.Refference LIKE @SearchPattern)";
                }

                if (!string.IsNullOrEmpty(ComplaintMethod_Id))
                {
                    whereClause += " AND cmp.ComplaintMethod_Id = @Method";
                }

                if (!string.IsNullOrEmpty(priority))
                {
                    whereClause += " AND cmp.Priority = @Priority";
                }

                int offset = (pageNumber - 1) * pageSize;
                int endRow = pageNumber * pageSize;

                string query = $@"
                                -- Main query with pagination via ROW_NUMBER()
                                WITH PagedResults AS (
                                    SELECT 
                                        cmp.Id,
                                        cmp.Refference,
                                        cm.Method,
                                        cmp.Complaint,
                                        cmp.CreatedUser,
                                        cb.Branch,
                                        cmp.Priority,
                                        cmp.CreatedDate,
                                        csm.Status,
                                        ROW_NUMBER() OVER (ORDER BY cmp.CreatedDate DESC) AS RowNum
                                    FROM Complaint_ManageProcess as cmp
                                    INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                    INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                    INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                    INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                    INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                    INNER JOIN Complaint_Status_Master as csm on csm.Id = cmp.Status
                                    {whereClause} -- The WHERE clause is now correctly inside the CTE
                                )
                                SELECT 
                                    Id,
                                    Refference,
                                    Method,
                                    Complaint,
                                    CreatedUser,
                                    Branch,
                                    Priority,
                                    CreatedDate,
                                    Status
                                FROM PagedResults
                                WHERE RowNum > @Offset AND RowNum <= @EndRow;

                                -- The COUNT query also needs to be updated to use the full join, just like the CTE
                                -- to ensure the count is accurate with filtering
                                SELECT COUNT(cmp.Id) 
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Status_Master as csm on csm.Id = cmp.Status
                                {whereClause};";


                var parameters = new DynamicParameters();
                parameters.Add("@Offset", offset);
                parameters.Add("@EndRow", endRow);
                if (!string.IsNullOrEmpty(searchString))
                    parameters.Add("@SearchPattern", "%" + searchString + "%");

                if (!string.IsNullOrEmpty(ComplaintMethod_Id))
                    parameters.Add("@Method", Convert.ToInt32(ComplaintMethod_Id));

                if (!string.IsNullOrEmpty(priority))
                    parameters.Add("@Priority", priority);

                return await _connection.QueryMultipleForPaginationAsync<ComplaintMaster>(query, parameters);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaginationResultsModel<ComplaintMaster>> getDepComplaintList(int pageNumber, int pageSize, string searchString, string ComplaintMethod_Id, string priority)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                var UserDep_Id = httpContext?.Session.GetString("UserDep_Id");

                string whereClause = $"WHERE cmp.Active = 1 AND cmp.Status =2 AND cmp.IsSentDep =1 AND cmp.IsSentCentral = 0 AND cmp.Dep_Id = {Convert.ToInt32(UserDep_Id)} AND (cmp.IsResolved IS NULL OR cmp.IsResolved <> 1)";
                if (!string.IsNullOrEmpty(searchString))
                {
                    whereClause += " AND (cmp.Cus_Name LIKE @SearchPattern OR cmp.Refference LIKE @SearchPattern)";
                }

                if (!string.IsNullOrEmpty(ComplaintMethod_Id))
                {
                    whereClause += " AND cmp.ComplaintMethod_Id = @Method";
                }

                if (!string.IsNullOrEmpty(priority))
                {
                    whereClause += " AND cmp.Priority = @Priority";
                }

                int offset = (pageNumber - 1) * pageSize;
                int endRow = pageNumber * pageSize;

                string query = $@"
                                -- Main query with pagination via ROW_NUMBER()
                                WITH PagedResults AS (
                                    SELECT 
                                        cmp.Id,
                                        cmp.Refference,
                                        cm.Method,
                                        cmp.Complaint,
                                        cmp.CreatedUser,
                                        cb.Branch,
                                        cmp.Priority,
                                        cmp.CreatedDate,
                                        csm.Status,
                                        ROW_NUMBER() OVER (ORDER BY cmp.CreatedDate DESC) AS RowNum
                                    FROM Complaint_ManageProcess as cmp
                                    INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                    INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                    INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                    INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                    INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                    INNER JOIN Complaint_Status_Master as csm on csm.Id = cmp.Status
                                    {whereClause} -- The WHERE clause is now correctly inside the CTE
                                )
                                SELECT 
                                    Id,
                                    Refference,
                                    Method,
                                    Complaint,
                                    CreatedUser,
                                    Branch,
                                    Priority,
                                    CreatedDate,
                                    Status
                                FROM PagedResults
                                WHERE RowNum > @Offset AND RowNum <= @EndRow;

                                -- The COUNT query also needs to be updated to use the full join, just like the CTE
                                -- to ensure the count is accurate with filtering
                                SELECT COUNT(cmp.Id) 
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Status_Master as csm on csm.Id = cmp.Status
                                {whereClause};";


                var parameters = new DynamicParameters();
                parameters.Add("@Offset", offset);
                parameters.Add("@EndRow", endRow);
                if (!string.IsNullOrEmpty(searchString))
                    parameters.Add("@SearchPattern", "%" + searchString + "%");

                if (!string.IsNullOrEmpty(ComplaintMethod_Id))
                    parameters.Add("@Method", Convert.ToInt32(ComplaintMethod_Id));

                if (!string.IsNullOrEmpty(priority))
                    parameters.Add("@Priority", priority);

                return await _connection.QueryMultipleForPaginationAsync<ComplaintMaster>(query, parameters);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ComplaintMaster> getComplainUsingId(int Id)
        {
            try
            {
                string query = @"
                                SELECT 
                                    cmp.Id,
                                    cmp.Refference,
                                    cm.Method,
                                    cmp.Complaint,
                                    cmp.Priority,
                                    cmp.CreatedDate,
                                    cmp.CreatedUser,
                                    cb.Branch,
                                    cp.Name Department,
                                    cn.Nature,
                                    cob.Branch AS ComBranch
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm ON cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Branch_Master as cob on cob.Id = cmp.Branch_Id
                                WHERE cmp.Id = @Id";

                // Use Dapper to query the single record
                var complaintDataTable = await _connection.SingleQueryReturn(query, Id);             



                var row = complaintDataTable.Rows[0];
                ComplaintMaster complainModel = new ComplaintMaster();

                complainModel.Id = Convert.ToUInt16(row["Id"]);
                complainModel.Refference = row["Refference"].ToString();
                complainModel.Method = row["Method"].ToString();
                complainModel.Complaint = row["Complaint"].ToString();
                complainModel.Priority = row["Priority"].ToString();
                complainModel.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                complainModel.CreatedUser = row["CreatedUser"].ToString();
                complainModel.Branch = row["Branch"].ToString();
                complainModel.Department = row["Department"].ToString();
                complainModel.Nature = row["Nature"].ToString();
                complainModel.ComBranch = row["ComBranch"].ToString();

                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments");
                string physicalPath = Path.Combine(rootPath, $"_{Id}.pdf");
                if (System.IO.File.Exists(physicalPath))
                {
                    // 3. Set the *virtual* path for your model (for browser access)
                    complainModel.AttachmentPath = $"/Attachments/_{Id}.pdf";
                }
                return (complainModel);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Complaint_ManageProcessModel getComplainProcessUsingId(int Id)
        {
            try
            {
                string query = $@" SELECT * FROM Complaint_ManageProcess WHERE Id={Id} ";
                var data = _connection.Return(query);
                var row = data.Rows[0];
                Complaint_ManageProcessModel comMangProcess = new Complaint_ManageProcessModel()
                {
                    Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                    ComplaintMethod_Id = row["ComplaintMethod_Id"] != DBNull.Value ? Convert.ToInt32(row["ComplaintMethod_Id"]) : 0,
                    Refference = row["Refference"] != DBNull.Value ? row["Refference"].ToString() : string.Empty,
                    Complaint = row["Complaint"] != DBNull.Value ? row["Complaint"].ToString() : string.Empty,
                    Cus_Name = row["Cus_Name"] != DBNull.Value ? row["Cus_Name"].ToString() : string.Empty,
                    Cus_Nic = row["Cus_Nic"] != DBNull.Value ? row["Cus_Nic"].ToString() : string.Empty,
                    Cus_Refference = row["Cus_Refference"] != DBNull.Value ? row["Cus_Refference"].ToString() : string.Empty,
                    Cus_MobileNumber = row["Cus_MobileNumber"] != DBNull.Value ? row["Cus_MobileNumber"].ToString() : string.Empty,
                    Dep_Id = row["Dep_Id"] != DBNull.Value ? Convert.ToInt32(row["Dep_Id"]) : 0,
                    Nature_Id = row["Nature_Id"] != DBNull.Value ? Convert.ToInt32(row["Nature_Id"]) : 0,
                    Branch_Id = row["Branch_Id"] != DBNull.Value ? Convert.ToInt32(row["Branch_Id"]) : 0,
                    Priority = row["Priority"] != DBNull.Value ? row["Priority"].ToString() : string.Empty,
                    IsSentCentral = row["IsSentCentral"] != DBNull.Value && Convert.ToBoolean(row["IsSentCentral"]),
                    IsSentCentralDateTime = row["IsSentCentralDateTime"] != DBNull.Value ? Convert.ToDateTime(row["IsSentCentralDateTime"]) : DateTime.MinValue,
                    IsSentDep = row["IsSentDep"] != DBNull.Value && Convert.ToBoolean(row["IsSentDep"]),
                    IsSentDepDateTime = row["IsSentDepDateTime"] != DBNull.Value ? Convert.ToDateTime(row["IsSentDepDateTime"]) : DateTime.MinValue,
                    ResolvedRemark = row["ResolvedRemark"] != DBNull.Value ? row["ResolvedRemark"].ToString() : string.Empty,
                    ResolvedUser = row["ResolvedUser"] != DBNull.Value ? row["ResolvedUser"].ToString() : string.Empty,
                    EditedDateTime = row["EditedDateTime"] != DBNull.Value ? Convert.ToDateTime(row["EditedDateTime"]) : DateTime.MinValue,
                    Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                    DeletedDate = row["DeletedDate"] != DBNull.Value ? Convert.ToDateTime(row["DeletedDate"]) : DateTime.MinValue,
                    DeletedUser = row["DeletedUser"] != DBNull.Value ? row["DeletedUser"].ToString() : string.Empty,
                    CreatedUser = row["CreatedUser"] != DBNull.Value ? row["CreatedUser"].ToString() : string.Empty,
                    CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue

                };


                return comMangProcess;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PaginationResultsModel<Complaint_ManageProcessModel>> getComplaissntList(int pageNumber, int pageSize, string searchString)
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

        public void UpdateComplaint(IFormCollection collection, IFormFile file)
        {
            try
            {
                var ComProcessId = collection["Id"].ToString();
                var ComplaintMethod_Id = collection["ComplaintMethod_Ids"].ToString();
                var Cu_Name = collection["Cus_Name"].ToString();
                var Cus_Nic = collection["Cus_Nic"].ToString();
                var Cus_Refference = collection["Cus_Refference"].ToString(); ;
                var Cus_MobileNumber = collection["Cus_MobileNumber"].ToString();
                var Dep_Id = collection["Dep_Id"].ToString();
                var Nature_Id = collection["Nature_Id"].ToString();
                var Priority = collection["Priority"].ToString();
                var Compaint = collection["Complaint"].ToString();
                var Branch_Id = collection["Branch_Id"].ToString();

                //string query = $@"UPDATE Complaint_ManageProcess SET ComplaintMethod_Id={Convert.ToInt32(ComplaintMethod_Id)}, Complaint={Compaint}, Cus_Name={Cu_Name}, Cus_Nic={Cus_Nic},
                //    Cus_Refference={Cus_Refference}, Cus_MobileNumber={Cus_MobileNumber}, Dep_Id={Dep_Id}, Nature_Id={Nature_Id}, Priority={Priority} WHERE Id={ComProcessId}";
                //_connection.Return(query);

                string query = @"
                                UPDATE Complaint_ManageProcess
                                SET 
                                    ComplaintMethod_Id = @ComplaintMethod_Id,
                                    Complaint = @Complaint,
                                    Cus_Name = @Cus_Name,
                                    Cus_Nic = @Cus_Nic,
                                    Cus_Refference = @Cus_Refference,
                                    Cus_MobileNumber = @Cus_MobileNumber,
                                    Dep_Id = @Dep_Id,
                                    Nature_Id = @Nature_Id,
                                    Branch = @branchId,
                                    Priority = @Priority,
                                    EditedDateTime = @EditedDateTime
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(ComProcessId), DbType.Int64);
                parameters.Add("@ComplaintMethod_Id", Convert.ToInt64(ComplaintMethod_Id), DbType.Int64);
                parameters.Add("@Complaint", Compaint, DbType.String);
                parameters.Add("@Cus_Name", Cu_Name, DbType.String);
                parameters.Add("@Cus_Nic", Cus_Nic, DbType.String);
                parameters.Add("@Cus_Refference", Cus_Refference, DbType.String);
                parameters.Add("@Cus_MobileNumber", Cus_MobileNumber, DbType.String);
                parameters.Add("@Dep_Id", Convert.ToInt64(Dep_Id), DbType.Int64);
                parameters.Add("@Nature_Id", Convert.ToInt64(Nature_Id), DbType.Int64);
                parameters.Add("branchId", Convert.ToInt64(Branch_Id), DbType.Int64);
                parameters.Add("@Priority", Priority, DbType.String);
                parameters.Add("@EditedDateTime", System.DateTime.Now, DbType.DateTime);

                _connection.ExecuteWithPara(query, parameters);

                var depQuery = $"UPDATE Complaint_Send_Departments SET Dep_Id={Convert.ToInt64(Dep_Id)} WHERE ComplaintMngProcess_Id={Convert.ToInt64(ComProcessId)} AND Active=1 AND EscalatiomMatrix=1";
                _connection.Return(depQuery);

                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Attachments");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var fileName = "_" + ComProcessId + ".pdf";
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

        public void UpdateSendComplaint(IFormCollection collection, IFormFile file)
        {
            try
            {
                var ComProcessId = collection["Id"].ToString();
                var ComplaintMethod_Id = collection["ComplaintMethod_Ids"].ToString();
                var Cu_Name = collection["Cus_Name"].ToString();
                var Cus_Nic = collection["Cus_Nic"].ToString();
                var Cus_Refference = collection["Cus_Refference"].ToString(); ;
                var Cus_MobileNumber = collection["Cus_MobileNumber"].ToString();
                var Dep_Id = collection["Dep_Id"].ToString();
                var Nature_Id = collection["Nature_Id"].ToString();
                var Priority = collection["Priority"].ToString();
                var Compaint = collection["Complaint"].ToString();
                var ResolvedStatus = collection["ResolvedStatus"].ToString();
                var ResolvedRemark = collection["ResolvedRemark"].ToString();
                var Branch_Id = collection["Branch_Id"].ToString();

                //string query = $@"UPDATE Complaint_ManageProcess SET ComplaintMethod_Id={Convert.ToInt32(ComplaintMethod_Id)}, Complaint={Compaint}, Cus_Name={Cu_Name}, Cus_Nic={Cus_Nic},
                //    Cus_Refference={Cus_Refference}, Cus_MobileNumber={Cus_MobileNumber}, Dep_Id={Dep_Id}, Nature_Id={Nature_Id}, Priority={Priority} WHERE Id={ComProcessId}";
                //_connection.Return(query);

                string query = @"
                                UPDATE Complaint_ManageProcess
                                SET 
                                    ComplaintMethod_Id = @ComplaintMethod_Id,
                                    Complaint = @Complaint,
                                    Cus_Name = @Cus_Name,
                                    Cus_Nic = @Cus_Nic,
                                    Cus_Refference = @Cus_Refference,
                                    Cus_MobileNumber = @Cus_MobileNumber,
                                    Dep_Id = @Dep_Id,
                                    Nature_Id = @Nature_Id,
                                    Branch_Id = @branchId,
                                    Priority = @Priority,
                                    EditedDateTime = @EditedDateTime,
                                    IsSentDep = @IsSentDep,
                                    IsSentDepDateTime = @IsSentDepDateTime,
                                    status = @status
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(ComProcessId), DbType.Int64);
                parameters.Add("@ComplaintMethod_Id", Convert.ToInt64(ComplaintMethod_Id), DbType.Int64);
                parameters.Add("@Complaint", Compaint, DbType.String);
                parameters.Add("@Cus_Name", Cu_Name, DbType.String);
                parameters.Add("@Cus_Nic", Cus_Nic, DbType.String);
                parameters.Add("@Cus_Refference", Cus_Refference, DbType.String);
                parameters.Add("@Cus_MobileNumber", Cus_MobileNumber, DbType.String);
                parameters.Add("@Dep_Id", Convert.ToInt64(Dep_Id), DbType.Int64);
                parameters.Add("@Nature_Id", Convert.ToInt64(Nature_Id), DbType.Int64);
                parameters.Add("branchId", Convert.ToInt64(Branch_Id), DbType.Int64);
                parameters.Add("@Priority", Priority, DbType.String);
                parameters.Add("@EditedDateTime", System.DateTime.Now, DbType.DateTime);
                parameters.Add("@IsSentDep", 1, DbType.Int32);
                parameters.Add("@IsSentDepDateTime", System.DateTime.Now, DbType.DateTime);

                if (ResolvedStatus == "No")
                {
                    parameters.Add("status", 2, DbType.Int32);
                }
                else
                {
                    parameters.Add("status", 3, DbType.Int32);
                }

                    _connection.ExecuteWithPara(query, parameters);

                var depQuery = $"UPDATE Complaint_Send_Departments SET Dep_Id={Convert.ToInt64(Dep_Id)} WHERE ComplaintMngProcess_Id={Convert.ToInt64(ComProcessId)} AND Active=1 AND EscalatiomMatrix=1";
                _connection.Return(depQuery);

                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Attachments");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var fileName = "_" + ComProcessId + ".pdf";
                    var filePath = Path.Combine(uploadsFolder, Path.GetFileName(fileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                if (ResolvedStatus == "Yes")
                {
                    ComplainResolve(Convert.ToInt16(ComProcessId), ResolvedRemark);
                }
                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateForwardToCentral(int Id, string remark)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                string query = $@" UPDATE Complaint_ManageProcess SET IsSentCentral = 1, Status = 3 , IsSentCentralDateTime = @IsSentCentralDateTime WHERE Id=@Id ";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(Id), DbType.Int64);
                parameters.Add("@IsSentCentralDateTime", System.DateTime.Now, DbType.DateTime);

                _connection.ExecuteWithPara(query, parameters);

                string dPuery = $@" SELECT * FROM Complaint_Send_Departments WHERE ComplaintMngProcess_Id={Id} ";

                var data = _connection.Return(dPuery);
                var row = data.Rows[0];
                var depSendCount = data.Rows.Count;
                var depQuery = "INSERT INTO Complaint_Send_Departments(ComplaintMngProcess_Id, Dep_Id, EscalatiomMatrix, Active, Status, ForwardUser, Remark, CreatedDate)" +
                   "VALUES (@comProccessId, @depId, @esMatrix, @active, @status, @forUser, @remark, @createdDate)";

                var depParameters = new DynamicParameters();
                depParameters.Add("comProccessId", Convert.ToInt64(Id), DbType.Int64);
                depParameters.Add("depId", Convert.ToInt64(4), DbType.Int64);
                depParameters.Add("esMatrix", depSendCount + 1, DbType.Int64);
                depParameters.Add("active", 1, DbType.Int32);
                depParameters.Add("status", 1, DbType.Int32);
                depParameters.Add("forUser", UserName, DbType.String);
                depParameters.Add("remark", remark, DbType.String);
                depParameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);
                _connection.ReturnWithPara(depQuery, depParameters);

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ComplainResolve(int Id, string Remark)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                string query = @"
                                UPDATE Complaint_ManageProcess 
                                SET 
                                    IsResolved = 1, 
                                    ResolvedDateTime = @ResolvedDateTime, 
                                    ResolvedRemark = @ResolvedRemark,
                                    Status = @Status,
                                    ResolvedUser = @ResolvedUser
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int64);
                parameters.Add("@ResolvedRemark", Remark, DbType.String);
                parameters.Add("@ResolvedDateTime", DateTime.Now, DbType.DateTime);
                parameters.Add("status", 4, DbType.Int32);
                parameters.Add("ResolvedUser", UserName, DbType.String);

                _connection.ExecuteWithPara(query, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //------------------------ Central Process ------------------------------------>   
        public async Task<PaginationResultsModel<ComplaintMaster>> getCentralComplaintList(int pageNumber, int pageSize, string searchString, string ComplaintMethod_Id, string priority)
        {
            try
            {
                string whereClause = "WHERE cmp.Active = 1";
                if (!string.IsNullOrEmpty(searchString))
                {
                    whereClause += " AND (cmp.Cus_Name LIKE @SearchPattern OR cmp.Refference LIKE @SearchPattern)";
                }

                if (!string.IsNullOrEmpty(ComplaintMethod_Id))
                {
                    whereClause += " AND cmp.ComplaintMethod_Id = @Method";
                }

                if (!string.IsNullOrEmpty(priority))
                {
                    whereClause += " AND cmp.Priority = @Priority";
                }


                int offset = (pageNumber - 1) * pageSize;
                int endRow = pageNumber * pageSize;

                string query = $@"
                                -- Main query with pagination via ROW_NUMBER()
                                WITH PagedResults AS (
                                    SELECT 
                                        cmp.Id,
                                        cmp.Refference,
                                        cm.Method,
                                        cmp.Complaint,
                                        cmp.CreatedUser,
                                        cb.Branch,
                                        cmp.Priority,
                                        cmp.CreatedDate,
                                        csm.Status,
                                        ROW_NUMBER() OVER (ORDER BY cmp.CreatedDate DESC) AS RowNum
                                    FROM Complaint_ManageProcess as cmp
                                    INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                    INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                    INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                    INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                    INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                    INNER JOIN Complaint_Status_Master as csm on csm.Id = cmp.Status
                                    {whereClause} -- The WHERE clause is now correctly inside the CTE
                                )
                                SELECT 
                                    Id,
                                    Refference,
                                    Method,
                                    Complaint,
                                    CreatedUser,
                                    Branch,
                                    Priority,
                                    CreatedDate,
                                    Status
                                FROM PagedResults
                                WHERE RowNum > @Offset AND RowNum <= @EndRow;

                                -- The COUNT query also needs to be updated to use the full join, just like the CTE
                                -- to ensure the count is accurate with filtering
                                SELECT COUNT(cmp.Id) 
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Status_Master as csm on csm.Id = cmp.Status
                                {whereClause};";


                var parameters = new DynamicParameters();
                parameters.Add("@Offset", offset);
                parameters.Add("@EndRow", endRow);
                if (!string.IsNullOrEmpty(searchString))
                    parameters.Add("@SearchPattern", "%" + searchString + "%");

                if (!string.IsNullOrEmpty(ComplaintMethod_Id))
                    parameters.Add("@Method", Convert.ToInt32(ComplaintMethod_Id));

                if (!string.IsNullOrEmpty(priority))
                    parameters.Add("@Priority", priority);

                return await _connection.QueryMultipleForPaginationAsync<ComplaintMaster>(query, parameters);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateForwardToDepartment(int Id, int Department, string remark)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                string query = $@" UPDATE Complaint_ManageProcess SET IsSentCentral = 0, Dep_Id={Department}, Status = 2 WHERE Id={Id} ";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(Id), DbType.Int64);
                parameters.Add("@IsSentCentralDateTime", System.DateTime.Now, DbType.DateTime);

                _connection.ExecuteWithPara(query, parameters);

                string dPuery = $@" SELECT * FROM Complaint_Send_Departments WHERE ComplaintMngProcess_Id={Id} ";

                var data = _connection.Return(dPuery);
                var row = data.Rows[0];
                var depSendCount = data.Rows.Count;
                var depQuery = "INSERT INTO Complaint_Send_Departments(ComplaintMngProcess_Id, Dep_Id, EscalatiomMatrix, Active, Status, ForwardUser, Remark, CreatedDate)" +
                   "VALUES (@comProccessId, @depId, @esMatrix, @active, @status, @forUser, @remark, @createdDate)";

                var depParameters = new DynamicParameters();
                depParameters.Add("comProccessId", Convert.ToInt64(Id), DbType.Int64);
                depParameters.Add("depId", Convert.ToInt64(Department), DbType.Int64);
                depParameters.Add("esMatrix", depSendCount + 1, DbType.Int64);
                depParameters.Add("active", 1, DbType.Int32);
                depParameters.Add("status", 1, DbType.Int32);
                depParameters.Add("forUser", UserName, DbType.String);
                depParameters.Add("remark", remark, DbType.String);
                depParameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);
                _connection.ReturnWithPara(depQuery, depParameters);

                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //------------------------ Dashboard ------------------------------------>   
        public async Task<DashboardComplaintCountModel> GetDashboardComplaintCounts()
        {
            try
            {
                string query = @"
                SELECT 
                MIN(cmp.CreatedDate) AS FromCreatedDate,
                MAX(cmp.CreatedDate) AS ToCreatedDate,
                COUNT(*) AS TotalCount,
                SUM(CASE WHEN cmp.IsResolved = 1 THEN 1 ELSE 0 END) AS ResolveCount,
                SUM(CASE WHEN cmp.IsResolved IS NULL OR cmp.IsResolved = 0 THEN 1 ELSE 0 END) AS PendingCount
                FROM Complaint_ManageProcess AS cmp; ";

                string query1 = @"
                SELECT 
                ct.Method, COUNT(c.Id) AS ComplaintCount
                FROM Complaint_ManageProcess c
                INNER JOIN Complaint_Method_Master ct ON c.ComplaintMethod_Id = ct.Id
                GROUP BY ct.Method
                ORDER BY ComplaintCount DESC";

                string query2 = @"
                SELECT 
                d.Id, d.Name AS DepName, COUNT(c.Id) AS ComplaintCount
                FROM Complaint_Department_Master d
                LEFT JOIN Complaint_ManageProcess c ON d.Id = c.Dep_Id
                WHERE d.Active=1 AND d.Status=1
                GROUP BY d.Id, d.Name
                ORDER BY ComplaintCount DESC";

                var dt = await _connection.SingleQueryReturn(query,0);
                if (dt.Rows.Count == 0)
                    return new DashboardComplaintCountModel();

                var row = dt.Rows[0];

                var dt1 = await _connection.SingleQueryReturn(query1, 0);
                var dt2 = await _connection.SingleQueryReturn(query2, 0);

                List<ComplaintMethodCountModel> methodCounts = new List<ComplaintMethodCountModel>();
                List<ComplaintDepartmentCountModel> DepCounts = new List<ComplaintDepartmentCountModel>();
                foreach (DataRow methodRow in dt1.Rows)
                {
                    methodCounts.Add(new ComplaintMethodCountModel
                    {
                        Method = methodRow["Method"].ToString(),
                        ComplaintCount = Convert.ToInt32(methodRow["ComplaintCount"])
                    });
                }

                foreach (DataRow methodRow in dt2.Rows)
                {
                    DepCounts.Add(new ComplaintDepartmentCountModel
                    {
                        Department = methodRow["DepName"].ToString(),
                        ComplaintCount = Convert.ToInt32(methodRow["ComplaintCount"])
                    });
                }

                return new DashboardComplaintCountModel
                {
                    TotalCount = Convert.ToInt32(row["TotalCount"]),
                    PendingCount = Convert.ToInt32(row["PendingCount"]),
                    ResolveCount = Convert.ToInt32(row["ResolveCount"]),
                    FromCreatedDate = Convert.ToDateTime(row["FromCreatedDate"]),
                    ToCreatedDate = Convert.ToDateTime(row["ToCreatedDate"]),
                    ComplaintMethodCounts = methodCounts,
                    ComplaintDepartmentCounts = DepCounts
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //------------------------ Complain History ------------------------------------>  
        public async Task<List<ComplaintMaster>> getComplainNumberList()
        {
            try
            {
                string Query = $"SELECT Id,Refference FROM Complaint_ManageProcess WHERE Active = 1 ORDER BY Refference DESC";
                var Data = _connection.Return(Query);
                var Row = Data.Rows[0];

                List<ComplaintMaster> ComplainList = new List<ComplaintMaster>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    ComplaintMaster bModel = new ComplaintMaster()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Refference = BRow["Refference"].ToString()                       
                    };
                    ComplainList.Add(bModel);
                }
                return ComplainList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Complaint_ManageProcessModel>> GetComplaintHistoryDetails(int Id)
        {
            try
            {     
                string query1 = @"
                SELECT D.Id, D.CreatedDate , Us.Name AS ForwordUser, Dep.Name AS UserDepName, D.EscalatiomMatrix AS MatrixOrder , 
                M.IsResolved, M.ResolvedDateTime, M.ResolvedRemark, ResUs.Name AS ResolvedUserName, ResDep.Name AS DepName, M.IsSentDep, 
                M.IsSentCentral, D.Remark
                FROM Complaint_Send_Departments AS D
                INNER JOIN  Complaint_ManageProcess As M ON D.ComplaintMngProcess_Id = M.Id
                INNER JOIN  Complaint_User As Us ON D.ForwardUser = us.UserName
                INNER JOIN  Complaint_Department_Master As Dep ON Us.Dep_Id = Dep.Id
                LEFT JOIN Complaint_User AS ResUs ON M.ResolvedUser = ResUs.UserName
                LEFT JOIN Complaint_Department_Master AS ResDep ON d.Dep_Id = ResDep.Id
                WHERE M.Id = @Id order by D.EscalatiomMatrix ASC";

                var Data = await _connection.SingleQueryReturn(query1, Id);

                var row = Data.Rows[0];
                Complaint_ManageProcessModel complainModel = new Complaint_ManageProcessModel();
                complainModel.Id = Convert.ToUInt16(row["Id"]);


                List<Complaint_ManageProcessModel> methodCounts = new List<Complaint_ManageProcessModel>();                
          
                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    Complaint_ManageProcessModel bModel = new Complaint_ManageProcessModel()
                    {
                        ForwordUser = BRow["ForwordUser"].ToString(),
                        Dep = BRow["UserDepName"].ToString(),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"].ToString()),
                        MatrixOrder = Convert.ToInt32(BRow["MatrixOrder"]),                       

                        IsResolved = BRow["IsResolved"] != DBNull.Value && Convert.ToBoolean(BRow["IsResolved"]),
                        ResolvedDateTime = BRow["ResolvedDateTime"] != DBNull.Value ? Convert.ToDateTime(BRow["ResolvedDateTime"]) : (DateTime?)null,
                        ResolvedRemark = BRow["ResolvedRemark"] != DBNull.Value ? BRow["ResolvedRemark"].ToString() : "",
                        ResolvedUser = BRow["ResolvedUserName"] != DBNull.Value ? BRow["ResolvedUserName"].ToString() : "",

                        DepartmentName = BRow["DepName"].ToString(),
                        IsSentDep = BRow["IsSentDep"] != DBNull.Value && Convert.ToBoolean(BRow["IsSentDep"]),
                        IsSentCentral = BRow["IsSentCentral"] != DBNull.Value && Convert.ToBoolean(BRow["IsSentCentral"]),
                        Remark = BRow["Remark"].ToString(),

                    };
                    methodCounts.Add(bModel);
                }
                return methodCounts.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //------------------------ customer inform ------------------------------------>  
        public async Task<List<CustomerNotificationMasterModel>> getNotificationList()
        {
            try
            {
                string Query = $"SELECT Id, Notification FROM Complaint_CustomerNotification_Master WHERE Active = 1 ORDER BY Notification ASC";
                var Data = _connection.Return(Query);
                var Row = Data.Rows[0];

                List<CustomerNotificationMasterModel> ComplainList = new List<CustomerNotificationMasterModel>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    CustomerNotificationMasterModel bModel = new CustomerNotificationMasterModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Notification = BRow["Notification"].ToString()
                    };
                    ComplainList.Add(bModel);
                }
                return ComplainList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateCustomerInformDetails(int ComplainNo, int NotifiID, string Complaint, bool isNotified, IFormFile file)
        {
            try
            {                        
                string query = @"
                                UPDATE Complaint_ManageProcess
                                SET 
                                IsCusNotified = @isNotified,
                                CusNotificationId = @NotifiID,
                                CusNotificationRemark = @Complaint,
                                CusNotifiedDate = @NotifiedDate
                                WHERE Id = @ComplainNo";

                var parameters = new DynamicParameters();
                parameters.Add("@ComplainNo", Convert.ToInt64(ComplainNo), DbType.Int64);
                parameters.Add("@NotifiID", Convert.ToInt64(NotifiID), DbType.Int64);
                parameters.Add("@Complaint", Complaint, DbType.String);                
                parameters.Add("@isNotified", isNotified ? 1 : 0, DbType.Byte);
                parameters.Add("@NotifiedDate", System.DateTime.Now, DbType.DateTime);

                _connection.ExecuteWithPara(query, parameters);

            

                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Attachments/Customer_Inform_Doc");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var fileName = "_" + ComplainNo + ".pdf";
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

        public async Task<List<ComplaintMaster>> getCusInfoCompNoList()
        {
            try
            {
                string Query = $"SELECT Id,Refference FROM Complaint_ManageProcess WHERE Active = 1 AND IsResolved = 1 AND IsCusNotified IS NULL ORDER BY Refference DESC";
                var Data = _connection.Return(Query);
                var Row = Data.Rows[0];

                List<ComplaintMaster> ComplainList = new List<ComplaintMaster>();

                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    ComplaintMaster bModel = new ComplaintMaster()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Refference = BRow["Refference"].ToString()
                    };
                    ComplainList.Add(bModel);
                }
                return ComplainList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
