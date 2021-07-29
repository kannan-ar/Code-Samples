using System.ComponentModel.DataAnnotations;

namespace VideoStreamer.Data.Entities
{

    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
