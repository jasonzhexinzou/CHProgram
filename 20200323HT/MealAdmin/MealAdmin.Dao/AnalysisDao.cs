using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XInject.Attributes;
using XFramework.XDataBase;
using XFramework.XDataBase.SqlServer;
using System.Data.SqlClient;
using MealAdmin.Entity.Enum;
using MealAdmin.Entity.View;
using XFramework.XUtil;
using System.Configuration;

namespace MealAdmin.Dao
{
    /// <summary>
    /// 订单
    /// </summary>
    public class AnalysisDao : IAnalysisDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        private static string _dbName = ConfigurationManager.AppSettings["NonPositionDBName"];


        public List<P_TA> LoadTA()
        {
            try
            {
                List<P_TA> rtnData;
                string sqlString = "SELECT DISTINCT TA as Name from dbo.P_PreApproval_COST WHERE MRTerritoryCode IS NOT NULL AND MRTerritoryCode <>'' ";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_TA>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_PreApproval> LoadRD(string sltTA)
        {
            try
            {
                List<P_PreApproval> rtnData;
                string sqlString = "SELECT DISTINCT [RDTerritoryCode] from dbo.P_PreApproval_COST WHERE [RDTerritoryCode] IS NOT NULL AND [RDTerritoryCode] <>'' and [TA] in  (" + sltTA + ") ";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
        public List<P_PreApproval> LoadRM(string sltRD)
        {
            try
            {
                List<P_PreApproval> rtnData;
                string sqlString = "SELECT DISTINCT [CostCenter] FROM dbo.P_PreApproval_COST where [CostCenter] IS NOT NULL AND [CostCenter] <>'' and [RDTerritoryCode]=" + sltRD;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
        public List<P_PreApproval> LoadDM(string sltRM)
        {
            try
            {
                List<P_PreApproval> rtnData;
                string sqlString = "SELECT DISTINCT [DMTerritoryCode] FROM dbo.P_PreApproval_COST where [DMTerritoryCode] IS NOT NULL AND [DMTerritoryCode] <>'' and  [CostCenter]=" + sltRM;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
        public List<P_PreApproval_TERRITORY> LoadCountChart(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM)
        {
            try
            {
                List<P_PreApproval_TERRITORY> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionPreAmount = string.Empty;
                string conditionPreState = string.Empty;
                string conditionRD = string.Empty;
                string conditionRM = string.Empty;
                string conditionDM = string.Empty;
                string sqlString = string.Empty;
                if (_MTBegin != null)
                {
                    conditionstartDate = " where a.MeetingDate >= '" + _MTBegin + "'  ";
                }
                if (_MTEnd != null)
                {
                    conditionendDate = " AND a.MeetingDate < '" + _MTEnd + "'  ";
                }
                if (htType == "0")
                {
                    conditionHT = " AND a.[HospitalAddress] <> N'院外' ";
                }
                if (htType == "1")
                {
                    conditionHT = " AND a.[HospitalAddress] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (PreAmount == "1")
                {
                    //非0元
                    conditionPreAmount = " AND (a.BudgetTotal > 0) ";
                }
                if (PreAmount == "0")
                {
                    //0元
                    conditionPreAmount = " AND (a.BudgetTotal = 0) ";
                }
                if (PreState == "0")
                {
                    //待审批
                    conditionPreState = " AND (a.[State] in ('0','1','3','7')) ";
                }
                if (PreState == "1")
                {
                    //审批通过
                    conditionPreState = " AND (a.[State] in ('5','6','9')) ";
                }
                if (PreState == "2")
                {
                    //审批驳回
                    conditionPreState = " AND (a.[State] in ('2','4','8')) ";
                }
                if (PreState == "3")
                {
                    //已取消
                    conditionPreState = " AND (a.[State] = 10) ";
                }
                if (sltRD != null)
                {
                    conditionRD = " AND (a.RDTerritoryCode = " + sltRD + ") ";
                }
                if (sltRM != null)
                {
                    conditionRM = " AND (a.CostCenter = " + sltRM + ") ";
                }
                if (sltDM != null)
                {
                    conditionDM = " AND (a.DMTerritoryCode = " + sltDM + ") ";
                }
                //选择全部TA
                string sqlStringTA = "SELECT a.TA as ConCode,COUNT(a.ID) as PreCount,CONVERT(DECIMAL(13,2),SUM(a.BudgetTotal)) as PrePrice FROM [P_PreApproval_COST] a  ";
                string sqlgroupStringTA = " group by a.TA  order by a.TA ";
                if (sltTA == null)
                {
                    sqlString = sqlStringTA + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + sqlgroupStringTA;
                }
                //选择TA之后全部RD
                string sqlStringRD = " SELECT a.TA,a.RDTerritoryCode as ConCode,b.MUD_ID_RD,b.TERRITORY_RD, "
                       + " case when b.TERRITORY_RD is null or b.TERRITORY_RD = '' then N'已删除' "
                       + " when b.TERRITORY_RD is not null and b.TERRITORY_RD <> '' and (b.MUD_ID_RD is null or b.MUD_ID_RD = '') then N'空岗' "
                       + " else c.[Name] end as [NAME], "
                       + " COUNT(a.ID) as PreCount,CONVERT(DECIMAL(13,2),SUM(a.BudgetTotal)) as PrePrice FROM [P_PreApproval_COST] a  "
                       + " LEFT OUTER JOIN (SELECT distinct TERRITORY_RD,MUD_ID_RD FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.RDTerritoryCode=b.TERRITORY_RD "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_RD = c.UserId ";
                string sqlgroupStringRD = " group by a.TA,a.RDTerritoryCode,case when b.TERRITORY_RD is null or b.TERRITORY_RD = '' then N'已删除' "
                       + " when b.TERRITORY_RD is not null and b.TERRITORY_RD <> '' and (b.MUD_ID_RD is null or b.MUD_ID_RD = '') then N'空岗' "
                       + " else c.[Name] end, b.MUD_ID_RD, b.TERRITORY_RD "
                       + " order by a.TA ";
                if (sltTA != null && sltRD == null)
                {
                    sqlString = sqlStringRD + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + sqlgroupStringRD;
                }
                //选择RD之后全部RM
                string sqlStringRM = "  SELECT a.TA,a.RDTerritoryCode,b.MUD_ID_RM,a.CostCenter as ConCode,b.TERRITORY_RM, "
                       + " case when b.TERRITORY_RM is null or b.TERRITORY_RM='' then N'已删除' "
                       + " when b.TERRITORY_RM is not null and b.TERRITORY_RM<>'' and (b.MUD_ID_RM is null or b.MUD_ID_RM='') then N'空岗' "
                       + " else c.[Name] end as [NAME], "
                       + " COUNT(a.ID) as PreCount,CONVERT(DECIMAL(13,2),SUM(a.BudgetTotal)) as PrePrice FROM [P_PreApproval_COST] a  "
                       + " LEFT OUTER JOIN (SELECT distinct TERRITORY_RM,MUD_ID_RM FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.CostCenter=b.TERRITORY_RM "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_RM = c.UserId ";
                string sqlgroupStringRM = " group by a.TA,a.RDTerritoryCode,b.MUD_ID_RM,a.CostCenter,b.TERRITORY_RM, "
                       + " case when b.TERRITORY_RM is null or b.TERRITORY_RM='' then N'已删除' "
                       + " when b.TERRITORY_RM is not null and b.TERRITORY_RM<>'' and (b.MUD_ID_RM is null or b.MUD_ID_RM='') then N'空岗' "
                       + " else c.[Name] end "
                       + " order by a.TA ";
                if (sltTA != null && sltRD != null && sltRM == null)
                {
                    sqlString = sqlStringRM + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + sqlgroupStringRM;
                }
                //选择RM之后全部DM
                string sqlStringDM = " SELECT a.TA,a.RDTerritoryCode,a.CostCenter,b.MUD_ID_DM,a.DMTerritoryCode as ConCode,b.TERRITORY_DM, "
                       + " case when b.TERRITORY_DM is null or b.TERRITORY_DM = '' then N'已删除' "
                       + " when b.TERRITORY_DM is not null and b.TERRITORY_DM <> '' and (b.MUD_ID_DM is null or b.MUD_ID_DM = '') then N'空岗' "
                       + " else c.[Name] end as [NAME], "
                       + " COUNT(a.ID) as PreCount,CONVERT(DECIMAL(13,2),SUM(a.BudgetTotal)) as PrePrice FROM[P_PreApproval_COST] a "
                       + " LEFT OUTER JOIN (SELECT distinct TERRITORY_DM,MUD_ID_DM FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.DMTerritoryCode=b.TERRITORY_DM "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_DM = c.UserId ";
                string sqlgroupStringDM = "  group by a.TA,a.RDTerritoryCode,b.MUD_ID_DM,b.TERRITORY_DM,a.DMTerritoryCode,a.CostCenter, "
                       + " case when b.TERRITORY_DM is null or b.TERRITORY_DM= '' then N'已删除'  "
                       + " when b.TERRITORY_DM is not null and b.TERRITORY_DM<>'' and (b.MUD_ID_DM is null or b.MUD_ID_DM= '') then N'空岗' "
                       + " else c.[Name] end "
                       + " order by a.TA ";
                if (sltTA != null && sltRD != null && sltRM != null && sltDM == null)
                {
                    sqlString = sqlStringDM + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + conditionRM + sqlgroupStringDM;
                }
                //选择DM之后全部MR
                string sqlStringMR = "SELECT a.TA,a.RDTerritoryCode,a.CostCenter,a.DMTerritoryCode,a.MRTerritoryCode as ConCode,b.MUD_ID_MR,b.TERRITORY_MR, "
                       + "  case when b.TERRITORY_MR is null or b.TERRITORY_MR = '' then N'已删除' "
                       + " when b.TERRITORY_MR is not null and b.TERRITORY_MR <> '' and (b.MUD_ID_MR is null or b.MUD_ID_MR = '') then N'空岗' "
                       + " else c.[Name] end as [NAME], "
                       + " COUNT(a.ID) as PreCount,CONVERT(DECIMAL(13,2),SUM(a.BudgetTotal)) as PrePrice FROM[P_PreApproval_COST] a "
                       + " LEFT OUTER JOIN (SELECT distinct TERRITORY_MR,MUD_ID_MR FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.MRTerritoryCode=b.TERRITORY_MR "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_MR = c.UserId ";
                string sqlgroupStringMR = "  group by a.TA,a.RDTerritoryCode,a.MRTerritoryCode,b.MUD_ID_MR,b.TERRITORY_MR,a.CostCenter,a.DMTerritoryCode, "
                       + " case when b.TERRITORY_MR is null or b.TERRITORY_MR= '' then N'已删除'  "
                       + " when b.TERRITORY_MR is not null and b.TERRITORY_MR<>'' and (b.MUD_ID_MR is null or b.MUD_ID_MR= '') then N'空岗' "
                       + "  else c.[Name] end "
                       + " order by a.TA ";
                if (sltTA != null && sltRD != null && sltRM != null && sltDM != null)
                {
                    sqlString = sqlStringMR + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + conditionRM + conditionDM + sqlgroupStringMR;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval_TERRITORY>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_PreApproval_CountAmount> ExportCountAmount(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM)
        {
            try
            {
                List<P_PreApproval_CountAmount> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionPreAmount = string.Empty;
                string conditionPreState = string.Empty;
                string conditionRD = string.Empty;
                string conditionRM = string.Empty;
                string conditionDM = string.Empty;
                string sqlString = string.Empty;
                if (_MTBegin != null)
                {
                    conditionstartDate = " where a.MeetingDate >= '" + _MTBegin + "'  ";
                }
                if (_MTEnd != null)
                {
                    conditionendDate = " AND a.MeetingDate < '" + _MTEnd + "'  ";
                }
                if (htType == "0")
                {
                    conditionHT = " AND a.[HospitalAddress] <> N'院外' ";
                }
                if (htType == "1")
                {
                    conditionHT = " AND a.[HospitalAddress] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (PreAmount == "1")
                {
                    //非0元
                    conditionPreAmount = " AND (a.BudgetTotal > 0) ";
                }
                if (PreAmount == "0")
                {
                    //0元
                    conditionPreAmount = " AND (a.BudgetTotal = 0) ";
                }
                if (PreState == "0")
                {
                    //待审批
                    conditionPreState = " AND (a.[State] in ('0','1','3','7')) ";
                }
                if (PreState == "1")
                {
                    //审批通过
                    conditionPreState = " AND (a.[State] in ('5','6','9')) ";
                }
                if (PreState == "2")
                {
                    //审批驳回
                    conditionPreState = " AND (a.[State] in ('2','4','8')) ";
                }
                if (PreState == "3")
                {
                    //已取消
                    conditionPreState = " AND (a.[State] = 10) ";
                }
                if (sltRD != null)
                {
                    conditionRD = " AND (a.RDTerritoryCode = " + sltRD + ") ";
                }
                if (sltRM != null)
                {
                    conditionRM = " AND (a.CostCenter = " + sltRM + ") ";
                }
                if (sltDM != null)
                {
                    conditionDM = " AND (a.DMTerritoryCode = " + sltDM + ") ";
                }
                //选择全部TA
                string sqlStringTA = "SELECT  m.ConCode,'' as [NAME],'' AS MUDID,sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount,  "
                    + "sum(m.PreCount) as TotalCount,CONVERT(DECIMAL(13,2),SUM(m.PrePrice)) as TotalPrice from "
                    + "(SELECT a.TA as ConCode,case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount,case when a.BudgetTotal > 0 then COUNT(a.ID)end as NonZeroPreCount, "
                    + " COUNT(a.ID) as PreCount,SUM(a.BudgetTotal) as PrePrice FROM [P_PreApproval_COST] a ";
                string sqlgroupStringTA = "  group by a.TA,a.BudgetTotal) m group by m.ConCode  order by TotalCount desc  ";
                if (sltTA == null)
                {
                    sqlString = sqlStringTA + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + sqlgroupStringTA;
                }
                //选择TA之后全部RD
                string sqlStringRD = " SELECT  m.ConCode,m.[NAME],m.MUDID,sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                       + " sum(m.PreCount) as TotalCount,CONVERT(DECIMAL(13,2),SUM(m.PrePrice)) as TotalPrice from  "
                       + " (SELECT a.TA,a.RDTerritoryCode as ConCode, case when b.TERRITORY_RD is null or b.TERRITORY_RD='' then N'已删除' "
                       + "  when b.TERRITORY_RD is not null and b.TERRITORY_RD <> '' and(b.MUD_ID_RD is null or b.MUD_ID_RD = '') then N'空岗' "
                       + "  else c.[Name] end as [NAME],b.MUD_ID_RD as MUDID,b.TERRITORY_RD, "
                       + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount,case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount,  "
                       + "  COUNT(a.ID) as PreCount,SUM(a.BudgetTotal) as PrePrice FROM [P_PreApproval_COST] a  "
                       + " LEFT OUTER JOIN  (SELECT distinct TERRITORY_RD,MUD_ID_RD FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.RDTerritoryCode=b.TERRITORY_RD "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_RD = c.UserId ";
                string sqlgroupStringRD = " group by a.TA,a.RDTerritoryCode,[NAME],b.MUD_ID_RD,b.TERRITORY_RD,a.BudgetTotal) m "
                       + " group by m.ConCode,m.[NAME],m.MUDID "
                       + " order by TotalCount desc ";
                if (sltTA != null && sltRD == null)
                {
                    sqlString = sqlStringRD + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + sqlgroupStringRD;
                }
                //选择RD之后全部RM
                string sqlStringRM = " SELECT  m.ConCode,m.[NAME],m.MUDID,  sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                       + " sum(m.PreCount) as TotalCount,CONVERT(DECIMAL(13,2),SUM(m.PrePrice)) as TotalPrice from  "
                       + " (SELECT a.TA,a.RDTerritoryCode,a.CostCenter as ConCode,b.TERRITORY_RM, case when b.TERRITORY_RM is null or b.TERRITORY_RM='' then N'已删除' "
                       + " when b.TERRITORY_RM is not null and b.TERRITORY_RM <> '' and(b.MUD_ID_RM is null or b.MUD_ID_RM = '') then N'空岗' "
                       + " else c.[Name] end as [NAME],b.MUD_ID_RM as MUDID, "
                       + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount, case when a.BudgetTotal > 0 then COUNT(a.ID)end as NonZeroPreCount, "
                       + " COUNT(a.ID) as PreCount,SUM(a.BudgetTotal) as PrePrice FROM [P_PreApproval_COST] a  "
                       + " LEFT OUTER JOIN (SELECT distinct TERRITORY_RM,MUD_ID_RM FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.CostCenter=b.TERRITORY_RM "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_RM = c.UserId ";
                string sqlgroupStringRM = " group by a.TA,a.RDTerritoryCode,a.CostCenter,b.MUD_ID_RM,b.TERRITORY_RM,[NAME],a.BudgetTotal) m "
                       + " group by m.ConCode,m.[NAME],m.MUDID "
                       + " order by TotalCount desc ";
                if (sltTA != null && sltRD != null && sltRM == null)
                {
                    sqlString = sqlStringRM + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + sqlgroupStringRM;
                }
                //选择RM之后全部DM
                string sqlStringDM = " SELECT  m.ConCode,m.[NAME],m.MUDID,sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                       + " sum(m.PreCount) as TotalCount,CONVERT(DECIMAL(13,2),SUM(m.PrePrice)) as TotalPrice from  "
                       + " ( SELECT a.TA,a.RDTerritoryCode,a.CostCenter,b.MUD_ID_DM as MUDID,a.DMTerritoryCode as ConCode,b.TERRITORY_DM, "
                       + "case when b.TERRITORY_DM is null or b.TERRITORY_DM='' then N'已删除' "
                       + " when b.TERRITORY_DM is not null and b.TERRITORY_DM <> '' and(b.MUD_ID_DM is null or b.MUD_ID_DM = '') then N'空岗' "
                       + " else c.[Name] end as [NAME],  "
                       + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount,case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount, "
                       + " COUNT(a.ID) as PreCount,SUM(a.BudgetTotal) as PrePrice FROM [P_PreApproval_COST] a "
                       + " LEFT OUTER JOIN (SELECT distinct TERRITORY_DM,MUD_ID_DM FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.DMTerritoryCode=b.TERRITORY_DM "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_DM = c.UserId ";
                string sqlgroupStringDM = "  group by a.TA,a.RDTerritoryCode,a.CostCenter,a.DMTerritoryCode,b.MUD_ID_DM,b.TERRITORY_DM,[NAME] ,a.BudgetTotal) m "
                       + " group by m.ConCode,m.[NAME],m.MUDID  "
                       + " order by TotalCount desc ";
                if (sltTA != null && sltRD != null && sltRM != null && sltDM == null)
                {
                    sqlString = sqlStringDM + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + conditionRM + sqlgroupStringDM;
                }
                //选择DM之后全部MR
                string sqlStringMR = "SELECT  m.ConCode,m.[NAME],m.MUDID, sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                       + " sum(m.PreCount) as TotalCount,CONVERT(DECIMAL(13,2),SUM(m.PrePrice)) as TotalPrice from "
                       + "  ( SELECT a.TA,a.RDTerritoryCode,a.CostCenter,a.DMTerritoryCode,a.MRTerritoryCode as ConCode,b.MUD_ID_MR as MUDID,b.TERRITORY_MR, "
                       + "  case when b.TERRITORY_MR is null or b.TERRITORY_MR='' then N'已删除'  when b.TERRITORY_MR is not null and b.TERRITORY_MR <> '' and(b.MUD_ID_MR is null or b.MUD_ID_MR = '') then N'空岗' "
                       + "  else c.[Name] end as [NAME], case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount, "
                       + " case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount, "
                       + " COUNT(a.ID) as PreCount,SUM(a.BudgetTotal) as PrePrice FROM [P_PreApproval_COST] a "
                       + " LEFT OUTER JOIN (SELECT distinct TERRITORY_MR,MUD_ID_MR FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.MRTerritoryCode=b.TERRITORY_MR "
                       + " LEFT OUTER JOIN WP_QYUSER c ON b.MUD_ID_MR = c.UserId ";
                string sqlgroupStringMR = "  group by a.TA,a.RDTerritoryCode,a.CostCenter,a.DMTerritoryCode,a.MRTerritoryCode,b.MUD_ID_MR,b.TERRITORY_MR, "
                       + "  [NAME] ,a.BudgetTotal) m  "
                       + " group by m.ConCode,m.[NAME],m.MUDID "
                       + " order by TotalCount desc ";
                if (sltTA != null && sltRD != null && sltRM != null && sltDM != null)
                {
                    sqlString = sqlStringMR + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + conditionRM + conditionDM + sqlgroupStringMR;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval_CountAmount>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_PreApproval_HospitalRanking> ExportHospitalRanking(DateTime? _MTBegin, DateTime? _MTEnd, string PreAmount, string PreState, string sltTA, string htType, string sltRD, string sltRM, string sltDM)
        {
            try
            {
                List<P_PreApproval_HospitalRanking> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionPreAmount = string.Empty;
                string conditionPreState = string.Empty;
                string conditionRD = string.Empty;
                string conditionRM = string.Empty;
                string conditionDM = string.Empty;
                string sqlString = string.Empty;
                if (_MTBegin != null)
                {
                    conditionstartDate = " where a.MeetingDate >= '" + _MTBegin + "'  ";
                }
                if (_MTEnd != null)
                {
                    conditionendDate = " AND a.MeetingDate < '" + _MTEnd + "'  ";
                }
                if (htType == "0")
                {
                    conditionHT = " AND a.[HospitalAddress] <> N'院外' ";
                }
                if (htType == "1")
                {
                    conditionHT = " AND a.[HospitalAddress] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (PreAmount == "1")
                {
                    //非0元
                    conditionPreAmount = " AND (a.BudgetTotal > 0) ";
                }
                if (PreAmount == "0")
                {
                    //0元
                    conditionPreAmount = " AND (a.BudgetTotal = 0) ";
                }
                if (PreState == "0")
                {
                    //待审批
                    conditionPreState = " AND (a.[State] in ('0','1','3','7')) ";
                }
                if (PreState == "1")
                {
                    //审批通过
                    conditionPreState = " AND (a.[State] in ('5','6','9')) ";
                }
                if (PreState == "2")
                {
                    //审批驳回
                    conditionPreState = " AND (a.[State] in ('2','4','8')) ";
                }
                if (PreState == "3")
                {
                    //已取消
                    conditionPreState = " AND (a.[State] = 10) ";
                }
                if (sltRD != null)
                {
                    conditionRD = " AND (a.RDTerritoryCode = " + sltRD + ") ";
                }
                if (sltRM != null)
                {
                    conditionRM = " AND (a.CostCenter = " + sltRM + ") ";
                }
                if (sltDM != null)
                {
                    conditionDM = " AND (a.DMTerritoryCode = " + sltDM + ") ";
                }
                //选择全部TA
                string sqlStringTA = "SELECT  m.HospitalCode,m.HospitalName,sum(m.ZeroPreCount) as ZeroCount,sum(m.NonZeroPreCount) as NonZeroCount,  "
                    + "sum(m.PreCount) as newTotalCount,CONVERT(DECIMAL(13,2),SUM(m.PrePrice)) as newTotalPrice from  "
                    + "(SELECT a.HospitalCode as HospitalCode,a.HospitalName as HospitalName, "
                    + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount, "
                    + " case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount,COUNT(a.ID) as PreCount, "
                    + " SUM(a.BudgetTotal) as PrePrice  FROM [P_PreApproval_COST] a ";
                string sqlgroupStringTA = " group by a.HospitalCode,a.HospitalName,a.BudgetTotal) m  "
                    + " group by m.HospitalCode,m.HospitalName "
                    + " order by newTotalCount desc ";
                if (sltTA == null)
                {
                    sqlString = sqlStringTA + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + sqlgroupStringTA;
                }

                string sqlStringRD = " SELECT  m.HospitalCode,m.HospitalName,sum(m.ZeroPreCount) as ZeroCount,sum(m.NonZeroPreCount) as NonZeroCount, "
                       + " sum(m.PreCount) as newTotalCount,CONVERT(DECIMAL(13,2),SUM(m.PrePrice)) as newTotalPrice from  "
                       + " (SELECT a.HospitalCode as HospitalCode,a.HospitalName as HospitalName, "
                       + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount,  "
                       + " case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount,COUNT(a.ID) as PreCount,  "
                       + " SUM(a.BudgetTotal) as PrePrice  FROM [P_PreApproval_COST] a  ";
                string sqlStringJoinRD = " LEFT OUTER JOIN (SELECT distinct TERRITORY_RD FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.RDTerritoryCode=b.TERRITORY_RD ";
                string sqlStringJoinRM = " LEFT OUTER JOIN (SELECT distinct TERRITORY_RM  FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.CostCenter=b.TERRITORY_RM ";
                string sqlStringJoinDM = " LEFT OUTER JOIN (SELECT distinct TERRITORY_DM  FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.DMTerritoryCode=b.TERRITORY_DM ";
                string sqlStringJoinMR = " LEFT OUTER JOIN (SELECT distinct TERRITORY_MR  FROM  " + _dbName + ".[dbo].[Territory_Hospital]) b on a.MRTerritoryCode=b.TERRITORY_MR ";
                string sqlgroupStringRD = " group by a.HospitalCode,a.HospitalName,a.BudgetTotal) m "
                       + " group by m.HospitalCode,m.HospitalName "
                       + " order by newTotalCount desc ";
                //选择TA之后全部RD
                if (sltTA != null && sltRD == null)
                {
                    sqlString = sqlStringRD + sqlStringJoinRD + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + sqlgroupStringRD;
                }
                //选择RD之后全部RM                
                if (sltTA != null && sltRD != null && sltRM == null)
                {
                    sqlString = sqlStringRD + sqlStringJoinRM + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + sqlgroupStringRD;
                }
                //选择RM之后全部DM               
                if (sltTA != null && sltRD != null && sltRM != null && sltDM == null)
                {
                    sqlString = sqlStringRD + sqlStringJoinDM + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + conditionRM + sqlgroupStringRD;
                }
                //选择DM之后全部MR              
                if (sltTA != null && sltRD != null && sltRM != null && sltDM != null)
                {
                    sqlString = sqlStringRD + sqlStringJoinMR + conditionstartDate + conditionendDate + conditionHT + conditionPreAmount + conditionPreState + conditionTA + conditionRD + conditionRM + conditionDM + sqlgroupStringRD;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval_HospitalRanking>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_PreApproval_LIST_VIEW> LoadCountList(string meetingdate, string HTType, string PreAmount, string PreState, string sltTA)
        {
            try
            {
                List<P_PreApproval_LIST_VIEW> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionPreAmount = string.Empty;
                string conditionPreState = string.Empty;
                string sqlString = string.Empty;
                conditionDate = " where MeetingDate >= DATEADD(month, datediff(month, 0, '" + meetingdate + "'), 0) AND MeetingDate <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + meetingdate + "'), 0))";
                if (HTType == "0")
                {
                    conditionHT = " AND [HospitalAddress] <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionHT = " AND [HospitalAddress] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (TA in (" + sltTA + ")) ";
                }
                if (PreAmount == "1")
                {
                    //非0元
                    conditionPreAmount = " AND (BudgetTotal > 0) ";
                }
                if (PreAmount == "0")
                {
                    //0元
                    conditionPreAmount = " AND (BudgetTotal = 0) ";
                }
                if (PreState == "0")
                {
                    //待审批
                    conditionPreState = " AND ([State] in ('0','1','3','7')) ";
                }
                if (PreState == "1")
                {
                    //审批通过
                    conditionPreState = " AND ([State] in ('5','6','9')) ";
                }
                if (PreState == "2")
                {
                    //审批驳回
                    conditionPreState = " AND ([State] in ('2','4','8')) ";
                }
                if (PreState == "3")
                {
                    //已取消
                    conditionPreState = " AND ([State] = 10) ";
                }
                sqlString = "SELECT ID,ApplierMUDID,TA,HospitalCode,BudgetTotal,CASE WHEN HospitalAddress = N'院外' THEN 2 ELSE 1 END AS HTType "
                                   + "  FROM [P_PreApproval_COST] "
                                   + conditionDate
                                   + conditionHT
                                   + conditionPreAmount
                                   + conditionPreState
                                   + conditionTA
                                   + " order by TA ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval_LIST_VIEW>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_PreApproval_Hospital_VIEW> LoadHospital(string meetingdate)
        {
            try
            {
                List<P_PreApproval_Hospital_VIEW> rtnData;
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                string conditionDate = string.Empty;
                string sqlString = string.Empty;
                conditionDate = " where [CopyDate] >= DATEADD(month, datediff(month, 0, '" + meetingdate + "'), 0) AND [CopyDate] <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + meetingdate + "'), 0))";

                sqlString = "SELECT [HospitalCode],[Address] "
                                   + "  FROM [P_HOSPITAL_COST] "
                                   + conditionDate
                                   + " and [MainAddress]=N'主地址' and [IsDelete]=0 ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval_Hospital_VIEW>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_MARKET_TA> LoadMarketTA(int hTType)
        {
            try
            {
                List<P_MARKET_TA> rtnData;
                string sqlString = "SELECT ta.market AS Market, STUFF( ( SELECT ',' + CONVERT(VARCHAR(100), b.ta, 25) FROM P_Market_TA b WHERE HTType =" + hTType.ToString() + " AND ta.market = b.market FOR XML PATH(''), TYPE"
                    + ").value('.', 'NVARCHAR(MAX)'),1,1,'' ) AS TAS, OrderIndex FROM ( SELECT a.market, a.orderindex FROM dbo.P_Market_TA a WHERE HTType = " + hTType.ToString() + " GROUP BY a.market, orderindex ) ta ORDER BY orderindex";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_MARKET_TA>(sqlString, new SqlParameter[] {
                    });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_ORDER_OVERVIEW> LoadOrderview(string beginDate, string endDate, int hTType)
        {
            try
            {
                List<P_ORDER_OVERVIEW> rtnData;
                string sqlString = string.Empty;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                sqlString = " SELECT A.Mon AS [Month],ISNULL(B.TA,'') AS TA,ISNULL(B.OrderCnt,0) AS OrderCnt, CAST(ISNULL(B.OrderPrice, 0) AS FLOAT)/1000 AS OrderPrice FROM ( SELECT 1 AS Mon UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 "
                            + " UNION SELECT 9 UNION SELECT 10 UNION SELECT 11 UNION SELECT 12 ) A"
                            + " LEFT JOIN ( SELECT DATEPART(MONTH,P_ORDER_COST.DeliverTime) AS [Month],TA,COUNT(ID) AS OrderCnt,SUM( CASE WHEN P_ORDER_COST.GSKConfirmAmount is not null THEN P_ORDER_COST.GSKConfirmAmount "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                            + " WHEN P_ORDER_COST.GSKConfirmAmount is null and P_ORDER_COST.XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END ) AS OrderPrice FROM dbo.P_ORDER_COST P_ORDER_COST"
                            + " WHERE State NOT IN(5,11) AND P_ORDER_COST.DeliverTime>=@beginDate AND P_ORDER_COST.DeliverTime<=@endDate ";
                if (hTType == 1)
                {
                    sqlString += " AND P_ORDER_COST.Address <> N'院外'";
                }
                else if (hTType == 2)
                {
                    sqlString += " AND P_ORDER_COST.Address = N'院外'";
                }
                else if (hTType == 3)
                {
                    sqlString = string.Empty;
                    sqlString = " SELECT A.Mon AS [Month],ISNULL(B.TA,'') AS TA,ISNULL(B.OrderCnt,0) AS OrderCnt, CAST(ISNULL(B.OrderPrice, 0) AS FLOAT)/1000 AS OrderPrice FROM ( SELECT 1 AS Mon UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 "
                            + " UNION SELECT 9 UNION SELECT 10 UNION SELECT 11 UNION SELECT 12 ) A"
                            + " LEFT JOIN ( SELECT DATEPART(MONTH,DeliverTime) AS [Month],TA,COUNT(ID) AS OrderCnt,SUM( CASE WHEN P_ORDER.XmsTotalPrice >=0 THEN P_ORDER.XmsTotalPrice else P_ORDER.TotalPrice END ) AS OrderPrice "
                            + " FROM dbo.P_ORDER P_ORDER"
                            + " WHERE State NOT IN(5,11) AND P_ORDER.DeliverTime>=@beginDate AND P_ORDER.DeliverTime<=@endDate ";

                }
                sqlString += " GROUP BY DATEPART(MONTH,DeliverTime), TA ) B ON A.Mon = B.Month ORDER BY A.Mon";
                if (hTType == 3)
                {
                    sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_OVERVIEW>(sqlString, new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@beginDate", beginDate),
                        SqlParameterFactory.GetSqlParameter("@endDate", endDate)
                    });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_MARKET_TA_VIEW> LoadMarketTAData()
        {
            try
            {
                List<P_MARKET_TA_VIEW> rtnData;
                string sqlString = "SELECT * FROM P_Market_TA ";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_MARKET_TA_VIEW>(sqlString, new SqlParameter[] {
                    });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public int SaveGroupSetting(string name, List<string> taList, int htType, int cnt, string oldMarket)
        {
            try
            {
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction();
                    #region delete
                    SqlCommand commandDelete = new SqlCommand(" DELETE FROM [P_Market_TA] WHERE Market = '" + oldMarket + "' AND HTType = '" + htType.ToString() + "'",
                                            conn);
                    commandDelete.Transaction = tran;
                    commandDelete.Parameters.AddRange(new SqlParameter[] { });
                    commandDelete.ExecuteNonQuery();
                    #endregion

                    #region insert 
                    foreach (var item in taList)
                    {
                        SqlCommand commandAdd = new SqlCommand(" INSERT INTO [P_Market_TA] ([Market],[TA],[HTType],[OrderIndex]) VALUES('" + name + "','" + item + "','" + htType.ToString() + "','" + cnt.ToString() + "')",
                                conn);
                        commandAdd.Transaction = tran;
                        commandAdd.Parameters.AddRange(new SqlParameter[] { });
                        commandAdd.ExecuteNonQuery();
                    }
                    #endregion
                    tran.Commit();
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.Error("SaveGroupSetting" + ex);
                return 0;
            }
        }

        public int DeleteGroupSetting(string market, int hTType)
        {
            try
            {
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    var nowDate = DateTime.Now;
                    int res = 0;
                    res = sqlServerTemplate.ExecuteNonQuery("DELETE FROM [P_Market_TA] WHERE Market = '" + market + "' AND HTType = '" + hTType.ToString() + "'",
                            new SqlParameter[] { });
                    return res;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return 0;
            }
        }

        public List<P_TA> LoadTAInOrderCost()
        {
            try
            {
                List<P_TA> rtnData;
                string sqlString = "SELECT DISTINCT A.TA AS Name FROM dbo.P_ORDER_COST A INNER JOIN dbo.P_PreApproval_COST B ON A.CN = B.HTCode WHERE B.MRTerritoryCode IS NOT NULL AND B.MRTerritoryCode <>'' " +
                    " UNION SELECT TERRITORY_TA FROM " + _dbName + ".dbo.Territory_Hospital_COST";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_TA>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_ORDER_LIST_VIEW> LoadOrderList(string date, string hTType)
        {
            try
            {
                string conditionDate = " AND DeliverTime >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND DeliverTime <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";
                string conditionType = string.Empty;
                if (hTType == "1") //院内HT
                {
                    conditionType = " AND Address <> N'院外'";
                }
                else if (hTType == "2")
                {
                    conditionType = " AND Address = N'院外'";
                }
                List<P_ORDER_LIST_VIEW> rtnData;
                string sqlString = "SELECT ID,TA,UserId,HospitalId,CASE WHEN Address = N'院外' THEN 2 ELSE 1 END AS HTType, CASE WHEN GSKConfirmAmount is not null THEN GSKConfirmAmount "
                                 + " WHEN GSKConfirmAmount is null and XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                                 + " WHEN GSKConfirmAmount is null and XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END  AS GSKConfirmAmount FROM dbo.P_ORDER_COST WHERE State NOT IN(5,11) AND 1=1 "
                                 + conditionDate
                                 + conditionType;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_LIST_VIEW>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<Territory_Hospital> LoadTAHospitalList(string date)
        {
            try
            {
                List<Territory_Hospital> rtnData;
                string conditionDate = " AND CopyDate >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND CopyDate <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";
                string sqlString = "SELECT * from [Territory_Hospital_COST] where TERRITORY_TA is not null and TERRITORY_TA <> '' " + conditionDate;
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<Territory_Hospital>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_RES_HOSPITAL> LoadResHospital(string v)
        {
            try
            {
                List<P_RES_HOSPITAL> rtnData;
                string rangTable = "dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_" + v.Replace("-", "");
                string rangTableNull = "dbo.P_HOSPITAL_RANGE_RESTAURANT_COST_EMPTY";
                string sqlString = " IF OBJECT_ID('" + rangTable + "','u') IS NOT NULL"
                                + " SELECT GskHospital,COUNT(DISTINCT ResId) AS ResIdCnt FROM ( SELECT CASE WHEN CHARINDEX('_', GskHospital)>0 THEN SUBSTRING(GskHospital,0,charindex('_',GskHospital)) ELSE GskHospital END AS GskHospital,ResId FROM " + rangTable + " ) a GROUP BY a.GskHospital"
                                + " ELSE "
                                + " SELECT GskHospital,COUNT(DISTINCT ResId) AS ResIdCnt FROM ( SELECT CASE WHEN CHARINDEX('_', GskHospital)>0 THEN SUBSTRING(GskHospital,0,charindex('_',GskHospital)) ELSE GskHospital END AS GskHospital,ResId FROM " + rangTableNull + " ) a GROUP BY a.GskHospital";
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_RES_HOSPITAL>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return new List<P_RES_HOSPITAL>();
            }
        }

        public List<Territory_Hospital> LoadTAHospitalListByTA(string date, string ta, string hTType)
        {
            try
            {
                List<Territory_Hospital> rtnData;
                string conditionTA = string.Empty;
                string conditionType = string.Empty;
                if (!string.IsNullOrEmpty(ta))
                    conditionTA = "AND TERRITORY_TA in (" + ta + ")";
                if (hTType == "1") //院内HT
                {
                    conditionType = " AND B.Address <> N'院外' AND B.MainAddress = N'主地址'";
                }
                else if (hTType == "2")
                {
                    conditionType = " AND B.Address = N'院外' AND B.MainAddress = N'主地址'";
                }
                string conditionDate = " AND A.CopyDate >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND A.CopyDate <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";
                string sqlString = "SELECT A.*, B.GskHospital as GskHospitalView from [Territory_Hospital_COST] A LEFT JOIN dbo.P_HOSPITAL_COST B ON A.HospitalCode = REPLACE(B.GskHospital,'-OH','')"
                                + " where TERRITORY_TA is not null and TERRITORY_TA <> '' " + conditionDate + conditionTA + conditionType;
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<Territory_Hospital>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_ORDER_LIST_VIEW> LoadOrderListByTA(string date, string hTType, string sltTA)
        {
            try
            {
                string conditionDate = " AND DeliverTime >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND DeliverTime <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";
                string conditionType = string.Empty;
                string conditionTA = string.Empty;
                if (!string.IsNullOrEmpty(sltTA))
                    conditionTA = "AND TA in (" + sltTA + ")";
                if (hTType == "1") //院内HT
                {
                    conditionType = " AND Address <> N'院外'";
                }
                else if (hTType == "2")
                {
                    conditionType = " AND Address = N'院外'";
                }
                List<P_ORDER_LIST_VIEW> rtnData;
                string sqlString = "SELECT ID,TA,UserId,HospitalId,CASE WHEN Address = N'院外' THEN 2 ELSE 1 END AS HTType, CASE WHEN GSKConfirmAmount is not null THEN GSKConfirmAmount "
                                 + " WHEN GSKConfirmAmount is null and XmsTotalPrice < 0 THEN P_ORDER_COST.TotalPrice "
                                 + " WHEN GSKConfirmAmount is null and XmsTotalPrice >= 0 THEN P_ORDER_COST.XmsTotalPrice END  AS GSKConfirmAmount FROM dbo.P_ORDER_COST WHERE State NOT IN(5,11) AND 1=1 "
                                 + conditionDate
                                 + conditionType
                                 + conditionTA;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_LIST_VIEW>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }

        public List<P_ORDER_TERRITORY> LoadOrderChart(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM)
        {
            try
            {
                List<P_ORDER_TERRITORY> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionOrderState = string.Empty;
                string conditionRD = string.Empty;
                string conditionRM = string.Empty;
                string conditionDM = string.Empty;
                string sqlString = string.Empty;
                if (dTBegin != null)
                {
                    conditionstartDate = " where a.DeliverTime >= '" + dTBegin + "'  ";
                }
                if (dTEnd != null)
                {
                    conditionendDate = " AND a.DeliverTime <= '" + dTEnd + "'  ";
                }
                if (hTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (hTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (orderState != null && orderState != "")
                {
                    if(orderState == "ALL")
                    {
                        conditionOrderState = " AND (a.[State] in ('5','6','7','8','9','11','12')) ";
                    }
                    else
                    {
                        string[] con = orderState.Split(',').ToArray();
                        conditionOrderState = " AND ( 1<>1 ";
                        foreach (string str in con)
                        {
                            if (str == "订单完成")
                            {
                                conditionOrderState += " OR (a.[State] in ('6','7','9') and a.IsRetuen<>3) ";
                            }
                            if (str == "退单成功")
                            {
                                conditionOrderState += " OR (a.[State] in ('11')) ";
                            }
                            if (str == "退单失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('12') OR a.IsRetuen=3 ) ";
                            }
                            if (str == "预定失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('5')) ";
                            }
                            if (str == "未送达")
                            {
                                conditionOrderState += " OR (a.[State] in ('8')) ";
                            }
                        }
                        conditionOrderState += " )";
                    }
                }
                if (sltRD != null)
                {
                    conditionRD = " AND (b.RDTerritoryCode = " + sltRD + ") ";
                }
                if (sltRM != null)
                {
                    conditionRM = " AND (b.CostCenter = " + sltRM + ") ";
                }
                if (sltDM != null)
                {
                    conditionDM = " AND (b.DMTerritoryCode = " + sltDM + ") ";
                }
                //选择全部TA
                string sqlStringTA = "SELECT a.TA as ConCode,COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                                    + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END ))"
                                    + " as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode ";
                string sqlgroupStringTA = " GROUP BY a.TA ORDER BY a.TA ";
                if (sltTA == null)
                {
                    sqlString = sqlStringTA + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + sqlgroupStringTA;
                }
                //选择TA之后全部RD
                string sqlStringRD = " SELECT a.TA,b.RDTerritoryCode as ConCode,c.MUD_ID_RD, case when c.TERRITORY_RD is null or c.TERRITORY_RD = '' then N'已删除' "
                       + " when c.TERRITORY_RD is not null and c.TERRITORY_RD <> '' and (c.MUD_ID_RD is null or c.MUD_ID_RD = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_RD,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.RDTerritoryCode = c.TERRITORY_RD  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_RD = d.UserId  ";
                string sqlgroupStringRD = " group by a.TA,b.RDTerritoryCode,[NAME], c.MUD_ID_RD, c.TERRITORY_RD order by a.TA  ";
                if (sltTA != null && sltRD == null)
                {
                    sqlString = sqlStringRD + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + sqlgroupStringRD;
                }
                //选择RD之后全部RM
                string sqlStringRM = " SELECT a.TA,b.RDTerritoryCode, c.MUD_ID_RD,b.CostCenter as ConCode,c.MUD_ID_RM, case when c.TERRITORY_RM is null or c.TERRITORY_RM = '' then N'已删除' "
                       + " when c.TERRITORY_RM is not null and c.TERRITORY_RM <> '' and (c.MUD_ID_RM is null or c.MUD_ID_RM = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_RM,MUD_ID_RM,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.CostCenter = c.TERRITORY_RM  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_RM = d.UserId  ";

                string sqlgroupStringRM = " group by a.TA,b.RDTerritoryCode,c.TERRITORY_RM,c.MUD_ID_RD,b.CostCenter,c.MUD_ID_RM,[NAME]  order by a.TA  ";
                if (sltTA != null && sltRD != null && sltRM == null)
                {
                    sqlString = sqlStringRM + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + sqlgroupStringRM;
                }
                //选择RM之后全部DM
                string sqlStringDM = " SELECT a.TA,b.RDTerritoryCode, c.MUD_ID_RD,b.CostCenter as RMTerritoryCode,c.MUD_ID_RM, b.DMTerritoryCode as ConCode,c.MUD_ID_DM,"
                        + " case when c.TERRITORY_DM is null or c.TERRITORY_DM = '' then N'已删除' "
                       + " when c.TERRITORY_DM is not null and c.TERRITORY_DM <> '' and (c.MUD_ID_DM is null or c.MUD_ID_DM = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_DM,MUD_ID_DM,MUD_ID_RM,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.DMTerritoryCode = c.TERRITORY_DM  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_DM = d.UserId  ";
                string sqlgroupStringDM = "  group by a.TA,b.RDTerritoryCode,c.MUD_ID_RD,b.CostCenter,c.MUD_ID_RM, b.DMTerritoryCode,c.TERRITORY_DM,c.MUD_ID_DM, [NAME]  order by a.TA ";
                if (sltTA != null && sltRD != null && sltRM != null && sltDM == null)
                {
                    sqlString = sqlStringDM + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + conditionRM + sqlgroupStringDM;
                }
                //选择DM之后全部MR
                string sqlStringMR = " SELECT a.TA,b.RDTerritoryCode, c.MUD_ID_RD,b.CostCenter as RMTerritoryCode,c.MUD_ID_RM, b.DMTerritoryCode, c.MUD_ID_DM, b.MRTerritoryCode as ConCode,c.MUD_ID_MR,"
                        + " case when c.TERRITORY_MR is null or c.TERRITORY_MR = '' then N'已删除' "
                       + " when c.TERRITORY_MR is not null and c.TERRITORY_MR <> '' and (c.MUD_ID_MR is null or c.MUD_ID_MR = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_MR,MUD_ID_MR,MUD_ID_DM,MUD_ID_RM,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.MRTerritoryCode = c.TERRITORY_MR  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_MR = d.UserId  ";
                string sqlgroupStringMR = " group by a.TA,b.RDTerritoryCode,c.MUD_ID_RD,b.CostCenter,c.MUD_ID_RM, b.DMTerritoryCode,c.MUD_ID_DM,b.MRTerritoryCode,c.TERRITORY_MR,c.MUD_ID_MR,[NAME]  order by a.TA ";


                if (sltTA != null && sltRD != null && sltRM != null && sltDM != null)
                {
                    sqlString = sqlStringMR + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + conditionRM + conditionDM + sqlgroupStringMR;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_TERRITORY>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_ORDER_TERRITORY> LoadCntAmoAttData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM)
        {
            try
            {
                List<P_ORDER_TERRITORY> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionOrderState = string.Empty;
                string conditionRD = string.Empty;
                string conditionRM = string.Empty;
                string conditionDM = string.Empty;
                string sqlString = string.Empty;
                if (dTBegin != null)
                {
                    conditionstartDate = " where a.DeliverTime >= '" + dTBegin + "'  ";
                }
                if (dTEnd != null)
                {
                    conditionendDate = " AND a.DeliverTime <= '" + dTEnd + "'  ";
                }
                if (hTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (hTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (orderState != null && orderState != "")
                {
                    if (orderState == "ALL")
                    {
                        conditionOrderState = " AND (a.[State] in ('5','6','7','8','9','11','12')) ";
                    }
                    else
                    {
                        string[] con = orderState.Split(',').ToArray();
                        conditionOrderState = " AND ( 1<>1 ";
                        foreach (string str in con)
                        {
                            if (str == "订单完成")
                            {
                                conditionOrderState += " OR (a.[State] in ('6','7','9') and a.IsRetuen<>3) ";
                            }
                            if (str == "退单成功")
                            {
                                conditionOrderState += " OR (a.[State] in ('11')) ";
                            }
                            if (str == "退单失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('12') OR a.IsRetuen=3 ) ";
                            }
                            if (str == "预定失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('5')) ";
                            }
                            if (str == "未送达")
                            {
                                conditionOrderState += " OR (a.[State] in ('8')) ";
                            }
                        }
                        conditionOrderState += " )";
                    }
                }
                if (sltRD != null)
                {
                    conditionRD = " AND (b.RDTerritoryCode = " + sltRD + ") ";
                }
                if (sltRM != null)
                {
                    conditionRM = " AND (b.CostCenter = " + sltRM + ") ";
                }
                if (sltDM != null)
                {
                    conditionDM = " AND (b.DMTerritoryCode = " + sltDM + ") ";
                }
                //选择全部TA
                string sqlStringTA = "SELECT a.TA as ConCode,COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                                    + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END ))"
                                    + " as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode ";
                string sqlgroupStringTA = " GROUP BY a.TA ORDER BY a.TA ";
                if (sltTA == null)
                {
                    sqlString = sqlStringTA + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + sqlgroupStringTA;
                }
                //选择TA之后全部RD
                string sqlStringRD = " SELECT a.TA,b.RDTerritoryCode as ConCode,c.MUD_ID_RD as MUDID, case when c.TERRITORY_RD is null or c.TERRITORY_RD = '' then N'已删除' "
                       + " when c.TERRITORY_RD is not null and c.TERRITORY_RD <> '' and (c.MUD_ID_RD is null or c.MUD_ID_RD = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_RD,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.RDTerritoryCode = c.TERRITORY_RD  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_RD = d.UserId  ";
                string sqlgroupStringRD = " group by a.TA,b.RDTerritoryCode,[NAME], c.MUD_ID_RD, c.TERRITORY_RD order by a.TA  ";
                if (sltTA != null && sltRD == null)
                {
                    sqlString = sqlStringRD + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + sqlgroupStringRD;
                }
                //选择RD之后全部RM
                string sqlStringRM = " SELECT a.TA,b.RDTerritoryCode, c.MUD_ID_RD,b.CostCenter as ConCode,c.MUD_ID_RM as MUDID, case when c.TERRITORY_RM is null or c.TERRITORY_RM = '' then N'已删除' "
                       + " when c.TERRITORY_RM is not null and c.TERRITORY_RM <> '' and (c.MUD_ID_RM is null or c.MUD_ID_RM = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_RM,MUD_ID_RM,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.CostCenter = c.TERRITORY_RM  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_RM = d.UserId  ";

                string sqlgroupStringRM = " group by a.TA,b.RDTerritoryCode,c.TERRITORY_RM,c.MUD_ID_RD,b.CostCenter,c.MUD_ID_RM,[NAME]  order by a.TA  ";
                if (sltTA != null && sltRD != null && sltRM == null)
                {
                    sqlString = sqlStringRM + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + sqlgroupStringRM;
                }
                //选择RM之后全部DM
                string sqlStringDM = " SELECT a.TA,b.RDTerritoryCode, c.MUD_ID_RD,b.CostCenter as RMTerritoryCode,c.MUD_ID_RM, b.DMTerritoryCode as ConCode,c.MUD_ID_DM as MUDID,"
                        + " case when c.TERRITORY_DM is null or c.TERRITORY_DM = '' then N'已删除' "
                       + " when c.TERRITORY_DM is not null and c.TERRITORY_DM <> '' and (c.MUD_ID_DM is null or c.MUD_ID_DM = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_DM,MUD_ID_DM,MUD_ID_RM,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.DMTerritoryCode = c.TERRITORY_DM  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_DM = d.UserId  ";
                string sqlgroupStringDM = "  group by a.TA,b.RDTerritoryCode,c.MUD_ID_RD,b.CostCenter,c.MUD_ID_RM, b.DMTerritoryCode,c.TERRITORY_DM,c.MUD_ID_DM, [NAME]  order by a.TA ";
                if (sltTA != null && sltRD != null && sltRM != null && sltDM == null)
                {
                    sqlString = sqlStringDM + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + conditionRM + sqlgroupStringDM;
                }
                //选择DM之后全部MR
                string sqlStringMR = " SELECT a.TA,b.RDTerritoryCode, c.MUD_ID_RD,b.CostCenter as RMTerritoryCode,c.MUD_ID_RM, b.DMTerritoryCode, c.MUD_ID_DM, b.MRTerritoryCode as ConCode,c.MUD_ID_MR as MUDID,"
                        + " case when c.TERRITORY_MR is null or c.TERRITORY_MR = '' then N'已删除' "
                       + " when c.TERRITORY_MR is not null and c.TERRITORY_MR <> '' and (c.MUD_ID_MR is null or c.MUD_ID_MR = '') then N'空岗' else d.[Name] end as [NAME], "
                       + " COUNT(a.ID) as OrderCount,CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                       + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice ,SUM(CASE WHEN a.RealCount IS NULL THEN  a.AttendCount ELSE a.RealCount END) AS PreAttendCount "

                       + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN "
                       + " ( SELECT DISTINCT TERRITORY_MR,MUD_ID_MR,MUD_ID_DM,MUD_ID_RM,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital ) c ON b.MRTerritoryCode = c.TERRITORY_MR  "
                       + " LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_MR = d.UserId  ";
                string sqlgroupStringMR = " group by a.TA,b.RDTerritoryCode,c.MUD_ID_RD,b.CostCenter,c.MUD_ID_RM, b.DMTerritoryCode,c.MUD_ID_DM,b.MRTerritoryCode,c.TERRITORY_MR,c.MUD_ID_MR,[NAME]  order by a.TA ";


                if (sltTA != null && sltRD != null && sltRM != null && sltDM != null)
                {
                    sqlString = sqlStringMR + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + conditionRM + conditionDM + sqlgroupStringMR;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_TERRITORY>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_ORDER_HOSPITAL_RANKING> LoadHosRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA, string sltRD, string sltRM, string sltDM)
        {
            try
            {
                List<P_ORDER_HOSPITAL_RANKING> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionOrderState = string.Empty;
                string conditionRD = string.Empty;
                string conditionRM = string.Empty;
                string conditionDM = string.Empty;
                string sqlString = string.Empty;
                if (dTBegin != null)
                {
                    conditionstartDate = " where a.DeliverTime >= '" + dTBegin + "'  ";
                }
                if (dTEnd != null)
                {
                    conditionendDate = " AND a.DeliverTime <= '" + dTEnd + "'  ";
                }
                if (hTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (hTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (orderState != null && orderState != "")
                {
                    if (orderState == "ALL")
                    {
                        conditionOrderState = " AND (a.[State] in ('5','6','7','8','9','11','12')) ";
                    }
                    else
                    {
                        string[] con = orderState.Split(',').ToArray();
                        conditionOrderState = " AND ( 1<>1 ";
                        foreach (string str in con)
                        {
                            if (str == "订单完成")
                            {
                                conditionOrderState += " OR (a.[State] in ('6','7','9') and a.IsRetuen<>3) ";
                            }
                            if (str == "退单成功")
                            {
                                conditionOrderState += " OR (a.[State] in ('11')) ";
                            }
                            if (str == "退单失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('12') OR a.IsRetuen=3 ) ";
                            }
                            if (str == "预定失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('5')) ";
                            }
                            if (str == "未送达")
                            {
                                conditionOrderState += " OR (a.[State] in ('8')) ";
                            }
                        }
                        conditionOrderState += " )";
                    }
                }
                if (sltRD != null)
                {
                    conditionRD = " AND (b.RDTerritoryCode = " + sltRD + ") ";
                }
                if (sltRM != null)
                {
                    conditionRM = " AND (b.CostCenter = " + sltRM + ") ";
                }
                if (sltDM != null)
                {
                    conditionDM = " AND (b.DMTerritoryCode = " + sltDM + ") ";
                }
                //选择全部TA
                string sqlStr = " SELECT a.HospitalId,a.HospitalName, SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                                    + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END ) as OrderPrice"
                                    + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode ";
                string sqlgroupString = " GROUP BY a.HospitalId,a.HospitalName ORDER BY OrderPrice DESC ";
                if (sltTA == null)
                {
                    sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + sqlgroupString;
                }
                //选择TA之后全部RD
                if (sltTA != null && sltRD == null)
                {
                    sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + sqlgroupString;
                }
                //选择RD之后全部RM
                if (sltTA != null && sltRD != null && sltRM == null)
                {
                    sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + sqlgroupString;
                }
                //选择RM之后全部DM
                if (sltTA != null && sltRD != null && sltRM != null && sltDM == null)
                {
                    sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + conditionRM + sqlgroupString;
                }
                //选择DM之后全部MR
                if (sltTA != null && sltRD != null && sltRM != null && sltDM != null)
                {
                    sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + conditionRD + conditionRM + conditionDM + sqlgroupString;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_HOSPITAL_RANKING>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_APPLIER_RANKING> LoadApplierRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA)
        {
            try
            {
                List<P_APPLIER_RANKING> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionOrderState = string.Empty;
                string sqlString = string.Empty;
                if (dTBegin != null)
                {
                    conditionstartDate = " where a.DeliverTime >= '" + dTBegin + "'  ";
                }
                if (dTEnd != null)
                {
                    conditionendDate = " AND a.DeliverTime <= '" + dTEnd + "'  ";
                }
                if (hTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (hTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (orderState != null && orderState != "")
                {
                    if (orderState == "ALL")
                    {
                        conditionOrderState = " AND (a.[State] in ('5','6','7','8','9','11','12')) ";
                    }
                    else
                    {
                        string[] con = orderState.Split(',').ToArray();
                        conditionOrderState = " AND ( 1<>1 ";
                        foreach (string str in con)
                        {
                            if (str == "订单完成")
                            {
                                conditionOrderState += " OR (a.[State] in ('6','7','9') and a.IsRetuen<>3) ";
                            }
                            if (str == "退单成功")
                            {
                                conditionOrderState += " OR (a.[State] in ('11')) ";
                            }
                            if (str == "退单失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('12') OR a.IsRetuen=3 ) ";
                            }
                            if (str == "预定失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('5')) ";
                            }
                            if (str == "未送达")
                            {
                                conditionOrderState += " OR (a.[State] in ('8')) ";
                            }
                        }
                        conditionOrderState += " )";
                    }
                }
                string sqlStr = " SELECT a.TA,b.ApplierName,b.ApplierMUDID, b.RDTerritoryCode,c.MUD_ID_RD, case when c.TERRITORY_RD is null or c.TERRITORY_RD = '' then N'已删除' "
                                + " when c.TERRITORY_RD is not null and c.TERRITORY_RD <> '' and (c.MUD_ID_RD is null or c.MUD_ID_RD = '') then N'空岗' else d.[Name] end as RDName, "
                                + " COUNT(b.ApplierMUDID) as OrderCount, CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount "
                                + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice "
                                + " FROM dbo.P_ORDER_COST a LEFT JOIN dbo.P_PreApproval_COST b ON a.CN = b.HTCode LEFT JOIN (SELECT DISTINCT TERRITORY_RD,MUD_ID_RD "
                                + " FROM  " + _dbName + ".dbo.Territory_Hospital ) c ON b.RDTerritoryCode = c.TERRITORY_RD LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_RD = d.UserId ";
                string sqlgroupString = " GROUP BY b.ApplierMUDID,b.ApplierName,a.TA, b.RDTerritoryCode,c.MUD_ID_RD,case when c.TERRITORY_RD is null or c.TERRITORY_RD = '' then N'已删除' "
                                        + " when c.TERRITORY_RD is not null and c.TERRITORY_RD <> '' and (c.MUD_ID_RD is null or c.MUD_ID_RD = '') then N'空岗' else d.[Name] END ";
                sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + sqlgroupString;
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_APPLIER_RANKING>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_HOSPITAL_RANKING> LoadHospitalRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType, string sltTA)
        {
            try
            {
                List<P_HOSPITAL_RANKING> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string conditionOrderState = string.Empty;
                string sqlString = string.Empty;
                string sqlStr = string.Empty;
                if (dTBegin != null)
                {
                    conditionstartDate = " where a.DeliverTime >= '" + dTBegin + "'  ";
                }
                if (dTEnd != null)
                {
                    conditionendDate = " AND a.DeliverTime <= '" + dTEnd + "'  ";
                }
                if (hTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (hTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }
                if (orderState != null && orderState != "")
                {
                    if (orderState == "ALL")
                    {
                        conditionOrderState = " AND (a.[State] in ('5','6','7','8','9','11','12')) ";
                    }
                    else
                    {
                        string[] con = orderState.Split(',').ToArray();
                        conditionOrderState = " AND ( 1<>1 ";
                        foreach (string str in con)
                        {
                            if (str == "订单完成")
                            {
                                conditionOrderState += " OR (a.[State] in ('6','7','9') and a.IsRetuen<>3) ";
                            }
                            if (str == "退单成功")
                            {
                                conditionOrderState += " OR (a.[State] in ('11')) ";
                            }
                            if (str == "退单失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('12') OR a.IsRetuen=3 ) ";
                            }
                            if (str == "预定失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('5')) ";
                            }
                            if (str == "未送达")
                            {
                                conditionOrderState += " OR (a.[State] in ('8')) ";
                            }
                        }
                        conditionOrderState += " )";
                    }
                }
                if(hTType == "all")
                {
                    sqlStr = " SELECT a.TA,a.HospitalId,a.HospitalName, COUNT(a.ID) as OrderCount, CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount "
                               + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice "
                               + " FROM ( SELECT TA,REPLACE(HospitalId,'-OH','') AS HospitalId, REPLACE(HospitalName,'院外-','') AS HospitalName,ID,GSKConfirmAmount,XmsTotalPrice,DeliverTime,TotalPrice,State,IsRetuen,Address FROM dbo.P_ORDER_COST ) a ";
                }
                else
                {
                    sqlStr = " SELECT a.TA,a.HospitalId,a.HospitalName, COUNT(a.ID) as OrderCount, CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount "
                                + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice "
                                + " FROM dbo.P_ORDER_COST a ";
                }
                string sqlgroupString = " GROUP BY a.HospitalId,a.HospitalName,a.TA ";
                sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + conditionTA + sqlgroupString;
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_HOSPITAL_RANKING>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<P_RESTAURANT_RANKING> LoadRestaurantRankingData(DateTime? dTBegin, DateTime? dTEnd, string orderState, string hTType)
        {
            try
            {
                List<P_RESTAURANT_RANKING> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionOrderState = string.Empty;
                string sqlString = string.Empty;
                if (dTBegin != null)
                {
                    conditionstartDate = " where a.DeliverTime >= '" + dTBegin + "'  ";
                }
                if (dTEnd != null)
                {
                    conditionendDate = " AND a.DeliverTime <= '" + dTEnd + "'  ";
                }
                if (hTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (hTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (orderState != null && orderState != "")
                {
                    if (orderState == "ALL")
                    {
                        conditionOrderState = " AND (a.[State] in ('5','6','7','8','9','11','12')) ";
                    }
                    else
                    {
                        string[] con = orderState.Split(',').ToArray();
                        conditionOrderState = " AND ( 1<>1 ";
                        foreach (string str in con)
                        {
                            if (str == "订单完成")
                            {
                                conditionOrderState += " OR (a.[State] in ('6','7','9') and a.IsRetuen<>3) ";
                            }
                            if (str == "退单成功")
                            {
                                conditionOrderState += " OR (a.[State] in ('11')) ";
                            }
                            if (str == "退单失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('12') OR a.IsRetuen=3 ) ";
                            }
                            if (str == "预定失败")
                            {
                                conditionOrderState += " OR (a.[State] in ('5')) ";
                            }
                            if (str == "未送达")
                            {
                                conditionOrderState += " OR (a.[State] in ('8')) ";
                            }
                        }
                        conditionOrderState += " )";
                    }
                }
                string sqlStr = " SELECT Channel,RestaurantId,RestaurantName,COUNT(ID) as OrderCount, "
                                + " CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount "
                                + " WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice "
                                + " FROM dbo.P_ORDER_COST a ";
                string sqlgroupString = " GROUP BY Channel,RestaurantId,RestaurantName ";
                sqlString = sqlStr + conditionstartDate + conditionendDate + conditionHT + conditionOrderState + sqlgroupString;
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_RESTAURANT_RANKING>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_TA> LoadRankingTA()
        {
            try
            {
                List<P_TA> rtnData;
                string sqlString = "SELECT DISTINCT TA as Name from dbo.P_PreApproval_COST WHERE MRTerritoryCode IS NOT NULL AND MRTerritoryCode <>'' UNION SELECT DISTINCT TERRITORY_TA AS TA FROM " + _dbName + ".dbo.Territory_Hospital_COST";
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_TA>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
        public List<P_Order_RANKING> LoadOrderRankingChart(string deliverdate, string HTType, string sltTA)
        {
            try
            {
                List<P_Order_RANKING> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string sqlString = string.Empty;
                conditionDate = " AND a.[DeliverTime] >= DATEADD(month, datediff(month, 0, '" + deliverdate + "'), 0) AND a.[DeliverTime] <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + deliverdate + "'), 0))";
                if (HTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }

                sqlString = "select m.OrderCount,COUNT (m.ApplierMUDID) as MRCount from  "
                                   + " (SELECT distinct b.ApplierMUDID,COUNT(a.EnterpriseOrderId) AS OrderCount "
                                   //+ "  CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                                   //+ "  WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice  "
                                   //+ "  WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice  "
                                   + "  FROM [P_ORDER_COST] a  "
                                   + "  LEFT OUTER JOIN [P_PreApproval_COST] b on a.CN=b.HTCode  "
                                   + "  where a.[State] not in (5,11)  "
                                   + conditionDate
                                   + conditionHT
                                   + conditionTA
                                   + "  GROUP BY b.ApplierMUDID) m  "
                                   + "  group by m.OrderCount  ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_Order_RANKING>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_Amount_RANKING> LoadAmountRankingChart(string deliverdate, string HTType, string sltTA)
        {
            try
            {
                List<P_Amount_RANKING> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string sqlString = string.Empty;
                conditionDate = " AND a.[DeliverTime] >= DATEADD(month, datediff(month, 0, '" + deliverdate + "'), 0) AND a.[DeliverTime] <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + deliverdate + "'), 0))";
                if (HTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }

                sqlString = "select m.OrderPrice,COUNT (m.ApplierMUDID) as MRCount from  "
                                   + " (SELECT distinct b.ApplierMUDID, "
                                   + "  CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                                   + "  WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice  "
                                   + "  WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice  "
                                   + "  FROM [P_ORDER_COST] a  "
                                   + "  LEFT OUTER JOIN [P_PreApproval_COST] b on a.CN=b.HTCode  "
                                   + "  where a.[State] not in (5,11)  "
                                   + conditionDate
                                   + conditionHT
                                   + conditionTA
                                   + "  GROUP BY b.ApplierMUDID) m  "
                                   + "  group by m.OrderPrice  ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_Amount_RANKING>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }
        public List<P_Order_MRName> LoadMRName(string deliverdate, string HTType, string sltTA)
        {
            try
            {
                List<P_Order_MRName> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionDate = string.Empty;
                string conditionHT = string.Empty;
                string conditionTA = string.Empty;
                string sqlString = string.Empty;
                conditionDate = " AND a.[DeliverTime] >= DATEADD(month, datediff(month, 0, '" + deliverdate + "'), 0) AND a.[DeliverTime] <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + deliverdate + "'), 0))";
                if (HTType == "0")
                {
                    conditionHT = " AND a.[Address] <> N'院外' ";
                }
                if (HTType == "1")
                {
                    conditionHT = " AND a.[Address] = N'院外' ";
                }
                if (sltTA != null)
                {
                    conditionTA = " AND (a.TA in (" + sltTA + ")) ";
                }

                sqlString = "SELECT distinct b.ApplierMUDID,c.[Name], COUNT(a.EnterpriseOrderId) AS OrderCount,  "
                                   + "  CONVERT(DECIMAL(13,2),SUM(CASE WHEN a.GSKConfirmAmount is not null THEN GSKConfirmAmount  "
                                   + "  WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice < 0 THEN a.TotalPrice  "
                                   + "  WHEN a.GSKConfirmAmount is null and a.XmsTotalPrice >= 0 THEN a.XmsTotalPrice END )) as OrderPrice  "
                                   + "  FROM [P_ORDER_COST] a  "
                                   + "  LEFT OUTER JOIN [P_PreApproval_COST] b on a.CN=b.HTCode  "
                                   + "  LEFT OUTER JOIN [WP_QYUSER] c on c.UserId=b.ApplierMUDID  "
                                   + "  where a.[State] not in (5,11)  "
                                   + conditionDate
                                   + conditionHT
                                   + conditionTA
                                   + "  GROUP BY b.ApplierMUDID,c.[Name]  ";

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_Order_MRName>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
            }
            return null;
        }

        public List<Territory_Hospital> LoadTAHospitalOHList(string date)
        {
            try
            {
                List<Territory_Hospital> rtnData;
                string conditionDate = " AND CopyDate >= DATEADD(month, datediff(month, 0, '" + date + "'), 0) AND CopyDate <= dateadd(second, -1, DATEADD(month, datediff(month, -1, '" + date + "'), 0))";
                string sqlString = "SELECT * from [Territory_Hospital_COST] where TERRITORY_TA is not null and TERRITORY_TA <> '' " + conditionDate
                                + " AND HospitalCode IN ( SELECT REPLACE(GskHospital,'-OH','') AS GskHospital FROM P_HOSPITAL_cost WHERE GskHospital LIKE '%-OH'" + conditionDate + " and IsDelete=0 )";
                var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<Territory_Hospital>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
    }
}