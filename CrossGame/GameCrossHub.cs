using CrossGame.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CrossGame
{
    public class GameCrossHub : Hub
    {
        private readonly ApplicationContext db;
        public GameCrossHub(ApplicationContext db)
        {
            this.db = db;
        }
        public async Task Start(string userName)
        {
            var isWaitGame = db.Games.Any(g => g.Status == "wait");
            if (isWaitGame)
            {
                var waitGame = db.Games.First(g => g.Status == "wait");
                waitGame.Player2 = userName;
                db.Update(waitGame);
                db.SaveChanges();
                await Clients.All.SendAsync("Wait", waitGame.Id, waitGame.Player1, waitGame.Player2);
            }
            else
            {
                Game newGame = new() { Player1 = userName, Status = "wait" };
                db.Games.Add(newGame);
                db.SaveChanges();
                await Clients.All.SendAsync("Wait", newGame.Id, newGame.Player1, newGame.Player2);
            }
        }
        public async Task PingPlayers(string idGame)
        {
            Console.WriteLine("??????????????????????????");
            await Clients.All.SendAsync("ConnectionTest", idGame);
        }
        public async Task ResponsePing(string idGame, string userName)
        {
            var currentGame = db.Games.First(g => g.Id == Convert.ToInt32(idGame));
            if(currentGame.Player1 == userName)
                currentGame.StatePl1 = "OK";
            if (currentGame.Player2 == userName)
            {
                currentGame.StatePl2 = "OK";
                //await Clients.All.SendAsync("DoYouReady", currentGame.Player1);
            }

            if(currentGame.StatePl1=="OK" && currentGame.StatePl2=="OK")
            {
                string goPlayer;
                var numberPlayerForStep1 = new Random().Next(0, 2);
                if (numberPlayerForStep1 == 1)
                    goPlayer = currentGame.Player1;
                else
                    goPlayer = currentGame.Player2;
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!! step1 for :" + goPlayer + "  " + numberPlayerForStep1);
                currentGame.Status = "go";
                await Clients.All.SendAsync("GoGame", idGame, goPlayer);
            }
            db.Games.Update(currentGame);
            db.SaveChanges();
        }
    }
}
