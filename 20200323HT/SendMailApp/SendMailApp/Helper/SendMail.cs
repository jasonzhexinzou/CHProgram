using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SendMailApp.Helper
{
    public class SendMail
    {
        public bool AmazonSES(string Subject, string mailTo, string Body, string FilePath)
        {
            String FROM = "cn_dms_prod@gsk.com";
            String FROMNAME = "cn_dms_prod@gsk.com";
            String SMTP_USERNAME = "AKIAIHZIQK74DCRBRYMQ";
            String SMTP_PASSWORD = "Av567RSLSBZUtaDNe/oCeDdDGP/AaYpkSeAt7NtHPFGe";
            String HOST = "email-smtp.us-east-1.amazonaws.com";
            int PORT = 25;


            //String FROM = "cn.igsk@gsk.com";
            //String FROMNAME = "Catering Service";
            //String SMTP_USERNAME = "AKIAIHZIQK74DCRBRYMQ";
            //String SMTP_PASSWORD = "Av567RSLSBZUtaDNe/oCeDdDGP/AaYpkSeAt7NtHPFGe";
            //String HOST = "email-smtp.us-east-1.amazonaws.com";
            //int PORT = 587;
            String SUBJECT = Subject;
            String BODY = Body;

            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            //收件人
            string[] mailTos = mailTo.Split(';');
            foreach (string To in mailTos)
            {
                if (!string.IsNullOrEmpty(To))
                    message.To.Add(new MailAddress(To));
            }
            message.Subject = SUBJECT;
            message.Body = BODY;

            if (FilePath != "" && FilePath != null)
            {
                message.Attachments.Add(new Attachment(FilePath));
                //message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            }

            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                // Enable SSL encryption
                client.EnableSsl = true;

                try
                {
                    client.Send(message);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public string ReplaceText(string path, string S3Paht)
        {

            //path = HttpContext.Current.Server.MapPath("EmailTemplate\\emailTemplate.html");  

            //if (path == string.Empty)  
            //{  
            //    return string.Empty;  
            //}  
            System.IO.StreamReader sr = new System.IO.StreamReader(path);
            string str = string.Empty;
            str = sr.ReadToEnd();
            str = str.Replace("$FileLink$", S3Paht);

            return str;
        }
    }
}
