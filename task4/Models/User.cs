using System.ComponentModel.DataAnnotations;

namespace task4.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }
    }
}