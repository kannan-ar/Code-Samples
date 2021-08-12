using System.ComponentModel.DataAnnotations;

namespace VideoStreamer.Data.Entities
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
