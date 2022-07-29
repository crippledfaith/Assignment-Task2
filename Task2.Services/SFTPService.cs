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
                SftpClient sftp = new SftpClient(server.Url, server.UserName, server.Password);
                sftp.Connect();
                var files = GetAllFiles(sftp, "/");
                sftp.Disconnect();
                await SavePathAsync(files);
            }

            return;
        }
        /// <summary>
        /// Recursivelty get all files from SFTP server
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        private List<string> GetAllFiles(SftpClient client, string path)
        {
            var result = new List<string>();

            var files = client.ListDirectory(path);

            foreach (var file in files.Where(q => !q.IsDirectory))
            {
                result.Add(file.FullName);
            }


            foreach (var file in files.Where(q => q.IsDirectory && !q.FullName.Contains("/.") && !q.FullName.Contains("/..")))
            {
                result.AddRange(GetAllFiles(client, file.FullName));
            }

            return result;
        }


        public override ServerType GetServerType()
        {
            return ServerType.SFTP;
        }

    }
}