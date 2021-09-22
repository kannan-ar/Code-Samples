using System.ComponentModel.DataAnnotations;

namespace VideoStreamer.Infrastructure.Entities
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
