using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class InvitationEmailModel
    {
        public string DisplayName { get; set; }
        public UserModel InvitedBy { get; set; }
        public DateTime InvitedDate { get; set; }
        public string ConfirmationUrl { get; set; }
    }
}
