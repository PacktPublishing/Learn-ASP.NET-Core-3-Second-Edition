using Microsoft.AspNetCore.Http;
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
using Newtonsoft.Json;

namespace TicTacToe.Middlewares
{
    public class CommunicationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserService _userService;

        public CommunicationMiddleware(RequestDelegate next,
         IUserService userService)
        {
            _next = next;
            _userService = userService;
        }


        //public async Task Invoke(HttpContext context)
        //{
        //    if (context.Request.Path.Equals(
        //      "/CheckEmailConfirmationStatus"))
        //    {
        //        await ProcessEmailConfirmation(context);
        //    }
        //    else
        //    {
        //        await _next?.Invoke(context);
        //    }
        //}

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
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

        private async Task ProcessEmailConfirmation(HttpContext context)
        {
            var email = context.Request.Query["email"];
            var user = await _userService.GetUserByEmail(email);

            if (string.IsNullOrEmpty(email))
            {
                await context.Response.WriteAsync("BadRequest:Email is required");
            }
            else if (
             (await _userService.GetUserByEmail(email)).IsEmailConfirmed)
            {
                await context.Response.WriteAsync("OK");
            }
            else
            {
                await context.Response.WriteAsync(
                 "WaitingForEmailConfirmation");
                user.IsEmailConfirmed = true;
                user.EmailConfirmationDate = DateTime.Now;
                _userService.UpdateUser(user).Wait();

            }
        }

        private static Task SendStringAsync(WebSocket socket,
        string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text,
             true, ct);
        }

        private static async Task<string> ReceiveStringAsync(
        WebSocket socket, CancellationToken ct =
         default(CancellationToken))
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

        public async Task ProcessEmailConfirmation(HttpContext context,
         WebSocket currentSocket, CancellationToken ct, string email)
        {
            UserModel user = await _userService.GetUserByEmail(email);
            while (!ct.IsCancellationRequested &&
             !currentSocket.CloseStatus.HasValue &&
             user?.IsEmailConfirmed == false)
            {
                if (user.IsEmailConfirmed)
                {
                    await SendStringAsync(currentSocket, "OK", ct);
                }
                else
                {
                    user.IsEmailConfirmed = true;
                    user.EmailConfirmationDate = DateTime.Now;

                    await _userService.UpdateUser(user);
                    await SendStringAsync(currentSocket, "OK", ct);
                }

                Task.Delay(500).Wait();
                user = await _userService.GetUserByEmail(email);
            }
        }
        private async Task ProcessGameInvitationConfirmation(HttpContext
         context)
        {
            var id = context.Request.Query["id"];
            if (string.IsNullOrEmpty(id))
                await context.Response.WriteAsync("BadRequest:Id is required");

            var gameInvitationService =
              context.RequestServices.GetService<IGameInvitationService>();
            var gameInvitationModel =
              await gameInvitationService.Get(Guid.Parse(id));

            if (gameInvitationModel.IsConfirmed)
                await context.Response.WriteAsync(
                 JsonConvert.SerializeObject(new
                 {
                     Result = "OK",
                     Email = gameInvitationModel.InvitedBy,
                     gameInvitationModel.EmailTo
                 }));
            else
            {
                await context.Response.WriteAsync(
                  "WaitGameInvitationConfirmation");
            }
        }
        private async Task
         ProcessGameInvitationConfirmation(HttpContext context,
         WebSocket webSocket, CancellationToken ct, string parameters)
        {
            var gameInvitationService =
              context.RequestServices.GetService<IGameInvitationService>();
            var id = Guid.Parse(parameters);
            var gameInvitationModel = await gameInvitationService.Get(id);
            while (!ct.IsCancellationRequested &&
                   !webSocket.CloseStatus.HasValue &&
                    gameInvitationModel?.IsConfirmed == false)
            {
                await SendStringAsync(webSocket,
                 JsonConvert.SerializeObject(new
                 {
                     Result = "OK",
                     Email = gameInvitationModel.InvitedBy,
                     gameInvitationModel.EmailTo,
                     gameInvitationModel.Id
                 }), ct);

                Task.Delay(500).Wait();

                gameInvitationModel = await gameInvitationService.Get(id);
            }
        }
    }
}
