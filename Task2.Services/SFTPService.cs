using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Task2.Infrastructure;
using Task2.Infrastructure.Context;

namespace Task2.Services
{
    public class SFTPService : AHostedService, IService
    {
        public SFTPService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext) : base(logger, serverContext)
        {
            IsEnable = true;
            Interval = 60;
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
        private List<string> GetAllFiles(SftpClient sftp, string url)
        {
            var result = new List<string>();

            var files = sftp.ListDirectory(url);

            foreach (var file in files.Where(q => !q.IsDirectory))
            {
                result.Add(file.FullName);
            }


            foreach (var file in files.Where(q => q.IsDirectory && !q.FullName.Contains("/.") && !q.FullName.Contains("/..")))
            {
                result.AddRange(GetAllFiles(sftp, file.FullName));
            }

            return result;
        }


        public override ServerType GetServerType()
        {
            return ServerType.SFTP;
        }

    }
}