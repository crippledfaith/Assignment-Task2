using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task2.Infrastructure;
using Task2.Infrastructure.Context;

namespace Task2.Services
{
    public class LocalService : AHostedService, IService
    {
        public LocalService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext) : base(logger, serverContext)
        {
            IsEnable = true;
            Interval = 60;
        }


        public override async Task ExcuteAsync()
        {
            foreach (var server in Servers)
            {
                var files = GetAllFiles(server.Url);
                await SavePathAsync(files);
            }
            return;
        }

        private List<string> GetAllFiles(string url)
        {
            var result = new List<string>();
            result = Directory.GetFiles(url).ToList();
            foreach (var director in Directory.GetDirectories(url))
            {
                result.AddRange(GetAllFiles(director));
            }

            return result;
        }

        public override ServerType GetServerType()
        {
            return ServerType.Local;
        }

    }
}