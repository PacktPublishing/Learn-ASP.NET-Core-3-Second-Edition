using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Models
{
    public class TwoFactorCodeModel
    {

        [Key]
        public long Id { get; set; }
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public UserModel User { get; set; }
        public string TokenProvider { get; set; }
        public string TokenCode { get; set; }
    }
}
