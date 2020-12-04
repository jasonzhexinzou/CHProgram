using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity
{
    public class PreApproval
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 张文帝
        /// </summary>
        public string ApplierName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApplierMUDID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApplierMobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ModifyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HTCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Market { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TA { get; set; }
        /// <summary>
        /// 辽宁省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 辽阳市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HospitalCode { get; set; }
        /// <summary>
        /// 辽宁省辽阳市中国人民解放军201医院
        /// </summary>
        public string HospitalName { get; set; }
        /// <summary>
        /// 辽阳市白塔区卫国路148号
        /// </summary>
        public string HospitalAddress { get; set; }
        /// <summary>
        /// 测试
        /// </summary>
        public string MeetingName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MeetingDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int AttendCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CostCenter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double BudgetTotal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsDMFollow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BUHeadName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BUHeadMUDID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BUHeadApproveDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsReAssign { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReAssignBUHeadName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReAssignBUHeadMUDID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReAssignBUHeadApproveDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MMCoEImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MMCoEApproveState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsBudgetChange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsMMCoEChange { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsUsed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsFinished { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsFreeSpeaker { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SpeakerServiceImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SpeakerBenefitImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReAssignOperatorName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReAssignOperatorMUDID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Position { get; set; }
    }

    public class FoodsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string foodId { get; set; }
        /// <summary>
        /// 正餐套餐一
        /// </summary>
        public string foodName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double price { get; set; }
        /// <summary>
        /// 【妙手回春】BBQ鸡柳比萨一张、藤椒鸡球一份、可乐一听、保温包一个
        /// </summary>
        public string describe { get; set; }
    }

    public class Foods
    {
        /// <summary>
        /// 
        /// </summary>
        public string resId { get; set; }
        /// <summary>
        /// city1+1城市比萨
        /// </summary>
        public string resName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string resLogo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string resAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string resTel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FoodsItem> foods { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double allPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double foodFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double packageFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double sendFee { get; set; }
    }

    public class Details
    {
        /// <summary>
        /// 
        /// </summary>
        public int attendCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deliveryAddress { get; set; }
        /// <summary>
        /// 张文帝
        /// </summary>
        public string consignee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string deliverTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string oldDeliverTime { get; set; }
        /// <summary>
        /// 测试订单
        /// </summary>
        public string remark { get; set; }
    }

    public class Hospital
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GskHospital { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OldGskHospital { get; set; }
        /// <summary>
        /// 辽宁省辽阳市中国人民解放军201医院
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OldName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FirstLetters { get; set; }
        /// <summary>
        /// 辽阳市白塔区卫国路148号
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int External { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 是
        /// </summary>
        public string IsXMS { get; set; }
        /// <summary>
        /// 是
        /// </summary>
        public string IsBDS { get; set; }
        /// <summary>
        /// 否
        /// </summary>
        public string IsMT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsDelete { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RelateUserList { get; set; }
    }

    public class FoodDetail
    {
        /// <summary>
        /// 
        /// </summary>
        public string inTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string supplier { get; set; }
        /// <summary>
        /// 葛兰素史克（中国）投资有限公司
        /// </summary>
        public string invoiceTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dutyParagraph { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PreApproval preApproval { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Foods foods { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Details details { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Hospital hospital { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string orderInfo { get; set; }
    }
}
