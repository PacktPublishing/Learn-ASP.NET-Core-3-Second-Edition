using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.TagHelpers
{
    [HtmlTargetElement("Gravatar")]
    public class GravatarTagHelper : TagHelper
    {
        private ILogger<GravatarTagHelper> _logger;
        public GravatarTagHelper(ILogger<GravatarTagHelper> logger)
        {
            _logger = logger;
        }
        public string Email { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            byte[] photo = null;
            if (CheckIsConnected())
            {
                photo = GetPhoto(Email);
            }
            else
            {
                photo = File.ReadAllBytes(Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "images", "no-photo.jpg"));
            }
            string base64String = Convert.ToBase64String(photo);
            output.TagName = "img";
            output.Attributes.SetAttribute("src",
            $"data:image/jpeg;base64,{base64String}");
        }

        private byte[] GetPhoto(string email)
        {
            var httpClient = new HttpClient();
            return httpClient.GetByteArrayAsync(
            new Uri($"http://www.gravatar.com/avatar/ { HashEmailForGravatar(email) }")).Result;
        }

        private static string HashEmailForGravatar(string email)
        {
            var md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(
            Encoding.ASCII.GetBytes(email.ToLower()));
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        private bool CheckIsConnected()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var gravatarResponse = httpClient.GetAsync(
                    "http://www.gravatar.com/avatar/").Result;
                    return (gravatarResponse.IsSuccessStatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Cannot check the gravatar service status: { ex} ");
                return false;
            }
        }
    }
}
