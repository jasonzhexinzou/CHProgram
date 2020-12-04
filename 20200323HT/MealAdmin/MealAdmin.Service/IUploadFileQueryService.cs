using System;
using MealAdmin.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealAdmin.Service
{
    public interface IUploadFileQueryService
    {
        List<P_UploadFileQuery_TXT> LoadPage(string HTCode, string ApplierMUDID, string Begin, string End, string State,int rows, int page, out int total);
        List<P_UploadFileQuery_TXT> UploadFileLoad(string HTCode, string ApplierMUDID, string Begin, string End, string State);
        List<P_UploadFileQuery_TXT> ExportUploadFile(string HTCode, string ApplierMUDID, string Begin, string End, string State);
        List<P_UploadFileQuery_TXT> RecordsLoad(string HTCode, string ApplierMUDID, string Begin, string End, string State, int rows, int page, out int total);
        P_PREUPLOADORDER FindPreUploadFile(string id);
        P_PREUPLOADORDER FindPreUploadFileByHTCode(string HTCode);
        List<P_OrderApproveHistory> GetApproval(string id);//获取上传文件审批流程
        List<P_UploadFileQuery_TXT> GetUpdateFileByID(string UpdateFilelID);

        //定时导出上传文件（图片）
        List<P_UploadFileQuery> TimingExpotr();


    }
}
