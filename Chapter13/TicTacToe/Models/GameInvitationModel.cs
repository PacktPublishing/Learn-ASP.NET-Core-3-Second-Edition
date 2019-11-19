using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class GameInvitationModel
    {
        [Key]
        public Guid Id { get; set; }
        public string EmailTo { get; set; }

        public string InvitedBy { get; set; }

        [ForeignKey(nameof(InvitedByUserId))]
        public UserModel InvitedByUser { get; set; }
        public Guid InvitedByUserId { get; set; }

        public bool IsConfirmed { get; set; }
        public DateTime ConfirmationDate { get; set; }
    }
}
