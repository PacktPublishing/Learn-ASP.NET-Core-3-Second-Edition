using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.ViewEngines;

namespace TicTacToe.Helpers
{
    public class EmailViewRenderHelper
    {
        IWebHostEnvironment _hostingEnvironment;
        IConfiguration _configurationRoot;
        IHttpContextAccessor _httpContextAccessor;
        public async Task<string> RenderTemplate<T>(string template,
        IWebHostEnvironment hostingEnvironment, IConfiguration
        configurationRoot, IHttpContextAccessor httpContextAccessor,
        T model) where T : class
        {
            _hostingEnvironment = hostingEnvironment;
            _configurationRoot = configurationRoot;
            _httpContextAccessor = httpContextAccessor;
            var renderer = httpContextAccessor.HttpContext.RequestServices
            .GetRequiredService<IEmailViewEngine>();
            return await renderer.RenderEmailToString<T>(template, model);
        }
    }
}
