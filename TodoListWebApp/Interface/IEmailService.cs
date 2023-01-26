using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListWebApp.Models;

namespace TodoListWebApp.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageContent message);
    }
}
