using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task2.Infrastructure.Context;
using File = Task2.Infrastructure.Context.File;

namespace Task2.Infrastructure
{
    public abstract class AHostedService : IHostedService, IDisposable
    {
        private int _executionCount = 0;
        private Timer? _timer = null;
        private readonly ILogger<AHostedService> _logger;
        private List<Server> _servers;

        public IDbContextFactory<ServerContext> ServerContextFactory { get; private set; }
        public ServiceSetting Settings { get; }
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

        public abstract Task DownloadAsync(Server server, Dictionary<string, string> paths);
        public abstract ServerType GetServerType();
        public AHostedService(ILogger<AHostedService> logger, IDbContextFactory<ServerContext> serverContext, IOptions<ServiceSetting> settings)
        {
            _logger = logger;
            ServerContextFactory = serverContext;
            Settings = settings.Value;
            Interval = settings.Value.ServiceInterval;
            _logger.LogInformation("{Type} Service Initialized", this.ToString());
        }

        /// <summary>
        /// Saves file Path to Database and download files locally
        /// </summary>
        public async Task SyncPathAsync(Dictionary<string, DateTime> paths, Server server)
        {
            try
            {
                var context = ServerContextFactory.CreateDbContext();
                var files = context?.Files?.Where(q => q.ServerType == GetServerType()).ToList();
                Dictionary<string, string> newPaths = new Dictionary<string, string>();
                if (files != null && context != null)
                {
                    foreach (var xpath in paths)
                    {
                        var path = xpath.Key;
                        string localFilePath = GetFileLocalPath(paths.Keys.ToList(), path, server.Name, Settings.UseFlatDirectory);

                        var localFileCreationDate = xpath.Value;
                        var localTime = localFileCreationDate.ToLocalTime();
                        if (System.IO.File.Exists(localFilePath))
                        {
                            var fileDownloaded = files.FirstOrDefault(q => q.Path == path && (localTime - q.CreationDate).TotalSeconds > .1);
                            if (fileDownloaded != null)
                            {
                                fileDownloaded.CreationDate = localTime;
                                context.Update(fileDownloaded);
                                newPaths.Add(path, localFilePath);
                                System.IO.File.Delete(localFilePath);
                            }
                        }
                        else
                        {
                            context.Add(new File()
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = Path.GetFileName(path),
                                Path = path,
                                ServerType = server.ServerType,
                                CreationDate = localTime,
                            });
                            newPaths.Add(path, localFilePath);
                        }
                    }

                    if (newPaths.Count > 0)
                    {
                        await DownloadAsync(server, newPaths);
                        await context.SaveChangesAsync();
                    }

                }
                _logger.LogInformation("{Count} new file paths found from {Type}.", newPaths.Count, ToString());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving.");
            }
        }



        /// <summary>
        /// Start Service
        /// </summary>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            if (IsEnable)
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(Interval));

            return Task.CompletedTask;
        }
        /// <summary>
        /// Stop Service
        /// </summary>
        /// <param name="stoppingToken"></param>
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
        /// <summary>
        /// Get Service Setting from Database
        /// </summary>
        /// <returns>List Of Server</returns>
        private async Task<List<Server>> GetServerInfomationAsync()
        {
            List<Server> server = new List<Server>();
            try
            {
                _logger.LogInformation("Getting Server Information From Database.");
                var context = ServerContextFactory.CreateDbContext();
                if (context != null && context.Servers != null)
                {
                    server = await context.Servers.Where(q => q.ServerType == GetServerType()).ToListAsync();
                    if (server == null)
                    {
                        server = new List<Server>();
                        _logger.LogWarning($"No Serve of type{GetServerType()} found");
                    }
                }
                else
                {
                    server = new List<Server>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while read data.");
            }
            return server;
        }
        private async void DoWork(object? state)
        {
            var count = Interlocked.Increment(ref _executionCount);
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("Excuting File Check {Type}", ToString());
            try
            {
                await ExcuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in excuting service{Type}", ToString());
            }
            _timer?.Change(TimeSpan.FromSeconds(Interval), TimeSpan.FromSeconds(Interval));
            _logger.LogInformation("{Type} Hosted Service called: {Count}", ToString(), count);
        }
        /// <summary>
        /// Get the local path as it is in the server
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="curentFilePath"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        private string GetFileLocalPath(List<string> paths, string curentFilePath, string serverName, bool useFlatDirectory)
        {
            var localPath = Path.Combine(Settings.ServiceLocalPath, serverName);
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            if (!useFlatDirectory)
            {
                var commonPath = GetCommonPath(paths);
                string directoryPath = curentFilePath.Replace(Path.GetFileName(curentFilePath), "");
                var serverPath = commonPath.Length > 1 ? directoryPath.Replace(commonPath, "") : directoryPath;

                List<string> pathSplit = serverPath.Split('/').ToList();
                var pathSplit2 = serverPath.Split('\\').ToList();
                if (pathSplit.Count > 1 && pathSplit2.Count > 1)
                {
                    pathSplit.AddRange(pathSplit2);
                }
                else if (pathSplit2.Count > 1)
                {
                    pathSplit = pathSplit2;
                }
                else if (pathSplit.Count == 1)
                {
                    pathSplit = new List<string>();
                }
                foreach (var path in pathSplit)
                {
                    localPath = Path.Combine(localPath, path);
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }
                }
                localPath = Path.Combine(localPath, Path.GetFileName(curentFilePath));
            }
            else
            {
                var sha1 = System.Security.Cryptography.SHA1.Create();
                byte[] buf = System.Text.Encoding.UTF8.GetBytes(curentFilePath);
                byte[] hash = sha1.ComputeHash(buf, 0, buf.Length);
                //var hashstr  = Convert.ToBase64String(hash);
                var hashstr = BitConverter.ToString(hash).Replace("-", "");
                var extension = Path.GetExtension(curentFilePath);
                localPath = Path.Combine(localPath, $"{hashstr}{extension}");
            }
            return localPath;
        }

        private string GetCommonPath(List<string> paths)
        {
            var ignored = new HashSet<char>("-& _");
            var transformed = paths.Select(s => s.Where(c => !ignored.Contains(c)).ToArray()).ToList();
            var commonPrefix = new string(transformed.First()
               .Take(transformed.Min(s => s.Length))
               .TakeWhile((c, i) => transformed.All(s => s[i] == c))
               .ToArray());
            return commonPrefix;
        }
    }
}
