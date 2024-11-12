using Microsoft.AspNetCore.Mvc;
namespace SERVER
{
    public class Program
    {
        public static Random rnd = new Random();    //new random object

        //record to store the data from the post request
        record Info(string postName, string postLocation, string postItem, string postQuantity, string postPayment);
        public static void Main(string[] args)
        {
            //create a new builder
            var builder = WebApplication.CreateBuilder(args);

            //add controllers
            builder.Services.AddControllers();

            //build the app
            var app = builder.Build();

            //allow cors
            app.UseCors(x=>x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin=> true));

            //map the get request to the root
            app.MapGet("/", () => "ICA 06");

            //map the get request to the /get path and return a welcome message
            app.MapGet("/get", () =>
            {
                return "<h2>Welcome To Tim Hortons<h2>";
            });

            //map the post request to the /locations path and return an array of locations
            app.MapPost("/locations", () =>
            {
                string[] locations = new string[] { "Edmonton", "Calgary", "Red Deer"};

                return locations;
            });

            //map the post request to the /menu path and return an array of menu items
            app.MapPost("/menu", () =>
            {
                string[] menu = new string[] { "Muffins: $2.29", "Croissant: $2.19", "Cookies: $1.49", "Donut: $1.99", "Iced Cap: $2.49", "Coffee: $2.09"};
                

                return menu;
            });

            //map the post request to the /prices path and return an array of prices
            app.MapPost("/prices", () =>
            {
                double[] prices = new double[] {2.29,2.19,1.49,1.99,2.49,2.09};
                return prices;
            });

            //map the post request to the /order path and return the order details
            app.MapPost("/order", (Info data) =>
            {
                //generate a random time taken to prepare the order
                int timeTaken = rnd.Next(5, 31);
                Console.WriteLine(data.postName);

                //create a response object
                var response = (object)new
                {
                    name = data.postName,
                    location = data.postLocation,
                    item = data.postItem,
                    quantity = data.postQuantity,
                    payment = data.postPayment,
                    time = timeTaken

                };
                return response;
            });

            //run the app
            app.Run();
        }
    }
}
