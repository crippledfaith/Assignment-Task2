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
        private Dictionary<string, DateTime> GetAllFiles(FtpClient client, string url)
        {
            var result = new Dictionary<string, DateTime>();

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

        public override Task DownloadAsync(string path, string toLocalPath)
        {
            throw new NotImplementedException();
        }
    }
}