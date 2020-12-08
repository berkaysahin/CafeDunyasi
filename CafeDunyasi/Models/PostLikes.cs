using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class PostLikes
    {
        [Key]
        public int Id { get; set; }

        public int PostID { get; set; }

        public int UserID { get; set; }
    }
}
