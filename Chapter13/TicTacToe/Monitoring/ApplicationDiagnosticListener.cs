using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DiagnosticAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TicTacToe.Monitoring
{
    public class ApplicationDiagnosticListener
    {
        [DiagnosticName("TicTacToe.MiddlewareStarting")]
        public virtual void OnMiddlewareStarting(HttpContext httpContext)
        {
            Console.WriteLine($"TicTacToe Middleware Starting, path: {httpContext.Request.Path}");
        }

        [DiagnosticName("TicTacToe.NewUserRegistration")]
        public virtual void NewUserRegistration(string name)
        {
            Console.WriteLine($"New User Registration {name}");
        }
    }
}
