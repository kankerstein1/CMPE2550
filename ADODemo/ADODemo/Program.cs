using Microsoft.Data.SqlClient;

namespace ADODemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //for CORS
            builder.Services.AddControllers();

            var app = builder.Build();

            //allow CORS policy so that it can be accessed from anywhere
            app.UseCors(x => x.AllowAnyHeader()
                              .AllowAnyMethod()
                              .SetIsOriginAllowed(origin => true)
                              );

            app.MapGet("/", () => "Ado Demo!");
            /*Define connection string that includes information about 
             * your database servername and authentication
             */

            string connectionString = "Server=data.cnt.sast.ca,24680;" +
                                       "Database=demo_db2550_Northwind;" +
                                       "User Id= demoUser;" +
                                       "Password= temP2020#;" +
                                       "Encrypt=False";

            app.MapGet("/RetriveData", () =>
            {
                Console.WriteLine("Inside RetriveData");

                //Step 1: establish db connection
                //pass connection string here
                //use SQLConnection class to open connection to db
                try 
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    //Step 2: open connection
                    connection.Open();
                    Console.WriteLine("Connection is open");

                    //Step 3: write query
                    string query = "select * from Employees where EmployeeID = @empId";

                    //step 4: Execute query
                    //SQLCommand Class Object is required
                    //need to pass query and connection object
                    SqlCommand command = new SqlCommand(query,connection);

                    //adding parameters to the command
                    command.Parameters.AddWithValue("@empId", 4);

                    //step 5: Run query
                    //ExecuteReader() - to run retrieval queries
                    SqlDataReader reader = command.ExecuteReader();

                    //step 6: reading data from returned data set

                    //read() reads one record at a time
                    while(reader.Read()) //false if no data or end of data set
                    {
                        //Access the data using reader['column name'] or reader.GetXX() methods
                        Console.WriteLine($"Id: {reader["EmployeeID"]} Fn: {reader["FirstName"]}");

                        //return data object using json object
                    }
                    reader.Close();
                    //TEST IT WITH STORED PROCEDURES
                    Console.WriteLine("Stored Procedure Time");
                    /*
                     * Step1: Create a Stored Procedure in the DB
                     * Step 2: Call Stored Procedure
                     * Stored Procedure Defintion created in Database
                     * --create procedure SelectEmployee(@EmployeeID int)
                        --As
                        --select * from Employees where EmployeeID = @EmployeeID
                        --go
                     */
                    //name of sp as query
                    query = "SelectEmployee";

                    command = new SqlCommand(query, connection);

                    //change command type
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    //make sure arguement name should match name of parameter inside sql
                    command.Parameters.AddWithValue("@EmplooyeID", 5);

                    reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        //Access the data using reader['column name'] or reader.GetXX() methods
                        Console.WriteLine($"Id: {reader["EmployeeID"]} Fn: {reader["FirstName"]}");
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect Error" + ex.Message);
                }
                
            });
            app.Run();
        }
    }
}
