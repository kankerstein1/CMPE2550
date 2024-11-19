using Microsoft.Data.SqlClient;

namespace ICA08
{
    record Info(string studentId);
    public class Program
    {
        //list to hold students
        static List<Student> students = new List<Student>();
        static List<Class> classes = new List<Class>();
        public static void Main(string[] args)
        {
            //new builder
            var builder = WebApplication.CreateBuilder(args);

            //add controllers
            builder.Services.AddControllers();

            //build app
            var app = builder.Build();

            //allow CORS policy
            app.UseCors(x=>x.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin=> true));

            //connection string to database
            string connection = "Server=data.cnt.sast.ca,24680;" +
                                "Database=ClassTrak;" +
                                "User Id=demoUser;" +
                                "Password=temP2020#;" +
                                "Encrypt=False;";

            app.MapGet("/retrieve", () =>
            {
                //establish connection
                try
                {
                    //open connection
                    SqlConnection conn = new SqlConnection(connection);
                    conn.Open();

                    //make a query string
                    string query = "select * from Students where first_name like 'E%' or first_name like 'F%'";

                    //execute query
                    SqlCommand command = new SqlCommand(query, conn);

                    //run query
                    SqlDataReader reader = command.ExecuteReader();

                    //clear list
                    students.Clear();

                    //read data
                    while (reader.Read())
                    {
                        //new student instance
                        Student student = new Student();

                        //assign properties to the values from the database
                        student.StudentId = (int)reader["student_id"];
                        student.FirstName = reader["first_name"].ToString();
                        student.LastName = reader["last_name"].ToString();
                        student.SchoolId = (int)reader["school_id"];

                        //add to students list
                        students.Add(student);

                        //test if query worked
                        Console.WriteLine(reader["first_name"]);
                    }

                    //close reader
                    reader.Close();
                    conn.Close();

                    //create new object to hold data
                    var data = (object)new
                    {
                        studentList = students
                    };

                    //return data object
                    return data;
                }
                catch (Exception ex)
                {
                    //we encountered an error
                    Console.WriteLine("Connect Error " + ex.Message);
                }
                return "";
            });

            //retrieve student by id
            app.MapPost("/retrieveClass", (Info info) =>
            {
                //establish connection
                try
                {
                    //open connection
                    SqlConnection conn = new SqlConnection(connection);
                    conn.Open();

                    Console.WriteLine(info.studentId);
                    //make a query string
                    string query = "select c.class_id, c.class_desc, c.days, c.start_date, c.instructor_id, i.first_name, i.last_name " +
                    "from Classes c " +
                    "join class_to_student cs on c.class_id = cs.class_id " +
                    "join Students s on cs.student_id = s.student_id " +
                    "join Instructors i on i.instructor_id = c.instructor_id " +
                    $"where s.student_id = {info.studentId}";

                    //execute query
                    SqlCommand command = new SqlCommand(query, conn);

                    //run query
                    SqlDataReader reader = command.ExecuteReader();

                    //clear list
                    classes.Clear();

                    //read data
                    while (reader.Read())
                    {
                        //new class instance
                        Class c = new Class();

                        //assign properties to the values from the database
                        c.ClassId = (int)reader["class_id"];
                        c.ClassDesc = reader["class_desc"].ToString();

                        //check if days is null
                        if (!reader.IsDBNull(reader.GetOrdinal("days")))
                        {
                            c.Days = (int)reader["days"];
                        }
                        else
                        {
                            //set days to 0 if null
                            c.Days = 0;
                        }
                        c.StartDate = reader["start_date"].ToString();
                        c.InstructorId = (int)reader["instructor_id"];
                        c.InstructorFirstName = reader["first_name"].ToString();
                        c.InstructorLastName = reader["last_name"].ToString();

                        //add to list
                        classes.Add(c);

                        //error check
                        Console.WriteLine(reader["class_desc"]);
                    }
                    //close reader
                    reader.Close();
                    conn.Close();

                    //create new object to hold data
                    var data = (object)new
                    {
                        classList = classes
                    };

                    //return data object
                    return data;
                }
                catch (Exception ex)
                {
                    //connection issue
                    Console.WriteLine("Connect Error" + ex.Message);
                }
                return "";
            });
            //root path
            app.MapGet("/", () => "ICA 08");

            //run app
            app.Run();
        }
    }
}
