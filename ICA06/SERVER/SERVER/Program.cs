namespace SERVER
{
    public class Program
    {
        record Info(string postName, string postLocation, string postItem, int postQuantity, string postPayment);
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            var app = builder.Build();

            ;
            app.UseCors(x=>x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin=> true));
            app.MapGet("/", () => "ICA 06");
                
            app.MapGet("/get", () =>
            {
                return "<h2>Welcome To Tim Hortons<h2>";
            });

            app.MapPost("/locations", () =>
            {
                string[] locations = new string[] { "Edmonton", "Calgary", "Red Deer"};

                return locations;
            });

            app.MapPost("/menu", () =>
            {
                string[] menu = new string[] { "Muffins: $2.29", "Croissant: $2.19", "Cookies: $1.49", "Donut: $1.99", "Iced Cap: $2.49", "Coffee: $2.09"};
                

                return menu;
            });

            app.MapPost("/prices", () =>
            {
                double[] prices = new double[] {2.29,2.19,1.49,1.99,2.49,2.09};
                return prices;
            });

            app.MapPost("/order", (Info data) =>
            {
                Console.WriteLine(data.postName);
                var response = (object)new
                {
                    name = data.postName,
                    location = data.postLocation,
                    item = data.postItem,
                    quantity = data.postQuantity,
                    payment = data.postPayment
                };
                return response;
            });
            app.Run();
        }
    }
}
