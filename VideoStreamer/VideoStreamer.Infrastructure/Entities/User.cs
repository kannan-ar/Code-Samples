using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VideoStreamer.Infrastructure.Entities
{
    [Index(nameof(UserName), Name = nameof(UserName), IsUnique = true)]
    public class User : Entity
    {
        [Required, StringLength(50)]
        public string UserName { get; set; }

        [Required, StringLength(255)]
        public string Password { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
