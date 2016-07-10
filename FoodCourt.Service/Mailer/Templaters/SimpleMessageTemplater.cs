using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCourt.Service.Mailer.Templaters
{
    public class SimpleMessageTemplater : IMessageTemplater
    {
        public List<string> ParseEmailTemplates(string kind, List<EmailDTO> emailDtos)
        {
            string template = RetrieveEmailTemplate(kind);
            List<string> parsedTemplates = new List<string>();

            foreach (EmailDTO emailDto in emailDtos)
            {
                string parsedTemplate = ParseSingleTemplate(template, emailDto);
                    parsedTemplates.Add(parsedTemplate);
            }

            return parsedTemplates;
        }

        private string ParseSingleTemplate(string template, EmailDTO emailDto)
        {
            var dtoStringProperties = emailDto
                .GetType()
                .GetProperties()
                .Where(p => p.PropertyType == typeof(string));

            foreach (var singleProperty in dtoStringProperties)
            {
                string propertyValue = (string) singleProperty.GetValue(emailDto);
                template = template.Replace("{{" + singleProperty.Name + "}}", propertyValue);
            }

            return template;
        }


        private string RetrieveEmailTemplate(string kind)
        {
            switch (kind)
            {
                case "Invite":
                    return _templateInvite;
                case "OrderNotification":
                    return _templateOrderNotification;
                case "OrderWarning":
                    return _templateOrderWarning;
            }
            throw new ArgumentException("Invalid email template kind: \"" + kind + "\"");
        }

        private string _templateInvite = @"<h1>Hi there</h1>
<p>{{GroupOwner}} invited you to join group {{GroupName}} here on FoodCourt so you can easily order food from now on.</p>
<p>If you are interested (or hugry) please click below link:</p>

<code>{{PasswordSetUrl}}</code>

<p>Beware. Above link is one-use-only. Once used it becames inactive!</p>";

        private string _templateOrderNotification = @"<p>The poll has been closed and resolved. To see it's results, visit the link below:</p><code>{{PollUrl}}</code>";
        private string _templateOrderWarning = @"<p>It seems that we cannot match your orders with others.</p>

<p><a href='{{PollUrl}}'>Revisit the Food Court</a> and expand your wishlist to make it more probable to get matched.</p>

<p>If you don't mind ordering food alone, you can ignore this message and wait for another email, informing that the poll is closed and resolved.</p>";
    }
}
