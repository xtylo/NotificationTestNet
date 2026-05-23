using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public record CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
