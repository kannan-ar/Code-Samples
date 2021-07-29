using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoStreamer.Data.Entities
{
    [Index(nameof(UserName), Name = nameof(UserName), IsUnique = true)]
    public class User : BaseEntity
    {
        [Required, StringLength(50)]
        public string UserName { get; set; }

        [Required, StringLength(255)]
        public string Password { get; set; }
    }
}
