using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class Stories
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string BusinessID { get; set; }
    }
}
