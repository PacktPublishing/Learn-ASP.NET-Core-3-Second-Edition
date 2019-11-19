using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicTacToe.Middlewares;

namespace TicTacToe.Extensions
{
    public static class CommunicationMiddlewareExtension
    {
        public static IApplicationBuilder
           UseCommunicationMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CommunicationMiddleware>();
        }
    }
}
