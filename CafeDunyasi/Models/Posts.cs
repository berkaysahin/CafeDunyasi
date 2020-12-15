using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class Posts
    {
        [Key]
        public int Id { get; set; }

        public string UserID { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
