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
            IsEnable = false;
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
        /// <summary>
        /// Recursivelty get all files from Local Folder
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
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

        public override Task DownloadAsync(Server server, Dictionary<string, string> paths)
        {
            foreach (var path in paths)
            {
                System.IO.File.Copy(path.Key, path.Value);
            }

            return Task.CompletedTask;
        }
    }
}