using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace HubspotSample
{
class Program
{
    public static readonly string VALIDATION_URL = "https://localhost:5001/services/API/activate?u=cmacfarl&vToken=de6c7976-1512-4325-a233-5cc1c4f9116b";
    
    static void Main(string[] args)
    {
        string subject;
        string body;
        
        IConfigurationBuilder builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");
    
        IConfigurationRoot config = builder.Build();
        DestinationConfiguration destConfig = config.GetSection("EmailDestination").Get<DestinationConfiguration>();
        MailConfiguration mailConfig = config.GetSection("HubspotMailConfiguration").Get<MailConfiguration>();
        HubspotEmailService emailService = new HubspotEmailService(mailConfig);
        
        body = HubspotEmailService.makeEmailText(out subject, destConfig.FullName, destConfig.Username, VALIDATION_URL); 
        emailService.sendEmail(destConfig.EmailAddress, subject + " uses AlternateView", body, true);
        emailService.sendEmail(destConfig.EmailAddress, subject + " uses Body", body, false);
    }
}
}