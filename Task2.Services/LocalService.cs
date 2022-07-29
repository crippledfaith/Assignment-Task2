using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task2.Infrastructure;
using Task2.Infrastructure.Context;

namespace Task2.Services
{
    public class LocalService : AHostedService, IService
    {
        public LocalService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext, IOptions<ServiceSetting> settings) : base(logger, serverContext, settings)
        {
            IsEnable = true;
        }


        public override async Task ExcuteAsync()
        {
            foreach (var server in Servers)
            {
                var files = GetAllFiles(server.Url);
                await SyncPathAsync(files, server);
            }
            return;
        }

        private Dictionary<string, DateTime> GetAllFiles(string url)
        {
            var result = new Dictionary<string, DateTime>();
            Directory.GetFiles(url).ToList().ForEach(q => result.Add(q, new FileInfo(q).CreationTimeUtc));
            foreach (var director in Directory.GetDirectories(url))
            {
                foreach (var item in GetAllFiles(director))
                {
                    result.Add(item.Key, item.Value);
                }
            }

            return result;
        }

        public override ServerType GetServerType()
        {
            return ServerType.Local;
        }

        public override Task DownloadAsync(Server server, string path, string toLocalPath)
        {
            System.IO.File.Copy(path, toLocalPath);
            return Task.CompletedTask;
        }
    }
}