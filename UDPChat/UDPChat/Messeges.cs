using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPChat
{
    public class Messeges
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("time")]/////////////////////////
        public DateTime Time { get; set; }

        [Column("sourceUserId")]
        public int SourceUserId { get; set; }

        [Column("destUserId")]
        public int DestUserId { get; set; }


        public Users SourceUser { get; set; }
    }
}