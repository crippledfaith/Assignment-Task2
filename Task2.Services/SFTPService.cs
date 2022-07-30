using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Task2.Infrastructure;
using Task2.Infrastructure.Context;

namespace Task2.Services
{
    public class SFTPService : AHostedService, IService
    {
        public SFTPService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext, IOptions<ServiceSetting> settings) : base(logger, serverContext, settings)
        {
            IsEnable = true;
        }

        public override async Task ExcuteAsync()
        {
            foreach (var server in Servers)
            {
                SftpClient sFtpClient = new SftpClient(server.Url, server.UserName, server.Password);
                sFtpClient.Connect();
                var files = GetAllFiles(sFtpClient, "/");
                sFtpClient.Disconnect();
                await SyncPathAsync(files, server);
            }

            return;
        }
        /// <summary>
        /// Recursivelty get all files from SFTP server
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        private Dictionary<string, DateTime> GetAllFiles(SftpClient client, string path)
        {
            var result = new Dictionary<string, DateTime>();
            var files = client.ListDirectory(path);

            foreach (var file in files.Where(q => !q.IsDirectory))
            {
                result.Add(file.FullName, file.LastWriteTimeUtc);
            }


            foreach (var file in files.Where(q => q.IsDirectory && !q.FullName.Contains("/.") && !q.FullName.Contains("/..")))
            {
                foreach (var item in GetAllFiles(client, file.FullName))
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }


        public override ServerType GetServerType()
        {
            return ServerType.SFTP;
        }

        public override Task DownloadAsync(Server server, Dictionary<string, string> paths)
        {
            using (SftpClient sFtpClient = new SftpClient(server.Url, server.UserName, server.Password))
            {
                sFtpClient.Connect();
                foreach (var path in paths)
                {
                    using (Stream fileStream = System.IO.File.Create(path.Value))
                    {
                        sFtpClient.DownloadFile(path.Key, fileStream);
                    }
                }

            }
            return Task.CompletedTask;
        }
    }
}