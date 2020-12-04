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

namespace MealAdmin.Dao
{
    public class EvaluateDao : IEvaluateDao
    {
        [Bean("sqlServerTemplFactory")]
        public DbTemplateFactory sqlServerTemplFactory { get; set; }

        [Bean("sqlServerTemplFactoryNonHT")]
        public DbTemplateFactory sqlServerTemplFactoryNonHT { get; set; }

        #region 新建评价
        /// <summary>
        /// 新建评价
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Add(P_EVALUATE entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var res = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_EVALUATE " +
                    " (ID,OrderID,RestaurantId,Star,OnTime,OnTimeDiscrpion,IsSafe,SafeDiscrpion,SafeImage,Health,HealthDiscrpion,HealthImage,Pack,PackDiscrpion,PackImage,CostEffective,CostEffectiveDiscrpion,CostEffectiveImage,OtherDiscrpion,OtherDiscrpionImage,State,CreateDate)" +
                    " SELECT @ID,@OrderID,@RestaurantId,@Star,@OnTime,@OnTimeDiscrpion,@IsSafe,@SafeDiscrpion,@SafeImage,@Health,@HealthDiscrpion,@HealthImage,@Pack,@PackDiscrpion,@PackImage,@CostEffective,@CostEffectiveDiscrpion,@CostEffectiveImage,@OtherDiscrpion,@OtherDiscrpionImage,@State,@CreateDate " +
                    " WHERE NOT EXISTS (SELECT * FROM P_EVALUATE WHERE OrderID=@OrderID)",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@OrderID", entity.OrderID),
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", entity.RestaurantId),
                        SqlParameterFactory.GetSqlParameter("@Star", entity.Star),
                        SqlParameterFactory.GetSqlParameter("@OnTime", entity.OnTime),
                        SqlParameterFactory.GetSqlParameter("@OnTimeDiscrpion", entity.OnTimeDiscrpion),
                        SqlParameterFactory.GetSqlParameter("@IsSafe", entity.IsSafe),
                        SqlParameterFactory.GetSqlParameter("@SafeDiscrpion", entity.SafeDiscrpion),
                        SqlParameterFactory.GetSqlParameter("@SafeImage", entity.SafeImage),
                        SqlParameterFactory.GetSqlParameter("@Health", entity.Health),
                        SqlParameterFactory.GetSqlParameter("@HealthDiscrpion", entity.HealthDiscrpion),
                        SqlParameterFactory.GetSqlParameter("@HealthImage", entity.HealthImage),
                        SqlParameterFactory.GetSqlParameter("@Pack", entity.Pack),
                        SqlParameterFactory.GetSqlParameter("@PackDiscrpion", entity.PackDiscrpion),
                        SqlParameterFactory.GetSqlParameter("@PackImage", entity.PackImage),
                        SqlParameterFactory.GetSqlParameter("@CostEffective", entity.CostEffective),
                        SqlParameterFactory.GetSqlParameter("@CostEffectiveDiscrpion", entity.CostEffectiveDiscrpion),
                        SqlParameterFactory.GetSqlParameter("@CostEffectiveImage", entity.CostEffectiveImage),
                        SqlParameterFactory.GetSqlParameter("@OtherDiscrpion", entity.OtherDiscrpion),
                        SqlParameterFactory.GetSqlParameter("@OtherDiscrpionImage", entity.OtherDiscrpionImage),
                        SqlParameterFactory.GetSqlParameter("@State", entity.State),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate)
                    });


                var _res = 0;
                if (entity.OnTime != 0)
                {
                    _res = sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [State]={OrderState.EVALUATED} WHERE ID=@ID ",
                  new SqlParameter[]
                  {
                      SqlParameterFactory.GetSqlParameter("@ID", entity.OrderID)
                  });
                }
                else
                {
                    var receiveDate = DateTime.Now;
                    _res = sqlServerTemplate.Update($"UPDATE [P_ORDER] SET [State]={OrderState.FOODLOSE}, ReceiveState={OrderState.FOODLOSE},ReceiveDate=@ReceiveDate,IsSpecialOrder=@IsSpecialOrder,RealCount=0 WHERE ID=@ID ",
                   new SqlParameter[]
                   {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.OrderID),
                        SqlParameterFactory.GetSqlParameter("@ReceiveDate", receiveDate),
                        SqlParameterFactory.GetSqlParameter("@IsSpecialOrder", entity.Normal==0?3:0)
                   });
                }

                if (res == 1 && _res == 1)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region 根据订单ID查询评价
        /// <summary>
        /// 根据订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public P_EVALUATE LoadByOrderID(Guid OrderID)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var evaluate = sqlServerTemplate.Find<P_EVALUATE>(
                    "SELECT * FROM P_EVALUATE WHERE OrderID=@OrderID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@OrderID", OrderID)
                    });
                return evaluate;
            }
        }
        #endregion

        #region 根据1.0订单ID查询评价
        /// <summary>
        /// 根据1.0订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public P_EVALUATE LoadByOldOrderID(Guid OrderID)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var evaluate = sqlServerTemplate.Find<P_EVALUATE>(
                    "SELECT * FROM P_EVALUATE WHERE OrderID=@OrderID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@OrderID", OrderID)
                    });
                return evaluate;
            }
        }
        #endregion

        #region 根据订单ID查询评价
        /// <summary>
        /// 根据订单ID查询评价
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public P_EVALUATE LoadNonHTEvaluateByOrderID(Guid OrderID)
        {
            var sqlServerTemplate = sqlServerTemplFactoryNonHT.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var evaluate = sqlServerTemplate.Find<P_EVALUATE>(
                    "SELECT * FROM P_EVALUATE WHERE OrderID=@OrderID", new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@OrderID", OrderID)
                    });
                return evaluate;
            }
        }
        #endregion

        #region 根据餐厅Id查询评价
        /// <summary>
        /// 根据餐厅Id查询评价
        /// </summary>
        /// <param name="resId"></param>
        /// <returns></returns>
        public List<P_EVALUATE> LoadByResId(string resId)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_EVALUATE>($"SELECT * FROM [P_EVALUATE] WHERE [RestaurantId]=@RestaurantId",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@RestaurantId", resId)
                    });
                return list;
            }
        }
        #endregion

        #region 载入餐厅评分
        /// <summary>
        /// 载入餐厅评分
        /// </summary>
        /// <param name="resIds"></param>
        /// <returns></returns>
        public List<P_RESTAURANT_START_VIEW> LoadStarByResIds(string[] resIds)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();
                var list = sqlServerTemplate.Load<P_RESTAURANT_START_VIEW>($@"SELECT RestaurantId, SUM(Star) AS Star, COUNT(RestaurantId) AS [Count] FROM P_EVALUATE 
                    WHERE RestaurantId IN('{string.Join("','", resIds)}') GROUP BY RestaurantId ", null);
                return list;
            }
        }
        #endregion

        #region 添加审批记录
        /// <summary>
        /// 添加审批记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddOrderApprove(P_ORDER_APPROVE entity)
        {
            var sqlServerTemplate = sqlServerTemplFactory.CreateDbTemplate();
            using (var conn = sqlServerTemplate.GetSqlConnection())
            {
                conn.Open();

                var res = sqlServerTemplate.ExecuteNonQuery("INSERT INTO P_ORDER_APPROVE " +
                    " (ID,OrderID,CN,UserId,OldState,NewState,Result,Comment,IsWXClient,CreateDate,CreateUserId)" +
                    "VALUES (@ID,@OrderID,@CN,@UserId,@OldState,@NewState,@Result,@Comment,@IsWXClient,@CreateDate,@CreateUserId) ",
                    new SqlParameter[]
                    {
                        SqlParameterFactory.GetSqlParameter("@ID", entity.ID),
                        SqlParameterFactory.GetSqlParameter("@OrderID", entity.OrderID),
                        SqlParameterFactory.GetSqlParameter("@CN", entity.CN),
                        SqlParameterFactory.GetSqlParameter("@UserId", entity.UserId),
                        SqlParameterFactory.GetSqlParameter("@OldState", entity.OldState),
                        SqlParameterFactory.GetSqlParameter("@NewState", entity.NewState),
                        SqlParameterFactory.GetSqlParameter("@Result", entity.Result),
                        SqlParameterFactory.GetSqlParameter("@Comment", entity.Comment),
                        SqlParameterFactory.GetSqlParameter("@IsWXClient", entity.IsWXClient),
                        SqlParameterFactory.GetSqlParameter("@CreateDate", entity.CreateDate),
                        SqlParameterFactory.GetSqlParameter("@CreateUserId", entity.CreateUserId)
                    });
                return res;
            }
        }
        #endregion
    }
}