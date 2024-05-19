using System;
using System.ComponentModel.DataAnnotations;

namespace task4.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        // private DateTime _sendDate;
        // public DateTime SendDate
        // {
        //     get => _sendDate;
        //     set => _sendDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        // }
        public DateTime SendDate { get; set; }

        public bool IsRead { get; set; }
    }
}