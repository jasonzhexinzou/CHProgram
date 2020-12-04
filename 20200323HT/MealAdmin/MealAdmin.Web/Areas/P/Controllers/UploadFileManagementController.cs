using System;
using MealAdmin.Entity;
using MealAdmin.Entity.View;
using IamPortal.AppLogin;
using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MealAdmin.Service;
using MealAdmin.Util;
using MealAdmin.Web.Filter;
using MealAdmin.Web.Handler;
using MealAdminApiClient;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.XInject.Attributes;
using XFramework.XUtil;
using Amazon.S3;
using Amazon.S3.Model;
using NPOI.HSSF.UserModel;
using System.Threading;

namespace MealAdmin.Web.Areas.P.Controllers
{
    public class UploadFileManagementController : AdminBaseController
    {
        private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
        private static string awsSecretKey = "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";
        private static string bucketName = "wechat";
        AmazonS3Config config = new AmazonS3Config()
        {
            ServiceURL = "http://s3.amazonaws.com",
            RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
        };
        // GET: P/UploadFileManagement

        [Bean("uploadOrderService")]
        public IUploadOrderService uploadOrderService { get; set; }

        [Bean("uploadFileQueryService")]
        public IUploadFileQueryService uploadFileQueryService { get; set; }
        [Bean("preApprovalService")]
        public IPreApprovalService PreApprovalService { get; set; }
        [Bean("operationAuditService")]
        public IOperationAuditService operationAuditService { get; set; }
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]

        // GET: P/UploadFileManagement
        [Bean("userInfoService")]
        public IUserInfoService userInfoService { get; set; }

        // GET: P/UploadFileManagement
        public ActionResult UploadFileQuery()
        {
            return View();
        }

        [HttpPost]
        //[AdminSessionFilter(Order = 1)]
        //[PermissionFilter(Order = 2, Permission = "00000000-0000-0000-2000-000000000001")]
        //[OperationAuditFilter(Operation = "查询上传文件", OperationAuditTypeName = "查询上传文件")]
        public JsonResult UploadFileLoad(string HTCode, string ApplierMUDID, string Begin, string End, string State, int rows, int page)
        {
            int total;
            var list = uploadFileQueryService.LoadPage(HTCode, ApplierMUDID, Begin, End, State, rows, page, out total);
            var rtnList = new List<P_UploadFileQuery_View>();
            foreach (var i in list)
            {
                rtnList.Add(GetDisplayObj(i));
            }
            return Json(new { state = 1, rows = rtnList, total = total });
        }
        public ActionResult Redistribution()
        {
            return View();
        }

        public ActionResult UploadFileDetails(string id)
        {
            ViewBag.id = id;
            return View();
        }

        public ActionResult UploadFileDetailsInfo(Guid id)
        {
            ViewBag.id = id;
            return View();
        }

        #region 上传文件详情导出PDF
        /// <summary>
        /// 上传文件详情导出PDF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PdfExport(string id)
        {
            var list = uploadFileQueryService.GetUpdateFileByID(id);  //通过ID查询上传文件详情          
            var obj = new List<P_UploadFileQuery_View>();
            var p = GetDisplayObj(list[0]);


            var Approval = uploadFileQueryService.GetApproval(id);
            var _Approval = new List<P_OrderApproveHistory_Time>();
            foreach (var i in Approval)
            {
                _Approval.Add(GetTimeObj(i));
            }

            #region 写入到客户端

            Document document = new Document(PageSize.A4);
            var path = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            if (!Directory.Exists(Server.MapPath("~/UploadFileManagement/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/UploadFileManagement/"));
            }
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/UploadFileManagement/"));
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                                                                   //遍历删除文件   防止临时文件过多。
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    System.IO.File.Delete(i.FullName);      //删除指定文件
                }
            }
            PdfWriter.GetInstance(document, new FileStream(Server.MapPath("~/UploadFileManagement/") + path + ".pdf", FileMode.Create));
            document.Open();
            BaseFont baseFont = BaseFont.CreateFont(@"c:\Windows\fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont);
            #endregion

            #region 文本
            PdfPTable table = new PdfPTable(4);
            PdfPCell cell1 = new PdfPCell(new Phrase($"申请人姓名:{p.ApplierName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell1);
            PdfPCell cell2 = new PdfPCell(new Phrase($"申请人MUDID:{p.ApplierMUDID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell2);
            PdfPCell cell3 = new PdfPCell(new Phrase($"申请人职位:{p.Position}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell3);
            //PdfPCell cell4 = new PdfPCell(new Phrase($"申请人手机号码:{p.ApplierMobile}", font))
            PdfPCell cell4 = new PdfPCell(new Phrase($"申请人手机号码:", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell4);
            PdfPCell cell5 = new PdfPCell(new Phrase($"HT编号:{p.HTCode}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell5);
            PdfPCell cell6 = new PdfPCell(new Phrase($"Market:{p.Market}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell6);
            PdfPCell cell143 = new PdfPCell(new Phrase($"VeevaMeetingID:{p.VeevaMeetingID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell143);
            PdfPCell cell7 = new PdfPCell(new Phrase($"TA:{p.TA}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell7);
            PdfPCell cell8 = new PdfPCell(new Phrase($"省份:{p.Province}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell8);
            PdfPCell cell9 = new PdfPCell(new Phrase($"城市:{p.City}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell9);
            PdfPCell cell10 = new PdfPCell(new Phrase($"医院名称:{p.HospitalName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell10);
            PdfPCell cell11 = new PdfPCell(new Phrase($"会议名称:{p.MeetingName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell11);
            PdfPCell cell12 = new PdfPCell(new Phrase($"参会人数:{p.AttendCount}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell12);
            PdfPCell cell13 = new PdfPCell(new Phrase($"大区区域代码:{p.CostCenter}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell13);
            PdfPCell cell14 = new PdfPCell(new Phrase($"供应商:{p.Channel}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell14);
            PdfPCell cell15 = new PdfPCell(new Phrase($"订单号:{p.CN}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell15);
            PdfPCell cell16 = new PdfPCell(new Phrase($"送餐日期:{p.DeliverDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell16);
            PdfPCell cell17 = new PdfPCell(new Phrase($"送餐时间:{p.DeliverTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell17);
            PdfPCell cell18 = new PdfPCell(new Phrase($"用餐人数:{p.AttendCounts}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell18);
            PdfPCell cell19 = new PdfPCell(new Phrase($"总份数:{p.FoodCount}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell19);
            PdfPCell cell20 = new PdfPCell(new Phrase($"预订金额:{p.TotalPrice}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell20);
            PdfPCell cell21 = new PdfPCell(new Phrase($"实际金额:{p.XmsTotalPrice}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell21);
            PdfPCell cell22 = new PdfPCell(new Phrase($"金额调整原因:{p.ChangeTotalPriceReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell22);
            //是否申请退单
            PdfPCell cell47 = new PdfPCell(new Phrase($"是否申请退单:{p.IsCancel}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell47);
            //是否退单成功
            PdfPCell cell48 = new PdfPCell(new Phrase($"是否退单成功:{p.CancelState}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell48);
            //是否收餐/未送达
            PdfPCell cell49 = new PdfPCell(new Phrase($"是否收餐/未送达:{p.IsReceive}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell49);
            PdfPCell cell23 = new PdfPCell(new Phrase($"确认收餐日期:{p.ReceiveDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell23);
            PdfPCell cell24 = new PdfPCell(new Phrase($"确认收餐时间:{p.ReceiveTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell24);
            PdfPCell cell25 = new PdfPCell(new Phrase($"用户确认金额:{p.RealPrice}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell25);
            PdfPCell cell50 = new PdfPCell(new Phrase($"是否与预订餐品一致:{p.IsMealSame}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell50);
            PdfPCell cell26 = new PdfPCell(new Phrase($"用户确认金额调整原因:{p.RealPriceChangeReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell26);
            PdfPCell cell27 = new PdfPCell(new Phrase($"用户确认金额调整备注:{p.RealPriceChangeRemark}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell27);
            PdfPCell cell28 = new PdfPCell(new Phrase($"实际用餐人数:{p.RealCount}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell28);
            PdfPCell cell29 = new PdfPCell(new Phrase($"实际用餐人数调整原因:{p.RealCountChangeReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell29);
            PdfPCell cell30 = new PdfPCell(new Phrase($"实际用餐人数调整备注:{p.RealCountChangeRemrak}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell30);
            PdfPCell cell31 = new PdfPCell(new Phrase($"订单状态:{p.State}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell31);
            PdfPCell cell51 = new PdfPCell(new Phrase($"项目组特殊备注:{p.SpecialReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell51);
            var IsOrderUpload = p.IsOrderUpload;
            PdfPCell cell32 = new PdfPCell(new Phrase($"是否上传文件:{IsOrderUpload}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell32);
            var IsReAssign = p.IsReAssign;
            PdfPCell cell33 = new PdfPCell(new Phrase($"是否重新分配:{IsReAssign}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell33);
            PdfPCell cell34 = new PdfPCell(new Phrase($"上传文件审批直线经理姓名:{p.ReAssignBUHeadName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell34);
            PdfPCell cell35 = new PdfPCell(new Phrase($"上传文件审批直线经理MUDID:{p.ReAssignBUHeadMUDID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell35);
            PdfPCell cell52 = new PdfPCell(new Phrase($"上传文件审批日期:{p.ApproveDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell52);
            PdfPCell cell36 = new PdfPCell(new Phrase($"上传文件审批状态:{p.Stated}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell36);
            var IsAttentSame = p.IsAttentSame;
            PdfPCell cell37 = new PdfPCell(new Phrase($"签到人数是否等于实际用餐人数:{IsAttentSame}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell37);
            PdfPCell cell38 = new PdfPCell(new Phrase($"签到人数调整原因:{p.AttentSameReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell38);
            PdfPCell cell53 = new PdfPCell(new Phrase($"是否与会议信息一致:{p.IsMeetingSame}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell53);
            PdfPCell cell54 = new PdfPCell(new Phrase($"会议信息不一致原因:{p.MeetingSameReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell54);
            PdfPCell cell55 = new PdfPCell(new Phrase($"退单原因/未送达，会议未正常召开原因/会议文件丢失原因:{p.SpecialUploadReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell55);
            var IsReopen = p.IsReopen;
            PdfPCell cell39 = new PdfPCell(new Phrase($"上传文件是否Re-Open:{IsReopen}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell39);
            PdfPCell cell40 = new PdfPCell(new Phrase($"上传文件Re-Open操作人:{p.ReopenOperatorName}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell40);
            PdfPCell cell41 = new PdfPCell(new Phrase($"上传文件Re-Open操作人MUDID:{p.ReopenOperatorMUDID}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell41);
            PdfPCell cell42 = new PdfPCell(new Phrase($"上传文件Re-Open日期:{p.ReopenOperateDate}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell42);
            PdfPCell cell43 = new PdfPCell(new Phrase($"上传文件Re-Open时间:{p.ReopenOperateTime}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell43);
            PdfPCell cell44 = new PdfPCell(new Phrase($"上传文件Re-Open原因:{p.ReopenReason}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell44);
            PdfPCell cell45 = new PdfPCell(new Phrase($"上传文件Re-Open审批状态:{p.UploadReOpenState}", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell45);
            PdfPCell cell46 = new PdfPCell(new Phrase($"***************************审批流程***************************", font))
            {
                BorderWidth = 0,
                HorizontalAlignment = 0,
                Colspan = 4
            };
            table.AddCell(cell46);
            #endregion

            #region 表格
            table.SetWidths(new float[] { 1, 1, 2, 1 });
            PdfPCell cell_a1 = new PdfPCell(new Phrase("操作人", font));
            table.AddCell(cell_a1);
            PdfPCell cell_a2 = new PdfPCell(new Phrase("操作", font));
            table.AddCell(cell_a2);
            PdfPCell cell_a3 = new PdfPCell(new Phrase("审批意见", font));
            table.AddCell(cell_a3);
            PdfPCell cell_a4 = new PdfPCell(new Phrase("审批时间", font));
            table.AddCell(cell_a4);

            foreach (var item in _Approval)
            {
                table.AddCell(new PdfPCell(new Phrase(item.UserName, font)));
                table.AddCell(new PdfPCell(new Phrase(item.ActionType, font)));
                table.AddCell(new PdfPCell(new Phrase(item.Comments, font)));
                table.AddCell(new PdfPCell(new Phrase(item.ApproveDate, font)));
            }
            #endregion


            document.Add(table);
            document.Close();

            return File(Server.MapPath("~/UploadFileManagement/") + path + ".pdf", "application/pdf", path + ".pdf");
        }
        #endregion

        #region 上传文件查询详情
        public JsonResult DetailsLoad(string id)
        {
            var list = uploadFileQueryService.GetUpdateFileByID(id);  //通过ID查询上传文件详情          
            var obj = new List<P_UploadFileQuery_View>();
            var p = GetDisplayObj(list[0]);


            var Approval = uploadFileQueryService.GetApproval(id);
            var _Approval = new List<P_OrderApproveHistory_Time>();
            foreach (var i in Approval)
            {
                _Approval.Add(GetTimeObj(i));
            }

            return Json(new { state = 1, data = p, data1 = _Approval });
        }
        #endregion

        #region reopen
        /// <summary>
        /// RE-open
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadReopen(string UploadID, string reason, string remark, string originatorMUDID, string originatorName)
        {
            var oldID = UploadID;
            UploadID = UploadID.Replace(",", "','");
            var res = uploadOrderService.UpdateReopen(UploadID, CurrentAdminUser.Email, CurrentAdminUser.Name, reason, remark, originatorMUDID, originatorName);
            //发消息
            if (res > 0)
            {
                var idLists = oldID.Split(',');
                foreach (var item in idLists)
                {
                    var preUpload = uploadOrderService.LoadPreUploadOrder(Guid.Parse(item));
                    var preApproval = PreApprovalService.FindPreApprovalByHTCode(preUpload.HTCode).FirstOrDefault();
                    var Order = uploadOrderService.FindOrderByHTCode(preUpload.HTCode);
                    string applicantMsg = $"{preUpload.HTCode}，【{preApproval.HospitalName}】【{preApproval.MeetingDate.Value.ToString("yyyy/MM/dd")}】的会议支持文件已被Re-Open，原因：{reason}。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/Upload/Details/{item}'>点击这里</a>重新上传文件。";
                    var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(Order.IsTransfer == 0 ? preUpload.ApplierMUDID : Order.TransferUserMUDID, applicantMsg);
                    P_OrderApproveHistory history = new P_OrderApproveHistory();
                    if (remark.Contains("财务"))
                    {
                        history = new P_OrderApproveHistory()
                        {
                            ID = Guid.NewGuid(),
                            PID = preUpload.ID,
                            UserName = remark,
                            UserId = "",
                            ActionType = 2,
                            ApproveDate = DateTime.Now,
                            Comments = reason,
                            type = 2
                        };
                    }
                    else
                    {
                        history = new P_OrderApproveHistory()
                        {
                            ID = Guid.NewGuid(),
                            PID = preUpload.ID,
                            UserName = originatorName,
                            UserId = originatorMUDID,
                            ActionType = 2,
                            ApproveDate = DateTime.Now,
                            Comments = reason,
                            type = 2
                        };
                    }
                    var hisRes = uploadOrderService.AddOrderApproveHistory(history);
                }
            }
            return Json(new { state = 1 });

        }
        #endregion

        #region 上传文件重新分配

        #region 上传文件重新分配页面
        /// <summary>
        /// 上传文件重新分配页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Reassign()
        {
            return View();
        }
        #endregion

        #region 获取订单信息
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="MUID"></param>
        /// <param name="htCode"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadPreApproval(string MUID, string htCode, int page, int rows)
        {
            int total = 0;
            var list = uploadOrderService.LoadUploadOrder(MUID, htCode, rows, page, out total);
            var rtnList = new List<P_ORDER_VIEW>();
            foreach (var item in list)
            {
                rtnList.Add(GetOrderDisplayObj(item));
            }
            return Json(new { state = 1, rows = rtnList, total = total });
        }


        private P_ORDER_VIEW GetOrderDisplayObj(P_ORDER itm)
        {
            P_ORDER_VIEW rtnData = new P_ORDER_VIEW();
            rtnData.ID = itm.ID.ToString();
            switch (itm.State)
            {
                case 1:
                    rtnData.State = "订单待审批"; break;
                case 2:
                    rtnData.State = "订单审批被驳回"; break;
                case 3:
                    rtnData.State = "订单提交成功"; break;
                case 4:
                    rtnData.State = "预订成功"; break;
                case 5:
                    rtnData.State = "预订失败"; break;
                case 6:
                    rtnData.State = "已收餐"; break;
                case 7:
                    rtnData.State = "系统已收餐"; break;
                case 8:
                    rtnData.State = "未送达"; break;
                case 9:
                    rtnData.State = "已评价"; break;
                case 10:
                    rtnData.State = "申请退订"; break;
                case 11:
                    rtnData.State = "退订成功"; break;
                case 12:
                    rtnData.State = "退订失败"; break;
                default:
                    rtnData.State = string.Empty; break;
            }
            if (itm.IsRetuen != 0)
            {
                if (itm.IsRetuen == 1)
                {
                    rtnData.Return = "申请退单";
                }
                else if (itm.IsRetuen == 2)
                {
                    rtnData.Return = "退单成功";
                }
                else
                {
                    rtnData.Return = "退单失败";
                }
            }
            else
            {
                rtnData.Return = string.Empty;
            }
            rtnData.IsNormal = itm.IsSpecialOrder == 3 ? "否" : "是";
            if (itm.IsSpecialOrder == 1)
            {
                rtnData.SpecialOrder = "呼叫中心操作退单";
            }
            else if (itm.IsSpecialOrder == 2)
            {
                rtnData.SpecialOrder = "会议支持文件丢失";
            }
            else if (itm.IsSpecialOrder == 3)
            {
                rtnData.SpecialOrder = "未送达，会议未正常召开";
            }
            else
            {
                rtnData.SpecialOrder = string.Empty;
            }

            rtnData.HTCode = itm.CN != null ? itm.CN : string.Empty;
            rtnData.UserName = itm.Consignee != null ? itm.Consignee : string.Empty;
            rtnData.MUDID = itm.UserId != null ? itm.UserId : string.Empty;
            rtnData.Market = itm.Market != null ? itm.Market : string.Empty;
            rtnData.TA = itm.TA != null ? itm.TA : string.Empty;
            rtnData.Channel = itm.Channel != null ? itm.Channel.ToUpper() : string.Empty;
            rtnData.CallCenter = itm.Channel == "xms" ? "400-820-5577" : "400-6868-912";
            rtnData.AttendCount = itm.AttendCount.ToString();
            rtnData.RealCount = itm.RealCount != null ? itm.RealCount : string.Empty;
            rtnData.RealCountChangeReason = itm.RealCountChangeReason != null ? itm.RealCountChangeReason : string.Empty;
            rtnData.RealCountChangeRemrak = itm.RealCountChangeRemrak != null ? itm.RealCountChangeRemrak : string.Empty;
            rtnData.TotalPrice = itm.TotalPrice.ToString();
            if (itm.XmsTotalPrice > -1)
            {
                rtnData.RealityPrice = itm.XmsTotalPrice.ToString();
            }
            else
            {
                rtnData.RealityPrice = itm.TotalPrice.ToString();
            }
            rtnData.ChangeTotalPriceReason = itm.ChangeTotalPriceReason != null ? itm.ChangeTotalPriceReason : string.Empty;
            rtnData.RealPrice = itm.RealPrice != null ? itm.RealPrice : string.Empty;
            rtnData.RealPriceChangeReason = itm.RealPriceChangeReason != null ? itm.RealPriceChangeReason : string.Empty;
            rtnData.RealPriceChangeRemark = itm.RealPriceChangeRemark != null ? itm.RealPriceChangeRemark : string.Empty;
            rtnData.HospitalName = itm.HospitalName != null ? itm.HospitalName : string.Empty;
            rtnData.HospitalId = itm.HospitalId != null ? itm.HospitalId : string.Empty;
            rtnData.Address = itm.Address + itm.DeliveryAddress;
            rtnData.Consignee = itm.Consignee != null ? itm.Consignee : string.Empty;
            rtnData.Phone = itm.Phone != null ? itm.Phone : string.Empty;
            rtnData.DeliverTime = itm.DeliverTime.ToString("yyyy-MM-dd HH:mm:ss");
            rtnData.MeetingName = itm.MeetingName != null ? itm.MeetingName : string.Empty;
            rtnData.Remark = itm.Remark != null ? itm.Remark : string.Empty;
            rtnData.IsTransfer = itm.IsTransfer == 0 ? "否" : "是";
            rtnData.TransferOperatorMUDID = itm.TransferOperatorMUDID != null ? itm.TransferOperatorMUDID : string.Empty;
            rtnData.TransferOperatorName = itm.TransferOperatorName != null ? itm.TransferOperatorName : string.Empty;
            rtnData.TransferUserMUDID = itm.TransferUserMUDID != null ? itm.TransferUserMUDID : string.Empty;
            rtnData.TransferUserName = itm.TransferUserName != null ? itm.TransferUserName : string.Empty;
            rtnData.TransferOperateDate = itm.TransferOperateDate != null ? itm.TransferOperateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;


            if (itm.State == 6 || itm.State == 7 || itm.State == 8 || itm.State == 9)
            {
                if (itm.IsOrderUpload == 0)
                {
                    rtnData.IsShowTransfer = "1";
                }
                else
                {
                    var res = uploadOrderService.FindApproveState(itm.CN);
                    if (res)
                    {
                        rtnData.IsShowTransfer = "1";
                    }
                    else
                    {
                        rtnData.IsShowTransfer = "0";
                    }
                }
            }
            #region
            //重新分配 条件补充   ****** 韩利胜 18/06/12 10:00 ********   
            else if (itm.State == 4)
            {
                //预定成功状态
                rtnData.IsShowTransfer = "1";
            }
            //else if (itm.State == 9)
            //{
            //    //订单已评价
            //    if (itm.IsOrderUpload == 0)
            //    {
            //        //未上传文件
            //        rtnData.IsShowTransfer = "1";
            //    }
            //    else
            //    {
            //        rtnData.IsShowTransfer = "0";
            //    }
            //}
            #endregion 
            else
            {
                rtnData.IsShowTransfer = "0";
            }


            return rtnData;
        }
        #endregion

        #region 重新分配页面
        public ActionResult Pop(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Pops(string ids, string hts)
        {
            ViewBag.IDS = ids;
            ViewBag.HTS = hts;
            return View();
        }
        #endregion

        #region 保存上传文件重新分配
        public JsonResult UploadTransfer(Guid UploadID, string userId, string userName)
        {
            var res = uploadOrderService.UpdateTransfer(UploadID, CurrentAdminUser.Email, CurrentAdminUser.Name, userId, userName);
            return Json(new { state = 1 });
        }
        public JsonResult UploadTransfers(string UploadID, string htcode, string userId, string userName)
        {

            UploadID = UploadID.Replace(",", "','");
            //htcode = htcode.Replace(",", "','");
            //string[] strArray = htcode.Split(',');
            //string[] uplArray = UploadID.Split(',');
            var res = uploadOrderService.UpdateTransfers(UploadID, CurrentAdminUser.Email, CurrentAdminUser.Name, userId, userName);
            //for (int i = 0; i < strArray.Length; i++)
            //{
            //var cont = "上传文件重新分配:" + strArray[i];
            //var num = operationAuditService.AddAudit("3", cont);

            //}
            var cont = "上传文件重新分配:" + htcode;
            var num = operationAuditService.AddAudit("3", cont);

            return Json(new { state = 1 });
        }
        #endregion

        #endregion

        #region 审批流程时间处理
        private P_OrderApproveHistory_Time GetTimeObj(P_OrderApproveHistory itm)
        {
            P_OrderApproveHistory_Time rtnData = new Entity.P_OrderApproveHistory_Time();
            if (itm.ActionType == 1)
            {
                rtnData.ActionType = "上传文件提交成功";
            }
            if (itm.ActionType == 2)
            {
                rtnData.ActionType = "上传文件审批驳回";
            }
            if (itm.ActionType == 3)
            {
                rtnData.ActionType = "上传文件审批通过";
            }
            if (itm.ActionType == 4)
            {
                rtnData.ActionType = "上传文件修改成功";
            }

            rtnData.ApproveDate = itm.ApproveDate.Value.ToString("yyyy-MM-dd HH:mm");
            rtnData.Comments = itm.Comments;
            rtnData.ID = itm.ID;
            rtnData.PID = itm.PID;
            rtnData.type = itm.type;
            rtnData.UserId = itm.UserId;
            rtnData.UserName = itm.UserName;
            return rtnData;
        }
        #endregion

        #region 数据校验方法
        private P_UploadFileQuery_View GetDisplayObj(P_UploadFileQuery_TXT itm)
        {
            P_UploadFileQuery_View rtnData = new P_UploadFileQuery_View();
            rtnData.ID = itm.ID.ToString();
            rtnData.ApplierName = itm.ApplierName != null ? itm.ApplierName : string.Empty;   //申请人姓名
            rtnData.ApplierMUDID = itm.ApplierMUDID != null ? itm.ApplierMUDID : string.Empty;   //申请人MUDID
            rtnData.ApplierMobile = itm.ApplierMobile != null ? itm.ApplierMobile : string.Empty;   //申请人手机号码
            rtnData.HTCode = itm.HTCode != null ? itm.HTCode : string.Empty;   //HT编号
            rtnData.Market = itm.Market != null ? itm.Market : string.Empty;   //Market
            rtnData.VeevaMeetingID = itm.VeevaMeetingID != null ? itm.VeevaMeetingID : string.Empty;   //VeevaMeetingID
            rtnData.TA = itm.TA != null ? itm.TA : string.Empty;   //TA
            rtnData.Province = itm.Province != null ? itm.Province : string.Empty;   //省份
            rtnData.City = itm.City != null ? itm.City : string.Empty;   //城市
            rtnData.HospitalName = itm.HospitalName != null ? itm.HospitalName : string.Empty;   //医院名称
            rtnData.MeetingName = itm.MeetingName != null ? itm.MeetingName : string.Empty;   //会议名称
            rtnData.AttendCount = itm.AttendCount.ToString() != null ? itm.AttendCount.ToString() : "0";   //参会人数
            rtnData.CostCenter = itm.CostCenter != null ? itm.CostCenter : string.Empty;   //成本中心
            rtnData.Channel = itm.Channel != null ? itm.Channel : string.Empty;   //供应商
            rtnData.CN = itm.CN != null ? itm.CN : string.Empty;   //订单号
            rtnData.DeliverDate = itm.DeliverTime != null ? itm.DeliverTime.ToString("yyyy-MM-dd") : string.Empty;   //送餐日期
            rtnData.DeliverTime = itm.DeliverTime != null ? itm.DeliverTime.ToString("HH:mm:ss") : string.Empty;   //送餐时间
            rtnData.AttendCounts = itm.AttendCounts.ToString() != null ? itm.AttendCounts.ToString() : "0";   //用餐人数
            rtnData.FoodCount = itm.FoodCount.ToString() != null ? itm.FoodCount.ToString() : "0";   //总份数
            if (string.IsNullOrEmpty(itm.TotalPrice.ToString()) || itm.TotalPrice.ToString() == "0")   //预订金额
            {
                rtnData.TotalPrice = "0.00";
            }
            else
            {
                rtnData.TotalPrice = itm.TotalPrice.ToString("n");
            }
            if (string.IsNullOrEmpty(itm.XmsTotalPrice.ToString()) || itm.XmsTotalPrice.ToString() == "0")   //实际金额
            {
                rtnData.XmsTotalPrice = "0.00";
            }
            else if (itm.XmsTotalPrice.ToString() == "-1.00")
            {
                rtnData.XmsTotalPrice = itm.TotalPrice.ToString("n");
            }
            else
            {
                rtnData.XmsTotalPrice = itm.XmsTotalPrice.ToString("n");
            }

            rtnData.ChangeTotalPriceReason = itm.ChangeTotalPriceReason != null ? itm.ChangeTotalPriceReason : string.Empty;   //金额调整原因
            rtnData.ReceiveDate = itm.ReceiveDate != null ? itm.ReceiveDate.ToString("yyyy-MM-dd") : string.Empty;   //确认收餐日期
            rtnData.ReceiveTime = itm.ReceiveDate != null ? itm.ReceiveDate.ToString("HH:mm:ss") : string.Empty;   //确认收餐时间
            if (string.IsNullOrEmpty(itm.RealPrice) || itm.RealPrice.ToString() == "0")   //用户确认金额
            {
                rtnData.RealPrice = "0.00";
            }
            else
            {
                rtnData.RealPrice = itm.RealPrice;
            }
            rtnData.RealPriceChangeReason = itm.RealPriceChangeReason != null ? itm.RealPriceChangeReason : string.Empty;   //用户确认金额调整原因
            rtnData.RealPriceChangeRemark = itm.RealPriceChangeRemark != null ? itm.RealPriceChangeRemark : string.Empty;   //用户确认金额调整备注
            rtnData.RealCount = itm.RealCount != null ? itm.RealCount : "0";   //实际用餐人数
            rtnData.RealCountChangeReason = itm.RealCountChangeReason != null ? itm.RealCountChangeReason : string.Empty;   //实际用餐人数调整原因
            rtnData.RealCountChangeRemrak = itm.RealCountChangeRemrak != null ? itm.RealCountChangeRemrak : string.Empty;   //实际用餐人数调整备注
            switch (itm.State)  //订单状态
            {
                case 1:
                    rtnData.State = "订单待审批"; break;
                case 2:
                    rtnData.State = "订单审批被驳回"; break;
                case 3:
                    rtnData.State = "订单提交成功"; break;
                case 4:
                    rtnData.State = "预订成功"; break;
                case 5:
                    rtnData.State = "预订失败"; break;
                case 6:
                    rtnData.State = "已收餐"; break;
                case 7:
                    rtnData.State = "系统已收餐"; break;
                case 8:
                    rtnData.State = "未送达"; break;
                case 9:
                    rtnData.State = "已评价"; break;
                case 10:
                    rtnData.State = "申请退订"; break;
                case 11:
                    rtnData.State = "退单成功"; break;
                case 12:
                    rtnData.State = "退单失败"; break;
                default:
                    rtnData.State = string.Empty; break;
            }
            rtnData.IsOrderUpload = itm.IsOrderUpload == 1 ? "是" : "否";   //是否上传文件
            rtnData.IsReAssign = itm.IsReAssign == true ? "是" : "否";   //是否重新分配
            rtnData.ReAssignBUHeadName = itm.ReAssignBUHeadName != null ? itm.ReAssignBUHeadName : string.Empty;   //上传文件审批直线经理姓名
            rtnData.ReAssignBUHeadMUDID = itm.ReAssignBUHeadMUDID != null ? itm.ReAssignBUHeadMUDID : string.Empty;   //上传文件审批直线经理MUDID
            if (itm.Stated.ToString() == "1")   //上传文件审批状态
            {
                rtnData.Stated = "上传文件提交成功";
            }
            else if (itm.Stated.ToString() == "2")
            {
                rtnData.Stated = "上传文件审批被驳回";
            }
            else if (itm.Stated.ToString() == "3")
            {
                rtnData.Stated = "上传文件审批被财务reopen";
            }
            else if (itm.Stated.ToString() == "4")
            {
                rtnData.Stated = "上传文件审批通过";
            }
            else
            {
                rtnData.Stated = string.Empty;
            }
            rtnData.IsAttentSame = itm.IsAttentSame == 1 ? "是" : "否";   //签到人数是否等于实际用餐人数
            rtnData.AttentSameReason = itm.AttentSameReason != null ? itm.AttentSameReason : string.Empty;   //签到人数调整原因
            rtnData.IsReopen = itm.IsReopen == 1 ? "是" : "否";   //上传文件是否Re-Open
            rtnData.ReopenOperatorName = itm.ReopenOperatorName != null ? itm.ReopenOperatorName : string.Empty;   //上传文件Re-Open操作人
            rtnData.ReopenOperatorMUDID = itm.ReopenOperatorMUDID != null ? itm.ReopenOperatorMUDID : string.Empty;   //上传文件Re-Open操作人MUDID
            rtnData.ReopenOperateDate = itm.ReopenOperateDate != null ? itm.ReopenOperateDate.Value.ToString("yyyy-MM-dd") : string.Empty;   //上传文件Re-Open日期
            rtnData.ReopenOperateTime = itm.ReopenOperateDate != null ? itm.ReopenOperateDate.Value.ToString("HH:mm:ss") : string.Empty;   //上传文件Re-Open时间
            rtnData.ReopenReason = itm.ReopenReason != null ? itm.ReopenReason : string.Empty;   //上传文件Re-Open原因
            if (itm.UploadReOpenState.ToString() == "1")   //上传文件Re-Open审批状态
            {
                rtnData.UploadReOpenState = "上传文件提交成功";
            }
            else if (itm.UploadReOpenState.ToString() == "2")
            {
                rtnData.UploadReOpenState = "上传文件审批被驳回";
            }
            else if (itm.UploadReOpenState.ToString() == "3")
            {
                rtnData.UploadReOpenState = "上传文件审批被财务reopen";
            }
            else if (itm.UploadReOpenState.ToString() == "4")
            {
                rtnData.UploadReOpenState = "上传文件审批通过";
            }
            else
            {
                rtnData.UploadReOpenState = string.Empty;
            }

            string imageOne = string.Empty;
            string imageTwo = string.Empty;
            if (!string.IsNullOrEmpty(itm.MMCoEImageOne))
            {
                foreach (var imgItem in itm.MMCoEImageOne.Split(','))
                {
                    var imageOneSrc = ConfigurationManager.AppSettings["AWSService"].ToString() + imgItem;
                    imageOne += "<img src=" + imageOneSrc + " style='height:30px;' onclick=window.open('" + imageOneSrc + "') /> &nbsp;&nbsp;";   //签到表文件
                }
                itm.MMCoEImageOne = imageOne;
                rtnData.MMCoEImageOne = itm.MMCoEImageOne;
            }
            else
            {
                rtnData.MMCoEImageOne = string.Empty;
            }
            if (!string.IsNullOrEmpty(itm.MMCoEImageTwo))
            {
                foreach (var imgItem in itm.MMCoEImageTwo.Split(','))
                {
                    var imageTwoSrc = ConfigurationManager.AppSettings["AWSService"].ToString() + imgItem;
                    imageTwo += "<img src=" + imageTwoSrc + " style='height:30px;' onclick=window.open('" + imageTwoSrc + "') /> &nbsp;&nbsp;";   //签到表文件
                }
                itm.MMCoEImageTwo = imageTwo;
                rtnData.MMCoEImageTwo = itm.MMCoEImageTwo;
            }
            else
            {
                rtnData.MMCoEImageTwo = string.Empty;
            }
            if (!string.IsNullOrEmpty(itm.MMCoEImageThree))
            {
                foreach (var imgItem in itm.MMCoEImageThree.Split(','))
                {
                    var imageTwoSrc = ConfigurationManager.AppSettings["AWSService"].ToString() + imgItem;
                    imageTwo += "<img src=" + imageTwoSrc + " style='height:30px;' onclick=window.open('" + imageTwoSrc + "') /> &nbsp;&nbsp;";   //其他文件
                }
                itm.MMCoEImageThree = imageTwo;
                rtnData.MMCoEImageThree = itm.MMCoEImageThree;
            }
            else
            {
                rtnData.MMCoEImageThree = string.Empty;
            }
            rtnData.Position = itm.Position;
            rtnData.IsCancel = itm.IsCancel;
            rtnData.CancelState = itm.CancelState == null ? string.Empty : itm.CancelState;
            rtnData.IsReceive = itm.IsReceive;
            rtnData.IsMealSame = itm.IsMealSame == null ? string.Empty : itm.IsMealSame;
            rtnData.SpecialReason = itm.SpecialReason == null ? string.Empty : itm.SpecialReason;
            rtnData.ApproveDate = itm.ApproveDate == null ? string.Empty : itm.ApproveDate.Value.ToString("yyyy/MM/dd");
            rtnData.IsMeetingSame = itm.IsMeetingSame == null ? string.Empty : itm.IsMeetingSame;
            rtnData.MeetingSameReason = itm.MeetingSameReason == null ? string.Empty : itm.MeetingSameReason;
            rtnData.SpecialUploadReason = itm.SpecialUploadReason == null ? string.Empty : itm.SpecialUploadReason;
            return rtnData;
        }
        #endregion

        #region 导出上传文件查询
        public void ExportUploadList(string HTCode, string ApplierMUDID, string Begin, string End, string State)
        {
            #region 抓取数据    
            var list = uploadFileQueryService.ExportUploadFile(HTCode, ApplierMUDID, Begin, End, State);
            #endregion

            #region 构建Excel
            FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_UploadFileWeeklyReport.xls"), FileMode.Open, FileAccess.Read);
            //FileStream file11 = new FileStream(Server.MapPath("/Template/Template_UploadFileWeeklyReport.xls"), FileMode.Open, FileAccess.Read);
            HSSFWorkbook wk = new HSSFWorkbook(file11);

            ISheet sheet = wk.GetSheet("Report");

            #endregion

            #region 制作表体
            for (var i = 1; i <= list.Count; i++)
            {
                var item = list[i - 1];
                IRow row = sheet.CreateRow(i);
                ICell cell = null;
                var j = 0;
                if (item != null)
                {
                    cell = row.CreateCell(j);
                    cell.SetCellValue(item.ApplierName);   //申请人姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ApplierMUDID);   //申请人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Position);   //申请人职位
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ApplierMobile);   //申请人手机号码
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HTCode);   //HT编号
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Market);   //Market
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.VeevaMeetingID);   //VeevaMeetingID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.TA);   //TA
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Province);   //省份
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.City);   //城市
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.HospitalName);   //医院名称
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.MeetingName);   //会议名称
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.AttendCount);   //参会人数
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CostCenter);   //成本中心
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.Channel);   //供应商
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CN);   //订单号
                    cell = row.CreateCell(++j);
                    if (item.DeliverTime != null)
                    {
                        cell.SetCellValue(item.DeliverTime);   //送餐日期
                    }
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.DeliverTime == null ? "" : item.DeliverTime.ToString("HH:mm:ss"));   //送餐时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.AttendCount);   //用餐人数
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.FoodCount);   //总份数
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(Convert.ToDouble(item.TotalPrice));   //预订金额

                    string money = "";

                    if (string.IsNullOrEmpty(item.XmsTotalPrice.ToString()) || item.XmsTotalPrice.ToString() == "0")   //实际金额
                    {
                        money = "0.00";
                    }
                    else if (item.XmsTotalPrice.ToString() == "-1.00")
                    {
                        money = item.TotalPrice.ToString("n");
                    }
                    else
                    {
                        money = item.XmsTotalPrice.ToString("n");
                    }

                    cell = row.CreateCell(++j);
                    cell.SetCellValue(Convert.ToDouble(money));   //实际金额
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ChangeTotalPriceReason);   //金额调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsCancel);   //是否申请退单
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.CancelState);   //是否退单成功
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsReceive);   //是否收餐/未送达
                    cell = row.CreateCell(++j);
                    if (item.ReceiveDate != null)
                    {
                        cell.SetCellValue(item.ReceiveDate);   //确认收餐日期
                    }
                    //cell = row.CreateCell(++j);
                    //cell.SetCellValue(item.ReceiveDate == null ? "" : item.ReceiveDate.ToString("HH:mm:ss"));   //确认收餐时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(Convert.ToDouble(item.RealPrice));   //用户确认金额
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsMealSame);   //用户确认金额
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.RealPriceChangeReason);   //用户确认金额调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.RealPriceChangeRemark);   //用户确认金额调整备注
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.RealCount);   //实际用餐人数
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.RealCountChangeReason);   //实际用餐人数调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.RealCountChangeRemrak);   //实际用餐人数调整备注
                    cell = row.CreateCell(++j);
                    var numState = int.Parse(item.State.ToString());
                    string strState = "";
                    switch (numState)
                    {
                        case 1:
                            strState = "订单待审批";
                            break;
                        case 2:
                            strState = "订单审批被驳回";
                            break;
                        case 3:
                            strState = "订单提交成功";
                            break;
                        case 4:
                            strState = "预订成功";
                            break;
                        case 5:
                            strState = "预订失败";
                            break;
                        case 6:
                            strState = "已收餐";
                            break;
                        case 7:
                            strState = "系统已收餐";
                            break;
                        case 8:
                            strState = "未送达";
                            break;
                        case 9:
                            strState = "已评价";
                            break;
                        case 10:
                            strState = "申请退订";
                            break;
                        case 11:
                            strState = "退订成功";
                            break;
                        case 12:
                            strState = "退订失败";
                            break;
                        default:
                            break;
                    }
                    cell.SetCellValue(strState);   //订单状态
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.SpecialReason);
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsOrderUpload == 1 ? "是" : "否");   //是否上传文件
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsReAssign == true ? "是" : "否");   //是否重新分配
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReAssignBUHeadName);   //上传文件审批直线经理姓名
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReAssignBUHeadMUDID);   //上传文件审批直线经理MUDID
                    cell = row.CreateCell(++j);
                    if (item.ApproveDate != null)
                    {
                        cell.SetCellValue(item.ApproveDate.Value);   //上传文件审批日期
                    }
                    cell = row.CreateCell(++j);
                    var numStateMU = int.Parse(item.Stated.ToString());
                    string strStateMU = "";
                    switch (numStateMU)
                    {
                        case 1:
                            strStateMU = "上传文件提交成功";
                            break;
                        case 2:
                            strStateMU = "上传文件审批被驳回";
                            break;
                        case 3:
                            strStateMU = "上传文件审批被财务reopen";
                            break;
                        case 4:
                            strStateMU = "上传文件审批通过";
                            break;
                        default:
                            break;
                    }
                    cell.SetCellValue(strStateMU);   // 上传文件审批状态
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsAttentSame == 1 ? "是" : "否");   //签到人数是否等于实际用餐人数
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.AttentSameReason);   //签到人数调整原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsMeetingSame);   //是否与会议信息一致
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.MeetingSameReason);   //会议信息不一致原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.SpecialUploadReason);   //退单原因/未送达，会议未正常召开原因/会议文件丢失原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.IsReopen == 1 ? "是" : "否");   //上传文件是否Re-Open
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReopenOperatorName);   //上传文件Re-Open操作人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReopenOperatorMUDID);   //上传文件Re-Open操作人MUDID
                    cell = row.CreateCell(++j);
                    if (item.ReopenOperateDate != null)
                    {
                        cell.SetCellValue(item.ReopenOperateDate.Value);   //上传文件Re-Open日期
                    }
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReopenOperateDate == null ? "" : item.ReopenOperateDate.Value.ToString("HH:mm:ss"));    //上传文件Re-Open时间
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReopenOriginatorName);   //上传文件Re-Open操作人
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReopenOriginatorMUDID);   //上传文件Re-Open操作人MUDID
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReopenReason);   //上传文件Re-Open原因
                    cell = row.CreateCell(++j);
                    cell.SetCellValue(item.ReopenRemark);   //上传文件Re-Open原因
                    cell = row.CreateCell(++j);
                    var numStateOPen = int.Parse(item.UploadReOpenState.ToString());
                    string strStateOpen = "";
                    switch (numStateOPen)
                    {
                        case 1:
                            strStateOpen = "上传文件提交成功";
                            break;
                        case 2:
                            strStateOpen = "上传文件审批被驳回";
                            break;
                        case 3:
                            strStateOpen = "上传文件审批被财务reopen";
                            break;
                        case 4:
                            strStateOpen = "上传文件审批通过";
                            break;
                        default:
                            break;
                    }
                    cell.SetCellValue(strStateOpen);   //上传文件Re-Open审批状态
                }

            }
            #endregion

            #region 写入到客户端
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                wk.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                Response.ContentType = "application/vnd.ms-excel";
                Response.BinaryWrite(ms.ToArray());
            }
            #endregion
        }
        #endregion

        #region 定时导出上传文件
        public void TimingExport()
        {
            //查询列表
            var list = uploadFileQueryService.TimingExpotr();
            try
            {
                //用于临时存储文件
                var tempFilePath1 = Server.MapPath("~/Temp");
                if (!Directory.Exists(tempFilePath1))//判断文件夹是否存在 
                {
                    Directory.CreateDirectory(tempFilePath1);//不存在则创建文件夹 
                }
                var tempFilePath2 = Server.MapPath("~/TempZip");
                if (!Directory.Exists(tempFilePath2))//判断文件夹是否存在 
                {
                    Directory.CreateDirectory(tempFilePath2);//不存在则创建文件夹 
                }
                DirectoryInfo dir = new DirectoryInfo(tempFilePath1);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                                                                       //遍历删除文件   防止临时文件过多。
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        System.IO.File.Delete(i.FullName);      //删除指定文件
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("导出上传文件图片-操作文件错误", e);
            }
            //s3 下载文件
            var s3Handler = new S3Handler();
            string dayOfWeekPath = string.Empty;
            foreach (var record in list)
            {
                dayOfWeekPath = record.BUHeadApproveDate.DayOfWeek.ToString();
                var tempFilePath = Server.MapPath("~/Temp/") + dayOfWeekPath;
                if (!Directory.Exists(tempFilePath))//判断文件夹是否存在 
                {
                    Directory.CreateDirectory(tempFilePath);//不存在则创建文件夹 
                }
                //判断图片不为空
                if (!string.IsNullOrEmpty(record.MMCoEImageOne))   //签到表文件
                {
                    //多个图片是用 , 拼接路径
                    var imgList = record.MMCoEImageOne.Split(',');
                    for (var i = 0; i < imgList.Length; i++)
                    {
                        var fileName = i + 1;
                        //删掉字符串中的 /
                        var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                        string extension = Path.GetExtension(imgList[i]);
                        s3Handler.S3Download(s3key, Server.MapPath("~/Temp/") + dayOfWeekPath + "/" + record.HTCode + "/Sign-in Sheet/" + "UploadFile" + fileName + extension);
                    }
                }
                if (!string.IsNullOrEmpty(record.MMCoEImageTwo))   //现场照片文件
                {
                    //多个图片是用 , 拼接路径
                    var imgList = record.MMCoEImageTwo.Split(',');
                    for (var i = 0; i < imgList.Length; i++)
                    {
                        var fileName = i + 1;
                        //删掉字符串中的 /
                        var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                        string extension = Path.GetExtension(imgList[i]);
                        s3Handler.S3Download(s3key, Server.MapPath("~/Temp/") + dayOfWeekPath + "/" + record.HTCode + "/Meeting Photo/" + "UploadFile" + fileName + extension);
                    }
                }
                if (!string.IsNullOrEmpty(record.MMCoEImageThree))   //其他文件
                {
                    //多个图片是用 , 拼接路径
                    var imgList = record.MMCoEImageThree.Split(',');
                    for (var i = 0; i < imgList.Length; i++)
                    {
                        var fileName = i + 1;
                        //删掉字符串中的 /
                        var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                        string extension = Path.GetExtension(imgList[i]);
                        s3Handler.S3Download(s3key, Server.MapPath("~/Temp/") + dayOfWeekPath + "/" + record.HTCode + "/OtherImage/" + "UploadFile" + fileName + extension);
                    }
                }
            }

            //添加文件夹下的所有文件到压缩包里
            var tempFilePathAfter = Server.MapPath("~/Temp");
            DirectoryInfo dirAfter = new DirectoryInfo(tempFilePathAfter);
            FileSystemInfo[] fileinfoAfter = dirAfter.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfoAfter)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    using (ZipFile zip = new ZipFile(System.Text.Encoding.Default))
                    {
                        zip.AddDirectory(Server.MapPath("~/Temp/") + i.Name);
                        zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                        var fileId = Guid.NewGuid();
                        var tempFilePathZip = Server.MapPath("~/TempZip/") + i.Name;
                        if (!Directory.Exists(tempFilePathZip))//判断文件夹是否存在 
                        {
                            Directory.CreateDirectory(tempFilePathZip);//不存在则创建文件夹 
                        }
                        zip.Save(Server.MapPath("~/TempZip/") + i.Name + "\\" + fileId + ".zip");

                        //每周导出一次,导出当前日期前七天的
                        using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                        {
                            var key = string.Format("UploadDocumentReport/") + i.Name + "UploadDocument.zip";
                            //var request = new PutObjectRequest()
                            //{
                            //    BucketName = bucketName,
                            //    CannedACL = S3CannedACL.PublicRead,
                            //    Key = key,
                            //    InputStream = new FileStream(Server.MapPath("~/TempZip/") + fileId + ".zip", FileMode.Open, FileAccess.Read)
                            //};
                            //client.PutObject(request);

                            // Create list to store upload part responses.
                            List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();

                            // Setup information required to initiate the multipart upload.
                            InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
                            {
                                BucketName = bucketName,
                                Key = key,
                                CannedACL = S3CannedACL.PublicRead,
                            };

                            // Initiate the upload.
                            InitiateMultipartUploadResponse initResponse = client.InitiateMultipartUpload(initiateRequest);

                            // Upload parts.
                            string filePath = Server.MapPath("~/TempZip/") + i.Name + "\\" + fileId + ".zip";
                            long contentLength = new FileInfo(filePath).Length;
                            long partSize = 5 * (long)Math.Pow(2, 20); // 5 MB

                            try
                            {
                                long filePosition = 0;
                                for (int ii = 1; filePosition < contentLength; ii++)
                                {
                                    UploadPartRequest uploadRequest = new UploadPartRequest
                                    {
                                        BucketName = bucketName,
                                        Key = key,
                                        UploadId = initResponse.UploadId,
                                        PartNumber = ii,
                                        PartSize = partSize,
                                        FilePosition = filePosition,
                                        FilePath = filePath
                                    };

                                    // Upload a part and add the response to our list.
                                    uploadResponses.Add(client.UploadPart(uploadRequest));

                                    filePosition += partSize;
                                }

                                // Setup to complete the upload.
                                CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest
                                {
                                    BucketName = bucketName,
                                    Key = key,
                                    UploadId = initResponse.UploadId
                                };
                                completeRequest.AddPartETags(uploadResponses);

                                // Complete the upload.
                                CompleteMultipartUploadResponse completeUploadResponse = client.CompleteMultipartUpload(completeRequest);
                            }
                            catch (Exception exception)
                            {
                                // Abort the upload.
                                AbortMultipartUploadRequest abortMPURequest = new AbortMultipartUploadRequest
                                {
                                    BucketName = bucketName,
                                    Key = key,
                                    UploadId = initResponse.UploadId
                                };
                                client.AbortMultipartUploadAsync(abortMPURequest);
                            }
                        }
                    }
                }
            }

            //历史导出保存
            //using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
            //{

            //    string startTime = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            //    string endTime = DateTime.Now.ToString("yyyy-MM-dd");
            //    string time = startTime.Substring(0, 7);
            //    var key = string.Format("UploadFileReportHistory/" + time + "/" + startTime + "--" + endTime + "/WeeklyUploadFiles.zip");
            //    var request = new PutObjectRequest()
            //    {
            //        BucketName = bucketName,
            //        CannedACL = S3CannedACL.PublicRead,
            //        Key = key,
            //        InputStream = new FileStream(Server.MapPath("~/TempZip/") + fileId + ".zip", FileMode.Open, FileAccess.Read)
            //    };
            //    client.PutObject(request);
            //}
            //var tempFilePath1 = Server.MapPath("~/upload/image");
            //DirectoryInfo dirAfter = new DirectoryInfo(tempFilePath1);
            //FileSystemInfo[] fileinfoAfter = dirAfter.GetFileSystemInfos();
            //foreach (FileSystemInfo i in fileinfoAfter)
            //{
            //    using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
            //    {

            //        var key = string.Format("UPLOADS/{0}", i.Name);
            //        var request = new PutObjectRequest()
            //        {
            //            BucketName = bucketName,
            //            CannedACL = S3CannedACL.PublicRead,
            //            Key = key,
            //            InputStream = new FileStream(tempFilePath1 + $@"\{i.Name}", FileMode.Open, FileAccess.Read)
            //        };
            //        client.PutObject(request);
            //    }
            //}
        }

        #endregion

        #region 生成随机数字方法
        /// <summary>
        /// 生成随机数字方法
        /// </summary>
        /// <param name="no">生成位数</param>
        /// <returns></returns>
        public string GetHTCodeNo(int no)
        {
            char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(10);
            Random rd = new Random();
            for (int i = 0; i < no; i++)
            {
                newRandom.Append(constant[rd.Next(10)]);
            }
            return newRandom.ToString();
        }
        #endregion

        #region 导出上传文件图片
        /// <summary>
        /// 导出上传文件图片
        /// </summary>
        /// <param name="HTCode"></param>
        /// <param name="ApplierMUDID"></param>
        /// <param name="Begin"></param>
        /// <param name="End"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public JsonResult ExportUploadFileImage(string HTCode, string ApplierMUDID, string Begin, string End, string State)
        {
            var total = 0;
            //查询列表
            var list = uploadFileQueryService.LoadPage(HTCode, ApplierMUDID, Begin, End, State, int.MaxValue, 1, out total);
            AdminUser adminUser = Session[ConstantHelper.CurrentAdminUser] as AdminUser;
            UploadFileDownloadImage uploadFileDownloadImage = new UploadFileDownloadImage(list, Server.MapPath("~/Temp"), adminUser.Email, PreApprovalService);
            Thread thread = new Thread(uploadFileDownloadImage.InitImage);
            thread.Start();
            return Json(new
            {
                state = 1,
                message = "系统正在生成文件，稍后将以邮件方式发送下载链接，请注意查收邮件。"
            });
        }
        /// <summary>
        /// 结合上面方法使用
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ActionResult DownloadByPath(string filePath)
        {
            return File(filePath, "application/zip", "ExportImg.zip");
        }
        #endregion

        #region 转换
        static string FormatterNull(string str)
        {
            return str == null ? string.Empty : str;
        }

        static string FormatterNull(DateTime str)
        {
            if (str.Ticks == 0)
            {
                return string.Empty;
            }
            return str.ToString("yyyy-MM-dd");
        }

        static string FormatterNull(TimeSpan str)
        {
            if (str.Ticks == 0)
            {
                return string.Empty;
            }
            return $"{string.Format("{0:D2}", str.Hours)}:{string.Format("{0:D2}", str.Minutes)}:{string.Format("{0:D2}", str.Seconds)}";
        }

        static string FormatterNull(int str)
        {
            return $"{str}";
        }

        static string FormatterNull(decimal str)
        {
            return $"{str}";
        }
        #endregion

        #region 根据userId查找用户信息
        /// <summary>
        /// 根据userId查找用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public JsonResult Find(string userId)
        {
            var res = userInfoService.Find(userId);
            return Json(new { state = 1, data = res });
        }
        #endregion

        #region 导出Re-Open模板
        /// <summary>
        /// 导出Re-Open模板
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportReport()
        {

            XSSFWorkbook book = new XSSFWorkbook();
            #region var headerStyle = book.CreateCellStyle();
            var headerStyle = book.CreateCellStyle();

            var headerFontStyle = book.CreateFont();
            headerFontStyle.Color = NPOI.HSSF.Util.HSSFColor.White.Index;
            headerFontStyle.Boldweight = short.MaxValue;
            headerFontStyle.FontHeightInPoints = 10;

            headerStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
            headerStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightOrange.Index;
            headerStyle.Alignment = HorizontalAlignment.Center;
            headerStyle.SetFont(headerFontStyle);
            #endregion
            var sheet = book.CreateSheet("report");
            var row = sheet.CreateRow(0);
            #region header
            sheet.SetColumnWidth(0, 20 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);
            sheet.SetColumnWidth(5, 20 * 256);
            sheet.SetColumnWidth(6, 20 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 20 * 256);


            var cell = row.CreateCell(0);
            cell.SetCellValue("申请人姓名");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(1);
            cell.SetCellValue("申请人MUDID");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(2);
            cell.SetCellValue("申请人手机号码");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(3);
            cell.SetCellValue("HT编号");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(4);
            cell.SetCellValue("会议日期");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(5);
            cell.SetCellValue("Re-Open发起人姓名");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(6);
            cell.SetCellValue("Re-Open发起人MUDID");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(7);
            cell.SetCellValue("Re-Open原因");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(8);
            cell.SetCellValue("Re-Open备注");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(9);
            cell.SetCellValue("要求Re-open日期");
            cell.CellStyle = headerStyle;
            cell = row.CreateCell(10);
            cell.SetCellValue("备注");
            cell.CellStyle = headerStyle;
            #endregion

            byte[] excelData;
            using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
            {
                book.Write(_ms);
                excelData = _ms.ToArray();
                //_ms.Close();
            }

            ViewBag.Msg = "导出成功！";
            return File(excelData, "application/vnd.ms-excel", HttpUtility.UrlEncode("UploadFile_" + DateTime.Now.ToString("yyyyMMdd") + GetHTCodeNo(4) + ".xlsx", System.Text.Encoding.UTF8));

        }
        #endregion



        #region 批量Re-Open导入
        /// <summary>
        /// 批量Re-Open导入
        /// </summary>
        /// <returns></returns>
        public JsonResult Import(HttpPostedFileBase file)
        {
            #region 解析为Excel格式对象
            var workbook = new XSSFWorkbook(file.InputStream);
            var sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
            }
            #endregion

            #region 判断表头是否符合格式
            var row = sheet.GetRow(0);
            if (row == null)
            {
                return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
            }

            var titleTemplate = "申请人姓名,申请人MUDID,申请人手机号码,HT编号,会议日期,Re-Open发起人姓名,Re-Open发起人MUDID,Re-Open原因,Re-Open备注,要求Re-open日期,备注".Split(',');
            var titleValues = new string[9];

            for (var i = 0; i <9; i++)
            {
                titleValues[i] = row.GetCell(0) != null ? row.GetCell(i).StringCellValue : string.Empty;
                if (titleValues[i] != titleTemplate[i])
                {
                    return Json(new { state = 0, txt = "导入的文件格式不正确" }, "text/html", JsonRequestBehavior.AllowGet);
                }
            }
            #endregion           

            #region 读取表体
            var excelRows = new List<P_REOPEN_VIEW>();
            string FailRemarktxt = "";
            string Failtxt = "";
            string strHTCode = "";
            string subHTCode = "";
            var sRows = new List<P_REOPEN_VIEW>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                row = sheet.GetRow(i);
                if (row == null) continue;                
                string celltxt = GetStringFromCell(row.GetCell(0)) + GetStringFromCell(row.GetCell(1)) + GetStringFromCell(row.GetCell(2)) + GetStringFromCell(row.GetCell(3)) + GetStringFromCell(row.GetCell(4))
                    + GetStringFromCell(row.GetCell(5)) + GetStringFromCell(row.GetCell(6)) + GetStringFromCell(row.GetCell(7)) + GetStringFromCell(row.GetCell(8)) + GetStringFromCell(row.GetCell(9));
                if (string.IsNullOrEmpty(celltxt)) break;
                if (string.IsNullOrEmpty(GetStringFromCell(row.GetCell(3))) || string.IsNullOrEmpty(GetStringFromCell(row.GetCell(1))) || string.IsNullOrEmpty(GetStringFromCell(row.GetCell(8))))
                {
                    return Json(new { state = 0, txt = "发现有空值的列" + "</br>" + "请检查：申请人MUDID或HT编号或Re-Open备注" }, "text/html", JsonRequestBehavior.AllowGet);
                }
                if (GetStringFromCell(row.GetCell(8)) != "财务线下审批驳回" && GetStringFromCell(row.GetCell(8)) != "直线经理线下审批驳回")
                {
                    FailRemarktxt += GetStringFromCell(row.GetCell(3)) + " Re-Open备注校验失败，请检查该项" + "</br>";
                }
                //获取Excel中HTCode拼接字符串
                strHTCode += "'" + GetStringFromCell(row.GetCell(3)) + "',";
                excelRows.Add(new P_REOPEN_VIEW()
                {
                    ApplierMUDID = GetStringFromCell(row.GetCell(1)),
                    HTCode = GetStringFromCell(row.GetCell(3)),
                    OriginatorName = GetStringFromCell(row.GetCell(5)),
                    OriginatorMUDID = GetStringFromCell(row.GetCell(6)),
                    ReopenReason = GetStringFromCell(row.GetCell(7)),
                    ReopenRemark = GetStringFromCell(row.GetCell(8)),
                    CurrentUserId = CurrentAdminUser.Email,
                    CurrentUserName = CurrentAdminUser.Name
                });
            }
            //截取掉最后一个","得到的字符串
            subHTCode = strHTCode.Substring(0, strHTCode.Length - 1);
            //查询符合[P_PreUploadOrder]表中的HTCode
            var inHTCode = uploadOrderService.LoadDataByInHTCode(subHTCode);
            if (inHTCode != null && inHTCode.Count > 0)
            {
                foreach (var s in excelRows)
                {
                    var inData = inHTCode.Where(p => p.HTCode.Trim() == s.HTCode && p.ApplierMUDID.Trim() == s.ApplierMUDID);
                    if (inData == null || inData.Count() == 0)
                    {
                        Failtxt += s.HTCode + "/" + s.ApplierMUDID + "校验失败，请检查该项" + "</br>";
                    }
                    else
                    {
                        sRows.Add(new P_REOPEN_VIEW()
                        {
                            ApplierMUDID = s.ApplierMUDID,
                            HTCode = s.HTCode,
                            OriginatorName = s.OriginatorName,
                            OriginatorMUDID = s.OriginatorMUDID,
                            ReopenReason = s.ReopenReason,
                            ReopenRemark = s.ReopenRemark,
                            CurrentUserId = s.CurrentUserId,
                            CurrentUserName = s.CurrentUserName
                        });
                    }
                }
            }
            else
            {
                //Excel中所有HTCode都不在P_PreUploadOrder表中
                foreach (var s in excelRows)
                {
                    Failtxt += s.HTCode + "/" + s.ApplierMUDID + "校验失败，请检查该项" + "</br>";
                }
            }
            if (FailRemarktxt != "" || Failtxt != "")
            {
                return Json(new { state = 0, txt = Failtxt + FailRemarktxt }, "text/html", JsonRequestBehavior.AllowGet);
            }
            //var HTCode = excelRows.Select(p => new { HTCode = p.HTCode, ApplierMUDID=p.ApplierMUDID }).ToList();
            //var listHTCode = uploadOrderService.LoadHTCode();
            ////不存在于[P_PreUploadOrder]表中的HTCode
            //var NoHTCode
            //    = HTCode.Select(a => new { a.HTCode, a.ApplierMUDID }).ToList()
            //    .Except(listHTCode.Select(a => new { a.HTCode, a.ApplierMUDID }).ToList())
            //.Select(a => new NoHTCode
            //{
            //    HTCode = a.HTCode,
            //    ApplierMUDID = a.ApplierMUDID
            //}).ToList();
            //foreach (var s in NoHTCode)
            //{
            //    Failtxt+= s.HTCode +"/"+s.ApplierMUDID+ "校验失败，请检查该项" + "</br>";
            //}

            #endregion

            // 文件中是否有重复数据
            var listRepeat = sRows.GroupBy(a => a.HTCode).Where(a => a.Count() > 1).Select(a => a.Key).ToList();
            if (listRepeat.Count() > 0)
            {
                return Json(new { state = 0, txt = "Excel中发现HTCode重复数据", data = listRepeat }, "text/html", JsonRequestBehavior.AllowGet);
            }

            var fails = new List<P_REOPEN_VIEW>();
            uploadOrderService.Import(sRows, ref fails);
            foreach (var item in sRows)
            {
                var preApproval = PreApprovalService.FindPreApprovalByHTCode(item.HTCode).FirstOrDefault();
                var preUploader = uploadOrderService.FindActivityOrderByHTCode(item.HTCode);
                var Order = uploadOrderService.FindOrderByHTCode(item.HTCode);
                string applicantMsg = $"{preApproval.HTCode}，【{preApproval.HospitalName}】【{preApproval.MeetingDate.Value.ToString("yyyy/MM/dd")}】的会议支持文件已被Re-Open，原因：{item.ReopenReason}。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/Upload/Details/{preUploader.ID}'>点击这里</a>重新上传文件。";
                var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(Order.IsTransfer == 0 ? preApproval.ApplierMUDID : Order.TransferUserMUDID, applicantMsg);

                P_OrderApproveHistory history = new P_OrderApproveHistory();

                if (item.ReopenRemark.Contains("财务"))
                {
                    history = new P_OrderApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = preUploader.ID,
                        UserName = item.ReopenRemark,
                        UserId = "",
                        ActionType = 2,
                        ApproveDate = DateTime.Now,
                        Comments = item.ReopenReason,
                        type = 2
                    };
                }
                else
                {
                    history = new P_OrderApproveHistory()
                    {
                        ID = Guid.NewGuid(),
                        PID = preUploader.ID,
                        UserName = item.OriginatorName,
                        UserId = item.OriginatorMUDID,
                        ActionType = 2,
                        ApproveDate = DateTime.Now,
                        Comments = item.ReopenReason,
                        type = 2
                    };
                }

                var hisRes = uploadOrderService.AddOrderApproveHistory(history);
            }

            return Json(new { state = 1 }, "text/html", JsonRequestBehavior.AllowGet);
        }
        #endregion

        private string GetStringFromCell(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                default:
                    return string.Empty;
            }
        }

        public class UploadFileDownloadImage
        {
            private List<P_UploadFileQuery_TXT> UploadFileList;
            private string tempFilePath1;
            private string email;

            private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
            private static string awsSecretKey = "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";
            private static string bucketName = "wechat";
            private IPreApprovalService PreApprovalService;
            AmazonS3Config config = new AmazonS3Config()
            {
                ServiceURL = "http://s3.amazonaws.com",
                RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
            };
            public UploadFileDownloadImage(List<P_UploadFileQuery_TXT> UploadFileList, string tempFilePath1, string email, IPreApprovalService PreApprovalService)
            {
                this.UploadFileList = UploadFileList;
                this.tempFilePath1 = tempFilePath1;
                this.email = email;
                this.PreApprovalService = PreApprovalService;
            }
            public void InitImage()
            {
                try
                {
                    if (!Directory.Exists(tempFilePath1))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath1);//不存在则创建文件夹 
                    }
                }
                catch (Exception e)
                {
                    LogHelper.Error("导出讲者服务协议图片-操作文件错误", e);
                }
                //s3 下载文件
                var s3Handler = new S3Handler();
                var time = DateTime.Now.ToString("yyyyMMddhhmmss");
                foreach (var record in UploadFileList)
                {
                    //判断图片不为空
                    if (!string.IsNullOrEmpty(record.MMCoEImageOne))   //签到表文件
                    {
                        //多个图片是用 , 拼接路径
                        var imgList = record.MMCoEImageOne.Split(',');
                        for (var i = 0; i < imgList.Length; i++)
                        {
                            var fileName = i + 1;
                            //删掉字符串中的 /
                            var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                            string extension = Path.GetExtension(imgList[i]);
                            s3Handler.S3Download(s3key, tempFilePath1 + "/" + time + "/" + record.HTCode + "/Sigh-in Sheet/" + "UploadFile" + fileName + extension);
                        }
                    }
                    if (!string.IsNullOrEmpty(record.MMCoEImageTwo))   //现场照片文件
                    {
                        //多个图片是用 , 拼接路径
                        var imgList = record.MMCoEImageTwo.Split(',');
                        for (var i = 0; i < imgList.Length; i++)
                        {
                            var fileName = i + 1;
                            //删掉字符串中的 /
                            var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                            string extension = Path.GetExtension(imgList[i]);
                            s3Handler.S3Download(s3key, tempFilePath1 + "/" + time + "/" + record.HTCode + "/Meeting Photo/" + "UploadFile" + fileName + extension);
                        }
                    }
                    if (!string.IsNullOrEmpty(record.MMCoEImageThree))   //其他文件
                    {
                        //多个图片是用 , 拼接路径
                        var imgList = record.MMCoEImageThree.Split(',');
                        for (var i = 0; i < imgList.Length; i++)
                        {
                            var fileName = i + 1;
                            //删掉字符串中的 /
                            var s3key = imgList[i].Substring(1, imgList[i].Length - 1);
                            string extension = Path.GetExtension(imgList[i]);
                            s3Handler.S3Download(s3key, tempFilePath1 + "/" + time + "/" + record.HTCode + "/OtherImage/" + "UploadFile" + fileName + extension);
                        }
                    }
                }
                //添加文件夹下的所有文件到压缩包里
                var tempFilePathAfter = tempFilePath1 + "/" + time + "/";
                using (ZipFile zip = new ZipFile(System.Text.Encoding.Default))
                {
                    zip.AddDirectory(tempFilePathAfter);
                    zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                    var fileId = Guid.NewGuid();
                    if (!Directory.Exists(tempFilePath1 + "Zip/" + time + "/"))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(tempFilePath1 + "Zip/" + time + "/");//不存在则创建文件夹 
                    }
                    zip.Save(tempFilePath1 + "Zip/" + time + "/" + fileId + ".zip");

                    //每周导出一次,导出当前日期前七天的
                    using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                    {
                        var key = string.Format("PreApprovalCache/") + fileId + ".zip";
                        //var request = new PutObjectRequest()
                        //{
                        //    BucketName = bucketName,
                        //    CannedACL = S3CannedACL.PublicRead,
                        //    Key = key,
                        //    InputStream = new FileStream(Server.MapPath("~/TempZip/") + fileId + ".zip", FileMode.Open, FileAccess.Read)
                        //};
                        //client.PutObject(request);

                        // Create list to store upload part responses.
                        List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();

                        // Setup information required to initiate the multipart upload.
                        InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
                        {
                            BucketName = bucketName,
                            Key = key,
                            CannedACL = S3CannedACL.PublicRead,
                        };

                        // Initiate the upload.
                        InitiateMultipartUploadResponse initResponse = client.InitiateMultipartUpload(initiateRequest);

                        // Upload parts.
                        string filePath = tempFilePath1 + "Zip/" + time + "/" + fileId + ".zip";
                        long contentLength = new FileInfo(filePath).Length;
                        long partSize = 5 * (long)Math.Pow(2, 20); // 5 MB

                        try
                        {
                            long filePosition = 0;
                            for (int ii = 1; filePosition < contentLength; ii++)
                            {
                                UploadPartRequest uploadRequest = new UploadPartRequest
                                {
                                    BucketName = bucketName,
                                    Key = key,
                                    UploadId = initResponse.UploadId,
                                    PartNumber = ii,
                                    PartSize = partSize,
                                    FilePosition = filePosition,
                                    FilePath = filePath
                                };

                                // Upload a part and add the response to our list.
                                uploadResponses.Add(client.UploadPart(uploadRequest));

                                filePosition += partSize;
                            }

                            // Setup to complete the upload.
                            CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest
                            {
                                BucketName = bucketName,
                                Key = key,
                                UploadId = initResponse.UploadId,
                            };
                            completeRequest.AddPartETags(uploadResponses);

                            // Complete the upload.
                            CompleteMultipartUploadResponse completeUploadResponse = client.CompleteMultipartUpload(completeRequest);

                            //发送邮件
                            PreApprovalService.InsertFileLink("http://wechat.s3.cn-north-1.amazonaws.com.cn/PreApprovalCache/" + fileId + ".zip", email);
                        }
                        catch (Exception exception)
                        {
                            // Abort the upload.
                            AbortMultipartUploadRequest abortMPURequest = new AbortMultipartUploadRequest
                            {
                                BucketName = bucketName,
                                Key = key,
                                UploadId = initResponse.UploadId
                            };
                            client.AbortMultipartUploadAsync(abortMPURequest);
                        }
                    }
                }
                DirectoryInfo dir = new DirectoryInfo(tempFilePath1 + "/" + time);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
                                                                       //遍历删除文件   防止临时文件过多。
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)            //判断是否文件夹
                    {
                        DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                        subdir.Delete(true);          //删除子目录和文件
                    }
                    else
                    {
                        System.IO.File.Delete(i.FullName);      //删除指定文件
                    }
                }
                dir.Delete(true);
            }
        }

        public ActionResult UnFinishOrder()
        {
            return View();
        }

        #region 加载未完成订单报表
        /// <summary>
        /// 加载未完成订单报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUDID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [HttpPost]
        [OperationAuditFilter(Operation = "加载未完成订单报表", OperationAuditTypeName = "加载未完成订单报表")]
        public JsonResult LoadUnFinishOrder(string CN, string MUDID, string StartDate, string EndDate, int page, int rows)
        {
            int total = 0;
            var list = uploadOrderService.LoadUnFinishOrder(CN, MUDID, StartDate, EndDate, page, rows, out total)
                .Select(a => new
                {
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = a.c14 == null ? string.Empty : a.c14.Value.ToString("yyyy/MM/dd"),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = a.c40 == null ? string.Empty : a.c40.Value.ToString("yyyy/MM/dd"),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = a.c44 == null ? string.Empty : a.c44.Value.ToString("yyyy/MM/dd"),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = a.c54 == null ? string.Empty : a.c54.Value.ToString("yyyy/MM/dd"),
                    c55 = FormatterNull(a.c55),
                    c56 = FormatterNull(a.c56),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                    c61 = FormatterNull(a.c61),
                    c62 = FormatterNull(a.c62),
                    c63 = FormatterNull(a.c63),
                    c64 = FormatterNull(a.c64),
                    c65 = FormatterNull(a.c65),
                    c66 = a.c66 == null ? string.Empty : a.c66.Value.ToString("yyyy/MM/dd"),
                    c67 = FormatterNull(a.c67),
                    c68 = FormatterNull(a.c68),
                    c69 = FormatterNull(a.c69),
                    c70 = FormatterNull(a.c70),
                    c71 = FormatterNull(a.c71),
                    c72 = FormatterNull(a.c72),
                    c73 = a.c73 == null ? string.Empty : a.c73.Value.ToString("yyyy/MM/dd"),
                    c74 = FormatterNull(a.c74),
                    c75 = FormatterNull(a.c75),
                    c76 = FormatterNull(a.c76),
                }).ToArray();
            var num = list.Count();
            return Json(new { state = 1, rows = list, total = total });
        }
        #endregion

        #region 发送未完成订单提醒消息
        /// <summary>
        /// 加载未完成订单报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [OperationAuditFilter(Operation = "加载未完成订单报表", OperationAuditTypeName = "加载未完成订单报表")]
        public JsonResult SendUnFinishOrder(string CN, string MUDID, string StartDate, string EndDate)
        {
            var list = uploadOrderService.LoadUnFinishOrderForMessage(CN, MUDID, StartDate, EndDate)
                .Select(a => new
                {
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = a.c14 == null ? string.Empty : a.c14.Value.ToString("yyyy/MM/dd"),
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = a.c40 == null ? string.Empty : a.c40.Value.ToString("yyyy/MM/dd"),
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = a.c44 == null ? string.Empty : a.c44.Value.ToString("yyyy/MM/dd"),
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = a.c54 == null ? string.Empty : a.c54.Value.ToString("yyyy/MM/dd"),
                    c55 = FormatterNull(a.c55),
                    c56 = FormatterNull(a.c56),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                    c61 = FormatterNull(a.c61),
                    c62 = FormatterNull(a.c62),
                    c63 = FormatterNull(a.c63),
                    c64 = FormatterNull(a.c64),
                    c65 = FormatterNull(a.c65),
                    c66 = a.c66 == null ? string.Empty : a.c66.Value.ToString("yyyy/MM/dd"),
                    c67 = FormatterNull(a.c67),
                    c68 = FormatterNull(a.c68),
                    c69 = FormatterNull(a.c69),
                    c70 = FormatterNull(a.c70),
                    c71 = FormatterNull(a.c71),
                    c72 = FormatterNull(a.c72),
                    c73 = a.c73 == null ? string.Empty : a.c73.Value.ToString("yyyy/MM/dd"),
                    c74 = FormatterNull(a.c74),
                    c75 = FormatterNull(a.c75),
                    c76 = FormatterNull(a.c76),
                }).ToArray();
            if (list.ToList().Count > 0)
            {
                //向代表发送消息
                string message = string.Empty;
                var userList = list.Where(x => x.c61 == "否").Select(p => p.c2).ToList().Distinct();
                foreach (var user in userList)
                {
                    var count = list.Where(p => p.c2 == user).Count();
                    var warningMsg = $"您有{count}个订单已超过1个月尚未完成上传文件审批通过。上传会议文件流程是HT订餐不可或缺的流程，也是GSK合规要求必须完成的步骤。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/Upload/UploadOrder'>点击这里</a>，选择相应订单并进行上传文件操作。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(user, warningMsg);
                }
                //向转单代表发送消息
                var userListTr = list.Where(x => x.c61 == "是").Select(p => p.c65).ToList().Distinct();
                foreach (var user in userListTr)
                {
                    var count = list.Where(p => p.c65 == user).Count();
                    var warningMsg = $"您有{count}个被重新分配订单已超过1个月尚未完成上传文件审批通过。上传会议文件流程是HT订餐不可或缺的流程，也是GSK合规要求必须完成的步骤。请<a href='{ConfigurationManager.AppSettings["MealH5SiteUrl"]}/P/Upload/UploadOrder'>点击这里</a>，选择相应订单并进行上传文件操作。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(user, warningMsg);
                }
                //向DM发送消息
                var dmList = list.Where(x => x.c68 == "否" || x.c68 == "").Select(p => p.c43).Distinct().ToList();
                foreach (var user in dmList)
                {
                    message = string.Empty;
                    var reportUserList = list.Select(p => new { name = p.c1, user = p.c2, dm = p.c43 }).Where(p => p.dm == user).ToList().Distinct();
                    foreach (var repUser in reportUserList)
                    {
                        var userOrderCount = list.Where(p => p.c2 == repUser.user).Count();
                        message += $"姓名：{repUser.name}，MUDID：{repUser.user}，有{userOrderCount}个订单；";
                    }
                    message = message.Length > 1 ? message.Substring(0, message.Length - 1) : message;
                    var warningMsg = $"你的下属尚有超过订餐日期一个月，且未完成上传文件审批通过的订单：{message}。上传会议文件流程是HT订餐不可或缺的流程，也是GSK合规要求必须完成的步骤。请提醒代表尽快完成。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(user, warningMsg);
                }
                //向离职代表的DM发送消息
                var dmListTr = list.Where(x => x.c68 == "是").Select(p => p.c72).Distinct().ToList();
                foreach (var user in dmListTr)
                {
                    var messageTr = string.Empty;
                    var reportUserList = list.Where(x => x.c61 == "否").Select(p => new { name = p.c1, user = p.c2, dm = p.c72 }).Where(p => p.dm == user).ToList().Distinct();
                    foreach (var repUser in reportUserList)
                    {
                        var userOrderCount = list.Where(p => p.c2 == repUser.user).Count();
                        messageTr += $"姓名：{repUser.name}，MUDID：{repUser.user}，有{userOrderCount}个订单；";
                    }

                    var reportUserListTr = list.Where(x => x.c61 == "是").Select(p => new { name = p.c64, user = p.c65, dm = p.c72 }).Where(p => p.dm == user).ToList().Distinct();
                    foreach (var repUser in reportUserListTr)
                    {
                        var userOrderCount = list.Where(p => p.c65 == repUser.user).Count();
                        messageTr += $"姓名：{repUser.name}，MUDID：{repUser.user}，有{userOrderCount}个订单；";
                    }

                    messageTr = messageTr.Length > 1 ? messageTr.Substring(0, messageTr.Length - 1) : messageTr;
                    var warningMsg = $"你的下属尚有超过订餐日期一个月，且未完成上传文件审批通过的订单：{messageTr}。上传会议文件流程是HT订餐不可或缺的流程，也是GSK合规要求必须完成的步骤。请提醒代表尽快完成。如有疑问，请联系：cn.chinarx-pmo@gsk.com。";
                    var rtnVal = WxMessageClientChannelFactory.GetChannel().SendText(user, warningMsg);
                }
            }
            return Json(new { state = 1 });
        }
        #endregion

        # region 导出未完成订单报表
        /// <summary>
        /// 导出未完成订单报表
        /// </summary>
        /// <param name="CN"></param>
        /// <param name="MUID"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [OperationAuditFilter(Operation = "导出未完成订单报表", OperationAuditTypeName = "导出未完成订单报表")]
        public void ExportUnFinishOrder(string CN, string MUDID, string StartDate, string EndDate)
        {
            var list = uploadOrderService.LoadUnFinishOrderForMessage(CN, MUDID, StartDate, EndDate)
                .Select(a => new
                {
                    c1 = FormatterNull(a.c1),
                    c2 = FormatterNull(a.c2),
                    c3 = FormatterNull(a.c3),
                    c4 = FormatterNull(a.c4),
                    c5 = FormatterNull(a.c5),
                    c6 = FormatterNull(a.c6),
                    c7 = FormatterNull(a.c7),
                    c8 = FormatterNull(a.c8),
                    c9 = FormatterNull(a.c9),
                    c10 = FormatterNull(a.c10),
                    c11 = FormatterNull(a.c11),
                    c12 = FormatterNull(a.c12),
                    c13 = FormatterNull(a.c13),
                    c14 = a.c14,
                    c15 = FormatterNull(a.c15),
                    c16 = FormatterNull(a.c16),
                    c17 = FormatterNull(a.c17),
                    c18 = FormatterNull(a.c18),
                    c19 = FormatterNull(a.c19),
                    c20 = FormatterNull(a.c20),
                    c21 = FormatterNull(a.c21),
                    c22 = FormatterNull(a.c22),
                    c23 = FormatterNull(a.c23),
                    c24 = FormatterNull(a.c24),
                    c25 = FormatterNull(a.c25),
                    c26 = FormatterNull(a.c26),
                    c27 = FormatterNull(a.c27),
                    c28 = FormatterNull(a.c28),
                    c29 = FormatterNull(a.c29),
                    c30 = FormatterNull(a.c30),
                    c31 = FormatterNull(a.c31),
                    c32 = FormatterNull(a.c32),
                    c33 = FormatterNull(a.c33),
                    c34 = FormatterNull(a.c34),
                    c35 = FormatterNull(a.c35),
                    c36 = FormatterNull(a.c36),
                    c37 = FormatterNull(a.c37),
                    c38 = FormatterNull(a.c38),
                    c39 = FormatterNull(a.c39),
                    c40 = a.c40,
                    c41 = FormatterNull(a.c41),
                    c42 = FormatterNull(a.c42),
                    c43 = FormatterNull(a.c43),
                    c44 = a.c44,
                    c45 = FormatterNull(a.c45),
                    c46 = FormatterNull(a.c46),
                    c47 = FormatterNull(a.c47),
                    c48 = FormatterNull(a.c48),
                    c49 = FormatterNull(a.c49),
                    c50 = FormatterNull(a.c50),
                    c51 = FormatterNull(a.c51),
                    c52 = FormatterNull(a.c52),
                    c53 = FormatterNull(a.c53),
                    c54 = a.c54,
                    c55 = FormatterNull(a.c55),
                    c56 = FormatterNull(a.c56),
                    c57 = FormatterNull(a.c57),
                    c58 = FormatterNull(a.c58),
                    c59 = FormatterNull(a.c59),
                    c60 = FormatterNull(a.c60),
                    c61 = FormatterNull(a.c61),
                    c62 = FormatterNull(a.c62),
                    c63 = FormatterNull(a.c63),
                    c64 = FormatterNull(a.c64),
                    c65 = FormatterNull(a.c65),
                    c66 = a.c66,
                    c67 = FormatterNull(a.c67),
                    c68 = FormatterNull(a.c68),
                    c69 = FormatterNull(a.c69),
                    c70 = FormatterNull(a.c70),
                    c71 = FormatterNull(a.c71),
                    c72 = FormatterNull(a.c72),
                    c73 = a.c73,
                    c74 = FormatterNull(a.c74),
                    c75 = FormatterNull(a.c75),
                    c76 = FormatterNull(a.c76),
                }).ToList();
            if (list.Count > 0)
            {
                FileStream file11 = new FileStream(Server.MapPath("~/Template/Template_UnFinishOrderReports.xls"), FileMode.Open, FileAccess.Read);
                HSSFWorkbook wk = new HSSFWorkbook(file11);

                ISheet sheet = wk.GetSheet("Report");
                int dataCnt = list.Count;
                for (int i = 1; i <= dataCnt; i++)
                {
                    IRow row = sheet.CreateRow(i);
                    ICell cell = null;

                    row = sheet.CreateRow(i);
                    #region data cell
                    var a = 0;
                    cell = row.CreateCell(a);                 //申请人姓名
                    cell.SetCellValue(list[i - 1].c1);
                    cell = row.CreateCell(++a);            //申请人MUDID
                    cell.SetCellValue(list[i - 1].c2);
                    cell = row.CreateCell(++a);            //申请人职位
                    cell.SetCellValue(list[i - 1].c3);
                    cell = row.CreateCell(++a);            //申请人手机号码
                    cell.SetCellValue("");
                    cell = row.CreateCell(++a);            //HT编号
                    cell.SetCellValue(list[i - 1].c5);
                    cell = row.CreateCell(++a);            //Market
                    cell.SetCellValue(list[i - 1].c6);
                    cell = row.CreateCell(++a);            //TA
                    cell.SetCellValue(list[i - 1].c7);
                    cell = row.CreateCell(++a);            //省份
                    cell.SetCellValue(list[i - 1].c8);
                    cell = row.CreateCell(++a);            //城市
                    cell.SetCellValue(list[i - 1].c9);
                    cell = row.CreateCell(++a);            //医院编码
                    cell.SetCellValue(list[i - 1].c10);
                    cell = row.CreateCell(++a);             //医院名称
                    cell.SetCellValue(list[i - 1].c11);
                    cell = row.CreateCell(++a);             //成本中心
                    cell.SetCellValue(list[i - 1].c12);
                    cell = row.CreateCell(++a);             //供应商
                    cell.SetCellValue(list[i - 1].c13);
                    cell = row.CreateCell(++a);             //送餐日期
                    cell.SetCellValue(list[i - 1].c14.Value);
                    cell = row.CreateCell(++a);             //送餐时间
                    cell.SetCellValue(list[i - 1].c15);
                    cell = row.CreateCell(++a);             //餐厅编码
                    cell.SetCellValue(list[i - 1].c16);
                    cell = row.CreateCell(++a);             //餐厅名称
                    cell.SetCellValue(list[i - 1].c17);
                    cell = row.CreateCell(++a);             //用餐人数
                    cell.SetCellValue(list[i - 1].c18);
                    cell = row.CreateCell(++a);             //实际金额
                    double budgetTotal;
                    double.TryParse(list[i - 1].c19, out budgetTotal);
                    cell.SetCellValue(budgetTotal);// "预算金额");
                    cell = row.CreateCell(++a);             //是否申请退单
                    cell.SetCellValue(list[i - 1].c20);
                    cell = row.CreateCell(++a);             //是否退单成功
                    cell.SetCellValue(list[i - 1].c21);
                    cell = row.CreateCell(++a);             //退单失败平台发起配送需求
                    cell.SetCellValue(list[i - 1].c22);
                    cell = row.CreateCell(++a);             //退单失败反馈配送需求
                    cell.SetCellValue(list[i - 1].c23);
                    cell = row.CreateCell(++a);             //预定/退单失败原因
                    cell.SetCellValue(list[i - 1].c24);
                    cell = row.CreateCell(++a);             //是否收餐/未送达
                    cell.SetCellValue(list[i - 1].c25);
                    cell = row.CreateCell(++a);             //用户确认金额
                    double actualTotal;
                    double.TryParse(list[i - 1].c26, out actualTotal);
                    cell.SetCellValue(actualTotal);
                    cell = row.CreateCell(++a);             //是否与预定餐品一致
                    cell.SetCellValue(list[i - 1].c27);
                    cell = row.CreateCell(++a);             //用户确认金额调整原因
                    cell.SetCellValue(list[i - 1].c28);
                    cell = row.CreateCell(++a);             //用户确认金额调整备注
                    cell.SetCellValue(list[i - 1].c29);
                    cell = row.CreateCell(++a);             //实际用餐人数
                    cell.SetCellValue(list[i - 1].c30);
                    cell = row.CreateCell(++a);             //实际用餐人数调整原因
                    cell.SetCellValue(list[i - 1].c31);
                    cell = row.CreateCell(++a);             //实际用餐人数调整备注
                    cell.SetCellValue(list[i - 1].c32);
                    cell = row.CreateCell(++a);             //直线经理姓名
                    cell.SetCellValue(list[i - 1].c33);
                    cell = row.CreateCell(++a);             //直线经理MUDID
                    cell.SetCellValue(list[i - 1].c34);
                    cell = row.CreateCell(++a);             //Level2 Manager 姓名
                    cell.SetCellValue(list[i - 1].c35);
                    cell = row.CreateCell(++a);             //Level2 Manager MUDID
                    cell.SetCellValue(list[i - 1].c36);
                    cell = row.CreateCell(++a);             //Level3 Manager 姓名
                    cell.SetCellValue(list[i - 1].c37);
                    cell = row.CreateCell(++a);             //Level3 Manager MUDID
                    cell.SetCellValue(list[i - 1].c38);
                    cell = row.CreateCell(++a);             //是否上传文件
                    cell.SetCellValue(list[i - 1].c39);
                    cell = row.CreateCell(++a);             //上传文件提交日期
                    if (list[i - 1].c40 != null)
                    {
                        cell.SetCellValue(list[i - 1].c40.Value);
                    }
                    cell = row.CreateCell(++a);             //上传文件提交时间
                    cell.SetCellValue(list[i - 1].c41);
                    cell = row.CreateCell(++a);             //上传文件审批直线经理姓名
                    cell.SetCellValue(list[i - 1].c42);
                    cell = row.CreateCell(++a);             //上传文件审批直线经理MUDID
                    cell.SetCellValue(list[i - 1].c43);
                    cell = row.CreateCell(++a);             //上传文件审批日期
                    if (list[i - 1].c44 != null)
                    {
                        cell.SetCellValue(list[i - 1].c44.Value);
                    }
                    cell = row.CreateCell(++a);             //上传文件审批时间
                    cell.SetCellValue(list[i - 1].c45);
                    cell = row.CreateCell(++a);             //上传文件审批状态
                    cell.SetCellValue(list[i - 1].c46);
                    cell = row.CreateCell(++a);             //签到人数是否等于实际用餐人数
                    cell.SetCellValue(list[i - 1].c47);
                    cell = row.CreateCell(++a);             //签到人数调整原因
                    cell.SetCellValue(list[i - 1].c48);
                    cell = row.CreateCell(++a);             //是否与会议信息一致
                    cell.SetCellValue(list[i - 1].c49);
                    cell = row.CreateCell(++a);             //会议信息不一致原因
                    cell.SetCellValue(list[i - 1].c50);
                    cell = row.CreateCell(++a);             //上传文件是否Re-Open
                    cell.SetCellValue(list[i - 1].c51);
                    cell = row.CreateCell(++a);             //上传文件Re-Open操作人
                    cell.SetCellValue(list[i - 1].c52);
                    cell = row.CreateCell(++a);             //上传文件Re-Open操作人MUDID
                    cell.SetCellValue(list[i - 1].c53);
                    cell = row.CreateCell(++a);             //上传文件Re-Open日期
                    if (list[i - 1].c54 != null)
                    {
                        cell.SetCellValue(list[i - 1].c54.Value);
                    }
                    cell = row.CreateCell(++a);             //上传文件Re-Open时间
                    cell.SetCellValue(list[i - 1].c55);
                    cell = row.CreateCell(++a);             //上传文件Re-Open发起人姓名
                    cell.SetCellValue(list[i - 1].c56);
                    cell = row.CreateCell(++a);             //上传文件Re-Open发起人MUDID
                    cell.SetCellValue(list[i - 1].c57);
                    cell = row.CreateCell(++a);             //上传文件Re-Open原因
                    cell.SetCellValue(list[i - 1].c58);
                    cell = row.CreateCell(++a);             //上传文件Re-Open备注 
                    cell.SetCellValue(list[i - 1].c59);
                    cell = row.CreateCell(++a);             //上传文件Re-Open审批状态
                    cell.SetCellValue(list[i - 1].c60);
                    cell = row.CreateCell(++a);             //上传文件是否重新分配
                    cell.SetCellValue(list[i - 1].c61);
                    cell = row.CreateCell(++a);             //上传文件重新分配操作人
                    cell.SetCellValue(list[i - 1].c62);
                    cell = row.CreateCell(++a);             //上传文件重新分配操作人MUDID
                    cell.SetCellValue(list[i - 1].c63);
                    cell = row.CreateCell(++a);             //上传文件被重新分配人姓名
                    cell.SetCellValue(list[i - 1].c64);
                    cell = row.CreateCell(++a);             //上传文件被重新分配人MUDID
                    cell.SetCellValue(list[i - 1].c65);
                    cell = row.CreateCell(++a);             //上传文件被重新分配日期
                    if (list[i - 1].c66 != null)
                    {
                        cell.SetCellValue(list[i - 1].c66.Value);
                    }
                    cell = row.CreateCell(++a);             //上传文件被重新分配时间
                    cell.SetCellValue(list[i - 1].c67);
                    cell = row.CreateCell(++a);             //上传文件否重新分配审批人
                    cell.SetCellValue(list[i - 1].c68);
                    cell = row.CreateCell(++a);             //上传文件重新分配审批人-操作人
                    cell.SetCellValue(list[i - 1].c69);
                    cell = row.CreateCell(++a);             //上传文件重新分配审批人-操作人MUDID
                    cell.SetCellValue(list[i - 1].c70);
                    cell = row.CreateCell(++a);             //上传文件被重新分配审批人姓名
                    cell.SetCellValue(list[i - 1].c71);
                    cell = row.CreateCell(++a);             //上传文件被重新分配审批人MUDID
                    cell.SetCellValue(list[i - 1].c72);
                    cell = row.CreateCell(++a);             //上传文件重新分配审批人日期
                    if (list[i - 1].c73 != null)
                    {
                        cell.SetCellValue(list[i - 1].c73.Value);
                    }
                    cell = row.CreateCell(++a);             //上传文件重新分配审批人时间
                    cell.SetCellValue(list[i - 1].c74);
                    cell = row.CreateCell(++a);             //项目组特殊备注
                    cell.SetCellValue(list[i - 1].c75);
                    cell = row.CreateCell(++a);             //WorkDay是否离职
                    cell.SetCellValue(list[i - 1].c76);
                    #endregion
                }
                using (System.IO.MemoryStream _ms = new System.IO.MemoryStream())
                {
                    wk.Write(_ms);
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.BinaryWrite(_ms.ToArray());
                }
            }
        }
        #endregion
        //public class NoHTCode
        //{         
        //    public string HTCode { get; set; }
        //    public string ApplierMUDID { get; set; }

        //}
    }
}