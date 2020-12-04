using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MealAdmin.Web.Handler
{
    public class S3Handler
    {
        private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
        private static string awsSecretKey = "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";
        private static string bucketName = "wechat";
        AmazonS3Config config = new AmazonS3Config()
        {
            ServiceURL = "http://s3.amazonaws.com",
            RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
        };
        public string S3Upload(HttpPostedFileBase file)
        {
            string filePath = "HR_Uploads/" + file.FileName;
            using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
            {
                var request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead,
                    Key = string.Format(filePath),
                    InputStream = file.InputStream
                };
                client.PutObject(request);
            }
            filePath = "https://s3.cn-north-1.amazonaws.com.cn/imtpath/HR_Uploads/" + file.FileName;
            return filePath;
        }

        #region s3文件下载
        /// <summary>
        /// s3文件下载
        /// </summary>
        /// <param name="key"></param>
        /// <param name="output"></param>
        public void S3Download(string key,string output)
        {
            using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
            {
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = key
                };

                GetObjectResponse response = client.GetObject(request);
                response.WriteResponseStreamToFile(output);
            }
        }
        #endregion

    }
}