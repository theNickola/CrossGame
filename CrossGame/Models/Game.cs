using Microsoft.EntityFrameworkCore;

namespace CrossGame.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Player1 { get; set; }
        public string StatePl1 { get; set; } = "";
        public string Player2 { get; set; } = "";
        public string StatePl2 { get; set; } = "";
        public string Step { get; set; } = "";
        public string Winner { get; set; } = "";

        public override string ToString()
        {
            return Id + " " + Status + " " + Player1 + " " + StatePl1 + " " + Player2 + " " + StatePl2 + " " + Step + " " + Winner;
        }
    }
}
