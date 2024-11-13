using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServerStuff
{
    public class Program
    {
        record Data(string action, string player1, string player2, int previous, int current);
        static int turnCount = 0;
        static Random rnd = new Random();
        static bool playerTurn = false;
        static int previousPos = 32;
        static int currentPos = 32;
        static int p1Move = 0;
        static int p2Move = 0;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //add controllers
            builder.Services.AddControllers();

            var app = builder.Build();

            //allow cors
            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true));

            app.MapGet("/", () => "ICA 07!");

            app.MapPost("/game", (Data response)=>{
                //sanitize input
                string cleanAction = CleanInputs(response.action);
                string cleanP1 = CleanInputs(response.player1);
                string cleanP2 = CleanInputs(response.player2);

                if (cleanAction == "NewGame")
                {
                    turnCount = 0;
                    playerTurn = !playerTurn;
                    var data = (object)new
                    {
                        status = $"Welcome {cleanP1} and {cleanP2}",
                        p1 = cleanP1,
                        p2 = cleanP2,
                        turn = playerTurn,
                    };
                    return data;

                }
                if(cleanAction == "QuitGame")
                {
                    turnCount = 0;
                    playerTurn = false;
                    previousPos = 32;
                    currentPos = 32;
                    p1Move = 0;
                    p2Move = 0;
                    //create a response object
                    var data = (object)new
                    {
                        status = "Quit Game"
                    };
                    return data;
                }
                if(cleanAction == "MakeMove")
                {
                    previousPos = response.current;
                    
                    turnCount++;
                    p1Move = rnd.Next(1, 7);
                    p2Move = rnd.Next(1, 7);
                    int displacement = p1Move - p2Move;
                    currentPos = previousPos  + displacement;
                    playerTurn = !playerTurn;

                    var data = (object)new
                    {
                        status = $"Player 1 rolled {p1Move}, Player 2 rolled {p2Move}",
                        p1 = $"{cleanP1}",
                        p2 = $"{cleanP2}",
                        turn = playerTurn,
                        previous = previousPos,
                        current = currentPos,
                        turnsTaken = turnCount,
                        displacement = displacement
                    };
                    return data;
                }
                return "";
            });
            app.Run();
        }
        public static string CleanInputs(string input)
        {
            string clean;

            //clean here
            clean = Regex.Replace(input.Trim(), "<.*?|&;$>", string.Empty);
            return clean;
        }
    }
}
