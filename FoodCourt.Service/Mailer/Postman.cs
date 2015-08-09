using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using FoodCourt.Service.Mailer;
using RazorEngine;
using RazorEngine.Templating;
using SendGrid;

namespace FoodCourt.Service
{
    public class Postman
    {
        private string _apiKey;
        private string _sender;

        const string SendgridApiKey = "SendGridApiKey";
        const string SendgridSender = "SendGridSender";

        private Web _transport;

        protected Web Transport
        {
            get
            {
                if (_transport == null)
                {
                    _transport = new Web(_apiKey);
                }

                return _transport;
            }
        }

        private string _templatesPath;

        public Postman(string emailTemplatePath)
        {
            _apiKey = ConfigurationManager.AppSettings[SendgridApiKey] as string;
            _sender = ConfigurationManager.AppSettings[SendgridSender] as string;
            _templatesPath = emailTemplatePath;
        }

        public async void Send(string kind, List<string> recipients, List<EmailDTO> emailDtos)
        {
            if (recipients.Count() != emailDtos.Count())
            {
                throw new InvalidOperationException("Lists of recipients and DTOs are not equal.");
            }

            string subject = RetrieveEmailSubject(kind);
            List<string> messages = await ParseEmailTemplates(kind, emailDtos);

            var recipientsCnt = recipients.Count();
            for (int i = 0; i < recipientsCnt; i++)
            {
                SendSingleMessage(
                        recipients.ElementAt(i),
                        subject,
                        messages.ElementAt(i)
                    );
            }
        }

        public async void SendSingleMessage(string recipient, string subject, string messageBody)
        {
            SendGridMessage message = new SendGridMessage();
            message.From = new MailAddress(_sender);
            message.AddTo(recipient);

            message.Subject = subject;
            message.Html = messageBody;

            await Transport.DeliverAsync(message);
        }

        private string RetrieveEmailSubject(string kind)
        {
            return ConfigurationManager.AppSettings["EmailTemplate" + kind] as string;
        }

        private async Task<List<string>> ParseEmailTemplates(string kind, List<EmailDTO> emailDtos)
        {
            string template = await RetrieveEmailTemplate(kind);
            List<string> parsedTemplates = new List<string>();

            foreach (EmailDTO emailDto in emailDtos)
            {
                string parsedTemplate = Engine.Razor.RunCompile(template, "kind", typeof(EmailDTO), emailDto);
                parsedTemplates.Add(parsedTemplate);
            }

            return parsedTemplates;
        }

        private async Task<string> RetrieveEmailTemplate(string kind)
        {
            var templateStringBuilder = new StringBuilder();
            var fullTemplatePath = _templatesPath + "/" + kind + ".cshtml";

            using (StreamReader reader = new StreamReader(fullTemplatePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!line.StartsWith("@model"))
                    {
                        templateStringBuilder.AppendLine(line);
                    }
                }
            }

            return templateStringBuilder.ToString();
        }
    }
}
