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
            app.Run();
        }
    }
}
