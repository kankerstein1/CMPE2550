using ICA_10.Models;
using System.Security.Cryptography;
using System.Text.RegularExpressions;


namespace ICA_10
{
    record Info(string locationId, string customerId);
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

            //root path
            app.MapGet("/", () =>
            {
                Console.WriteLine("Here");
                //get db context
                var db = new Kankerstein1RestaurantDbContext();

                //get list of locartion name and id
                var locations = from l in db.Locations
                                select new
                                {
                                    l.LocationName,
                                    l.Locationid
                                };
                
                return locations.ToList();
            });

            //get all customers at a location
            app.MapGet("/customer/{locationId}/{customerId}", (string locationId, string customerId) =>
                {
                    try
                    {
                        Console.WriteLine(locationId);

                        //get db context
                        var db = new Kankerstein1RestaurantDbContext();

                        //get customer name
                        var customer = from l in db.Customers
                                       where l.Cid == int.Parse(customerId)
                                       let person = $"{l.Fname} {l.Lname}"
                                       select person;

                        //get location name
                        var location = from l in db.Locations
                                       where l.Locationid == int.Parse(locationId)
                                       select l.LocationName;
                        Console.WriteLine(location.FirstOrDefault());

                        //if no customer found
                        if (customer.Count() == 0)
                        {
                            return (object)new
                            {
                                response = "No customer found"
                            };
                        }
                        else
                        {

                            //Select o.OrderId,o.OrderDate,o.PaymentMethod,i.ItemName,i.ItemPrice, o.ItemCount
                            //from Customers c
                            //join Orders o on c.Cid = o.Cid
                            //join Items i on o.Itemid = i.Itemid
                            //where c.Cid = 117

                            //get table data for order id order date payment method item name item price and count for customer at location
                            var tableData = from c in db.Customers
                                            join o in db.Orders on c.Cid equals o.Cid
                                            join i in db.Items on o.Itemid equals i.Itemid
                                            where c.Cid == int.Parse(customerId) && o.Locationid == int.Parse(locationId)
                                            select new
                                            {
                                                o.OrderId,
                                                o.OrderDate,
                                                o.PaymentMethod,
                                                i.ItemName,
                                                i.ItemPrice,
                                                o.ItemCount
                                            };

                            //return response
                            var responseData = (object)new
                            {
                                response = $"Orders Placed by {customer.FirstOrDefault()} at location {location.FirstOrDefault()}",
                                tableData = tableData.ToList()
                            };
                            return responseData;
                        }
                    }
                    catch (Exception e)
                    {
                        //return error message
                        Console.WriteLine(e.Message);
                        var responseData = (object)new
                        {
                            response = $"{e.Message}"
                        };
                        return responseData;
                        
                    }
                    
                });

            app.Run();
        }
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
