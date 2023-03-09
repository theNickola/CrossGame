using CrossGame.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
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
            var isWaitGame = db.Games.Any(g => g.Status == "wait" && g.Player2=="");
            if (isWaitGame)
            {
                var waitGame = db.Games.First(g => g.Status == "wait" && g.Player2 == "");
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
        public async Task PingPlayer1(string idGame, string player1)
        {
            await Clients.All.SendAsync("ConnectionTest", idGame, player1);
        }
        public async Task GoGame(string idGame)
        {
            var currentGame = db.Games.First(g => g.Id.ToString() == idGame);
            string goPlayer;
            var numberPlayerForStep1 = new Random().Next(1, 3);
            if (numberPlayerForStep1 == 1)
                goPlayer = currentGame.Player1;
            else
                goPlayer = currentGame.Player2;
            currentGame.Status = "go";
            currentGame.Mover = goPlayer;
            db.Games.Update(currentGame);
            db.SaveChanges();
            await Clients.All.SendAsync("GoStep", idGame, goPlayer, "");
        }
        public async Task StepPlayer(string idGame, string namePlayer, string step)
        {
            var currentGame = db.Games.First(g => g.Id.ToString() == idGame);
            if (currentGame.Mover == namePlayer)
            {
                if (currentGame.Player1 == namePlayer)
                {
                    currentGame.StepsPl1 += step + ",";
                    if(isWin(currentGame.StepsPl1))
                    {
                        await Clients.All.SendAsync("End", idGame, currentGame.Player1, step);
                        return;
                    }
                    if (isEnd(currentGame.StepsPl1, currentGame.StepsPl2))
                    {
                        await Clients.All.SendAsync("End", idGame, "", step);
                        return;
                    }
                    currentGame.Mover = currentGame.Player2;
                }
                else
                {
                    currentGame.StepsPl2 += step + ",";
                    if (isWin(currentGame.StepsPl2))
                    {
                        await Clients.All.SendAsync("End", idGame, currentGame.Player2, step);
                        return;
                    }
                    if (isEnd(currentGame.StepsPl1, currentGame.StepsPl2))
                    {
                        await Clients.All.SendAsync("End", idGame, "", step);
                        return;
                    }
                    currentGame.Mover = currentGame.Player1;
                }
            }
            db.Update(currentGame);
            db.SaveChanges();
            await Clients.All.SendAsync("GoStep", idGame, currentGame.Mover, step);
        }

        private bool isEnd(string stepsPl1, string stepsPl2)
        {
            int steps1 = stepsPl1.Split(',').Length - 1;
            int steps2 = stepsPl2.Split(',').Length - 1;
            if (steps1 + steps2 == 9)
                return true;
            return false;
        }

        private bool isWin(string AllSteps)
        {
            AllSteps = AllSteps.Remove(AllSteps.Length - 1);
            string[] steps = AllSteps.Split(',');

            char[,] gameField = new char[3,3];

            for (int i = 0; i < steps.Length; i++)
            {
                int x1 = Convert.ToInt32(steps[i][0])-48;
                int x2 = Convert.ToInt32(steps[i][1]-48);
                gameField[x1, x2] = 'X';
            }

            for (int i = 0; i < 3; i++)
            {
                int isRow = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (gameField[i, j] == 'X')
                        isRow++;
                }
                if (isRow == 3)
                {
                    return true;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                int isColumn = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (gameField[j, i] == 'X')
                        isColumn++;
                }
                if (isColumn == 3)
                {
                    return true;
                }
            }

            if (gameField[0, 0] == gameField[1, 1] && gameField[0, 0] == gameField[2, 2] && gameField[0, 0] == 'X')
            {
                return true;
            }

            if (gameField[0, 2] == gameField[1, 1] && gameField[1, 1] == gameField[2, 0] && gameField[1, 1] == 'X')
            {
                return true;
            }

            return false;
        }
    }
}
