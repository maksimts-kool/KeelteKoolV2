using KeelteKoolV2.Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeelteKoolV2.Core.ServiceInterface
{
    public interface IEmailingServices
    {
        void SendEmail(EmailDTO dto);
        void SendEmailToken(EmailTokenDTO dto, string token);
    }
}
