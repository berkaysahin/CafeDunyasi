using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class FollowingAccounts
    {
        [Key]
        public int Id { get; set; }
        public string UserID { get; set; }
        public int BusinessID { get; set; }
    }
}
