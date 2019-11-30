using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Options;
using TicTacToe.Services;

namespace TicTacToe.Extensions
{
    public static class EmailServiceExtension
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            services.Configure<EmailServiceOptions>(configuration.GetSection("Email"));
            if (hostingEnvironment.IsDevelopment() || hostingEnvironment.IsStaging())
            {
                services.AddSingleton<IEmailService, EmailService>();
            }
            else
            {
                //services.AddSingleton<IEmailService, SendGridEmailService>();
            }
            return services;
        }
    }
}
