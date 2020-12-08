using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class PostComments
    {
        [Key]
        public int Id { get; set; }

        public int PostID { get; set; }

        public int UserID { get; set; }

        public int Comment { get; set; }
    }
}
