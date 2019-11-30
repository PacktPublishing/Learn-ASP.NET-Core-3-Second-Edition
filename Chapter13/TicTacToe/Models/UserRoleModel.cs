using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class UserRoleModel : IdentityUserRole<Guid>
    {
        [Key]
        public long Id { get; set; }
    }
}
