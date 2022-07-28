using System.ComponentModel.DataAnnotations;

namespace Task2.Context
{
    public class File : IEntity
    {
        [Key]
        public string Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public ServerType ServerType { get; set; }

    }
}
