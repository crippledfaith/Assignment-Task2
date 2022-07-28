using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Task2.Context;
using File = Task2.Context.File;

namespace Task2
{
    public abstract class AHostedService : IHostedService, IDisposable
    {
        private int _executionCount = 0;
        private Timer? _timer = null;
        private readonly ILogger<AHostedService> _logger;
        private List<Server> _servers;

        public IDbContextFactory<ServerContext> ServerContextFactory { get; private set; }
        public bool IsEnable { get; set; }
        public int Interval { get; set; } = 10;

        public List<Server> Servers
        {
            get
            {
                return _servers = _servers ?? GetServerInfomationAsync().Result;
            }
        }

        public abstract Task ExcuteAsync();
        public abstract ServerType GetServerType();

        public AHostedService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext)
        {
            _logger = logger;
            this.ServerContextFactory = serverContext;
            _servers = new List<Server>();
            _logger.LogInformation("Service Initialized");

        }

        public async Task<List<Server>> GetServerInfomationAsync()
        {
            List<Server> server = new List<Server>();
            try
            {
                _logger.LogInformation("Getting Server Information From Database.");
                var context = ServerContextFactory.CreateDbContext();
                server = await context.Servers.Where(q => q.ServerType == GetServerType()).ToListAsync();
                if (server == null)
                {
                    server = new List<Server>();
                    _logger.LogWarning($"No Serve of type{GetServerType()} found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while read data.");
            }
            return server;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            if (IsEnable)
                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                    TimeSpan.FromSeconds(Interval));

            return Task.CompletedTask;
        }

        public async Task SavePathAsync(List<string> paths)
        {
            try
            {
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
                if (paths.Count > 0)
                {
                    await context.SaveChangesAsync();
                }
                _logger.LogInformation("{Count} new file paths found from {Type}.", paths.Count, this.ToString());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving.");
            }

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

        private async void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref _executionCount);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("Excuting File Check", this.ToString());
            try
            {
                await ExcuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in excuting service{Type}", this.ToString());
            }
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(Interval),
            TimeSpan.FromSeconds(Interval));
            _logger.LogInformation("{Type} Hosted Service called: {Count}", this.ToString(), count);
        }
    }
}
