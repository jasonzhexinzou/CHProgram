using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMailApp.Helper
{
    public class S3Upload
    {
        private static string awsAccessKey = "AKIAPJZX4QFWMMMA2RJA";
        private static string awsSecretKey = "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";
        private static string bucketName = "wechat"; //文件夹
        private static string S3Path = "https://s3.cn-north-1.amazonaws.com.cn/";
        AmazonS3Config config = new AmazonS3Config()
        {
            ServiceURL = "http://s3.amazonaws.com",
            RegionEndpoint = Amazon.RegionEndpoint.CNNorth1
        };

        public string localFileS3Upload(string filePath)
        {
            try
            {
                var fileName = Guid.NewGuid().ToString();
                string name = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                name = name.Substring(0, name.LastIndexOf('.'));
                string k = filePath.Substring(filePath.LastIndexOf('.'));

                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, config))
                {
                    var key = string.Format("Report/{0}" + k, name);
                    var requestS3 = new PutObjectRequest()
                    {
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead,
                        Key = key,
                        InputStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)
                    };
                    client.PutObject(requestS3);
                    client.Dispose();
                }

                string FullPath = S3Path + bucketName + "/Report/" + name + k;
                return FullPath;
            }
            catch
            {
                return null;
            }

        }
    }
}
