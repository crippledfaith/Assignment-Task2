using System.ComponentModel.DataAnnotations;

namespace Task2.Infrastructure.Context
{
    public class FileModel : IEntity
    {
        [Key]
        public string Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string ServerId { get; set; }
        public ServeModel Server { get; set; }
    }
}
