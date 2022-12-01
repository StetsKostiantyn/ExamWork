using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPChat
{
    public class Users
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("time")]/////////////////////////
        public DateTime Time { get; set; }

        [Column("address")]
        public string Address { get; set; }

        public List<Messeges> Messeges { get; set; }
    }
}
