using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class V_UnFinishOrder
    {
        public string c1 { get; set; }                  //申请人姓名
        public string c2 { get; set; }                  //申请人MUDID
        public string c3 { get; set; }                  //申请人职位
        public string c4 { get; set; }                  //申请人手机号码
        public string c5 { get; set; }                  //HT编号
        public string c6 { get; set; }                  //Market
        public string c7 { get; set; }                  //TA
        public string c8 { get; set; }                 //省份
        public string c9 { get; set; }                 //城市
        public string c10 { get; set; }                 //医院编码
        public string c11 { get; set; }                 //医院名称
        public string c12 { get; set; }                 //成本中心
        public string c13 { get; set; }                 //供应商
        public DateTime? c14 { get; set; }                 //送餐日期
        public TimeSpan c15 { get; set; }                 //送餐时间
        public string c16 { get; set; }                 //餐厅编码
        public string c17 { get; set; }                 //餐厅名称
        public int c18 { get; set; }                 //用餐人数
        public decimal c19 { get; set; }                 //实际金额
        public string c20 { get; set; }                 //是否申请退单
        public string c21 { get; set; }                 //是否退单成功
        public string c22 { get; set; }                 //退单失败平台发起配送需求
        public string c23 { get; set; }                 //退单失败线下反馈配送需求
        public string c24 { get; set; }                 //预定/退单失败原因
        public string c25 { get; set; }                 //是否收餐/未送达
        public decimal c26 { get; set; }                 //用户确认金额
        public string c27 { get; set; }                    //是否与预定餐品一致
        public string c28 { get; set; }                 //用户确认金额调整原因
        public string c29 { get; set; }                 //用户确认金额调整备注
        public int c30 { get; set; }                 //实际用餐人数
        public string c31 { get; set; }                 //实际用餐人数调整原因
        public string c32 { get; set; }                 //实际用餐人数调整备注
        public string c33 { get; set; }                 //直线经理姓名
        public string c34 { get; set; }                 //直线经理MUDID
        public string c35 { get; set; }                 //Level2 Manager 姓名
        public string c36 { get; set; }                 //Level2 Manager MUDID
        public string c37 { get; set; }                 //Level3 Manager 姓名
        public string c38 { get; set; }                 //Level3 Manager MUDID
        public string c39 { get; set; }                 //是否上传文件
        public DateTime? c40 { get; set; }                //上传文件提交日期
        public TimeSpan c41 { get; set; }                //上传文件提交时间
        public string c42 { get; set; }                //上传文件审批直线经理姓名
        public string c43 { get; set; }                //上传文件审批直线经理MUDID
        public DateTime? c44 { get; set; }                //上传文件审批日期
        public TimeSpan c45 { get; set; }                //上传文件审批时间
        public string c46 { get; set; }                //上传文件审批状态
        public string c47 { get; set; }                //签到人数是否等于实际用餐人数
        public string c48 { get; set; }                //签到人数调整原因
        public string c49 { get; set; }                //是否与会议信息一致
        public string c50 { get; set; }                //会议信息不一致原因
        public string c51 { get; set; }                //上传文件是否Re-Open
        public string c52 { get; set; }                //上传文件Re-Open操作人
        public string c53 { get; set; }                //上传文件Re-Open操作人MUDID
        public DateTime? c54 { get; set; }                //上传文件Re-Open日期
        public TimeSpan c55 { get; set; }                //上传文件Re-Open时间
        public string c56 { get; set; }                //上传文件Re-Open发起人姓名
        public string c57 { get; set; }                //上传文件Re-Open发起人MUDID
        public string c58 { get; set; }                //上传文件Re-Open原因
        public string c59 { get; set; }                //上传文件Re-Open备注
        public string c60 { get; set; }                //上传文件Re-Open审批状态
        public string c61 { get; set; }                //上传文件是否重新分配
        public string c62 { get; set; }                //上传文件重新分配操作人
        public string c63 { get; set; }                //上传文件重新分配操作人MUDID
        public string c64 { get; set; }                //上传文件被重新分配人姓名
        public string c65 { get; set; }                //上传文件被重新分配人MUDID
        public DateTime? c66 { get; set; }                //上传文件被重新分配日期
        public TimeSpan c67 { get; set; }                //上传文件被重新分配时间
        public string c68 { get; set; }                //上传文件否重新分配审批人
        public string c69 { get; set; }                //上传文件重新分配审批人-操作人
        public string c70 { get; set; }                //上传文件重新分配审批人-操作人MUDID
        public string c71 { get; set; }                //上传文件被重新分配审批人姓名
        public string c72 { get; set; }                //上传文件被重新分配审批人MUDID
        public DateTime? c73 { get; set; }                //上传文件重新分配审批人日期
        public TimeSpan c74 { get; set; }                //上传文件重新分配审批人时间
        public string c75 { get; set; }                //项目组特殊备注
        public string c76 { get; set; }                //Workday是否离职
    }
}
