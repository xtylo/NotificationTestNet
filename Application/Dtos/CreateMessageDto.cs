using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Dtos
{
    public record CreateMessageDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "The message body must be between 1 and 1000 characters.")]
        public string Body { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
