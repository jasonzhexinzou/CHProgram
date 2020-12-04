using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Entity.View
{
    public class HT_Order_Report
    {
        public string c0 { get; set; }                  //申请人姓名
        public string c1 { get; set; }                  //申请人MUDID
        public string c132 { get; set; }                  //申请人职位
        public string c2 { get; set; }                  //申请人手机号码
        public DateTime c3 { get; set; }                  //预申请申请日期
        public TimeSpan c4 { get; set; }                  //预申请申请时间
        public DateTime c5 { get; set; }                  //预申请修改日期
        public TimeSpan c6 { get; set; }                  //预申请修改时间
        public string c7 { get; set; }                  //HT编号
        public string c8 { get; set; }                  //Market
        public string c143 { get; set; }                //VeevaMeetingID
        public string c9 { get; set; }                  //TA
        public string c10 { get; set; }                 //省份
        public string c11 { get; set; }                 //城市
        public string c12 { get; set; }                 //医院编码
        public string c13 { get; set; }                 //医院名称
        public string c14 { get; set; }                 //医院地址
        public string c15 { get; set; }                 //会议名称
        public DateTime c16 { get; set; }                 //会议日期
        public TimeSpan c17 { get; set; }                 //会议时间
        public int c18 { get; set; }                 //参会人数
        public string c19 { get; set; }                 //成本中心
        public decimal c20 { get; set; }                 //预算金额
        public string c21 { get; set; }                 //直线经理是否随访
        public string c22 { get; set; }                 //是否由外部免费讲者来讲
        public string c23 { get; set; }                 //预申请审批人姓名
        public string c24 { get; set; }                 //预申请审批人MUDID
        public DateTime c25 { get; set; }                 //预申请审批日期
        public TimeSpan c26 { get; set; }                 //预申请审批时间
        public string c27 { get; set; }                 //预申请审批状态
        public string c28 { get; set; }                 //预申请是否重新分配审批人
        public string c29 { get; set; }                 //预申请重新分配审批人-操作人
        public string c30 { get; set; }                 //预申请重新分配审批人-操作人MUDID
        public string c31 { get; set; }                 //预申请被重新分配审批人姓名
        public string c32 { get; set; }                 //预申请被重新分配审批人MUDID
        public DateTime c33 { get; set; }                 //预申请重新分配审批人日期
        public TimeSpan c34 { get; set; }                 //预申请重新分配审批人时间
        public string c35 { get; set; }                 //供应商
        public string c36 { get; set; }                 //订单号
        public DateTime c37 { get; set; }                 //下单日期
        public TimeSpan c38 { get; set; }                 //下单时间
        public DateTime c39 { get; set; }                 //送餐日期
        public TimeSpan c40 { get; set; }                 //送餐时间
        public string c41 { get; set; }                 //餐厅编码
        public string c42 { get; set; }                 //预订餐厅
        public int c43 { get; set; }                 //用餐人数
        public int c44 { get; set; }                 //总份数
        public decimal c45 { get; set; }                 //预订金额
        public decimal c46 { get; set; }                 //实际金额
        public string c47 { get; set; }                 //金额调整原因
        public string c48 { get; set; }                 //送餐地址
        public string c49 { get; set; }                 //收餐人姓名
        public string c50 { get; set; }                 //收餐人电话
        public string c51 { get; set; }                 //下单备注
        public string c52 { get; set; }                 //是否预定成功
        public DateTime c53 { get; set; }                 //预定成功日期
        public TimeSpan c54 { get; set; }                 //预定成功时间
        public string c55 { get; set; }                 //是否申请退单
        public string c56 { get; set; }                 //是否退单成功
        public string c57 { get; set; }                 //退单失败平台发起配送需求
        public string c58 { get; set; }                 //退单失败线下反馈配送需求
        public string c59 { get; set; }                 //预定/退单失败原因
        public string c60 { get; set; }                 //是否收餐/未送达
        public DateTime c61 { get; set; }                 //确认收餐日期
        public TimeSpan c62 { get; set; }                 //确认收餐时间
        public decimal c63 { get; set; }                 //用户确认金额
        public string c64 { get; set; }                 //用户确认金额调整原因
        public string c65 { get; set; }                 //用户确认金额调整备注
        public int c66 { get; set; }                 //实际用餐人数
        public string c67 { get; set; }                 //实际用餐人数调整原因
        public string c68 { get; set; }                 //实际用餐人数调整备注
        public string c69 { get; set; }                 //未送达描述
        public string c70 { get; set; }                 //准点率
        public string c71 { get; set; }                 //准点率描述
        public string c72 { get; set; }                 //食品安全存在问题
        public string c73 { get; set; }                 //食品安全问题描述
        public string c74 { get; set; }                 //餐品卫生及新鲜
        public string c75 { get; set; }                 //餐品卫生描述
        public string c76 { get; set; }                 //餐品包装
        public string c77 { get; set; }                 //餐品包装描述
        public string c78 { get; set; }                 //餐品性价比
        public string c79 { get; set; }                 //餐品性价比描述
        public string c80 { get; set; }                 //其他评价
        public string c81 { get; set; }                 //在线评分
        public DateTime c82 { get; set; }                 //评论日期
        public TimeSpan c83 { get; set; }                 //评论时间
        public string c84 { get; set; }                 //1=同一医院当日多场
        public string c85 { get; set; }                 //2=同一代表当日多场
        public string c86 { get; set; }                 //3=同一餐厅当日多场
        public string c87 { get; set; }                 //4=同一代表同一医院当日多场
        public string c88 { get; set; }                 //5=同一代表同一餐厅当日多场
        public string c89 { get; set; }                 //6=同一代表同一医院同一餐厅当日多场
        public string c90 { get; set; }                 //7=参会人数>=60
        public string c91 { get; set; }                 //8=参会人数<60,订单分数>=60
        public string c92 { get; set; }                 //9=代表自提
        public string c93 { get; set; }                 //直线经理姓名
        public string c94 { get; set; }                 //直线经理MUDID
        public string c95 { get; set; }                 //Level2 Manager 姓名
        public string c96 { get; set; }                 //Level2 Manager MUDID
        public string c97 { get; set; }                 //Level3 Manager 姓名
        public string c98 { get; set; }                 //Level3 Manager MUDID
        public string c99 { get; set; }                 //是否上传文件
        public DateTime c100 { get; set; }                //上传文件提交日期
        public TimeSpan c101 { get; set; }                //上传文件提交时间
        public string c102 { get; set; }                //上传文件审批直线经理姓名
        public string c103 { get; set; }                //上传文件审批直线经理MUDID
        public DateTime c104 { get; set; }                //上传文件审批日期
        public TimeSpan c105 { get; set; }                //上传文件审批时间
        public string c106 { get; set; }                //上传文件审批状态
        public string c107 { get; set; }                //签到人数是否等于实际用餐人数
        public string c108 { get; set; }                //签到人数
        public string c109 { get; set; }                //签到人数调整原因
        public string c110 { get; set; }                //上传文件是否Re-Open
        public string c111 { get; set; }                //上传文件Re-Open操作人
        public string c112 { get; set; }                //上传文件Re-Open操作人MUDID
        public DateTime c113 { get; set; }                //上传文件Re-Open日期
        public TimeSpan c114 { get; set; }                //上传文件Re-Open时间
        public string c115 { get; set; }                //上传文件Re-Open原因
        public string c116 { get; set; }                //上传文件Re-Open审批状态
        public string c117 { get; set; }                //上传文件是否重新分配
        public string c118 { get; set; }                //上传文件重新分配操作人
        public string c119 { get; set; }                //上传文件重新分配操作人MUDID
        public string c120 { get; set; }                //上传文件被重新分配人姓名
        public string c121 { get; set; }                //上传文件被重新分配人MUDID
        public DateTime c122 { get; set; }                //上传文件被重新分配日期
        public TimeSpan c123 { get; set; }                //上传文件被重新分配时间
        public string c124 { get; set; }                //上传文件否重新分配审批人
        public string c125 { get; set; }                //上传文件重新分配审批人-操作人
        public string c126 { get; set; }                //上传文件重新分配审批人-操作人MUDID
        public string c127 { get; set; }                //上传文件被重新分配审批人姓名
        public string c128 { get; set; }                //上传文件被重新分配审批人MUDID
        public DateTime c129 { get; set; }                //上传文件重新分配审批人日期
        public TimeSpan c130 { get; set; }                //上传文件重新分配审批人时间
        public string c131 { get; set; }                //项目组特殊备注
        public string c133 { get; set; }                //上传文件Re-Open发起人姓名
        public string c134 { get; set; }                //上传文件Re-Open发起人MUDID
        public string c135 { get; set; }                //上传文件Re-Open备注
        public string c136 { get; set; }                //菜品详情
        public string c137 { get; set; }                //订单状态
        public string c138 { get; set; }                  //是否与预定餐品一致
        public string c139 { get; set; }                  //是否与会议信息一致
        public string c140 { get; set; }                  //会议信息不一致原因
        public string c141 { get; set; }                  //RDSDName
        public string c142 { get; set; }                  //RDSDMUDID
        public string c144 { get; set; }                  //MRTerritoryCode
        public string c145 { get; set; }                  //RD Territory Code

        public string c146 { get; set; }                //SupplierSpecialRemark
        public string c147 { get; set; }                //IsCompleteDelivery
        public decimal? c148 { get; set; }              //SupplierConfirmAmount
        public decimal? c149 { get; set; }              //GSKConfirmAmount
        public string c150 { get; set; }                //GSKConAAReason
        public decimal? c151 { get; set; }              //MealPaymentAmount
        public string c152 { get; set; }                //MealPaymentPO
        public string c153 { get; set; }                //AccountingTime
    }
}
