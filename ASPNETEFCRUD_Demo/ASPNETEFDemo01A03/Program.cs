using ASPNETEFDemo01A03.Models;

namespace ASPNETEFDemo01A03
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            // Make sure to include CORS stuff while testing it from client side

            app.MapGet("/", () => "Hello World!");

            // End point to test EF retrieval part
            app.MapGet("/RetData", () =>
            {
                var db = new DemoDb2550NorthwindContext();

                // Query style of LINQ queries 
                // Part 1
                // UNCOMMENT THE FOLLOWING TO TEST IT WITH QUERY STYLE
                // select * from products p
                /*
                var results = from p in db.Products
                              join c in db.Categories
                                on p.CategoryId equals c.CategoryId
                              orderby p.ProductName // descending- if required
                              select new { p.ProductId, p.ProductName, c.CategoryId, c.CategoryName };

                return results.ToList();
                */

                // PART 2
                // UNCOMMENT THE FOLLOWING TO TEST IT WITH METHOD STYLE
                // METHOD STYLE
               
                var results = db.Products      //targeting specific columns
                                .Where(x=> x.CategoryId == x.Category.CategoryId)// Joining condition
                                .OrderBy(x => x.ProductName)  // Ascending 
                                //.OrderByDescending(x=> x.ProductName)
                                .ThenBy(x=> x.ProductId)
                                .Select(x => new { x.ProductId, x.ProductName, x.Category.CategoryId,x.Category.CategoryName });

                return results.ToList();
                
            });

            //for inserting cATEGORY INTO db
            //crud        ASP             SQL
            //Create C  - Post            Insert
            //Read   R  - Get             Select
            //Update U  - Put             Update
            //Delete D  - Delete          Delete

            //testing with map get because no client side
            app.MapGet("/InsertCategory", () =>
            {
                Console.WriteLine("inside insert");

                //Add new data to DB
                //Create data object of class/table
                Category c = new Category();

                //hard coding values here
                //you can get values from client
                c.CategoryName = "Test Category";
                c.Description = "Test Description";

                //insert the object to DB
                try
                {
                    var db = new DemoDb2550NorthwindContext();

                    db.Add(c); //like insert query
                    db.SaveChanges(); //making the changes permanent in DB
                    Console.WriteLine("Insert Successful");
                    return "Inserted Succesfully";
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error" + ex.Message);
                    return "Error" + ex.Message; //HTML data
                }
            });

            app.MapGet("/delete/{cid}", (string cid)=>
            {
                Console.WriteLine($"Inside delete endpoint: {cid} ");

                using (var db = new DemoDb2550NorthwindContext())
                {
                    //using block destroys resources once we are out of using block
                    try
                    {
                        bool valid = int.TryParse(cid,out int id);
                        //try to delete from DB here
                        if(db.Categories.Find(id) is Category c)
                        {
                            //inside is a match has been found
                            //delete here

                            
                            db.Categories.Remove(c);

                            //finalize delete
                            db.SaveChanges();

                            Console.WriteLine("delete worked");
                            return new
                            {
                                status = $"Successfully Delete {cid}"
                            };
                        }
                        else
                        {
                            //not found
                            return new
                            {
                                status = "Not found"
                            };
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return new
                        {
                            status = "Error" + e.Message
                        };
                    }
                }
                
            });

            app.MapGet("/update/{cid}", (string cid) =>
            {
                Console.WriteLine("inside update");
                using (var db = new DemoDb2550NorthwindContext())
                {
                    try
                    {
                        if(db.Categories.Find(int.Parse(cid)) is Category c)
                        {
                            c.CategoryName = "New name";
                            c.Description = "new desc";

                            db.Categories.Update(c);
                            db.SaveChanges();
                            Console.WriteLine("updated");
                            return (object)new
                            {
                                status = $"Successfully updated {cid}"
                            };
                        }
                        else
                        {
                            return (object)new
                            {
                                status = "Not found"
                            };
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return new
                        {
                            status = "Error" + e.Message
                        };
                        
                    }
                }
            });
            app.Run();
        }
    }
}
