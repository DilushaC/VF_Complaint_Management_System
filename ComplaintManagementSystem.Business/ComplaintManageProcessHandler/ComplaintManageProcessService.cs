using ComplaignManagementSystem.Data.Context;
using ComplaignManagementSystem.Data.Models;
using ComplaintManagementSystem.Business.ConncetionHandler;
using ComplaintManagementSystem.Business.EmailHandler;
using Dapper;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using static ComplaintManagementSystem.Business.ConncetionHandler._ConnectionService;
//using static Google.Protobuf.Collections.MapField<TKey, TValue>;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComplaintManagementSystem.Business.ComplaintManageProcessHandler
{
    public class ComplaintManageProcessService : IComplaintManageProcessService
    {
        private readonly _ConnectionService _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;

        public ComplaintManageProcessService(_ConnectionService connection, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _connection = connection;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _config = config;
            _baseUrl = config["FileSettings:BaseUrl"];
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

                string Query = $"SELECT * FROM Complaint_ManageProcess WHERE Active=1";
                var Data = _connection.Return(Query);
                var newCode = Data.Rows.Count + 1;
                var Refference = "CMVF" + newCode.ToString("D5");
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

                var Officer_EPF = collection["OfficerEPF"].ToString();
                var Officer_Name = collection["OfficerName"].ToString();

                var query = "INSERT INTO Complaint_ManageProcess (ComplaintMethod_Id, OfficerEPF, OfficerName, Refference, Complaint, Cus_Name, Cus_Nic, Cus_Refference, Cus_MobileNumber, Dep_Id, Nature_Id, Branch_Id, Priority, IsSentCentral, IsSentDep, IsSentDepDateTime, Status, Active, CreatedUser, CreatedDate, IsCentralComment) " +
                    "VALUES (@comMethodId, @officerEPF, @officerName, @reff, @complaint, @cusName , @cus_Nic, @cusReff, @cusMob, @depId, @natId, @branchId, @priority, @isCentral, @isDep, @isSentDepDate, @status, @active, @createdUser, @createdDate, @isCentralComment)" +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new DynamicParameters();
                parameters.Add("comMethodId", Convert.ToInt64(ComplaintMethod_Id), DbType.Int64);
                parameters.Add("officerEPF", Officer_EPF, DbType.String);
                parameters.Add("officerName", Officer_Name, DbType.String);
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
                parameters.Add("isCentralComment", 0, DbType.Int32);

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
                //if (ResolvedStatus == "Yes")
                //{
                //    ComplainResolve(ProcessId, ResolvedRemark);
                //}

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

                string Query = $"SELECT * FROM Complaint_ManageProcess WHERE Active=1";
                var Data = _connection.Return(Query);
                var newCode = Data.Rows.Count + 1;
                var Refference = "CMVF" + newCode.ToString("D5");
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

                var Officer_EPF = collection["OfficerEPF"].ToString();
                var Officer_Name = collection["OfficerName"].ToString();

                var query = "";

                query = "INSERT INTO Complaint_ManageProcess (ComplaintMethod_Id, OfficerEPF, OfficerName, Refference, Complaint, Cus_Name, Cus_Nic, Cus_Refference, Cus_MobileNumber, Dep_Id, Nature_Id, Branch_Id, Priority, IsSentCentral, IsSentDep, IsSentDepDateTime, Status, Active, CreatedUser, CreatedDate, IsCentralComment) " +
                "VALUES (@comMethodId, @officerEPF, @officerName, @reff, @complaint, @cusName , @cus_Nic, @cusReff, @cusMob, @depId, @natId, @branchId, @priority, @isCentral, @isDep, @isSentDepDate, @status, @active, @createdUser, @createdDate, @isCentralComment)" +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new DynamicParameters();
                parameters.Add("comMethodId", Convert.ToInt64(ComplaintMethod_Id), DbType.Int64);
                parameters.Add("officerEPF", Officer_EPF, DbType.String);
                parameters.Add("officerName", Officer_Name, DbType.String);
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
                if (ResolvedStatus == "No")
                {
                    parameters.Add("status", 2, DbType.Int32);
                }

                parameters.Add("isSentDepDate", System.DateTime.Now, DbType.DateTime);
                parameters.Add("active", 1, DbType.Int32);
                parameters.Add("createdUser", UserName, DbType.String);
                parameters.Add("createdDate", System.DateTime.Now, DbType.DateTime);
                parameters.Add("isCentralComment", 0, DbType.Int32);

                var ProcessId = _connection.InsertAndGetId(query, parameters);

                var depQuery = "INSERT INTO Complaint_Send_Departments(ComplaintMngProcess_Id, Dep_Id, EscalatiomMatrix, Active, Status, ForwardUser, CreatedDate)" +
                    "VALUES (@comProccessId, @depId, @esMatrix, @active, @status, @forUser, @createdDate)";
                var depParameters = new DynamicParameters();
                depParameters.Add("comProccessId", Convert.ToInt64(ProcessId), DbType.Int64);
                depParameters.Add("depId", Convert.ToInt64(Dep_Id), DbType.Int64);
                depParameters.Add("esMatrix", 1, DbType.Int64);
                depParameters.Add("active", 1, DbType.Int32);
                if (Convert.ToInt64(Dep_Id) == 4)
                    depParameters.Add("status", 3, DbType.Int32);
                else
                    depParameters.Add("status", 2, DbType.Int32);

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
                //EmailInsert(ProcessId, "Created", "CreatedTemplate");
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
                                    cmp.Cus_Name,
	                                cmp.Cus_Nic,
	                                cmp.Cus_MobileNumber,
	                                cmp.Cus_Refference,
	                                cmp.Cus_Email,
                                    cb.Branch,
                                    cp.Name Department,
                                    cn.Nature,
                                    cob.Branch AS ComBranch,
                                    cmp.IsCentralComment,
                                    cmp.CentralComment,
                                    s.Status,
                                    cmp.IsResolved,
                                    cmp.ResolvedRemark,
                                    cmp.ResolvedUser,
                                    cmp.ResolvedDateTime
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm ON cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Branch_Master as cob on cob.Id = cmp.Branch_Id
                                INNER JOIN Complaint_Status_Master as s on s.Id = cmp.Status
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
                complainModel.Cus_Name = row["Cus_Name"].ToString();
                complainModel.Cus_Nic = row["Cus_Nic"].ToString();
                complainModel.Cus_MobileNumber = row["Cus_MobileNumber"].ToString();
                complainModel.Cus_Refference = row["Cus_Refference"].ToString();
                complainModel.IsCentralComment = Convert.ToBoolean(row["IsCentralComment"]);
                complainModel.CentralComment = row["CentralComment"].ToString();
                complainModel.Status = row["Status"].ToString();
                complainModel.ResolvedRemark = row["ResolvedRemark"].ToString();
                complainModel.ResolvedUser = row["ResolvedUser"].ToString();
                complainModel.ResolvedDateTime = row["ResolvedDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ResolvedDateTime"]);
                //complainModel.ResolvedDateTime = Convert.ToDateTime(row["ResolvedDateTime"]);
                complainModel.IsResolved = row["IsResolved"] == DBNull.Value ? false : Convert.ToBoolean(row["IsResolved"]);

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

        public async Task<List<SendDepartmentModel>> getSendDepList(int ProcessId)
        {
            try
            {
                string Query = $"SELECT SD.Id,SD.ComplaintMngProcess_Id,SD.Dep_Id,DP.Name AS DepartmentName,SD.EscalatiomMatrix,SD.Status AS Status, " +
                            $"SD.ForwardUser,SD.Remark,SD.CreatedDate FROM Complaint_Send_Departments AS SD " +
                            $"INNER JOIN Complaint_Department_Master AS DP ON DP.Id = SD.Dep_Id " +
                            $"WHERE SD.Active=1 AND SD.ComplaintMngProcess_Id={ProcessId}" +
                            $"ORDER BY SD.EscalatiomMatrix";
                var Data = _connection.Return(Query);
                List<SendDepartmentModel> depList = new List<SendDepartmentModel>();
                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    SendDepartmentModel bModel = new SendDepartmentModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        ComplaintMngProcess_Id = Convert.ToInt32(BRow["ComplaintMngProcess_Id"]),
                        Dep_Id = Convert.ToInt32(BRow["Dep_Id"]),
                        EscalatiomMatrix = Convert.ToInt32(BRow["EscalatiomMatrix"]),
                        DepartmentName = BRow["DepartmentName"].ToString(),
                        ForwardUser = BRow["ForwardUser"].ToString(),
                        Remark = BRow["Remark"].ToString(),
                        Status = Convert.ToBoolean(BRow["Status"]),
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

        public async Task<List<PendingDetailsModel>> getPendingDetailList(int ProcessId)
        {
            try
            {
                string Query = $"SELECT Id,ProcessId,Remark,CreatedDate,Active FROM Complaint_PendingDetails WHERE ProcessId={ProcessId}";
                var Data = _connection.Return(Query);
                List<PendingDetailsModel> PendingList = new List<PendingDetailsModel>();
                for (int i = 0; i < Data.Rows.Count; i++)
                {
                    var BRow = Data.Rows[i];
                    PendingDetailsModel bModel = new PendingDetailsModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Remark = BRow["Remark"].ToString(),
                        CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                    };
                    PendingList.Add(bModel);
                }
                return PendingList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Complaint_ManageProcessModel> getComplainMasterUsingId(int Id)
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
                                    cob.Branch AS ComBranch,
                                    cmp.IsCusNotified,
	                                ccn.Notification,
	                                cmp.CusNotificationRemark,
	                                cmp.CusNotifiedDate,
                                    cmp.Cus_Name,
	                                cmp.Cus_Nic,
	                                cmp.Cus_MobileNumber,
	                                cmp.Cus_Refference,
	                                cmp.Cus_Email,
	                                ru.Name ResolvedUser,
	                                cmp.ResolvedDateTime,
	                                cmp.ResolvedRemark,
                                    cmp.CentralComment,
                                    cmp.IsCentralComment
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm ON cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Branch_Master as cob on cob.Id = cmp.Branch_Id
                                INNER JOIN Complaint_CustomerNotification_Master as ccn on ccn.Id = cmp.CusNotificationId
                                INNER JOIN Complaint_User as ru on ru.UserName = cmp.ResolvedUser
                                WHERE cmp.Id = @Id";

                // Use Dapper to query the single record
                var complaintDataTable = await _connection.SingleQueryReturn(query, Id);

                Complaint_ManageProcessModel complainModel = new Complaint_ManageProcessModel();

                if (complaintDataTable.Rows.Count != 0)
                {
                    var row = complaintDataTable.Rows[0];

                    complainModel.Id = Convert.ToUInt16(row["Id"]);
                    complainModel.Refference = row["Refference"].ToString();
                    complainModel.ComplaintMethod = row["Method"].ToString();
                    complainModel.Complaint = row["Complaint"].ToString();
                    complainModel.Priority = row["Priority"].ToString();
                    complainModel.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                    complainModel.CreatedUser = row["CreatedUser"].ToString();
                    complainModel.Branch = row["Branch"].ToString();
                    complainModel.Dep = row["Department"].ToString();
                    complainModel.Nature = row["Nature"].ToString();
                    complainModel.ComBranch = row["ComBranch"].ToString();
                    complainModel.IsCusNotified = Convert.ToBoolean(row["IsCusNotified"]);
                    complainModel.CusNotification = row["Notification"].ToString();
                    complainModel.CusNotificationRemark = row["CusNotificationRemark"].ToString();
                    complainModel.CusNotifiedDate = Convert.ToDateTime(row["CusNotifiedDate"]);
                    complainModel.Cus_Name = row["Cus_Name"].ToString();
                    complainModel.Cus_Nic = row["Cus_Nic"].ToString();
                    complainModel.Cus_MobileNumber = row["Cus_MobileNumber"].ToString();
                    complainModel.Cus_Refference = row["Cus_Refference"].ToString();
                    complainModel.ResolvedUser = row["ResolvedUser"].ToString();
                    complainModel.ResolvedRemark = row["ResolvedRemark"].ToString();
                    complainModel.ResolvedDateTime = Convert.ToDateTime(row["ResolvedDateTime"]);
                    complainModel.IsCentralComment = Convert.ToBoolean(row["IsCentralComment"]);
                    complainModel.CentralComment = row["CentralComment"].ToString();

                    string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments");
                    string physicalPath = Path.Combine(rootPath, $"_{Id}.pdf");
                    if (System.IO.File.Exists(physicalPath))
                    {
                        // 3. Set the *virtual* path for your model (for browser access)
                        complainModel.AttachmentPath = $"_{Id}.pdf";
                    }

                    string CrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Attachments", "Customer_Inform_Doc");
                    string CphysicalPath = Path.Combine(CrootPath, $"_{Id}.pdf");
                    if (System.IO.File.Exists(CphysicalPath))
                    {
                        // 3. Set the *virtual* path for your model (for browser access)
                        complainModel.CAttachmentPath = $"_{Id}.pdf";
                    }
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
                string Query = $"SELECT * FROM Complaint_ManageProcess WHERE Active=1";
                var Data = _connection.Return(Query);
                var newCode = Data.Rows.Count + 1;
                var Refference = "CMVF" + newCode.ToString("D5");
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

                string query = "";

                query = @"
                          UPDATE Complaint_ManageProcess
                          SET 
                              Refference = @Refference,
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
                parameters.Add("@Refference", Refference, DbType.String);
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
                parameters.Add("status", 2, DbType.Int32);

                parameters.Add("@IsSentDepDateTime", System.DateTime.Now, DbType.DateTime);

                if (ResolvedStatus == "No")
                {
                    parameters.Add("status", 2, DbType.Int32);
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
                //EmailInsert(Id, "Resolved", "ResolvedTemplate");
                //sendEmail(Convert.ToInt16(Id));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ComplainCentralUpdate(int Id, string CentralComment)
        {
            try
            {
                string query = @"
                                UPDATE Complaint_ManageProcess 
                                SET 
                                    IsCentralComment = 1, 
                                    CentralCommentDatetime = @CentralCommentDatetime, 
                                    CentralComment = @CentralComment
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id, DbType.Int64);
                parameters.Add("@CentralComment", CentralComment, DbType.String);
                parameters.Add("@CentralCommentDatetime", DateTime.Now, DbType.DateTime);
                _connection.ExecuteWithPara(query, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ComplainCentralPendingUpdate(int Id, string CentralPendingComment)
        {
            try
            {
                var query = "INSERT INTO Complaint_PendingDetails (ProcessId, Remark, CreatedDate, Active)" +
                                    "VALUES (@pId, @remark, @createdDate, @active);";

                var parameters = new DynamicParameters();
                parameters.Add("@pId", Id, DbType.Int64);
                parameters.Add("@remark", CentralPendingComment, DbType.String);
                parameters.Add("@createdDate", System.DateTime.Now, DbType.DateTime);
                parameters.Add("@active", 1, DbType.Int32);

                _connection.ReturnWithPara(query, parameters);
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
                                WHERE RowNum > @Offset AND RowNum <= @EndRow
                                ORDER BY Refference DESC;

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
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                var UserDep_Id = Convert.ToInt32(httpContext?.Session.GetString("UserDep_Id"));
                var UserPermission = httpContext?.Session.GetString("UserPermission")?.Trim();

                string query = null;
                string query1 = null;
                string query2 = null;

                if (string.Equals(UserPermission, "Central User", StringComparison.OrdinalIgnoreCase) || string.Equals(UserPermission, "Admin", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(UserPermission, "Admin User", StringComparison.OrdinalIgnoreCase))
                {
                    query = $@"
                        SELECT 
                        MIN(cmp.CreatedDate) AS FromCreatedDate,
                        MAX(cmp.CreatedDate) AS ToCreatedDate,
                        COUNT(*) AS TotalCount,
                        SUM(CASE WHEN cmp.IsResolved = 1 THEN 1 ELSE 0 END) AS ResolveCount,
                        SUM(CASE WHEN cmp.IsResolved IS NULL OR cmp.IsResolved = 0 THEN 1 ELSE 0 END) AS PendingCount
                        FROM Complaint_ManageProcess AS cmp WHERE cmp.Active=1; ";

                    query1 = @"
                        SELECT 
                        ct.Method, COUNT(c.Id) AS ComplaintCount
                        FROM Complaint_ManageProcess c
                        INNER JOIN Complaint_Method_Master ct ON c.ComplaintMethod_Id = ct.Id
                        WHERE c.Active=1
                        GROUP BY ct.Method
                        ORDER BY ComplaintCount DESC";

                    query2 = @"
                        SELECT 
                        d.Id, d.Name AS DepName, COUNT(c.Id) AS ComplaintCount
                        FROM Complaint_Department_Master d
                        LEFT JOIN Complaint_ManageProcess c ON d.Id = c.Dep_Id
                        WHERE d.Active=1 AND d.Status=1 AND c.Active=1
                        GROUP BY d.Id, d.Name
                        ORDER BY ComplaintCount DESC";
                }
                else
                {
                    query = $@"
                        SELECT 
                        MIN(cmp.CreatedDate) AS FromCreatedDate,
                        MAX(cmp.CreatedDate) AS ToCreatedDate,
                        COUNT(*) AS TotalCount,
                        SUM(CASE WHEN cmp.IsResolved = 1 THEN 1 ELSE 0 END) AS ResolveCount,
                        SUM(CASE WHEN cmp.IsResolved IS NULL OR cmp.IsResolved = 0 THEN 1 ELSE 0 END) AS PendingCount
                        FROM Complaint_ManageProcess AS cmp WHERE cmp.Active=1 AND cmp.Dep_Id={UserDep_Id}; ";

                    query1 = $@"
                        SELECT 
                        ct.Method, COUNT(c.Id) AS ComplaintCount
                        FROM Complaint_ManageProcess c
                        INNER JOIN Complaint_Method_Master ct ON c.ComplaintMethod_Id = ct.Id
                        WHERE c.Active=1 AND c.Dep_Id={UserDep_Id}
                        GROUP BY ct.Method
                        ORDER BY ComplaintCount DESC";

                    query2 = $@"
                        SELECT 
                        d.Id, d.Name AS DepName, COUNT(c.Id) AS ComplaintCount
                        FROM Complaint_Department_Master d
                        LEFT JOIN Complaint_ManageProcess c ON d.Id = c.Dep_Id
                        WHERE d.Active=1 AND d.Status=1 AND c.Active=1 AND c.Dep_Id={UserDep_Id}
                        GROUP BY d.Id, d.Name
                        ORDER BY ComplaintCount DESC";
                }




                var dt = await _connection.SingleQueryReturn(query, 0);
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
                var Data = await Task.Run(() => _connection.Return(Query));

                List<ComplaintMaster> ComplainList = new List<ComplaintMaster>();
                if (Data != null && Data.Rows.Count > 0)
                {
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
                SELECT D.Id, M.Refference AS Refference, B.Branch AS Branch, CM.Method AS Method, D.CreatedDate , Us.Name AS ForwordUser, Dep.Name AS UserDepName, 
                N.Nature AS Nature, M.Complaint AS Complaint, D.EscalatiomMatrix AS MatrixOrder, M.Cus_Refference AS CusRefference, 
                M.IsResolved, M.ResolvedDateTime, M.ResolvedRemark, ResUs.Name AS ResolvedUserName, ResDep.Name AS DepName, M.IsSentDep, 
                M.IsSentCentral, D.Remark, S.Status
                FROM Complaint_Send_Departments AS D
                INNER JOIN  Complaint_ManageProcess As M ON D.ComplaintMngProcess_Id = M.Id
                INNER JOIN  Complaint_User As Us ON D.ForwardUser = us.UserName
                INNER JOIN  Complaint_Department_Master As Dep ON Us.Dep_Id = Dep.Id
                INNER JOIN Complaint_Nature_Master AS N ON N.Id = M.Nature_Id
                INNER JOIN Complaint_Method_Master AS CM ON CM.Id = M.ComplaintMethod_Id
                INNER JOIN Complaint_Branch_Master AS B ON B.Id = M.Branch_Id
                INNER JOIN Complaint_Status_Master AS S ON S.Id = M.Status
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
                        Refference = BRow["Refference"].ToString(),
                        Dep = BRow["UserDepName"].ToString(),
                        Nature = BRow["Nature"].ToString(),
                        Complaint = BRow["Complaint"].ToString(),
                        ComplaintMethod = BRow["Method"].ToString(),
                        Branch = BRow["Branch"].ToString(),
                        Cus_Refference = BRow["CusRefference"].ToString(),
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
                        StatusName = BRow["Status"].ToString(),

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
                var Data = await Task.Run(() => _connection.Return(Query));

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

                List<ComplaintMaster> ComplainList = new List<ComplaintMaster>();
                if (Data != null && Data.Rows.Count > 0)
                {
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
                }
                return ComplainList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ComplaintMaster>> getCusNotifiedCompNoList()
        {
            try
            {
                string Query = $"SELECT Id,Refference FROM Complaint_ManageProcess WHERE Active = 1 AND IsResolved = 1 AND IsCusNotified = 1 ORDER BY Refference DESC";
                var Data = _connection.Return(Query);

                List<ComplaintMaster> ComplainList = new List<ComplaintMaster>();
                if (Data != null && Data.Rows.Count > 0)
                {
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
                }
                return ComplainList.ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteComplaint(int id)
        {
            try
            {
                string query = @"
                                UPDATE Complaint_ManageProcess
                                SET Active = 0
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(id), DbType.Int64);

                _connection.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Complaint_ManageProcessModel>> getCreatedComplainLists()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                string whereClause = $"WHERE cmp.Active = 1 AND cmp.status = 2 AND (cmp.IsResolved IS NULL OR cmp.IsResolved <> 1)";

                string query = $@"                                
                              SELECT 
                                    cmp.Id,
                                    cmp.Refference,
                                    cm.Method,
                                    cmp.Complaint,
                                    cmp.CreatedUser,
                                    cb.Branch,
                                    ccb.Branch ComBranch,
                                    cmp.Dep_Id,
                                    cp.Name Department,
                                    cmp.Priority,
                                    cmp.CreatedDate,
                                    cmp.IsSentCentral,
                                    cmp.IsSentCentralDateTime,
                                    cmp.IsSentDep,
                                    cmp.IsSentDepDateTime,
                                    cmp.status,
                                    s.status statusName,
                                    cu.Name,
                                    cmp.Cus_Name,
                                    cmp.Cus_Email,
                                    cmp.Cus_Nic,
                                    cmp.Cus_Refference,
                                    cmp.Cus_MobileNumber,
                                    cmp.Branch_Id
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Branch_Master as ccb on ccb.Id = cmp.Branch_Id
                                INNER JOIN Complaint_Status_Master as s on s.Id = cmp.status
                                {whereClause};";

                var Data = await Task.Run(() => _connection.Return(query));
                List<Complaint_ManageProcessModel> ComplainList = new List<Complaint_ManageProcessModel>();
                if (Data != null && Data.Rows.Count > 0)
                {
                    for (int i = 0; i < Data.Rows.Count; i++)
                    {
                        var BRow = Data.Rows[i];
                        Complaint_ManageProcessModel bModel = new Complaint_ManageProcessModel()
                        {
                            Id = Convert.ToInt32(BRow["Id"]),
                            Refference = BRow["Refference"].ToString(),
                            ComplaintMethod = BRow["Method"].ToString(),
                            Complaint = BRow["Complaint"].ToString(),
                            //CreatedUser = BRow["CreatedUser"].ToString(),
                            ComBranch = BRow["Branch"].ToString(),
                            Branch = BRow["ComBranch"].ToString(),
                            Dep_Id = Convert.ToInt32(BRow["Dep_Id"]),
                            Dep = BRow["Department"].ToString(),
                            Priority = BRow["Priority"].ToString(),
                            CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                            IsSentCentral = BRow["IsSentCentral"] == DBNull.Value ? false : Convert.ToBoolean(BRow["IsSentCentral"]),
                            IsSentCentralDateTime = BRow["IsSentCentralDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(BRow["IsSentCentralDateTime"]),
                            IsSentDep = BRow["IsSentDep"] == DBNull.Value ? false : Convert.ToBoolean(BRow["IsSentDep"]),
                            IsSentDepDateTime = BRow["IsSentDepDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(BRow["IsSentDepDateTime"]),
                            Status = Convert.ToInt32(BRow["status"]),
                            StatusName = BRow["statusName"].ToString(),
                            CreatedUser = BRow["Name"].ToString(),
                            Cus_Name = BRow["Cus_Name"].ToString(),
                            Cus_Email = BRow["Cus_Email"].ToString(),
                            Cus_Nic = BRow["Cus_Nic"].ToString(),
                            Cus_Refference = BRow["Cus_Refference"].ToString(),
                            Cus_MobileNumber = BRow["Cus_MobileNumber"].ToString(),
                            Branch_Id = Convert.ToInt32(BRow["Branch_Id"]),
                        };

                        string attachmentPath = Path.Combine(
                                    Directory.GetCurrentDirectory(),
                                    _config["FileSettings:AttachmentsRootPath"]
                                );

                        string baseUrl = _baseUrl;
                        string filePath = Path.Combine(
                                            attachmentPath,
                                            $"_{bModel.Id}.pdf"
                                        );

                        if (File.Exists(filePath))
                        {
                            bModel.AttachmentPath = $"/Attachments/_{bModel.Id}.pdf";
                            bModel.downloadUrl = $"{baseUrl}/ComplaintManageProcess/DownloadAttachment?fileName=_{bModel.Id}.pdf";
                        }

                        ComplainList.Add(bModel);
                    }
                }
                return ComplainList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Complaint_ManageProcessModel>> getCreatedCentralComplainLists()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                string whereClause = $"WHERE cmp.Active = 1 AND cmp.status = 3 AND (cmp.IsResolved IS NULL OR cmp.IsResolved <> 1)";

                string query = $@"
                                
                              SELECT 
                                    cmp.Id,
                                    cmp.Refference,
                                    cm.Method,
                                    cmp.Complaint,
                                    cmp.CreatedUser,
                                    cb.Branch,
                                    ccb.Branch ComBranch,
                                    cmp.Dep_Id,
                                    cp.Name Department,
                                    cmp.Priority,
                                    cmp.CreatedDate,
                                    cmp.IsSentCentral,
                                    cmp.IsSentCentralDateTime,
                                    cmp.IsSentDep,
                                    cmp.IsSentDepDateTime,
                                    cmp.status,
                                    s.status statusName,
                                    cu.Name,
                                    cmp.Cus_Name,
                                    cmp.Cus_Email,
                                    cmp.Cus_Nic,
                                    cmp.Cus_Refference,
                                    cmp.Cus_MobileNumber,
                                    cmp.Branch_Id
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Branch_Master as ccb on ccb.Id = cmp.Branch_Id
                                INNER JOIN Complaint_Status_Master as s on s.Id = cmp.status
                                {whereClause};";

                var parameters = new DynamicParameters();
                var Data = await Task.Run(() => _connection.Return(query));
                List<Complaint_ManageProcessModel> ComplainList = new List<Complaint_ManageProcessModel>();
                if (Data != null && Data.Rows.Count > 0)
                {
                    for (int i = 0; i < Data.Rows.Count; i++)
                    {
                        var BRow = Data.Rows[i];
                        Complaint_ManageProcessModel bModel = new Complaint_ManageProcessModel()
                        {
                            Id = Convert.ToInt32(BRow["Id"]),
                            Refference = BRow["Refference"].ToString(),
                            ComplaintMethod = BRow["Method"].ToString(),
                            Complaint = BRow["Complaint"].ToString(),
                            //CreatedUser = BRow["CreatedUser"].ToString(),
                            ComBranch = BRow["Branch"].ToString(),
                            Branch = BRow["ComBranch"].ToString(),
                            Dep_Id = Convert.ToInt32(BRow["Dep_Id"]),
                            Dep = BRow["Department"].ToString(),
                            Priority = BRow["Priority"].ToString(),
                            CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                            IsSentCentral = BRow["IsSentCentral"] == DBNull.Value ? false : Convert.ToBoolean(BRow["IsSentCentral"]),
                            IsSentCentralDateTime = BRow["IsSentCentralDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(BRow["IsSentCentralDateTime"]),
                            IsSentDep = BRow["IsSentDep"] == DBNull.Value ? false : Convert.ToBoolean(BRow["IsSentDep"]),
                            IsSentDepDateTime = BRow["IsSentDepDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(BRow["IsSentDepDateTime"]),
                            Status = Convert.ToInt32(BRow["status"]),
                            StatusName = BRow["statusName"].ToString(),
                            CreatedUser = BRow["Name"].ToString(),
                            Cus_Name = BRow["Cus_Name"].ToString(),
                            Cus_Email = BRow["Cus_Email"].ToString(),
                            Cus_Nic = BRow["Cus_Nic"].ToString(),
                            Cus_Refference = BRow["Cus_Refference"].ToString(),
                            Cus_MobileNumber = BRow["Cus_MobileNumber"].ToString(),
                            Branch_Id = Convert.ToInt32(BRow["Branch_Id"]),
                        };

                        string attachmentPath = Path.Combine(
                                    Directory.GetCurrentDirectory(),
                                    _config["FileSettings:AttachmentsRootPath"]
                                );

                        string baseUrl = _baseUrl;
                        string filePath = Path.Combine(
                                            attachmentPath,
                                            $"_{bModel.Id}.pdf"
                                        );

                        if (File.Exists(filePath))
                        {
                            bModel.AttachmentPath = $"/Attachments/_{bModel.Id}.pdf";
                            bModel.downloadUrl = $"{baseUrl}/ComplaintManageProcess/DownloadAttachment?fileName=_{bModel.Id}.pdf";
                        }

                        ComplainList.Add(bModel);
                    }
                }
                return ComplainList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserModel>> getCentralResPersons()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");
                string whereClause = $"WHERE cur.Role='Central User' and u.Active=1";

                string query = $@"                                
                              SELECT u.Id, u.UserName,u.Name,u.Email FROM Complaint_User AS u
                                    INNER JOIN Complaint_User_Permission AS cup ON cup.UserId = u.Id
                                    INNER JOIN Complaint_User_Role AS cur ON cur.Id = cup.UserRoleId
                                {whereClause};";

                var parameters = new DynamicParameters();
                var Data = await Task.Run(() => _connection.Return(query));
                List<UserModel> UserList = new List<UserModel>();
                if (Data != null && Data.Rows.Count > 0)
                {
                    for (int i = 0; i < Data.Rows.Count; i++)
                    {
                        var BRow = Data.Rows[i];
                        UserModel bModel = new UserModel()
                        {
                            Id = Convert.ToInt32(BRow["Id"]),
                            UserName = BRow["UserName"].ToString(),
                            Name = BRow["Name"].ToString(),
                            Email = BRow["Email"].ToString(),
                            //CreatedUser = BRow["CreatedUser"].ToString(),

                        };
                        UserList.Add(bModel);
                    }
                }
                return UserList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Complaint_ManageProcessModel> getCreatedComplainListsId(int id)
        {
            try
            {

                string query = $@"
                                
                              SELECT 
                                    cmp.Id,
                                    cmp.Refference,
                                    cm.Method,
                                    cmp.Complaint,
                                    cmp.CreatedUser,
                                    cb.Branch,
                                    ccb.Branch ComBranch,
                                    cmp.Dep_Id,
                                    cp.Name Department,
                                    cmp.Priority,
                                    cmp.CreatedDate,
                                    cmp.IsSentCentral,
                                    cmp.IsSentCentralDateTime,
                                    cmp.IsSentDep,
                                    cmp.IsSentDepDateTime,
                                    cmp.status,
                                    s.status statusName,
                                    cu.Name,
                                    cu.Email CreatedUserEmail,
                                    ru.Name ResolvedUser,
                                    ru.Email ResolvedUserEmail,
                                    cmp.ResolvedDateTime,
                                    cmp.CusNotificationRemark,
                                    cmp.ResolvedRemark,
                                    cmp.Cus_Name,
                                    cmp.Cus_Email,
                                    cmp.Cus_Nic,
                                    cmp.Cus_Refference,
                                    cmp.Cus_MobileNumber
                                FROM Complaint_ManageProcess as cmp
                                INNER JOIN Complaint_Method_Master as cm on cm.Id = cmp.ComplaintMethod_Id
                                INNER JOIN Complaint_Department_Master as cp on cp.Id = cmp.Dep_Id
                                INNER JOIN Complaint_Nature_Master as cn on cn.Id = cmp.Nature_Id
                                INNER JOIN Complaint_User as cu on cu.UserName = cmp.CreatedUser
                                INNER JOIN Complaint_User as ru on ru.UserName = cmp.ResolvedUser
                                INNER JOIN Complaint_Branch_Master as cb on cb.Id = cu.BranchId
                                INNER JOIN Complaint_Branch_Master as ccb on ccb.Id = cmp.Branch_Id
                                INNER JOIN Complaint_Status_Master as s on s.Id = cmp.status
                                WHERE cmp.Active = 1 AND cmp.Id=@id";

                var complaintDataTable = await _connection.SingleQueryReturn(query, id);


                var row = complaintDataTable.Rows[0];
                Complaint_ManageProcessModel complainModel = new Complaint_ManageProcessModel();

                complainModel.Id = Convert.ToInt32(row["Id"]);
                complainModel.Refference = row["Refference"].ToString();
                complainModel.ComplaintMethod = row["Method"].ToString();
                complainModel.Complaint = row["Complaint"].ToString();
                //complainModel.CreatedUser = BRow["CreatedUser"].ToString();
                complainModel.ComBranch = row["Branch"].ToString();
                complainModel.Branch = row["ComBranch"].ToString();
                complainModel.Dep_Id = Convert.ToInt32(row["Dep_Id"]);
                complainModel.Dep = row["Department"].ToString();
                complainModel.Priority = row["Priority"].ToString();
                complainModel.CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
                complainModel.Status = Convert.ToInt32(row["status"]);
                complainModel.StatusName = row["statusName"].ToString();
                complainModel.CreatedUser = row["Name"].ToString();
                complainModel.CreatedUserEmail = row["CreatedUserEmail"].ToString();
                complainModel.ResolvedUser = row["ResolvedUser"].ToString();
                complainModel.ResolvedUserEmail = row["ResolvedUserEmail"].ToString();
                complainModel.ResolvedDateTime = Convert.ToDateTime(row["ResolvedDateTime"]);
                complainModel.CusNotificationRemark = row["CusNotificationRemark"].ToString();
                complainModel.ResolvedRemark = row["ResolvedRemark"].ToString();
                complainModel.Cus_Name = row["Cus_Name"].ToString();
                complainModel.Cus_Email = row["Cus_Email"].ToString();
                complainModel.Cus_Nic = row["Cus_Nic"].ToString();
                complainModel.Cus_Refference = row["Cus_Refference"].ToString();
                complainModel.Cus_MobileNumber = row["Cus_MobileNumber"].ToString();

                return (complainModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Complaint_Department_MasterModel getDepResPerson(int depId)
        {
            string query = $@" SELECT * FROM Complaint_Department_Master WHERE Id={depId} ";
            var data = _connection.Return(query);
            var row = data.Rows[0];
            Complaint_Department_MasterModel depList = new Complaint_Department_MasterModel()
            {
                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                Name = row["Name"] != DBNull.Value ? row["Name"].ToString() : string.Empty,
                Code = row["Code"] != DBNull.Value ? row["Code"].ToString() : string.Empty,
                DepHeadName = row["DepHeadName"] != DBNull.Value ? row["DepHeadName"].ToString() : string.Empty,
                DepHeadEmail = row["DepHeadEmail"] != DBNull.Value ? row["DepHeadEmail"].ToString() : string.Empty,
                DepResName = row["DepResName"] != DBNull.Value ? row["DepResName"].ToString() : string.Empty,
                DepResEmail = row["DepResEmail"] != DBNull.Value ? row["DepResEmail"].ToString() : string.Empty,
                Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                //Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]),
                CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue
            };
            return depList;
        }

        public BranchModel getBranchResPerson(int branchId)
        {
            string query = $@" SELECT * FROM Complaint_Branch_Master WHERE Id={branchId} ";
            var data = _connection.Return(query);
            var row = data.Rows[0];
            BranchModel branchModal = new BranchModel()
            {
                Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                Branch = row["Branch"] != DBNull.Value ? row["Branch"].ToString() : string.Empty,
                Code = row["Code"] != DBNull.Value ? row["Code"].ToString() : string.Empty,
                BranchEmail = row["BranchEmail"] != DBNull.Value ? row["BranchEmail"].ToString() : string.Empty,
                Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                //Status = row["Status"] != DBNull.Value && Convert.ToBoolean(row["Status"]),
                CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.MinValue
            };
            return branchModal;
        }

        public async Task<List<EmailRecipientsModel>> getCcEmails()
        {
            string query = $@" SELECT * FROM Complaint_EmailRecipients WHERE Active = 1";
            var data = _connection.Return(query);
            //var row = data.Rows[0];
            //EmailRecipientsModel emailRecipients = new EmailRecipientsModel()
            //{
            //    Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
            //    Email = row["Name"] != DBNull.Value ? row["Email"].ToString() : string.Empty,
            //    Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
            //};
            //return emailRecipients;


            List<EmailRecipientsModel> emailRecipientsList = new List<EmailRecipientsModel>();
            if (data != null && data.Rows.Count > 0)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    var BRow = data.Rows[i];
                    EmailRecipientsModel emailRecipients = new EmailRecipientsModel()
                    {
                        Id = Convert.ToInt32(BRow["Id"]),
                        Email = BRow["Email"].ToString(),
                        Status = Convert.ToInt32(BRow["Status"]),
                    };
                    emailRecipientsList.Add(emailRecipients);
                }
            }
            return emailRecipientsList.ToList();

        }

        public async Task EmailInsert(int comProcessId, string EmailType, string EmailTemplate)
        {

            try
            {
                var query = "INSERT INTO Complaint_Email (ComProcessId, EmailType, EmailTemplateName, IsSent, Active) " +
                                "VALUES (@comProcessId, @emailType, @emailTemplateName, @isSent , @active);";

                var parameters = new DynamicParameters();
                parameters.Add("comProcessId", comProcessId, DbType.Int32);
                parameters.Add("emailType", EmailType, DbType.String);
                parameters.Add("emailTemplateName", EmailTemplate, DbType.String);
                parameters.Add("isSent", 0, DbType.Int32);
                parameters.Add("active", 1, DbType.Int32);

                _connection.ReturnWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task sendEmail(int comProcessId)
        {
            Complaint_ManageProcessModel ComplaintList = getCreatedComplainListsId(comProcessId).Result;
            var ccEmails = getCcEmails();
            var resPerson = getDepResPerson(ComplaintList.Dep_Id);

            EmailRequest request = new EmailRequest
            {
                To = resPerson.DepResName,
                //To = toEmail,
                Subject = "Complaint Resolved Notification",
                TemplateName = "ResolvedTemplate",
                Model = ComplaintList, // List<ComplaintMaster>
                ccEmailsModel = ccEmails.Result.Select(a => a.Email).ToList() // List<ComplaintMaster>
            };

            await _emailService.SendAsync(request);

            return;
        }

        public async Task<List<EmailModel>> getEmails()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var UserName = httpContext?.Session.GetString("UserName");

                string query = $@"                                
                              SELECT 
                                    Id,
                                    ComProcessId,
                                    EmailType,
                                    EmailTemplateName,
                                    IsSent,
                                    Active
                                FROM Complaint_Email
                                WHERE Active=1 AND IsSent=0";

                var parameters = new DynamicParameters();
                var Data = await Task.Run(() => _connection.Return(query));
                List<EmailModel> emailList = new List<EmailModel>();
                if (Data != null && Data.Rows.Count > 0)
                {
                    for (int i = 0; i < Data.Rows.Count; i++)
                    {
                        var BRow = Data.Rows[i];
                        EmailModel bModel = new EmailModel()
                        {
                            Id = Convert.ToInt32(BRow["Id"]),
                            ComProcessId = Convert.ToInt32(BRow["ComProcessId"]),
                            EmailType = BRow["EmailType"].ToString(),
                            EmailTemplateName = BRow["EmailTemplateName"].ToString(),
                            IsSent = BRow["IsSent"] == DBNull.Value ? false : Convert.ToBoolean(BRow["IsSent"]),
                            Active = BRow["Active"] == DBNull.Value ? false : Convert.ToBoolean(BRow["Active"]),
                        };
                        emailList.Add(bModel);
                    }
                }
                return emailList.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task deleteEmail(int EmailId)
        {
            try
            {
                string query = @"
                                DELETE FROM Complaint_Email
                                WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("@Id", Convert.ToInt64(EmailId), DbType.Int64);

                _connection.ExecuteWithPara(query, parameters);
                return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<Complaint_ManageProcessModel>> GetReportData(string sDate, string eDate)
        {
            try
            {
                string whereClause = string.Empty;
                if(sDate != null && eDate != null)
                {
                    whereClause = $"WHERE CMP.IsResolved = 1 AND (@sDate IS NULL OR CAST(CMP.CreatedDate AS DATE) >= @sDate) " +
                                            $"AND (@eDate IS NULL OR CAST(CMP.CreatedDate AS DATE) <= @eDate)";
                }
                else
                {
                    whereClause = $"WHERE CMP.IsResolved = 1";
                }

                string query = $@"                                
                                SELECT CMP.Refference,CMP.CreatedDate,M.Method, CMP.Cus_Refference, CMP.Cus_MobileNumber, CMP.Cus_Name,CMP.Cus_Nic,B.Branch,DEP.Name Department, N.Code NatureCode, CMP.Complaint, 
                                CMP.ResolvedRemark, CMP.ResolvedDateTime, U.Name ResolvedUser, CMP.IsCusNotified,CN.Notification, CMP.CusNotificationRemark, CMP.CusNotifiedDate, CMP.OfficerEPF, CMP.OfficerName 
                                FROM Complaint_ManageProcess AS CMP
                                INNER JOIN Complaint_Department_Master AS DEP ON DEP.Id = CMP.Dep_Id
                                INNER JOIN Complaint_Branch_Master AS B ON B.Id = CMP.Branch_Id
                                INNER JOIN Complaint_Method_Master AS M ON M.Id = CMP.ComplaintMethod_Id
                                INNER JOIN Complaint_Nature_Master AS N ON N.Id = CMP.Nature_Id
                                INNER JOIN Complaint_User AS U ON U.UserName = CMP.ResolvedUser
                                INNER JOIN Complaint_CustomerNotification_Master AS CN ON CN.Id = CMP.CusNotificationId
                                {whereClause}
                                ORDER BY CMP.Refference";

                var parameters = new DynamicParameters();
                parameters.Add("@sDate", string.IsNullOrEmpty(sDate) ? null : sDate);
                parameters.Add("@eDate", string.IsNullOrEmpty(eDate) ? null : eDate);

                var Data = await Task.Run(() => _connection.ReturnWithPara(query, parameters));
                
                //var Data = await Task.Run(() => _connection.Return(query));
                List<Complaint_ManageProcessModel> ComplainList = new List<Complaint_ManageProcessModel>();
                if (Data != null && Data.Rows.Count > 0)
                {
                    for (int i = 0; i < Data.Rows.Count; i++)
                    {
                        var BRow = Data.Rows[i];
                        Complaint_ManageProcessModel bModel = new Complaint_ManageProcessModel()
                        {
                            //Id = Convert.ToInt32(BRow["Id"]),
                            Refference = BRow["Refference"].ToString(),
                            ComplaintMethod = BRow["Method"].ToString(),
                            Complaint = BRow["Complaint"].ToString(),
                            //CreatedUser = BRow["CreatedUser"].ToString(),
                            ComBranch = BRow["Branch"].ToString(),
                            //Branch = BRow["ComBranch"].ToString(),
                            Dep = BRow["Department"].ToString(),
                            NatureCode = BRow["NatureCode"].ToString(),
                            CreatedDate = Convert.ToDateTime(BRow["CreatedDate"]),
                            //Status = Convert.ToInt32(BRow["status"]),
                            //CreatedUser = BRow["Name"].ToString(),
                            Cus_Name = BRow["Cus_Name"].ToString(),
                            Cus_Nic = BRow["Cus_Nic"].ToString(),
                            Cus_Refference = BRow["Cus_Refference"].ToString(),
                            Cus_MobileNumber = BRow["Cus_MobileNumber"].ToString(),
                            ResolvedRemark = BRow["ResolvedRemark"].ToString(),
                            ResolvedUser = BRow["ResolvedUser"].ToString(),
                            ResolvedDateTime = Convert.ToDateTime(BRow["ResolvedDateTime"]),
                            IsCusNotified = Convert.ToBoolean(BRow["IsCusNotified"]),
                            CusNotification = BRow["Notification"].ToString(),
                            CusNotificationRemark = BRow["CusNotificationRemark"].ToString(),
                            OfficerEPF = BRow["OfficerEPF"].ToString(),
                            OfficerName = BRow["OfficerName"].ToString(),
                        };
                        ComplainList.Add(bModel);
                    }
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
