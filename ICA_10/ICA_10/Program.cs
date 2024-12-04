using ICA_10.Models;

namespace ICA_10
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //new builder
            var builder = WebApplication.CreateBuilder(args);

            //add controllers
            builder.Services.AddControllers();

            //build app
            var app = builder.Build();

            //allow CORS policy
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true));

            app.MapGet("/", () =>
            {
                Console.WriteLine("Here");
                var db = new Kankerstein1RestaurantDbContext();

                var locations = from l in db.Locations
                                select new
                                {
                                    l.LocationName
                                };
                var locationIds = from l in db.Locations
                                select new
                                {
                                   l.Locationid
                                };
                Console.WriteLine(locations);
                var response= (object)new
                {
                    locations = locations.ToList(),
                    ids = locationIds.ToList()
                };
                return response;
            });

            app.MapGet("/customer", () =>
                {
                    Console.WriteLine("Here");
                    var db = new Kankerstein1RestaurantDbContext();

                    var locations = from l in db.Locations
                                    select new
                                    {
                                        l.LocationName
                                    };
                    Console.WriteLine(locations);
                    return locations.ToList();

                });

            app.Run();
        }
    }
}
