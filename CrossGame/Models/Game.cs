using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CrossGame.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Player1 { get; set; }
        public string StepsPl1 { get; set; } = "";
        public string Player2 { get; set; } = "";
        public string StepsPl2 { get; set; } = "";
        public string FirstMover { get; set; } = "";
        public string Mover { get; set; } = "";
        public string Winner { get; set; } = "";
    }
}
