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
    public class OrderDao : IOrderDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        private string _dbName = ConfigurationManager.AppSettings["NonPositionDBName"];

        #region 载入用户的订单
        /// <summary>
        /// 载入用户的订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadByUserId(string userId, DateTime begin, DateTime end, int[] state, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_ORDER>(rows, page, out total,
                    $"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE (([UserId]=@UserId and isnull(IsTransfer,0)=0) or ([TransferUserMUDID]=@UserId and IsTransfer=1)) AND ([CreateDate] > @begin AND [CreateDate] < @end) AND State IN ({string.Join(",", state)}) ",
                    " ORDER BY CreateDate DESC  ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId),
                        SqlParameterFactory.GetSqlParameter("@begin", begin),
                        SqlParameterFactory.GetSqlParameter("@end", end)
                    });
                return list;
            }

        }
        #endregion

        #region 载入用户1.0的订单
        /// <summary>
        /// 载入用户的订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadOldOrderByUserId(string userId, DateTime begin, DateTime end, int[] state, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_ORDER>(rows, page, out total,
                    $"SELECT *,null as [RealCount], null as [RealCountChangeReason], null as [RealCountChangeRemrak], null as [RealPrice], null as [RealPriceChangeReason], null as [RealPriceChangeRemark], null as [SpecialRemarksProjectTeam], null as [IsSpecialOrder], null as [SpecialOrderReason], null as [SpecialOrderOperatorName], null as [SpecialOrderOperatorMUDID], null as [SpecialOrderOperateDate], 1 as [IsOrderUpload], null as [IsPushOne], null as [IsPushTwo], null as [IsTransfer], null as [TransferOperatorMUDID], null as [TransferOperatorName], null as [TransferUserMUDID], null as [TransferUserName], null as [TransferOperateDate] FROM [P_ORDER] WITH(NOLOCK) WHERE [UserId]=@UserId AND ([CreateDate] > @begin AND [CreateDate] < @end) AND State IN ({string.Join(",", state)}) ",
                    " ORDER BY CreateDate DESC  ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId),
                        SqlParameterFactory.GetSqlParameter("@begin", begin),
                        SqlParameterFactory.GetSqlParameter("@end", end)
                    });
                return list;
            }

        }
        #endregion

        #region 载入用户待收餐的订单
        /// <summary>
        /// 载入用户待收餐的订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="state"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadReceiveOrderByUserId(string userId, DateTime begin, DateTime end, int[] state, int rows, int page, out int total)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.LoadPages<P_ORDER>(rows, page, out total,
                    $"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE (([UserId]=@UserId and isnull(IsTransfer,0)=0) or ([TransferUserMUDID]=@UserId and IsTransfer=1)) AND ([CreateDate] > @begin AND [CreateDate] < @end) AND DeliverTime<@CurrentTime AND State IN ({string.Join(",", state)}) ",
                    " ORDER BY CreateDate DESC  ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userId),
                        SqlParameterFactory.GetSqlParameter("@begin", begin),
                        SqlParameterFactory.GetSqlParameter("@end", end),
                        SqlParameterFactory.GetSqlParameter("@CurrentTime", DateTime.Now),
                    });
                return list;
            }

        }
        #endregion

        #region 增加新订单
        /// <summary>
        /// 增加新订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO [P_ORDER] ([ID], [UserId], [Market], [HospitalId], [Address], [CN], [RestaurantId], 
[RestaurantName], [RestaurantLogo], [TotalPrice], [DeliveryGeo], [Detail], [FoodCount], [AttendCount], 
[DeliveryAddress], [Consignee], [Phone], [DeliverTime], [Remark], [MMCoEImage], [State],[ReceiveCode], [CreateDate], [XmsOrderId], 
[SendOrderDate], [XmsTotalPrice], [IsOuterMeeting], [RestaurantTel], [RestaurantAddress], [MMCoEApproveState], [PO], [WBS], 
[IsNonHT], [MeetingCode], [MeetingName], [TA], [EnterpriseOrderId], [Channel], [Province], [City], [HospitalName], [ActionState]) 
VALUES (@ID, @UserId, @Market, @HospitalId, @Address, @CN, @RestaurantId, 
@RestaurantName, @RestaurantLogo, @TotalPrice, @DeliveryGeo, @Detail, @FoodCount, @AttendCount, 
@DeliveryAddress, @Consignee, @Phone, @DeliverTime, @Remark, @MMCoEImage, @State, @ReceiveCode, @CreateDate, @XmsOrderId, 
@SendOrderDate, @XmsTotalPrice, @IsOuterMeeting, @RestaurantTel, @RestaurantAddress, @MMCoEApproveState, @PO, @WBS, 
@IsNonHT, @MeetingCode, @MeetingName, @TA, @EnterpriseOrderId, @Channel, @Province, @City, @HospitalName,'0') ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@UserId", entity.UserId),
                        SqlParameterFactory.GetSqlParameter("@Market", entity.Market),
                        SqlParameterFactory.GetSqlParameter("@HospitalId", entity.HospitalId),
                        SqlParameterFactory.GetSqlParameter("@Address", entity.Address),
                        SqlParameterFactory.GetSqlParameter("@CN", entity.CN),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                        SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                        SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                        SqlParameterFactory.GetSqlParameter("@DeliveryGeo", entity.DeliveryGeo),
                        SqlParameterFactory.GetSqlParameter("@Detail", entity.Detail),
                        SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                        SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                        SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                        SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@ReceiveCode", entity.ReceiveCode),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", entity.XmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@SendOrderDate", entity.SendOrderDate),
                        SqlParameterFactory.GetSqlParameter("@XmsTotalPrice", entity.XmsTotalPrice),
                        SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                        SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                        SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                        SqlParameterFactory.GetSqlParameter("@PO", entity.PO),
                        SqlParameterFactory.GetSqlParameter("@WBS", entity.WBS),
                        SqlParameterFactory.GetSqlParameter("@MeetingCode", entity.MeetingCode),
                        SqlParameterFactory.GetSqlParameter("@MeetingName", entity.MeetingName),
                        SqlParameterFactory.GetSqlParameter("@TA", entity.TA),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", entity.IsNonHT),
                        SqlParameterFactory.GetSqlParameter("@EnterpriseOrderId", entity.EnterpriseOrderId),
                        SqlParameterFactory.GetSqlParameter("@Channel", entity.Channel),
                        SqlParameterFactory.GetSqlParameter("@Province", entity.Province),
                        SqlParameterFactory.GetSqlParameter("@City", entity.City),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", entity.HospitalName)
                    });

            }

        }
        #endregion

        #region 增加新订单到缓存表
        /// <summary>
        /// 增加新订单到缓存表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddCache(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO [P_ORDERCACHE] ([ID], [UserId], [Market], [HospitalId], [Address], [CN], [RestaurantId], 
[RestaurantName], [RestaurantLogo], [TotalPrice], [DeliveryGeo], [Detail], [FoodCount], [AttendCount], 
[DeliveryAddress], [Consignee], [Phone], [DeliverTime], [Remark], [MMCoEImage], [State],[ReceiveCode], [CreateDate], [XmsOrderId], 
[SendOrderDate], [XmsTotalPrice], [IsOuterMeeting], [RestaurantTel], [RestaurantAddress], [MMCoEApproveState], [PO], [WBS], 
[IsNonHT], [MeetingCode], [MeetingName], [TA], [EnterpriseOrderId], [Channel], [Province], [City], [HospitalName],[IsRetuen],[IsChange],[IsReturn],[IsOrderUpload],[IsPushOne],[IsPushTwo],[IsTransfer]) 
VALUES (@ID, @UserId, @Market, @HospitalId, @Address, @CN, @RestaurantId, 
@RestaurantName, @RestaurantLogo, @TotalPrice, @DeliveryGeo, @Detail, @FoodCount, @AttendCount, 
@DeliveryAddress, @Consignee, @Phone, @DeliverTime, @Remark, @MMCoEImage, @State, @ReceiveCode, @CreateDate, @XmsOrderId, 
@SendOrderDate, @XmsTotalPrice, @IsOuterMeeting, @RestaurantTel, @RestaurantAddress, @MMCoEApproveState, @PO, @WBS, 
@IsNonHT, @MeetingCode, @MeetingName, @TA, @EnterpriseOrderId, @Channel, @Province, @City, @HospitalName,0,0,0,0,0,0,0) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@UserId", entity.UserId),
                        SqlParameterFactory.GetSqlParameter("@Market", entity.Market),
                        SqlParameterFactory.GetSqlParameter("@HospitalId", entity.HospitalId),
                        SqlParameterFactory.GetSqlParameter("@Address", entity.Address),
                        SqlParameterFactory.GetSqlParameter("@CN", entity.CN),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                        SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                        SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                        SqlParameterFactory.GetSqlParameter("@DeliveryGeo", entity.DeliveryGeo),
                        SqlParameterFactory.GetSqlParameter("@Detail", entity.Detail),
                        SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                        SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                        SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                        SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@ReceiveCode", entity.ReceiveCode),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", entity.XmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@SendOrderDate", entity.SendOrderDate),
                        SqlParameterFactory.GetSqlParameter("@XmsTotalPrice", entity.XmsTotalPrice),
                        SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                        SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                        SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                        SqlParameterFactory.GetSqlParameter("@PO", entity.PO),
                        SqlParameterFactory.GetSqlParameter("@WBS", entity.WBS),
                        SqlParameterFactory.GetSqlParameter("@MeetingCode", entity.MeetingCode),
                        SqlParameterFactory.GetSqlParameter("@MeetingName", entity.MeetingName),
                        SqlParameterFactory.GetSqlParameter("@TA", entity.TA),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", entity.IsNonHT),
                        SqlParameterFactory.GetSqlParameter("@EnterpriseOrderId", entity.EnterpriseOrderId),
                        SqlParameterFactory.GetSqlParameter("@Channel", entity.Channel),
                        SqlParameterFactory.GetSqlParameter("@Province", entity.Province),
                        SqlParameterFactory.GetSqlParameter("@City", entity.City),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", entity.HospitalName),
                    });

            }

        }
        #endregion

        #region 写入p_ordercache p_order 占用CN号
        public int AddOrder(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            try
            {

                using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction();
                    #region insert p_ordercache
                    //p_ordercache
                    SqlCommand commandAddCache = new SqlCommand(
                                            " INSERT INTO[P_ORDERCACHE]([ID], [UserId], [Market], [HospitalId], [Address], [CN], [RestaurantId], " +
                                            " [RestaurantName], [RestaurantLogo], [TotalPrice], [DeliveryGeo], [Detail], [FoodCount], [AttendCount], " +
                                            " [DeliveryAddress], [Consignee], [Phone], [DeliverTime], [Remark], [MMCoEImage], [State],[ReceiveCode], [CreateDate], [XmsOrderId], " +
                                            " [SendOrderDate], [XmsTotalPrice], [IsOuterMeeting], [RestaurantTel], [RestaurantAddress], [MMCoEApproveState], [PO], [WBS], " +
                                            " [IsNonHT], [MeetingCode], [MeetingName], [TA], [EnterpriseOrderId], [Channel], [Province], [City], [HospitalName]," +
                                            " [IsRetuen],[IsChange],[IsReturn],[IsOrderUpload],[IsPushOne],[IsPushTwo],[IsTransfer]) " +
                                            " VALUES(@ID, @UserId, @Market, @HospitalId, @Address, @CN, @RestaurantId, " +
                                            " @RestaurantName, @RestaurantLogo, @TotalPrice, @DeliveryGeo, @Detail, @FoodCount, @AttendCount, " +
                                            " @DeliveryAddress, @Consignee, @Phone, @DeliverTime, @Remark, @MMCoEImage, @State, @ReceiveCode, @CreateDate, @XmsOrderId, " +
                                            " @SendOrderDate, @XmsTotalPrice, @IsOuterMeeting, @RestaurantTel, @RestaurantAddress, @MMCoEApproveState, @PO, @WBS, " +
                                            " @IsNonHT, @MeetingCode, @MeetingName, @TA, @EnterpriseOrderId, @Channel, @Province, @City, @HospitalName, 0, 0, 0, 0, 0, 0, 0) ",
                                            conn);
                    commandAddCache.Transaction = tran;
                    commandAddCache.Parameters.AddRange(
                        new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@UserId", entity.UserId),
                        SqlParameterFactory.GetSqlParameter("@Market", entity.Market),
                        SqlParameterFactory.GetSqlParameter("@HospitalId", entity.HospitalId),
                        SqlParameterFactory.GetSqlParameter("@Address", entity.Address),
                        SqlParameterFactory.GetSqlParameter("@CN", entity.CN),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                        SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                        SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                        SqlParameterFactory.GetSqlParameter("@DeliveryGeo", entity.DeliveryGeo),
                        SqlParameterFactory.GetSqlParameter("@Detail", entity.Detail),
                        SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                        SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                        SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                        SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@ReceiveCode", entity.ReceiveCode),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", entity.XmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@SendOrderDate", entity.SendOrderDate),
                        SqlParameterFactory.GetSqlParameter("@XmsTotalPrice", entity.XmsTotalPrice),
                        SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                        SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                        SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                        SqlParameterFactory.GetSqlParameter("@PO", entity.PO),
                        SqlParameterFactory.GetSqlParameter("@WBS", entity.WBS),
                        SqlParameterFactory.GetSqlParameter("@MeetingCode", entity.MeetingCode),
                        SqlParameterFactory.GetSqlParameter("@MeetingName", entity.MeetingName),
                        SqlParameterFactory.GetSqlParameter("@TA", entity.TA),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", entity.IsNonHT),
                        SqlParameterFactory.GetSqlParameter("@EnterpriseOrderId", entity.EnterpriseOrderId),
                        SqlParameterFactory.GetSqlParameter("@Channel", entity.Channel),
                        SqlParameterFactory.GetSqlParameter("@Province", entity.Province),
                        SqlParameterFactory.GetSqlParameter("@City", entity.City),
                        SqlParameterFactory.GetSqlParameter("@HospitalName", entity.HospitalName),
                        }
                    );
                    commandAddCache.ExecuteNonQuery();
                    #endregion

                    #region insert p_order
                    //p_order
                    SqlCommand commandAddOrder = new SqlCommand(
                                " INSERT INTO[P_ORDER]([ID], [UserId], [Market], [HospitalId], [Address], [CN], [RestaurantId], " +
                                " [RestaurantName], [RestaurantLogo], [TotalPrice], [DeliveryGeo], [Detail], [FoodCount], [AttendCount], " +
                                " [DeliveryAddress], [Consignee], [Phone], [DeliverTime], [Remark], [MMCoEImage], [State],[ReceiveCode], [CreateDate], [XmsOrderId], " +
                                " [SendOrderDate], [XmsTotalPrice], [IsOuterMeeting], [RestaurantTel], [RestaurantAddress], [MMCoEApproveState], [PO], [WBS], " +
                                " [IsNonHT], [MeetingCode], [MeetingName], [TA], [EnterpriseOrderId], [Channel], [Province], [City], [HospitalName], [ActionState]) " +
                                " VALUES(@ID, @UserId, @Market, @HospitalId, @Address, @CN, @RestaurantId, " +
                               "  @RestaurantName, @RestaurantLogo, @TotalPrice, @DeliveryGeo, @Detail, @FoodCount, @AttendCount, " +
                                " @DeliveryAddress, @Consignee, @Phone, @DeliverTime, @Remark, @MMCoEImage, @State, @ReceiveCode, @CreateDate, @XmsOrderId, " +
                                " @SendOrderDate, @XmsTotalPrice, @IsOuterMeeting, @RestaurantTel, @RestaurantAddress, @MMCoEApproveState, @PO, @WBS, " +
                                " @IsNonHT, @MeetingCode, @MeetingName, @TA, @EnterpriseOrderId, @Channel, @Province, @City, @HospitalName, '0') ",
                                conn);
                    commandAddOrder.Transaction = tran;
                    commandAddOrder.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                            SqlParameterFactory.GetSqlParameter("@UserId", entity.UserId),
                            SqlParameterFactory.GetSqlParameter("@Market", entity.Market),
                            SqlParameterFactory.GetSqlParameter("@HospitalId", entity.HospitalId),
                            SqlParameterFactory.GetSqlParameter("@Address", entity.Address),
                            SqlParameterFactory.GetSqlParameter("@CN", entity.CN),
                            SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                            SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                            SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                            SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                            SqlParameterFactory.GetSqlParameter("@DeliveryGeo", entity.DeliveryGeo),
                            SqlParameterFactory.GetSqlParameter("@Detail", entity.Detail),
                            SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                            SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                            SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                            SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                            SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                            SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                            SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                            SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                            SqlParameterFactory.GetSqlParameter("@State", entity.State),
                            SqlParameterFactory.GetSqlParameter("@ReceiveCode", entity.ReceiveCode),
                            SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                            SqlParameterFactory.GetSqlParameter("@XmsOrderId", entity.XmsOrderId),
                            SqlParameterFactory.GetSqlParameter("@SendOrderDate", entity.SendOrderDate),
                            SqlParameterFactory.GetSqlParameter("@XmsTotalPrice", entity.XmsTotalPrice),
                            SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                            SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                            SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                            SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                            SqlParameterFactory.GetSqlParameter("@PO", entity.PO),
                            SqlParameterFactory.GetSqlParameter("@WBS", entity.WBS),
                            SqlParameterFactory.GetSqlParameter("@MeetingCode", entity.MeetingCode),
                            SqlParameterFactory.GetSqlParameter("@MeetingName", entity.MeetingName),
                            SqlParameterFactory.GetSqlParameter("@TA", entity.TA),
                            SqlParameterFactory.GetSqlParameter("@IsNonHT", entity.IsNonHT),
                            SqlParameterFactory.GetSqlParameter("@EnterpriseOrderId", entity.EnterpriseOrderId),
                            SqlParameterFactory.GetSqlParameter("@Channel", entity.Channel),
                            SqlParameterFactory.GetSqlParameter("@Province", entity.Province),
                            SqlParameterFactory.GetSqlParameter("@City", entity.City),
                            SqlParameterFactory.GetSqlParameter("@HospitalName", entity.HospitalName)
                        }
                    );
                    commandAddOrder.ExecuteNonQuery();
                    #endregion

                    #region 占用CN号
                    SqlCommand commandPreCN = new SqlCommand(
                            "UPDATE [P_PreApproval] SET IsUsed=1,ActionState='0' WHERE [HTCode]=@HTCode ",
                            conn);
                    commandPreCN.Transaction = tran;
                    commandPreCN.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@HTCode", entity.CN)
                        }
                    );
                    commandPreCN.ExecuteNonQuery();
                    #endregion

                    tran.Commit();
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.Error("新建订单失败HT单号：" + entity.CN + ex);
                return 0;
            }
        }
        #endregion

        public int DeleteOrder(Guid ID, string cn)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            try
            {

                using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction();
                    #region delete p_ordercache
                    //p_ordercache
                    SqlCommand commandAddCache = new SqlCommand(
                                            " delete from [P_ORDERCACHE] where ID = @ID ",
                                            conn);
                    commandAddCache.Transaction = tran;
                    commandAddCache.Parameters.AddRange(
                        new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                        }
                    );
                    commandAddCache.ExecuteNonQuery();
                    #endregion

                    #region delete p_order
                    //p_order
                    SqlCommand commandAddOrder = new SqlCommand(
                                " delete from [P_ORDER] where ID = @ID ",
                                conn);
                    commandAddOrder.Transaction = tran;
                    commandAddOrder.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", ID)
                        }
                    );
                    commandAddOrder.ExecuteNonQuery();
                    #endregion

                    #region 取消占用CN号
                    SqlCommand commandPreCN = new SqlCommand(
                            "UPDATE [P_PreApproval] SET IsUsed=0,ActionState='0' WHERE [HTCode]=@HTCode ",
                            conn);
                    commandPreCN.Transaction = tran;
                    commandPreCN.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@HTCode", cn)
                        }
                    );
                    commandPreCN.ExecuteNonQuery();
                    #endregion

                    tran.Commit();
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.Error("删除订单失败，HT单号：" + cn + ex);
                return 0;
            }

        }

        public int UpdateOrder(Guid ID, string XmsOrderId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(@"UPDATE [P_ORDER] SET XmsOrderId = @XmsOrderId,ActionState='0' WHERE [ID] = @ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", XmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });

            }

        }

        public int ChangeOrder(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            try
            {
                using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction();
                    #region 更新p_ordercache
                    SqlCommand commandChangeCache = new SqlCommand(
                                "UPDATE P_ORDERCACHE SET ChangeID=@ChangeID, ChangeDetail=@ChangeDetail, IsChange=@IsChange, State=@State, MMCoEImage=@MMCoEImage,IsReturn=@IsReturn,IsRetuen=@IsRetuen, "
                                + " RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, TotalPrice=@TotalPrice, FoodCount=@FoodCount, "
                                + " AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, "
                                + " IsOuterMeeting=@IsOuterMeeting, RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, MMCoEApproveState=@MMCoEApproveState "
                                + " WHERE ID=@ID",
                                conn);
                    commandChangeCache.Transaction = tran;
                    commandChangeCache.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                            SqlParameterFactory.GetSqlParameter("@ChangeID", entity.ChangeID),
                            SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                            SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                            SqlParameterFactory.GetSqlParameter("@State", entity.State),
                            SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                            SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                            SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                            SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                            SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                            SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                            SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                            SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                            SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                            SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                            SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                            SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                            SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                            SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                            SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                            SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                            SqlParameterFactory.GetSqlParameter("@IsReturn", 0),
                            SqlParameterFactory.GetSqlParameter("@IsRetuen", 0)

                        }
                    );
                    commandChangeCache.ExecuteNonQuery();
                    #endregion

                    #region 更新p_order
                    SqlCommand commandChangeOrder = new SqlCommand(
                                "UPDATE P_ORDER SET ChangeID=@ChangeID, ChangeDetail=@ChangeDetail, IsChange=@IsChange, State=@State, MMCoEImage=@MMCoEImage,IsReturn=@IsReturn,IsRetuen=@IsRetuen, "
                                + " RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, TotalPrice=@TotalPrice, FoodCount=@FoodCount, "
                                + " AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, "
                                + " IsOuterMeeting=@IsOuterMeeting, RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, MMCoEApproveState=@MMCoEApproveState, ActionState='0' "
                                + " WHERE ID=@ID",
                                conn);
                    commandChangeOrder.Transaction = tran;
                    commandChangeOrder.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                            SqlParameterFactory.GetSqlParameter("@ChangeID", entity.ChangeID),
                            SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                            SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                            SqlParameterFactory.GetSqlParameter("@State", entity.State),
                            SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                            SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                            SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                            SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                            SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                            SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                            SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                            SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                            SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                            SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                            SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                            SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                            SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                            SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                            SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                            SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                            SqlParameterFactory.GetSqlParameter("@IsReturn", 0),
                            SqlParameterFactory.GetSqlParameter("@IsRetuen", 0)
                        }
                    );
                    commandChangeOrder.ExecuteNonQuery();
                    #endregion

                    tran.Commit();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("修改失败，HT单号：" + entity.CN + ex);
                return 0;
            }
        }

        public int RestoreOrder(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            try
            {
                using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
                {
                    conn.Open();
                    var tran = conn.BeginTransaction();
                    #region 还原p_ordercache
                    SqlCommand commandChangeCache = new SqlCommand(
                                "UPDATE P_ORDERCACHE SET ChangeID=@ChangeID, ChangeDetail=@ChangeDetail, IsChange=@IsChange, State=@State, MMCoEImage=@MMCoEImage,IsReturn=@IsReturn,IsRetuen=@IsRetuen, "
                                + " RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, TotalPrice=@TotalPrice, FoodCount=@FoodCount, "
                                + " AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, "
                                + " IsOuterMeeting=@IsOuterMeeting, RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, MMCoEApproveState=@MMCoEApproveState "
                                + " WHERE ID=@ID",
                                conn);
                    commandChangeCache.Transaction = tran;
                    commandChangeCache.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                            SqlParameterFactory.GetSqlParameter("@ChangeID", entity.ChangeID),
                            SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                            SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                            SqlParameterFactory.GetSqlParameter("@State", entity.State),
                            SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                            SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                            SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                            SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                            SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                            SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                            SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                            SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                            SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                            SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                            SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                            SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                            SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                            SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                            SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                            SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                            SqlParameterFactory.GetSqlParameter("@IsReturn", 0),
                            SqlParameterFactory.GetSqlParameter("@IsRetuen", 0)

                        }
                    );
                    #endregion

                    #region 还原p_order
                    SqlCommand commandChangeOrder = new SqlCommand(
                                "UPDATE P_ORDER SET ChangeID=@ChangeID, ChangeDetail=@ChangeDetail, IsChange=@IsChange, State=@State, MMCoEImage=@MMCoEImage,IsReturn=@IsReturn,IsRetuen=@IsRetuen, "
                                + " RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, TotalPrice=@TotalPrice, FoodCount=@FoodCount, "
                                + " AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, "
                                + " IsOuterMeeting=@IsOuterMeeting, RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, MMCoEApproveState=@MMCoEApproveState, ActionState='0' "
                                + " WHERE ID=@ID",
                                conn);
                    commandChangeOrder.Transaction = tran;
                    commandChangeOrder.Parameters.AddRange(
                        new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                            SqlParameterFactory.GetSqlParameter("@ChangeID", entity.ChangeID),
                            SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                            SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                            SqlParameterFactory.GetSqlParameter("@State", entity.State),
                            SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                            SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                            SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                            SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                            SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                            SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                            SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                            SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                            SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                            SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                            SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                            SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                            SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                            SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                            SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                            SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                            SqlParameterFactory.GetSqlParameter("@IsReturn", 0),
                            SqlParameterFactory.GetSqlParameter("@IsRetuen", 0)
                        }
                    );
                    #endregion

                    tran.Commit();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("修改失败，HT单号：" + entity.CN + ex);
                return 0;
            }
        }

        #region 审批后下单
        /// <summary>
        /// 审批后下单
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        public int SaveXmsOrderId(Guid ID, string xmsOrderId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                return sqlServerTemplate.Update("UPDATE P_ORDER SET [XmsOrderId]=@XmsOrderId,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", ID),
                        SqlParameterFactory.GetSqlParameter("@xmsOrderId", xmsOrderId)
                    });
            }
        }
        #endregion

        #region 修改订单
        /// <summary>
        /// 修改订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Change(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    "UPDATE P_ORDER SET ChangeID=@ChangeID, ChangeDetail=@ChangeDetail, IsChange=@IsChange, State=@State, MMCoEImage=@MMCoEImage,IsReturn=@IsReturn,IsRetuen=@IsRetuen, "
                    + " RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, TotalPrice=@TotalPrice, FoodCount=@FoodCount, "
                    + " AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, "
                    + " IsOuterMeeting=@IsOuterMeeting, RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, MMCoEApproveState=@MMCoEApproveState,ActionState='0' "
                    + " WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@ChangeID", entity.ChangeID),
                        SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                        SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                        SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                        SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                        SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                        SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                        SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                        SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                        SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                        SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                        SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                        SqlParameterFactory.GetSqlParameter("@IsReturn", 0),
                        SqlParameterFactory.GetSqlParameter("@IsRetuen", 0)

                    });
            }
        }
        #endregion

        #region 修改缓存订单
        /// <summary>
        /// 修改缓存订单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ChangeCache(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    "UPDATE P_ORDERCACHE SET ChangeID=@ChangeID, ChangeDetail=@ChangeDetail, IsChange=@IsChange, State=@State, MMCoEImage=@MMCoEImage,IsReturn=@IsReturn,IsRetuen=@IsRetuen, "
                    + " RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, TotalPrice=@TotalPrice, FoodCount=@FoodCount, "
                    + " AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, "
                    + " IsOuterMeeting=@IsOuterMeeting, RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, MMCoEApproveState=@MMCoEApproveState "
                    + " WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@ChangeID", entity.ChangeID),
                        SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                        SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                        SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                        SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                        SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                        SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                        SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                        SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                        SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                        SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                        SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),
                        SqlParameterFactory.GetSqlParameter("@IsReturn", 0),
                        SqlParameterFactory.GetSqlParameter("@IsRetuen", 0)

                    });
            }
        }
        #endregion

        #region 修改订单（从未在小秘书下过单）
        /// <summary>
        /// 修改订单（从未在小秘书下过单）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ChangeForNonXms(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    "UPDATE P_ORDER SET Detail=@Detail, State=@State, MMCoEImage=@MMCoEImage, "
                    + " RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, TotalPrice=@TotalPrice, FoodCount=@FoodCount, "
                    + " AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, "
                    + " IsOuterMeeting=@IsOuterMeeting, RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, MMCoEApproveState=@MMCoEApproveState,ActionState='0' "
                    + " WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@Detail", entity.Detail),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                        SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                        SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                        SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                        SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                        SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                        SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                        SqlParameterFactory.GetSqlParameter("@IsOuterMeeting", entity.IsOuterMeeting),
                        SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                        SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", entity.MMCoEApproveState),


                    });
            }
        }
        #endregion

        #region 根据订单号查询订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindByID(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_ORDER>("SELECT * FROM P_ORDER WITH(NOLOCK) WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
        }
        #endregion

        #region 根据订单号查询缓存订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindCacheOrderByID(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_ORDER>("SELECT * FROM P_ORDERCACHE WITH(NOLOCK) WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
        }
        #endregion

        #region 根据订单号查询1.0订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindOldOrderByID(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_ORDER>("SELECT * FROM P_ORDER WITH(NOLOCK) WHERE ID=@ID",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
        }
        #endregion

        #region 根据订单号查询订单
        /// <summary>
        /// 根据订单号查询订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P_ORDER FindByXmlOrderId(string xmsOrderId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_ORDER>("SELECT TOP 1 * FROM P_ORDER WITH(NOLOCK) WHERE XmsOrderId=@XmsOrderId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", xmsOrderId)
                    });
            }
        }
        #endregion

        #region 取消订单
        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CancelOrder(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var dateTime = DateTime.Now;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [State]={OrderState.RETURNING},[IsRetuen]={OrderIsRetuen.YES},[IsReturn]={OrderIsRetuen.YES},ReturnOrderDate=@ReturnOrderDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@ReturnOrderDate", dateTime)
                    });
            }
        }
        #endregion

        #region 取消订单成功
        /// <summary>
        /// 取消订单成功
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <returns></returns>
        public int CancelOrderSuccess(string xmsOrderId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [State]={OrderState.RETURNSUCCES},[IsRetuen]={OrderIsRetuen.SUCCESS},ActionState='0' WHERE XmsOrderId=@XmsOrderId ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", xmsOrderId)
                    });
            }
        }
        #endregion

        #region 取消订单失败
        /// <summary>
        /// 取消订单失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsReason"></param>
        /// <returns></returns>
        public int CancelOrderFail(string xmsOrderId, string xmsReason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"UPDATE [P_ORDER] SET [State]={OrderState.RETURNFAIL},[IsRetuen]={OrderIsRetuen.FAIL},XmsOrderReason=@XmsOrderReason,ActionState='0' WHERE XmsOrderId=@XmsOrderId ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", xmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderReason", xmsReason)
                    });
            }
        }
        #endregion

        #region 原单配送
        /// <summary>
        /// 原单配送
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int OriginalOrderSend(Guid ID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [IsRetuen]={OrderIsRetuen.POSTFOOD},[IsDelivery]={OrderIsRetuen.YES},ActionState='0' WHERE ID=@ID AND [State]={OrderState.RETURNFAIL} AND [IsRetuen]={OrderIsRetuen.FAIL} ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", ID)
                    });
            }
        }
        #endregion

        #region 原单配送(小秘书反馈成功)
        /// <summary>
        /// 原单配送(小秘书反馈成功)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public int OriginalOrderSendSuccess(string xmsOrderId, string xmsReason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"UPDATE [P_ORDER] SET [IsRetuen]={OrderIsRetuen.POSTSUCCESS},[State]={OrderState.RETURNFAIL},[XmsOrderReason] =@XmsOrderReason,ActionState='0'"
                    + $" WHERE [XmsOrderId]=@xmsOrderId AND (([State]={OrderState.RETURNFAIL} AND ([IsRetuen]={OrderIsRetuen.POSTFOOD} or [ReturnOrderDate]>[DeliverTime])) "
                    + $" or ([State]={OrderState.RETURNING}))",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@xmsOrderId", xmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderReason", xmsReason)
                    });
            }
        }
        #endregion

        #region 原单配送(小秘书反馈失败)
        /// <summary>
        /// 原单配送(小秘书反馈失败)
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsReason"></param>
        /// <returns></returns>
        public int OriginalOrderSendFail(string xmsOrderId, string xmsReason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"UPDATE [P_ORDER] SET [State]={OrderState.RETURNFAIL}, [IsRetuen]={OrderIsRetuen.POSTFAIL}, XmsOrderReason=@XmsOrderReason,ActionState='0' "
                    + $" WHERE [XmsOrderId]=@xmsOrderId AND (([State]={OrderState.RETURNFAIL} AND ([IsRetuen]={OrderIsRetuen.POSTFOOD} or [ReturnOrderDate]>[DeliverTime])) "
                    + $" or ([State]={OrderState.RETURNING} AND [ReturnOrderDate]>[DeliverTime]))",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@XmsOrderReason", xmsReason),
                        SqlParameterFactory.GetSqlParameter("@xmsOrderId", xmsOrderId)
                    });
            }
        }
        #endregion

        #region 确认收餐
        /// <summary>
        /// 确认收餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Confirm(Guid id, string price, string reason, string remark, string count, string cReason, string cRemark, string isSame)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var receiveDate = DateTime.Now;

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [State]={OrderState.PERSIONRECEIVE}, ReceiveState={OrderState.PERSIONRECEIVE}, ReceiveDate=@ReceiveDate," +
                    " RealCount=@RealCount,RealCountChangeReason=@RealCountChangeReason,RealCountChangeRemrak=@RealCountChangeRemrak,RealPriceChangeReason=@RealPriceChangeReason,RealPriceChangeRemark=@RealPriceChangeRemark,RealPrice=@RealPrice,IsMealSame=@IsSame,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@RealCount", count),
                        SqlParameterFactory.GetSqlParameter("@RealPrice", price),
                        SqlParameterFactory.GetSqlParameter("@RealCountChangeReason", cReason),
                        SqlParameterFactory.GetSqlParameter("@RealCountChangeRemrak", cRemark),
                        SqlParameterFactory.GetSqlParameter("@RealPriceChangeReason", reason),
                        SqlParameterFactory.GetSqlParameter("@RealPriceChangeRemark", remark),
                         SqlParameterFactory.GetSqlParameter("@ReceiveDate", receiveDate),
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@IsSame", isSame),
                    });
            }
        }
        #endregion

        #region 查询需要系统收餐的订单
        /// <summary>
        /// 查询需要系统收餐的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadOrders()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM P_ORDER WITH(NOLOCK) WHERE DATEDIFF(HH, DeliverTime, GETDATE())>=168 AND (State={OrderState.SCHEDULEDSUCCESS} or State={OrderState.RETURNFAIL})", null);
            }
        }
        #endregion

        #region 系统收餐
        /// <summary>
        /// 系统收餐
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SystemConfirm(Guid id, string price, string count)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var receiveDate = DateTime.Now;

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [State]={OrderState.SYSTEMRECEIVE}, ReceiveState={OrderState.SYSTEMRECEIVE}, ReceiveDate=@ReceiveDate, RealPrice=@RealPrice, RealCount=@RealCount,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@RealCount", count),
                        SqlParameterFactory.GetSqlParameter("@RealPrice", price),
                        SqlParameterFactory.GetSqlParameter("@ReceiveDate", receiveDate),
                        SqlParameterFactory.GetSqlParameter("@ID", id)
                    });
            }
        }
        #endregion

        #region 系统收餐
        /// <summary>
        /// 系统收餐
        /// </summary>
        /// <returns></returns>
        public int SystemConfirm()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE P_ORDER SET State={OrderState.SYSTEMRECEIVE}, ReceiveState={OrderState.SYSTEMRECEIVE}, ReceiveDate=GETDATE(),ActionState='0' WHERE DATEDIFF(HH, DeliverTime, GETDATE())>=24 AND State={OrderState.SCHEDULEDSUCCESS}  ", null);
            }
        }
        #endregion

        #region 未送达
        /// <summary>
        /// 未送达
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Lost(Guid id)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var receiveDate = DateTime.Now;

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [State]={OrderState.FOODLOSE}, ReceiveState={OrderState.FOODLOSE},ReceiveDate=@ReceiveDate,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", id),
                        SqlParameterFactory.GetSqlParameter("@ReceiveDate", receiveDate)
                    });
            }
        }
        #endregion

        #region 预定成功
        /// <summary>
        /// 预定成功
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="code"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public int ScheduledSuccess(string xmsOrderId, string code, string remark)
        {
            var time = DateTime.Now;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"UPDATE [P_ORDER] SET [State]={OrderState.SCHEDULEDSUCCESS}, ReceiveCode=@ReceiveCode, ReceiveTime=@ReceiveTime,ActionState='0' WHERE XmsOrderId=@XmsOrderId  ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ReceiveCode", code),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", xmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@ReceiveTime", time)
                    });
            }
        }
        #endregion

        public int ADDLOG(string message)
        {
            var time = DateTime.Now;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"insert into ADD_LOG values(@message,@exp,@time) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@message", message),
                        SqlParameterFactory.GetSqlParameter("@exp", message),
                        SqlParameterFactory.GetSqlParameter("@time", time)
                    });
            }
        }

        #region 预定成功(改单)
        /// <summary>
        /// 预定成功(改单)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ScheduledSuccessForChange(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"UPDATE [P_ORDER] SET [State]=@State, ReceiveCode=@ReceiveCode, XmsOrderId=@XmsOrderId, Detail=@Detail, [XmsTotalPrice]=-1.00, [ChangeTotalPriceReason]=null, "
                    + $" ChangeDetail=@ChangeDetail, IsChange=@IsChange, OldXmlOrderId=@OldXmlOrderId,ActionState='0' WHERE ID=@ID ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@ReceiveCode", entity.ReceiveCode),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", entity.XmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@Detail", entity.Detail),
                        SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                        SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                        SqlParameterFactory.GetSqlParameter("@OldXmlOrderId", entity.OldXmlOrderId),
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID)
                    });
            }
        }
        #endregion

        #region 预定失败
        /// <summary>
        /// 预定失败
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public int ScheduledFail(string xmsOrderId, string remark)
        {
            var time = DateTime.Now;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"UPDATE [P_ORDER] SET [State]={OrderState.SCHEDULEDFAIL}, XmsOrderReason=@XmsOrderReason, ReceiveTime=@ReceiveTime,ActionState='0' WHERE XmsOrderId=@XmsOrderId  ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@XmsOrderReason", remark),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", xmsOrderId),
                        SqlParameterFactory.GetSqlParameter("@ReceiveTime", time)
                    });
            }
        }
        #endregion

        #region 预定失败(改单)
        /// <summary>
        /// 预定失败(改单)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ScheduledFailForChange(P_ORDER entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    @"UPDATE P_ORDER SET ChangeID=@ChangeID, ChangeDetail=@ChangeDetail, State=@State, IsChange=@IsChange, RestaurantId=@RestaurantId, RestaurantName=@RestaurantName, RestaurantLogo=@RestaurantLogo, 
                        TotalPrice=@TotalPrice, FoodCount=@FoodCount, AttendCount=@AttendCount, DeliveryAddress=@DeliveryAddress, Consignee=@Consignee, Phone=@Phone, DeliverTime=@DeliverTime, Remark=@Remark, MMCoEImage=@MMCoEImage, 
                        RestaurantTel=@RestaurantTel, RestaurantAddress=@RestaurantAddress, XmsOrderReason=@XmsOrderReason,ActionState='0' "
                        + $" WHERE ID=@ID  ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ChangeID", entity.ChangeID),
                        SqlParameterFactory.GetSqlParameter("@ChangeDetail", entity.ChangeDetail),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@IsChange", entity.IsChange),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@RestaurantName", entity.RestaurantName),
                        SqlParameterFactory.GetSqlParameter("@RestaurantLogo", entity.RestaurantLogo),
                        SqlParameterFactory.GetSqlParameter("@TotalPrice", entity.TotalPrice),
                        SqlParameterFactory.GetSqlParameter("@FoodCount", entity.FoodCount),
                        SqlParameterFactory.GetSqlParameter("@AttendCount", entity.AttendCount),
                        SqlParameterFactory.GetSqlParameter("@DeliveryAddress", entity.DeliveryAddress),
                        SqlParameterFactory.GetSqlParameter("@Consignee", entity.Consignee),
                        SqlParameterFactory.GetSqlParameter("@Phone", entity.Phone),
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", entity.DeliverTime),
                        SqlParameterFactory.GetSqlParameter("@Remark", entity.Remark),
                        SqlParameterFactory.GetSqlParameter("@MMCoEImage", entity.MMCoEImage),
                        SqlParameterFactory.GetSqlParameter("@RestaurantTel", entity.RestaurantTel),
                        SqlParameterFactory.GetSqlParameter("@RestaurantAddress", entity.RestaurantAddress),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderReason", entity.XmsOrderReason),
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID)
                    });
            }
        }
        #endregion

        #region 小秘书修改订单金额
        /// <summary>
        /// 小秘书修改订单金额
        /// </summary>
        /// <param name="xmsOrderId"></param>
        /// <param name="xmsTotalPrice"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int XmsChangeTotalFee(string xmsOrderId, decimal xmsTotalPrice, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                    $"UPDATE P_ORDER SET XmsTotalPrice=@XmsTotalPrice, ChangeTotalPriceReason=@ChangeTotalPriceReason,ActionState='0' WHERE XmsOrderId=@XmsOrderId ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@XmsTotalPrice", xmsTotalPrice),
                        SqlParameterFactory.GetSqlParameter("@ChangeTotalPriceReason", reason),
                        SqlParameterFactory.GetSqlParameter("@XmsOrderId", xmsOrderId)
                    });
            }
        }
        #endregion

        #region MMCoE审批
        /// <summary>
        /// MMCoE审批
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int MMCoEResult(Guid orderID, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var orderState = 0;
            if (state == 2)
            {
                orderState = OrderState.REJECT;
            }
            else
            {
                orderState = OrderState.SUBMITTED;
            }

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                        $"UPDATE [P_ORDER] SET [State]=@State, MMCoEApproveState=@MMCoEApproveState, MMCoEReason=@MMCoEReason WHERE ID=@ID AND MMCoEApproveState=@OldApproveState",
                        new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@State", orderState),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", state),
                        SqlParameterFactory.GetSqlParameter("@MMCoEReason", reason),
                        SqlParameterFactory.GetSqlParameter("@ID", orderID),
                        SqlParameterFactory.GetSqlParameter("@OldApproveState", MMCoEApproveState.WAITAPPROVE)
                        });
            }
        }
        #endregion

        #region MMCoE审批退回
        /// <summary>
        /// MMCoE审批退回
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public int _MMCoEResult(Guid orderID, int state, string reason)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var orderState = OrderState.WAITAPPROVE;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update(
                        $"UPDATE [P_ORDER] SET [State]=@State, MMCoEApproveState=@MMCoEApproveState, MMCoEReason=@MMCoEReason WHERE ID=@ID AND MMCoEApproveState=@OldApproveState",
                        new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@State", orderState),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", state),
                        SqlParameterFactory.GetSqlParameter("@MMCoEReason", reason),
                        SqlParameterFactory.GetSqlParameter("@ID", orderID),
                        SqlParameterFactory.GetSqlParameter("@OldApproveState", MMCoEApproveState.APPROVESUCCESS)
                        });
            }
        }
        #endregion

        #region 返回未评价订单数量
        /// <summary>
        /// 返回未评价订单数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int NotEvaluateCount(string userid, int isNonHT)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_COUNT>(
                    $"SELECT COUNT(0) AS 'Count' FROM P_ORDER WITH(NOLOCK) WHERE State IN (6,7) AND UserId=@UserId AND IsNonHT=@IsNonHT",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@UserId", userid),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                    }).Count;
            }
        }
        #endregion

        #region 后台订单服务
        #region 后台订单服务-list
        private const string _sqlOrderMntPageSelect = "SELECT P_ORDER.ID, DCUser.Name AS DCUserName, DCUser.PhoneNumber AS DCPhoneNum, DCUser1.Name AS DCUserName1, DCUser1.PhoneNumber AS DCPhoneNum1, "
                                                    + "P_ORDER.UserId AS MUDID, P_ORDER.Market, P_ORDER.CN,convert(nvarchar,cast(dbo.P_MEETING.BudgetTotal as money),1) as BudgetTotal, P_ORDER.EnterpriseOrderId AS XMSOrderID, P_ORDER.IsReturn AS TJIsReturn, P_ORDER.IsDelivery AS TJIsDelivery,  "
                                                    + "P_ORDER.CreateDate AS OrderingTime, P_ORDER.DeliverTime AS SendTime, P_ORDER.RestaurantId AS RestaurantId, P_ORDER.RestaurantName AS RestName, P_ORDER.AttendCount AS UserQuantity, P_ORDER.FoodCount AS MealQuantity, "
                                                    + "P_ORDER.TotalPrice, P_ORDER_XMS_REPORT.totalFee AS XMSTotalPrice, P_ORDER_XMS_REPORT.feeModifyReason AS ChangePriceReason, P_ORDER_XMS_REPORT.TYYYDRDC, "
                                                    + "P_ORDER.PO, P_ORDER.WBS, P_ORDER.IsNonHT, P_ORDER.TA, P_ORDER.MeetingCode AS MeetCode, P_ORDER.MeetingName AS MeetName, P_ORDER.ReceiveTime,"
                                                    + "P_ORDER_XMS_REPORT.TYDBDRDC, P_ORDER_XMS_REPORT.TYDBTYYYDRDC, P_ORDER_XMS_REPORT.CHRSDYLS, P_ORDER_XMS_REPORT.CHRSXYLSDDFSDYLS,P_ORDER_XMS_REPORT.TYCTDRDC ,P_ORDER_XMS_REPORT.TYDBTYCTDRDC ,P_ORDER_XMS_REPORT.TYDBTYYYTYCTDRDC, "
                                                    + "P_ORDER_XMS_REPORT.customerPickup AS CustomerPickup, P_ORDER.Province AS ProvinceName, P_ORDER.City AS CityName, P_ORDER.HospitalId AS GskHospital, P_ORDER.HospitalName AS HospitalName, "
                                                    + "P_ORDER.Address AS HospitalAddr, P_ORDER.DeliveryAddress AS HospitalRoom, P_ORDER.Consignee AS SCUserName, P_ORDER.Phone AS SCPhoneNum,P_ORDER.Remark as Remark, P_ORDER.State AS OrderState, "
                                                    + "P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.OnTimeDiscrpion AS EOnTimeDesc, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.SafeDiscrpion AS EIsSafeDesc, P_EVALUATE.Health AS EHealth, "
                                                    + "P_EVALUATE.HealthDiscrpion AS EHealthDesc, P_EVALUATE.Pack AS EPack, P_EVALUATE.PackDiscrpion AS EPackDesc, P_EVALUATE.CostEffective AS ECost, "
                                                    + "P_EVALUATE.CostEffectiveDiscrpion AS ECostDesc, P_EVALUATE.OtherDiscrpion AS EOtherDesc, ISNULL(P_EVALUATE.Star, 99) AS EStar, P_EVALUATE.CreateDate AS ECreateDate, "
                                                    + "P_ORDER.IsRetuen AS IsReturn, P_ORDER_XMS_REPORT.cancelFailReason AS XMSOrderCancelReason, P_ORDER.ReceiveState, P_ORDER_XMS_REPORT.cancelFeedback AS XMSCancelFeedback, "
                                                    + "P_ORDER_XMS_REPORT.cancelState AS XMSCancelState, P_ORDER_XMS_REPORT.bookState AS XMSBookState, P_ORDER.Channel AS Channel "
                                                    + "FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN "
                                                    + "WP_QYUSER AS DCUser ON P_ORDER.UserId = DCUser.UserId LEFT OUTER JOIN "
                                                    + "P_USERINFO AS DCUser1 ON P_ORDER.UserId = DCUser1.UserId LEFT OUTER JOIN "
                                                    + "P_ORDER_XMS_REPORT ON P_ORDER.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId LEFT OUTER JOIN "
                                                    + "P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID LEFT OUTER JOIN "
                                                    + "P_MEETING ON P_ORDER.CN = P_MEETING.Code "
                                                    + "WHERE (P_ORDER.CN LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@State IS NULL) OR (P_ORDER.State = @State)) "
                                                    + "AND ((@Supplier='') OR (P_ORDER.Channel = @Supplier))  "
                                                    + "AND ((@CreateTimeBegin IS NULL) OR (P_ORDER.CreateDate >= @CreateTimeBegin)) AND ((@CreateTimeEnd IS NULL) OR (P_ORDER.CreateDate < @CreateTimeEnd)) "
                                                    + "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) "
                                                    + "AND (P_ORDER.IsNonHT = @IsNonHT) ";

        private const string _sqlNonOrderMntPageSelect = "SELECT P_ORDER.ID, DCUser.Name AS DCUserName, DCUser.PhoneNumber AS DCPhoneNum, DCUser1.Name AS DCUserName1, DCUser1.PhoneNumber AS DCPhoneNum1, "
                                                   + "P_ORDER.UserId AS MUDID, P_ORDER.Market, P_ORDER.CN, P_ORDER.EnterpriseOrderId AS XMSOrderID,P_ORDER.TA, P_ORDER.MeetingCode AS MeetCode, P_ORDER.MeetingName AS MeetName, "
                                                   + "P_ORDER.CreateDate AS OrderingTime, P_ORDER.DeliverTime AS SendTime, P_ORDER.RestaurantId AS RestaurantId, P_ORDER.RestaurantName AS RestName, P_ORDER.AttendCount AS UserQuantity, P_ORDER.FoodCount AS MealQuantity, "
                                                   + "P_ORDER.TotalPrice, P_ORDER_XMS_REPORT.totalFee AS XMSTotalPrice, P_ORDER_XMS_REPORT.feeModifyReason AS ChangePriceReason, P_ORDER_XMS_REPORT.TYYYDRDC, "
                                                   + "P_ORDER.PO, P_ORDER.WBS, P_ORDER.IsNonHT, P_ORDER.ReceiveTime, "
                                                   + "P_ORDER_XMS_REPORT.TYDBDRDC, P_ORDER_XMS_REPORT.TYDBTYYYDRDC, P_ORDER_XMS_REPORT.CHRSDYLS, P_ORDER_XMS_REPORT.CHRSXYLSDDFSDYLS, "
                                                   + "P_ORDER_XMS_REPORT.customerPickup AS CustomerPickup, P_ORDER.Province AS ProvinceName, P_ORDER.City AS CityName, P_ORDER.HospitalId AS GskHospital, P_ORDER.HospitalName AS HospitalName, "
                                                   + "P_ORDER.Address AS HospitalAddr, P_ORDER.DeliveryAddress AS HospitalRoom, P_ORDER.Consignee AS SCUserName, P_ORDER.Phone AS SCPhoneNum,P_ORDER.Remark as Remark, P_ORDER.State AS OrderState, "
                                                   + "P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.OnTimeDiscrpion AS EOnTimeDesc, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.SafeDiscrpion AS EIsSafeDesc, P_EVALUATE.Health AS EHealth, "
                                                   + "P_EVALUATE.HealthDiscrpion AS EHealthDesc, P_EVALUATE.Pack AS EPack, P_EVALUATE.PackDiscrpion AS EPackDesc, P_EVALUATE.CostEffective AS ECost, "
                                                   + "P_EVALUATE.CostEffectiveDiscrpion AS ECostDesc, P_EVALUATE.OtherDiscrpion AS EOtherDesc, ISNULL(P_EVALUATE.Star, 99) AS EStar, P_EVALUATE.CreateDate AS ECreateDate, "
                                                   + "P_ORDER.IsRetuen AS IsReturn, P_ORDER_XMS_REPORT.cancelFailReason AS XMSOrderCancelReason, P_ORDER.ReceiveState, P_ORDER_XMS_REPORT.cancelFeedback AS XMSCancelFeedback, "
                                                   + "P_ORDER_XMS_REPORT.cancelState AS XMSCancelState, P_ORDER_XMS_REPORT.bookState AS XMSBookState, P_ORDER.Channel AS Channel "
                                                   + "FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN "
                                                   + "WP_QYUSER AS DCUser ON P_ORDER.UserId = DCUser.UserId LEFT OUTER JOIN "
                                                   + "P_USERINFO AS DCUser1 ON P_ORDER.UserId = DCUser1.UserId LEFT OUTER JOIN "
                                                   + "P_ORDER_XMS_REPORT ON P_ORDER.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId LEFT OUTER JOIN "
                                                   + "P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID "
                                                   + "WHERE (P_ORDER.PO=@CN) AND (P_ORDER.UserId=@MUDID)AND (P_ORDER.HospitalId=@HospitalCode or (P_ORDER.HospitalId=@OldHospitalCode and @OldHospitalCode<>'')) AND (P_ORDER.RestaurantId=@RestaurantId) AND ((@State IS NULL) OR (P_ORDER.State = @State)) "
                                                   + "AND ((@Supplier='') OR (P_ORDER.Channel = @Supplier))  "
                                                   + "AND ((@CreateTimeBegin IS NULL) OR (P_ORDER.CreateDate >= @CreateTimeBegin)) AND ((@CreateTimeEnd IS NULL) OR (P_ORDER.CreateDate < @CreateTimeEnd)) "
                                                   + "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) "
                                                   + "AND (P_ORDER.IsNonHT = @IsNonHT) ";

        private const string _sqlOrderMntPageOrderBy = "ORDER BY P_ORDER.DeliverTime DESC ";

        public List<P_ORDER_DAILY_VIEW> LoadOrderMntPage(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT, int rows, int page, out int total)
        {
            List<P_ORDER_DAILY_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            string sqlString = _sqlOrderMntPageSelect;
            if (isNonHT == 1)
            {
                sqlString = _sqlNonOrderMntPageSelect;
            }
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_ORDER_DAILY_VIEW>(rows, page, out total,
                   sqlString,
                    _sqlOrderMntPageOrderBy, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@State", srh_State),
                        SqlParameterFactory.GetSqlParameter("@Supplier", Supplier),
                        SqlParameterFactory.GetSqlParameter("@CreateTimeBegin", srh_CreateTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@CreateTimeEnd", srh_CreateTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                    });
            }

            return rtnData;
        }
        #endregion

        #region 后台订单服务-report
        public List<P_ORDER_DAILY_VIEW> LoadOrderMnt(string srh_CN, string srh_MUDID, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT)
        {
            List<P_ORDER_DAILY_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            string sqlString = _sqlOrderMntPageSelect;
            if (isNonHT == 1)
            {
                sqlString = _sqlNonOrderMntPageSelect;
            }
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_ORDER_DAILY_VIEW>(
                    sqlString + _sqlOrderMntPageOrderBy,
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@State", srh_State),
                        SqlParameterFactory.GetSqlParameter("@Supplier", Supplier),
                        SqlParameterFactory.GetSqlParameter("@CreateTimeBegin", srh_CreateTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@CreateTimeEnd", srh_CreateTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 后台订单审核-List
        private const string _sqlOrderApprovePageSelect = "SELECT P_ORDER.ID, P_ORDER.Market, P_ORDER.CN, P_ORDER.PO, WP_QYUSER.Name AS DCUserName, WP_QYUSER.PhoneNumber AS DCPhoneNum, P_ORDER.UserId AS MUDID,  "
                                                       + "P_ORDER.HospitalName AS HospitalName, P_ORDER.Consignee AS SCUserName, P_ORDER.Phone AS SCPhoneNum, P_ORDER.TotalPrice, P_ORDER.DeliverTime AS SendTime,  "
                                                       + "P_ORDER.State AS OrderState, P_ORDER.MMCoEApproveState "
                                                       + "FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN "
                                                       + "WP_QYUSER ON P_ORDER.UserId = WP_QYUSER.UserId "
                                                       + "WHERE (P_ORDER.CN LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND (P_ORDER.MMCoEApproveState = @MMCoEApproveState) AND (P_ORDER.IsNonHT = @IsNonHT)"
                                                       + "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) ";

        private const string _sqlNonOrderApprovePageSelect = "SELECT P_ORDER.ID, P_ORDER.Market, P_ORDER.CN, P_ORDER.PO, WP_QYUSER.Name AS DCUserName, WP_QYUSER.PhoneNumber AS DCPhoneNum, P_ORDER.UserId AS MUDID,  "
                                                       + "P_ORDER.HospitalName AS HospitalName, P_ORDER.Consignee AS SCUserName, P_ORDER.Phone AS SCPhoneNum, P_ORDER.TotalPrice, P_ORDER.DeliverTime AS SendTime,  "
                                                       + "P_ORDER.State AS OrderState, P_ORDER.MMCoEApproveState "
                                                       + "FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN "
                                                       + "WP_QYUSER ON P_ORDER.UserId = WP_QYUSER.UserId "
                                                       + "WHERE (P_ORDER.PO LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND (P_ORDER.MMCoEApproveState = @MMCoEApproveState) AND (P_ORDER.IsNonHT = @IsNonHT)"
                                                       + "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) ";


        private const string _sqlOrderApprovePageOrderBy = "ORDER BY P_ORDER.DeliverTime DESC ";
        private const string _sqlNonHTOrderApprovePageOrderBy = "ORDER BY P_ORDER.DeliverTime DESC ";

        public List<P_ORDER_APPROVE_VIEW> LoadOrderApprovePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int srh_MMCoEApproveState, int isNonHt, int rows, int page, out int total)
        {
            List<P_ORDER_APPROVE_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var sqlString = _sqlOrderApprovePageSelect;
            if (isNonHt == 1)
            {
                sqlString = _sqlNonOrderApprovePageSelect;
            }
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_ORDER_APPROVE_VIEW>(rows, page, out total,
                   sqlString,
                    _sqlOrderApprovePageOrderBy, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", srh_MMCoEApproveState),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHt)
                    });
            }

            return rtnData;
        }
        #endregion

        #region 后台订单评价-List
        private const string _sqlOrderEvaluateSltField = "SELECT P_ORDER.ID, P_ORDER.CN, P_ORDER.PO, P_ORDER.UserId AS MUDID, DCUser.Name AS DCUserName, P_ORDER.DeliverTime AS SendTime, P_ORDER.RestaurantName AS RestName, "
                                                        + "P_EVALUATE.Star AS EStar, P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.Health AS EHealth, P_EVALUATE.Pack AS EPack, "
                                                        + "P_EVALUATE.CostEffective AS ECost, P_ORDER.RestaurantId AS ResCode, P_ORDER.ReceiveState AS State,P_EVALUATE.OnTimeDiscrpion AS EOnTimeRemark, P_EVALUATE.CreateDate AS AppDate, "
                                                        + "P_EVALUATE.SafeDiscrpion AS EIsSafeRemark, P_EVALUATE.HealthDiscrpion AS EHealthRemark, P_EVALUATE.PackDiscrpion AS EPackRemark, P_EVALUATE.CostEffectiveDiscrpion AS ECostRemark, P_EVALUATE.OtherDiscrpion AS EOtherRemark, P_ORDER.Channel as Channel "
                                                        + "FROM P_ORDER INNER JOIN P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID LEFT OUTER JOIN "
                                                        + "WP_QYUSER AS DCUser ON P_ORDER.UserId = DCUser.UserId ";
        private const string _sqlOrderEvaluateSltCondition1 = "WHERE (P_ORDER.CN LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND  "
                                                            + "((@Star=0) OR (P_EVALUATE.Star=@Star)) AND ((@Channel='') OR (P_ORDER.Channel=@Channel)) AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) ";
        private const string _sqlOrderEvaluateSltCondition2 = "WHERE (P_ORDER.CN LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND "
                                                            + "((@Star=0) OR (P_EVALUATE.Star=@Star)) AND ((@Channel='') OR (P_ORDER.Channel=@Channel)) AND "
                                                            + "((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) AND ((@L60 IS NOT NULL AND P_EVALUATE.OnTime = @L60) OR (@UsSafe IS NOT NULL AND P_EVALUATE.IsSafe = @UsSafe) OR (@UnSend IS NOT NULL AND P_EVALUATE.OnTime = @UnSend)) ";
        private const string _sqlNonOrderEvaluateSltField = "SELECT P_ORDER.ID, P_ORDER.CN, P_ORDER.PO, P_ORDER.UserId AS MUDID, DCUser.Name AS DCUserName, P_ORDER.DeliverTime AS SendTime, P_ORDER.RestaurantName AS RestName, "
                                                        + "P_EVALUATE.Star AS EStar, P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.Health AS EHealth, P_EVALUATE.Pack AS EPack, "
                                                        + "P_EVALUATE.CostEffective AS ECost, P_ORDER.Channel as Channel "
                                                        + "FROM P_ORDER INNER JOIN P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID LEFT OUTER JOIN "
                                                        + "WP_QYUSER AS DCUser ON P_ORDER.UserId = DCUser.UserId ";
        private const string _sqlNonOrderEvaluateSltCondition1 = "WHERE (P_ORDER.PO LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND P_ORDER.IsNonHT=@IsNonHT AND "
                                                           + "((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) ";
        private const string _sqlNonOrderEvaluateSltCondition2 = "WHERE (P_ORDER.PO LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND P_ORDER.IsNonHT=@IsNonHT AND "
                                                            + "((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) AND ((@L60 IS NOT NULL AND P_EVALUATE.OnTime = @L60) OR (@UsSafe IS NOT NULL AND P_EVALUATE.IsSafe = @UsSafe) OR (@UnSend IS NOT NULL AND P_EVALUATE.OnTime = @UnSend)) ";

        private const string _sqlOrderEvaluateSltOrderBy = "ORDER BY P_ORDER.DeliverTime DESC ";
        public List<P_ORDER_EVALUATE_VIEW> LoadOrderEvaluatePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHT, int star, string channel, int rows, int page, out int total)
        {
            List<P_ORDER_EVALUATE_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            if (srh_Large60 == false && srh_UnSafe == false && srh_UnSend == false)
            {
                var sqlString = _sqlOrderEvaluateSltField;
                var sqlString1 = _sqlOrderEvaluateSltCondition1;
                if (isNonHT == 1)
                {
                    sqlString = _sqlNonOrderEvaluateSltField;
                    sqlString1 = _sqlNonOrderEvaluateSltCondition1;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.LoadPages<P_ORDER_EVALUATE_VIEW>(rows, page, out total,
                       sqlString + sqlString1,
                        _sqlOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@Star", star),
                        SqlParameterFactory.GetSqlParameter("@Channel", channel)

                        });
                }
            }
            else
            {
                int? large60, unSafe, unSend;
                if (srh_Large60 == true)
                {
                    large60 = 5;
                }
                else
                {
                    large60 = null;
                }
                if (srh_UnSafe == true)
                {
                    unSafe = 1;
                }
                else
                {
                    unSafe = null;
                }
                if (srh_UnSend == true)
                {
                    unSend = 0;
                }
                else
                {
                    unSend = null;
                }
                var sqlString = _sqlOrderEvaluateSltField;
                var sqlString1 = _sqlOrderEvaluateSltCondition2;
                if (isNonHT == 1)
                {
                    sqlString = _sqlNonOrderEvaluateSltField;
                    sqlString1 = _sqlNonOrderEvaluateSltCondition2;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.LoadPages<P_ORDER_EVALUATE_VIEW>(rows, page, out total,
                       sqlString + sqlString1,
                        _sqlOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@L60", large60),
                        SqlParameterFactory.GetSqlParameter("@UsSafe", unSafe),
                        SqlParameterFactory.GetSqlParameter("@UnSend", unSend),
                        SqlParameterFactory.GetSqlParameter("@Star", star),
                        SqlParameterFactory.GetSqlParameter("@Channel", channel)
                        });
                }
            }

            return rtnData;
        }
        public List<P_ORDER_EVALUATE_VIEW> LoadOrderEvaluate(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHT, int star, string channel)
        {
            List<P_ORDER_EVALUATE_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            if (srh_Large60 == false && srh_UnSafe == false && srh_UnSend == false)
            {
                var sqlString = _sqlOrderEvaluateSltField;
                var sqlString1 = _sqlOrderEvaluateSltCondition1;
                if (isNonHT == 1)
                {
                    sqlString = _sqlNonOrderEvaluateSltField;
                    sqlString1 = _sqlNonOrderEvaluateSltCondition1;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_EVALUATE_VIEW>(sqlString + sqlString1 + _sqlOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@Star", star),
                        SqlParameterFactory.GetSqlParameter("@Channel", channel)
                        });
                }
            }
            else
            {
                int? large60, unSafe, unSend;
                if (srh_Large60 == true)
                {
                    large60 = 5;
                }
                else
                {
                    large60 = null;
                }
                if (srh_UnSafe == true)
                {
                    unSafe = 1;
                }
                else
                {
                    unSafe = null;
                }
                if (srh_UnSend == true)
                {
                    unSend = 0;
                }
                else
                {
                    unSend = null;
                }
                var sqlString = _sqlOrderEvaluateSltField;
                var sqlString1 = _sqlOrderEvaluateSltCondition2;
                if (isNonHT == 1)
                {
                    sqlString = _sqlNonOrderEvaluateSltField;
                    sqlString1 = _sqlNonOrderEvaluateSltCondition2;
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_EVALUATE_VIEW>(sqlString + sqlString1 + _sqlOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@L60", large60),
                        SqlParameterFactory.GetSqlParameter("@UsSafe", unSafe),
                        SqlParameterFactory.GetSqlParameter("@UnSend", unSend),
                        SqlParameterFactory.GetSqlParameter("@Star", star),
                        SqlParameterFactory.GetSqlParameter("@Channel", channel)
                        });
                }
            }

            return rtnData;
        }
        #endregion
        #endregion

        #region 后台订单审核 根据订单ID 获取订单信息
        private const string _sqlFindOrderById = "SELECT ID, ChangeID, UserId, Market, HospitalId, Address, CN, PO, RestaurantId, RestaurantName, RestaurantLogo, TotalPrice, DeliveryGeo, Detail, ChangeDetail, FoodCount, AttendCount, DeliveryAddress, "
                                                + "Consignee, Phone, DeliverTime, Remark, MMCoEImage, State, ReceiveCode, CreateDate, XmsOrderId, OldXmlOrderId, SendOrderDate, ChangeOrderDate, ReturnOrderDate, XmsTotalPrice, "
                                                + "ChangeTotalPriceReason, ReceiveDate, IsRetuen, IsChange, XmsOrderReason, IsOuterMeeting, RestaurantTel, RestaurantAddress, ApproveReason, MMCoEApproveState, MMCoEReason "
                                                + "FROM P_ORDER WITH(NOLOCK)"
                                                + "WHERE (ID = @OrderID)";
        public P_ORDER GetOrderInfo(Guid OrderID)
        {
            P_ORDER rtnData;

            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Find<P_ORDER>(_sqlFindOrderById, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@OrderID", OrderID)
                    });
            }
            return rtnData;
        }
        #endregion

        #region 同步周报日报
        /// <summary>
        /// 同步周报日报
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public int SyncReport(List<P_ORDER_XMS_REPORT> list)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                string sqlDelReport = $"DELETE P_ORDER_XMS_REPORT WHERE XmsOrderId IN ('{string.Join("','", list.Select(a => a.XmsOrderId).ToArray())}') ";
                SqlCommand comDelReport = new SqlCommand(sqlDelReport, conn);
                comDelReport.Transaction = tran;
                comDelReport.ExecuteNonQuery();


                foreach (var report in list)
                {
                    string sqlReport = "INSERT INTO [P_ORDER_XMS_REPORT] ([XmsOrderId] ,[totalFee] ,[customerPickup] ,[cancelFeedback] ,[cancelFailReason] ,[feeModifyReason] ,[bookState] ,[cancelState] ,[TYYYDRDC] ,[TYDBDRDC] ,[TYDBTYYYDRDC] ,[CHRSDYLS] ,[CHRSXYLSDDFSDYLS],[TYCTDRDC],[TYDBTYCTDRDC],[TYDBTYYYTYCTDRDC]) "
                        + " VALUES (@XmsOrderId, @totalFee, @customerPickup, @cancelFeedback, @cancelFailReason, @feeModifyReason, @bookState, @cancelState, @TYYYDRDC, @TYDBDRDC, @TYDBTYYYDRDC, @CHRSDYLS, @CHRSXYLSDDFSDYLS,@TYCTDRDC,@TYDBTYCTDRDC,@TYDBTYYYTYCTDRDC)";
                    SqlCommand commandReport = new SqlCommand(sqlReport, conn);
                    commandReport.Transaction = tran;
                    commandReport.Parameters.AddRange(new SqlParameter[]
                        {
                            SqlParameterFactory.GetSqlParameter("@XmsOrderId", report.XmsOrderId),
                            SqlParameterFactory.GetSqlParameter("@totalFee", report.totalFee),
                            SqlParameterFactory.GetSqlParameter("@customerPickup", report.customerPickup),
                            SqlParameterFactory.GetSqlParameter("@cancelFeedback", report.cancelFeedback),
                            SqlParameterFactory.GetSqlParameter("@cancelFailReason", report.cancelFailReason),
                            SqlParameterFactory.GetSqlParameter("@feeModifyReason", report.feeModifyReason),
                            SqlParameterFactory.GetSqlParameter("@bookState", report.bookState),
                            SqlParameterFactory.GetSqlParameter("@cancelState", report.cancelState),
                            SqlParameterFactory.GetSqlParameter("@TYYYDRDC", report.TYYYDRDC),
                            SqlParameterFactory.GetSqlParameter("@TYDBDRDC", report.TYDBDRDC),
                            SqlParameterFactory.GetSqlParameter("@TYDBTYYYDRDC", report.TYDBTYYYDRDC),
                            SqlParameterFactory.GetSqlParameter("@CHRSDYLS", report.CHRSDYLS),
                            SqlParameterFactory.GetSqlParameter("@CHRSXYLSDDFSDYLS", report.CHRSXYLSDDFSDYLS),
                            SqlParameterFactory.GetSqlParameter("@TYCTDRDC", report.TYCTDRDC),
                            SqlParameterFactory.GetSqlParameter("@TYDBTYCTDRDC", report.TYDBTYCTDRDC),
                            SqlParameterFactory.GetSqlParameter("@TYDBTYYYTYCTDRDC", report.TYDBTYYYTYCTDRDC)
                        });
                    commandReport.ExecuteNonQuery();
                }
                tran.Commit();
            }
            return 1;
        }
        #endregion

        #region 载入简报
        /// <summary>
        /// 载入简报
        /// </summary>
        /// <returns></returns>
        public P_LOADORDER_BRIEF LoadBriefing(int isNonHT)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_LOADORDER_BRIEF>("EXEC P_LOADORDER_BRIEF @Today, @IsNonHT ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@Today", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                    });
                return (list == null || list.Count < 1) ? new P_LOADORDER_BRIEF() : list[0];
            }
        }
        #endregion

        #region PO相关
        public int SavePO(P_PO po)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery($"INSERT INTO [P_PO] ([ID],[PO],[IsUsed]) VALUES (@ID,@PO,@IsUsed)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", po.ID),
                        SqlParameterFactory.GetSqlParameter("@PO", po.PO),
                        SqlParameterFactory.GetSqlParameter("@IsUsed", po.IsUsed)
                    });
            }
        }

        public P_PO FindByPO(string po)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Find<P_PO>($"SELECT * FROM [P_PO] WHERE PO=@PO",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PO", po)
                    });
            }
        }

        public int EditPO(string po, int isUsed)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.Update($"UPDATE [P_PO] SET IsUsed=@IsUsed WHERE PO=@PO",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@PO", po),
                        SqlParameterFactory.GetSqlParameter("@IsUsed", isUsed)
                    });
            }
        }
        #endregion

        #region 未送达写入用户确认金额
        /// <summary>
        /// 未送达写入用户确认金额
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="totalPrice"></param>
        /// <returns></returns>
        public int unUserPrice(Guid orderID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                return sqlServerTemplate.Update($"UPDATE [P_ORDER] SET RealPrice=0, XmsTotalPrice=0 WHERE ID=@ID",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ID",orderID)
                    });
            }
        }
        #endregion 

        #region 获取当日有效订单
        /// <summary>
        /// 获取当日有效订单
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadOrdersNow(DateTime begin, DateTime end)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE [DeliverTime] > @begin AND [DeliverTime] < @end AND State NOT IN (5,11) ORDER BY CreateDate DESC ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@begin", begin),
                        SqlParameterFactory.GetSqlParameter("@end", end)
                    });
                return list;
            }

        }
        #endregion

        #region 获取当日所有订单
        /// <summary>
        /// 获取当日所有订单
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadAllOrdersNow(DateTime begin, DateTime end)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE [DeliverTime] > @begin AND [DeliverTime] < @end ORDER BY CreateDate DESC ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@begin", begin),
                        SqlParameterFactory.GetSqlParameter("@end", end)
                    });
                return list;
            }

        }
        #endregion

        #region 获取1.0当日有效订单
        /// <summary>
        /// 获取1.0当日有效订单
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<P_OTHERORDER> LoadOtherOrdersNow(DateTime begin, DateTime end)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_OTHERORDER>($"SELECT * FROM [P_ORDER] WHERE [DeliverTime] > @begin AND [DeliverTime] < @end AND State NOT IN (5,11) AND IsNonHT=0 ORDER BY CreateDate DESC ", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@begin", begin),
                        SqlParameterFactory.GetSqlParameter("@end", end)
                    });
                return list;
            }

        }
        #endregion

        public int SaveChange(string HTCode, string SpecialOrderReason, string SpecialRemarksProjectTeam, string SpecialOrderOperatorName, string SpecialOrderOperatorMUDID, int IsSpecialOrder, int State)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_ORDER SET SpecialOrderReason=@SpecialOrderReason,SpecialRemarksProjectTeam=@SpecialRemarksProjectTeam, SpecialOrderOperatorName=@SpecialOrderOperatorName,SpecialOrderOperatorMUDID=@SpecialOrderOperatorMUDID,SpecialOrderOperateDate=@SpecialOrderOperateDate,IsSpecialOrder=@IsSpecialOrder,ActionState='0' " +
                    "WHERE CN = @HTCode AND State=@State ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode",HTCode ),
                        SqlParameterFactory.GetSqlParameter("@SpecialOrderReason",SpecialOrderReason),
                        SqlParameterFactory.GetSqlParameter("@SpecialRemarksProjectTeam", SpecialRemarksProjectTeam),
                        SqlParameterFactory.GetSqlParameter("@SpecialOrderOperatorName", SpecialOrderOperatorName),
                        SqlParameterFactory.GetSqlParameter("@SpecialOrderOperatorMUDID",SpecialOrderOperatorMUDID),
                        SqlParameterFactory.GetSqlParameter("@SpecialOrderOperateDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@State", State),
                        SqlParameterFactory.GetSqlParameter("@IsSpecialOrder",IsSpecialOrder)
                    });
            }
        }


        #region 订单查询
        private const string _sqlOrderPageSelect1 = @"SELECT P_PreApproval.ApplierName, P_PreApproval.ApplierMUDID, WP_QYUSER.Position, P_PreApproval.ApplierMobile, P_PreApproval.CreateDate AS PRECreateDate, P_PreApproval.CurrentApproverName AS CurrentApproverName,  P_PreApproval.CurrentApproverMUDID AS CurrentApproverMUDID, 
                                                    P_PreApproval.ModifyDate AS PREModifyDate, P_PreApproval.HTCode, P_PreApproval.Market,P_PreApproval.VeevaMeetingID, P_PreApproval.TA, P_PreApproval.Province,
                                                     P_PreApproval.City, P_PreApproval.HospitalCode, P_PreApproval.HospitalName, P_PreApproval.HospitalAddress, P_PreApproval.MeetingName, P_PreApproval.MeetingDate, 
                                                     P_PreApproval.AttendCount AS PREAttendCount, P_PreApproval.CostCenter, P_PreApproval.BudgetTotal, P_PreApproval.IsDMFollow, P_PreApproval.IsFreeSpeaker,
                                                     P_PreApproval.RDSDName,P_PreApproval.RDSDMUDID, P_PreApproval.BUHeadName AS PREBUHeadName, P_PreApproval.BUHeadMUDID AS PREBUHeadMUDID,
                                                     P_PreApproval.BUHeadApproveDate AS PREBUHeadApproveDate, P_PreApproval.State AS PREState, P_PreApproval.IsReAssign AS PREIsReAssign, P_PreApproval.ReAssignOperatorName AS PREReAssignOperatorName, 
                                                     P_PreApproval.ReAssignOperatorMUDID AS PREReAssignOperatorMUDID, P_PreApproval.ReAssignBUHeadName AS PREReAssignBUHeadName, P_PreApproval.ReAssignBUHeadMUDID AS PREReAssignBUHeadMUDID, 
                                                     P_PreApproval.ReAssignBUHeadApproveDate AS PREReAssignBUHeadApproveDate, P_ORDER.Channel, P_ORDER.EnterpriseOrderId, P_ORDER.CreateDate AS ORDCreateDate, P_ORDER.DeliverTime,
                                                      P_ORDER.RestaurantId, P_ORDER.RestaurantName, P_ORDER.AttendCount AS ORDAttendCount, P_ORDER.FoodCount, P_ORDER.TotalPrice, 
                                                      CASE WHEN P_ORDER.XmsTotalPrice< 0 THEN P_ORDER.TotalPrice ELSE P_ORDER.XmsTotalPrice END AS totalFee, P_ORDER_XMS_REPORT.feeModifyReason,
                                                      P_ORDER.DeliveryAddress, P_ORDER.Consignee, P_ORDER.Phone, P_ORDER.Remark,case when P_ORDER.State in(3,5) then N'否' else N'是'  end as bookState,
                                                       P_ORDER.ReceiveTime, P_ORDER.IsRetuen,case when P_ORDER.IsRetuen=0 then null when P_ORDER.State= 5 then null WHEN P_ORDER.IsReturn= 1 and P_ORDER.IsRetuen= 2 then N'是' else N'否' end as cancelState, P_ORDER_XMS_REPORT.cancelFeedback, P_ORDER_XMS_REPORT.cancelFailReason,
                                                       P_ORDER.ReceiveState, P_ORDER.ReceiveDate, P_ORDER.State AS ORDState, P_ORDER.RealPrice, P_ORDER.RealPriceChangeReason, P_ORDER.RealPriceChangeRemark,
                                                       P_ORDER.RealCount, P_ORDER.RealCountChangeReason, P_ORDER.RealCountChangeRemrak, P_ORDER.IsOrderUpload, P_EVALUATE.OnTime AS EOnTime,
                                                       P_EVALUATE.OnTimeDiscrpion AS EOnTimeDesc, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.SafeDiscrpion AS EIsSafeDesc, P_EVALUATE.Health AS EHealth,
                                                        P_EVALUATE.HealthDiscrpion AS EHealthDesc, P_EVALUATE.Pack AS EPack, P_EVALUATE.PackDiscrpion AS EPackDesc, P_EVALUATE.CostEffective AS ECost,
                                                         P_EVALUATE.CostEffectiveDiscrpion AS ECostDesc, P_EVALUATE.OtherDiscrpion AS EOtherDesc, ISNULL(P_EVALUATE.Star, 99) AS EStar, P_EVALUATE.CreateDate AS ECreateDate, 
	                                                     P_ORDER_XMS_REPORT.TYYYDRDC, P_ORDER_XMS_REPORT.TYDBDRDC, P_ORDER_XMS_REPORT.TYCTDRDC, P_ORDER_XMS_REPORT.TYDBTYYYDRDC, P_ORDER_XMS_REPORT.TYDBTYCTDRDC, 
	                                                     P_ORDER_XMS_REPORT.TYDBTYYYTYCTDRDC, P_ORDER_XMS_REPORT.CHRSDYLS, P_ORDER_XMS_REPORT.CHRSXYLSDDFSDYLS, P_ORDER_XMS_REPORT.customerPickup, 
	                                                     P_PreUploadOrder.CreateDate AS PUOCreateDate, P_PreUploadOrder.BUHeadName AS PUOBUHeadName, P_PreUploadOrder.BUHeadMUDID AS PUOBUHeadMUDID, 
	                                                     P_PreUploadOrder.BUHeadApproveDate AS ApproveDate, P_PreUploadOrder.State AS PUOState, P_PreUploadOrder.IsAttentSame, P_PreUploadOrder.AttentSameReason, 
	                                                     P_PreUploadOrder.IsReopen, P_PreUploadOrder.ReopenOperatorName, P_PreUploadOrder.ReopenOperatorMUDID, P_PreUploadOrder.ReopenOperateDate, P_PreUploadOrder.ReopenReason, 
	                                                     P_ORDER.IsTransfer, P_ORDER.TransferOperatorName, P_ORDER.TransferOperatorMUDID, P_ORDER.TransferUserName, P_ORDER.TransferUserMUDID, P_ORDER.TransferOperateDate, 
	                                                     P_PreUploadOrder.IsReAssign, P_PreUploadOrder.ReAssignOperatorName, P_PreUploadOrder.ReAssignOperatorMUDID, P_PreUploadOrder.ReAssignBUHeadName, 
	                                                     P_PreUploadOrder.ReAssignBUHeadMUDID, P_PreUploadOrder.ReAssignBUHeadApproveDate, P_ORDER.SpecialOrderReason as SpecialReason, P_ORDER.IsMealSame,P_PreUploadOrder.IsMeetingInfoSame,
                                                     P_PreUploadOrder.MeetingInfoSameReason,P_PreApproval.MRTerritoryCode,P_PreApproval.RDTerritoryCode, P_ORDER.SupplierSpecialRemark,P_ORDER.IsCompleteDelivery,P_ORDER.SupplierConfirmAmount,
                                                     CASE WHEN P_ORDER.GSKConfirmAmount is not null THEN P_ORDER.GSKConfirmAmount
                                                     WHEN P_ORDER.GSKConfirmAmount is null and P_ORDER.XmsTotalPrice< 0 THEN P_ORDER.TotalPrice  
                                                     WHEN P_ORDER.GSKConfirmAmount is null and P_ORDER.XmsTotalPrice>=0 THEN P_ORDER.XmsTotalPrice END AS GSKConfirmAmount,
                                                     P_ORDER.GSKConAAReason,P_ORDER.MealPaymentAmount,P_ORDER.MealPaymentPO,P_ORDER.AccountingTime  
                                                     FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN P_PreApproval ON P_ORDER.CN = P_PreApproval.HTCode
                                                     LEFT OUTER JOIN P_ORDER_XMS_REPORT ON P_ORDER.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId LEFT OUTER JOIN P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID LEFT OUTER JOIN
                                                      P_PreUploadOrder ON P_ORDER.CN = P_PreUploadOrder.HTCode LEFT OUTER JOIN WP_QYUSER ON P_ORDER.UserId = WP_QYUSER.UserId  
                                                      WHERE 1=1 ";
        private const string _sqlOrderPageSelect = "SELECT P_PreApproval.ApplierName, P_PreApproval.ApplierMUDID,WP_QYUSER.Position, P_PreApproval.ApplierMobile, P_PreApproval.CreateDate AS PRECreateDate, P_PreApproval.ModifyDate AS PREModifyDate, "
                                                    + "P_PreApproval.HTCode, P_PreApproval.Market,P_PreApproval.VeevaMeetingID, P_PreApproval.TA, P_PreApproval.Province, P_PreApproval.City, P_PreApproval.HospitalCode, P_PreApproval.HospitalName, P_PreApproval.HospitalAddress, "
                                                    + "P_PreApproval.MeetingName, P_PreApproval.MeetingDate, P_PreApproval.AttendCount AS PREAttendCount, P_PreApproval.CostCenter, P_PreApproval.BudgetTotal, P_PreApproval.IsDMFollow, "
                                                    + "P_PreApproval.IsFreeSpeaker,P_PreApproval.RDSDName,P_PreApproval.RDSDMUDID, db_owner.GetDMName(P_PreApproval.ApplierMUDID) AS PREBUHeadName, db_owner.GetDMUserID(P_PreApproval.ApplierMUDID) AS PREBUHeadMUDID, P_PreApproval.BUHeadApproveDate AS PREBUHeadApproveDate, "
                                                    + "P_PreApproval.State AS PREState, P_PreApproval.IsReAssign AS PREIsReAssign, P_PreApproval.ReAssignOperatorName AS PREReAssignOperatorName, P_PreApproval.ReAssignOperatorMUDID AS PREReAssignOperatorMUDID, "
                                                    + "P_PreApproval.ReAssignBUHeadName AS PREReAssignBUHeadName, P_PreApproval.ReAssignBUHeadMUDID AS PREReAssignBUHeadMUDID, P_PreApproval.ReAssignBUHeadApproveDate AS PREReAssignBUHeadApproveDate, "
                                                    + "P_ORDER.Channel, P_ORDER.EnterpriseOrderId, P_ORDER.CreateDate AS ORDCreateDate, P_ORDER.DeliverTime, "
                                                    + "P_ORDER.RestaurantId, P_ORDER.RestaurantName, P_ORDER.AttendCount AS ORDAttendCount, P_ORDER.FoodCount, P_ORDER.TotalPrice, "
                                                    + "CASE WHEN P_ORDER.XmsTotalPrice < 0 THEN P_ORDER.TotalPrice ELSE  P_ORDER.XmsTotalPrice END AS totalFee, P_ORDER_XMS_REPORT.feeModifyReason, P_ORDER.DeliveryAddress, P_ORDER.Consignee, P_ORDER.Phone, "
                                                    + "P_ORDER.Remark,case when P_ORDER.State in(3,5) then N'否' else N'是'  end as bookState, P_ORDER.ReceiveTime, P_ORDER.IsRetuen,case when P_ORDER.IsRetuen=0 then null when P_ORDER.State=5 then null when P_ORDER.IsReturn=1 and P_ORDER.IsRetuen=2 then N'是' else N'否' end as cancelState, P_ORDER_XMS_REPORT.cancelFeedback, P_ORDER_XMS_REPORT.cancelFailReason,"
                                                    + "P_ORDER.ReceiveState, P_ORDER.ReceiveDate, P_ORDER.State AS ORDState, P_ORDER.RealPrice, P_ORDER.RealPriceChangeReason, P_ORDER.RealPriceChangeRemark, "
                                                    + "P_ORDER.RealCount, P_ORDER.RealCountChangeReason, P_ORDER.RealCountChangeRemrak, P_ORDER.IsOrderUpload, "
                                                    + "P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.OnTimeDiscrpion AS EOnTimeDesc, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.SafeDiscrpion AS EIsSafeDesc, P_EVALUATE.Health AS EHealth, "
                                                    + "P_EVALUATE.HealthDiscrpion AS EHealthDesc, P_EVALUATE.Pack AS EPack, P_EVALUATE.PackDiscrpion AS EPackDesc, P_EVALUATE.CostEffective AS ECost, "
                                                    + "P_EVALUATE.CostEffectiveDiscrpion AS ECostDesc, P_EVALUATE.OtherDiscrpion AS EOtherDesc, ISNULL(P_EVALUATE.Star, 99) AS EStar, P_EVALUATE.CreateDate AS ECreateDate, "
                                                    + "P_ORDER_XMS_REPORT.TYYYDRDC, P_ORDER_XMS_REPORT.TYDBDRDC, P_ORDER_XMS_REPORT.TYCTDRDC, P_ORDER_XMS_REPORT.TYDBTYYYDRDC, "
                                                    + "P_ORDER_XMS_REPORT.TYDBTYCTDRDC, P_ORDER_XMS_REPORT.TYDBTYYYTYCTDRDC, P_ORDER_XMS_REPORT.CHRSDYLS, P_ORDER_XMS_REPORT.CHRSXYLSDDFSDYLS, P_ORDER_XMS_REPORT.customerPickup, "
                                                    + "P_PreUploadOrder.CreateDate AS PUOCreateDate, P_PreUploadOrder.BUHeadName AS PUOBUHeadName, P_PreUploadOrder.BUHeadMUDID AS PUOBUHeadMUDID, P_PreUploadOrder.BUHeadApproveDate AS ApproveDate, "
                                                    + "P_PreUploadOrder.State AS PUOState, P_PreUploadOrder.IsAttentSame, P_PreUploadOrder.AttentSameReason, P_PreUploadOrder.IsReopen, P_PreUploadOrder.ReopenOperatorName, P_PreUploadOrder.ReopenOperatorMUDID, P_PreUploadOrder.ReopenOperateDate, P_PreUploadOrder.ReopenReason, "
                                                    + "P_ORDER.IsTransfer, P_ORDER.TransferOperatorName, P_ORDER.TransferOperatorMUDID, P_ORDER.TransferUserName, P_ORDER.TransferUserMUDID, P_ORDER.TransferOperateDate, "
                                                    + "P_PreUploadOrder.IsReAssign, P_PreUploadOrder.ReAssignOperatorName, P_PreUploadOrder.ReAssignOperatorMUDID, P_PreUploadOrder.ReAssignBUHeadName, P_PreUploadOrder.ReAssignBUHeadMUDID, P_PreUploadOrder.ReAssignBUHeadApproveDate, P_ORDER.SpecialOrderReason as SpecialReason , "
                                                    + "db_owner.GetDMName(db_owner.GetDMUserID(P_PreApproval.ApplierMUDID)) AS Level2Name, db_owner.GetDMUserID(db_owner.GetDMUserID(P_PreApproval.ApplierMUDID)) AS Level2UserId, "
                                                    + "db_owner.GetDMName(db_owner.GetDMUserID(db_owner.GetDMUserID(P_PreApproval.ApplierMUDID))) AS Level3Name, db_owner.GetDMUserID(db_owner.GetDMUserID(db_owner.GetDMUserID(P_PreApproval.ApplierMUDID))) AS Level3UserId,P_ORDER.IsMealSame,P_PreUploadOrder.IsMeetingInfoSame,P_PreUploadOrder.MeetingInfoSameReason "
                                                    + "FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN "
                                                    + "P_PreApproval ON P_ORDER.CN = P_PreApproval.HTCode LEFT OUTER JOIN "
                                                    + "P_ORDER_XMS_REPORT ON P_ORDER.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId LEFT OUTER JOIN "
                                                    + "P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID LEFT OUTER JOIN "
                                                    + "P_PreUploadOrder ON P_ORDER.CN = P_PreUploadOrder.HTCode LEFT OUTER JOIN "
                                                    + "WP_QYUSER ON P_ORDER.UserId = WP_QYUSER.UserId "
                                                    + "WHERE substring(P_ORDER.CN,1,1)='H'AND ((P_PreApproval.HospitalCode=@HospitalCode or (P_PreApproval.HospitalCode=@OldHospitalCode and @OldHospitalCode<>'')) or @HospitalCode='') AND (P_ORDER.RestaurantId =@RestaurantId or @RestaurantId='') AND (P_PreApproval.CostCenter like '%'+@CostCenter+'%' or P_PreApproval.CostCenter like '%'+@OldCostCenter+'%') AND (P_ORDER.CN=@CN or @CN='') AND (P_ORDER.UserId=@MUDID or @MUDID='') AND ((@State IS NULL) OR (P_ORDER.State = @State)) "
                                                    + "AND ((@Supplier='') OR (P_ORDER.Channel = @Supplier))  "
                                                    + "AND ((@CreateTimeBegin IS NULL) OR (P_ORDER.CreateDate >= @CreateTimeBegin)) AND ((@CreateTimeEnd IS NULL) OR (P_ORDER.CreateDate < @CreateTimeEnd)) "
                                                    + "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) ";
        private const string _sqlNonOrderSb = "SELECT P_ORDER.ID, DCUser.Name AS DCUserName, DCUser.PhoneNumber AS DCPhoneNum, DCUser1.Name AS DCUserName1, DCUser1.PhoneNumber AS DCPhoneNum1, "
                                               + "P_ORDER.UserId AS MUDID, P_ORDER.Market, P_ORDER.CN, P_ORDER.EnterpriseOrderId AS XMSOrderID,P_ORDER.TA, P_ORDER.MeetingCode AS MeetCode, P_ORDER.MeetingName AS MeetName, "
                                               + "P_ORDER.CreateDate AS OrderingTime, P_ORDER.DeliverTime AS SendTime, P_ORDER.RestaurantId AS RestaurantId, P_ORDER.RestaurantName AS RestName, P_ORDER.AttendCount AS UserQuantity, P_ORDER.FoodCount AS MealQuantity, "
                                               + "P_ORDER.TotalPrice,  P_ORDER.XmsTotalPrice AS XMSTotalPrice, P_ORDER_XMS_REPORT.feeModifyReason AS ChangePriceReason, P_ORDER_XMS_REPORT.TYYYDRDC, "
                                               + "P_ORDER.PO, P_ORDER.WBS, P_ORDER.IsNonHT, P_ORDER.ReceiveTime, "
                                               + "P_ORDER_XMS_REPORT.TYDBDRDC, P_ORDER_XMS_REPORT.TYDBTYYYDRDC, P_ORDER_XMS_REPORT.CHRSDYLS, P_ORDER_XMS_REPORT.CHRSXYLSDDFSDYLS, "
                                               + "P_ORDER_XMS_REPORT.customerPickup AS CustomerPickup, P_ORDER.Province AS ProvinceName, P_ORDER.City AS CityName, P_ORDER.HospitalId AS GskHospital, P_ORDER.HospitalName AS HospitalName, "
                                               + "P_ORDER.Address AS HospitalAddr, P_ORDER.DeliveryAddress AS HospitalRoom, P_ORDER.Consignee AS SCUserName, P_ORDER.Phone AS SCPhoneNum,P_ORDER.Remark as Remark, P_ORDER.State AS OrderState, "
                                               + "P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.OnTimeDiscrpion AS EOnTimeDesc, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.SafeDiscrpion AS EIsSafeDesc, P_EVALUATE.Health AS EHealth, "
                                               + "P_EVALUATE.HealthDiscrpion AS EHealthDesc, P_EVALUATE.Pack AS EPack, P_EVALUATE.PackDiscrpion AS EPackDesc, P_EVALUATE.CostEffective AS ECost, "
                                               + "P_EVALUATE.CostEffectiveDiscrpion AS ECostDesc, P_EVALUATE.OtherDiscrpion AS EOtherDesc, ISNULL(P_EVALUATE.Star, 99) AS EStar, P_EVALUATE.CreateDate AS ECreateDate, "
                                               + "P_ORDER.IsRetuen AS IsReturn, P_ORDER_XMS_REPORT.cancelFailReason AS XMSOrderCancelReason, P_ORDER.ReceiveState, P_ORDER_XMS_REPORT.cancelFeedback AS XMSCancelFeedback, "
                                               + "P_ORDER_XMS_REPORT.cancelState AS XMSCancelState, P_ORDER_XMS_REPORT.bookState AS XMSBookState, P_ORDER.Channel AS Channel "
                                               + "FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN "
                                               + "WP_QYUSER AS DCUser ON P_ORDER.UserId = DCUser.UserId LEFT OUTER JOIN "
                                               + "P_USERINFO AS DCUser1 ON P_ORDER.UserId = DCUser1.UserId LEFT OUTER JOIN "
                                               + "P_ORDER_XMS_REPORT ON P_ORDER.XmsOrderId = P_ORDER_XMS_REPORT.XmsOrderId LEFT OUTER JOIN "
                                               + "P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID "
                                               + "WHERE 1=1";
        private const string _sqlOrderPageOrderBy = "ORDER BY P_ORDER.DeliverTime DESC ";

        public List<HT_ORDER_REPORT_VIEW> LoadOrderReportPage(string srh_CN, string srh_MUDID,string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD, int rows, int page, out int total)
        {
            List<HT_ORDER_REPORT_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            string sqlString = _sqlOrderPageSelect1;
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(srh_CN))
            {
                sqlString += " AND substring(P_ORDER.CN, 1, 1) = 'H'";
                sqlString += "  AND(P_ORDER.CN = @CN or @CN = '')";
                listParams.Add(new SqlParameter("@CN", srh_CN));
            }
            if (!string.IsNullOrEmpty(srh_MUDID))
            {
                sqlString += "AND(P_ORDER.UserId = @MUDID or @MUDID = '')";
                listParams.Add(new SqlParameter("@MUDID", srh_MUDID));
            }
            if (!string.IsNullOrEmpty(srh_TACode))
            {
                sqlString += "AND(P_PreApproval.MRTerritoryCode = @TACode or @TACode = '')";
                listParams.Add(new SqlParameter("@TACode", srh_TACode));
            }
            if (!string.IsNullOrEmpty(srh_HospitalCode))
            {
                if (srh_HospitalCode.Split(',').Length > 1)
                {
                    sqlString += "AND((P_PreApproval.HospitalCode = @HospitalCode or(P_PreApproval.HospitalCode = @OldHospitalCode and @OldHospitalCode <> '')) or @HospitalCode = '')";
                    listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]));
                    listParams.Add(new SqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',')[1]));
                }
                else
                {
                    sqlString += "AND (P_ORDER.HospitalId=@HospitalCode) ";
                    listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode));
                }
            }
            if (!string.IsNullOrEmpty(srh_RestaurantId))
            {
                sqlString += "AND(P_ORDER.RestaurantId = @RestaurantId or @RestaurantId = '')";
                listParams.Add(new SqlParameter("@RestaurantId", srh_RestaurantId));
            }
            if (!string.IsNullOrEmpty(srh_CostCenter))
            {
                if (srh_CostCenter.Split(',').Length > 1)
                {
                    sqlString += "AND(P_PreApproval.CostCenter like '%' + @CostCenter + '%' or P_PreApproval.CostCenter like '%' + @OldCostCenter + '%')";
                    listParams.Add(new SqlParameter("@CostCenter", srh_CostCenter.Split(',')[0]));
                    listParams.Add(new SqlParameter("@OldCostCenter", srh_CostCenter.Split(',')[1]));
                }
                else
                {
                    sqlString += "AND(P_PreApproval.CostCenter like '%' + @CostCenter + '%')";
                    listParams.Add(new SqlParameter("@CostCenter", srh_CostCenter));
                }
            }
            if (!string.IsNullOrEmpty(RD))
            {
                sqlString += "AND(P_PreApproval.RDTerritoryCode like '%' + @RDTerritoryCode + '%')";
                listParams.Add(new SqlParameter("@RDTerritoryCode", RD));
            }
            if (srh_CreateTimeBegin != null)
            {
                sqlString += "AND (('" + srh_CreateTimeBegin + "' IS NULL) OR (P_ORDER.CreateDate >= '" + srh_CreateTimeBegin + "')) ";
                //sqlString += "AND ( (P_ORDER.CreateDate >= @CreateTimeBegin)) ";
                //listParams.Add(new SqlParameter("@CreateTimeBegin", srh_CreateTimeBegin));
            }
            if (srh_CreateTimeEnd != null)
            {
                sqlString += "AND (('" + srh_CreateTimeEnd + "' IS NULL) OR (P_ORDER.CreateDate < '" + srh_CreateTimeEnd + "')) ";
                //sqlString += "AND ( (P_ORDER.CreateDate < @CreateTimeEnd)) ";

                //listParams.Add(new SqlParameter("@CreateTimeEnd", srh_CreateTimeEnd));
            }
            if (srh_State != null)
            {
                sqlString += " AND((@State IS NULL) OR(P_ORDER.State = @State))  ";
                listParams.Add(new SqlParameter("@State", srh_State));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                sqlString += "AND ((@Supplier='') OR (P_ORDER.Channel = @Supplier))  ";
                listParams.Add(new SqlParameter("@Supplier", Supplier));
            }
            if (!string.IsNullOrEmpty(IsSpecialOrder))
            {
                if (IsSpecialOrder == "9")
                {
                    sqlString += "AND (P_ORDER.IsSpecialOrder in (1,2))  ";
                }
                else
                {
                    sqlString += "AND ((@IsSpecialOrder='') OR (P_ORDER.IsSpecialOrder = @IsSpecialOrder))  ";
                    listParams.Add(new SqlParameter("@IsSpecialOrder", IsSpecialOrder));
                }

            }
            if (srh_DeliverTimeBegin != null)
            {
                sqlString += "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) ";
                listParams.Add(new SqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin));
            }
            if (srh_DeliverTimeEnd != null)
            {
                sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd))";
                listParams.Add(new SqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd));
            }
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<HT_ORDER_REPORT_VIEW>(rows, page, out total,
                   sqlString, _sqlOrderPageOrderBy, listParams.ToArray());

            }
            //using (var conn = sqlServerTemplate.GetSqlConnection())
            //{
            //    conn.Open();
            //    rtnData = sqlServerTemplate.LoadPages<HT_ORDER_REPORT_VIEW>(rows, page, out total,
            //       _sqlOrderPageSelect,
            //        _sqlOrderPageOrderBy, new SqlParameter[]
            //        {
            //            SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
            //            SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
            //            SqlParameterFactory.GetSqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]),
            //            SqlParameterFactory.GetSqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',').Length>1?srh_HospitalCode.Split(',')[1]:string.Empty),
            //            SqlParameterFactory.GetSqlParameter("@RestaurantId", srh_RestaurantId.Trim()),
            //            SqlParameterFactory.GetSqlParameter("@CostCenter", srh_CostCenter.Split(',')[0]),
            //            SqlParameterFactory.GetSqlParameter("@OldCostCenter", srh_CostCenter.Split(',').Length>1?srh_CostCenter.Split(',')[1]:string.Empty),
            //            SqlParameterFactory.GetSqlParameter("@State", srh_State),
            //            SqlParameterFactory.GetSqlParameter("@Supplier", Supplier),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeBegin", srh_CreateTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeEnd", srh_CreateTimeEnd),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd)
            //        });
            //}
            return rtnData;
        }

        public List<HT_ORDER_REPORT_VIEW> LoadOrderReport(string srh_CN, string srh_MUDID, string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD)
        {
            try
            {


                List<HT_ORDER_REPORT_VIEW> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string sqlString = _sqlOrderPageSelect1;
                var listParams = new List<SqlParameter>();
                if (!string.IsNullOrEmpty(srh_CN))
                {
                    sqlString += " AND substring(P_ORDER.CN, 1, 1) = 'H'";
                    sqlString += "  AND(P_ORDER.CN = @CN or @CN = '')";
                    listParams.Add(new SqlParameter("@CN", srh_CN));
                }
                if (!string.IsNullOrEmpty(srh_MUDID))
                {
                    sqlString += "AND(P_ORDER.UserId = @MUDID or @MUDID = '')";
                    listParams.Add(new SqlParameter("@MUDID", srh_MUDID));
                }
                if (!string.IsNullOrEmpty(srh_TACode))
                {
                    sqlString += "AND(P_PreApproval.MRTerritoryCode = @TACode or @TACode = '')";
                    listParams.Add(new SqlParameter("@TACode", srh_TACode));
                }
                if (!string.IsNullOrEmpty(srh_HospitalCode))
                {
                    if (srh_HospitalCode.Split(',').Length > 1)
                    {
                        sqlString += "AND((P_PreApproval.HospitalCode = @HospitalCode or(P_PreApproval.HospitalCode = @OldHospitalCode and @OldHospitalCode <> '')) or @HospitalCode = '')";
                        listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]));
                        listParams.Add(new SqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',')[1]));
                    }
                    else
                    {
                        sqlString += "AND (P_ORDER.HospitalId=@HospitalCode) ";
                        listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode));
                    }
                }
                if (!string.IsNullOrEmpty(srh_RestaurantId))
                {
                    sqlString += "AND(P_ORDER.RestaurantId = @RestaurantId or @RestaurantId = '')";
                    listParams.Add(new SqlParameter("@RestaurantId", srh_RestaurantId));
                }
                if (!string.IsNullOrEmpty(srh_CostCenter))
                {
                    if (srh_CostCenter.Split(',').Length > 1)
                    {
                        sqlString += "AND(P_PreApproval.CostCenter like '%' + @CostCenter + '%' or P_PreApproval.CostCenter like '%' + @OldCostCenter + '%')";
                        listParams.Add(new SqlParameter("@CostCenter", srh_CostCenter.Split(',')[0]));
                        listParams.Add(new SqlParameter("@OldCostCenter", srh_CostCenter.Split(',')[1]));
                    }
                    else
                    {
                        sqlString += "AND(P_PreApproval.CostCenter like '%' + @CostCenter + '%')";
                        listParams.Add(new SqlParameter("@CostCenter", srh_CostCenter));
                    }
                }
                if (!string.IsNullOrEmpty(RD))
                {
                    sqlString += "AND(P_PreApproval.RDTerritoryCode like '%' + @RDTerritoryCode + '%')";
                    listParams.Add(new SqlParameter("@RDTerritoryCode", RD));
                }
                if (srh_CreateTimeBegin != null)
                {
                    sqlString += "AND (('" + srh_CreateTimeBegin + "' IS NULL) OR (P_ORDER.CreateDate >= '" + srh_CreateTimeBegin + "')) ";
                    //sqlString += "AND ( (P_ORDER.CreateDate >= @CreateTimeBegin)) ";
                    // listParams.Add(new SqlParameter("@CreateTimeBegin", srh_CreateTimeBegin));
                }
                if (srh_CreateTimeEnd != null)
                {
                    sqlString += "AND (('" + srh_CreateTimeEnd + "' IS NULL) OR (P_ORDER.CreateDate < '" + srh_CreateTimeEnd + "')) ";
                    // sqlString += "AND ((@CreateTimeEnd IS NULL) OR (P_ORDER.CreateDate < @CreateTimeEnd)) ";
                    //sqlString += "AND ( (P_ORDER.CreateDate < @CreateTimeEnd)) ";

                    listParams.Add(new SqlParameter("@CreateTimeEnd", srh_CreateTimeEnd));
                }
                if (srh_State != null)
                {
                    sqlString += " AND((@State IS NULL) OR(P_ORDER.State = @State))  ";
                    listParams.Add(new SqlParameter("@State", srh_State));
                }
                if (!string.IsNullOrEmpty(Supplier))
                {
                    sqlString += "AND ((@Supplier='') OR (P_ORDER.Channel = @Supplier))  ";
                    listParams.Add(new SqlParameter("@Supplier", Supplier));
                }
                if (!string.IsNullOrEmpty(IsSpecialOrder))
                {
                    if (IsSpecialOrder == "9")
                    {
                        sqlString += "AND (P_ORDER.IsSpecialOrder in (1,2))  ";
                    }
                    else
                    {
                        sqlString += "AND ((@IsSpecialOrder='') OR (P_ORDER.IsSpecialOrder = @IsSpecialOrder))  ";
                        listParams.Add(new SqlParameter("@IsSpecialOrder", IsSpecialOrder));
                    }

                }
                if (srh_DeliverTimeBegin != null)
                {
                    sqlString += "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin));
                }
                if (srh_DeliverTimeEnd != null)
                {
                    sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd))";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd));
                }

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    LogHelper.Error("aabbbcc");
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<HT_ORDER_REPORT_VIEW>(
                       sqlString + _sqlOrderPageOrderBy, listParams.ToArray());

                }
                return rtnData;
            }
            catch (Exception e)
            {

                LogHelper.Error(e.Message);
            }
            return null;



            //20190122
            //var sqlServerTemplate1 = sqlServerTemplFactory.CreateDbTemplate();
            //using (var conn1 = sqlServerTemplate1.GetSqlConnection())
            //{
            //    conn1.Open();
            //    for (int i = 0; i < rtnData.Count; i++)
            //    {
            //        var pre = rtnData[i];

            //        var entity = sqlServerTemplate1.Find<LineManager>(
            //        "select up.Name AS LineManagerName, up.UserId AS LineManagerId  from[WP_QYUSER] u left join[WP_QYUSER] up on u.LineManagerId = up.ID where u.UserId = @UserId",
            //        new SqlParameter[]
            //        {
            //            SqlParameterFactory.GetSqlParameter("@UserId",  pre.ApplierMUDID),
            //        });
            //        pre.ApplierMUDID = entity.LineManagerId;
            //        entity = sqlServerTemplate1.Find<LineManager>(
            //        "select up.Name AS LineManagerName, up.UserId AS LineManagerId from[WP_QYUSER] u left join[WP_QYUSER] up on u.LineManagerId = up.ID where u.UserId = @UserId",
            //        new SqlParameter[]
            //        {
            //            SqlParameterFactory.GetSqlParameter("@UserId",  pre.ApplierMUDID),
            //        });
            //        pre.ApplierMUDID = entity.LineManagerId;
            //        rtnData[i].Level2UserId = entity.LineManagerId;
            //        rtnData[i].Level2Name = entity.LineManagerName;
            //        entity = sqlServerTemplate1.Find<LineManager>(
            //       "select up.Name AS LineManagerName, up.UserId AS LineManagerId from[WP_QYUSER] u left join[WP_QYUSER] up on u.LineManagerId = up.ID where u.UserId = @UserId",
            //       new SqlParameter[]
            //       {
            //            SqlParameterFactory.GetSqlParameter("@UserId",  pre.ApplierMUDID),
            //       });
            //        rtnData[i].Level3UserId = entity.LineManagerId;
            //        rtnData[i].Level3Name = entity.LineManagerName;

            //    }


            //}
            //var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();

            //using (var conn = sqlServerTemplate.GetSqlConnection())
            //{
            //    conn.Open();
            //    rtnData = sqlServerTemplate.Load<HT_ORDER_REPORT_VIEW>(
            //        _sqlOrderPageSelect + _sqlOrderPageOrderBy,
            //        new SqlParameter[]
            //        {
            //            SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
            //            SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
            //            SqlParameterFactory.GetSqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]),
            //            SqlParameterFactory.GetSqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',').Length>1?srh_HospitalCode.Split(',')[1]:string.Empty),
            //            SqlParameterFactory.GetSqlParameter("@RestaurantId", srh_RestaurantId.Trim()),
            //            SqlParameterFactory.GetSqlParameter("@CostCenter", srh_CostCenter.Split(',')[0]),
            //            SqlParameterFactory.GetSqlParameter("@OldCostCenter", srh_CostCenter.Split(',').Length>1?srh_CostCenter.Split(',')[1]:string.Empty),
            //            SqlParameterFactory.GetSqlParameter("@State", srh_State),
            //            SqlParameterFactory.GetSqlParameter("@Supplier", Supplier),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeBegin", srh_CreateTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeEnd", srh_CreateTimeEnd),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd)
            //        });
            //}
            //return rtnData;

        }

        #endregion

        #region 后台订单服务-report
        public List<P_ORDER_DAILY_VIEW> LoadNonHTOrderMnt(string srh_CN, string srh_MUDID, string srh_HospitalCode, string srh_RestaurantId, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT)
        {
            List<P_ORDER_DAILY_VIEW> rtnData;
            //20190114
            string sqlString = _sqlNonOrderSb;
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(srh_CN))
            {
                sqlString += " AND (P_ORDER.PO=@CN) ";
                listParams.Add(new SqlParameter("@CN", srh_CN));
            }
            if (!string.IsNullOrEmpty(srh_MUDID))
            {
                sqlString += " AND (P_ORDER.UserId=@MUID)";
                listParams.Add(new SqlParameter("@MUID", srh_MUDID));
            }
            if (!string.IsNullOrEmpty(srh_HospitalCode))
            {
                if (srh_HospitalCode.Split(',').Length > 1)
                {
                    sqlString += "AND (P_ORDER.HospitalId=@HospitalCode or (P_ORDER.HospitalId=@OldHospitalCode and @OldHospitalCode<>''))";
                    listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]));
                    listParams.Add(new SqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',')[1]));
                }
                else
                {
                    sqlString += "AND (P_ORDER.HospitalId=@HospitalCode) ";
                    listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode));
                }
            }
            if (!string.IsNullOrEmpty(srh_RestaurantId))
            {
                sqlString += "  AND (P_ORDER.RestaurantId=@RestaurantId) ";
                listParams.Add(new SqlParameter("@RestaurantId", srh_RestaurantId));
            }
            if (srh_CreateTimeBegin != null)
            {
                sqlString += "AND ((@CreateTimeBegin IS NULL) OR (P_ORDER.CreateDate >= @CreateTimeBegin)) ";
                listParams.Add(new SqlParameter("@CreateTimeBegin", srh_CreateTimeBegin));
            }
            if (srh_CreateTimeEnd != null)
            {
                sqlString += "AND ((@CreateTimeEnd IS NULL) OR (P_ORDER.CreateDate < @CreateTimeEnd))";
                listParams.Add(new SqlParameter("@CreateTimeEnd", srh_CreateTimeEnd));
            }
            if (srh_State != null)
            {
                sqlString += " AND (P_ORDER.State = @State) ";
                listParams.Add(new SqlParameter("@State", srh_State));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                sqlString += "AND(P_ORDER.Channel = @Supplier)";
                listParams.Add(new SqlParameter("@Supplier", Supplier));
            }
            if (srh_DeliverTimeBegin != null)
            {
                sqlString += "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) ";
                listParams.Add(new SqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin));
            }
            if (srh_DeliverTimeEnd != null)
            {
                sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd))";
                listParams.Add(new SqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd));
            }
            if (isNonHT != null)
            {
                sqlString += "AND (P_ORDER.IsNonHT = @IsNonHT) ";
                listParams.Add(new SqlParameter("@IsNonHT", isNonHT));
            }
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.Load<P_ORDER_DAILY_VIEW>(
                    sqlString + _sqlNonHTOrderApprovePageOrderBy, listParams.ToArray()
                   );
            }
            //var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            //string sqlString = _sqlOrderMntPageSelect;
            //if (isNonHT == 1)
            //{
            //    sqlString = _sqlNonOrderMntPageSelect;
            //}
            //using (var conn = sqlServerTemplate.GetSqlConnection())
            //{
            //    conn.Open();
            //    rtnData = sqlServerTemplate.Load<P_ORDER_DAILY_VIEW>(
            //        sqlString + _sqlOrderMntPageOrderBy,
            //        new SqlParameter[]
            //        {
            //            SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
            //            SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
            //            SqlParameterFactory.GetSqlParameter("@State", srh_State),
            //            SqlParameterFactory.GetSqlParameter("@Supplier", Supplier),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeBegin", srh_CreateTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeEnd", srh_CreateTimeEnd),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
            //            SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
            //        });
            //}
            return rtnData;
        }

        public List<P_ORDER_DAILY_VIEW> LoadNonHTOrderMntPage(string srh_CN, string srh_MUDID, string srh_HospitalCode, string srh_RestaurantId, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, int isNonHT, int rows, int page, out int total)
        {
            List<P_ORDER_DAILY_VIEW> rtnData;
            //20190114
            string sqlString = _sqlNonOrderSb;
            var listParams = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(srh_CN))
            {
                sqlString += " AND (P_ORDER.PO=@CN) ";
                listParams.Add(new SqlParameter("@CN", srh_CN));
            }
            if (!string.IsNullOrEmpty(srh_MUDID))
            {
                sqlString += " AND (P_ORDER.UserId=@MUID)";
                listParams.Add(new SqlParameter("@MUID", srh_MUDID));
            }
            if (!string.IsNullOrEmpty(srh_HospitalCode))
            {
                if (srh_HospitalCode.Split(',').Length > 1)
                {
                    sqlString += "AND (P_ORDER.HospitalId=@HospitalCode or (P_ORDER.HospitalId=@OldHospitalCode and @OldHospitalCode<>''))";
                    listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]));
                    listParams.Add(new SqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',')[1]));
                }
                else
                {
                    sqlString += "AND (P_ORDER.HospitalId=@HospitalCode) ";
                    listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode));
                }
            }
            if (!string.IsNullOrEmpty(srh_RestaurantId))
            {
                sqlString += "  AND (P_ORDER.RestaurantId=@RestaurantId) ";
                listParams.Add(new SqlParameter("@RestaurantId", srh_RestaurantId));
            }
            if (srh_CreateTimeBegin != null)
            {
                sqlString += "AND ((@CreateTimeBegin IS NULL) OR (P_ORDER.CreateDate >= @CreateTimeBegin)) ";
                listParams.Add(new SqlParameter("@CreateTimeBegin", srh_CreateTimeBegin));
            }
            if (srh_CreateTimeEnd != null)
            {
                sqlString += "AND ((@CreateTimeEnd IS NULL) OR (P_ORDER.CreateDate < @CreateTimeEnd))";
                listParams.Add(new SqlParameter("@CreateTimeEnd", srh_CreateTimeEnd));
            }
            if (srh_State != null)
            {
                sqlString += " AND (P_ORDER.State = @State) ";
                listParams.Add(new SqlParameter("@State", srh_State));
            }
            if (!string.IsNullOrEmpty(Supplier))
            {
                sqlString += "AND(P_ORDER.Channel = @Supplier)";
                listParams.Add(new SqlParameter("@Supplier", Supplier));
            }
            if (srh_DeliverTimeBegin != null)
            {
                sqlString += "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) ";
                listParams.Add(new SqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin));
            }
            if (srh_DeliverTimeEnd != null)
            {
                sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd))";
                listParams.Add(new SqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd));
            }
            if (isNonHT != null)
            {
                sqlString += "AND (P_ORDER.IsNonHT = @IsNonHT) ";
                listParams.Add(new SqlParameter("@IsNonHT", isNonHT));
            }
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_ORDER_DAILY_VIEW>(rows, page, out total,
                   sqlString,
                    _sqlNonHTOrderApprovePageOrderBy, listParams.ToArray());

            }
            //var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            //string sqlString = _sqlNonOrderMntPageSelect;
            //using (var conn = sqlServerTemplate.GetSqlConnection())
            //{
            //    conn.Open();
            //    rtnData = sqlServerTemplate.LoadPages<P_ORDER_DAILY_VIEW>(rows, page, out total,
            //       sqlString,
            //        _sqlNonHTOrderApprovePageOrderBy, new SqlParameter[]
            //        {
            //            SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
            //            SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
            //            SqlParameterFactory.GetSqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]),
            //            SqlParameterFactory.GetSqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',').Length>1?srh_HospitalCode.Split(',')[1]:string.Empty),
            //            SqlParameterFactory.GetSqlParameter("@RestaurantId", srh_RestaurantId.Trim()),
            //            SqlParameterFactory.GetSqlParameter("@State", srh_State),
            //            SqlParameterFactory.GetSqlParameter("@Supplier", Supplier),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeBegin", srh_CreateTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@CreateTimeEnd", srh_CreateTimeEnd),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
            //            SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
            //            SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
            //        });
            //}
            return rtnData;
        }

        public List<P_ORDER_APPROVE_VIEW> LoadNonHTOrderApprovePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int srh_MMCoEApproveState, int isNonHt, int rows, int page, out int total)
        {
            List<P_ORDER_APPROVE_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            var sqlString = _sqlNonOrderApprovePageSelect;
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                rtnData = sqlServerTemplate.LoadPages<P_ORDER_APPROVE_VIEW>(rows, page, out total,
                   sqlString,
                    _sqlNonHTOrderApprovePageOrderBy, new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@MMCoEApproveState", srh_MMCoEApproveState),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHt)
                    });
            }

            return rtnData;
        }
        #endregion

        #region 后台订单评价-List
        private const string _sqlNonHTOrderEvaluateSltField = "SELECT P_ORDER.ID, P_ORDER.CN, P_ORDER.PO, P_ORDER.UserId AS MUDID, DCUser.Name AS DCUserName, P_ORDER.DeliverTime AS SendTime, P_ORDER.RestaurantName AS RestName, "
                                                        + "P_EVALUATE.Star AS EStar, P_EVALUATE.OnTime AS EOnTime, P_EVALUATE.IsSafe AS EIsSafe, P_EVALUATE.Health AS EHealth, P_EVALUATE.Pack AS EPack, "
                                                        + "P_EVALUATE.CostEffective AS ECost "
                                                        + "FROM P_ORDER INNER JOIN P_EVALUATE ON P_ORDER.ID = P_EVALUATE.OrderID LEFT OUTER JOIN "
                                                        + "WP_QYUSER AS DCUser ON P_ORDER.UserId = DCUser.UserId ";
        private const string _sqlNonHTOrderEvaluateSltCondition1 = "WHERE (P_ORDER.PO LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND P_ORDER.IsNonHT=@IsNonHT AND "
                                                           + "((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) ";
        private const string _sqlNonHTOrderEvaluateSltCondition2 = "WHERE (P_ORDER.PO LIKE '%' + @CN + '%') AND (P_ORDER.UserId LIKE '%' + @MUDID + '%') AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) AND P_ORDER.IsNonHT=@IsNonHT AND "
                                                            + "((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd)) AND ((@L60 IS NOT NULL AND P_EVALUATE.OnTime = @L60) OR (@UsSafe IS NOT NULL AND P_EVALUATE.IsSafe = @UsSafe) OR (@UnSend IS NOT NULL AND P_EVALUATE.OnTime = @UnSend)) ";

        private const string _sqlNonHTOrderEvaluateSltOrderBy = "ORDER BY P_ORDER.DeliverTime DESC ";
        public List<P_ORDER_EVALUATE_VIEW> LoadNonHTOrderEvaluatePage(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHT, int rows, int page, out int total)
        {
            List<P_ORDER_EVALUATE_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();

            if (srh_Large60 == false && srh_UnSafe == false && srh_UnSend == false)
            {
                var sqlString = _sqlNonHTOrderEvaluateSltField;
                var sqlString1 = _sqlNonHTOrderEvaluateSltCondition1;
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.LoadPages<P_ORDER_EVALUATE_VIEW>(rows, page, out total,
                       sqlString + sqlString1,
                        _sqlNonHTOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                        });
                }
            }
            else
            {
                int? large60, unSafe, unSend;
                if (srh_Large60 == true)
                {
                    large60 = 1;
                }
                else
                {
                    large60 = null;
                }
                if (srh_UnSafe == true)
                {
                    unSafe = 1;
                }
                else
                {
                    unSafe = null;
                }
                if (srh_UnSend == true)
                {
                    unSend = 0;
                }
                else
                {
                    unSend = null;
                }
                var sqlString = _sqlNonHTOrderEvaluateSltField;
                var sqlString1 = _sqlNonHTOrderEvaluateSltCondition2;
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.LoadPages<P_ORDER_EVALUATE_VIEW>(rows, page, out total,
                       sqlString + sqlString1,
                        _sqlNonHTOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@L60", large60),
                        SqlParameterFactory.GetSqlParameter("@UsSafe", unSafe),
                        SqlParameterFactory.GetSqlParameter("@UnSend", unSend),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                        });
                }
            }

            return rtnData;
        }
        public List<P_ORDER_EVALUATE_VIEW> LoadNonHTOrderEvaluate(string srh_CN, string srh_MUDID, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, bool srh_Large60, bool srh_UnSafe, bool srh_UnSend, int isNonHT)
        {
            List<P_ORDER_EVALUATE_VIEW> rtnData;
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            if (srh_Large60 == false && srh_UnSafe == false && srh_UnSend == false)
            {
                var sqlString = _sqlNonHTOrderEvaluateSltField;
                var sqlString1 = _sqlNonHTOrderEvaluateSltCondition1;
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_EVALUATE_VIEW>(sqlString + sqlString1 + _sqlNonHTOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                        });
                }
            }
            else
            {
                int? large60, unSafe, unSend;
                if (srh_Large60 == true)
                {
                    large60 = 1;
                }
                else
                {
                    large60 = null;
                }
                if (srh_UnSafe == true)
                {
                    unSafe = 1;
                }
                else
                {
                    unSafe = null;
                }
                if (srh_UnSend == true)
                {
                    unSend = 0;
                }
                else
                {
                    unSend = null;
                }
                var sqlString = _sqlNonHTOrderEvaluateSltField;
                var sqlString1 = _sqlNonHTOrderEvaluateSltCondition2;
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_ORDER_EVALUATE_VIEW>(sqlString + sqlString1 + _sqlNonHTOrderEvaluateSltOrderBy, new SqlParameter[]
                        {
                        SqlParameterFactory.GetSqlParameter("@CN", srh_CN),
                        SqlParameterFactory.GetSqlParameter("@MUDID", srh_MUDID),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin),
                        SqlParameterFactory.GetSqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd),
                        SqlParameterFactory.GetSqlParameter("@L60", large60),
                        SqlParameterFactory.GetSqlParameter("@UsSafe", unSafe),
                        SqlParameterFactory.GetSqlParameter("@UnSend", unSend),
                        SqlParameterFactory.GetSqlParameter("@IsNonHT", isNonHT)
                        });
                }
            }

            return rtnData;
        }
        #endregion

        #region *****************消息推送相关*********************

        #region 获取需要发送确认收餐消息的订单(收餐时间后一小时)
        /// <summary>
        /// 获取需要发送确认收餐消息的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadConfirmOrders(DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WHERE  [State] IN (4,12) AND DeliverTime < @DeliverTime AND IsPushOne = 0 ORDER BY CreateDate DESC",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", nowDate)
                    });
                return list;
            }
        }
        #endregion

        #region 更新推送状态（送餐时间后一小时未确认收餐）
        /// <summary>
        /// 更新推送状态
        /// </summary>
        /// <param name="xmsOrderIds"></param>
        /// <returns></returns>
        public int UpdatePushOne(string xmsOrderIds)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var res = sqlServerTemplate.Update($"UPDATE [P_ORDER] SET IsPushOne = 1 WHERE XmsOrderId IN ( '" + xmsOrderIds + "' )",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        #region 更新上传文件状态
        /// <summary>
        /// 更新上传文件状态
        /// </summary>
        /// <param name="state"></param>
        /// <param name="htCode"></param>
        /// <returns></returns>
        public int UpdateOrderUpload(int state, string htCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var res = sqlServerTemplate.Update($"UPDATE [P_ORDER] SET IsOrderUpload = @IsOrderUpload WHERE CN = @CN AND State IN (6,7,8,9) ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@IsOrderUpload", state),
                        SqlParameterFactory.GetSqlParameter("@CN", htCode)
                    });
                return res;
            }
        }
        #endregion

        #region 获取需要发送确认收餐消息的订单(晚十点)
        /// <summary>
        /// 获取需要发送确认收餐消息的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadConfirmOrders()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            var dateTime = DateTime.Now;
            dateTime = dateTime.AddHours(-1);
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WHERE State IN (4,12) AND DeliverTime<@DeliverTime",
                    new SqlParameter[]{
                        SqlParameterFactory.GetSqlParameter("@DeliverTime",dateTime)
                    });
                return list;
            }
        }
        #endregion

        #region 获取需要上传文件的订单(晚十点)
        /// <summary>
        /// 获取需要上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadUploadOrders()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WHERE  State IN(6,7,8,9) AND IsOrderUpload = 0 ",
                    new SqlParameter[] { });
                return list;
            }
        }
        #endregion

        #region 获取上传文件需要审批的订单(晚十点)
        /// <summary>
        /// 获取上传文件需要审批的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadUploadFailOrders()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT a.* FROM P_ORDER a LEFT JOIN P_PreUploadOrder b ON a.CN = b.HTCode WHERE a.State IN(6,7,8,9) AND a.IsOrderUpload = 1 and b.State <> 4 ",
                    new SqlParameter[] { });
                return list;
            }
        }
        #endregion

        #region 获取需要发送确认收餐消息的订单(晚六点)
        /// <summary>
        /// 获取需要发送确认收餐消息的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadOrderConfirms(DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE  [State] IN(4,12) AND DeliverTime < @DeliverTime ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", nowDate)
                    });
                return list;
            }
        }
        #endregion

        #region 查询指定用户需要收餐的订单
        /// <summary>
        /// 查询指定用户需要收餐的订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadUserConfirmOrders(string userId, DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE  [State] IN(4,12) AND DeliverTime < @DeliverTime AND UserId=@UserId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@DeliverTime", nowDate),
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return list;
            }
        }
        #endregion

        #region 获取需要上传文件的订单(晚六点)
        /// <summary>
        /// 获取需要上传文件的订单(晚六点)
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadOrderUpload(DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE  [State] IN(6,7,8,9) AND ReceiveDate < @ReceiveDate AND IsOrderUpload = 0 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ReceiveDate", nowDate)
                    });
                return list;
            }
        }
        #endregion

        #region 查询指定用户需要上传文件的订单
        /// <summary>
        /// 查询指定用户需要上传文件的订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadUserOrderUpload(string userId, DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE  [State] IN(6,7,8,9) AND ReceiveDate < @ReceiveDate AND IsOrderUpload = 0 AND (UserId=@UserId OR TransferUserMUDID=@UserId)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ReceiveDate", nowDate),
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return list;
            }
        }
        #endregion

        #region 根据HTCode查询订单
        /// <summary>
        /// 根据HTCode查询订单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public P_ORDER FindOrderByCN(string htCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Find<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE CN=@CN AND State NOT IN(5,11) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@CN", htCode)
                    });
                return list;
            }
        }
        #endregion

        #region 获取需要审批上传文件的订单
        /// <summary>
        /// 获取需要审批上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadFailOrder(DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT a.* FROM P_ORDER a WITH(NOLOCK) LEFT JOIN P_PreUploadOrder b ON a.CN = b.HTCode WHERE a.State IN(6, 7, 8, 9) AND a.IsOrderUpload = 1 and b.State <> 4 and b.ModifyDate < @ModifyDate ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", nowDate)
                    });
                return list;
            }
        }
        #endregion

        #region 获取需要审批上传文件的订单
        /// <summary>
        /// 获取需要审批上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_PREUPLOADORDER> LoadFailUploadOrder(DateTime nowDate)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_PREUPLOADORDER>($"SELECT b.* FROM P_ORDER a WITH(NOLOCK) LEFT JOIN P_PreUploadOrder b ON a.CN = b.HTCode WHERE a.State IN(6, 7, 8, 9) AND a.IsOrderUpload = 1 and b.State <> 4 and b.ModifyDate < @ModifyDate ",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", nowDate)
                    });
                return list;
            }
        }
        #endregion

        #region 获取指定用户需要审批上传文件的订单
        /// <summary>
        /// 获取指定用户需要审批上传文件的订单
        /// </summary>
        /// <param name="nowDate"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadFailOrder(DateTime nowDate, string userId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT a.* FROM P_ORDER a WITH(NOLOCK) LEFT JOIN P_PreUploadOrder b ON a.CN = b.HTCode WHERE a.State IN(6, 7, 8, 9) AND a.IsOrderUpload = 1 and b.State <> 4 and b.ModifyDate < @ModifyDate and a.UserId=@UserId",
                    new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@ModifyDate", nowDate),
                        SqlParameterFactory.GetSqlParameter("@UserId", userId)
                    });
                return list;
            }
        }
        #endregion

        #region 收餐后一小时未上传文件
        /// <summary>
        /// 收餐后一小时未上传文件
        /// </summary>
        /// <param name="nowTime"></param>
        /// <returns></returns>
        public List<P_ORDER> LoadUploadFiles(DateTime nowTime)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE  [State] IN(6,7,8,9) AND ReceiveDate < @ReceiveDate AND IsOrderUpload = 0 AND IsPushTwo = 0 ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ReceiveDate", nowTime)
                    });
                return list;
            }
        }
        #endregion

        #region 更新推送状态（确认收餐后一小时未上传文件）
        /// <summary>
        /// 更新推送状态
        /// </summary>
        /// <param name="xmsOrderIds"></param>
        /// <returns></returns>
        public int UpdatePushTwo(string xmsOrderIds)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var coon = sqlServerTemplate.GetSqlConnection())
            {
                coon.Open();
                var res = sqlServerTemplate.Update($"UPDATE [P_ORDER] SET IsPushTwo = 1 WHERE XmsOrderId IN ( '" + xmsOrderIds + "' )",
                    new SqlParameter[] { });
                return res;
            }
        }
        #endregion

        #endregion


        #region **********************状态默认********************

        #region 获取需要自动更改为失败状态的订单
        /// <summary>
        /// 获取需要自动更改为失败状态的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadAutoChangeFail()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var DateTimeNow = DateTime.Now;
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE (State = 3 OR IsRetuen = 4) AND DeliverTime < @DeliverTime ORDER BY CreateDate DESC ",
                    new SqlParameter[]
                    {
                       SqlParameterFactory.GetSqlParameter("@DeliverTime", DateTime.Now)
                    });
                return list;
            }
        }
        #endregion

        #endregion

        #region 获取需要自动更改为成功状态的订单
        /// <summary>
        /// 获取需要自动更改为成功状态的订单
        /// </summary>
        /// <returns></returns>
        public List<P_ORDER> LoadAutoChangeSuccess()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var DateTimeNow = DateTime.Now;
                DateTimeNow = DateTimeNow.AddHours(-18);
                var list = sqlServerTemplate.Load<P_ORDER>($"SELECT * FROM [P_ORDER] WITH(NOLOCK) WHERE IsRetuen=1 AND ReturnOrderDate < @ReturnOrderDate ORDER BY CreateDate DESC ",
                    new SqlParameter[]
                    {
                       SqlParameterFactory.GetSqlParameter("@ReturnOrderDate", DateTimeNow)
                    });
                return list;
            }
        }
        #endregion 

        //转交订单
        public int AutoTransferOrder(string HTCode, string TransferUserMUDID, string TransferUserName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_ORDER SET IsTransfer=1,TransferOperatorMUDID='user01.admin', TransferOperatorName='System',TransferUserMUDID=@TransferUserMUDID,TransferUserName=@TransferUserName,TransferOperateDate=@TransferOperateDate,ActionState='0' " +
                    "WHERE CN = @HTCode  and State<>5 and state<>11",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode",HTCode ),
                        SqlParameterFactory.GetSqlParameter("@TransferUserMUDID",TransferUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferUserName", TransferUserName),
                        SqlParameterFactory.GetSqlParameter("@TransferOperateDate", DateTime.Now)
                    });
            }
        }


        //转交上传文件
        public int AutoTransferUpload(string HTCode, string TransferUserMUDID, string TransferUserName)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "UPDATE P_PreUploadOrder SET IsTransfer=1,TransferOperatorMUDID='user01.admin', TransferOperatorName='System',TransferUserMUDID=@TransferUserMUDID,TransferUserName=@TransferUserName,TransferOperateDate=@TransferOperateDate,ActionState='0' " +
                    "WHERE HTCode = @HTCode ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode",HTCode ),
                        SqlParameterFactory.GetSqlParameter("@TransferUserMUDID",TransferUserMUDID),
                        SqlParameterFactory.GetSqlParameter("@TransferUserName", TransferUserName),
                        SqlParameterFactory.GetSqlParameter("@TransferOperateDate", DateTime.Now)
                    });
            }
        }

        public int AddAutoTransferHistory(string HTCode, string FromMUDID, string ToMUDID, int Type)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                return sqlServerTemplate.ExecuteNonQuery(
                    "insert into P_AutoTransferDetail values(newid(),@HTCode,@FromMUDID,@ToMUDID,@CreateDate,@Type)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode",HTCode ),
                        SqlParameterFactory.GetSqlParameter("@FromMUDID",FromMUDID),
                        SqlParameterFactory.GetSqlParameter("@ToMUDID", ToMUDID),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", DateTime.Now),
                        SqlParameterFactory.GetSqlParameter("@Type", Type),
                    });
            }
        }

        public List<P_ORDER> LoadDataByInHTCode(string subHTCode)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<P_ORDER>("SELECT * from [P_ORDER] where [CN]  in(" + subHTCode + ") ",
                    new SqlParameter[]
                    {
                    });
                return rtnData;
            }
        }

        public List<HT_Order_Report> LoadReportByHTOrder(string HTCode, string EnterpriseOrderId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnData = sqlServerTemplate.Load<HT_Order_Report>("SELECT * from [P_HT_Order_Report] where HTCode=@HTCode and [xmsOrderId]=@xmsOrderId   ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@HTCode", HTCode),
                        SqlParameterFactory.GetSqlParameter("@xmsOrderId", EnterpriseOrderId)
                    });
                return rtnData;
            }
        }

       
        #region 导入HT数据
        /// <summary>
        /// 导入HT数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Import(string sql)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                SqlCommand commandAdd = new SqlCommand(
                    sql,
                    conn);
                commandAdd.Transaction = tran;
                commandAdd.Parameters.AddRange(
                    new SqlParameter[]
                    {
                    }
                );
                commandAdd.ExecuteNonQuery();
                tran.Commit();
                return 1;
            }
        }
        #endregion

        #region 导入HT数据--报表
        /// <summary>
        /// 导入HT数据--报表
        /// </summary>
        /// <param name="reportsql"></param>
        /// <returns></returns>
        public int ImportReport(string reportsql)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection() as SqlConnection)
            {
                conn.Open();
                var tran = conn.BeginTransaction();

                SqlCommand commandAdd = new SqlCommand(
                    reportsql,
                    conn);
                commandAdd.Transaction = tran;
                commandAdd.Parameters.AddRange(
                    new SqlParameter[]
                    {
                    }
                );
                commandAdd.ExecuteNonQuery();
                tran.Commit();
                return 1;
            }
        }
        #endregion

        #region 发送订单报表
        public int GetOrderCount(string srh_CN, string srh_MUDID, string srh_TACode, string srh_HospitalCode, string srh_RestaurantId, string srh_CostCenter, DateTime? srh_CreateTimeBegin, DateTime? srh_CreateTimeEnd, DateTime? srh_DeliverTimeBegin, DateTime? srh_DeliverTimeEnd, int? srh_State, string Supplier, string IsSpecialOrder, string RD)
        {
            try
            {
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string sqlString = @"SELECT P_ORDER.ID FROM P_ORDER WITH(NOLOCK) LEFT OUTER JOIN P_PreApproval ON P_ORDER.CN = P_PreApproval.HTCode
                                    WHERE 1=1 AND P_ORDER.State <> 5 AND P_ORDER.State <>11 ";
                var listParams = new List<SqlParameter>();
                if (!string.IsNullOrEmpty(srh_CN))
                {
                    sqlString += " AND substring(P_ORDER.CN, 1, 1) = 'H'";
                    sqlString += "  AND(P_ORDER.CN = @CN or @CN = '')";
                    listParams.Add(new SqlParameter("@CN", srh_CN));
                }
                if (!string.IsNullOrEmpty(srh_MUDID))
                {
                    sqlString += "AND(P_ORDER.UserId = @MUDID or @MUDID = '')";
                    listParams.Add(new SqlParameter("@MUDID", srh_MUDID));
                }
                if (!string.IsNullOrEmpty(srh_TACode))
                {
                    sqlString += "AND(P_PreApproval.MRTerritoryCode = @TACode or @TACode = '')";
                    listParams.Add(new SqlParameter("@TACode", srh_TACode));
                }
                if (!string.IsNullOrEmpty(srh_HospitalCode))
                {
                    if (srh_HospitalCode.Split(',').Length > 1)
                    {
                        sqlString += "AND((P_PreApproval.HospitalCode = @HospitalCode or(P_PreApproval.HospitalCode = @OldHospitalCode and @OldHospitalCode <> '')) or @HospitalCode = '')";
                        listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode.Split(',')[0]));
                        listParams.Add(new SqlParameter("@OldHospitalCode", srh_HospitalCode.Split(',')[1]));
                    }
                    else
                    {
                        sqlString += "AND (P_ORDER.HospitalId=@HospitalCode) ";
                        listParams.Add(new SqlParameter("@HospitalCode", srh_HospitalCode));
                    }
                }
                if (!string.IsNullOrEmpty(srh_RestaurantId))
                {
                    sqlString += "AND(P_ORDER.RestaurantId = @RestaurantId or @RestaurantId = '')";
                    listParams.Add(new SqlParameter("@RestaurantId", srh_RestaurantId));
                }
                if (!string.IsNullOrEmpty(srh_CostCenter))
                {
                    if (srh_CostCenter.Split(',').Length > 1)
                    {
                        sqlString += "AND(P_PreApproval.CostCenter like '%' + @CostCenter + '%' or P_PreApproval.CostCenter like '%' + @OldCostCenter + '%')";
                        listParams.Add(new SqlParameter("@CostCenter", srh_CostCenter.Split(',')[0]));
                        listParams.Add(new SqlParameter("@OldCostCenter", srh_CostCenter.Split(',')[1]));
                    }
                    else
                    {
                        sqlString += "AND(P_PreApproval.CostCenter like '%' + @CostCenter + '%')";
                        listParams.Add(new SqlParameter("@CostCenter", srh_CostCenter));
                    }
                }
                if (!string.IsNullOrEmpty(RD))
                {
                    sqlString += "AND(P_PreApproval.RDTerritoryCode like '%' + @RDTerritoryCode + '%')";
                    listParams.Add(new SqlParameter("@RDTerritoryCode", RD));
                }
                if (srh_CreateTimeBegin != null)
                {
                    sqlString += "AND (('" + srh_CreateTimeBegin + "' IS NULL) OR (P_ORDER.CreateDate >= '" + srh_CreateTimeBegin + "')) ";
                    //sqlString += "AND ( (P_ORDER.CreateDate >= @CreateTimeBegin)) ";
                    // listParams.Add(new SqlParameter("@CreateTimeBegin", srh_CreateTimeBegin));
                }
                if (srh_CreateTimeEnd != null)
                {
                    sqlString += "AND (('" + srh_CreateTimeEnd + "' IS NULL) OR (P_ORDER.CreateDate < '" + srh_CreateTimeEnd + "')) ";
                    // sqlString += "AND ((@CreateTimeEnd IS NULL) OR (P_ORDER.CreateDate < @CreateTimeEnd)) ";
                    //sqlString += "AND ( (P_ORDER.CreateDate < @CreateTimeEnd)) ";

                    listParams.Add(new SqlParameter("@CreateTimeEnd", srh_CreateTimeEnd));
                }
                if (srh_State != null)
                {
                    sqlString += " AND((@State IS NULL) OR(P_ORDER.State = @State))  ";
                    listParams.Add(new SqlParameter("@State", srh_State));
                }
                if (!string.IsNullOrEmpty(Supplier))
                {
                    sqlString += "AND ((@Supplier='') OR (P_ORDER.Channel = @Supplier))  ";
                    listParams.Add(new SqlParameter("@Supplier", Supplier));
                }
                if (!string.IsNullOrEmpty(IsSpecialOrder))
                {
                    if (IsSpecialOrder == "9")
                    {
                        sqlString += "AND (P_ORDER.IsSpecialOrder in (1,2))  ";
                    }
                    else
                    {
                        sqlString += "AND ((@IsSpecialOrder='') OR (P_ORDER.IsSpecialOrder = @IsSpecialOrder))  ";
                        listParams.Add(new SqlParameter("@IsSpecialOrder", IsSpecialOrder));
                    }

                }
                if (srh_DeliverTimeBegin != null)
                {
                    sqlString += "AND ((@DeliverTimeBegin IS NULL) OR (P_ORDER.DeliverTime >= @DeliverTimeBegin)) ";
                    listParams.Add(new SqlParameter("@DeliverTimeBegin", srh_DeliverTimeBegin));
                }
                if (srh_DeliverTimeEnd != null)
                {
                    sqlString += " AND ((@DeliverTimeEnd IS NULL) OR (P_ORDER.DeliverTime < @DeliverTimeEnd))";
                    listParams.Add(new SqlParameter("@DeliverTimeEnd", srh_DeliverTimeEnd));
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    var res = sqlServerTemplate.Load<P_ORDER>(sqlString, listParams.ToArray());
                    return res.Count;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e.Message);
                return 0;
            }
        }


        #endregion

        #region 同步订单表
        public int SyncOrder()
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtnCount = sqlServerTemplate.ExecuteNonQuery(@"DELETE FROM P_ORDER_COST WHERE ID IN (SELECT ID FROM [P_ORDER] WHERE ActionState = 0);",
                    new SqlParameter[]
                    {
                        
                    });

                var rtnData = sqlServerTemplate.ExecuteNonQuery(@"INSERT INTO P_ORDER_COST
                                SELECT [ID],[ChangeID],[UserId],[Market],[HospitalId],[Address],[CN],[RestaurantId],[RestaurantName],[RestaurantLogo],[TotalPrice],[DeliveryGeo],[Detail],[ChangeDetail],[FoodCount],[AttendCount],[DeliveryAddress]
                                ,[Consignee],[Phone],[DeliverTime],[Remark],[MMCoEImage],[State],[ReceiveCode],[CreateDate],[XmsOrderId],[OldXmlOrderId],[SendOrderDate],[ChangeOrderDate],[ReturnOrderDate],[XmsTotalPrice],[ChangeTotalPriceReason]
                                ,[ReceiveDate],[IsRetuen],[IsChange],[XmsOrderReason],[IsOuterMeeting],[RestaurantTel],[RestaurantAddress],[ApproveReason],[MMCoEApproveState],[MMCoEReason],[ReceiveState],[IsReturn],[IsDelivery]
                                ,[PO],[WBS],[IsNonHT],[Channel],[MeetingCode],[MeetingName],[TA],[ReceiveTime],[EnterpriseOrderId],[Province],[City],[HospitalName],[RealCount],[RealCountChangeReason],[RealCountChangeRemrak],[RealPrice]
                                ,[RealPriceChangeReason],[RealPriceChangeRemark],[SpecialRemarksProjectTeam],[IsSpecialOrder],[SpecialOrderReason],[SpecialOrderOperatorName],[SpecialOrderOperatorMUDID],[SpecialOrderOperateDate],[IsOrderUpload]
                                ,[IsPushOne],[IsPushTwo],[IsTransfer],[TransferOperatorMUDID],[TransferOperatorName],[TransferUserMUDID],[TransferUserName],[TransferOperateDate],[IsMealSame],[SupplierSpecialRemark],[IsCompleteDelivery]
                                ,[SupplierConfirmAmount],[GSKConfirmAmount],[GSKConAAReason],[MealPaymentAmount],[MealPaymentPO],[AccountingTime],'1'   FROM [P_ORDER] WHERE ActionState = 0;",
                    new SqlParameter[]
                    {

                    });

                if (rtnData > 0)
                {
                    sqlServerTemplate.ExecuteNonQuery(@"UPDATE P_ORDER SET ActionState = 1 WHERE ActionState = 0;",
                    new SqlParameter[]
                    {

                    });
                }

                return rtnData;
            }
        }
        #endregion

        #region 费用分析前台-向上订单分析
        public List<P_Order_Count_Amount> LoadUpOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            string sqlString = string.Empty;
            try
            {
                List<P_Order_Count_Amount> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;

                if(position.ToUpper() == "RM")
                {
                    sqlString = " SELECT CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice "
                        + " WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount,B.CostCenter AS OwnTerritory,B.RDTerritoryCode AS BelongTerritory "
                        + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( SELECT * FROM dbo.P_PreApproval_COST WHERE RDTerritoryCode = ( "
                        + " SELECT TOP 1 TERRITORY_RD FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_RM = '" + territoryCode + "')) B"
                        + " ON A.CN = B.HTCode WHERE A.State NOT IN (5,11) ";
                }

                if(position.ToUpper() == "RD")
                {
                    sqlString = " SELECT CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice "
                        + " WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount, B.RDTerritoryCode AS OwnTerritory, B.TA AS BelongTerritory "
                        + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( SELECT * FROM dbo.P_PreApproval_COST WHERE TA = (  "
                        + " SELECT TOP 1 TERRITORY_TA FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_RD = '" + territoryCode + "')) B"
                        + " ON A.CN = B.HTCode WHERE A.State NOT IN (5,11) ";
                }

                if (position.ToUpper() == "TA")
                {
                    sqlString = " SELECT CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice "
                        + " WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount, B.TA AS OwnTerritory, B.BUName AS BelongTerritory "
                        + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( SELECT * FROM dbo.P_PreApproval_COST P_PreApproval_COST LEFT JOIN (SELECT TA.TerritoryTA,BU.BUName FROM P_TAINFO TA INNER JOIN P_BUINFO BU ON TA.BUID = BU.ID) C  "
                        + " ON P_PreApproval_COST.TA = C.TerritoryTA  WHERE TA IN ( SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = (SELECT BUID FROM dbo.P_TAINFO WHERE TerritoryTA = '" + territoryCode + "' ))) B"
                        + " ON A.CN = B.HTCode WHERE A.State NOT IN (5,11) ";
                }

                if(position.ToUpper() == "BU")
                {
                    sqlString = " SELECT CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice "
                        + " WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount, B.TA AS OwnTerritory, B.BUName AS BelongTerritory "
                        + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( SELECT * FROM dbo.P_PreApproval_COST P_PreApproval_COST LEFT JOIN (SELECT TA.TerritoryTA,BU.BUName FROM P_TAINFO TA INNER JOIN P_BUINFO BU ON TA.BUID = BU.ID) C  "
                        + " ON P_PreApproval_COST.TA = C.TerritoryTA  WHERE TA IN ( SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' ))) B"
                        + " ON A.CN = B.HTCode WHERE A.State NOT IN (5,11) ";
                }
                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_Order_Count_Amount>(sqlString, new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@StartDate", begin),
                        SqlParameterFactory.GetSqlParameter("@EndDate", end)
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
        #endregion

        #region 费用分析前台-向下订单分析
        public List<P_Order_By_State> LoadDownOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            string sqlString = string.Empty;
            try
            {
                List<P_Order_By_State> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;

                if (position.ToUpper() == "RM")
                {
                    sqlString = " SELECT B.TERRITORY_DM AS DMTerritoryCode,B.DMName,B.TERRITORY_MR AS MRTerritoryCode,B.MRName, "
                            + " CASE WHEN (A.State IN (3,4,6,7,8,9,10) AND A.IsRetuen <> 3) THEN N'预定成功' WHEN A.State = 5 THEN N'预定失败' WHEN A.State = 11 THEN N'退单成功' WHEN A.State = 12 OR A.IsRetuen = 3 THEN '退单失败' END AS OrderState, "
                            + " CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount "
                            + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( "
                            + " SELECT E.HTCode, F.TERRITORY_DM,F.DMName,G.TERRITORY_MR,G.MRName FROM dbo.P_PreApproval_COST E "
                            + " INNER JOIN ( "
                            + " SELECT DISTINCT TERRITORY_DM,MUD_ID_DM,CASE WHEN M.MUD_ID_DM IS NULL OR M.MUD_ID_DM ='' THEN '空岗' ELSE N.Name END AS DMName FROM " + _dbName + ".dbo.Territory_Hospital M"
                            + " LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_DM = N.UserId WHERE TERRITORY_RM = '" + territoryCode + "') F ON E.DMTerritoryCode = F.TERRITORY_DM"
                            + " INNER JOIN ( "
                            + " SELECT DISTINCT M.TERRITORY_MR,M.MUD_ID_MR,CASE WHEN M.MUD_ID_MR IS NULL OR M.MUD_ID_MR ='' THEN '空岗' ELSE N.Name END AS MRName FROM " + _dbName + ".dbo.Territory_Hospital M"
                            + " LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_MR = N.UserId WHERE TERRITORY_RM = '" + territoryCode + "' ) G ON E.MRTerritoryCode = G.TERRITORY_MR"
                            + " ) B ON A.CN = B.HTCode ";
                }

                if (position.ToUpper() == "BU")
                {
                    sqlString = " SELECT B.TerritoryTA AS DMTerritoryCode,B.TerritoryHeadName AS DMName,B.TERRITORY_RD AS MRTerritoryCode,B.RDName AS MRName, "
                            + " CASE WHEN (A.State IN (3,4,6,7,8,9,10) AND A.IsRetuen <> 3) THEN N'预定成功' WHEN A.State = 5 THEN N'预定失败' WHEN A.State = 11 THEN N'退单成功' WHEN A.State = 12 OR A.IsRetuen = 3 THEN '退单失败' END AS OrderState, "
                            + " CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount "
                            + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( "
                            + " SELECT E.HTCode,F.TerritoryTA,F.TerritoryHeadName,G.TERRITORY_RD,G.RDName FROM dbo.P_PreApproval_COST E "
                            + " INNER JOIN ( SELECT TerritoryTA,TerritoryHeadName FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) F ON E.TA = F.TerritoryTA"
                            + " INNER JOIN ( SELECT DISTINCT M.TERRITORY_RD,M.MUD_ID_RD,CASE WHEN M.MUD_ID_RD IS NULL OR M.MUD_ID_RD = '' THEN '空岗' ELSE N.Name END AS RDName FROM ( "
                            + " SELECT DISTINCT TERRITORY_RD,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_TA IN( "
                            + " SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) ) M"
                            + " LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_RD = N.UserId ) G ON E.RDTerritoryCode = G.TERRITORY_RD"
                            + " ) B ON A.CN = B.HTCode ";
                }

                if (position.ToUpper() == "RD")
                {
                    sqlString = " SELECT B.TERRITORY_RM AS DMTerritoryCode,B.DMName,"
                            + " CASE WHEN (A.State IN (3,4,6,7,8,9,10) AND A.IsRetuen <> 3) THEN N'预定成功' WHEN A.State = 5 THEN N'预定失败' WHEN A.State = 11 THEN N'退单成功' WHEN A.State = 12 OR A.IsRetuen = 3 THEN '退单失败' END AS OrderState, "
                            + " CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount "
                            + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( "
                            + " SELECT E.HTCode, F.TERRITORY_RM,F.DMName FROM dbo.P_PreApproval_COST E"
                            + " INNER JOIN ( "
                            + " SELECT DISTINCT M.TERRITORY_RM, M.MUD_ID_RM, CASE WHEN M.MUD_ID_RM IS NULL OR M.MUD_ID_RM ='' THEN '空岗' ELSE N.Name END AS DMName FROM " + _dbName + ".dbo.Territory_Hospital M"
                            + " LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_RM = N.UserId WHERE TERRITORY_RD = '" + territoryCode + "') F ON E.CostCenter = F.TERRITORY_RM"
                            + " ) B ON A.CN = B.HTCode ";
                }

                if (position.ToUpper() == "TA")
                {
                    sqlString = " SELECT B.TERRITORY_RD AS DMTerritoryCode,B.DMName,"
                            + " CASE WHEN (A.State IN (3,4,6,7,8,9,10) AND A.IsRetuen <> 3) THEN N'预定成功' WHEN A.State = 5 THEN N'预定失败' WHEN A.State = 11 THEN N'退单成功' WHEN A.State = 12 OR A.IsRetuen = 3 THEN '退单失败' END AS OrderState, "
                            + " CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount "
                            + " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( "
                            + " SELECT E.HTCode, F.TERRITORY_RD,F.DMName FROM dbo.P_PreApproval_COST E"
                            + " INNER JOIN ( "
                            + " SELECT DISTINCT M.TERRITORY_RD, M.MUD_ID_RD, CASE WHEN M.MUD_ID_RD IS NULL OR M.MUD_ID_RD ='' THEN '空岗' ELSE N.Name END AS DMName FROM " + _dbName + ".dbo.Territory_Hospital M"
                            + " LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_RD = N.UserId WHERE TERRITORY_TA = '" + territoryCode + "') F ON E.RDTerritoryCode = F.TERRITORY_RD"
                            + " ) B ON A.CN = B.HTCode ";
                }

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_Order_By_State>(sqlString, new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@StartDate", begin),
                        SqlParameterFactory.GetSqlParameter("@EndDate", end)
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
        #endregion

        #region 获取订单统计信息
        public V_COST_SUMMARY GetOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            string Territory = string.Join("','", TerritoryStr);
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            V_COST_SUMMARY rtnData = new V_COST_SUMMARY();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtn1 = sqlServerTemplate.Find<P_COUNT>(@"SELECT COUNT(DISTINCT B.[CN]) AS Count FROM [P_PreApproval] A JOIN [P_ORDER] B ON A.HTCode = B.CN WHERE A.CostCenter IN ('" + Territory + "') AND B.DeliverTime >= @StartDate AND B.DeliverTime < @EndDate AND B.State NOT IN ('5','11');",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.OrderCount = rtn1.Count.ToString("N0");

                var rtn2 = sqlServerTemplate.Find<P_SUM>(@"SELECT SUM(CASE WHEN B.XmsTotalPrice< 0 THEN B.TotalPrice ELSE B.XmsTotalPrice END) AS Count FROM [P_PreApproval] A JOIN [P_ORDER] B ON A.HTCode = B.CN WHERE A.CostCenter IN ('" + Territory + "') AND B.DeliverTime >= @StartDate AND B.DeliverTime < @EndDate AND B.State NOT IN ('5','11');",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.RealPrice = rtn2.Count.ToString("N2");
            }


            return rtnData;
        }
        #endregion

        #region 获取特殊订单统计信息
        public V_COST_SUMMARY GetSpecialOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            string Territory = string.Join("','", TerritoryStr);
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            V_COST_SUMMARY rtnData = new V_COST_SUMMARY();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtn1 = sqlServerTemplate.Find<P_COUNT>(@"SELECT COUNT(DISTINCT B.[USERID]) AS Count FROM [P_PreApproval] A JOIN [P_ORDER] B ON A.HTCode = B.CN JOIN [P_ORDER_XMS_REPORT] C ON B.XmsOrderId = C.XmsOrderId
      WHERE A.CostCenter IN ('" + Territory + "') AND B.DeliverTime >= @StartDate AND B.DeliverTime < @EndDate AND B.State NOT IN ('5','11') AND (C.TYDBTYYYTYCTDRDC = '是' OR B.IsRetuen IN('3', '4', '5', '6') OR (B.IsSpecialOrder = 1 AND B.SpecialOrderReason = '会议支持文件丢失')); ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.SpecialOrderApplierCount = rtn1.Count.ToString("N0");

                var rtn2 = sqlServerTemplate.Find<P_COUNT>(@"SELECT COUNT(DISTINCT B.[CN]) AS Count FROM [P_PreApproval] A JOIN [P_ORDER] B ON A.HTCode = B.CN JOIN [P_ORDER_XMS_REPORT] C ON B.XmsOrderId = C.XmsOrderId
      WHERE A.CostCenter IN ('" + Territory + "') AND B.DeliverTime >= @StartDate AND B.DeliverTime < @EndDate AND B.State NOT IN ('5','11') AND (C.TYDBTYYYTYCTDRDC = '是' OR B.IsRetuen IN('3', '4', '5', '6') OR (B.IsSpecialOrder = 1 AND B.SpecialOrderReason = '会议支持文件丢失')); ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.SpecialOrderCount = rtn2.Count.ToString("N0");
            }

            return rtnData;
        }
        #endregion

        #region 获取未完成订单统计信息
        public V_COST_SUMMARY GetUnfinishedOrderList(List<string> TerritoryStr, string StartDate, string EndDate)
        {
            string Territory = string.Join("','", TerritoryStr);
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            V_COST_SUMMARY rtnData = new V_COST_SUMMARY();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var rtn1 = sqlServerTemplate.Find<P_COUNT>(@"SELECT COUNT(DISTINCT B.[USERID]) AS Count FROM [P_PreApproval] A JOIN [P_ORDER] B ON A.HTCode = B.CN LEFT JOIN [dbo].[P_PreUploadOrder] C ON B.CN = C.HTCode
      WHERE A.CostCenter IN ('" + Territory + "') AND B.DeliverTime >= '"+ StartDate + "' AND B.DeliverTime < '" + EndDate + "'  AND B.State NOT IN ('3','5','11') AND (C.STATE IS NULL OR C.STATE <> 4); ",
                    new SqlParameter[]
                    {
                        //SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        //SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.UnfinishedOrderApplierCount = rtn1.Count.ToString("N0");

                var rtn2 = sqlServerTemplate.Find<P_COUNT>(@"SELECT COUNT(DISTINCT B.[CN]) AS Count FROM [P_PreApproval] A JOIN [P_ORDER] B ON A.HTCode = B.CN LEFT JOIN [dbo].[P_PreUploadOrder] C ON B.CN = C.HTCode
      WHERE A.CostCenter IN ('" + Territory + "') AND B.DeliverTime >= '" + StartDate + "' AND B.DeliverTime < '" + EndDate + "' AND B.State NOT IN ('3','5','11') AND (C.STATE IS NULL OR C.STATE <> 4); ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@StartDate", StartDate),
                        SqlParameterFactory.GetSqlParameter("@EndDate", EndDate)
                    });
                rtnData.UnfinishedOrderCount = rtn2.Count.ToString("N0");
            }

            return rtnData;
        }
        #endregion

        #region 有效预申请/订单分析
        /// <summary>
        /// 订单数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="position"></param>
        /// <param name="territoryCode"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<P_PreOrder_Order> LoadPreOrderAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            string sqlString = string.Empty;
            try
            {
                List<P_PreOrder_Order> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;

                if (position.ToUpper() == "RM")
                {
                    sqlString = " SELECT A.TERRITORY_DM AS DMTerritoryCode,A.DMName AS DMName,D.MRTerritoryCode AS MRTerritoryCode,C.MRName AS MRName,D.OrderAmount AS OrderAmount,D.ID FROM "
                        + " ( SELECT DISTINCT X.TERRITORY_DM, CASE WHEN X.MUD_ID_DM IS NULL OR X.MUD_ID_DM = '' THEN '空岗' ELSE Y.Name END AS DMName FROM " + _dbName + ".dbo.Territory_Hospital X LEFT JOIN "
                        + " dbo.WP_QYUSER Y ON X.MUD_ID_DM = Y.UserId WHERE X.TERRITORY_RM = '" + territoryCode + "' ) A "
                        + " LEFT JOIN ( "
                        + " SELECT E.ID,F.DMTerritoryCode, F.MRTerritoryCode, CASE WHEN E.GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice < 0 THEN E.TotalPrice WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice >= 0 THEN E.XmsTotalPrice "
                        + " END AS OrderAmount FROM dbo.P_ORDER_COST E INNER JOIN dbo.P_PreApproval_COST F ON E.CN = F.HTCode WHERE E.State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate "
                        + " ) D ON A.TERRITORY_DM = D.DMTerritoryCode"
                        + " LEFT JOIN ( "
                        + " SELECT DISTINCT M.TERRITORY_MR, CASE WHEN M.MUD_ID_MR IS NULL OR M.MUD_ID_MR = '' THEN '空岗' ELSE N.Name END AS MRName FROM ( SELECT DISTINCT TERRITORY_MR, MUD_ID_MR FROM " + _dbName + ".dbo.Territory_Hospital "
                        + " WHERE TERRITORY_DM IN ( SELECT DISTINCT TERRITORY_DM FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_RM = '" + territoryCode + "' )) M LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_MR = N.UserId "
                        + " ) C ON D.MRTerritoryCode = C.TERRITORY_MR ";
                }

                if (position.ToUpper() == "BU")
                {
                    sqlString = " SELECT TerritoryTA AS DMTerritoryCode,TerritoryHeadName AS DMName,D.RDTerritoryCode AS MRTerritoryCode,C.RDName AS MRName,D.OrderAmount AS OrderAmount,D.ID FROM "
                        + " ( SELECT TerritoryTA,TerritoryHeadName FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) A "
                        + " LEFT JOIN ( "
                        + " SELECT E.ID, E.TA,F.RDTerritoryCode, CASE WHEN E.GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice < 0 THEN E.TotalPrice WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice >= 0 THEN E.XmsTotalPrice "
                        + " END AS OrderAmount FROM dbo.P_ORDER_COST E INNER JOIN dbo.P_PreApproval_COST F ON E.CN = F.HTCode WHERE E.State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate "
                        + " ) D ON A.TerritoryTA = D.TA"
                        + " LEFT JOIN ( "
                        + " SELECT DISTINCT M.TERRITORY_RD, CASE WHEN M.MUD_ID_RD IS NULL OR M.MUD_ID_RD = '' THEN '空岗' ELSE N.Name END AS RDName FROM ( SELECT DISTINCT TERRITORY_RD, MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital "
                        + " WHERE TERRITORY_TA IN ( SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' ))) M LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_RD = N.UserId "
                        + " ) C ON D.RDTerritoryCode = C.TERRITORY_RD ";

                            //+ " CASE WHEN A.GSKConfirmAmount is not null THEN GSKConfirmAmount WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice < 0 THEN A.TotalPrice WHEN A.GSKConfirmAmount is null and A.XmsTotalPrice >= 0 THEN A.XmsTotalPrice END AS OrderAmount "
                            //+ " FROM ( SELECT * FROM dbo.P_ORDER_COST WHERE State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate ) A INNER JOIN ( "
                            //+ " SELECT E.HTCode,F.TerritoryTA,F.TerritoryHeadName,G.TERRITORY_RD,G.RDName FROM dbo.P_PreApproval_COST E "
                            //+ " INNER JOIN ( SELECT TerritoryTA,TerritoryHeadName FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) F ON E.TA = F.TerritoryTA"
                            //+ " INNER JOIN ( SELECT DISTINCT M.TERRITORY_RD,M.MUD_ID_RD,CASE WHEN M.MUD_ID_RD IS NULL OR M.MUD_ID_RD = '' THEN '空岗' ELSE N.Name END AS RDName FROM ( "
                            //+ " SELECT DISTINCT TERRITORY_RD,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_TA IN( "
                            //+ " SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) ) M"
                            //+ " LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_RD = N.UserId ) G ON E.RDTerritoryCode = G.TERRITORY_RD"
                            //+ " ) B ON A.CN = B.HTCode ";
                }

                if (position.ToUpper() == "RD")
                {
                    sqlString = " SELECT A.TERRITORY_RM AS DMTerritoryCode,A.RMName AS DMName,D.OrderAmount AS OrderAmount,D.ID FROM "
                            + " ( SELECT DISTINCT X.TERRITORY_RM, CASE WHEN X.MUD_ID_RM IS NULL OR X.MUD_ID_RM = '' THEN '空岗' ELSE Y.Name END AS RMName FROM " + _dbName + ".dbo.Territory_Hospital X LEFT JOIN  "
                            + " dbo.WP_QYUSER Y ON X.MUD_ID_RM = Y.UserId WHERE X.TERRITORY_RD = '" + territoryCode + "' ) A "
                            + " LEFT JOIN ( SELECT E.ID,F.CostCenter, CASE WHEN E.GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice < 0 THEN E.TotalPrice "
                            + " WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice >= 0 THEN E.XmsTotalPrice END AS OrderAmount FROM dbo.P_ORDER_COST E INNER JOIN dbo.P_PreApproval_COST F "
                            + " ON E.CN = F.HTCode WHERE E.State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate ) D ON A.TERRITORY_RM = D.CostCenter ";
                }

                if (position.ToUpper() == "TA")
                {
                    sqlString = " SELECT A.TERRITORY_TA AS DMTerritoryCode,A.TAName AS DMName,D.OrderAmount AS OrderAmount,D.ID FROM "
                            + " ( SELECT DISTINCT X.TERRITORY_RM, CASE WHEN X.MUD_ID_RM IS NULL OR X.MUD_ID_RM = '' THEN '空岗' ELSE Y.Name END AS RMName FROM " + _dbName + ".dbo.Territory_Hospital X LEFT JOIN  "
                            + " dbo.WP_QYUSER Y ON X.MUD_ID_RM = Y.UserId WHERE X.TERRITORY_RD = '" + territoryCode + "' ) A "
                            + " LEFT JOIN ( SELECT E.ID,E.TA, CASE WHEN E.GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice < 0 THEN E.TotalPrice "
                            + " WHEN E.GSKConfirmAmount IS NULL AND E.XmsTotalPrice >= 0 THEN E.XmsTotalPrice END AS OrderAmount FROM dbo.P_ORDER_COST E INNER JOIN dbo.P_PreApproval_COST F  "
                            + " ON E.CN = F.HTCode WHERE E.State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate ) D ON A.TERRITORY_TA = D.TA  ";
                }

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreOrder_Order>(sqlString, new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@StartDate", begin),
                        SqlParameterFactory.GetSqlParameter("@EndDate", end)
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

        public List<P_PreOrder_PreApproval> LoadPreAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            string sqlString = string.Empty;
            try
            {
                List<P_PreOrder_PreApproval> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;

                if (position.ToUpper() == "RM")
                {
                    sqlString = " SELECT A.TERRITORY_DM AS DMTerritoryCode,A.DMName AS DMName,B.MRTerritoryCode AS MRTerritoryCode,C.MRName AS MRName,B.BudgetTotal AS PreAmount,B.ID FROM "
                        + " ( SELECT DISTINCT X.TERRITORY_DM, CASE WHEN X.MUD_ID_DM IS NULL OR X.MUD_ID_DM = '' THEN '空岗' ELSE Y.Name END AS DMName FROM " + _dbName + ".dbo.Territory_Hospital X LEFT JOIN "
                        + " dbo.WP_QYUSER Y ON X.MUD_ID_DM = Y.UserId WHERE X.TERRITORY_RM = '" + territoryCode + "' ) A "
                        + " LEFT JOIN ( "
                        + " SELECT ID,BudgetTotal,DMTerritoryCode,MRTerritoryCode FROM dbo.P_PreApproval_COST WHERE BudgetTotal > 0 AND State IN ( 5, 6, 9 ) AND MeetingDate >= @StartDate AND MeetingDate < @EndDate "
                        + " ) B ON A.TERRITORY_DM = B.DMTerritoryCode"
                        + " LEFT JOIN ( "
                        + " SELECT DISTINCT M.TERRITORY_MR, CASE WHEN M.MUD_ID_MR IS NULL OR M.MUD_ID_MR = '' THEN '空岗' ELSE N.Name END AS MRName FROM ( SELECT DISTINCT TERRITORY_MR, MUD_ID_MR FROM " + _dbName + ".dbo.Territory_Hospital "
                        + " WHERE TERRITORY_DM IN ( SELECT DISTINCT TERRITORY_DM FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_RM = '" + territoryCode + "' )) M LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_MR = N.UserId "
                        + " ) C ON B.MRTerritoryCode = C.TERRITORY_MR ";
                }

                if (position.ToUpper() == "BU")
                {
                    sqlString = " SELECT TerritoryTA AS DMTerritoryCode,TerritoryHeadName AS DMName,B.RDTerritoryCode AS MRTerritoryCode,C.RDName AS MRName,B.BudgetTotal AS PreAmount,B.ID FROM "
                        + " ( SELECT TerritoryTA,TerritoryHeadName FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) A "
                        + " LEFT JOIN ( "
                        + " SELECT ID, BudgetTotal,TA,RDTerritoryCode FROM dbo.P_PreApproval_COST WHERE BudgetTotal>0 AND State IN (5,6,9) AND MeetingDate >= @StartDate AND MeetingDate < @EndDate "
                        + " ) B ON A.TerritoryTA = B.TA"
                        + " LEFT JOIN ( "
                        + " SELECT DISTINCT M.TERRITORY_RD, CASE WHEN M.MUD_ID_RD IS NULL OR M.MUD_ID_RD = '' THEN '空岗' ELSE N.Name END AS RDName FROM ( SELECT DISTINCT TERRITORY_RD, MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital "
                        + " WHERE TERRITORY_TA IN ( SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = 'RES' ))) M LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_RD = N.UserId "
                        + " ) C ON B.RDTerritoryCode = C.TERRITORY_RD ";

                    //" SELECT F.TerritoryTA AS DMTerritoryCode,F.TerritoryHeadName AS DMName,G.TERRITORY_RD AS MRTerritoryCode, G.RDName AS MRName,E.BudgetTotal AS PreAmount "
                    //        + " FROM dbo.P_PreApproval_COST E  "
                    //        + " INNER JOIN ( SELECT TerritoryTA,TerritoryHeadName FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) F ON E.TA = F.TerritoryTA"
                    //        + " INNER JOIN ( SELECT DISTINCT M.TERRITORY_RD,M.MUD_ID_RD,CASE WHEN M.MUD_ID_RD IS NULL OR M.MUD_ID_RD = '' THEN '空岗' ELSE N.Name END AS RDName FROM ( "
                    //        + " SELECT DISTINCT TERRITORY_RD,MUD_ID_RD FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_TA IN( "
                    //        + " SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' )) ) M"
                    //        + " LEFT JOIN dbo.WP_QYUSER N ON M.MUD_ID_RD = N.UserId ) G ON E.RDTerritoryCode = G.TERRITORY_RD"
                    //        + " WHERE E.State IN (5,6,9) AND E.BudgetTotal>0 AND E.MeetingDate >= @StartDate AND E.MeetingDate < @EndDate ";
                }

                if (position.ToUpper() == "RD")
                {
                    sqlString = " SELECT A.TERRITORY_RM AS DMTerritoryCode,A.RMName AS DMName,B.BudgetTotal AS PreAmount,B.ID FROM "
                            + " ( SELECT DISTINCT X.TERRITORY_RM, CASE WHEN X.MUD_ID_RM IS NULL OR X.MUD_ID_RM = '' THEN '空岗' ELSE Y.Name END AS RMName FROM " + _dbName + ".dbo.Territory_Hospital X LEFT JOIN  "
                            + " dbo.WP_QYUSER Y ON X.MUD_ID_RM = Y.UserId WHERE X.TERRITORY_RD = '" + territoryCode + "' ) A "
                            + " LEFT JOIN ( SELECT ID,BudgetTotal,CostCenter FROM dbo.P_PreApproval_COST WHERE BudgetTotal > 0 AND State IN ( 5, 6, 9 ) AND MeetingDate >= @StartDate AND MeetingDate < @EndDate "
                            + " ) B ON A.TERRITORY_RM = B.CostCenter ";
                }

                if (position.ToUpper() == "TA")
                {
                    sqlString = " SELECT A.TERRITORY_TA AS DMTerritoryCode,A.TAName AS DMName,B.BudgetTotal AS PreAmount,B.ID FROM "
                            + " ( SELECT DISTINCT X.TERRITORY_TA, CASE WHEN Y.TerritoryHeadName IS NULL OR Y.TerritoryHeadName = '' THEN '空岗' ELSE Y.TerritoryHeadName END AS TAName FROM " + _dbName + ".dbo.Territory_Hospital X LEFT JOIN  "
                            + " dbo.P_TAINFO Y ON X.TERRITORY_TA = Y.TerritoryTA WHERE X.TERRITORY_TA = '" + territoryCode + "' ) A "
                            + " LEFT JOIN ( SELECT ID,BudgetTotal,TA FROM dbo.P_PreApproval_COST WHERE BudgetTotal > 0 AND State IN ( 5, 6, 9 ) AND MeetingDate >= @StartDate AND MeetingDate < @EndDate "
                            + " ) B ON A.TERRITORY_RM = B.TA ";
                }

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreOrder_PreApproval>(sqlString, new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@StartDate", begin),
                        SqlParameterFactory.GetSqlParameter("@EndDate", end)
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

        public List<P_PreOrder_Hospital_View> LoadHospitalAnalysisData(string userId, string position, string territoryCode, string begin, string end)
        {
            string sqlString = string.Empty;
            try
            {
                List<P_PreOrder_Hospital_View> rtnData;
                var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
                string conditionstartDate = string.Empty;
                string conditionendDate = string.Empty;

                if (position.ToUpper() == "RM")
                {
                    sqlString = " SELECT A.HospitalCode,B.Name AS HospitalName,C.HospitalId AS HosOrder,C.OrderCount,C.OrderAmount,D.PreCount,D.PreAmount,D.HospitalCode AS HosPre FROM ( "
                            + " SELECT DISTINCT HospitalCode FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_RM = '" + territoryCode + "' ) A   "
                            + " LEFT JOIN (SELECT * FROM " + _dbName + ".dbo.P_HOSPITAL WHERE MainAddress = N'主地址') B ON A.HospitalCode = B.GskHospital "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalId,'-OH','') AS HospitalId,COUNT(ID) AS OrderCount, SUM(CASE WHEN GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice < 0 THEN TotalPrice "
                            + " WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice >= 0 THEN XmsTotalPrice END ) AS OrderAmount FROM dbo.P_ORDER_COST WHERE State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate "
                            + " GROUP BY REPLACE(HospitalId,'-OH','') "
                            + " ) C ON A.HospitalCode = c.HospitalId COLLATE Chinese_PRC_CI_AS "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalCode,'-OH','') AS HospitalCode,COUNT(ID) AS PreCount, SUM(BudgetTotal) AS PreAmount FROM dbo.P_PreApproval_COST "
                            + " WHERE State IN (5,6,9) AND BudgetTotal>0 AND MeetingDate >= @StartDate AND MeetingDate < @EndDate GROUP BY REPLACE(HospitalCode,'-OH','') "
                            + " ) D ON D.HospitalCode = A.HospitalCode ORDER BY C.OrderAmount DESC ";
                }

                if (position.ToUpper() == "BU")
                {
                    sqlString = " SELECT A.HospitalCode,B.Name AS HospitalName,C.HospitalId AS HosOrder,C.OrderCount,C.OrderAmount,D.PreCount,D.PreAmount,D.HospitalCode AS HosPre FROM ( SELECT DISTINCT HospitalCode FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_TA IN(  "
                            + " SELECT TerritoryTA FROM dbo.P_TAINFO WHERE BUID = ( SELECT ID FROM dbo.P_BUINFO WHERE BUName = '" + territoryCode + "' ))) A  "
                            + " LEFT JOIN (SELECT * FROM " + _dbName + ".dbo.P_HOSPITAL WHERE MainAddress = N'主地址') B ON A.HospitalCode = B.GskHospital "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalId,'-OH','') AS HospitalId,COUNT(ID) AS OrderCount, SUM(CASE WHEN GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice < 0 THEN TotalPrice "
                            + " WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice >= 0 THEN XmsTotalPrice END ) AS OrderAmount FROM dbo.P_ORDER_COST WHERE State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate "
                            + " GROUP BY REPLACE(HospitalId,'-OH','') "
                            + " ) C ON A.HospitalCode = c.HospitalId COLLATE Chinese_PRC_CI_AS "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalCode,'-OH','') AS HospitalCode,COUNT(ID) AS PreCount, SUM(BudgetTotal) AS PreAmount FROM dbo.P_PreApproval_COST "
                            + " WHERE State IN (5,6,9) AND BudgetTotal>0 AND MeetingDate >= @StartDate AND MeetingDate < @EndDate GROUP BY REPLACE(HospitalCode,'-OH','') "
                            + " ) D ON D.HospitalCode = A.HospitalCode ORDER BY C.OrderAmount DESC ";
                }

                if (position.ToUpper() == "RD")
                {
                    sqlString = " SELECT A.HospitalCode,B.Name AS HospitalName,C.HospitalId AS HosOrder,C.OrderCount,C.OrderAmount,D.PreCount,D.PreAmount,D.HospitalCode AS HosPre FROM ( "
                            + " SELECT DISTINCT HospitalCode FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_RD = '" + territoryCode + "' ) A   "
                            + " LEFT JOIN (SELECT * FROM " + _dbName + ".dbo.P_HOSPITAL WHERE MainAddress = N'主地址') B ON A.HospitalCode = B.GskHospital "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalId,'-OH','') AS HospitalId,COUNT(ID) AS OrderCount, SUM(CASE WHEN GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice < 0 THEN TotalPrice "
                            + " WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice >= 0 THEN XmsTotalPrice END ) AS OrderAmount FROM dbo.P_ORDER_COST WHERE State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate "
                            + " GROUP BY REPLACE(HospitalId,'-OH','') "
                            + " ) C ON A.HospitalCode = c.HospitalId COLLATE Chinese_PRC_CI_AS "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalCode,'-OH','') AS HospitalCode,COUNT(ID) AS PreCount, SUM(BudgetTotal) AS PreAmount FROM dbo.P_PreApproval_COST "
                            + " WHERE State IN (5,6,9) AND BudgetTotal>0 AND MeetingDate >= @StartDate AND MeetingDate < @EndDate GROUP BY REPLACE(HospitalCode,'-OH','') "
                            + " ) D ON D.HospitalCode = A.HospitalCode ORDER BY C.OrderAmount DESC ";
                }

                if (position.ToUpper() == "TA")
                {
                    sqlString = " SELECT A.HospitalCode,B.Name AS HospitalName,C.HospitalId AS HosOrder,C.OrderCount,C.OrderAmount,D.PreCount,D.PreAmount,D.HospitalCode AS HosPre FROM ( "
                            + " SELECT DISTINCT HospitalCode FROM " + _dbName + ".dbo.Territory_Hospital WHERE TERRITORY_TA = '" + territoryCode + "' ) A   "
                            + " LEFT JOIN (SELECT * FROM " + _dbName + ".dbo.P_HOSPITAL WHERE MainAddress = N'主地址') B ON A.HospitalCode = B.GskHospital "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalId,'-OH','') AS HospitalId,COUNT(ID) AS OrderCount, SUM(CASE WHEN GSKConfirmAmount IS NOT NULL THEN GSKConfirmAmount WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice < 0 THEN TotalPrice "
                            + " WHEN GSKConfirmAmount IS NULL AND XmsTotalPrice >= 0 THEN XmsTotalPrice END ) AS OrderAmount FROM dbo.P_ORDER_COST WHERE State NOT IN (5,11) AND DeliverTime >= @StartDate AND DeliverTime < @EndDate "
                            + " GROUP BY REPLACE(HospitalId,'-OH','') "
                            + " ) C ON A.HospitalCode = c.HospitalId COLLATE Chinese_PRC_CI_AS "
                            + " LEFT JOIN ( "
                            + " SELECT REPLACE(HospitalCode,'-OH','') AS HospitalCode,COUNT(ID) AS PreCount, SUM(BudgetTotal) AS PreAmount FROM dbo.P_PreApproval_COST "
                            + " WHERE State IN (5,6,9) AND BudgetTotal>0 AND MeetingDate >= @StartDate AND MeetingDate < @EndDate GROUP BY REPLACE(HospitalCode,'-OH','') "
                            + " ) D ON D.HospitalCode = A.HospitalCode ORDER BY C.OrderAmount DESC ";
                }

                using (var conn = sqlServerTemplate.GetSqlConnection())
                {
                    conn.Open();
                    rtnData = sqlServerTemplate.Load<P_PreOrder_Hospital_View>(sqlString, new SqlParameter[] {
                        SqlParameterFactory.GetSqlParameter("@StartDate", begin),
                        SqlParameterFactory.GetSqlParameter("@EndDate", end)
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
        #endregion
    }
}