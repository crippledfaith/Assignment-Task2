using Microsoft.EntityFrameworkCore;

namespace Task2.Context
{
    public partial class ServerContext : DbContext
    {
        public DbSet<File> Files { get; set; }
        public DbSet<Server> Servers { get; set; }

        public ServerContext()
        {
        }

        public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=127.0.0.1;Port=5432;Database=Task2;User Id=postgres;Password=Test@123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            OnModelCreatingPartial(modelBuilder);
            new DbInitializer(modelBuilder).Seed();
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
    public class DbInitializer
    {
        private readonly ModelBuilder modelBuilder;

        public DbInitializer(ModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }

        public void Seed()
        {
            modelBuilder.Entity<Server>().HasData(
                new Server() { Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C278", Name = "LocalFile1Server", ServerType = ServerType.Local, Url = "D:\\testFTP", UserName = "", Password = "" },
                new Server() { Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C279", Name = "LocalFile2Server", ServerType = ServerType.Local, Url = "D:\\output", UserName = "", Password = "" },
                new Server() { Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C280", Name = "FTPFileServer", ServerType = ServerType.FTP, Url = "192.168.50.11", Port = 21, UserName = "TestFtp", Password = "Test123" },
                new Server() { Id = "62CD1744-DD07-4BB4-8EE0-FC1359E2C281", Name = "SFTPFileServer", ServerType = ServerType.SFTP, Url = "test.rebex.net", Port = 22, UserName = "demo", Password = "password" }
            );
        }
    }
}
