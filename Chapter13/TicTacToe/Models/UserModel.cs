using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Models
{
    public class UserModel : IdentityUser<Guid>
    {
        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }
        [Display(Name = "LastName")]
        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "PasswordRequired"), DataType(DataType.Password)]
        public string Password { get; set; }
        [NotMapped]
        public bool IsEmailConfirmed
        {
            get { return EmailConfirmed; }
        }
        public System.DateTime? EmailConfirmationDate { get; set; }
        public int Score { get; set; }
    }
}
