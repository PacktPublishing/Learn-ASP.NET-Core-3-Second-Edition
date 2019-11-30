using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicTacToe.Models;
using TicTacToe.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace TicTacToe.Middlewares
{
    public class CommunicationMiddleware
    {
        private readonly RequestDelegate _next;
        private DiagnosticSource _diagnosticSource;
        public CommunicationMiddleware(RequestDelegate next, DiagnosticSource diagnosticSource)
        {
            _next = next;
            _diagnosticSource = diagnosticSource;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                if (_diagnosticSource.IsEnabled("TicTacToe.MiddlewareStarting"))
                {
                    _diagnosticSource.Write("TicTacToe.MiddlewareStarting",
                        new
                        {
                            httpContext = context
                        });
                }

                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var ct = context.RequestAborted;
                var json = await ReceiveStringAsync(webSocket, ct);
                var command = JsonConvert.DeserializeObject<dynamic>(json);

                switch (command.Operation.ToString())
                {
                    case "CheckEmailConfirmationStatus":
                        {
                            await ProcessEmailConfirmation(context, webSocket, ct, command.Parameters.ToString());
                            break;
                        }

                    case "CheckGameInvitationConfirmationStatus":
                        {
                            await ProcessGameInvitationConfirmation(context, webSocket, ct, command.Parameters.ToString());
                            break;
                        }
                }
            }
            else if (context.Request.Path.Equals("/CheckEmailConfirmationStatus"))
            {
                await ProcessEmailConfirmation(context);
            }
            else if (context.Request.Path.Equals("/CheckGameInvitationConfirmationStatus"))
            {
                await ProcessGameInvitationConfirmation(context);
            }
            else
            {
                await _next?.Invoke(context);
            }
        }

        private async Task ProcessEmailConfirmation(HttpContext context, WebSocket currentSocket, CancellationToken ct, string email)
        {
            var userService = context.RequestServices.GetRequiredService<IUserService>();
            var user = await userService.GetUserByEmail(email);
            while (!ct.IsCancellationRequested && !currentSocket.CloseStatus.HasValue && user?.IsEmailConfirmed == false)
            {
                await SendStringAsync(currentSocket, "WaitEmailConfirmation", ct);
                await Task.Delay(500);
                user = await userService.GetUserByEmail(email);
            }

            if (user.IsEmailConfirmed)
            {
                await SendStringAsync(currentSocket, "OK", ct);
            }
        }

        private async Task ProcessEmailConfirmation(HttpContext context)
        {
            var userService = context.RequestServices.GetRequiredService<IUserService>();
            var email = context.Request.Query["email"];

            UserModel user = await userService.GetUserByEmail(email);

            if (string.IsNullOrEmpty(email))
            {
                await context.Response.WriteAsync("BadRequest:Email is required");
            }
            else if ((await userService.GetUserByEmail(email)).IsEmailConfirmed)
            {
                await context.Response.WriteAsync("OK");
            }
        }

        private static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                    throw new Exception("Unexpected message");

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private async Task ProcessGameInvitationConfirmation(HttpContext context)
        {
            var id = context.Request.Query["id"];
            if (string.IsNullOrEmpty(id))
                await context.Response.WriteAsync("BadRequest:Id is required");

            var gameInvitationService = context.RequestServices.GetService<IGameInvitationService>();
            var gameInvitationModel = await gameInvitationService.Get(Guid.Parse(id));

            if (gameInvitationModel.IsConfirmed)
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Result = "OK",
                    Email = gameInvitationModel.InvitedBy,
                    gameInvitationModel.EmailTo
                }));
            else
            {
                await context.Response.WriteAsync("WaitGameInvitationConfirmation");
            }
        }

        private async Task ProcessGameInvitationConfirmation(HttpContext context, WebSocket webSocket, CancellationToken ct, string parameters)
        {
            var gameInvitationService = context.RequestServices.GetService<IGameInvitationService>();
            var id = Guid.Parse(parameters);
            var gameInvitationModel = await gameInvitationService.Get(id);
            while (!ct.IsCancellationRequested && !webSocket.CloseStatus.HasValue && gameInvitationModel?.IsConfirmed == false)
            {
                await Task.Delay(500);
                gameInvitationModel = await gameInvitationService.Get(id);
                await SendStringAsync(webSocket, "WaitForConfirmation", ct);
            }

            if (gameInvitationModel.IsConfirmed)
            {
                await SendStringAsync(webSocket, JsonConvert.SerializeObject(new
                {
                    Result = "OK",
                    Email = gameInvitationModel.InvitedBy,
                    gameInvitationModel.EmailTo,
                    gameInvitationModel.Id
                }), ct);
            }
        }
    }
}
