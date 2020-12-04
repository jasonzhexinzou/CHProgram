using MealAdmin.Entity.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XDataBase;
using XFramework.XInject.Attributes;

namespace MealAdmin.Dao
{
    public class ReportDao : IReportDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }
        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        public List<v_caterreport> LoadCater(
            string CN, string MUID, DateTime? startDate, DateTime? endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier,
            int page, int rows, out int total)
        {
            var _sql = "";
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(CN))
            {
                _sql += " AND b.[预申请表CN号] LIKE @CN ";
                listParams.Add(new SqlParameter("@CN", $"%{CN}%"));
            }
            if (!string.IsNullOrEmpty(MUID))
            {
                _sql += " AND b.[MUDID] LIKE @MUID ";
                listParams.Add(new SqlParameter("@MUID", $"%{MUID}%"));
            }
            if (startDate != null)
            {
                _sql += " AND b.[送餐日期] > @startDate ";
                listParams.Add(new SqlParameter("@startDate", startDate.Value));
            }
            if (endDate != null)
            {
                _sql += " AND b.[送餐日期] < @endDate ";
                listParams.Add(new SqlParameter("@endDate", endDate.Value));
            }
            if (!string.IsNullOrEmpty(isOrderSuccess))
            {
                _sql += " AND b.[是否预定成功] = @isOrderSuccess ";
                listParams.Add(new SqlParameter("@isOrderSuccess", isOrderSuccess));
            }
            if (!string.IsNullOrEmpty(isReceived))
            {
                _sql += " AND b.[是否收餐/未送达] = @isReceived ";
                listParams.Add(new SqlParameter("@isReceived", isReceived));
            }
            if (!string.IsNullOrEmpty(isReturn))
            {
                _sql += " AND b.[是否申请退单] = @isReturn ";
                listParams.Add(new SqlParameter("@isReturn", isReturn));
            }
            if (!string.IsNullOrEmpty(isReturnSuccess))
            {
                _sql += " AND b.[是否退单成功] = @isReturnSuccess ";
                listParams.Add(new SqlParameter("@isReturnSuccess", isReturnSuccess));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                _sql += " AND b.[供应商] = @Supplier ";
                listParams.Add(new SqlParameter("@Supplier", Supplier));
            }

            var sql = @"
SELECT 
  
	b.[订餐人姓名] AS c0,
	b.[订餐人手机号] AS c1,
	b.[MUDID] AS c2,
	b.[Market] AS c3,
	b.[预申请表CN号] AS c4,
    b.[预算金额] AS c57,
    b.[供应商] AS c5,
	b.[订单号] AS c6,
	b.[下单日期] AS c7,
	b.[下单时间] AS c8,
	b.[送餐日期] AS c9,
	b.[送餐时间] AS c10,
	b.[餐厅编码] AS c55,
	b.[预订餐厅] AS c11,
	b.[参会人数] AS c12,
	b.[总份数] AS c13,
	b.[预订金额] AS c14,
	b.[实际金额] AS c15,
	b.[金额调整原因] AS c16,
	b.[1=同一医院当日多场] AS c17,
	b.[2=同一代表当日多场] AS c18,
    b.[3=同一餐厅当日多场] AS c58,
	b.[4=同一代表同一医院当日多场] AS c19,
    b.[5=同一代表同一餐厅当日多场] AS c59,
    b.[6=同一代表同一医院同一餐厅当日多场] AS c60,
	b.[7=参会人数>=60] AS c20,
	b.[8=参会人数<60,订单份数>=60] AS c21,
	b.[9=代表自提] AS c22,
	b.[省份] AS c23,
	b.[城市] AS c24,
	b.[医院编码] AS c25,
	b.[医院名称] AS c26,
	b.[医院地址] AS c27,
	b.[送餐地址] AS c28,
	b.[收餐人姓名] AS c29,
	b.[收餐人电话] AS c30,
    b.[下单备注] AS c56,
	b.[是否预定成功] AS c31,
    b.[预定成功日期] AS c32,
	b.[预定成功时间] AS c33,
	b.[是否收餐/未送达] AS c34,
    b.[未送达描述] AS c35,
	b.[准点率] AS c36,
	b.[准点率描述] AS c37,
	b.[食品安全存在问题] AS c38,
	b.[食品安全问题描述] AS c39,
	b.[餐品卫生及新鲜] AS c40,
	b.[餐品卫生描述] AS c41,
	b.[餐品包装] AS c42,
	b.[餐品包装描述] AS c43,
	b.[餐品性价比] AS c44,
	b.[餐品性价比描述] AS c45,
	b.[其他评价] AS c46,
	b.[在线评分] AS c47,
	b.[评论日期] AS c48,
	b.[评论时间] AS c49,
	b.[是否申请退单] AS c50,
	b.[是否退单成功] AS c51,
	b.[退单失败平台发起配送需求] AS c52,
	b.[退单失败线下反馈配送需求] AS c53,
	b.[预订/退单失败原因] AS c54
  FROM v_caterreport AS b WHERE 1=1 
  
";

            List<v_caterreport> rtnData = null;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<v_caterreport>(rows, page, out total,
                    sql + _sql, "ORDER BY b.[下单日期] DESC, b.[下单时间] DESC ", listParams.ToArray());


            }
            return rtnData;
        }

        public List<v_caterreport> LoadOldCater(
            string CN, string MUID, DateTime? startDate, DateTime? endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier,
            int page, int rows, out int total)
        {
            var _sql = "";
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(CN))
            {
                _sql += " AND b.[CN] LIKE @CN ";
                listParams.Add(new SqlParameter("@CN", $"%{CN}%"));
            }
            if (!string.IsNullOrEmpty(MUID))
            {
                _sql += " AND b.[MUDID] LIKE @MUID ";
                listParams.Add(new SqlParameter("@MUID", $"%{MUID}%"));
            }
            if (startDate != null)
            {
                _sql += " AND b.[DelieverDate] > @startDate ";
                listParams.Add(new SqlParameter("@startDate", startDate.Value));
            }
            if (endDate != null)
            {
                _sql += " AND b.[DelieverDate] < @endDate ";
                listParams.Add(new SqlParameter("@endDate", endDate.Value));
            }
            if (!string.IsNullOrEmpty(isOrderSuccess))
            {
                _sql += " AND b.[IsSuccess] = @isOrderSuccess ";
                listParams.Add(new SqlParameter("@isOrderSuccess", isOrderSuccess));
            }
            if (!string.IsNullOrEmpty(isReceived))
            {
                _sql += " AND b.[IsReceive] = @isReceived ";
                listParams.Add(new SqlParameter("@isReceived", isReceived));
            }
            if (!string.IsNullOrEmpty(isReturn))
            {
                _sql += " AND b.[IsReturn] = @isReturn ";
                listParams.Add(new SqlParameter("@isReturn", isReturn));
            }
            if (!string.IsNullOrEmpty(isReturnSuccess))
            {
                _sql += " AND b.[ReturnSuccess] = @isReturnSuccess ";
                listParams.Add(new SqlParameter("@isReturnSuccess", isReturnSuccess));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                _sql += " AND b.[Channel] = @Supplier ";
                listParams.Add(new SqlParameter("@Supplier", Supplier));
            }

            var sql = @"
SELECT 
  
	b.[UserName] AS c0,
	b.[UserMobile] AS c1,
	b.[MUDID] AS c2,
	b.[Market] AS c3,
	b.[CN] AS c4,
    b.[Budget] AS c57,
    b.[Channel] AS c5,
	b.[XmsOrderId] AS c6,
	b.[CreateDate] AS c7,
	b.[CreateTime] AS c8,
	b.[DelieverDate] AS c9,
	b.[DelieverTime] AS c10,
	b.[RestaurantId] AS c55,
	b.[RestaurantName] AS c11,
	b.[Attend] AS c12,
	b.[Count] AS c13,
	b.[BudgetTotal] AS c14,
	b.[ActualTotal] AS c15,
	b.[ChangeReason] AS c16,
	b.[TYYYDRDC] AS c17,
	b.[TYDBDRDC] AS c18,
    b.[TYCTDRDC] AS c58,
	b.[TYDBTYYYDRDC] AS c19,
    b.[TYDBTYCTDRDC] AS c59,
    b.[TYDBTYYYTYCTDRDC] AS c60,
	b.[OverPersonCount] AS c20,
	b.[OverFoodCount] AS c21,
	b.[Personal] AS c22,
	b.[Province] AS c23,
	b.[City] AS c24,
	b.[HospitalCode] AS c25,
	b.[HospitalName] AS c26,
	b.[HospitalAddress] AS c27,
	b.[SendAddress] AS c28,
	b.[Receiver] AS c29,
	b.[ReceiverMobile] AS c30,
    b.[Memo] AS c56,
	b.[IsSuccess] AS c31,
    b.[SuccessDate] AS c32,
	b.[SuccessTime] AS c33,
	b.[IsReceive] AS c34,
    b.[UnreceiveMemo] AS c35,
	b.[IsOnTime] AS c36,
	b.[OnTimeMemo] AS c37,
	b.[IsSafe] AS c38,
	b.[SafeMemo] AS c39,
	b.[IsFresh] AS c40,
	b.[FreshMemo] AS c41,
	b.[IsPackage] AS c42,
	b.[PackageMemo] AS c43,
	b.[IsCost] AS c44,
	b.[CostMemo] AS c45,
	b.[OtherScore] AS c46,
	b.[OnlineScore] AS c47,
	b.[CommentDate] AS c48,
	b.[CommentTime] AS c49,
	b.[IsReturn] AS c50,
	b.[ReturnSuccess] AS c51,
	b.[ReturnNeedSend] AS c52,
	b.[ReturnSendSuccess] AS c53,
	b.[ReturnFaildReason] AS c54
  FROM P_HT_OrderReport AS b WHERE 1=1 
  
";

            List<v_caterreport> rtnData = null;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<v_caterreport>(rows, page, out total,
                    sql + _sql, "ORDER BY b.[DelieverDate] DESC, b.[DelieverTime] DESC ", listParams.ToArray());


            }
            return rtnData;
        }

        public List<v_NonHT_caterreport> LoadCaterForNonHT(
            string CN, string MUID, string HospitalCode, string RestaurantId, DateTime? startDate, DateTime? endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier,
            int page, int rows, out int total)
        {
            var _sql = "";
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(CN))
            {
                _sql += " AND b.[PO No.]=@CN ";
                listParams.Add(new SqlParameter("@CN", CN));
            }
            if (!string.IsNullOrEmpty(MUID))
            {
                _sql += " AND b.[订餐人MUDID]=@MUID ";
                listParams.Add(new SqlParameter("@MUID", MUID));
            }
            if (!string.IsNullOrEmpty(HospitalCode))
            {
                if (HospitalCode.Split(',').Length > 1)
                {
                    _sql += " AND (b.[医院编码]=@HospitalCode or b.[医院编码]=@OldHospitalCode)";
                    listParams.Add(new SqlParameter("@HospitalCode", HospitalCode.Split(',')[0]));
                    listParams.Add(new SqlParameter("@OldHospitalCode", HospitalCode.Split(',')[1]));
                }
                else
                {
                    _sql += " AND b.[医院编码]=@HospitalCode ";
                    listParams.Add(new SqlParameter("@HospitalCode", HospitalCode));
                }
            }
            if (!string.IsNullOrEmpty(RestaurantId))
            {
                _sql += " AND b.[餐厅编码]=@RestaurantId ";
                listParams.Add(new SqlParameter("@RestaurantId", RestaurantId));
            }
            if (startDate != null)
            {
                _sql += " AND b.[送餐日期] > @startDate ";
                listParams.Add(new SqlParameter("@startDate", startDate.Value));
            }
            if (endDate != null)
            {
                _sql += " AND b.[送餐日期] < @endDate ";
                listParams.Add(new SqlParameter("@endDate", endDate.Value));
            }
            if (!string.IsNullOrEmpty(isOrderSuccess))
            {
                _sql += " AND b.[是否预定成功] = @isOrderSuccess ";
                listParams.Add(new SqlParameter("@isOrderSuccess", isOrderSuccess));
            }
            if (!string.IsNullOrEmpty(isReceived))
            {
                _sql += " AND b.[是否收餐/未送达] = @isReceived ";
                listParams.Add(new SqlParameter("@isReceived", isReceived));
            }
            if (!string.IsNullOrEmpty(isReturn))
            {
                _sql += " AND b.[是否申请退单] = @isReturn ";
                listParams.Add(new SqlParameter("@isReturn", isReturn));
            }
            if (!string.IsNullOrEmpty(isReturnSuccess))
            {
                _sql += " AND b.[是否退单成功] = @isReturnSuccess ";
                listParams.Add(new SqlParameter("@isReturnSuccess", isReturnSuccess));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                _sql += " AND b.[供应商] = @Supplier ";
                listParams.Add(new SqlParameter("@Supplier", Supplier));
            }

            var sql = @"

  SELECT 
  
	b.[订餐人姓名] AS c0,
	b.[订餐人手机号] AS c1,
	b.[订餐人MUDID] AS c2,
	b.[用餐人Market] AS c3,
	b.[用餐人TA] AS c4,
	b.[Meeting Code] AS c5,
	b.[会议名称] AS c6,
	b.[PO No.] AS c7,
	b.[WBS] AS c8,
    b.[供应商] AS c9,
	b.[订单号] AS c10,
	b.[下单日期] AS c11,
	b.[下单时间] AS c12,
	b.[送餐日期] AS c13,
	b.[送餐时间] AS c14,
	b.[餐厅编码] AS c55,
	b.[预订餐厅] AS c15,
	b.[参会人数] AS c16,
	b.[总份数] AS c17,
	b.[预订金额] AS c18,
	b.[实际金额] AS c19,
	b.[金额调整原因] AS c20,
	b.[省份] AS c21,
	b.[城市] AS c22,
	b.[医院编码] AS c23,
	b.[医院名称] AS c24,
	b.[医院地址] AS c25,
	b.[送餐地址] AS c26,
	b.[收餐人姓名] AS c27,
	b.[收餐人电话] AS c28,
    b.[下单备注] AS c56,
	b.[是否预定成功] AS c29,
    b.[预定成功日期] AS c30,
	b.[预定成功时间] AS c31,
	b.[是否收餐/未送达] AS c32,
    b.[未送达描述] AS c33,
	b.[准点率] AS c34,
	b.[准点率描述] AS c35,
	b.[食品安全存在问题] AS c36,
	b.[食品安全问题描述] AS c37,
	b.[餐品卫生及新鲜] AS c38,
	b.[餐品卫生描述] AS c39,
	b.[餐品包装] AS c40,
	b.[餐品包装描述] AS c41,
	b.[餐品性价比] AS c42,
	b.[餐品性价比描述] AS c43,
	b.[其他评价] AS c44,
	b.[在线评分] AS c45,
	b.[评论日期] AS c46,
	b.[评论时间] AS c47,
	b.[是否申请退单] AS c48,
	b.[是否退单成功] AS c49,
	b.[退单失败平台发起配送需求] AS c50,
	b.[退单失败线下反馈配送需求] AS c51,
	b.[预订/退单失败原因] AS c52
  
FROM v_NonHT_caterreport b WHERE 1=1
  
";
            List<v_NonHT_caterreport> rtnData = null;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<v_NonHT_caterreport>(rows, page, out total,
                    sql + _sql, "ORDER BY b.[下单日期] DESC, b.[下单时间] DESC ", listParams.ToArray());
            }
            return rtnData;
        }


        #region 报表查询
        /// <summary>
        /// 报表查询
        /// </summary>
        /// <param name="srh_CN"></param>
        /// <param name="srh_MUDID"></param>
        /// <param name="srh_CreateTimeBegin"></param>
        /// <param name="srh_CreateTimeEnd"></param>
        /// <param name="srh_DeliverTimeBegin"></param>
        /// <param name="srh_DeliverTimeEnd"></param>
        /// <param name="srh_State"></param>
        /// <param name="Supplier"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<HT_Order_Report> LoadOrderReport(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int page, int rows, out int total)
        {
            #region sql
            var sql = @"
                SELECT 
                	b.[申请人姓名] AS c0 ,
                    b.[申请人MUDID] AS c1 ,
                    b.[申请人手机号码] AS c2 ,
                    b.[预申请申请日期] AS c3 ,
                    b.[预申请申请时间] AS c4 ,
                    b.[预申请修改日期] AS c5 ,
                    b.[预申请修改时间] AS c6 ,
                    b.[HT编号] AS c7 ,
                    b.[Market] AS c8 ,
                    b.[TA] AS c9 ,
                    b.[省份] AS c10 ,
                    b.[城市] AS c11 ,
                    b.[医院编码] AS c12 ,
                    b.[医院名称] AS c13 ,
                    b.[医院地址] AS c14 ,
                    b.[会议名称] AS c15 ,
                    b.[会议日期] AS c16 ,
                    b.[会议时间] AS c17 ,
                    b.[参会人数] AS c18 ,
                    b.[成本中心] AS c19 ,
                    b.[预算金额] AS c20 ,
                    b.[直线经理是否随访] AS c21 ,
                    b.[是否由外部免费讲者来讲] AS c22 ,
                    b.[预申请审批人姓名] AS c23 ,
                    b.[预申请审批人MUDID] AS c24 ,
                    b.[预申请审批日期] AS c25 ,
                    b.[预申请审批时间] AS c26 ,
                    b.[预申请审批状态] AS c27 ,
                    b.[预申请是否重新分配审批人] AS c28 ,
                    b.[预申请重新分配审批人-操作人] AS c29 ,
                    b.[预申请重新分配审批人-操作人MUDID] AS c30 ,
                    b.[预申请被重新分配审批人姓名] AS c31 ,
                    b.[预申请被重新分配审批人MUDID] AS c32 ,
                    b.[预申请重新分配审批人日期] AS c33 ,
                    b.[预申请重新分配审批人时间] AS c34 ,
                    b.[供应商] AS c35 ,
                    b.[订单号] AS c36 ,
                    b.[下单日期] AS c37 ,
                    b.[下单时间] AS c38 ,
                    b.[送餐日期] AS c39 ,
                    b.[送餐时间] AS c40 ,
                    b.[餐厅编码] AS c41 ,
                    b.[预订餐厅] AS c42 ,
                    b.[用餐人数] AS c43 ,
                    b.[总份数] AS c44 ,
                    b.[预订金额] AS c45 ,
                    b.[实际金额] AS c46 ,
                    b.[金额调整原因] AS c47 ,
                    b.[送餐地址] AS c48 ,
                    b.[收餐人姓名] AS c49 ,
                    b.[收餐人电话] AS c50 ,
                    b.[下单备注] AS c51 ,
                    b.[是否预定成功] AS c52 ,
                    b.[预定成功日期] AS c53 ,
                    b.[预定成功时间] AS c54 ,
                    b.[是否申请退单] AS c55 ,
                    b.[是否退单成功] AS c56 ,
                    b.[退单失败平台发起配送需求] AS c57 ,
                    b.[退单失败线下反馈配送需求] AS c58 ,
                    b.[预订/退单失败原因] AS c59 ,
                    b.[是否收餐/未送达] AS c60 ,
                    b.[确认收餐日期] AS c61 ,
                    b.[确认收餐时间] AS c62 ,
                    b.[用户确认金额] AS c63 ,
                    b.[用户确认金额调整原因] AS c64 ,
                    b.[用户确认金额调整备注] AS c65 ,
                    b.[实际用餐人数] AS c66 ,
                    b.[实际用餐人数调整原因] AS c67 ,
                    b.[实际用餐人数调整备注] AS c68 ,
                    b.[未送达描述] AS c69 ,
                    b.[准点率] AS c70 ,
                    b.[准点率描述] AS c71 ,
                    b.[食品安全存在问题] AS c72 ,
                    b.[食品安全问题描述] AS c73 ,
                    b.[餐品卫生及新鲜] AS c74 ,
                    b.[餐品卫生描述] AS c75 ,
                    b.[餐品包装] AS c76 ,
                    b.[餐品包装描述] AS c77 ,
                    b.[餐品性价比] AS c78 ,
                    b.[餐品性价比描述] AS c79 ,
                    b.[其他评价] AS c80 ,
                    b.[在线评分] AS c81 ,
                    b.[评论日期] AS c82 ,
                    b.[评论时间] AS c83 ,
                    b.[1=同一医院当日多场] AS c84 ,
                    b.[2=同一代表当日多场] AS c85 ,
                    b.[3=同一餐厅当日多场] AS c86 ,
                    b.[4=同一代表同一医院当日多场] AS c87 ,
                    b.[5=同一代表同一餐厅当日多场] AS c88 ,
                    b.[6=同一代表同一医院同一餐厅当日多场] AS c89 ,
                    b.[7=参会人数>=60] AS c90 ,
                    b.[8=参会人数<60,订单份数>=60] AS c91 ,
                    b.[9=代表自提] AS c92 ,
                    b.[直线经理姓名] AS c93 ,
                    b.[直线经理MUDID] AS c94 ,
                    b.[Level2 Manager 姓名] AS c95 ,
                    b.[Level2 Manager MUDID] AS c96 ,
                    b.[Level3 Manager 姓名] AS c97 ,
                    b.[Level3 Manager MUDID] AS c98 ,
                    b.[是否上传文件] AS c99 ,
                    b.[上传文件提交日期] AS c100 ,
                    b.[上传文件提交时间] AS c101 ,
                    b.[上传文件审批直线经理姓名] AS c102 ,
                    b.[上传文件审批直线经理MUDID] AS c103 ,
                    b.[上传文件审批日期] AS c104 ,
                    b.[上传文件审批时间] AS c105 ,
                    b.[上传文件审批状态] AS c106 ,
                    b.[签到人数是否等于实际用餐人数] AS c107 ,
                    b.[签到人数] AS c108 ,
                    b.[签到人数调整原因] AS c109 ,
                    b.[上传文件是否Re-Open] AS c110 ,
                    b.[上传文件Re-Open操作人] AS c111 ,
                    b.[上传文件Re-Open操作人MUDID] AS c112 ,
                    b.[上传文件Re-Open日期] AS c113 ,
                    b.[上传文件Re-Open时间] AS c114 ,
                    b.[上传文件Re-Open原因] AS c115 ,
                    b.[上传文件Re-Open审批状态] AS c116 ,
                    b.[上传文件是否重新分配] AS c117 ,
                    b.[上传文件重新分配操作人] AS c118 ,
                    b.[上传文件重新分配操作人MUDID] AS c119 ,
                    b.[上传文件被重新分配人姓名] AS c120 ,
                    b.[上传文件被重新分配人MUDID] AS c121 ,
                    b.[上传文件被重新分配日期] AS c122 ,
                    b.[上传文件被重新分配时间] AS c123 ,
                    b.[上传文件是否重新分配审批人] AS c124 ,
                    b.[上传文件重新分配审批人-操作人] AS c125 ,
                    b.[上传文件重新分配审批人-操作人MUDID] AS c126 ,
                    b.[上传文件被重新分配审批人姓名] AS c127 ,
                    b.[上传文件被重新分配审批人MUDID] AS c128 ,
                    b.[上传文件重新分配审批人日期] AS c129 ,
                    b.[上传文件重新分配审批人时间] AS c130 ,
                    b.[项目组特殊备注] AS c131 
                FROM HT_Order_Report b WHERE 1=1
                ";
            #endregion
            var _sql = " AND b.[申请人MUDID] = 'TJ_016' ";
            List<HT_Order_Report> rtnData = null;
            var listParams = new List<SqlParameter>();
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<HT_Order_Report>(rows, page, out total,
                    sql + _sql, "ORDER BY b.[下单日期] DESC, b.[下单时间] DESC ", listParams.ToArray());
            }
            return rtnData;
        }
        #endregion

        #region 报表查询、导出
        /// <summary>
        /// 报表查询、导出
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="channel"></param>
        /// <param name="isOrderSuccess"></param>
        /// <param name="isReceived"></param>
        /// <param name="isReturn"></param>
        /// <param name="isReturnSuccess"></param>
        /// <param name="Supplier"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<HT_Order_Report> LoadReport(
            string CN, string MUID, string TACode, string HospitalCode, string RestaurantId, string CostCenter, DateTime? startDate, DateTime? endDate,
            string channel, string isOrderSuccess, string isReceived, string isReturn, string isReturnSuccess, string Supplier, string IsSpecialOrder, string OrderState,
            int page, int rows, out int total)
        {
            var _sql = "";
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(CN))
            {
                CN = CN.Replace(",", "','");
                _sql += " AND b.[HTCode] in ('" + CN + "') ";
                listParams.Add(new SqlParameter("@CN", CN));
            }
            if (!string.IsNullOrEmpty(MUID))
            {
                _sql += " AND b.[ApplierMUDID]=@MUID ";
                listParams.Add(new SqlParameter("@MUID", MUID));
            }
            if (!string.IsNullOrEmpty(TACode))
            {
                _sql += " AND b.[MRTerritoryCode]=@MRTerritoryCode ";
                listParams.Add(new SqlParameter("@MRTerritoryCode", TACode));
            }
            if (!string.IsNullOrEmpty(HospitalCode))
            {
                if (HospitalCode.Split(',').Length > 1)
                {
                    _sql += " AND (b.[HospitalCode]=@HospitalCode or b.[HospitalCode]=@OldHospitalCode)";
                    listParams.Add(new SqlParameter("@HospitalCode", HospitalCode.Split(',')[0]));
                    listParams.Add(new SqlParameter("@OldHospitalCode", HospitalCode.Split(',')[1]));
                }
                else
                {
                    _sql += " AND b.[HospitalCode]=@HospitalCode ";
                    listParams.Add(new SqlParameter("@HospitalCode", HospitalCode));
                }
            }
            if (!string.IsNullOrEmpty(RestaurantId))
            {
                _sql += " AND b.[RestaurantId]=@RestaurantId ";
                listParams.Add(new SqlParameter("@RestaurantId", RestaurantId));
            }
            if (!string.IsNullOrEmpty(CostCenter))
            {
                if (CostCenter.Split(',').Length > 1)
                {
                    _sql += " AND (b.[CostCenter]=@CostCenter or b.[CostCenter]=@OldCostCenter)";
                    listParams.Add(new SqlParameter("@CostCenter", CostCenter.Split(',')[0]));
                    listParams.Add(new SqlParameter("@OldCostCenter", CostCenter.Split(',')[1]));
                }
                else
                {
                    _sql += " AND b.[CostCenter]=@CostCenter ";
                    listParams.Add(new SqlParameter("@CostCenter", CostCenter));
                }
            }
            if (startDate != null)
            {
                _sql += " AND b.[DelieverDate] > @startDate ";
                listParams.Add(new SqlParameter("@startDate", startDate.Value));
            }
            if (endDate != null)
            {
                _sql += " AND b.[DelieverDate] < @endDate ";
                listParams.Add(new SqlParameter("@endDate", endDate.Value));
            }
            if (!string.IsNullOrEmpty(isOrderSuccess))
            {
                _sql += " AND b.[IsOrderSuccess] = @isOrderSuccess ";
                listParams.Add(new SqlParameter("@isOrderSuccess", isOrderSuccess));
            }
            if (!string.IsNullOrEmpty(isReceived))
            {
                _sql += " AND b.[IsOrderReceive] = @isReceived ";
                listParams.Add(new SqlParameter("@isReceived", isReceived));
            }
            if (!string.IsNullOrEmpty(isReturn))
            {
                _sql += " AND b.[IsReturn] = @isReturn ";
                listParams.Add(new SqlParameter("@isReturn", isReturn));
            }
            if (!string.IsNullOrEmpty(isReturnSuccess))
            {
                _sql += " AND b.[IsReturnSuccess] = @isReturnSuccess ";
                listParams.Add(new SqlParameter("@isReturnSuccess", isReturnSuccess));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                _sql += " AND b.[channel] = @Supplier ";
                listParams.Add(new SqlParameter("@Supplier", Supplier));
            }
            if (!string.IsNullOrEmpty(IsSpecialOrder))
            {
                if (IsSpecialOrder == "9")
                {
                    _sql += " AND c.[IsSpecialOrder] in (1,2) ";
                }
                else
                {
                    _sql += " AND c.[IsSpecialOrder] = @IsSpecialOrder ";
                    listParams.Add(new SqlParameter("@IsSpecialOrder", IsSpecialOrder));
                }
            }
            if (!string.IsNullOrEmpty(OrderState))
            {
                _sql += " AND b.[OrderState] = @OrderState ";
                listParams.Add(new SqlParameter("@OrderState", OrderState));
            }

            #region sql
            var sql = @"
                SELECT 
                	b.[ApplierName] AS c0 ,
                    b.[ApplierMUDID] AS c1 ,
                    b.[Position] AS c132,
                    b.[Mobile] AS c2 ,
                    b.[PreApprovalCreateDate] AS c3 ,
                    b.[PreApprovalCreateTime] AS c4 ,
                    b.[PreApprovalModifyDate] AS c5 ,
                    b.[PreApprovalModifyTime] AS c6 ,
                    b.[HTCode] AS c7 ,
                    b.[Market] AS c8 ,
                    b.[TA] AS c9 ,
                    b.[Province] AS c10 ,
                    b.[City] AS c11 ,
                    b.[HospitalCode] AS c12 ,
                    b.[HospitalName] AS c13 ,
                    b.[HospitalAddress] AS c14 ,
                    b.[MeetingName] AS c15 ,
                    b.[MeetingDate] AS c16 ,
                    b.[MeetingTime] AS c17 ,
                    b.[AttendCount] AS c18 ,
                    b.[CostCenter] AS c19 ,
                    b.[BudgetTotal] AS c20 ,
                    b.[IsDMFollow] AS c21 ,
                    b.[IsFreeSpeaker] AS c22 ,
                    b.[PreApproverName] AS c23 ,
                    b.[PreApproverMUDID] AS c24 ,
                    b.[PreApproveDate] AS c25 ,
                    b.[PreApproveTime] AS c26 ,
                    b.[PreApproveState] AS c27 ,
                    b.[PreApprovalIsReAssign] AS c28 ,
                    b.[PreApprovalReassignOperatorName] AS c29 ,
                    b.[PreApprovalReassignOperatorMUDID] AS c30 ,
                    b.[PreApprovalReassignName] AS c31 ,
                    b.[PreApprovalReassignMUDID] AS c32 ,
                    b.[PreApprovalReassignDate] AS c33 ,
                    b.[PreApprovalReassignTime] AS c34 ,
                    b.[channel] AS c35 ,
                    b.[xmsOrderId] AS c36 ,
                    b.[OrderCreateDate] AS c37 ,
                    b.[OrderCreateTime] AS c38 ,
                    b.[DelieverDate] AS c39 ,
                    b.[DelieverTime] AS c40 ,
                    b.[RestaurantId] AS c41 ,
                    b.[RestaurantName] AS c42 ,
                    b.[PersonCount] AS c43 ,
                    b.[TotalAmount] AS c45 ,
                    b.[ActualTotalAmount] AS c46 ,
                    b.[ActualTotalAmountReason] AS c47 ,
                    b.[Address] AS c48 ,
                    b.[ReceiverName] AS c49 ,
                    b.[ReceiverMobile] AS c50 ,
                    b.[Memo] AS c51 ,
                    b.[IsOrderSuccess] AS c52 ,
                    b.[OrderSuccessDate] AS c53 ,
                    b.[OrderSuccessTime] AS c54 ,
                    b.[IsReturn] AS c55 ,
                    b.[IsReturnSuccess] AS c56 ,
                    b.[IsReturnFailNeedSend] AS c57 ,
                    b.[IsReturnFailNeedSendState] AS c58 ,
                    b.[ReturnFailReason] AS c59 ,
                    b.[IsOrderReceive] AS c60 ,
                    b.[ConfirmOrderDate] AS c61 ,
                    b.[ConfirmOrderTime] AS c62 ,
                    b.[RealTotalAmount] AS c63 ,
                    b.[RealTotalAmountReason] AS c64 ,
                    b.[RealTotalAmountMemo] AS c65 ,
                    b.[RealPersonCount] AS c66 ,
                    b.[RealPersonCountReason] AS c67 ,
                    b.[RealPersonCountMemo] AS c68 ,
                    b.[UnReceiveMemo] AS c69 ,
                    b.[OnTimeRate] AS c70 ,
                    b.[OnTimeMemo] AS c71 ,
                    b.[IsSafe] AS c72 ,
                    b.[SafeMemo] AS c73 ,
                    b.[IsFresh] AS c74 ,
                    b.[FreshMemo] AS c75 ,
                    b.[IsPacking] AS c76 ,
                    b.[PackingMemo] AS c77 ,
                    b.[IsCost] AS c78 ,
                    b.[CostMemo] AS c79 ,
                    b.[OtherScore] AS c80 ,
                    b.[OnLineScore] AS c81 ,
                    b.[CommentDate] AS c82 ,
                    b.[CommentTime] AS c83 ,
                    b.[TYYYDRDC] AS c84 ,
                    b.[TYDBDRDC] AS c85 ,
                    b.[TYCTDRDC] AS c86 ,
                    b.[TYDBTYYYDRDC] AS c87 ,
                    b.[TYDBTYCTDRDC] AS c88 ,
                    b.[TYDBTTYYYTYCTDRDC] AS c89 ,
                    b.[OverCount] AS c90 ,
                    b.[PersonalGet] AS c92 ,
                    b.[DMName] AS c93 ,
                    b.[DMMUDID] AS c94 ,
                    b.[Level2DMName] AS c95 ,
                    b.[Level2DMMUDID] AS c96 ,
                    b.[Level3DMName] AS c97 ,
                    b.[Level3DMMUDID] AS c98 ,
                    b.[IsUploadFile] AS c99 ,
                    b.[UploadFileDate] AS c100 ,
                    b.[UploadFileTime] AS c101 ,
                    b.[UploadFileDMName] AS c102 ,
                    b.[UploadFileDMMUDID] AS c103 ,
                    b.[UploadFileApproveDate] AS c104 ,
                    b.[UploadFileApproveTime] AS c105 ,
                    b.[UploadFileState] AS c106 ,
                    b.[IsPersonCountSame] AS c107 ,
                    b.[PersonCountChangeReason] AS c109 ,
                    b.[UploadFileReopen] AS c110 ,
                    b.[UploadFileReopenOperatorName] AS c111 ,
                    b.[UploadFileReopenOperatorMUDID] AS c112 ,
                    b.[UploadFileReopenDate] AS c113 ,
                    b.[UploadFileReopenTime] AS c114 ,
                    b.[UploadFileReopenFromName] AS C133,
                    b.[UploadFileReopenFromMUDID] AS C134,
                    b.[UploadFileReopenReason] AS c115,
                    b.[UploadFileReopenMemo] AS c135,
                    b.[UploadFileReopenState] AS c116 ,
                    b.[UploadFileIsTransfer] AS c117 ,
                    b.[UploadFileTransferOperatorName] AS c118 ,
                    b.[UploadFileTransferOperatorMUDID] AS c119 ,
                    b.[UploadFileTransferName] AS c120 ,
                    b.[UploadFileTransferMUDID] AS c121 ,
                    b.[UploadFileTransferDate] AS c122 ,
                    b.[UploadFileTransferTime] AS c123 ,
                    b.[UploadFileIsReassign] AS c124 ,
                    b.[UploadFileReassignOperator] AS c125 ,
                    b.[UploadFileReassignOperatorMUDID] AS c126 ,
                    b.[UploadFileReassign] AS c127 ,
                    b.[UploadFileReassignMUDID] AS c128 ,
                    b.[UploadFileReassignDate] AS c129 ,
                    b.[UploadFileReassignTime] AS c130 ,
                    b.[ProjectMemo] AS c131,
                    b.[Details] AS c136,
                    b.[OrderState] AS c137,
                    b.[IsMealSame] AS c138,
                    b.[IsMeetingInfoSame] AS c139,
                    b.[MeetingInfoSameReason] AS c140,
                    b.[RDSDName] AS c141,
                    b.[RDSDMUDID] AS c142,
                    b.[VeevaMeetingID] AS c143,
					b.[MRTerritoryCode] AS c144,
                    b.[RDTerritoryCode] AS c145,
                    b.[SupplierSpecialRemark] AS c146,
					b.[IsCompleteDelivery] AS c147,
					b.[SupplierConfirmAmount] AS c148,					
					CASE WHEN b.[GSKConfirmAmount] is not null  THEN b.[GSKConfirmAmount]
                    WHEN b.[GSKConfirmAmount] is null  THEN b.[ActualTotalAmount] END AS c149,
					b.[GSKConAAReason] AS c150,
					b.[MealPaymentAmount] AS c151,
					b.[MealPaymentPO] AS c152,
					b.[AccountingTime] AS c153
                FROM [P_HT_Order_Report] b ";

            if (!string.IsNullOrEmpty(IsSpecialOrder))
            {
                sql += " LEFT JOIN [P_ORDER] C ON B.HTCODE = C.CN WHERE 1=1";
            }
            else
            {
                sql += " WHERE 1=1";
            }

            #endregion

            List<HT_Order_Report> rtnData = null;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<HT_Order_Report>(rows, page, out total,
                    sql + _sql, "ORDER BY b.[DelieverDate] DESC, b.[DelieverDate] DESC ", listParams.ToArray());


            }
            return rtnData;
        }
        #endregion 

    }
}