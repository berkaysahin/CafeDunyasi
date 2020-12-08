using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class Users
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [MaxLength(50)]
        public string Surname { get; set; }

        [Required]
        [StringLength(50)]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        [MaxLength(50)]
        public string EMail { get; set; }

        [Required]
        [StringLength(50)]
        [MaxLength(50)]
        public string Password { get; set; }

        public bool BusinessAccount { get; set; } = false;
    }
}
