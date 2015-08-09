using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SendGrid;

namespace FoodCourt.Service
{
    public class Postman
    {
        private string _apiKey;
        private string _sender;

        const string SendgridApiKey = "SendGridApiKey";
        const string SendgridSender = "SendGridSender";

        public Postman()
        {
            _apiKey = ConfigurationManager.AppSettings[SendgridApiKey] as string;
            _sender = ConfigurationManager.AppSettings[SendgridSender] as string;

            var transport = new Web(_apiKey);
        }

        public void Send()
        {
            
        }
    }
}
