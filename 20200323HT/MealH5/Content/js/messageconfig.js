
var MSG_OPTFAIL = '操作失败';
var MSG_TIMEOUT = '操作已超时';
var MSG_NEEDCHOOSEHOSPITAL = '请选择医院';
var MSG_NEEDCHOOSEHOSPITALADDRESS = '请选择医院地址';
var MSG_USERINBLACKNAMELIST = '您已被暂停服务，故暂时无法提交订单。如有疑问，请联系：cn.chinarx-pmo@gsk.com。';
var MSG_NOWORKINGTIME = '对不起，目前不支持下单服务。微信下单时间为每日 09:00-18:00。';
var MSG_NOPREAPPROVALWORKINGTIME = '对不起, 目前不支持提交预申请服务。预申请提交时间为每日 09:00-18:00。';
var MSG_NORESTAURANT = '该目标医院暂不支持送餐服务。如有疑问，请联系：cn.chinarx-pmo@gsk.com。';
var MSG_NEEDCHOOSEPROVINCE = '请先选择省份';
var MSG_NEEDATTENDCOUNT = '请填写用餐人数';
var MSG_NEEDCONSIGNEE = '请填写收餐人';
var MSG_NEEDPHONE = '请填写手机号';
var MSG_NEEDDELIVERTIME = '请选择送餐时间';
var MSG_NOORDERTIME = '对不起，目前不支持订单修改服务。订单修改时间为每日 09:00-20:00。';
var MSG_NOEDITPREAPPROVALTIME = '对不起, 目前不支持修改预申请服务。预申请修改时间为每日 09:00-20:00。';
var MSG_NOUPLOADFILETIME = '对不起，目前不支持上传文件服务。上传文件时间为每日 09:00-20:00。';
var MSG_MONEYOVERPROOF = '您的订单已超出人均60元标准，请修改订单';
var MSG_ORDERTIMEFAIL = '对不起，该订单已失效，请重新选择送餐时间。';
var MSG_TODAYORDERTIMEFAIL = '对不起，目前不支持当日下单服务。微信当日下单时间为每日9:00-10:00。';
var MSG_SUBMITORDERSUCCESS = '您的订单已提交成功，正在等待餐厅确认。';
var MSG_NEEDCHOOSEFOOD = '请先选餐';
var MSG_TOTALVOERPROOF = '您的订单金额已超出预申请预算，请修改预申请预算或修改订单金额。';
var MSG_TOTALTOOLOWER = '您的订单尚未达到餐厅起送标准，请修改订单金额。';
var MSG_ORDERAPPROVING = '您的订单已提交成功，正在等待中央订餐项目组审批。';
var MSG_ORDERCHANGEAPPROVING = '您的订单修改已提交成功，正在等待中央订餐项目组审批。';
var MSG_ORIGINALORDERSENDSUCCESS = '按原订单配送提交成功，正在等待餐厅确认。';
var MSG_ORDERCHANGE = '您的订单修改请求已提交成功，正在等待餐厅确认。';
var MSG_Market = '请选择Market';
var MSG_CityName = '请选择城市';
var MSG_MarketNone = '请前往【个人中心】页面维护完整信息后再进行操作。';
var MSG_PREAPPROVALMEETINGDATEERROR = '您的会议目前尚不可订餐，请在会议日期前1天提交订单。如有疑问，请联系：cn.chinarx-pmo@gsk.com。';
var MSG_OUTATTENDCOUNT = '用餐人数大于参会人数，请修改预申请或修改订单';
var MSG_UPLOADIMAGE = '文件上传失败，请您重新上传。<br\>如有疑问，请联系呼叫中心。<br\>BDS：400-6868-912<br\>XMS：400-820-5577'
var MSG_DELIVERYADDRESS = '请填写送餐详细地址'; 
var MSG_TA = '请选择TA';
var MSG_VEEVAMEETINGID = '请正确填写Veeva Meeting ID';
var MSG_VEEVAMEETINGIDNULL = '请填写Veeva Meeting ID';
var MSG_PROVINCE = '请选择省份';
var MSG_CITY = '请选择城市';
var MSG_HOSPITALNAME = '请选择医院名称';
var MSG_CONFERENCETITLE = '请填写会议名称';
var MSG_CONFERENCEDATE = '请选择会议日期';
var MSG_CONFERENCETIME = '请选择会议时间';
var MSG_ATTENDANCE = '请填写参会人数';
var MSG_COSTCENTER = '请选择大区区域代码';
var MSG_BUDGET = '请填写预算金额';
var MSG_FOLLOWVISIT = '请选择直线经理是否随访';
var MSG_ATTENDANCETYPE = '参会人数只能输入正整数';
var MSG_CONFERENCETITLELENGTH = '会议名称不能超过50个字符，请重新输入';
var MSG_BUDGETINFO = '您的预申请已超出人均60元标准，请修改预申请';
var MSG_ORDERSUCCEEDBUDGETEDITLESS = '您的订单已预定成功，修改预算金额需超过原预算金额';
var MSG_ORDERSUCCEEDATTENDANCEEDITLESS = '您的订单已预定成功，修改参会人数需超过原参会人数';
var MSG_PREAPPROVALSUBMITSUCCESS = '您的预申请已提交成功。';
var MSG_PREAPPROVALSUBMITSUCCESSWAITMMCOE = '您的预申请已提交成功，正在等待中央订餐项目组审批。';
var MSG_PREAPPROVALSUBMITSUCCESSWAITBUHEAD = '您的预申请已提交成功，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。';
var MSG_PREAPPROVALSUBMITSUCCESSWAITSECOND = '您的预申请已提交成功，正在等待二级经理审批。'
var MSG_EDITPREAPPROVALSUBMITSUCCESS = '您的预申请修改已提交成功。';
var MSG_EDITPREAPPROVALSUBMITSUCCESSWAITMMCOE = '您的预申请修改已提交成功，正在等待中央订餐项目组审批。';
var MSG_EDITPREAPPROVALSUBMITSUCCESSWAITBUHEAD = '您的预申请修改已提交成功，正在等待BU Head（Rx/Vx）或销售总监（DDT/TSKF）审批。';
var MSG_NOTSELECTSPEAKERFILE = '请选择外部免费讲者来讲';
var MSG_NOTUPLOADSPEAKERFILE = '请上传讲者协议文件';
var MSG_NOTUPLOADSPEAKERSERVICE = '请上传演讲服务协议';
var MSG_NOTUPLOADSPEAKERBENEFIT = '请上传利益冲突声明';
var MSG_ATTENDANCECOUNT = '您的预申请参会人数>=300人，请修改预申请。';
var MSG_BUDGETCOUNT = '您的预申请人均<=1元，请修改预申请。';
var MSG_TACHANGE = '您的TA已变更，不能修改医院信息。如需更改医院，请重新申请预申请。';
var MSG_TERRITORYCHANGE = '您的个人区域编码已变更，不能修改医院信息。如需更改医院，请重新申请预申请。';
var MSG_UNDERLINEHT = '线上HT的预算金额只能为0元，请修改预申请。';
var MSG_HTTYPE = '请选择HT形式';

//预申请会议日期
var MSG_MONDAYONE = "周一9:00~12:00 可申请的最早会议日期为周二6:00~22:00";   //星期一
var MSG_MONDAYTWO = "周一12:01~18：00 可申请的最早会议日期为周三06:00~22:00";   //星期一
var MSG_TUESDAYONE = "周二9:00~12:00 可申请的最早会议日期为周三6:00~22:00";   //星期二
var MSG_TUESDAYTWO = "周二12:01~18：00 可申请的最早会议日期为周四06:00~22:00";   //星期二
var MSG_WEDNESDAYONE = "周三9:00~12:00 可申请的最早会议日期为周四6:00~22:00";   //星期三
var MSG_WEDNESDAYTWO = "周三12:01~18：00 可申请的最早会议日期为周五06:00~22:00";   //星期三
var MSG_THURSDAYONE = "周四9:00~12:00 可申请的最早会议日期为周五6:00~22:00";   //星期四
var MSG_THURSDAYTWO = "周四12:01~18：00 可申请周六06:00~22:00，周日06:00~22:00，周一06:00~22:00";   //星期四
var MSG_FRIDAYONE = "周五 09:00~12:00 可申请周六06:00~22:00，周日06:00~22:00,周一06:00~22:00";   //星期五
var MSG_FRIDAYTWO = "周五12:01~18:00 可申请的最早会议日期为周二06:00~22:00";   //星期五
var MSG_SATURDAY = "周六可申请的最早会议日期为周二06:00~22:00";   //星期六
var MSG_SUNDAY = "周日可申请的最早会议日期为周二06:00~22:00";   //星期日

//上传文件
var MSG_UPLOADFILES = "对不起, 目前不支持上传文件服务。上传文件时间为每日 09:00-20:00。";   //上传文件时间超时提醒
var MSG_UPLOADFILESSUCCEED = "上传文件提交成功。";   //上传文件提交成功
var MSG_UPLOADFILESSUCCEEDSPECIAL = "退单原因提交成功。";   //上传文件提交成功
var MSG_UPLOADFILESSUCCEEDLOSS = "会议支持文件丢失原因提交成功。";   //上传文件提交成功
var MSG_UPLOADFILESSUCCEEDFOODLOSS = "未送达，会议未正常召开原因提交成功。";   //上传文件提交成功
var MSG_EDITUPLOADFILESSUCCEED = "上传文件修改成功。";   //上传文件提交成功
var MSG_EDITUPLOADFILESSUCCEEDSPECIAL = "退单原因修改成功。";   //上传文件提交成功
var MSG_EDITUPLOADFILESSUCCEEDLOSS = "会议支持文件丢失修改成功。";   //上传文件提交成功
var MSG_EDITUPLOADFILESSUCCEEDFOODLOSS = "未送达，会议未正常召开原因修改成功。";   //上传文件提交成功
var MSG_ERRCODE = function (errCode) {
    var errCode = errCode * 1;
    if (3001 <= errCode && errCode <= 3018) {
        var msg = '错误码：' + errCode;
        msg += '<br\>请联系：400-820-5577';
        return msg;
    }
    else if (3874 == errCode) {
        var msg  = '您的订单金额已超出预申请预算，请修改预申请预算或修改订单金额。';
        return msg;
    }
    else if (5555 == errCode) {
        var msg = '您的文件已经提交过，请勿重复提交。';
        return msg;
    }
    else if (6666 == errCode) {
        var msg = '您的直线经理信息有误，请隔日再尝试提交。';
        return msg;
    }
    else if (3019 == errCode) {
        var msg = '错误码：' + errCode;
        msg += '<br\>如有问题请联系：400-820-5577';
        return msg;
    }
    else if (9007 == errCode) {
        var msg = '订单审批失败，请刷新页面后重试。';
        return msg;
    }
    else if (8000 <= errCode && errCode <= 9000) {
        var msg = '错误码：' + errCode;
        msg += '<br\>请联系：400-6868-912';
        return msg;
    }
    else {
        var msg = '错误码：' + errCode;
        msg += '<br\>请联系呼叫中心';
        msg += '<br\>BDS：400-6868-912';
        msg += '<br\>XMS：400-820-5577';
        
        return msg;
    }
}


var MSG_ERRCODE_IsNeedCloseWindow = function (errCode) {
    var errCode = errCode * 1;
    if (3001 <= errCode && errCode <= 3018) {
        return false;
    }
    else if (8001 <= errCode && errCode <= 9000) {
        return false;
    }
    else if (3019 == errCode) {
        return false;
    }
    else {
        return true;
    }
}
//新增地址
var MSG_ADDRESSAPPROVALSUBMITSUCCESS = '您的地址申请已提交成功，正在等待直线经理审批';
var MSG_ADDRESSAPPROVALCANCELSUCCESS = '您的地址申请取消成功';
var MSG_ADDRESSAPPROVALUPDATESUCCESS = '您的地址申请修改已提交成功，正在等待直线经理审批';
var MSG_ADDRESSAPPROVALRESUBMITSUCCESS = '您的地址申请已重新提交成功，正在等待直线经理审批';

var MSG_ADDADDRESSWORKINGTIME = '对不起,目前不支持提交外送地址服务.外送地址提交时间为每日09:00-18:00.';
var MSG_UPDATEADDRESSWORKINGTIME = '对不起,目前不支持修改外送地址服务.外送地址修改时间为每日09:00-20:00.';
var MSG_RESUBMITADDRESSWORKINGTIME = '对不起,目前不支持重新提交外送地址服务.外送地址重新提交时间为每日09:00-20:00.';

//取消预申请
var MSG_PREAPPROVALCANCELSUCCESS = '您的预申请取消成功';


