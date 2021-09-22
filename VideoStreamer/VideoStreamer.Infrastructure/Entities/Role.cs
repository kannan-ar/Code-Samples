using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VideoStreamer.Infrastructure.Entities
{
    [Index(nameof(RoleName), Name = nameof(RoleName), IsUnique = true)]
    public class Role : Entity
    {
        [StringLength(50)]
        public string RoleName { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
