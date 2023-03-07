using CrossGame.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CrossGame
{
    public class GameCrossHub : Hub
    {
        private readonly ILogger<GameCrossHub> _logger;
        private readonly ApplicationContext db;
        public GameCrossHub(ILogger<GameCrossHub> logger, ApplicationContext db)
        {
            _logger = logger;
            this.db = db;
        }
        public async Task Send(
            string idGame, 
            string statusGame,
            string player1, 
            string statePl1,
            string player2, 
            string statePl2,
            string step,
            string winner)
        {
            List<Game> games = db.Games.ToList();
            Game? game = new Game();

            _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!!Сервер получил: " + idGame + "  " + statusGame + "  " + player1 + "  " + statePl1 + "  " + player2 + "  " + statePl2 + "  " + step);

            if (statusGame == "pre-start")
            {
                game = games.LastOrDefault(g => g.Status == "pre-start" && g.StatePl1 == "pending");
                if (game != null)
                {
                    game.Player2 = player1;
                    game.Status = "begin";
                    Random rndPlayer = new Random();
                    int firstPlayer = rndPlayer.Next(1, 3);
                    if (firstPlayer == 1)
                    {
                        game.StatePl1 = "play";
                        game.StatePl2 = "stop";
                    }
                    else
                    {
                        game.StatePl1 = "stop";
                        game.StatePl2 = "play";
                    }
                    await db.SaveChangesAsync();
                    _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!2nd User connected: " + game);
                    await Clients.All.SendAsync("Receive", 
                        game.Id, 
                        game.Status,
                        game.Player1, 
                        game.StatePl1,
                        game.Player2, 
                        game.StatePl2,
                        game.Step,
                        game.Winner);
                }
                else
                {
                    game = new Game() { Status = statusGame, Player1 = player1, StatePl1 = statePl1 };
                    db.Games.Add(game);
                    await db.SaveChangesAsync();
                    _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!Created Game "+ game);
                }
            }
            
            if (statusGame == "game")
            {
                _logger.LogInformation("?????????????????????????GoGame");
                game = games.LastOrDefault(g => g.Id == Convert.ToInt32(idGame));
                _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!  " + game);
                if (game != null)
                {
                    game.Status = statusGame;
                    await db.SaveChangesAsync();
                }
                _logger.LogInformation("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Сервер отправляет: " + game);
                await Clients.All.SendAsync("Receive", 
                    game.Id, 
                    game.Status,
                    game.Player1, 
                    game.StatePl1,
                    game.Player2, 
                    game.StatePl2,
                    game.Step,
                    game.Winner);
            }
            


        }
    }
}
