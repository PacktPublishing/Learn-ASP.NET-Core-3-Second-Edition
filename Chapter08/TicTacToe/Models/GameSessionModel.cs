using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class GameSessionModel
    {
        public Guid Id { get; set; }
        public Guid UserId1 { get; set; }
        public Guid UserId2 { get; set; }
        public UserModel User1 { get; set; }
        public UserModel User2 { get; set; }
        public IEnumerable<TurnModel> Turns { get; set; }
        public UserModel Winner { get; set; }
        public UserModel ActiveUser { get; set; }
        public Guid WinnerId { get; set; }
        public Guid ActiveUserId { get; set; }
        public bool TurnFinished { get; set; }
        public int TurnNumber { get; set; }
    }
}
