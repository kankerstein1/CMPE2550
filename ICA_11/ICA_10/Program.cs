using ICA_10.Models;
using System;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ICA_10
{
    record Info(string locationId, string customerId);

    record Add(string customerId, string itemOrdered, string quantity, string paymentMethod, string location);
    record UpdateOrder(string orderId,string customerId, string itemOrdered, string quantity, string paymentMethod, string location);
    public class Program

    {
        public static int _customerId = 0;
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
                        _customerId = int.Parse(customerId);
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

            app.MapGet("/items", () =>
            {

                try
                {
                    //new db context
                    var db = new Kankerstein1RestaurantDbContext();

                    //get list of items from Items table
                    var items = from i in db.Items
                                select i;
                    return items.ToList();

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

            app.MapDelete("/delete/{orderId}/{locationsId}", (string orderId, string locationsId) =>
            {
                try
                {
                    //clean inputs
                    string cleanId = CleanInputs(orderId);
                    string cleanLocation = CleanInputs(locationsId);

                    //check for valid ints
                    bool goodparse = int.TryParse(cleanId, out int validId);
                    bool goodLocation = int.TryParse(cleanLocation, out int validLocation);

                    //send error message if invalid
                    if (!goodparse || !goodLocation)
                    {
                        var responseData = (object)new
                        {
                            response = $"Invalid Order Id or Location Id"
                        };
                        return responseData;
                    }
                    else
                    {
                        //use db context
                        using (var db = new Kankerstein1RestaurantDbContext())
                        {
                            Console.WriteLine(validId);
                            Console.WriteLine(validLocation);
                            //check for a valid order with the location and id
                            if (db.Orders.Find(validLocation, validId) is Order o)
                            {
                                //remove the order
                                db.Orders.Remove(o);

                                //save it
                                db.SaveChanges();


                                //populate updated table data
                                var tableData = from c in db.Customers
                                                join or in db.Orders on c.Cid equals or.Cid
                                                join i in db.Items on or.Itemid equals i.Itemid
                                                where c.Cid == _customerId && or.Locationid == validLocation
                                                select new
                                                {
                                                    or.OrderId,
                                                    or.OrderDate,
                                                    or.PaymentMethod,
                                                    i.ItemName,
                                                    i.ItemPrice,
                                                    or.ItemCount
                                                };

                                //return new table
                                return (object)new
                                {
                                    response = "Order Succesfully Deleted",
                                    table = tableData.ToList()
                                };
                            }

                            else
                            {
                                return (object)new
                                {
                                    response = "Customer Not Found"
                                };
                            }
                        }

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

            app.MapPost("/add", (Add add) =>
            {
                try
                {
                    //santize inputs
                    int cleanCustomerId = int.Parse(CleanInputs(add.customerId));
                    int cleanItem = int.Parse(CleanInputs(add.itemOrdered));
                    int cleanQuantity = int.Parse(CleanInputs(add.quantity));
                    string cleanPayment = CleanInputs(add.paymentMethod);
                    int cleanLocation = int.Parse(CleanInputs(add.location));

                    //new db context
                    var db = new Kankerstein1RestaurantDbContext();

                    //if we find a customer populate a new order
                    if (db.Customers.Find(cleanCustomerId) is Customer C)
                    {
                        Order o = new Order();
                        o.Cid = cleanCustomerId;
                        o.Itemid = cleanItem;
                        o.ItemCount = cleanQuantity;
                        o.PaymentMethod = cleanPayment;
                        o.Locationid = cleanLocation;

                        //add to db
                        db.Add(o);

                        //save it
                        db.SaveChanges();

                        //new random 
                        Random rnd = new Random();

                        //return response with random time to simulate order time
                        return (object)new
                        {
                            response = $"Order Received. Your Order will be ready in {rnd.Next(5, 31)} minutes. You can edit it if you want"
                        };
                    }
                    else
                    {
                        return (object)new { response = "Customer Not Found" };
                    }
                }
                catch (Exception e)
                {
                    return (object)new
                    {
                        response = $"{e.Message}"
                    };
                }

            });

            app.MapPost("/updateForm", (Add update) =>
            {
                try
                {
                    //santize data
                    int cleanCustomerId = int.Parse(CleanInputs(update.customerId));
                    int cleanItem = int.Parse(CleanInputs(update.itemOrdered));
                    int cleanQuantity = int.Parse(CleanInputs(update.quantity));
                    string cleanPayment = CleanInputs(update.paymentMethod);
                    int cleanLocation = int.Parse(CleanInputs(update.location));

                    //new db context
                    var db = new Kankerstein1RestaurantDbContext();

                    //get list of order id
                    var orders = from o in db.Orders
                                 select o.OrderId;
                    var orderList = orders.ToList();

                    //max value ie the latest order id in the order table which is the order we just added
                    Console.WriteLine(orderList.Max());
                    //populate form with the order
                    string form = "<Form>";
                    form += "<label for='customerId'>Order Id:</label>";
                    form += $"<input type='text' id='orderId' name='orderId' value='{orderList.Max()}' disabled>";
                    form += "<label for='customerId'>Customer Id:</label>";
                    form += $"<input type='text' id='customerId' name='customerId' value='{cleanCustomerId}'disabled>";
                    form += "<label for='itemOrdered'>Item Ordered:</label>";
                    form += "<select id='itemOrdered' name='itemOrdered'>";
                    form += "<option value='default'>Select An Item</option>";

                    //build select of items with the items table
                    var items = from i in db.Items
                                select i;
                    foreach (var i in items.ToList())
                    {
                        form += $"<option value='{i.Itemid}'>{i.ItemName}</option>";
                    }

                    form += "</select>";
                    form += "<label for='quantity'>How Many:</label>";
                    form += "<input type='text' id='quantity' name='quantity' placeholder='0'>";
                    form += "<label for='paymentMethod'>Payment Method:</label>";
                    form += "<select id='paymentMethod' name='paymentMethod'>";
                    form += "<option value='default'>Select A Payment Method</option>";
                    form += "<option value='Cash'>Cash</option>";
                    form += "<option value='Credit'>Credit</option>";
                    form += "<option value='Debit'>Debit</option>";
                    form += "</select>";
                    form += "<label for='location'>Location:</label>";
                    form += "<select id='locationAdd' name='location' disabled>";

                    //build a list of locations with the location id
                    var locations = from l in db.Locations
                                    where l.Locationid == cleanLocation
                                    select l;
                    foreach (var l in locations.ToList())
                    {
                        form += $"<option value='{l.Locationid}'>{l.LocationName}</option>";
                    }

                    form += "</select>";
                    form += "<button type='button' id='update'>Update Order</button>";
                    form += "</form>";

                    //send new form back
                    return (object)new
                    {
                        response = "update",
                        updateForm = form
                    };

                }
                catch (Exception e)
                {
                    return (object)new
                    {
                        response = $"{e.Message}"
                    };
                }
            });

            app.MapPut("/updateForm", (UpdateOrder up) =>
            {
                try
                {
                    //santize data
                    int cleanOrder = int.Parse(CleanInputs(up.orderId));
                    int cleanCustomer = int.Parse(CleanInputs(up.customerId));
                    int cleanItem = int.Parse(CleanInputs(up.itemOrdered));
                    int cleanQuantity = int.Parse(CleanInputs(up.quantity));
                    string cleanPayment = CleanInputs(up.paymentMethod);
                    int cleanLocation = int.Parse(CleanInputs(up.location));

                    //new db context
                    var db = new Kankerstein1RestaurantDbContext();

                    //check for valid order and unbox
                    if(db.Orders.Find(cleanLocation,cleanOrder) is Order o)
                    {
                        //populate Order object with updated data
                        o.Itemid = cleanItem;
                        o.ItemCount = cleanQuantity;
                        o.PaymentMethod = cleanPayment;
                        
                        Console.WriteLine(cleanItem);
                        Console.WriteLine(cleanQuantity);
                        Console.WriteLine(cleanPayment);

                        //update db and save changes
                        db.Update(o);
                        db.SaveChanges();

                        return (object)new
                        {
                            response = "Order Updated. Now you are unable to make any changes"
                        };
                    }
                    else
                    {
                        return (object)new
                        {
                            response = $"Order Not Found"
                        };
                    }
                }
                catch (Exception e)
                {
                    return (object)new
                    {
                        response = $"{e.Message}"
                    };
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
