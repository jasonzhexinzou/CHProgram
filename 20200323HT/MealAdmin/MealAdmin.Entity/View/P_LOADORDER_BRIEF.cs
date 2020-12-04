using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class P_LOADORDER_BRIEF
    {
        public int TodayApprove { get; set; }//当日预申请审批通过数量
        public int TodayApproveZero { get; set; }//当日预申请审批通过数量金额是0元
        public int TodayApproveNotZero { get; set; }//当日预申请审批通过数量金额不是0元

        public int TodayUpLoadThroughCount { get; set; }//当日上传文件审批通过数量

        public int TomorrowExceed2000 { get; set; }

        public int TodayFail { get; set; }//当日订单预定失败量
        public int TodayFailXms { get; set; }//当日订单预定失败量XMS
        public int TodayFailBds { get; set; }//当日订单预定失败量BDS

        public int TodayCancelSuccess { get; set; }//当日退单成功量
        public int TodayCancelSuccessXms { get; set; }//当日退单成功量XMS
        public int TodayCancelSuccessBds { get; set; }//当日退单成功量BDS


        public int TodayCancelFail { get; set; }//当日退单失败量
        public int TodayCancelFailXms { get; set; }//当日退单失败量Xms
        public int TodayCancelFailBds { get; set; }//当日退单失败量Xms

        public int TomorrowDeliver { get; set; }//明日配送订单数量
        public int TomorrowDeliverXms { get; set; }//明日配送订单数量Xms
        public int TomorrowDeliverBds { get; set; }//明日配送订单数量Bds

        public decimal TomorrowDeliverTotal { get; set; }////明日配送订单总款
        public decimal TomorrowDeliverTotalXms { get; set; }////明日配送订单总款Xms
        public decimal TomorrowDeliverTotalBds { get; set; }////明日配送订单总款Bds

        public int TomorrowAttendCount60 { get; set; }//明日配送订单:参会人数>=60
        public int TomorrowFoodCount60 { get; set; }//明日配送订单:参会人数<60, 订单份数>=60


        public int TodayConfirmOrder { get; set; }   //当日确认收餐数量
    }
}
