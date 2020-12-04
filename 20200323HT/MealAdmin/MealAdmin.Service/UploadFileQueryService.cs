using MealAdmin.Dao;
using MealAdmin.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFramework.XInject.Attributes;

namespace MealAdmin.Service
{
    public class UploadFileQueryService : IUploadFileQueryService
    {
        [Bean("uploadFileQueryDao")]
        public IUploadFileQueryDao uploadFileQueryDao { get; set; }

        public List<P_UploadFileQuery_TXT> LoadPage(string HTCode, string ApplierMUDID, string Begin, string End, string State,int rows, int page, out int total)
        {
            return uploadFileQueryDao.LoadPage(HTCode, ApplierMUDID, Begin, End, State, rows, page, out total);
        }
        public List<P_UploadFileQuery_TXT> UploadFileLoad(string HTCode, string ApplierMUDID, string Begin, string End, string State)
        {
            return uploadFileQueryDao.UploadFileLoad(HTCode, ApplierMUDID, Begin, End, State);
        }

        public List<P_UploadFileQuery_TXT> ExportUploadFile(string HTCode, string ApplierMUDID, string Begin, string End, string State)
        {
            return uploadFileQueryDao.ExportUploadFile(HTCode, ApplierMUDID, Begin, End, State);
        }

        public List<P_UploadFileQuery_TXT> RecordsLoad(string HTCode, string ApplierMUDID, string Begin, string End, string State, int rows, int page, out int total)
        {
            return uploadFileQueryDao.RecordsLoad(HTCode, ApplierMUDID, Begin, End, State, rows, page, out total);
        }

        public P_PREUPLOADORDER FindPreUploadFile(string id)
        {
            return uploadFileQueryDao.FindPreUploadFile(id);
        }
        public P_PREUPLOADORDER FindPreUploadFileByHTCode(string HTCode)
        {
            return uploadFileQueryDao.FindPreUploadFileByHTCode(HTCode);
        }

        public List<P_OrderApproveHistory> GetApproval(string id)
        {
            return uploadFileQueryDao.GetApproval(id);
        }

        public List<P_UploadFileQuery_TXT> GetUpdateFileByID(string UpdateFilelID)
        {
            return uploadFileQueryDao.GetUpdateFileByID(UpdateFilelID);
        }

        #region 定时导出上传文件（图片）
        public List<P_UploadFileQuery> TimingExpotr()
        {
            return uploadFileQueryDao.TimingExpotr();
        }

        #endregion

    }
}
