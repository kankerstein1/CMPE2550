using Microsoft.Data.SqlClient;

namespace ASPADODemo01
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // For CORS stuff
            builder.Services.AddControllers();

            var app = builder.Build();
            // Allowing CORS policy so that it can be accessed from anywhere
            app.UseCors(x=>x
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin=>true)
                       );
            
            app.MapGet("/", () => "ASP ADO Demo 01");
            /* Define a connection string that includes information about 
             * your database server name and authentication
             * 
             */
            string connectionString = "Server=data.cnt.sast.ca,24680;" +
                                      "Database=demo_db2550_Northwind;"+
                                      "User Id= demoUser;"+
                                      "Password= temP2020#;"+
                                      "Encrypt=False";

            app.MapGet("/RetrieveData", () => {
                Console.WriteLine("Inside RetrieveData handlers");

                try
                {
                    // Step 1: Establish a connection to DB
                    // Pass connection string here
                    // Use SQLConnection class to open a connection to the db.
                    SqlConnection connection = new SqlConnection(connectionString);

                    // Step 2: Open the connection
                    connection.Open();

                    Console.WriteLine("Connection is open now");

                    // Step 3:  Prepare your query
                    string query = "Select * from Employees where EmployeeId= @empId";

                    // Step 4: Excecute SQL query directly from C# code
                    // SqlCommand class object is required 
                    // need to pass query and connection object
                    SqlCommand command = new SqlCommand(query, connection);

                    // Adding parameter to the command
                    command.Parameters.AddWithValue("@empId", 4);

                    // Step 5: Run your query 
                    // ExecuteReader()- to run your retrieval queries
                    SqlDataReader reader = command.ExecuteReader();
                    
                    //Step 6: Reading data from returned data set

                    // Read()- It will read one record at a time
                    while (reader.Read()) // false if no data or end of data set
                    { // Access data using reader['ColumnName'] or reader.GetXX() methods

                        Console.WriteLine($"{reader["EmployeeID"]}  {reader["LastName"]} {reader["FirstName"]}");    
                        // You have data now, you can return it back to user using JSON object
                    }

                    // TEST IT WITH STORED PROCEDURE

                    /*
                     * Step 1: Create a Stored procedure in the DB
                     * Step 2: Call your stored procedure 
                     */
                    /* Stored Procedure Definintion: Create it in your DB
                        create procedure SelectEmployee( @EmplooyeID int)
                        as
	                        select * from Employees
	                        where EmployeeID= @EmplooyeID
                        go
                     * 
                     */
                    // Close the connection 
                    connection.Close();

                    connection.Open();
                    Console.WriteLine("Testing it with Stored Procedure");
                    // Name of the SP as your query
                    query = "SelectEmployee";

                    command = new SqlCommand(query, connection);

                    // Change the command type to SP
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Make sure argument name should match the name of parameter inside SP
                    command.Parameters.AddWithValue("@EmplooyeID", 5);

                    reader = command.ExecuteReader();

                    while (reader.Read()) // false if no data or end of data set
                    { // Access data using reader['ColumnName'] or reader.GetXX() methods

                        Console.WriteLine($"{reader["EmployeeID"]}  {reader["LastName"]} {reader["FirstName"]}");
                        // You have data now, you can return it back to user using JSON object
                    }


                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while retrieving Data" +e.Message);
                }
            });


            // End point to manage DML operation: Delete
            // BaseURL/DeleteEmployee/5
            // Another alternative to pass values from client side
            app.MapGet("/DeleteEmployee/{id}", (int id) =>
            {
                Console.WriteLine("Inside Delete Employee " + id);

                try
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    // Open connection 
                    connection.Open();

                    //Prepeare your query
                    //string query = "delete from Employees where EmployeeID = "+ id;  // Regular string concat
                    string query = $"delete from Employees where EmployeeID ={id} ";   // String interpolation

                    SqlCommand command = new SqlCommand(query, connection);

                    // Add parameteres if required

                    // Run your query 
                    // ExecuteNonQuery()- for DML operations

                    int rowsAffected = command.ExecuteNonQuery();

                    return $"Number of employees deleted: {rowsAffected}";
                    // Return something
                }
                catch (Exception ex)
                {
                    return "Error" + ex.Message;
                }

            });
            app.Run();
        }
    }
}
