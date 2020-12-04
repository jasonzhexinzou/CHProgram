using MealAdmin.Entity;
using MealAdmin.Service;
using MealAdmin.Web.Filter;
using MealAdminApiClient;
using SFTPHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;
using System.Threading.Tasks;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class PersonnelController : Controller
    {
        [Bean("userInfoService")]
        public IUserInfoService userInfoService { get; set; }
        [Bean("uploadOrderService")]
        public IUploadOrderService uploadOrderService { get; set; }
        [Bean("preApprovalService")]
        public IPreApprovalService PreApprovalService { get; set; }

        // GET: P/Personnel
        [AdminSessionFilter(Order = 1)]
        [PermissionFilter(Order = 2, Permission = "00000000-0000-0000-6000-000000000001")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-5000-000000000001")]
        public JsonResult Load(string userId, string DMUserId, int rows, int page)
        {
            LogHelper.Info(Request.RawUrl);
            int total;
            var list = userInfoService.LoadPage(userId, DMUserId, rows, page, out total);

            return Json(new { state = 1, rows = list, total = total });
        }
        
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-5000-000000000001")]
        public JsonResult SyncQyUser()
        {
            LogHelper.Info("SyncQyUser Start!");
            string account = System.Configuration.ConfigurationManager.AppSettings["SFTPAccount"];
            string pass = System.Configuration.ConfigurationManager.AppSettings["SFTPPass"];
            SFTPOperation sftp = new SFTPOperation("esft-cn.gsk.com", "22", account, pass);
            //读取人员数据
            var employeeFile = sftp.ReadFile("shared/UserList/EXP_EMPLOYEE_USER_LIST_WX_DD.CSV");
            String csvSplitBy = "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)";
            StreamReader sr = new StreamReader(employeeFile, System.Text.UTF8Encoding.UTF8);
            List<SyncUserInfo> syncUserInfo = new List<SyncUserInfo>();
            DataTable dt = new DataTable();
            int i = 0, m = 0;
            sr.Peek();
            while (sr.Peek() > 0)
            {
                m = m + 1;
                string strd = sr.ReadLine();
                if (m >= 0 + 1)
                {
                    if (m == 0 + 1) //如果是字段行，则自动加入字段。  
                    {
                        MatchCollection mcs = Regex.Matches(strd, csvSplitBy);
                        foreach (Match mc in mcs)
                        {
                            dt.Columns.Add(mc.Value); //增加列标题  
                        }
                    }
                    else
                    {
                        MatchCollection mcs = Regex.Matches(strd, "(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)");
                        i = 0;
                        System.Data.DataRow dr = dt.NewRow();
                        foreach (Match mc in mcs)
                        {
                            dr[i] = mc.Value;
                            i++;
                        }
                        dt.Rows.Add(dr);  //DataTable 增加一行       
                    }
                }
            }
            foreach (DataRow item in dt.Rows)
            {
                SyncUserInfo syncUser = new SyncUserInfo();
                syncUser.ID = Guid.NewGuid();
                syncUser.UserId = item[0].ToString().Replace("\"", "").Trim();
                syncUser.UserName = item[1].ToString().Replace("\"", "");
                syncUser.UserNo = item[3].ToString().Replace("\"", "");
                syncUser.Email = item[4].ToString().Replace("\"", "");
                syncUser.LineManager = item[8].ToString().Replace("\"", "");
                syncUser.IsCurrent = item[9].ToString().Replace("\"", "") == "YES" ? 1 : 0;
                syncUser.Department = item[5].ToString().Replace("\"", "");
                syncUser.Title = item[11].ToString().Replace("\"", "").Replace("?", "").Replace("？", "").Replace("�", "").Replace("㡰", "");
                syncUserInfo.Add(syncUser);
            }
            //List<SyncUserInfo> syncUserInfo = new List<SyncUserInfo>();
            //SyncUserInfo syncUser = new SyncUserInfo();
            //syncUser.ID = Guid.NewGuid();
            //syncUser.UserId = "yw960402";
            //syncUser.UserName = "ppw53001";
            //syncUser.UserNo = "";
            //syncUser.Email = "";
            //syncUser.LineManager = "jjz63156";
            //syncUser.IsCurrent = 1;
            //syncUser.Department = "";
            //syncUser.Title = "";
            //syncUserInfo.Add(syncUser);


            //SyncUserInfo syncUser1 = new SyncUserInfo();
            //syncUser1.ID = Guid.NewGuid();
            //syncUser1.UserId = "jjz63156";
            //syncUser1.UserName = "Johnny Zhao";
            //syncUser1.UserNo = "";
            //syncUser1.Email = "";
            //syncUser1.LineManager = "jfh21155";
            //syncUser1.IsCurrent = 1;
            //syncUser1.Department = "";
            //syncUser1.Title = "";
            //syncUserInfo.Add(syncUser1);
            List<WP_QYUSER> qyUserList = new List<WP_QYUSER>();
            List<WP_QYUSER> qyUserListLine2 = new List<WP_QYUSER>();
            Guid weChatId = Guid.NewGuid();
            foreach (var item in syncUserInfo.ToList())
            {
                WP_QYUSER qyUser = new WP_QYUSER();
                qyUser.ID = item.ID;
                qyUser.WechatID = weChatId;
                qyUser.Name = item.UserName;
                qyUser.UserId = item.UserId;
                qyUser.Email = item.Email;
                qyUser.CreateDate = DateTime.Now;
                qyUser.Creator = Guid.Empty;
                qyUser.ModifyDate = qyUser.CreateDate;
                qyUser.Modifier = qyUser.Creator;
                var lineManager = syncUserInfo.FirstOrDefault(p => p.UserId == item.LineManager);
                if (lineManager != null)
                {
                    qyUser.LineManager = lineManager.UserId+","+lineManager.UserName;
                    qyUser.LineManagerID = lineManager.ID;
                }
                qyUser.State = item.IsCurrent == 1 ? 1 : 4;
                qyUser.DeptNames = item.Department;
                qyUser.Position = item.Title;
                qyUser.Gender = 0;
                qyUserList.Add(qyUser);
            }

            var LineManagerList = userInfoService.FindUserManagerInfo();

            foreach (var item in syncUserInfo.ToList())
            {
                WP_QYUSER qyUser = new WP_QYUSER();
                qyUser.ID = item.ID;
                qyUser.WechatID = weChatId;
                qyUser.Name = item.UserName;
                qyUser.UserId = item.UserId;
                qyUser.Email = item.Email;
                qyUser.CreateDate = DateTime.Now;
                qyUser.Creator = Guid.Empty;
                qyUser.ModifyDate = qyUser.CreateDate;
                qyUser.Modifier = qyUser.Creator;
                var LineManager1 = LineManagerList.Where(x => x.UserId == item.UserId).FirstOrDefault();
                if (LineManager1 != null && LineManager1.LineManagerId != null)
                {
                    qyUser.LineManager = LineManager1.LineManagerId;
                }
                else
                {
                    qyUser.LineManager = "";
                }
                qyUser.State = item.IsCurrent == 1 ? 1 : 4;
                qyUser.DeptNames = item.Department;
                qyUser.Position = item.Title;
                qyUser.Gender = 0;
                qyUserListLine2.Add(qyUser);
            }

            if (qyUserList.Count > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        userInfoService.SyncWorkDayUserInfo(qyUserList);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("SysUserError:Start");
                        LogHelper.Error(ex.Message);
                        LogHelper.Error("SysUserError:End");
                        throw ex;
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        LogHelper.Info("SyncQyUser Start!");
                        SyncPreChange(qyUserList, qyUserListLine2);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
                //对结果进行遍历
                foreach (var item in qyUserList)
                {
                    //查询用户是否有未完成的上传文件审批
                    var uploadOrderList = uploadOrderService.GetUploadOrderByUserId(item.UserId);
                    foreach (var uploadOrder in uploadOrderList)
                    {
                        if (item.State != 4)
                        {

                            //判断未完成的订单是否审批人发生了变化
                            LogHelper.Info("SyncQyUser:UserID" + item.UserId);
                            if(item.LineManager == null || item.LineManager == "")
                            {
                                continue;
                            }
                            var uploadInfoDMDelegate = userInfoService.isAgent(item.LineManager.Split(',')[0]);
                            if(uploadInfoDMDelegate != null && uploadOrder.BUHeadMUDID.Equals(uploadInfoDMDelegate) && uploadOrder.IsReAssign == false)
                            {
                                continue;
                            }
                            if ((!uploadOrder.BUHeadMUDID.Equals(item.LineManager.Split(',')[0]) && uploadOrder.IsReAssign == false) || (uploadOrder.IsReAssign == true && !uploadOrder.ReAssignBUHeadMUDID.Equals(item.LineManager.Split(',')[0])))
                            {
                                LogHelper.Info("SyncQyUser:LineManager" + item.LineManager);
                                //将待审批上传文件转交给新的审批人
                                PreApprovalService.UpdatePuoReAssign(uploadOrder.ID.ToString(), "系统自动", "系统自动", item.LineManager.Split(',')[0], item.LineManager.Split(',')[1]);
                                //发消息
                                var messageBase = "该订单已上传会议支持文件";
                                switch (uploadOrder.FileType)
                                {
                                    case 1:
                                        messageBase = "该订单已提交退单原因";
                                        break;
                                    case 2:
                                        messageBase = "该订单已提交会议支持文件丢失原因";
                                        break;
                                    case 3:
                                        messageBase = "该订单已提交未送达，会议未正常召开原因";
                                        break;
                                }
                                LogHelper.Info("SyncQyUser:HTCode" + uploadOrder.HTCode);
                                var approverMsg = $"{uploadOrder.HTCode}，{messageBase}，请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/Upload/Approval?id={uploadOrder.ID}&from=0'>点击这里</a>进行审批。";
                                var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(item.LineManager.Split(',')[0], approverMsg);
                            }

                        }

                    }

                }
            }
            LogHelper.Info("SyncQyUser Success!");
            //new TaskPlanController().AutoTransfer();
            return Json(new { state = 1 });
        }

        public void SyncPreChange(List<WP_QYUSER> qyUserListLine, List<WP_QYUSER> qyUserListLine2)
        {
            LogHelper.Info("SyncPreChange:Start");
            foreach (var item in qyUserListLine)
            {
                try
                {
                    var LineManagerOld = qyUserListLine2.Where(x => x.UserId == item.UserId).FirstOrDefault().LineManager;

                    if (item.LineManagerID != null && item.LineManager != null && LineManagerOld != null && !LineManagerOld.Equals(item.LineManager.Split(',')[0]))
                    {

                        var PreApprovalList = PreApprovalService.GetPreApprovalByUser(LineManagerOld);
                        foreach (var PreApproval in PreApprovalList)
                        {
                            var LineManagerID = userInfoService.Find(LineManagerOld).LineManagerID;
                            var UserList = userInfoService.FindUserByLineManager(LineManagerID);
                            List<string> list = new List<string>();
                            if (UserList != null)
                            {
                                list = UserList.Select(x => x.UserId).ToList();
                            }
                            if (list.Contains(item.UserId))
                            {
                                //PreApproval.CurrentApproverMUDID = item.LineManager.Split(',')[0];
                                //PreApprovalService.UpdateCurrentPreApprova(PreApproval);
                                LogHelper.Info("SyncQyUserLineManager:UserID" + item.UserId);
                                LogHelper.Info("SyncQyUserLineManager:CurrentApproverMUDID" + PreApproval.CurrentApproverMUDID);
                                //var approverMsg = $"{PreApproval.HTCode}，您有需要审批的预申请。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/PreApproval/MMCoEApprove/{PreApproval.ID}'>点击这里</a>进行审批。";
                                //var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(item.LineManager.Split(',')[0], approverMsg);
                            }
                        }
                    }

                }
                catch
                {
                    LogHelper.Info("SyncPreChange:Error");
                    LogHelper.Info("SyncPreChange:UserID-" + item.UserId);
                }
            }
            
        }


        public class SyncUserInfo
        {
            public Guid ID { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string UserNo { get; set; }
            public string Email { get; set; }
            public string LineManager { get; set; }
            public int IsCurrent { get; set; }
            public string Department { get; set; }
            public string Title { get; set; }
        }

    }
}