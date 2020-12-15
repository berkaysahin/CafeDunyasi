using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CafeDunyasi.Models
{
    public class City
    {
        [MaxLength(2)]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
    }
}
