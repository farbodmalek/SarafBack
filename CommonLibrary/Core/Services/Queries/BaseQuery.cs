using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Infrastructure.Utils;
using CommonLibrary.Infrastructure.Utils.Extensions;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CommonLibrary.Core.Services.Queries
{
    public class BaseQuery
    {
        private readonly LoginUserDTO loginedUser;
        private readonly List<string> allowedRoles = new() { "Admin", "", "", "" };
        public BaseQuery(LoginUserDTO loginedUser)
        {
            this.loginedUser = loginedUser;
        }
        protected List<int>? CheckSupervisorCodePermision(List<int>? SupervisorCodes)
        {
            if (loginedUser.ID == 0)
                return null;
            var supList = loginedUser?.BranchList?.Where(p => p.SupervisorCode.HasValue).ToList();

            if (supList?.Count > 0)
            {
                var supIds = supList.Where(p => p.BranchCode == p.SupervisorCode).Select(p => p.SupervisorCode.Value).ToList();
                if (!supIds.Any())
                    return null;
                if (SupervisorCodes?.Count > 0)
                {
                    if (SupervisorCodes.Except(supIds).Count() > 0)
                        return supIds;
                    else return SupervisorCodes;
                }
                else
                    return supIds;
            }
            else
                return SupervisorCodes;
            //var supList = loginedUser?.BranchList?.Where(p => p.SupervisorCode.HasValue).Select(p => p.SupervisorCode.Value).ToList();
            //if (supList?.Count > 0)
            //{
            //    if (SupervisorCodes?.Count > 0)
            //    {
            //        if (SupervisorCodes.Except(supList).Count() > 0)
            //            return supList;
            //        else return SupervisorCodes;
            //    }
            //    else
            //        return supList;
            //}
            //else
            //    return SupervisorCodes;
        }
        protected List<int>? CheckBranchesPermision(List<int>? Branches, List<int>? SupervisorCodes)
        {
            if (loginedUser.ID == 0)
                return null;

            if (loginedUser.BranchList?.Count > 0)
            {
                var supList = loginedUser?.BranchList?.Where(p => p.SupervisorCode.HasValue).Select(p => p.SupervisorCode.Value).ToList();
                if (Branches?.Count > 0)
                {
                    if (SupervisorCodes?.Count > 0)
                    {
                        if (SupervisorCodes.Except(supList).Count() > 0)
                        {
                            var list = this.loginedUser?.BranchList?.Where(p => SupervisorCodes.Contains(p.SupervisorCode.Value)).Select(p => p.BranchCode.Value).ToList();
                            if (list?.Count > 0)
                                return list;
                            else return loginedUser.Branches;
                        }
                        else return Branches;
                    }
                    else
                    {
                        using (IDbConnection db = new SqlConnection(GlobalConfig.Instance.ConnectionString))
                        {
                            //var sqlQuery = @$"Select SupervisorCode from dbo.TBranches where BranchCode in ({Branches.JoinToString()})";
                            var sqlQuery = @$"Select SupervisorCode from [Core].[Branches] where BranchCode in ({Branches.JoinToString()})";
                            var supIds = db.Query<int>(sqlQuery).ToList();
                            if (supIds.Except(supList).Count() > 0)
                            {
                                var list = this.loginedUser?.BranchList?.Where(p => supList.Contains(p.SupervisorCode.Value)).Select(p => p.BranchCode.Value).ToList();
                                if (list?.Count > 0)
                                    return list;
                                else return loginedUser.Branches;
                            }
                            else return Branches;
                        }
                    }
                }
                else
                    return loginedUser?.BranchList?.Where(p => p.SupervisorCode != p.BranchCode).Select(p => p.BranchCode.Value).ToList();
            }
            else
                return Branches;
        }
    }
}
