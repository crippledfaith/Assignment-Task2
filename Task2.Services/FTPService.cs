﻿using FluentFTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task2.Context;

namespace Task2.Services
{
    public class FTPService : AHostedService, IService
    {
        public FTPService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext) : base(logger, serverContext)
        {
            IsEnable = false;
            Interval = 60;
        }



        public ILogger<AHostedService> Logger { get; }

        public override async Task ExcuteAsync()
        {
            foreach (var server in Servers)
            {
                FtpClient client = new FtpClient(server.Url, server.Port, server.UserName, server.Password);
                client.ValidateCertificate += ClientValidateCertificate;
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.AutoConnect();
                var files = GetAllFiles(client, "/");
                client.Disconnect();
                client.Dispose();
                await SavePathAsync(files);
            }

            return;
        }
        private List<string> GetAllFiles(FtpClient client, string url)
        {
            var result = new List<string>();

            foreach (FtpListItem item in client.GetListing(url))
            {

                if (item.Type == FtpObjectType.File)
                {
                    result.Add(item.FullName);
                }
                else if (item.Type == FtpObjectType.Directory)
                {
                    result.AddRange(GetAllFiles(client, item.FullName));
                }
            }


            return result;
        }
        private void ClientValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {

            e.Accept = true;

        }

        public override ServerType GetServerType()
        {
            return ServerType.FTP;
        }

    }
}