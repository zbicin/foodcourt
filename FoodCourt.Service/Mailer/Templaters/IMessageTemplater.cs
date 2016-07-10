using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodCourt.Service.Mailer.Templaters
{
    public interface IMessageTemplater
    {
        List<string> ParseEmailTemplates(string kind, List<EmailDTO> emailDtos);
    }
}
