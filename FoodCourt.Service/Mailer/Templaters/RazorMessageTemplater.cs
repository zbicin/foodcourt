using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace FoodCourt.Service.Mailer.Templaters
{
    public class RazorMessageTemplater : IMessageTemplater
    {
        private readonly string _templatesPath;

        public RazorMessageTemplater(string templatesPath)
        {
            this._templatesPath = templatesPath;
        }

        public List<string> ParseEmailTemplates(string kind, List<EmailDTO> emailDtos)
        {
            string template = RetrieveEmailTemplate(kind);
            List<string> parsedTemplates = new List<string>();
            TemplateServiceConfiguration serviceConfiguration = new TemplateServiceConfiguration()
            {
                CachingProvider = new DefaultCachingProvider(t => { }),
                DisableTempFileLocking = true
            };

            foreach (EmailDTO emailDto in emailDtos)
            {
                using (var service = RazorEngineService.Create(serviceConfiguration))
                {
                    string parsedTemplate = service.RunCompile(template, kind + DateTime.Now.Ticks,
                        emailDto.GetType(), emailDto);
                    parsedTemplates.Add(parsedTemplate);
                }
            }

            return parsedTemplates;
        }

        private string RetrieveEmailTemplate(string kind)
        {
            var templateStringBuilder = new StringBuilder();
            var fullTemplatePath = _templatesPath + "/" + kind + ".cshtml";

            using (StreamReader reader = new StreamReader(fullTemplatePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null && !line.StartsWith("@model"))
                    {
                        templateStringBuilder.AppendLine(line);
                    }
                }
            }

            return templateStringBuilder.ToString();
        }
    }
}
