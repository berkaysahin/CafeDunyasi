using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class Posts
    {
        [Key]
        public int Id { get; set; }

        public int BusinessID { get; set; }

        [Required]
        public string ImageURL { get; set; }

        public DateTime Date { get; set; }
    }
}
