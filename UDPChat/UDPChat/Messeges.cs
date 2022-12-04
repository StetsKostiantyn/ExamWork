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

        [Column("senderId")]
        public int SenderId { get; set; }

        [Column("receiverId")]
        public int ReceiverId { get; set; }


        public Users Sender { get; set; }
        public Users Receiver { get; set; }
    }
}