using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    public class UserChecking
    {
        [Required]
        public string userId { get; set; }

        [Required]
        public string userPs { get; set; }
    }
}