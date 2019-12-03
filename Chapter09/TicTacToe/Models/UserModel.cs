using System;
using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Models
{
    public class UserModel
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "FirstName")]
        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "EmailRequired"),
         DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "PasswordRequired"),
         DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public System.DateTime? EmailConfirmationDate { get; set; }
        public int Score { get; set; }
    }
}
