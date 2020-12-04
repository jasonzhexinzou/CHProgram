using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealAdmin.Entity;
using XFramework.XDataBase;
using XFramework.XInject.Attributes;
using XFramework.XDataBase.SqlServer;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using MealAdmin.Entity.View;
using System.Configuration;
using System.Data;
using XFramework.XUtil;

namespace MealAdmin.Dao
{
    public class PreApprovalDao : IPreApprovalDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }
        private string selectSql = "SELECT p.*,u.Position  "
                                + "From P_PreApproval AS p WITH(NOLOCK) LEFT JOIN  WP_QYUSER AS u  "
                                + "ON p.ApplierMUDID=u.UserId "
                                + "WHERE 1=1  ";
        private string selectOrderBySql = " ORDER BY p.CreateDate DESC";
        private static string _dbName = ConfigurationManager.AppSettings["PositionDBName"];
        private string _NondbName = ConfigurationManager.AppSettings["NonPositionDBName"];
        #region 预申请查询
        public List<P_PreApproval> Load(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal, int rows, int page, out int total)
        {
            List<P_PreApproval> rtnData;
            string selectSqlForLoad = selectSql;
            if (srh_StartBUHeadApproveDate != "" && srh_EndBUHeadApproveDate != "")
            {
                srh_State = "5,6,9";
            }
            if (!string.IsNullOrEmpty(srh_HTCode))
            {
                srh_HTCode = srh_HTCode.Replace(",", "','");
                selectSqlForLoad += " and (p.HTCode in ('" + srh_HTCode + "'))";
            }
            if (!string.IsNullOrEmpty(srh_HospitalCode))
            {
                if (srh_HospitalCode.Split(',').Length > 1)
                {
                    selectSqlForLoad += " and (p.HospitalCode=@HospitalCode or p.HospitalCode=@OldHospitalCode)";
                }
                else
                {
                    selectSqlForLoad += " and (p.HospitalCode=@HospitalCode)";
                }
            }
            if (!string.IsNullOrEmpty(srh_CostCenter))
            {
                if (srh_CostCenter.Split(',').Length > 1)
                {
                    selectSqlForLoad += " and (p.CostCenter LIKE '%' + @CostCenter + '%' or p.CostCenter LIKE '%' + @OldCostCenter + '%')";
                }
                else
                {
                    selectSqlForLoad += " and (p.CostCenter LIKE '%' + @CostCenter + '%')";
                }
            }
            if (!string.IsNullOrEmpty(srh_RD))
            {
                selectSqlForLoad += " AND (p.RDTerritoryCode LIKE '%' + @RDTerritoryCode + '%')";
            }
            if (!string.IsNullOrEmpty(srh_ApplierMUDID))
            {
                selectSqlForLoad += " AND (p.ApplierMUDID=@ApplierMUDID)";
            }
            if (!string.IsNullOrEmpty(srh_ApplierTerritory))
            {
                selectSqlForLoad += " AND (p.MRTerritoryCode=@MRTerritoryCode)";
            }
            if (!string.IsNullOrEmpty(srh_BUHeadMUDID))
            {
                selectSqlForLoad += " AND (p.BUHeadMUDID=@BUHeadMUDID)";
            }
            if (!string.IsNullOrEmpty(srh_Market))
            {
                selectSqlForLoad += " AND (p.Market=@Market)";
            }
            if (!string.IsNullOrEmpty(srh_TA))
            {
                selectSqlForLoad += " AND (p.TA=@TA)";
            }
            if (!string.IsNullOrEmpty(srh_State))
            {
                selectSqlForLoad += " AND (p.State in (" + srh_State + "))";
            }
            if (!string.IsNullOrEmpty(srh_StartBUHeadApproveDate))
            {
                selectSqlForLoad += " AND (p.BUHeadApproveDate >= @StartBUHeadApproveDate)";
            }
            if (!string.IsNullOrEmpty(srh_EndBUHeadApproveDate))
            {
                selectSqlForLoad += " AND (p.BUHeadApproveDate <= @EndBUHeadApproveDate)";
            }
            if (!string.IsNullOrEmpty(srh_StartMeetingDate))
            {
                selectSqlForLoad += " AND (p.MeetingDate >= @StartMeetingDate)";
            }
            if (!string.IsNullOrEmpty(srh_EndMeetingDate))
            {
                selectSqlForLoad += " AND (p.MeetingDate <= @EndMeetingDate)";
            }
            if (srh_BudgetTotal == 1)
            {
                //非0元
                selectSqlForLoad += " AND (p.BudgetTotal > 0)";
            }
            else if (srh_BudgetTotal == 2)
            {
                //0元
                selectSqlForLoad += " AND (p.BudgetTotal = 0)";
            }

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_PreApproval>(rows, page, out total, selectSqlForLoad, selectOrderBySql,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", srh_HTCode),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]),
                        SqlParameterFactory.GetSqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',').Length>1?srh_HospitalCode.Split(',')[1]:string.Empty),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", srh_CostCenter.Split(',')[0]),
                        SqlParameterFactory.GetSqlParameter("@OldCostCenter", srh_CostCenter.Split(',').Length>1?srh_CostCenter.Split(',')[1]:string.Empty),
                        SqlParameterFactory.GetSqlParameter("@RDTerritoryCode", srh_RD),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", srh_ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@MRTerritoryCode", srh_ApplierTerritory),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", srh_BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@Market", srh_Market),
                        SqlParameterFactory.GetSqlParameter("@TA", srh_TA),
                        //SqlParameterFactory.GetSqlParameter("@State",srh_State),
                        SqlParameterFactory.GetSqlParameter("@StartBUHeadApproveDate", srh_StartBUHeadApproveDate==""?"1900-1-1":srh_StartBUHeadApproveDate),
                        SqlParameterFactory.GetSqlParameter("@EndBUHeadApproveDate", srh_EndBUHeadApproveDate==""?"9999-12-31":srh_EndBUHeadApproveDate+" 23:59:59.999"),
                        SqlParameterFactory.GetSqlParameter("@StartMeetingDate", srh_StartMeetingDate==""?"1900-1-1":srh_StartMeetingDate),
                        SqlParameterFactory.GetSqlParameter("@EndMeetingDate", srh_EndMeetingDate==""?"9999-12-31":srh_EndMeetingDate+" 23:59:59.999")

                    });
            }
            return rtnData;
        }
        #endregion

        #region 预申请查询
        public List<P_PreApproval> LoadFreeSpeakerFile(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal, int rows, int page, out int total)
        {
            List<P_PreApproval> rtnData;
            string selectSqlForLoad = selectSql;
            selectSqlForLoad += " and IsFreeSpeaker=1 ";
            if (srh_StartBUHeadApproveDate != "" && srh_EndBUHeadApproveDate != "")
            {
                srh_State = "5,6,9";
            }
            if (!string.IsNullOrEmpty(srh_HTCode))
            {
                srh_HTCode = srh_HTCode.Replace(",", "','");
                selectSqlForLoad += " and (p.HTCode in ('" + srh_HTCode + "'))";
            }
            if (!string.IsNullOrEmpty(srh_HospitalCode))
            {
                if (srh_HospitalCode.Split(',').Length > 1)
                {
                    selectSqlForLoad += " and (p.HospitalCode=@HospitalCode or p.HospitalCode=@OldHospitalCode)";
                }
                else
                {
                    selectSqlForLoad += " and (p.HospitalCode=@HospitalCode)";
                }
            }
            if (!string.IsNullOrEmpty(srh_CostCenter))
            {
                selectSqlForLoad += " and (p.CostCenter=@CostCenter)";
            }
            if (!string.IsNullOrEmpty(srh_RD))
            {
                selectSqlForLoad += " AND (p.RDTerritoryCode LIKE '%' + @RDTerritoryCode + '%')";
            }
            if (!string.IsNullOrEmpty(srh_ApplierMUDID))
            {
                selectSqlForLoad += " AND (p.ApplierMUDID=@ApplierMUDID)";
            }
            if (!string.IsNullOrEmpty(srh_ApplierTerritory))
            {
                selectSqlForLoad += " AND (p.MRTerritoryCode=@MRTerritoryCode)";
            }
            if (!string.IsNullOrEmpty(srh_BUHeadMUDID))
            {
                selectSqlForLoad += " AND (p.BUHeadMUDID=@BUHeadMUDID)";
            }
            if (!string.IsNullOrEmpty(srh_Market))
            {
                selectSqlForLoad += " AND (p.Market=@Market)";
            }
            if (!string.IsNullOrEmpty(srh_TA))
            {
                selectSqlForLoad += " AND (p.TA=@TA)";
            }
            if (!string.IsNullOrEmpty(srh_State))
            {
                selectSqlForLoad += " AND (p.State in (" + srh_State + "))";
            }
            if (!string.IsNullOrEmpty(srh_StartBUHeadApproveDate))
            {
                selectSqlForLoad += " AND (p.BUHeadApproveDate >= @StartBUHeadApproveDate)";
            }
            if (!string.IsNullOrEmpty(srh_EndBUHeadApproveDate))
            {
                selectSqlForLoad += " AND (p.BUHeadApproveDate <= @EndBUHeadApproveDate)";
            }
            if (!string.IsNullOrEmpty(srh_StartMeetingDate))
            {
                selectSqlForLoad += " AND (p.MeetingDate >= @StartMeetingDate)";
            }
            if (!string.IsNullOrEmpty(srh_EndMeetingDate))
            {
                selectSqlForLoad += " AND (p.MeetingDate <= @EndMeetingDate)";
            }
            if (srh_BudgetTotal == 1)
            {
                //非0元
                selectSqlForLoad += " AND (p.BudgetTotal > 0)";
            }
            else if (srh_BudgetTotal == 2)
            {
                //0元
                selectSqlForLoad += " AND (p.BudgetTotal = 0)";
            }
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_PreApproval>(rows, page, out total, selectSqlForLoad, selectOrderBySql,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", srh_HTCode),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]),
                        SqlParameterFactory.GetSqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',').Length>1?srh_HospitalCode.Split(',')[1]:string.Empty),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", srh_CostCenter.Trim()),
                        SqlParameterFactory.GetSqlParameter("@RDTerritoryCode", srh_RD),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", srh_ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@MRTerritoryCode", srh_ApplierTerritory),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", srh_BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@Market", srh_Market),
                        SqlParameterFactory.GetSqlParameter("@TA", srh_TA),
                        //SqlParameterFactory.GetSqlParameter("@State",srh_State),
                        SqlParameterFactory.GetSqlParameter("@StartBUHeadApproveDate", srh_StartBUHeadApproveDate==""?"1900-1-1":srh_StartBUHeadApproveDate),
                        SqlParameterFactory.GetSqlParameter("@EndBUHeadApproveDate", srh_EndBUHeadApproveDate==""?"9999-12-31":srh_EndBUHeadApproveDate+" 23:59:59.999"),
                        SqlParameterFactory.GetSqlParameter("@StartMeetingDate", srh_StartMeetingDate==""?"1900-1-1":srh_StartMeetingDate),
                        SqlParameterFactory.GetSqlParameter("@EndMeetingDate", srh_EndMeetingDate==""?"9999-12-31":srh_EndMeetingDate+" 23:59:59.999")

                    });
            }
            return rtnData;
        }
        #endregion

        #region 导出预申请查询
        public List<P_PreApproval> ExportPreApproval(string srh_HTCode, string srh_HospitalCode, string srh_CostCenter, string srh_ApplierMUDID, string srh_ApplierTerritory, string srh_BUHeadMUDID, string srh_Market, string srh_TA, string srh_State, string srh_StartBUHeadApproveDate, string srh_EndBUHeadApproveDate, string srh_StartMeetingDate, string srh_EndMeetingDate, string srh_RD, int srh_BudgetTotal)
        {
            try
            {
                List<P_PreApproval> rtnData;
                string selectSqlForLoad = selectSql;
                if (!string.IsNullOrEmpty(srh_HTCode))
                {
                    srh_HTCode = srh_HTCode.Replace(",", "','");
                    selectSqlForLoad += " and (p.HTCode in ('" + srh_HTCode + "'))";
                }
                if (!string.IsNullOrEmpty(srh_HospitalCode))
                {
                    if (srh_HospitalCode.Split(',').Length > 1)
                    {
                        selectSqlForLoad += " and (p.HospitalCode=@HospitalCode or p.HospitalCode=@OldHospitalCode)";
                    }
                    else
                    {
                        selectSqlForLoad += " and (p.HospitalCode=@HospitalCode)";
                    }
                }
                if (!string.IsNullOrEmpty(srh_CostCenter))
                {
                    if (srh_CostCenter.Split(',').Length > 1)
                    {
                        selectSqlForLoad += " and (p.CostCenter LIKE '%' + @CostCenter + '%' or p.CostCenter LIKE '%' + @OldCostCenter + '%')";
                    }
                    else
                    {
                        selectSqlForLoad += " and (p.CostCenter LIKE '%' + @CostCenter + '%')";
                    }
                }
                if (!string.IsNullOrEmpty(srh_RD))
                {
                    selectSqlForLoad += " AND (p.RDTerritoryCode LIKE '%' + @RDTerritoryCode + '%')";
                }
                if (!string.IsNullOrEmpty(srh_ApplierMUDID))
                {
                    selectSqlForLoad += " AND (p.ApplierMUDID=@ApplierMUDID)";
                }
                if (!string.IsNullOrEmpty(srh_ApplierTerritory))
                {
                    selectSqlForLoad += " AND (p.MRTerritoryCode=@MRTerritoryCode)";
                }
                if (!string.IsNullOrEmpty(srh_BUHeadMUDID))
                {
                    selectSqlForLoad += " AND (p.BUHeadMUDID=@BUHeadMUDID)";
                }
                if (!string.IsNullOrEmpty(srh_Market))
                {
                    selectSqlForLoad += " AND (p.Market=@Market)";
                }
                if (!string.IsNullOrEmpty(srh_TA))
                {
                    selectSqlForLoad += " AND (p.TA=@TA)";
                }
                if (!string.IsNullOrEmpty(srh_State))
                {
                    selectSqlForLoad += " AND (p.State in (" + srh_State + "))";
                }
                if (!string.IsNullOrEmpty(srh_StartBUHeadApproveDate))
                {
                    selectSqlForLoad += " AND (p.BUHeadApproveDate >= @StartBUHeadApproveDate)";
                }
                if (!string.IsNullOrEmpty(srh_EndBUHeadApproveDate))
                {
                    selectSqlForLoad += " AND (p.BUHeadApproveDate <= @EndBUHeadApproveDate)";
                }
                if (!string.IsNullOrEmpty(srh_StartMeetingDate))
                {
                    selectSqlForLoad += " AND (p.MeetingDate >= @StartMeetingDate)";
                }
                if (!string.IsNullOrEmpty(srh_EndMeetingDate))
                {
                    selectSqlForLoad += " AND (p.MeetingDate <= @EndMeetingDate)";
                }
                if (srh_BudgetTotal == 1)
                {
                    //非0元
                    selectSqlForLoad += " AND (p.BudgetTotal > 0)";
                }
                else if (srh_BudgetTotal == 2)
                {
                    //0元
                    selectSqlForLoad += " AND (p.BudgetTotal = 0)";
                }
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval>(selectSqlForLoad,
                    new SqlParameter[]
                        {
                       SqlParameterFactory.GetSqlParameter("@HTCode", srh_HTCode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", srh_ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@MRTerritoryCode", srh_ApplierTerritory),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]),
                        SqlParameterFactory.GetSqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',').Length>1?srh_HospitalCode.Split(',')[1]:string.Empty),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", srh_CostCenter.Split(',')[0]),
                        SqlParameterFactory.GetSqlParameter("@OldCostCenter", srh_CostCenter.Split(',').Length>1?srh_CostCenter.Split(',')[1]:string.Empty),
                        SqlParameterFactory.GetSqlParameter("@RDTerritoryCode", srh_RD),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", srh_BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@Market", srh_Market),
                        SqlParameterFactory.GetSqlParameter("@TA", srh_TA),
                        //SqlParameterFactory.GetSqlParameter("@State",srh_State),
                        SqlParameterFactory.GetSqlParameter("@StartBUHeadApproveDate", srh_StartBUHeadApproveDate==""?"1900-1-1":srh_StartBUHeadApproveDate +" 00:00:00"),
                        SqlParameterFactory.GetSqlParameter("@EndBUHeadApproveDate", srh_EndBUHeadApproveDate==""?"9999-12-31":srh_EndBUHeadApproveDate +" 23:59:49"),
                        SqlParameterFactory.GetSqlParameter("@StartMeetingDate", srh_StartMeetingDate==""?"1900-1-1":srh_StartMeetingDate +" 00:00:00"),
                        SqlParameterFactory.GetSqlParameter("@EndMeetingDate", srh_EndMeetingDate==""?"9999-12-31":srh_EndMeetingDate +" 23:59:49")

                        });
                }
                return rtnData;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }

        }
        #endregion


        #region 预申请最高审批人
        public List<D_COSTCENTER> CostCenterLoad(int rows, int page, out int total)
        {
            List<D_COSTCENTER> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<D_COSTCENTER>(rows, page, out total, "SELECT *  "
                                                               + "FROM D_CostCenter WHERE [TERRITORY_TA] is not null and [TERRITORY_TA]<>''  "
                                                               , " ORDER BY Market,[TERRITORY_TA] ",
                    new SqlParameter[]
                    {
                    });
            }
            return rtnData;
        }
        #endregion

        #region 从V_TERRITORY_TA查询不属于成本中心表的[TERRITORY_TA]
        public List<V_TERRITORY_TA> GetNewTERRITORY_TA()
        {
            List<V_TERRITORY_TA> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<V_TERRITORY_TA>("SELECT distinct [TERRITORY_TA] FROM [V_TERRITORY_TA] "
                      + " WHERE [TERRITORY_TA] is not null and [TERRITORY_TA]<>'' and [TERRITORY_TA] not in (  "
                      + " SELECT distinct [TERRITORY_TA] FROM [D_CostCenter]  "
                      + " WHERE [TERRITORY_TA] is not null and [TERRITORY_TA]<>'') ",
                    new SqlParameter[]
                    {
                    });
            }
            return rtnData;
        }
        #endregion

        //#region 将newTERRITORY_TA插入成本中心表

        //public int InsertnewTERRITORY_TA(List<D_COSTCENTER> list)
        //{
        //    var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
        //    using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
        //    {
        //        conn.Open();
        //        var tran = conn.BeginTransaction();
        //        foreach (var item in list)
        //        {
        //            SqlCommand commandAdd = new SqlCommand(
        //            "INSERT D_CostCenter (ID, Market, TA, Region, CreateDate, CreatedBy, TERRITORY_TA) "
        //            + " VALUES (@ID, @Market, @TA, @Region, @CreateDate, @CreatedBy, @TERRITORY_TA) ",
        //            conn);
        //            commandAdd.Transaction = tran;
        //            commandAdd.Parameters.AddRange(
        //            new SqlParameter[]
        //                   {
        //                    SqlParameterFactory.GetSqlParameter("@ID", item.ID),
        //                    SqlParameterFactory.GetSqlParameter("@Market", item.Market),
        //                    SqlParameterFactory.GetSqlParameter("@TA", item.TA),
        //                    SqlParameterFactory.GetSqlParameter("@Region", item.Region),
        //                    SqlParameterFactory.GetSqlParameter("@CreateDate", item.CreateDate),
        //                    SqlParameterFactory.GetSqlParameter("@CreatedBy", item.CreatedBy),
        //                    SqlParameterFactory.GetSqlParameter("@TERRITORY_TA", item.TERRITORY_TA)
        //                    });
        //            commandAdd.ExecuteNonQuery();
        //        }
        //        tran.Commit();
        //    }
        //    return 1;
        //}
        //#endregion

        #region list转datatable newTERRITORY_TA插入成本中心表
        public int InsertnewTERRITORY_TA(List<D_COSTCENTER> list)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            int insCnt = 0;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP 0 * FROM D_CostCenter ";
                cmd.Transaction = tran;
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataSet ds = new DataSet();
                adapter.SelectCommand = cmd;
                adapter.Fill(ds);

                var dt = ds.Tables[0];
                dt.Clear();
                foreach (var data in list)
                {
                    var dr = dt.NewRow();
                    dr[0] = data.ID;
                    dr[1] = data.Market;
                    dr[2] = data.TA;
                    dr[3] = data.BUHeadName;
                    dr[4] = data.Region;
                    dr[5] = DBNull.Value;
                    dr[6] = DBNull.Value;
                    dr[7] = DBNull.Value;
                    dr[8] = data.CreateDate;
                    dr[9] = data.CreatedBy;
                    dr[10] = DBNull.Value;
                    dr[11] = DBNull.Value;
                    dr[12] = data.BUHeadMUDID;
                    dr[13] = DBNull.Value;
                    dr[14] = DBNull.Value;
                    dr[15] = DBNull.Value;
                    dr[16] = data.TERRITORY_TA;
                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                bulkCopy.DestinationTableName = "D_CostCenter";
                bulkCopy.BatchSize = list.Count;

                bulkCopy.WriteToServer(dt);
                insCnt = bulkCopy.BatchSize;

                watch.Stop();

                LogHelper.Info("Tidy up the data time :" + watch.ElapsedMilliseconds);
                watch.Restart();
                watch.Start();

                tran.Commit();

                watch.Stop();

                LogHelper.Info("Commit time :" + watch.ElapsedMilliseconds);
                watch.Restart();
                watch.Start();

                conn.Close();

                watch.Stop();

                LogHelper.Info("Close time :" + watch.ElapsedMilliseconds);
            }

            return insCnt;
        }
        #endregion


        #region 导出预申请最高审批人
        public List<D_COSTCENTER> ExportCostCenterList()
        {
            List<D_COSTCENTER> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<D_COSTCENTER>("SELECT *  "
                                                               + "FROM D_CostCenter  "
                                                               + " WHERE [TERRITORY_TA] is not null and [TERRITORY_TA]<>''  "
                                                               + " ORDER BY Market,[TERRITORY_TA]  "
                                                               ,
                    new SqlParameter[]
                    {
                    });
            }
            return rtnData;
        }
        #endregion


        #region 通过ID查询成本中心信息

        public D_COSTCENTER GetCostCenterByID(string CostCenterID)
        {
            D_COSTCENTER rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<D_COSTCENTER>("SELECT * FROM D_CostCenter WHERE ID=@ID  ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", CostCenterID)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 通过ID查询预申请信息
        public List<P_PreApproval> GetPreApprovalID(string PerApprovalID)
        {
            List<P_PreApproval> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT p.*,u.Position "
                    + " FROM P_PreApproval AS p LEFT JOIN WP_QYUSER AS u ON p.ApplierMUDID=u.UserId "
                    + "WHERE (p.ID LIKE '%' + @ID + '%')",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", PerApprovalID)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 新增提交预申请信息
        /// <summary>
        /// 新增提交预申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_PreApproval entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT P_PreApproval (ID,VeevaMeetingID,CurrentApproverName,CurrentApproverMUDID,ApplierName,ApplierMUDID,ApplierMobile,CreateDate,ModifyDate,HTCode,Market,TA,Province," +
                    "City,HospitalCode,HospitalName,HospitalAddress,MeetingName,MeetingDate,AttendCount,CostCenter,BudgetTotal," +
                    "IsDMFollow,BUHeadName,BUHeadMUDID,BUHeadApproveDate,IsReAssign,ReAssignBUHeadName,ReAssignBUHeadMUDID," +
                    "ReAssignBUHeadApproveDate,State,MMCoEImage,IsBudgetChange,IsMMCoEChange,IsFreeSpeaker,RDSDName,RDSDMUDID,SpeakerServiceImage,SpeakerBenefitImage,HospitalAddressCode,RDTerritoryCode,MRTerritoryCode,HTType,ActionState,DMTerritoryCode ) "
                    + " VALUES (@ID,@VeevaMeetingID,@CurrentApproverName,@CurrentApproverMUDID,@ApplierName,@ApplierMUDID,@ApplierMobile,@CreateDate,@ModifyDate,@HTCode,@Market,@TA,@Province," +
                    "@City,@HospitalCode,@HospitalName,@HospitalAddress,@MeetingName,@MeetingDate,@AttendCount,@CostCenter," +
                    "@BudgetTotal,@IsDMFollow,@BUHeadName,@BUHeadMUDID,@BUHeadApproveDate,@IsReAssign,@ReAssignBUHeadName," +
                    "@ReAssignBUHeadMUDID,@ReAssignBUHeadApproveDate,@State,@MMCoEImage,@IsBudgetChange,@IsMMCoEChange,@IsFreeSpeaker,@RDSDName,@RDSDMUDID,@SpeakerServiceImage,@SpeakerBenefitImage,@HospitalAddressCode,@RDTerritoryCode,@MRTerritoryCode,@HTType,'0',@DMTerritoryCode ) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@VeevaMeetingID", entity.VeevaMeetingID),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverName", entity.CurrentApproverName),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverMUDID", entity.CurrentApproverMUDID),
                        SqlParameterFactory.GetSqlParameter("@ApplierName", entity.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", entity.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@ApplierMobile", entity.ApplierMobile),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", entity.ModifyDate),
                        SqlParameterFactory.GetSqlParameter("@HTCode", entity.HTCode),
                        SqlParameterFactory.GetSqlParameter("@Market", entity.Market),
                        SqlParameterFactory.GetSqlParameter("@TA", entity.TA),
                        SqlParameterFactory.GetSqlParameter("@Province", entity.Province),
                        SqlParameterFactory.GetSqlParameter("@City", entity.City),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", entity.HospitalCode),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", entity.HospitalName),
                        SqlParameterFactory.GetSqlParameter("@HospitalAddress", entity.HospitalAddress),
                        SqlParameterFactory.GetSqlParameter("@MeetingName", entity.MeetingName),
                        SqlParameterFactory.GetSqlParameter("@MeetingDate", entity.MeetingDate),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", entity.CostCenter),
                        SqlParameterFactory.GetSqlParameter("@BudgetTotal", entity.BudgetTotal),
                        SqlParameterFactory.GetSqlParameter("@IsDMFollow", entity.IsDMFollow),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", entity.BUHeadName),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", entity.BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@BUHeadApproveDate", entity.BUHeadApproveDate),
                        SqlParameterFactory.GetSqlParameter("@IsReAssign", entity.IsReAssign),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadName", entity.ReAssignBUHeadName),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadMUDID", entity.ReAssignBUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadApproveDate", entity.ReAssignBUHeadApproveDate),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@IsBudgetChange", entity.IsBudgetChange),
                        SqlParameterFactory.GetSqlParameter("@IsMMCoEChange", entity.IsMMCoEChange),
                        SqlParameterFactory.GetSqlParameter("@IsFreeSpeaker", entity.IsFreeSpeaker),
                        SqlParameterFactory.GetSqlParameter("@RDSDName", entity.RDSDName),
                        SqlParameterFactory.GetSqlParameter("@RDSDMUDID", entity.RDSDMUDID),
                        SqlParameterFactory.GetSqlParameter("@SpeakerServiceImage", entity.SpeakerServiceImage),
                        SqlParameterFactory.GetSqlParameter("@SpeakerBenefitImage", entity.SpeakerBenefitImage),
                        SqlParameterFactory.GetSqlParameter("@HospitalAddressCode", entity.HospitalAddressCode),
                        SqlParameterFactory.GetSqlParameter("@RDTerritoryCode", entity.RDTerritoryCode),
                        SqlParameterFactory.GetSqlParameter("@MRTerritoryCode", entity.MRTerritoryCode),
                        SqlParameterFactory.GetSqlParameter("@HTType", entity.HTType),
                        SqlParameterFactory.GetSqlParameter("@DMTerritoryCode", entity.DMTerritoryCode)
                    });
            }
        }
        #endregion
        #region 获取直线经理和userid
        /// <summary>
        /// 获取直线经理和userid
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public P_PreApproval GetNameUserId(string UserId)
        {


            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                var list = sqlServerTemplate.Find<P_PreApproval>($" SELECT db_owner.GetDMName(@CurrentApproverName) AS CurrentApproverName , db_owner.GetDMUserID(@CurrentApproverMUDID) AS CurrentApproverMUDID ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@TA", ta),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverName", UserId),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverMUDID", UserId)
                    });
                return list;
            }

        }
        #endregion
        #region 查询是否在是buheade
        //public D_COSTCENTER FindUserIdInfo(string UserId)
        //{
        //    var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
        //    using (var conn = sqlServerTemplate.GetSqlConnection())
        //    {

        //        conn.Open();
        //        var list = sqlServerTemplate.Find<D_COSTCENTER>($" SELECT db_owner.GetDMName(@CurrentApproverName) AS CurrentApproverName , db_owner.GetDMUserID(@CurrentApproverMUDID) AS CurrentApproverMUDID ",
        //            new SqlParameter[]
        //            {
        //                //SqlParameterFactory.GetSqlParameter("@TA", ta),
        //                SqlParameterFactory.GetSqlParameter("@CurrentApproverName", UserId),
        //                SqlParameterFactory.GetSqlParameter("@CurrentApproverMUDID", UserId)
        //            });
        //        return list;
        //    }
        //}
        #endregion
        #region 更新提交预申请信息
        /// <summary>
        /// 更新提交预申请信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(P_PreApproval entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_PreApproval SET ApplierName=@ApplierName,ApplierMUDID=@ApplierMUDID,ApplierMobile=@ApplierMobile," +
                    "CreateDate=@CreateDate,ModifyDate=@ModifyDate,HTCode=@HTCode,Market=@Market,VeevaMeetingID=@VeevaMeetingID,TA=@TA,Province=@Province," +
                    "City=@City,HospitalCode=@HospitalCode,HospitalName=@HospitalName,HospitalAddress=@HospitalAddress," +
                    "MeetingName=@MeetingName,MeetingDate=@MeetingDate,AttendCount=@AttendCount,CostCenter=@CostCenter," +
                    "BudgetTotal=@BudgetTotal,IsDMFollow=@IsDMFollow,BUHeadName=@BUHeadName,BUHeadMUDID=@BUHeadMUDID," +
                    "BUHeadApproveDate=@BUHeadApproveDate,IsReAssign=@IsReAssign,ReAssignBUHeadName=@ReAssignBUHeadName," +
                    "ReAssignBUHeadMUDID=@ReAssignBUHeadMUDID,ReAssignBUHeadApproveDate=@ReAssignBUHeadApproveDate,State=@State," +
                    "MMCoEImage=@MMCoEImage,IsBudgetChange=@IsBudgetChange,IsMMCoEChange=@IsMMCoEChange,IsFreeSpeaker=@IsFreeSpeaker,RDSDName=@RDSDName,RDSDMUDID=@RDSDMUDID,SpeakerServiceImage=@SpeakerServiceImage,HospitalAddressCode=@HospitalAddressCode," +
                    "SpeakerBenefitImage=@SpeakerBenefitImage,RDTerritoryCode = @RDTerritoryCode,DMTerritoryCode=@DMTerritoryCode,HTType = @HTType,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@ApplierName", entity.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", entity.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@ApplierMobile", entity.ApplierMobile),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", entity.ModifyDate),
                        SqlParameterFactory.GetSqlParameter("@HTCode", entity.HTCode),
                        SqlParameterFactory.GetSqlParameter("@Market", entity.Market),
                         SqlParameterFactory.GetSqlParameter("@VeevaMeetingID", entity.VeevaMeetingID),
                        SqlParameterFactory.GetSqlParameter("@TA", entity.TA),
                        SqlParameterFactory.GetSqlParameter("@Province", entity.Province),
                        SqlParameterFactory.GetSqlParameter("@City", entity.City),
                        SqlParameterFactory.GetSqlParameter("@HospitalCode", entity.HospitalCode),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", entity.HospitalName),
                        SqlParameterFactory.GetSqlParameter("@HospitalAddress", entity.HospitalAddress),
                        SqlParameterFactory.GetSqlParameter("@MeetingName", entity.MeetingName),
                        SqlParameterFactory.GetSqlParameter("@MeetingDate", entity.MeetingDate),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", entity.CostCenter),
                        SqlParameterFactory.GetSqlParameter("@BudgetTotal", entity.BudgetTotal),
                        SqlParameterFactory.GetSqlParameter("@IsDMFollow", entity.IsDMFollow),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", entity.BUHeadName),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", entity.BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@BUHeadApproveDate", entity.BUHeadApproveDate),
                        SqlParameterFactory.GetSqlParameter("@IsReAssign", entity.IsReAssign),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadName", entity.ReAssignBUHeadName),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadMUDID", entity.ReAssignBUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadApproveDate", entity.ReAssignBUHeadApproveDate),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@IsBudgetChange", entity.IsBudgetChange),
                        SqlParameterFactory.GetSqlParameter("@IsMMCoEChange", entity.IsMMCoEChange),
                        SqlParameterFactory.GetSqlParameter("@IsFreeSpeaker", entity.IsFreeSpeaker),
                        SqlParameterFactory.GetSqlParameter("@RDSDName", entity.RDSDName),
                        SqlParameterFactory.GetSqlParameter("@RDSDMUDID", entity.RDSDMUDID),
                        SqlParameterFactory.GetSqlParameter("@SpeakerServiceImage", entity.SpeakerServiceImage),
                        SqlParameterFactory.GetSqlParameter("@SpeakerBenefitImage", entity.SpeakerBenefitImage),
                        SqlParameterFactory.GetSqlParameter("@HospitalAddressCode", entity.HospitalAddressCode),
                        SqlParameterFactory.GetSqlParameter("@RDTerritoryCode", entity.RDTerritoryCode),
                        SqlParameterFactory.GetSqlParameter("@HTType", entity.HTType),
                        SqlParameterFactory.GetSqlParameter("@DMTerritoryCode", entity.DMTerritoryCode)
                    });
            }
        }
        #endregion
        #region 更新提交预申请信息
        /// <summary>
        /// 更新提交预申请当前审批人信息信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateCurrentPreApprova(P_PreApproval entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_PreApproval SET CurrentApproverMUDID=@CurrentApproverMUDID,CurrentApproverName=@CurrentApproverName,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverMUDID", entity.CurrentApproverMUDID),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverName", entity.CurrentApproverName),
                    });
            }
        }
        #endregion

        #region 更新历史记录表删除标记
        /// <summary>
        /// 更新历史记录表删除标记
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateHisPreApprovaDelete(Guid PID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_PreApprovalApproveHistory SET IsDelete=1 WHERE PID=@PID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID),
                    });
            }
        }
        #endregion


        #region 保存预申请最高审批人更改
        public int SaveChange(string ID, string txtTERRITORY_TA, string txtBUHeadName, string txtBUHeadMUDID, string name)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE [D_CostCenter] SET [TERRITORY_TA]=@TERRITORY_TA,BUHeadName=@BUHeadName,BUHeadMUDID=@BUHeadMUDID,ModifyDate=GETDATE(),ModifiedBy=@UserName  " +
                                               "WHERE ID=@ID ",
                 new SqlParameter[]
                 {
                        SqlParameterFactory.GetSqlParameter("@ID", ID),
                        SqlParameterFactory.GetSqlParameter("@TERRITORY_TA", txtTERRITORY_TA),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", txtBUHeadName),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", txtBUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@UserName", name)
                 });
            }
        }
        #endregion

        #region 同步更新预申请表BUHead
        public int UpdatePreApprovalBUHead(Guid ID, string txtBUHeadMUDID, string txtBUHeadName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE [P_PreApproval] SET [BUHeadName]=@BUHeadName,[BUHeadMUDID]=@BUHeadMUDID,[ActionState]='0'  " +
                                               "WHERE ID=@ID ",
                 new SqlParameter[]
                 {
                        SqlParameterFactory.GetSqlParameter("@ID", ID),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", txtBUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", txtBUHeadName)
                 });
            }
        }
        #endregion

        #region 删除提交预申请信息
        /// <summary>
        /// 删除提交预申请信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreApproval SET State=4,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
        }
        #endregion

        #region 预申请MMCoE审批记录
        public List<P_PreApprovalApproveHistory_View> RecordsLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_startMeetingDate, string srh_endMeetingDate, string srh_State, string srh_StartApproveDate, string srh_EndApproveDate, int rows, int page, out int total)
        {
            List<P_PreApprovalApproveHistory_View> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_PreApprovalApproveHistory_View>(rows, page, out total, "SELECT P_PreApprovalApproveHistory.ActionType,P_PreApprovalApproveHistory.ApproveDate,P_PreApprovalApproveHistory.Comments,P_PreApprovalApproveHistory.PID,P_PreApprovalApproveHistory.UserId,P_PreApprovalApproveHistory.UserName, P_PreApproval.*  "
                                                                                                             + "FROM P_PreApproval  RIGHT JOIN P_PreApprovalApproveHistory  "
                                                                                                             + "ON P_PreApproval.ID = P_PreApprovalApproveHistory.PID  "
                                                                                                             + "WHERE (P_PreApproval.HTCode LIKE '%' + @HTCode + '%') "
                                                                                                             + "AND (P_PreApproval.ApplierMUDID LIKE '%' + @ApplierMUDID + '%')  "
                                                                                                             + "AND (P_PreApprovalApproveHistory.UserId LIKE '%' + @srh_BUHeadMUDID + '%')  "
                                                                                                             + "AND (P_PreApproval.MeetingDate >= @StartMeetingDate) AND (P_PreApproval.MeetingDate <= @EndMeetingDate)  "
                                                                                                             + "AND (P_PreApproval.State LIKE '[' + @State + ']')  "
                                                                                                             + "AND (P_PreApprovalApproveHistory.ApproveDate >= @StartApproveDate) AND (P_PreApprovalApproveHistory.ApproveDate <= @EndApproveDate)   "
                                                                                                             + "AND (P_PreApprovalApproveHistory.ActionType=2 OR P_PreApprovalApproveHistory.ActionType=3) "
                                                                                                             + "AND (P_PreApprovalApproveHistory.Type=1)"
                                                                                                              , "ORDER BY P_PreApproval.CreateDate DESC",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", srh_HTCode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", srh_ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@srh_BUHeadMUDID", srh_BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@StartMeetingDate", srh_startMeetingDate==""?"1900-1-1":srh_startMeetingDate),
                        SqlParameterFactory.GetSqlParameter("@EndMeetingDate", srh_endMeetingDate==""?"9999-12-31":srh_endMeetingDate+" 23:59:59.999"),
                        SqlParameterFactory.GetSqlParameter("@State",srh_State),
                        SqlParameterFactory.GetSqlParameter("@StartApproveDate", srh_StartApproveDate==""?"1900-1-1":srh_StartApproveDate),
                        SqlParameterFactory.GetSqlParameter("@EndApproveDate", srh_EndApproveDate==""?"9999-12-31":srh_EndApproveDate+" 23:59:59.999")


                    });
            }
            return rtnData;
        }
        #endregion

        #region 预申请MMCoE审批
        public List<P_PreApproval> MMCoELoad(string srh_HTCode, string srh_ApplierMUDID, string srh_startMeetingDate, string srh_endMeetingDate, int rows, int page, out int total)
        {
            List<P_PreApproval> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_PreApproval>(rows, page, out total, "SELECT*  FROM P_PreApproval WHERE (P_PreApproval.HTCode LIKE '%' + @HTCode + '%')"
                    + " AND (P_PreApproval.ApplierMUDID LIKE '%' + @ApplierMUDID + '%')  "
                    + " AND P_PreApproval.State='1'  "
                    + " AND(MeetingDate >= @StartMeetingDate)  "
                    + " AND(MeetingDate <= @EndMeetingDate)  ",
                    " ORDER BY P_PreApproval.CreateDate DESC ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", srh_HTCode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", srh_ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@StartMeetingDate", srh_startMeetingDate==""?"1900-1-1":srh_startMeetingDate),
                        SqlParameterFactory.GetSqlParameter("@EndMeetingDate", srh_endMeetingDate==""?"9999-12-31":srh_endMeetingDate+" 23:59:59.999")
                    });
            }
            return rtnData;
        }
        #endregion

        #region 预申请MMCoE审批--通过
        public int Approved(Guid ID, string PID, string UserName, string UserId, int ActionType, DateTime ApproveDate, string txtComments, int type)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.ExecuteNonQuery(
                    "INSERT INTO P_PreApprovalApproveHistory (ID,PID,UserName,UserId,ActionType,ApproveDate,Comments,type) "
                    + " VALUES (@ID,@PID,@UserName,@UserId,@ActionType,@ApproveDate,@Comments,@type) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", ID),
                        SqlParameterFactory.GetSqlParameter("@PID", PID),
                        SqlParameterFactory.GetSqlParameter("@UserName", UserName),
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@ActionType", ActionType),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate",ApproveDate),
                        SqlParameterFactory.GetSqlParameter("@Comments", txtComments),
                        SqlParameterFactory.GetSqlParameter("@type", type)

                    });
                return res;
            }
        }
        #endregion

        #region 修改预申请状态--通过
        public int UpdateStadeApproved(string ID, string PreApprovalState)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreApproval SET State=@State,ActionState='0'  " +
                                               "WHERE ID=@ID ",
                 new SqlParameter[]
                 {
                        SqlParameterFactory.GetSqlParameter("@ID", ID),
                        SqlParameterFactory.GetSqlParameter("@State", PreApprovalState)

                 });
            }
        }
        #endregion

        #region 查找成本中心审批人信息
        /// <summary>
        /// 查找成本中心审批人信息
        /// </summary>
        /// <param name="ta">TA</param>
        /// <param name="region"></param>
        /// <param name="costCenter"></param>
        /// <returns></returns>
        public D_COSTCENTER FindInfo(string ta, string region, string costCenter)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Find<D_COSTCENTER>($"select * from D_CostCenter where TERRITORY_TA = @CostCenter ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@TA", ta),
                        //SqlParameterFactory.GetSqlParameter("@Region", region),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", costCenter)
                    });
                return list;
            }
        }
        #endregion

        #region 获取HTCode编号方法
        /// <summary>
        /// 获取HTCode编号方法
        /// </summary>
        /// <returns></returns>
        public HTCode GetHTCode()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var result = sqlServerTemplate.Find<HTCode>($"SELECT MAX(HTCodeString) as HTCodeString FROM (SELECT right(HTCode,10) as HTCodeString FROM P_PreApproval) as HTCodeString");
                return result;
            }
        }
        #endregion

        #region 根据HTCode获取是否在数据库中存在此编号
        /// <summary>
        /// 根据HTCode获取是否在数据库中存在此编号
        /// </summary>
        /// <param name="htcodeId"></param>
        /// <returns></returns>
        public HTCode GetHTCodeByID(string htcodeId)
        {
            HTCode rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<HTCode>("SELECT HTCode AS HTCodeString  FROM  (SELECT HTCode FROM P_PreApproval) AS HTCodeInfo WHERE HTCode = @HTCode ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", htcodeId)
                    });
            }
            return rtnData;
        }

        #endregion

        #region 获取预申请详情
        public P_PreApproval LoadPreApprovalInfo(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var result = sqlServerTemplate.Find<P_PreApproval>($"SELECT * FROM P_PreApproval where ID=@ID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)});
                return result;
            }
        }
        #endregion

        #region 预申请BUHead审批通过
        /// <summary>
        /// 预申请BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int BUHeadApprove(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreApproval SET State=@State,BUHeadApproveDate=@BUHeadApproveDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@State", state),
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@BUHeadApproveDate", DateTime.Now),
                    });
            }
        }
        #endregion


        #region 预申请MMCoE审批通过
        /// <summary>
        /// 预申请BUHead审批通过
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEApprove(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreApproval SET State=@State,BUHeadApproveDate=GETDATE() WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@State", state)
                    });
            }
        }
        #endregion

        #region 预申请BUHead审批驳回
        /// <summary>
        /// 预申请BUHead审批驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int BUHeadReject(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreApproval SET State=@State,BUHeadApproveDate=@BUHeadApproveDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                         SqlParameterFactory.GetSqlParameter("@State", state),
                        SqlParameterFactory.GetSqlParameter("@BUHeadApproveDate", DateTime.Now),
                    });
            }
        }
        #endregion

        #region 预申请MMCoE审批驳回
        /// <summary>
        /// 预申请BUHead审批驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEReject(Guid id, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreApproval SET State=@State WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@State", state),
                    });
            }
        }
        #endregion

        #region 成本中心导入
        public int ImportCostCenter(List<D_COSTCENTER> fails, string adminUser)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var nowDate = DateTime.Now;
                int state = 0;
                D_COSTCENTER costCenter = new D_COSTCENTER();
                foreach (var item in fails)
                {
                    costCenter.BUHeadMUDID = item.BUHeadMUDID;
                    costCenter.BUHeadName = item.BUHeadName;
                    costCenter.CostCenter = item.CostCenter;
                    costCenter.CreateDate = nowDate;
                    costCenter.CreatedBy = adminUser;
                    costCenter.ID = Guid.NewGuid();
                    costCenter.Market = item.Market;
                    costCenter.Region = item.Region;
                    costCenter.RegionManagerMUDID = item.RegionManagerMUDID;
                    costCenter.RegionManagerName = item.RegionManagerName;
                    costCenter.TA = item.TA;
                    costCenter.RDSDMUDID = item.RDSDMUDID;
                    costCenter.RDSDName = item.RDSDName;
                    costCenter.OldCostCenter = item.OldCostCenter;
                    state = sqlServerTemplate.ExecuteNonQuery(
                    "INSERT D_CostCenter (ID, Market, TA, BUHeadName, Region, RegionManagerName, RegionManagerMUDID, CostCenter, CreateDate, CreatedBy, BUHeadMUDID,RDSDMUDID,RDSDName,OldCostCenter) "
                    + " VALUES (@ID, @Market, @TA, @BUHeadName, @Region, @RegionManagerName, @RegionManagerMUDID, @CostCenter, @CreateDate, @CreatedBy, @BUHeadMUDID,@RDSDMUDID,@RDSDName,@OldCostCenter) ",
                    new SqlParameter[]
                    {
                            SqlParameterFactory.GetSqlParameter("@ID", costCenter.ID),
                            SqlParameterFactory.GetSqlParameter("@Market", costCenter.Market),
                            SqlParameterFactory.GetSqlParameter("@TA", costCenter.TA),
                            SqlParameterFactory.GetSqlParameter("@BUHeadName", costCenter.BUHeadName),
                            SqlParameterFactory.GetSqlParameter("@Region", costCenter.Region),
                            SqlParameterFactory.GetSqlParameter("@RegionManagerName",costCenter.RegionManagerName),
                            SqlParameterFactory.GetSqlParameter("@RegionManagerMUDID", costCenter.RegionManagerMUDID),
                            SqlParameterFactory.GetSqlParameter("@CostCenter", costCenter.CostCenter),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", costCenter.CreateDate),
                            SqlParameterFactory.GetSqlParameter("@CreatedBy", costCenter.CreatedBy),
                            SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", costCenter.BUHeadMUDID),
                            SqlParameterFactory.GetSqlParameter("@RDSDMUDID", costCenter.RDSDMUDID),
                            SqlParameterFactory.GetSqlParameter("@RDSDName", costCenter.RDSDName),
                            SqlParameterFactory.GetSqlParameter("@OldCostCenter", costCenter.OldCostCenter),
                    });
                }
                return state;
            }
        }
        #endregion

        #region 将未完成的预申请旧成本中心更新成新的成本中心
        public int UpdateUncompleteCostenterCodeByOldCostCenterCode(string CostCenterCode, string OldCostcenterCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                Guid ID = Guid.NewGuid();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_PreApproval set CostCenter=@CostCenterCode,ActionState='0' where CostCenter=@OldCostcenterCode and IsUsed=0 and MeetingDate>@TimeNow ",
                    new SqlParameter[]
                    {
                            SqlParameterFactory.GetSqlParameter("@CostCenterCode", CostCenterCode),
                            SqlParameterFactory.GetSqlParameter("@OldCostcenterCode", OldCostcenterCode),
                            SqlParameterFactory.GetSqlParameter("@TimeNow",DateTime.Now),
                    });
            }
        }
        #endregion

        #region 通过ID查询TERRITORY_TA

        public List<D_COSTCENTER> GetTERRITORY_TAByID(Guid id)
        {
            List<D_COSTCENTER> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<D_COSTCENTER>("SELECT * FROM D_CostCenter WHERE ID=@ID  ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 删除成本中心
        /// <summary>
        /// 删除成本中心
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public int Del(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("DELETE FROM D_CostCenter WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
        }
        #endregion

        #region 成本中心导入重复的Update

        public int UpdateCostCenter(List<D_COSTCENTER> excelRows, string adminUser, string _TA, string _Region, string _CostCenter)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var nowDate = DateTime.Now;
                int state = 0;
                D_COSTCENTER costCenter = new D_COSTCENTER();
                foreach (var item in excelRows)
                {
                    costCenter.BUHeadMUDID = item.BUHeadMUDID;
                    costCenter.BUHeadName = item.BUHeadName;
                    costCenter.CostCenter = item.CostCenter;
                    costCenter.OldCostCenter = item.OldCostCenter;
                    costCenter.CreateDate = item.CreateDate;
                    costCenter.CreatedBy = adminUser;
                    costCenter.ID = Guid.NewGuid();
                    costCenter.Market = item.Market;
                    costCenter.Region = item.Region;
                    costCenter.RegionManagerMUDID = item.RegionManagerMUDID;
                    costCenter.RegionManagerName = item.RegionManagerName;
                    costCenter.RDSDMUDID = item.RDSDMUDID;
                    costCenter.RDSDName = item.RDSDName;
                    costCenter.TA = item.TA;
                    costCenter.ModifyDate = nowDate;
                    costCenter.ModifiedBy = adminUser;
                    state = sqlServerTemplate.ExecuteNonQuery(
                    "Update D_CostCenter SET  Market=@Market,  BUHeadName=@BUHeadName, RegionManagerName=@RegionManagerName, RegionManagerMUDID=@RegionManagerMUDID,RDSDName=@RDSDName,RDSDMUDID=@RDSDMUDID,CostCenter=@CostCenter,OldCostCenter=@OldCostCenter, CreateDate=@CreateDate, CreatedBy=@CreatedBy, BUHeadMUDID=@BUHeadMUDID,ModifyDate=@ModifyDate,ModifiedBy=@ModifiedBy "
                    + "WHERE TA=@_TA AND Region=@_Region AND CostCenter = @_CostCenter",
                    new SqlParameter[]
                    {
                            SqlParameterFactory.GetSqlParameter("@ID", costCenter.ID),
                            SqlParameterFactory.GetSqlParameter("@Market", costCenter.Market),
                            SqlParameterFactory.GetSqlParameter("@BUHeadName", costCenter.BUHeadName),
                            SqlParameterFactory.GetSqlParameter("@RegionManagerName",costCenter.RegionManagerName),
                            SqlParameterFactory.GetSqlParameter("@RegionManagerMUDID", costCenter.RegionManagerMUDID),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", costCenter.CreateDate),
                            SqlParameterFactory.GetSqlParameter("@CreatedBy", costCenter.CreatedBy),
                            SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", costCenter.BUHeadMUDID),
                            SqlParameterFactory.GetSqlParameter("@_TA",_TA),
                            SqlParameterFactory.GetSqlParameter("@_Region",_Region),
                            SqlParameterFactory.GetSqlParameter("@CostCenter",costCenter.CostCenter),
                            SqlParameterFactory.GetSqlParameter("@_CostCenter",_CostCenter),
                            SqlParameterFactory.GetSqlParameter("@ModifyDate",costCenter.ModifyDate),
                            SqlParameterFactory.GetSqlParameter("@ModifiedBy",costCenter.ModifiedBy),
                            SqlParameterFactory.GetSqlParameter("@OldCostCenter",costCenter.OldCostCenter),
                            SqlParameterFactory.GetSqlParameter("@RDSDMUDID",costCenter.RDSDMUDID),
                            SqlParameterFactory.GetSqlParameter("@RDSDName",costCenter.RDSDName)
                    });
                }
                return state;
            }

        }


        #endregion

        #region 新增成本中心

        public int AddCostCenter(string sltMarket, string sltTA, string txtBUHeadName, string BUHeadMUDID, string Region, string RegionManagerName, string RegionManagerMUDID, string RDSDManagerName, string RDSDManagerMUDID, string CostCenter, string OldCostCenter, string CreateBy, DateTime CreateDateTIme)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                Guid ID = Guid.NewGuid();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT D_CostCenter (ID, Market, TA, BUHeadName, Region, RegionManagerName, RegionManagerMUDID,RDSDName,RDSDMUDID, CostCenter,OldCostCenter, CreateDate, CreatedBy, BUHeadMUDID) "
                    + " VALUES (@ID, @Market, @TA, @BUHeadName, @Region, @RegionManagerName, @RegionManagerMUDID,@RDSDManagerName,@RDSDManagerMUDID, @CostCenter,@OldCostCenter, @CreateDate, @CreatedBy, @BUHeadMUDID) ",
                    new SqlParameter[]
                    {
                            SqlParameterFactory.GetSqlParameter("@ID", ID),
                            SqlParameterFactory.GetSqlParameter("@Market", sltMarket),
                            SqlParameterFactory.GetSqlParameter("@TA",sltTA),
                            SqlParameterFactory.GetSqlParameter("@BUHeadName",txtBUHeadName),
                            SqlParameterFactory.GetSqlParameter("@Region", Region),
                            SqlParameterFactory.GetSqlParameter("@RegionManagerName",RegionManagerName),
                            SqlParameterFactory.GetSqlParameter("@RegionManagerMUDID", RegionManagerMUDID),
                            SqlParameterFactory.GetSqlParameter("@RDSDManagerName",RDSDManagerName),
                            SqlParameterFactory.GetSqlParameter("@RDSDManagerMUDID", RDSDManagerMUDID),
                            SqlParameterFactory.GetSqlParameter("@CostCenter", CostCenter),
                            SqlParameterFactory.GetSqlParameter("@OldCostCenter", OldCostCenter),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", CreateDateTIme),
                            SqlParameterFactory.GetSqlParameter("@CreatedBy", CreateBy),
                            SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", BUHeadMUDID)
                    });
            }

        }
        #endregion

        #region 查询成本中心是否存在
        public D_CostCenterCount findCostCenter(string sltTa, string Region, string CostCenter)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var count = sqlServerTemplate.Find<D_CostCenterCount>($"select count(*) as CostCenterCount from D_CostCenter where TA = @TA and Region = @Region and CostCenter = @CostCenter ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TA", sltTa),
                        SqlParameterFactory.GetSqlParameter("@Region", Region),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", CostCenter)
                    });
                return count;
            }
        }
        #endregion

        #region 通过ID查找成本中心
        public D_COSTCENTER FindCostCenterByID(Guid ID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Find<D_COSTCENTER>($"SELECT* FROM D_CostCenter WHERE ID = @ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
                return entity;
            }
        }
        #endregion

        #region 查询该成本中心是否有未审批的订单
        public int _Exist(string CostCenter)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                var Entity = sqlServerTemplate.Find<P_CostCenter_Count>($" Select COUNT(*) AS Count  "
                                                                                  + "FROM P_PreApproval "
                                                                                  + " WHERE P_PreApproval.State = '3' AND P_PreApproval.CostCenter = @CostCenter",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CostCenter", CostCenter)
                    });
                return Entity.Count;
            }

        }
        #endregion

        #region 查询该TERRITORY_TA是否有未审批的订单
        public int GetStateByTA(string TERRITORY_TA)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                var Entity = sqlServerTemplate.Find<P_CostCenter_Count>($" Select COUNT(*) AS Count  "
                                                                                  + "FROM P_PreApproval "
                                                                                  + " WHERE P_PreApproval.State = '3' AND P_PreApproval.TA = @TA",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TA", TERRITORY_TA)
                    });
                return Entity.Count;
            }

        }
        #endregion

        #region 查询MMCoE历史订单----1.0 CN字段以CN开头的
        /// <summary>
        /// 查询MMCoE历史订单----1.0 CN字段以CN开头的
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> GetMMCoEHisOrder()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($" SELECT * FROM P_ORDER WHERE CN like 'CN%'  ", new SqlParameter[]
                    {

                    });
                return list;
            }
        }
        #endregion

        public List<P_PreApprovalApproveHistory> GetApproval(string id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Load<P_PreApprovalApproveHistory>("select P_PreApprovalApproveHistory.*  "
                                                                                    + "from P_PreApproval join P_PreApprovalApproveHistory  "
                                                                                    + "on P_PreApproval.ID = P_PreApprovalApproveHistory.PID  "
                                                                                    + "where (P_PreApproval.ID = @ID) order by P_PreApprovalApproveHistory.ApproveDate",
                                                                                    new SqlParameter[]{
                                                                                        SqlParameterFactory.GetSqlParameter("@ID",id)
                                                                                    });
            }
        }


        public int AddPreApprovalApproveHistory(P_PreApprovalApproveHistory PreApprovalHistory)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("insert into P_PreApprovalApproveHistory values(@ID,@PID,@UserName,@UserID,@ActionType,@ApproveDate,@Comments,@Type,@IsDelete) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", PreApprovalHistory.ID),
                        SqlParameterFactory.GetSqlParameter("@PID", PreApprovalHistory.PID),
                        SqlParameterFactory.GetSqlParameter("@UserName", PreApprovalHistory.UserName),
                        SqlParameterFactory.GetSqlParameter("@UserID", PreApprovalHistory.UserId),
                        SqlParameterFactory.GetSqlParameter("@ActionType", PreApprovalHistory.ActionType),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate", PreApprovalHistory.ApproveDate),
                        SqlParameterFactory.GetSqlParameter("@Comments", PreApprovalHistory.Comments),
                        SqlParameterFactory.GetSqlParameter("@Type", PreApprovalHistory.type),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                    });
            }
        }
        public List<P_PreApprovalApproveHistory> LoadApprovalRecords(Guid PID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_PreApprovalApproveHistory>(
                    "SELECT * FROM [P_PreApprovalApproveHistory] WHERE PID =@PID AND (ActionType='2'OR ActionType='3') AND UserId<>'系统自动审批' AND IsDelete<>1 ORDER BY ApproveDate ASC ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID),

                    });
                return list;
            }
        }
        public List<PreApprovalState> LoadMyPreApprovalUserId(string UserId, DateTime Begin, DateTime End, string State, string Budget, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<PreApprovalState>(rows, page, out total,
                    "SELECT preApproval.[ID] as ID,[ApplierName],[ApplierMUDID],[ApplierMobile],preApproval.[CreateDate] as CreateDate,[ModifyDate],[HTCode],preApproval.[Market] as Market,preApproval.[TA] as TA,preApproval.[Province] as Province,preApproval.[City] as City,[HospitalCode],preApproval.[HospitalName] as HospitalName,[HospitalAddress],preApproval.[MeetingName] as MeetingName,[MeetingDate],preApproval.[AttendCount] as AttentCount,[CostCenter],[BudgetTotal],[IsDMFollow],[BUHeadName],[BUHeadMUDID],[BUHeadApproveDate],[IsReAssign],[ReAssignBUHeadName],[ReAssignBUHeadMUDID],[ReAssignBUHeadApproveDate],preApproval.[State] as State,preApproval.[MMCoEImage] as MMCoEImage,preApproval.[MMCoEApproveState] as MMCoEApproveState ,[IsBudgetChange],[IsMMCoEChange],[IsUsed],[IsFinished], ord.State as OrderState FROM [P_PreApproval] preApproval left join [dbo].[P_ORDER] ord on preApproval.htcode=ord.CN and preApproval.IsUsed = 1 and ord.State not in (5, 11) WHERE [ApplierMUDID]=@UserId AND ((@Budget='' and BudgetTotal>=0) or (@Budget='0' and BudgetTotal=0) or (@Budget='1' and BudgetTotal>0)) AND preApproval.[CreateDate] > @begin AND preApproval.[CreateDate] <= @end AND preApproval.[State] in (" + State + ")"
                    , "  ORDER BY preApproval.[CreateDate] DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@end", End),
                        SqlParameterFactory.GetSqlParameter("@Budget", Budget)
                    });
                return list;
            }
        }
        //20190125获当前用户有没有待审批的人
        public List<P_PreApproval> LoadMyApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_PreApproval>(rows, page, out total,
                    $"SELECT * FROM [P_PreApproval] WHERE (([BUHeadMUDID]=@UserId AND IsReAssign=0) or (ReAssignBUHeadMUDID=@UserId and IsReAssign=1)) AND [CreateDate] > @begin AND [CreateDate] <= @end AND [State] in (" + State + ") and ((ApplierName like '%" + Applicant + "%' or '" + Applicant + "'='') or (ApplierMUDID like '%" + Applicant + "%' or '" + Applicant + "'=''))",
                    " ORDER BY CreateDate DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@end", End),
                        SqlParameterFactory.GetSqlParameter("@Applicant", Applicant)
                    });
                return list;
            }
        }
        //20190125新的获当前用户有没有待审批的人
        public List<P_PreApproval> LoadCurrentApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_PreApproval>(rows, page, out total,
                    $"SELECT * FROM [P_PreApproval] WHERE (([CurrentApproverMUDID]=@UserId AND IsReAssign=0) or (ReAssignBUHeadMUDID=@UserId and IsReAssign=1)) AND [CreateDate] > @begin AND [CreateDate] <= @end AND [State] in (" + State + ") and ((ApplierName like '%" + Applicant + "%' or '" + Applicant + "'='') or (ApplierMUDID like '%" + Applicant + "%' or '" + Applicant + "'=''))",
                    " ORDER BY CreateDate DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@end", End),
                        SqlParameterFactory.GetSqlParameter("@Applicant", Applicant)
                    });
                return list;
            }
        }

        public List<P_PreApproval> LoadMyApproveAll(string UserId, string Applicant)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_PreApproval>(
                    $"SELECT * FROM [P_PreApproval] WHERE [BUHeadMUDID]=@UserId AND  State='3' and ((ApplierName like '%" + Applicant + "%' or '" + Applicant + "'='') or (ApplierMUDID like '%" + Applicant + "%' or '" + Applicant + "'=''))",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@Applicant", Applicant)
                    });
                return list;
            }
        }

        public List<P_PreApproval> LoadHTCode(string UserID)
        {
            List<P_PreApproval> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT *  FROM P_PreApproval WHERE (State=5 OR State=6 OR State=9) AND (DATEDIFF(DAY,BUHeadApproveDate,GETDATE())<=30) AND (DATEDIFF(DAY,GETDATE(),MeetingDate)>=0) AND(BudgetTotal>0) AND (IsUsed = 0 ) AND ([ApplierMUDID]=@UserID) ORDER BY MeetingDate",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserID", UserID)
                    });
            }
            return rtnData;
        }
        public List<P_PreApproval> FindPreApprovalByHTCode(string HTCode)
        {
            List<P_PreApproval> rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT * FROM P_PreApproval WHERE HTCode = @HTCode ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode)
                    });
            }
            return rtnData;
        }
        public bool HasApproveRights(string UserId)
        {
            bool result = false;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                //20190225权限
                //var rtnData = sqlServerTemplate.Find<D_CostCenterCount>("select sum(CostCenterCount) as CostCenterCount from(SELECT Count(*) as CostCenterCount FROM D_CostCenter WHERE BUHeadMUDID=@UserId union select count(*) as CostCenterCount from [P_UserDelegate] de inner join[dbo].[D_CostCenter] d on de.UserMUDID = d.BUHeadMUDID and de.StartTime < getdate() and de.EndTime > getdate() and IsEnable=1 WHERE DelegateUserMUDID = @UserId)a",
                //    new SqlParameter[]
                //    {
                //        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                //    });
                var rtnData = sqlServerTemplate.Find<D_CostCenterCount>("select count(*) as CostCenterCount from [dbo].[WP_QYUSER] up inner join [dbo].[WP_QYUSER] u on up.ID=u.LineManagerID where up.UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
                result = rtnData.CostCenterCount > 0;
            }
            return result;
        }
        //查询是否是buhead
        public bool HasApprove(string UserId)
        {
            bool result = false;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<D_CostCenterCount>("SELECT Count(*) as CostCenterCount FROM D_CostCenter WHERE BUHeadMUDID=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
                result = rtnData.CostCenterCount > 0;
            }
            return result;
        }

        public bool HasApproveByTA(string UserId, string TA)
        {
            bool result = false;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<D_CostCenterCount>("SELECT Count(*) as CostCenterCount FROM D_CostCenter WHERE BUHeadMUDID=@UserId AND TERRITORY_TA = @TA",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@TA", TA)
                    });
                result = rtnData.CostCenterCount > 0;
            }
            return result;
        }

        public bool HasFileApproveRights(string UserId)
        {
            bool result = false;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<D_CostCenterCount>("select count(*) as CostCenterCount from [dbo].[WP_QYUSER] up inner join [dbo].[WP_QYUSER] u on up.ID=u.LineManagerID where up.UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
                result = rtnData.CostCenterCount > 0;
            }
            return result;
        }
        public List<P_PreApprovalApproveHistory> FindPreApprovalApproveHistory(Guid PID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PreApprovalApproveHistory>("SELECT * from [P_PreApprovalApproveHistory] WHERE PID=@PID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID)
                    });
                return rtnData;
            }
        }
        public P_PreApprovalApproveHistory LoadApproveHistoryInfo(Guid PID, int Type)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_PreApprovalApproveHistory>("SELECT top 1 * FROM P_PreApprovalApproveHistory WHERE PID=@PID and Type=@Type order by [ApproveDate] desc",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID),
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                    });
                return rtnData;
            }
        }

        public P_PreApprovalApproveHistory LoadApproveHistory(Guid PID, int Type)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_PreApprovalApproveHistory>("SELECT top 1 * FROM P_PreApprovalApproveHistory WHERE PID=@PID and Type=@Type and ActionType in(2,3) order by [ApproveDate] desc",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID),
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                    });
                return rtnData;
            }
        }
        public P_PreApprovalApproveHistory LoadApproveHistoryRefused(Guid PID, int Type, string UserId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_PreApprovalApproveHistory>("SELECT top 1 * FROM P_PreApprovalApproveHistory WHERE PID=@PID and Type=@Type and ActionType =2 and UserId=@UserId order by  [ApproveDate] desc",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PID", PID),
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                    });
                return rtnData;
            }
        }

        #region 使用会议号
        /// <summary>
        /// 使用会议号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int UsedHTCode(string code)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Update(
                    "UPDATE [P_PreApproval] SET IsUsed=1,ActionState='0' WHERE [HTCode]=@HTCode ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", code)
                    });
                return entity;
            }
        }
        #endregion 

        #region 释放会议号
        /// <summary>
        /// 释放会议号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int ReleaseHTCode(string code)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Update(
                    "UPDATE [P_PreApproval] SET IsUsed=0,ActionState='0' WHERE [HTCode]=@HTCode ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", code)
                    });
                return entity;
            }
        }
        #endregion

        public P_ORDER FindActivityOrderByHTCode(string HTCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_ORDER>("SELECT * FROM P_Order WHERE State not in(5,11) and CN=@HTCode",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode)
                    });
                return rtnData;
            }
        }

        public P_PreApproval CheckPreApprovalState(string HTCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_PreApproval>("SELECT * FROM P_PreApproval WHERE HTCode=@HTCode",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode)
                    });
                return rtnData;
            }
        }

        public void FinishPreApproval(Guid OrderId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var entity = sqlServerTemplate.Update(
                    "  update [P_PreApproval] set IsFinished=1,ActionState='0' from[P_PreApproval] pre inner join P_ORDER ord on pre.HTCode = ord.CN where ord.ID = @OrderId ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@OrderId", OrderId)
                    });
            }
        }

        #region 审批状态查询
        private const string _sqlPreApprovalState = @"
                             SELECT b.[ID] AS c0 ,
                                    b.[HT编号] AS c1 ,
                                    b.[申请人姓名] AS c2 ,
                                    b.[申请人MUDID] AS c3 ,
                                    b.[审批类别] AS c4 ,
                                    b.[流程审批状态] AS c5 ,
                                    b.[提交日期] AS c6 ,
                                    b.[提交时间] AS c7 ,
                                    b.[审批人MUDID] AS c8 ,
                                    b.[审批人姓名] AS c9 ,
                                    b.[审批动作] AS c10 ,
                                    b.[审批理由] AS c11 ,
                                    b.[审批日期] AS c12 ,
                                    b.[审批时间] AS c13 ,
                                    b.[细节类型] AS c14
                             FROM Approval_State b WHERE 1=1
                            ";

        public List<Approval_State> LoadPreApprovalState(int rows, int page, out int total)
        {
            List<Approval_State> rtnData = null;
            var listParams = new List<SqlParameter>();
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<Approval_State>(rows, page, out total,
                    _sqlPreApprovalState, "ORDER BY b.[提交日期] DESC, b.[提交时间] DESC ", listParams.ToArray());
            }
            return rtnData;
        }


        public List<Approval_State> LoadUploadPage(string HTCode, string ApplierMUDID, string BUHeadMUDID, string Type, string State, int rows, int page, out int total)
        {
            var _sql = "";
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(HTCode))
            {
                _sql += " AND b.[HT编号] LIKE @HTCode ";
                listParams.Add(new SqlParameter("@HTCode", $"%{HTCode}%"));
            }
            if (!string.IsNullOrEmpty(ApplierMUDID))
            {
                _sql += " AND b.[申请人MUDID] LIKE @ApplierMUDID ";
                listParams.Add(new SqlParameter("@ApplierMUDID", $"%{ApplierMUDID}%"));
            }
            if (!string.IsNullOrEmpty(BUHeadMUDID))
            {
                _sql += " AND b.[审批人MUDID] LIKE @BUHeadMUDID ";
                listParams.Add(new SqlParameter("@BUHeadMUDID", $"%{BUHeadMUDID}%"));
            }
            if (!string.IsNullOrEmpty(Type))
            {
                _sql += " AND b.[审批类别] LIKE @Type ";
                listParams.Add(new SqlParameter("@Type", $"%{Type}%"));
            }
            if (!string.IsNullOrEmpty(State))
            {
                _sql += " AND b.[流程审批状态] LIKE @State ";
                listParams.Add(new SqlParameter("@State", $"%{State}%"));
            }

            List<Approval_State> rtnData = null;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<Approval_State>(rows, page, out total,
                    _sqlPreApprovalState, "ORDER BY b.[提交日期] DESC, b.[提交时间] DESC ", listParams.ToArray());
            }
            return rtnData;
        }

        #endregion

        #region 审批状态查询---搜索       

        public List<Approval_State> QueryLoad(string srh_HTCode, string srh_ApplierMUDID, string srh_BUHeadMUDID, string srh_Category, string srh_Type, int rows, int page, out int total)
        {
            var _sql = "";
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(srh_HTCode))
            {
                _sql += " AND b.[HT编号] LIKE @srh_HTCode ";
                listParams.Add(new SqlParameter("@srh_HTCode", $"%{srh_HTCode}%"));
            }
            if (!string.IsNullOrEmpty(srh_ApplierMUDID))
            {
                _sql += " AND b.[申请人MUDID] LIKE @srh_ApplierMUDID ";
                listParams.Add(new SqlParameter("@srh_ApplierMUDID", $"%{srh_ApplierMUDID}%"));
            }
            if (!string.IsNullOrEmpty(srh_BUHeadMUDID))
            {
                _sql += " AND b.[审批人MUDID] LIKE @srh_BUHeadMUDID ";
                listParams.Add(new SqlParameter("@srh_BUHeadMUDID", $"%{srh_BUHeadMUDID}%"));
            }
            if (!string.IsNullOrEmpty(srh_Category))
            {
                _sql += " AND b.[审批类别] LIKE @srh_Category ";
                listParams.Add(new SqlParameter("@srh_Category", $"%{srh_Category}%"));
            }
            if (!string.IsNullOrEmpty(srh_Type))
            {
                _sql += " AND b.[流程审批状态] LIKE @srh_Type ";
                listParams.Add(new SqlParameter("@srh_Type", $"%{srh_Type}%"));
            }
            List<Approval_State> rtnData = null;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<Approval_State>(rows, page, out total,
                    _sqlPreApprovalState + _sql, "ORDER BY b.[提交日期] DESC, b.[提交时间] DESC ", listParams.ToArray());
            }
            return rtnData;
        }





        #endregion

        #region 上传文件重新分配---搜索       

        public List<Approval_State> LoadPreApprovalUpload(string MUDID, int rows, int page, out int total)
        {
            var _sql = "";
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(MUDID))
            {
                _sql += " AND b.[申请人MUDID] LIKE @srh_ApplierMUDID ";
                listParams.Add(new SqlParameter("@srh_ApplierMUDID", $"%{MUDID}%"));
            }

            _sql += " AND b.[审批类别] LIKE @srh_Category ";
            listParams.Add(new SqlParameter("@srh_Category", $"%{"上传文件"}%"));

            List<Approval_State> rtnData = null;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<Approval_State>(rows, page, out total,
                    _sqlPreApprovalState + _sql, "ORDER BY b.[提交日期] DESC, b.[提交时间] DESC ", listParams.ToArray());
            }
            return rtnData;
        }

        #endregion

        public List<P_PreApproval> LoadPreApprovalByCostCenter(string CostCenter)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT * FROM P_PreApproval WHERE State=3 and [IsReAssign]=0 and CostCenter=@CostCenter",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CostCenter", CostCenter)
                    });
                return rtnData;
            }
        }


        public List<P_PreApproval> LoadPreApprovalByCurrentApprover(string CurrentApprover, string CostCenter)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT * FROM P_PreApproval WHERE State=3 and [IsReAssign]=0 and CurrentApproverMUDID=@CurrentApproverMUDID and CostCenter=@CostCenter",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverMUDID", CurrentApprover),
                        SqlParameterFactory.GetSqlParameter("@CostCenter", CostCenter)
                    });
                return rtnData;
            }
        }

        public int UpdatePendingPreAPprovalBUHead(string HTCode, string BUHeadName, string BUHeadMUDID, string PreBUHeadMUDID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.ExecuteNonQuery(
                    //"update P_PreApproval set BUHeadMUDID=@BUHeadMUDID ,BUHeadName=@BUHeadName,CurrentApproverName=@CurrentApproverName,CurrentApproverMUDID=@CurrentApproverMUDID where HTCode=@HTCode and BUHeadMUDID=@PreBUHeadMUDID",
                    "update P_PreApproval set BUHeadMUDID=@BUHeadMUDID ,BUHeadName=@BUHeadName,CurrentApproverName=@CurrentApproverName,CurrentApproverMUDID=@CurrentApproverMUDID,ActionState='0' where HTCode=@HTCode and CurrentApproverMUDID=@PreBUHeadMUDID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@BUHeadMUDID", BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@BUHeadName", BUHeadName),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverName", BUHeadName),
                        SqlParameterFactory.GetSqlParameter("@CurrentApproverMUDID", BUHeadMUDID),
                        SqlParameterFactory.GetSqlParameter("@PreBUHeadMUDID", PreBUHeadMUDID)

                    });
                return res;
            }
        }



        public List<Approval_State> LoadByID(string ids)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.Load<Approval_State>(
                    _sqlPreApprovalState + " AND b.[ID] IN ('" + ids + "')",
                    new SqlParameter[]
                    {
                     });
                return res;
            }
        }


        public int UpdatePreReAssign(string ids, string userId, string name, string mudId, string headName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.ExecuteNonQuery(
                    "update P_PreApproval set IsReAssign=1, ReAssignOperatorMUDID=@ReAssignOperatorMUDID, ReAssignOperatorName=@ReAssignOperatorName, ReAssignBUHeadMUDID=@ReAssignBUHeadMUDID,"
                  + " ReAssignBUHeadName=@ReAssignBUHeadName, ReAssignBUHeadApproveDate=GETDATE(),ActionState='0' where ID IN ('" + ids + "')",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ReAssignOperatorMUDID", userId),
                        SqlParameterFactory.GetSqlParameter("@ReAssignOperatorName", name),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadMUDID", mudId),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadName", headName)
                    });
                return res;
            }
        }


        public int UpdatePuoReAssign(string ids, string userId, string name, string mudId, string headName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.ExecuteNonQuery(
                    "update P_PreUploadOrder set IsReAssign=1, ReAssignOperatorMUDID=@ReAssignOperatorMUDID, ReAssignOperatorName=@ReAssignOperatorName, ReAssignBUHeadMUDID=@ReAssignBUHeadMUDID,"
                  + " ReAssignBUHeadName=@ReAssignBUHeadName, ReAssignBUHeadApproveDate=GETDATE(),ActionState='0' where ID IN ('" + ids + "')",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ReAssignOperatorMUDID", userId),
                        SqlParameterFactory.GetSqlParameter("@ReAssignOperatorName", name),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadMUDID", mudId),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadName", headName)
                    });
                return res;
            }
        }

        public int UpdatePuoReAssignByHTCode(string HtCode, string UserId, string Name, string MUDID, string HeadName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var res = sqlServerTemplate.ExecuteNonQuery(
                    "update P_PreUploadOrder set IsReAssign=1, ReAssignOperatorMUDID=@ReAssignOperatorMUDID, ReAssignOperatorName=@ReAssignOperatorName, ReAssignBUHeadMUDID=@ReAssignBUHeadMUDID,"
                  + " ReAssignBUHeadName=@ReAssignBUHeadName, ReAssignBUHeadApproveDate=GETDATE(),ActionState='0' where HTCode=@HTCode",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HtCode),
                        SqlParameterFactory.GetSqlParameter("@ReAssignOperatorMUDID", UserId),
                        SqlParameterFactory.GetSqlParameter("@ReAssignOperatorName", Name),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadMUDID", MUDID),
                        SqlParameterFactory.GetSqlParameter("@ReAssignBUHeadName", HeadName)
                    });
                return res;
            }
        }

        public int InsertFileLink(string FilePath, string Email)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "INSERT P_FileLink ([Id],[FilePath],[Email],[State],[CreateDate]) "
                    + " VALUES (@ID,@FilePath,@Email,@State,@CreateDate) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@FilePath", FilePath),
                        SqlParameterFactory.GetSqlParameter("@Email", Email),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@State", 0)
                    });
            }
        }


        #region 更新预申请医院地址
        /// <summary>
        /// 更新预申请医院地址
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int UpdateAddress(string preApprovalId, string hospitalAddress)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_PreApproval SET HospitalAddress=@HospitalAddress,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.Parse(preApprovalId)),
                        SqlParameterFactory.GetSqlParameter("@HospitalAddress", hospitalAddress)
                    });
            }
        }
        #endregion

        public string GetOldGskHospitalCodeByGskHospitalCode(string GskHospitalCode)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<P_HOSPITAL>("SELECT * FROM P_HOSPITAL WHERE GskHospital=@GskHospitalCode and OldGskHospital<>''",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospitalCode", GskHospitalCode)
                    });
                return rtnData == null ? string.Empty : rtnData.OldGskHospital;
            }
        }

        public string GetOldCostcenterByCostcenter(string Costcenter)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Find<D_COSTCENTER>("SELECT * FROM D_CostCenter WHERE Costcenter=@Costcenter and OldCostcenter<>''",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Costcenter", Costcenter)
                    });
                return rtnData == null ? string.Empty : rtnData.OldCostCenter;
            }
        }

        //查询未审批的预申请
        public List<P_PreApproval> GetPreApprovalByUser()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT pre.* FROM P_PreApproval pre Join WP_QYUSER qu on pre.CurrentApproverMUDID = qu.userid  WHERE qu.state = 4 and (pre.State=3 or pre.State = 7) ",
                    new SqlParameter[]
                    {

                    });
                return rtnData;
            }
        }

        public List<P_PreApproval> GetPreApprovalByUser(string UserID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT pre.* FROM P_PreApproval pre WHERE pre.CurrentApproverMUDID = @UserID and (pre.State=3 or pre.State = 7) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserID", UserID)
                    });
                return rtnData;
            }
        }

        #region 新增地址
        public List<P_AddressApproval> LoadMyAddressApprovalByUserId(string UserId, DateTime Begin, DateTime End, string State, string Budget, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_AddressApproval>(rows, page, out total,
                    "SELECT case when a.IsDeleteUpdate is null then 0 else a.IsDeleteUpdate end as IsDel, a.* FROM [P_AddressApproval] a left join (select * from P_HOSPITAL where MainAddress = N'主地址') b on a.GskHospital = b.GskHospital WHERE a.[ApplierMUDID]=@UserId AND a.[CreateDate] > @begin AND a.[CreateDate] <= @end AND a.[ApprovalStatus] in (" + State + ")"
                    , "  ORDER BY a.[CreateDate] DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@end", End)
                    });
                return list;
            }
        }

        public List<P_AddressApproval> LoadMyAddressApprove(string UserId, DateTime Begin, DateTime End, string State, string Applicant, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_AddressApproval>(rows, page, out total,
                    $"SELECT case when a.IsDeleteUpdate is null then 0 else a.IsDeleteUpdate end as IsDel, a.*  FROM [P_AddressApproval] a left join (select * from P_HOSPITAL where MainAddress = N'主地址') b on a.GskHospital = b.GskHospital WHERE (a.[LineManagerMUDID]=@UserId) AND a.[CreateDate] > @begin AND a.[CreateDate] <= @end AND a.[ApprovalStatus] in (" + State + ") and ((a.ApplierName like '%" + Applicant + "%' or '" + Applicant + "'='') or (a.ApplierMUDID like '%" + Applicant + "%' or '" + Applicant + "'=''))",
                    " ORDER BY a.CreateDate DESC", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId),
                        SqlParameterFactory.GetSqlParameter("@begin", Begin),
                        SqlParameterFactory.GetSqlParameter("@end", End),
                        SqlParameterFactory.GetSqlParameter("@Applicant", Applicant)
                    });
                return list;
            }
        }

        public P_AddressApproval_View LoadAddressApprovalInfo(Guid id)
        {
            List<P_HOSPITAL_NEW> p_hospital_list = new List<P_HOSPITAL_NEW>();
            P_AddressApproval_View p_addressApproval_View = new P_AddressApproval_View();
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var result = sqlServerTemplate.Find<P_AddressApproval_View>($"SELECT a.AddressNameDisplay, case when a.IsDeleteUpdate is null then 0 else a.IsDeleteUpdate end as IsDeleteUpdate, c.ApproveDate as RejectViewDate, c.ActionType as RejectViewResult, c.Comments as RejectViewReason,c.UserId as RejectViewLinemanagerMUDID,c.UserName as RejectViewLinemanagerName, a.*, a.MAddress as MainHospitalAddress,b.MainAddress,b.Type, b.CityId, b.ProvinceId,b.FirstLetters,b.Latitude as MainLat,b.Longitude as MainLng FROM [P_AddressApproval] a left join (select * from P_HOSPITAL where MainAddress = N'主地址') b on a.GskHospital = b.GskHospital "
                    + " left join (select top 1* from P_AddressApproveHistory where DA_ID = @ID and ActionType in(2,6,8) order by ApproveDate desc )c on a.ID = c.DA_ID where a.ID=@ID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
                if (result != null)
                {
                    List<string> distanceList = new List<string>();
                    if (result.OtherAddressDistance != "")
                    {
                        distanceList = result.OtherAddressDistance.Substring(0, result.OtherAddressDistance.Length - 1).Split(';').ToList();
                    }
                    var list = sqlServerTemplate.Load<P_HOSPITAL_NEW>(
                        " SELECT * FROM [P_HOSPITAL] "
                        + " WHERE GskHospital = @GskHospital and MainAddress <> N'主地址' and IsDelete = 0 "
                        + " ORDER BY MainAddress ",
                        new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", result.GskHospital)
                        });
                    if (list != null)
                    {
                        List<string> otherAddressList = new List<string>();
                        foreach (P_HOSPITAL_NEW item in list)
                        {
                            otherAddressList.Add(item.MainAddress + ":" + item.Address);
                        }
                        result.AddressList = otherAddressList;
                        result.DistanceList = distanceList;
                    }

                    var inProgressList = sqlServerTemplate.Load<P_AddressApproval>(
                    " SELECT * FROM [P_AddressApproval] WHERE GskHospital = @GskHospital ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@GskHospital", result.GskHospital)
                    });
                    if (inProgressList != null)
                    {
                        result.inProgressList = inProgressList;
                    }

                }
                return result;
            }
        }

        public P_AddressApproval_View LoadAddressApprovalInfoForUpdate(Guid id)
        {
            List<P_HOSPITAL_NEW> p_hospital_list = new List<P_HOSPITAL_NEW>();
            P_AddressApproval_View p_addressApproval_View = new P_AddressApproval_View();
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var result = sqlServerTemplate.Find<P_AddressApproval_View>($"select b.address as MainHospitalAddress, a.AddAddress as AddAddress,b.Name as HospitalName,a.Province,a.City, b.Latitude as MainLat,b.Longitude as MainLng "
                    + " FROM [P_AddressApproval] a left join (select * from P_HOSPITAL where MainAddress = N'主地址') b on a.GskHospital = b.GskHospital where a.ID=@ID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
                return result;
            }
        }

        public int AddressApprove(P_AddressApproval_View p_addressApproval_View, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            int approveStatus = 0;
            SqlParameter[] parameters = null;
            string sql = "";
            //通过
            if (reason == "")
            {
                //新提交
                if (p_addressApproval_View.ApprovalStatus == 0)
                    approveStatus = 1;
                //修改后
                else if (p_addressApproval_View.ApprovalStatus == 9)
                    approveStatus = 5;
                //重新提交后
                else if (p_addressApproval_View.ApprovalStatus == 10)
                    approveStatus = 7;
                sql = "UPDATE P_AddressApproval SET ApprovalStatus = @ApprovalStatus, LineManagerApproveDate = @LineManagerApproveDate where ID = @ID";
                parameters = new SqlParameter[]
                {
                    SqlParameterFactory.GetSqlParameter("@ApprovalStatus", approveStatus),
                    SqlParameterFactory.GetSqlParameter("@LineManagerApproveDate", DateTime.Now),
                    SqlParameterFactory.GetSqlParameter("@ID", p_addressApproval_View.ID)
                };
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();

                    var res = sqlServerTemplate.ExecuteNonQuery(sql, parameters);

                    if (res > -1)
                    {
                        res = sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproveHistory]([ID],[DA_ID],[UserName],[UserId],[ActionType],[ApproveDate],[Comments],[Type],[IsDelete])" +
                                                        " values (@ID, @DA_ID, @UserName, @UserId, @ActionType, @ApproveDate, @Comments, @Type, @IsDelete) ",
                            new SqlParameter[]
                            {
                                SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                SqlParameterFactory.GetSqlParameter("@DA_ID", p_addressApproval_View.ID),
                                SqlParameterFactory.GetSqlParameter("@UserName", p_addressApproval_View.LineManagerName),
                                SqlParameterFactory.GetSqlParameter("@UserId", p_addressApproval_View.LineManagerMUDID),
                                SqlParameterFactory.GetSqlParameter("@ActionType", approveStatus),
                                SqlParameterFactory.GetSqlParameter("@ApproveDate", DateTime.Now),
                                SqlParameterFactory.GetSqlParameter("@Comments", ""),
                                SqlParameterFactory.GetSqlParameter("@Type", 0),
                                SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                            });
                        if (res > 0)
                        {
                            res = sqlServerTemplate.ExecuteNonQuery("INSERT INTO [P_HOSPITAL] values (@CityId,@GskHospital,@Name,@FirstLetters,@Address,@Latitude,@Longitude,@Type,@External,@CreateDate,@ProvinceId,@IsXMS,@IsBDS,@IsMT,@Remark,@IsDelete,@OldGskHospital,@OldName,@RelateUserList,@MainAddress,@HospitalCode )",
                            new SqlParameter[]
                                        {
                                        SqlParameterFactory.GetSqlParameter("@CityId",p_addressApproval_View.CityId),
                                        SqlParameterFactory.GetSqlParameter("@GskHospital",p_addressApproval_View.GskHospital),
                                        SqlParameterFactory.GetSqlParameter("@Name",p_addressApproval_View.HospitalName),
                                        SqlParameterFactory.GetSqlParameter("@FirstLetters",p_addressApproval_View.FirstLetters),
                                        SqlParameterFactory.GetSqlParameter("@Address",p_addressApproval_View.AddAddress),
                                        SqlParameterFactory.GetSqlParameter("@Latitude",p_addressApproval_View.Latitude),
                                        SqlParameterFactory.GetSqlParameter("@Longitude",p_addressApproval_View.Longitude),
                                        SqlParameterFactory.GetSqlParameter("@Type",p_addressApproval_View.Type),
                                        SqlParameterFactory.GetSqlParameter("@External",0),
                                        SqlParameterFactory.GetSqlParameter("@CreateDate",p_addressApproval_View.CreateDate),
                                        SqlParameterFactory.GetSqlParameter("@ProvinceId",p_addressApproval_View.ProvinceId),
                                        SqlParameterFactory.GetSqlParameter("@IsXMS","是"),
                                        SqlParameterFactory.GetSqlParameter("@IsBDS","是"),
                                        SqlParameterFactory.GetSqlParameter("@IsMT","否"),
                                        SqlParameterFactory.GetSqlParameter("@Remark",""),
                                        SqlParameterFactory.GetSqlParameter("@IsDelete",0),
                                        SqlParameterFactory.GetSqlParameter("@OldGskHospital",""),
                                        SqlParameterFactory.GetSqlParameter("@OldName",""),
                                        SqlParameterFactory.GetSqlParameter("@RelateUserList",""),
                                        SqlParameterFactory.GetSqlParameter("@MainAddress",p_addressApproval_View.AddressName),
                                        SqlParameterFactory.GetSqlParameter("@HospitalCode",p_addressApproval_View.HospitalCode)
                                        });
                        }
                        else
                            return 0;
                    }
                    return res;
                }
            }
            //驳回
            else
            {
                //新提交
                if (p_addressApproval_View.ApprovalStatus == 0)
                    approveStatus = 2;
                //修改后
                else if (p_addressApproval_View.ApprovalStatus == 9)
                    approveStatus = 6;
                //重新提交后
                else if (p_addressApproval_View.ApprovalStatus == 10)
                    approveStatus = 8;
                sql = "UPDATE P_AddressApproval SET ApprovalStatus = @ApprovalStatus, RejectReason = @RejectReason, LineManagerApproveDate = @LineManagerApproveDate where ID = @ID";
                parameters = new SqlParameter[]
                {
                    SqlParameterFactory.GetSqlParameter("@ApprovalStatus", approveStatus),
                    SqlParameterFactory.GetSqlParameter("@RejectReason", reason),
                    SqlParameterFactory.GetSqlParameter("@LineManagerApproveDate", DateTime.Now),
                    SqlParameterFactory.GetSqlParameter("@ID", p_addressApproval_View.ID)
                };
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();

                    var res = sqlServerTemplate.ExecuteNonQuery(sql, parameters);
                    if (res > -1)
                    {
                        res = sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproveHistory]([ID],[DA_ID],[UserName],[UserId],[ActionType],[ApproveDate],[Comments],[Type],[IsDelete])" +
                                                        " values (@ID, @DA_ID, @UserName, @UserId, @ActionType, @ApproveDate, @Comments, @Type, @IsDelete) ",
                            new SqlParameter[]
                            {
                                SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                SqlParameterFactory.GetSqlParameter("@DA_ID", p_addressApproval_View.ID),
                                SqlParameterFactory.GetSqlParameter("@UserName", p_addressApproval_View.LineManagerName),
                                SqlParameterFactory.GetSqlParameter("@UserId", p_addressApproval_View.LineManagerMUDID),
                                SqlParameterFactory.GetSqlParameter("@ActionType", approveStatus),
                                SqlParameterFactory.GetSqlParameter("@ApproveDate", DateTime.Now),
                                SqlParameterFactory.GetSqlParameter("@Comments", reason),
                                SqlParameterFactory.GetSqlParameter("@Type", 0),
                                SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                            });
                    }
                    return res;
                }
            }


        }

        public int AddAddressApproveHistory(P_AddressApproveHistory p_addressApproveHistory)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproveHistory] values(@ID,@DA_ID,@UserName,@UserID,@ActionType,@ApproveDate,@Comments,@Type,@IsDelete) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", p_addressApproveHistory.ID),
                        SqlParameterFactory.GetSqlParameter("@DA_ID", p_addressApproveHistory.DA_ID),
                        SqlParameterFactory.GetSqlParameter("@UserName", p_addressApproveHistory.UserName),
                        SqlParameterFactory.GetSqlParameter("@UserID", p_addressApproveHistory.UserId),
                        SqlParameterFactory.GetSqlParameter("@ActionType", p_addressApproveHistory.ActionType),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@Comments", p_addressApproveHistory.Comments),
                        SqlParameterFactory.GetSqlParameter("@Type", p_addressApproveHistory.type),
                        SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                    });
            }
        }

        public List<P_AddressApproval_View> LoadMyAddressApproveAll(string UserId, string Applicant)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_AddressApproval_View>(
                    "SELECT a.*, b.Address as MainHospitalAddress, b.MainAddress, b.Type, b.CityId, b.ProvinceId,b.FirstLetters FROM [P_AddressApproval] a left join(select * from P_HOSPITAL where MainAddress = N'主地址') b on a.GskHospital = b.GskHospital"
                    + " WHERE a.[LineManagerMUDID]=@UserId AND  a.ApprovalStatus in( 0, 9, 10) and ((a.ApplierName like '%" + Applicant + "%' or '" + Applicant + "'='') or (a.ApplierMUDID like '%" + Applicant + "%' or '" + Applicant + "'=''))",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", UserId)
                    });
                return list;
            }
        }

        public List<P_AddressApproval> LoadInvalidAddressApplication(DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_AddressApproval>($"SELECT * FROM [P_AddressApproval] WITH(NOLOCK) WHERE ApprovalStatus in(0,9,10) AND CreateDate < @nowDate ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@nowDate", nowDate)
                    });
                return list;
            }
        }

        public int InvalidAddressApplication(List<Guid> guids, int state)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            var num = 0;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                foreach (var _id in guids)
                {
                    sqlServerTemplate.ExecuteNonQuery("insert into [P_AddressApproveHistory]([ID],[DA_ID],[UserName],[UserId],[ActionType],[ApproveDate],[Comments],[Type],[IsDelete])" +
                                                        " values (@ID, @DA_ID, @UserName, @UserId, @ActionType, @ApproveDate, @Comments, @Type, @IsDelete) ",
                            new SqlParameter[]
                            {
                                SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                                SqlParameterFactory.GetSqlParameter("@DA_ID", _id),
                                SqlParameterFactory.GetSqlParameter("@UserName", "系统自动审批"),
                                SqlParameterFactory.GetSqlParameter("@UserId", "系统自动审批"),
                                SqlParameterFactory.GetSqlParameter("@ActionType", 3),
                                SqlParameterFactory.GetSqlParameter("@ApproveDate", DateTime.Now),
                                SqlParameterFactory.GetSqlParameter("@Comments", "申请已失效，直线经理５个自然日内未审批"),
                                SqlParameterFactory.GetSqlParameter("@Type", 0),
                                SqlParameterFactory.GetSqlParameter("@IsDelete", 0),
                            });

                    sqlServerTemplate.ExecuteNonQuery("Update [P_AddressApproval] set ApprovalStatus = 3 where ID=@ID", new SqlParameter[] {
                                        SqlParameterFactory.GetSqlParameter("@ID", _id)
                                        });
                    num++;
                }
            }
            return num;
        }

        public List<P_AddressApproval_View> LoadAddressApprove(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID, string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete, int rows, int page, out int total)
        {
            List<P_AddressApproval_View> rtnData;
            string selectSqlForLoad = " select * from (select p.ID, p.DACode as DACode,p.ApplierName as ApplierName,p.ApplierMUDID as ApplierMUDID,case when u.Position is null then '' else u.Position end as Position, "
                                    + " p.CreateDate as CreateDate,p.Market as Market, p.TA as TA, p.GskHospital as GskHospital, p.Province as Province, p.City as City,p.HospitalName as HospitalName, "
                                    //+ " case when b.Address is null then p.MAddress else b.Address end as MainHospitalAddress, p.AddressName as AddressName,p.AddAddress as AddAddress, p.District as District, "
                                    + " p.MAddress as MainHospitalAddress, p.AddressName as AddressName,p.AddAddress as AddAddress, p.District as District, "
                                    + " p.Distance, b.Remark as Remark, "
                                    + " p.ApprovalStatus, "
                                    + " case when p.RejectReason is null then '' else p.RejectReason end  as RejectReason, "
                                    + " p.LineManagerName as LineManagerName, p.LineManagerMUDID as LineManagerMUDID, "
                                    + " p.LineManagerApproveDate , "
                                    + " case when p.IsDeleteUpdate is null then 0 else p.IsDeleteUpdate end as IsDelete, "
                                    + " p.AddressNameDisplay, p.DeleteDate "
                                    + " from[P_AddressApproval] p "
                                    + " left join " + _dbName + ".[dbo].[WP_QYUSER] U on p.ApplierMUDID = u.UserId "
                                    + " left join(select* from P_HOSPITAL where MainAddress = N'主地址') b on p.GskHospital = b.GskHospital ) s where 1=1 ";
            //+ " left join  P_HOSPITAL c on c.HospitalCode = p.HospitalCode where 1=1 ";
            if (!string.IsNullOrEmpty(srh_DACode))
            {
                selectSqlForLoad += " and (s.DACode LIKE '%' + @DACode + '%')";
            }
            if (!string.IsNullOrEmpty(srh_ApplierMUDID))
            {
                selectSqlForLoad += " and (s.ApplierMUDID LIKE '%' + @ApplierMUDID + '%')";
            }
            if (!string.IsNullOrEmpty(srh_ApproverMUDID))
            {
                selectSqlForLoad += " and (s.LineManagerMUDID LIKE '%' + @LineManagerMUDID + '%')";
            }
            if (!string.IsNullOrEmpty(srh_GskHospital))
            {
                selectSqlForLoad += " AND (s.GskHospital LIKE '%' + @GskHospital + '%')";
            }
            if (!string.IsNullOrEmpty(srh_State))
            {
                selectSqlForLoad += " AND (s.ApprovalStatus in (" + srh_State + "))";
            }
            if (!string.IsNullOrEmpty(srh_IsDelete))
            {
                selectSqlForLoad += " AND (s.IsDelete in (" + srh_IsDelete + ") )";
            }
            if (!string.IsNullOrEmpty(srh_StartApplyDate))
            {
                selectSqlForLoad += " AND (s.CreateDate >= @CreateDate)";
            }
            if (!string.IsNullOrEmpty(srh_EndApplyDate))
            {
                selectSqlForLoad += " AND (s.CreateDate <= @EndDate)";
            }

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {

                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_AddressApproval_View>(rows, page, out total, selectSqlForLoad, "order by s.CreateDate desc",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DACode", srh_DACode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", srh_ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@LineManagerMUDID", srh_ApproverMUDID),
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", srh_StartApplyDate==""?"1900-1-1":srh_StartApplyDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", srh_EndApplyDate==""?"9999-12-31":srh_EndApplyDate+" 23:59:59.999")
                    });
            }
            return rtnData;


        }

        public List<WP_QYUSER> LoadWPQYUSER()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<WP_QYUSER>($"SELECT * FROM [WP_QYUSER]",
                    new SqlParameter[]
                    {

                    });
                return list;
            }
        }

        public List<P_AddressApproveHistory> LoadAddressApprovalHistory(Guid DA_ID)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_AddressApproveHistory>($"SELECT * FROM [P_AddressApproveHistory] WHERE DA_ID=@DA_ID  order by ApproveDate ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DA_ID", DA_ID)
                    });
                return list;
            }
        }

        public List<P_AddressApproval_View> ExportAddressApprovalList(string srh_DACode, string srh_ApplierMUDID, string srh_ApproverMUDID, string srh_GskHospital, string srh_StartApplyDate, string srh_EndApplyDate, string srh_State, string srh_IsDelete)
        {
            List<P_AddressApproval_View> rtnData;
            string selectSqlForLoad = " select * from (select p.*,case when u.Position is null then '' else u.Position end as Position,p.MAddress as MainHospitalAddress,b.Remark AS Remark, case when p.IsDeleteUpdate is null then 0 else p.IsDeleteUpdate end as IsDelete from [P_AddressApproval] p left join " + _dbName + ".[dbo].[WP_QYUSER] U on p.ApplierMUDID = u.UserId"
                                    + " left join(select * from P_HOSPITAL where MainAddress = N'主地址') b on p.GskHospital = b.GskHospital ) s where 1=1 ";
            if (!string.IsNullOrEmpty(srh_DACode))
            {
                selectSqlForLoad += " and (s.DACode LIKE '%' + @DACode + '%')";
            }
            if (!string.IsNullOrEmpty(srh_ApplierMUDID))
            {
                selectSqlForLoad += " and (s.ApplierMUDID LIKE '%' + @ApplierMUDID + '%')";
            }
            if (!string.IsNullOrEmpty(srh_ApproverMUDID))
            {
                selectSqlForLoad += " and (s.LineManagerMUDID LIKE '%' + @LineManagerMUDID + '%')";
            }
            if (!string.IsNullOrEmpty(srh_GskHospital))
            {
                selectSqlForLoad += " AND (s.GskHospital LIKE '%' + @GskHospital + '%')";
            }
            if (!string.IsNullOrEmpty(srh_State))
            {
                selectSqlForLoad += " AND (s.ApprovalStatus in (" + srh_State + "))";
            }
            //if (!string.IsNullOrEmpty(srh_IsDelete))
            //{
            //    selectSqlForLoad += " AND (IsDelete in (" + srh_IsDelete + "))";
            //}
            if (!string.IsNullOrEmpty(srh_IsDelete))
            {
                selectSqlForLoad += " AND (s.isdelete in (" + srh_IsDelete + "))";
            }
            if (!string.IsNullOrEmpty(srh_StartApplyDate))
            {
                selectSqlForLoad += " AND (s.CreateDate >= @CreateDate)";
            }
            if (!string.IsNullOrEmpty(srh_EndApplyDate))
            {
                selectSqlForLoad += " AND (s.CreateDate <= @EndDate)";
            }

            selectSqlForLoad += "order by s.CreateDate desc";

            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_AddressApproval_View>(selectSqlForLoad,
                new SqlParameter[]
                    {
                       SqlParameterFactory.GetSqlParameter("@DACode", srh_DACode),
                        SqlParameterFactory.GetSqlParameter("@ApplierMUDID", srh_ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@LineManagerMUDID", srh_ApproverMUDID),
                        SqlParameterFactory.GetSqlParameter("@GskHospital", srh_GskHospital),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", srh_StartApplyDate==""?"1900-1-1":srh_StartApplyDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", srh_EndApplyDate==""?"9999-12-31":srh_EndApplyDate+" 23:59:59.999")
                    });
            }
            return rtnData;
        }

        public List<P_AddressApproval> LoadAddressApprovalByDACode(string dA_CODE)
        {
            List<P_AddressApproval> address_list = new List<P_AddressApproval>();
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                address_list = sqlServerTemplate.Load<P_AddressApproval>("select * from P_AddressApproval where DACode = @DACode",
                new SqlParameter[]
                    {
                       SqlParameterFactory.GetSqlParameter("@DACode", dA_CODE)
                    });
                return address_list;
            }
        }

        public List<P_AddressApproval> LoadMyAddressApproveCount(object userId, object begin, object end, object State, object Applicant)
        {
            List<P_AddressApproval> address_list = new List<P_AddressApproval>();
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                address_list = sqlServerTemplate.Load<P_AddressApproval>(
                    "SELECT * FROM [P_AddressApproval] WHERE ([LineManagerMUDID]=@UserId) AND [CreateDate] > @begin AND [CreateDate] <= @end AND [ApprovalStatus] in (" + State + ") and ((ApplierName like '%" + Applicant + "%' or '" + Applicant + "'='') or (ApplierMUDID like '%" + Applicant + "%' or '" + Applicant + "'='')) ORDER BY CreateDate DESC ",
                new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId),
                        SqlParameterFactory.GetSqlParameter("@begin", begin),
                        SqlParameterFactory.GetSqlParameter("@end", end),
                        SqlParameterFactory.GetSqlParameter("@Applicant", Applicant)
                    });
                return address_list;
            }
        }
        #endregion

        #region 取消预申请
        public int PreApprovalCancel(P_PreApproval p_preApproval)
        {
            var num = 0;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                num = sqlServerTemplate.ExecuteNonQuery("UPDATE P_PreApproval SET State=10,ActionState='0' WHERE ID=@ID ",
                 new SqlParameter[]
                 {
                        SqlParameterFactory.GetSqlParameter("@ID", p_preApproval.ID)

                 });
                if (num > 0)
                {
                    num = sqlServerTemplate.ExecuteNonQuery(
                    "INSERT INTO P_PreApprovalApproveHistory (ID,PID,UserName,UserId,ActionType,ApproveDate,Comments,type) "
                    + " VALUES (@ID,@PID,@UserName,@UserId,@ActionType,@ApproveDate,@Comments,@type) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", Guid.NewGuid()),
                        SqlParameterFactory.GetSqlParameter("@PID", p_preApproval.ID),
                        SqlParameterFactory.GetSqlParameter("@UserName", p_preApproval.ApplierName),
                        SqlParameterFactory.GetSqlParameter("@UserId", p_preApproval.ApplierMUDID),
                        SqlParameterFactory.GetSqlParameter("@ActionType", 5),
                        SqlParameterFactory.GetSqlParameter("@ApproveDate",DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@Comments", ""),
                        SqlParameterFactory.GetSqlParameter("@type", 10)

                    });
                }
            }
            return num;
        }
        #endregion

        #region 根据成本中心表TERRITORY_TA查询预申请表待审批状态数据
        public List<P_PreApproval> GetDataByTAAndState(string TERRITORY_TA)
        {
            List<P_PreApproval> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_PreApproval>("SELECT * FROM [P_PreApproval]  "
                    + "WHERE [TA]=@TA AND [State]='3' ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@TA", TERRITORY_TA)
                    });
            }
            return rtnData;
        }
        #endregion


        #region 费用分析前台-预申请分析
        public List<P_PreApproval_CountAmount_View> LoadPreApprovalData(string userId, string position, string territoryCode, string begin, string end)
        {
            string sqlString = string.Empty;
            try
            {
                List<P_PreApproval_CountAmount_View> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionDate = " WHERE MeetingDate >=  '" + begin + "' AND MeetingDate <  '" + end + "' ";

                if (position.ToUpper() == "RM")
                {
                    sqlString = " SELECT m.DMTerritoryCode,m.DMName,m.MRTerritoryCode,m.MRName, "
                        + "sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + " sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13,2),SUM(m.TotalBudget)) as TotalPrice,m.PreState from  "
                        + " (SELECT d.TERRITORY_DM AS DMTerritoryCode,d.DMName,e.TERRITORY_MR AS MRTerritoryCode,e.MRName, "
                        + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + "  case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount,COUNT(a.ID) as PreCount, "
                        + " SUM(a.BudgetTotal) as TotalBudget,case when a.[State] in ('5','6','9') then N'审批通过' "
                        + "when a.[State] in ('2','4','8') then N'审批驳回' when a.[State] in ('0','1','3','7') then N'待审批'  "
                        + "when a.[State] = 10 then N'已取消' end as PreState FROM (SELECT * FROM  dbo.P_PreApproval_COST  "+ conditionDate + " ) a  "
                        + "INNER JOIN ( SELECT DISTINCT b.TERRITORY_DM,b.MUD_ID_DM,CASE WHEN  b.TERRITORY_DM is not null and b.TERRITORY_DM<>'' "
                        + " and (b.MUD_ID_DM is null or b.MUD_ID_DM='') THEN '空岗' ELSE c.Name END AS DMName "
                        + " FROM   " + _NondbName + "  .dbo.Territory_Hospital b  "
                        + "  LEFT JOIN dbo.WP_QYUSER c ON b.MUD_ID_DM = c.UserId WHERE b.TERRITORY_RM = '" + territoryCode + "') d ON a.DMTerritoryCode = d.TERRITORY_DM "
                        + " INNER JOIN (  SELECT DISTINCT b.TERRITORY_MR,b.MUD_ID_MR,CASE WHEN  b.TERRITORY_MR is not null and b.TERRITORY_MR<>'' "
                        + " and (b.MUD_ID_MR is null or b.MUD_ID_MR='') THEN '空岗' ELSE c.Name END AS MRName  "
                        + " FROM   " + _NondbName + "  .dbo.Territory_Hospital b  "
                        + " LEFT JOIN dbo.WP_QYUSER c ON b.MUD_ID_MR = c.UserId WHERE b.TERRITORY_RM = '" + territoryCode + "' ) e ON a.MRTerritoryCode = e.TERRITORY_MR  "
                        + " GROUP BY d.TERRITORY_DM,d.DMName,e.TERRITORY_MR,e.MRName,a.BudgetTotal,a.[State]) m "
                        + " GROUP BY  m.DMTerritoryCode,m.DMName,m.MRTerritoryCode,m.MRName,m.PreState ";
                }

                if (position.ToUpper() == "RD")
                {
                    sqlString = " SELECT m.DMTerritoryCode,m.DMName, sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + " sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13,2),SUM(m.TotalBudget)) as TotalPrice,m.PreState from "
                        + "  (SELECT d.TERRITORY_RM AS DMTerritoryCode,d.DMName, "
                        + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + "  case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount,COUNT(a.ID) as PreCount, "
                        + " SUM(a.BudgetTotal) as TotalBudget,case when a.[State] in ('5','6','9') then N'审批通过' "
                        + "when a.[State] in ('2','4','8') then N'审批驳回' when a.[State] in ('0','1','3','7') then N'待审批'  "
                        + "when a.[State] = 10 then N'已取消' end as PreState FROM (SELECT * FROM  dbo.P_PreApproval_COST  " + conditionDate + " ) a  "
                        + " INNER JOIN ( SELECT DISTINCT b.TERRITORY_RM,b.MUD_ID_RM,CASE WHEN  b.TERRITORY_RM is not null and b.TERRITORY_RM<>'' "
                        + "  and (b.MUD_ID_RM is null or b.MUD_ID_RM='') THEN '空岗' ELSE c.Name END AS DMName "
                        + " FROM   " + _NondbName + "  .dbo.Territory_Hospital b  "
                        + "   LEFT JOIN dbo.WP_QYUSER c ON b.MUD_ID_RM = c.UserId WHERE b.TERRITORY_RD = '" + territoryCode + "') d ON a.CostCenter = d.TERRITORY_RM   "
                        + "GROUP BY d.TERRITORY_RM,d.DMName,a.BudgetTotal,a.[State]) m "
                        + " GROUP BY  m.DMTerritoryCode,m.DMName,m.PreState ";
                }

                if (position.ToUpper() == "TA")
                {
                    sqlString = " SELECT m.DMTerritoryCode,m.DMName, sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + "  sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13,2),SUM(m.TotalBudget)) as TotalPrice,m.PreState from  "
                        + "  (SELECT d.TERRITORY_RD AS DMTerritoryCode,d.DMName, "
                        + " case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + " case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount,COUNT(a.ID) as PreCount, "
                        + " SUM(a.BudgetTotal) as TotalBudget,case when a.[State] in ('5','6','9') then N'审批通过' "
                        + "when a.[State] in ('2','4','8') then N'审批驳回' when a.[State] in ('0','1','3','7') then N'待审批'  "
                        + "when a.[State] = 10 then N'已取消' end as PreState FROM (SELECT * FROM  dbo.P_PreApproval_COST  " + conditionDate + " ) a  "
                        + "  INNER JOIN ( SELECT DISTINCT b.TERRITORY_RD,b.MUD_ID_RD,CASE WHEN  b.TERRITORY_RD is not null and b.TERRITORY_RD<>'' "
                        + "    and (b.MUD_ID_RD is null or b.MUD_ID_RD='') THEN '空岗' ELSE c.Name END AS DMName  "
                        + " FROM   " + _NondbName + "  .dbo.Territory_Hospital b  "
                        + "LEFT JOIN dbo.WP_QYUSER c ON b.MUD_ID_RD = c.UserId WHERE b.TERRITORY_TA='" + territoryCode + "') d ON a.RDTerritoryCode = d.TERRITORY_RD "
                        + "GROUP BY d.TERRITORY_RD,d.DMName,a.BudgetTotal,a.[State]) m "
                        + "GROUP BY  m.DMTerritoryCode,m.DMName,m.PreState ";
                }

                if (position.ToUpper() == "BU")
                {
                    sqlString = " SELECT m.DMTerritoryCode,m.DMName,m.MRTerritoryCode,m.MRName, sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + "sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13,2),SUM(m.TotalBudget)) as TotalPrice,m.PreState from  "
                        + " (SELECT b.TerritoryTA AS DMTerritoryCode,b.TerritoryHeadName AS DMName,e.TERRITORY_RD  AS MRTerritoryCode,e.RDName AS MRName, "
                        + "  case when a.BudgetTotal=0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + " case when a.BudgetTotal>0 then  COUNT(a.ID) end as NonZeroPreCount,COUNT(a.ID) as PreCount, "
                        + " SUM(a.BudgetTotal) as TotalBudget,case when a.[State] in ('5','6','9') then N'审批通过' "
                        + "when a.[State] in ('2','4','8') then N'审批驳回' when a.[State] in ('0','1','3','7') then N'待审批'  "
                        + "when a.[State] = 10 then N'已取消' end as PreState FROM (SELECT * FROM  dbo.P_PreApproval_COST  " + conditionDate + " ) a  "
                        + "INNER JOIN ( SELECT TerritoryTA,TerritoryHeadName FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = 'RES' )) b ON a.TA = b.TerritoryTA "
                        + " INNER JOIN ( SELECT DISTINCT c.TERRITORY_RD,c.MUD_ID_RD,CASE WHEN c.TERRITORY_RD is not null and c.TERRITORY_RD<>'' "
                        + "  and (c.MUD_ID_RD is null or c.MUD_ID_RD='') THEN '空岗' ELSE d.Name END AS RDName FROM (  "
                        + " SELECT DISTINCT TERRITORY_RD,MUD_ID_RD FROM " + _NondbName + " .dbo.Territory_Hospital WHERE TERRITORY_TA IN "
                        + "( SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) ) c "
                        + "  LEFT JOIN dbo.WP_QYUSER d ON c.MUD_ID_RD = d.UserId ) e ON a.RDTerritoryCode = e.TERRITORY_RD "
                        + "  GROUP BY b.TerritoryTA,b.TerritoryHeadName,e.TERRITORY_RD,e.RDName,a.BudgetTotal,a.[State]) m "
                        + "  GROUP BY m.DMTerritoryCode,m.DMName,m.MRTerritoryCode,m.MRName,m.PreState ";
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval_CountAmount_View>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
        #endregion

        #region 费用分析前台-预申请上层分析
        public List<P_PreApproval_OwnBelongCountAmount> LoadPreApprovalUpData(string userId, string position, string territoryCode, string begin, string end)
        {
            string sqlString = string.Empty;
            try
            {
                List<P_PreApproval_OwnBelongCountAmount> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionDate = " WHERE MeetingDate >=  '" + begin + "' AND MeetingDate <  '" + end + "' ";
                if (position.ToUpper() == "RM")
                {
                    sqlString = " SELECT m.OwnTerritory,m.BelongTerritory, sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + "  sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13, 2), SUM(m.TotalBudget)) as TotalPrice from "
                        + "    (SELECT case when a.BudgetTotal = 0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + "   case when a.BudgetTotal > 0 then  COUNT(a.ID) end as NonZeroPreCount, COUNT(a.ID) as PreCount, "
                        + "   SUM(a.BudgetTotal) as TotalBudget, a.CostCenter AS OwnTerritory, a.RDTerritoryCode AS BelongTerritory "
                        + "    FROM (SELECT * FROM  dbo.P_PreApproval_COST  " + conditionDate + " ) a "
                        + "  WHERE a.RDTerritoryCode = (SELECT DISTINCT TERRITORY_RD FROM " + _NondbName + ".dbo.Territory_Hospital WHERE TERRITORY_RM = 'RES_RM_N1') "
                        + " and a.[State] in(5,6,9)  GROUP BY a.CostCenter,a.RDTerritoryCode,a.BudgetTotal ) m "
                        + " GROUP BY m.OwnTerritory, m.BelongTerritory ";
                }

                if (position.ToUpper() == "RD")
                {
                    sqlString = " SELECT m.OwnTerritory,m.BelongTerritory,  sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + " sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13, 2), SUM(m.TotalBudget)) as TotalPrice from "
                        + "   (SELECT case when a.BudgetTotal = 0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + "   case when a.BudgetTotal > 0 then  COUNT(a.ID) end as NonZeroPreCount, COUNT(a.ID) as PreCount, "
                        + "  SUM(a.BudgetTotal) as TotalBudget, a.RDTerritoryCode AS OwnTerritory, a.TA AS BelongTerritory "
                        + "   FROM (SELECT * FROM  dbo.P_PreApproval_COST  " + conditionDate + " ) a "
                        + "  WHERE a.TA IN(SELECT DISTINCT TERRITORY_TA FROM " + _NondbName + ".dbo.Territory_Hospital "
                        + "  WHERE TERRITORY_RD = 'RES_SD_02') and a.[State] in(5, 6, 9) "
                        + "GROUP BY a.RDTerritoryCode,a.TA,a.BudgetTotal) m "
                        + "GROUP BY m.OwnTerritory,m.BelongTerritory ";
                }

                if (position.ToUpper() == "TA")
                {
                    sqlString = "  SELECT m.OwnTerritory,m.BelongTerritory, sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + " sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13, 2), SUM(m.TotalBudget)) as TotalPrice from "
                        + "   (SELECT case when a.BudgetTotal = 0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + "   case when a.BudgetTotal > 0 then  COUNT(a.ID) end as NonZeroPreCount, COUNT(a.ID) as PreCount, "
                        + "  SUM(a.BudgetTotal) as TotalBudget, a.TA  AS OwnTerritory, b.BUName AS BelongTerritory "
                        + "    FROM (SELECT * FROM  dbo.P_PreApproval_COST  " + conditionDate + " ) a "
                        + "    LEFT JOIN(SELECT TA.TerritoryTA, BU.BUName  FROM P_TAINFO TA INNER JOIN P_BUINFO BU ON TA.BUID = BU.ID) b  ON a.TA = b.TerritoryTA "
                        + "   WHERE a.TA IN(SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = (SELECT BUID FROM dbo.P_TAINFO WHERE TerritoryTA = 'RES_TAL_02')) "
                        + " and a.[State] in(5,6,9) "
                        + " GROUP BY a.TA,b.BUName,a.BudgetTotal ) m "
                        + " GROUP BY m.OwnTerritory, m.BelongTerritory ";
                }

                if (position.ToUpper() == "BU")
                {
                    sqlString = "SELECT m.OwnTerritory,m.BelongTerritory, sum(m.ZeroPreCount) as newZeroCount,sum(m.NonZeroPreCount) as newNonZeroCount, "
                        + " sum(m.PreCount) as TotalCount, CONVERT(DECIMAL(13, 2), SUM(m.TotalBudget)) as TotalPrice from "
                        + "   (SELECT case when a.BudgetTotal = 0 then  COUNT(a.ID) end as ZeroPreCount, "
                        + "   case when a.BudgetTotal > 0 then  COUNT(a.ID) end as NonZeroPreCount, COUNT(a.ID) as PreCount, "
                        + "   SUM(a.BudgetTotal) as TotalBudget, a.TA  AS OwnTerritory, b.BUName AS BelongTerritory "
                        + "     FROM (SELECT * FROM  dbo.P_PreApproval_COST  " + conditionDate + " ) a "
                        + "    LEFT JOIN(SELECT TA.TerritoryTA, BU.BUName  FROM P_TAINFO TA INNER JOIN P_BUINFO BU ON TA.BUID = BU.ID) b  ON a.TA = b.TerritoryTA "
                        + "    WHERE a.TA IN(SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = (SELECT ID FROM dbo.P_BUINFO WHERE BUName = 'RES')) "
                        + "  and a.[State] in(5,6,9) "
                        + "GROUP BY a.TA,b.BUName,a.BudgetTotal) m "
                        + " GROUP BY m.OwnTerritory, m.BelongTerritory ";
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreApproval_OwnBelongCountAmount>(sqlString, new SqlParameter[] { });
                }
                return rtnData;
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return null;
            }
        }
        #endregion

        #region 同步预申请表
        public int SyncPreApproval()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnCount = sqlServerTemplate.ExecuteNonQuery(@"DELETE FROM P_PreApproval_COST WHERE ID IN (SELECT ID FROM [P_PreApproval] WHERE ActionState = 0);",
                    new SqlParameter[]
                    {

                    });

                var rtnData = sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO dbo.P_PreApproval_COST SELECT [ID],[ApplierName],[ApplierMUDID],[ApplierMobile],[CreateDate],[ModifyDate],[HTCode],[Market],[TA],[Province],[City],[HospitalCode],[HospitalName],[HospitalAddress]
                                                                ,[MeetingName],[MeetingDate],[AttendCount],[CostCenter],[BudgetTotal],[IsDMFollow],[BUHeadName],[BUHeadMUDID],[BUHeadApproveDate],[IsReAssign]
                                                                ,[ReAssignBUHeadName],[ReAssignBUHeadMUDID],[ReAssignBUHeadApproveDate],[State],[MMCoEImage],[MMCoEApproveState],[IsBudgetChange],[IsMMCoEChange]
                                                                ,[IsUsed],[IsFinished],[IsFreeSpeaker],[SpeakerServiceImage],[SpeakerBenefitImage],[ReAssignOperatorMUDID],[ReAssignOperatorName],[HospitalAddressCode]
                                                                ,[RDSDName],[RDSDMUDID] ,[VeevaMeetingID],[CurrentApproverName],[CurrentApproverMUDID],[RDTerritoryCode],[DMTerritoryCode],[MRTerritoryCode],[HTType],'1' 
                                                                FROM [P_PreApproval] WHERE ActionState = '0';",
                    new SqlParameter[]
                    {

                    });

                if (rtnData > 0)
                {
                    sqlServerTemplate.ExecuteNonQuery(@"UPDATE P_PreApproval SET ActionState = 1 WHERE ActionState = 0;",
                    new SqlParameter[]
                    {

                    });
                }
                return rtnData;
            }
        }
        #endregion

        #region 获取汇总数据
        public V_COST_SUMMARY GetPreApprovalList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            string Territory = string.Join("','", TerritoryStr);
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            V_COST_SUMMARY rtnData = new V_COST_SUMMARY();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtn1 = sqlServerTemplate.Find<P_COUNT>(@"SELECT COUNT(DISTINCT HTCODE) AS Count FROM [P_PreApproval] WHERE CostCenter IN ('" + Territory + "') AND MeetingDate >= @StartDate AND MeetingDate < @EndDate AND State in ('5','6','9');",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.PreApprovalCount = rtn1.Count.ToString("N0");

                var rtn2 = sqlServerTemplate.Find<P_SUM>(@"SELECT SUM(BudgetTotal) AS Count FROM [P_PreApproval] WHERE CostCenter IN ('" + Territory + "') AND MeetingDate >= @StartDate AND MeetingDate < @EndDate AND State in ('5','6','9');",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.BudgetTotal = rtn2.Count.ToString("N2");
            }

            return rtnData;
        }
        #endregion

    }
}
