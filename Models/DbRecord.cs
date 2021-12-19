using System;

namespace Models
{
    public class DbRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = $"{Guid.NewGuid()}";
        public string Game { get; set; } = null!;
    }
}