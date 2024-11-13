using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServerStuff
{
    public class Program
    {
        //record to transmit data
        record Data(string action, string player1, string player2, int previous, int current);

        //public member for turncount
        static int turnCount = 0;

        //new random value
        static Random rnd = new Random();

        //bool for player turn
        static bool playerTurn = false;

        //values to store previous and current position
        static int previousPos = 32;
        static int currentPos = 32;

        //values to store how far each player moves
        static int p1Move = 0;
        static int p2Move = 0;
        public static void Main(string[] args)
        {
            //create builder
            var builder = WebApplication.CreateBuilder(args);

            //add controllers
            builder.Services.AddControllers();

            //build
            var app = builder.Build();

            //allow cors
            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true));

            //root page
            app.MapGet("/", () => "ICA 07!");

            //page where all game content will be going on
            app.MapPost("/game", (Data response)=>{
                //sanitize input
                string cleanAction = CleanInputs(response.action);
                string cleanP1 = CleanInputs(response.player1);
                string cleanP2 = CleanInputs(response.player2);

                //if it's new game we want to initialize a new game
                if (cleanAction == "NewGame")
                {
                    //reset turn count
                    turnCount = 0;

                    //change turns
                    playerTurn = !playerTurn;

                    //create data object to send back
                    var data = (object)new
                    {
                        status = $"Welcome {cleanP1} and {cleanP2}",
                        p1 = cleanP1,
                        p2 = cleanP2,
                        turn = playerTurn,
                    };
                    //return the object
                    return data;

                }
                //qutting game we want to reset values
                if(cleanAction == "QuitGame")
                {
                    //resetting server variables
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

                    //return data
                    return data;
                }
                //action to move each turn
                if(cleanAction == "MakeMove")
                {
                    //storing previous turn
                    previousPos = response.current;
                    
                    //increment turn count
                    turnCount++;

                    //have each player "pull" strength be determined randomly 1-7
                    p1Move = rnd.Next(1, 7);
                    p2Move = rnd.Next(1, 7);

                    //displacement is how much it will move
                    int displacement = p1Move - p2Move;

                    //update postion based on how much it moves
                    currentPos = previousPos  + displacement;

                    //change turns
                    playerTurn = !playerTurn;

                    //build data object
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
                    //send it back
                    return data;
                }
                //send back nothing if we reach here. Bad result
                return "";
            });
            //run app
            app.Run();
        }
        //function goal is to sanitize inputs for clean and safe data
        //Params: string input: the string to santize
        //Returns: a clean and santized string :)
        public static string CleanInputs(string input)
        {
            //string to store cleaned input
            string clean;

            //trim and remove and special characters
            clean = Regex.Replace(input.Trim(), "<.*?|&;$>", string.Empty);

            //return it
            return clean;
        }
    }
}
