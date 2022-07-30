using FluentFTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task2.Infrastructure;
using Task2.Infrastructure.Context;

namespace Task2.Services
{
    public class FTPService : AHostedService, IService
    {
        public FTPService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext, IOptions<ServiceSetting> settings) : base(logger, serverContext, settings)
        {
            IsEnable = false;
        }

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
                await SyncPathAsync(files, server);
            }

            return;
        }
        /// <summary>
        /// Recursivelty get all files from FTP server
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        private Dictionary<string, DateTime> GetAllFiles(FtpClient client, string url)
        {
            var result = new Dictionary<string, DateTime>();

            foreach (FtpListItem item in client.GetListing(url))
            {

                if (item.Type == FtpObjectType.File)
                {
                    result.Add(item.FullName, item.Modified.ToUniversalTime());
                }
                else if (item.Type == FtpObjectType.Directory)
                {
                    foreach (var value in GetAllFiles(client, item.FullName))
                    {
                        result.Add(value.Key, value.Value);
                    }
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

        public override async Task DownloadAsync(Server server, Dictionary<string, string> paths)
        {
            using (FtpClient client = new FtpClient(server.Url, server.Port, server.UserName, server.Password))
            {
                client.ValidateCertificate += ClientValidateCertificate;
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.AutoConnect();
                foreach (var path in paths)
                {
                    await client.DownloadFileAsync(path.Value, path.Key);
                }
                client.Disconnect();
            }
            return;
        }
    }
}