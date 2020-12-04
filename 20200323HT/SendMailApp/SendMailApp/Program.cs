using SendMailApp.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SendMailApp
{
    class Program
    {
        static void Main(string[] args)
        {
            S3Upload s3BLL = new S3Upload();
            SendMail sendBLL = new SendMail();

            string TeplatePath = System.Configuration.ConfigurationManager.AppSettings["MailTemplatePath"].ToString() + "MailContent.html";
            string filePath = System.Configuration.ConfigurationManager.AppSettings["FilePath"].ToString();
            //返回S3地址链接
            string S3Path = s3BLL.localFileS3Upload(filePath);


            byte[] buffer = File.ReadAllBytes(filePath);

            string mailSubject = "Hospital Data Synchronization Task Succeed";

            //string mailBody = "EXTERNAL<br/><br/><br/><br/>" +
            //                  "Hi Everyone,<br/><br/>" +
            //                  "Today hospital data synchronization task had been completed , please click link and download the attachment.<br/><br/>" +
            //                  "Download link:" + S3Path +
            //                  "<br/><br/>&nbsp;&nbsp;&nbsp;&nbsp;" +
            //                  "Best Regards<br/><br/>" +
            //                  "This is automatically sent by system. Please do not reply.<br/><br/>" +
            //                  "GSK monitors email communications sent to and from GSK in order to protect GSK, our employees, customers, suppliers and business partners, from cyber threats and loss of GSK Information. <br/>;" +
            //                  "GSK monitoring is conducted with appropriate confidentiality controls and in accordance with local laws and after appropriate consultation.";

            string mailBody = sendBLL.ReplaceText(TeplatePath, S3Path);

            //正式收件人
            string mailTo = System.Configuration.ConfigurationManager.AppSettings["MailToUser"].ToString();

            if (buffer.Length > 5242880)
            {
                sendBLL.AmazonSES(mailSubject, mailTo, mailBody, "");
            }else
            {
                sendBLL.AmazonSES(mailSubject, mailTo, mailBody, filePath);
            }
        }
    }
}
