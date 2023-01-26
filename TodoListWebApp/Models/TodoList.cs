using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoListWebApp.Models
{
    public class TodoList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "List Content")]
        public string ListContent { get; set; }

        [Required]
        public DateTime? Time { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid email address.")]
        public string EmailAddress { get; set; }
    }

    public static class Encryption
    {
        public static string encrypt(string ToEncrypt)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(ToEncrypt));
        }
        public static string decrypt(string cypherString)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(cypherString));
        }
    }
}
