using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class BusinessInfo
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UsersID { get; set; }

        [Required]
        [StringLength(20)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public string City { get; set; }

        //[Required]
        //[StringLength(50)]
        //[MaxLength(50)]
        //public string EMail { get; set; }

        [Required]
        public string AvatarImg { get; set; }

        [Required]
        public string MenuImg { get; set; }
    }
}
