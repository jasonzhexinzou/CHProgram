namespace MealAdmin.Entity
{
    using System;
    using System.Collections.Generic;

    public partial class WP_QYDEPT
    {
        public  Guid ID { get; set; }
        public  Guid WechatID { get; set; }
        public  Guid ParentID { get; set; }
        public  string NamePath { get; set; }
        public  string MasterId { get; set; }
        public  string Name { get; set; }
        public  int WxDeptId { get; set; }
        public  DateTime CreateDate { get; set; }
        public  Guid Creator { get; set; }
        public  DateTime ModifyDate { get; set; }
        public  Guid Modifier { get; set; }
    }
}
