using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class RoleModel : IdentityRole<Guid>
    {
        public RoleModel()
        {
        }

        public RoleModel(string roleName) : base(roleName)
        {
        }
    }
}
