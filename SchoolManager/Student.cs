using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace SchoolManager
{
    public class Student
    {
        public int StudentID { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentPersonNumber { get; set; }
        public int ClassID { get; set; }
        public Class Class { get; set; }

        public static void FetchStudents(string connectionString)
        {
            Console.WriteLine("1. View all students");
            Console.WriteLine("2. View student in a class");
            Console.WriteLine("3. View student by ID"); //Stored procedure
            string choice = Console.ReadLine();
            Console.Clear();

            switch (choice)
            {
                case "1":

                    DisplayStudents();
                    break;

                case "2":

                    DisplayStudentsInClass(connectionString);
                    break;

                case "3":
                    StoredProcedure(connectionString);
                    break;

            }

        }

        public static void DisplayStudents()
        {
            using (var context = new SchoolContext())
            {
                var students = context.Students
                    .Include(s => s.Class) // Include class details if needed
                    .ToList();

                Console.WriteLine("StudentID | First Name | Last Name  | Person Number | Class Name");
                Console.WriteLine(new string('-', 60));

                foreach (var student in students)
                {
                    Console.WriteLine($"{student.StudentID,-10} | {student.StudentFirstName,-10} | {student.StudentLastName,-10} | {student.StudentPersonNumber,-14} | {(student.Class?.ClassName ?? "N/A"),-10}");
                }

            }
        }

        // Displays students in a specific class
        public static void DisplayStudentsInClass(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = "SELECT StudentID, StudentFirstName, StudentLastName, StudentPersonNumber, ClassID FROM Students";

                    // Show all available ClassIDs
                    using (SqlCommand command = new SqlCommand("SELECT DISTINCT ClassID FROM Classes", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("Available Classes:");
                            while (reader.Read())
                            {
                                Console.WriteLine($"ClassID: {reader["ClassID"]}");
                            }
                        }
                    }

                    // Prompt the user to enter a ClassID
                    Console.WriteLine("Enter a ClassID:");
                    int classId;
                    while (!int.TryParse(Console.ReadLine(), out classId))
                    {
                        Console.WriteLine("Invalid input. Please enter a numeric ClassID:");
                    }

                    query += " WHERE ClassID = @ClassID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ClassID", classId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("Students:");
                            while (reader.Read())
                            {
                                Console.WriteLine($"ID: {reader["StudentID"]}, NAME: {reader["StudentFirstName"]} {reader["StudentLastName"]}, PERSONAL NUMBER: {reader["StudentPersonNumber"]}, CLASSID: {reader["ClassID"]}");
                            }
                        }
                    }

                }

                else
                {
                    Console.WriteLine("Failed to connect to the database.");
                }
            }
        }

        // Executes a stored procedure
        public static void StoredProcedure(string connectionString)
        {
            Console.WriteLine("Enter the Student ID to search:");
            int studentId;
            while (!int.TryParse(Console.ReadLine(), out studentId))
            {
                Console.WriteLine("Invalid input. Please enter a numeric Student ID:");
            }

            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    try
                    {
                        using (SqlCommand command = new SqlCommand("GetStudentInfo", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@StudentID", studentId);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Console.WriteLine("Student Information:");
                                    Console.WriteLine($"ID: {reader["StudentID"]}");
                                    Console.WriteLine($"First Name: {reader["StudentFirstName"]}");
                                    Console.WriteLine($"Last Name: {reader["StudentLastName"]}");
                                    Console.WriteLine($"Person Number: {reader["StudentPersonNumber"]}");
                                    Console.WriteLine($"Class ID: {reader["ClassID"]}");
                                    Console.WriteLine($"Class Name: {reader["ClassName"]}");
                                }
                                else
                                {
                                    Console.WriteLine("No student found with the provided ID.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to the database.");
                }
            }
        }


    }
}
