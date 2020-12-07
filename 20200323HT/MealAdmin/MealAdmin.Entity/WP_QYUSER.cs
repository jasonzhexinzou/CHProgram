namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class WP_QYUSER
    {
        public  Guid ID { get; set; }
        public  Guid WechatID { get; set; }
        public  string Name { get; set; }
        public  string UserId { get; set; }
        public  string WechatAccount { get; set; }
        public  string PhoneNumber { get; set; }
        public  string Email { get; set; }
        public  int Gender { get; set; }
        public  string Position { get; set; }
        public  string WorkPlace { get; set; }
        public  string DeviceId { get; set; }
        public  int State { get; set; }
        public  string DeptNames { get; set; } 
        public  string headimgurl { get; set; }
        public  DateTime CreateDate { get; set; }
        public  Guid Creator { get; set; }
        public  DateTime ModifyDate { get; set; }
        public  Guid Modifier { get; set; }
        public  int JobState { get; set; }
        public  DateTime LeaveDate { get; set; }
        public  Guid DefaultCostCenterID { get; set; }
        public string LineManager { get; set; }
        public  Guid LineManagerID { get; set; }
        public  bool UserAgreement { get; set; }
        public  DateTime UserAgreementDate { get; set; }
        public  Guid UserDefaultCostCenterID { get; set; }
    }
}
