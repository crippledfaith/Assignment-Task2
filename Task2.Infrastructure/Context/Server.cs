using System.ComponentModel.DataAnnotations;

namespace Task2.Context
{
    public class Server : IEntity
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public ServerType ServerType { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
