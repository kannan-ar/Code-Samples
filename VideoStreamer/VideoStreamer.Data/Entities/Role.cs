using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VideoStreamer.Data.Entities
{
    [Index(nameof(RoleName), Name = nameof(RoleName), IsUnique = true)]
    public class Role : BaseEntity
    {
        [StringLength(50)]
        public string RoleName { get; set; }
    }
}
