using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Helpers;

namespace TicTacToe.Services
{
    public class EmailTemplateRenderService : IEmailTemplateRenderService
    {
        private IWebHostEnvironment _hostingEnvironment;
        private IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        public EmailTemplateRenderService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> RenderTemplate<T>(string templateName, T model, string host) where T : class
        {
            var html = await new EmailViewRenderHelper().RenderTemplate(templateName, _hostingEnvironment,
            _configuration, _httpContextAccessor, model);
            var targetDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Emails");
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            string dateTime = DateTime.Now.ToString("ddMMHHyyHHmmss");
            var targetFileName = Path.Combine(targetDir,
            templateName.Replace("/", "_").Replace("\\", "_") + "." + dateTime + ".html");
            html = html.Replace("{ViewOnLine}", $"{host.TrimEnd('/')}/Emails/{Path.GetFileName(targetFileName)}");
            html = html.Replace("{ServerUrl}", host);
            File.WriteAllText(targetFileName, html);
            return html;
        }
    }
}
