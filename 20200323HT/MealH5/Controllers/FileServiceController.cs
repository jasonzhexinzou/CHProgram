using Amazon.S3;
using Amazon.S3.Model;
using MealH5.Areas.P.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XFramework.WeChatAPI.SessionHandlers;
using XFramework.XInject.Attributes;
using XFramework.XUtil;

namespace MealH5.Controllers
{
    /// <summary>
    /// 无状态的文件服务
    /// </summary>
    public class FileServiceController : Controller
    {
        [Bean("qyApiHandler")]
        public QyApiHandler qyApiHandler { get; set; }

        private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
        private static string awsSecretKey = "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";
        private static string bucketName = "wechat";
        AmazonS3Config config = new AmazonS3Config()
        {
            ServiceURL = "http://s3.amazonaws.com",
            RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
        };


        #region 从腾讯微信服务器下载微信手机端上传的图片
        /// <summary>
        /// 从腾讯微信服务器下载微信手机端上传的图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WxUserFilter]
        [HttpPost]
        public JsonResult Save(string id)
        {
            var ids = id.Split(',');
            var server_file_names = new List<string>();
            foreach (var mediaid in ids)
            {
                var uplpad_image = Server.MapPath("~/upload/image");
                if (!System.IO.Directory.Exists(uplpad_image))
                {
                    System.IO.Directory.CreateDirectory(uplpad_image);
                }
                var fileName = Guid.NewGuid().ToString();
                LogHelper.Info("mediaid:"+ mediaid + "     fileName:" + fileName);
                var res = qyApiHandler.Media_Get(mediaid, uplpad_image + $@"\{fileName}.jpg");
                LogHelper.Info("resFileName:" + res.filename);
                if (string.IsNullOrEmpty(res.filename) || res.errcode== "40014")
                {
                    qyApiHandler.RefreshToken();

                    qyApiHandler.Media_Get(mediaid, uplpad_image + $@"\{fileName}.jpg");

                }
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    var key = string.Format("UPLOADS/{0}.jpg", fileName);
                    var request = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead,
                        Key = key,
                        InputStream = new FileStream(uplpad_image + $@"\{fileName}.jpg", FileMode.Open, FileAccess.Read)
                    };
                    client.PutObject(request);
                }
                server_file_names.Add($@"/UPLOADS/{fileName}.jpg");
              
            }

            return Json(new { state = 1, data = server_file_names });
        }
        #endregion

        public ContentResult Test()
        {

            var mediaid = "r_ErfjW6y04SeuUobeya5LSc37efaOA1BxvC6RiVLBaVkWx1PTovK-OIQ1A7FDGf";
            var fileName = Guid.Parse("c24c59e3-e1fc-479a-babd-52de32d4dc4a");

            var uplpad_image = Server.MapPath("~/upload/image");
            if (!System.IO.Directory.Exists(uplpad_image))
            {
                System.IO.Directory.CreateDirectory(uplpad_image);
            }
            var res = qyApiHandler.Media_Get(mediaid, uplpad_image + $@"\{fileName}.jpg");
            if (string.IsNullOrEmpty(res.filename))
            {
                qyApiHandler.RefreshToken();

                var r=qyApiHandler.Media_Get(mediaid, uplpad_image + $@"\{fileName}.jpg");

            }
            using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
            {
                var key = string.Format("UPLOADS/{0}.jpg", fileName);
                var request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = key,
                    InputStream = new FileStream(uplpad_image + $@"\{fileName}.jpg", FileMode.Open, FileAccess.Read)
                };
                client.PutObject(request);
            }


            return Content("success!");
        }

    }
}