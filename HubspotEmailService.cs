using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace HubspotSample
{
    public class HubspotEmailService
    {
        private string EmailAccountName { get; set; }
        private string EmailAccount { get; set; }
        private string SmtpGateway { get; set; }
        private string SmtpAccount { get; set; }
        private string SmtpPassword { get; set; }
        private bool EmailEnabled { get; set; }

        public HubspotEmailService(MailConfiguration mailConfig)
        {
            SmtpAccount = mailConfig.SmtpAccount;
            SmtpPassword = mailConfig.SmtpPassword;
            SmtpGateway = mailConfig.SmtpGateway;
            
            EmailAccountName = "FIRST Tech Challenge EMS Team";
            EmailAccount = "noreply@ftclive.org";
            EmailEnabled = true;
        }

        private MemoryStream firstLogoFooter
        {
            get
            {
                var webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData("https://frc-cdn.firstinspires.org/eventweb_common/FIRSTLogos/FIRST_Vert_RGB_md.png");
                MemoryStream ms = new MemoryStream(imageBytes);
                return ms;
            }
        }
        public bool sendEmail(string to, string subject, string body, bool useAlternateView)
        {
            try
            {
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(EmailAccount, EmailAccountName), 
                    IsBodyHtml = true
                };

                if (useAlternateView) {
                    AlternateView emailView = AlternateView.CreateAlternateViewFromString(body + footerForEmails(), null, MediaTypeNames.Text.Html);
                    LinkedResource firstLogo = new LinkedResource(firstLogoFooter, MediaTypeNames.Image.Jpeg)
                    {
                        ContentId = "firstLogo"
                    };
                    emailView.LinkedResources.Add(firstLogo);
                    message.AlternateViews.Add(emailView);
                } else {
                    message.Body = body + footerForEmails();
                }

                message.To.Add(to);
                message.Subject = subject;
                SmtpClient sender = senderClient;
                sender.Send(message);
                Console.WriteLine("Email Sent (To: {0}, Subject: {1}) ", to, subject);
                return true;
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Email Not Sent, Failed (To: {0}, Subject: {1}, Exception: {2}) ", to, subject, ex);
                return false;
            }
        }

        public static string makeEmailText(out string subject, string fullName, string username, string validation_url)
        {
            subject = "FTC Events API - Email Verification";
            return @"
Hello " + fullName + @",
<br /><br />
Thank you for your interest in the FTC Events API- please click the link below to activate your account.
<br /><br />
If you did not make this request, or have changed your mind, simply ignore this message. After 72 hours, the request will be automatically 
deleted.
<br /><br />
Your Username: <b>" + username + @"</b><br />
Validation Link: <a href=" + "\"" + validation_url + "\"" + @" target=" + "\"_blank\"" + ">" + validation_url + @"</a>
<br /><br />
Thank you,
<br /><br />
-The EMS Development Team";
        }

        public static string makeEmailThatSucceeds(out string subject, string fullName)
        {
            subject = "FTC Events API Access";
            return " Hello " + fullName + "<br /><br /> Thank you for your interest in the FTC Events API. <br /><br /> -The EMS Development Team";
        }

        private static string footerForEmails()
        {
            return "<br /><br /><img src=\"cid:firstLogo\"><br /><em><b><span style=\"color:red;font-weight:bold;\">F</span>or <span style=\"color:red;font-weight:bold;\">I</span>nspiration and <span style=\"color:red;font-weight:bold;\">R</span>ecognition of <span style=\"color:red;font-weight:bold;\">S</span>cience and <span style=\"color:red;font-weight:bold;\">T</span>echnology</b></em><br />200 Bedford Street&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;Manchester, NH 03101&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;<a href=\"http://www.firstinspires.org\">www.firstinspires.org</a><br /><br /><span style=\"color:gray;font-size:11px;\">-----------------------------------------------<br />&copy; Copyright " + DateTime.Now.Year + " <em>FIRST</em> Robotics Competition. All Rights Reserved. <em>FIRST</em> Robotics Competition is part of the <a href=\"http://www.firstinspires.org/\"><em>FIRST</em> Progression of Programs</a>.<br />This message is systematically generated. Please do not attempt to reply or send e-mail to this account as it is not monitored. The information is intended to be used solely by the recipient(s) named.  Any disclosure, copying or distribution of this message or the taking of any action based on its contents, other than by the intended recipient for its intended purpose, is strictly prohibited.</span>";
        }

        private SmtpClient sendClient = null;
        private SmtpClient senderClient
        {
            get
            {
                if (sendClient == null)
                {
	                sendClient = new SmtpClient(SmtpGateway)
	                {
		                Port = 587,
		                EnableSsl = true,
		                DeliveryMethod = SmtpDeliveryMethod.Network,
		                UseDefaultCredentials = false
	                };
                    NetworkCredential cred = new NetworkCredential(SmtpAccount, SmtpPassword);
                    sendClient.Credentials = cred;
                }
                return sendClient;
            }
        }
    }
}