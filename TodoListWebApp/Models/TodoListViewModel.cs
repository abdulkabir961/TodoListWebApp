using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListWebApp.Models
{
    public class TodoListViewModel
    {
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
}
