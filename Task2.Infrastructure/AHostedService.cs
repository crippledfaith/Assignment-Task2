using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task2.Context;
using File = Task2.Context.File;

namespace Task2
{
    public abstract class AHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<AHostedService> _logger;
        public IDbContextFactory<ServerContext> ServerContextFactory { get; private set; }
        private Timer? _timer = null;
        public abstract Task ExcuteAsync();
        public abstract ServerType GetServerType();
        public bool IsEnable { get; set; }
        private List<Server> _servers;
        public List<Server> Servers
        {
            get
            {
                return _servers = _servers ?? GetServerInfomationAsync().Result;
            }
        }
        public AHostedService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext)
        {
            _logger = logger;
            this.ServerContextFactory = serverContext;
            _logger.LogInformation("Service Initialized");

        }

        public async Task<List<Server>> GetServerInfomationAsync()
        {
            _logger.LogInformation("Getting Server Information From Database.");
            var context = ServerContextFactory.CreateDbContext();
            List<Server> server = await context.Servers.Where(q => q.ServerType == GetServerType()).ToListAsync();
            if (server == null)
            {
                _logger.LogWarning($"No Serve of type{GetServerType()} found");
            }
            return server;
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            if (IsEnable)
                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }
        public async Task SavePathAsync(List<string> paths)
        {
            _logger.LogInformation("Saveing new file paths to Database.");
            var context = ServerContextFactory.CreateDbContext();
            var files = context.Files.Where(q => q.ServerType == GetServerType() && paths.Any(r => r == q.Path));
            paths.RemoveAll(q => files.Any(r => r.Path == q));
            paths.ForEach(q =>
            {
                context.Add(new File()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Path.GetFileName(q),
                    Path = q,
                    ServerType = GetServerType()
                });
            });
            await context.SaveChangesAsync();
        }
        private async void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref executionCount);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("Excuting FileCheck");
            try
            {
                await ExcuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in excuting service{Type}", this.ToString());
            }

            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5),
           TimeSpan.FromSeconds(5));
            _logger.LogInformation("{Type} Hosted Service is working. Count: {Count}", this.ToString(), count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
