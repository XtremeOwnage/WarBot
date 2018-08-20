using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WarBot.Storage.Models
{
    public class User
    {
        [Key]
        public ulong ID { get; set; }
    }
}
