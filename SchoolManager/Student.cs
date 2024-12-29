using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace SchoolManager
{
    public class Student
    {
        public int StudentID { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public int StudentPersonNumber { get; set; }
        public int ClassID { get; set; }
        public Class Class { get; set; }

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
        public static void FetchStudents(string connectionString)
        {
            Console.WriteLine("Would you like to see all the students or filter by class? (all/class)");
            string filter = Console.ReadLine();

            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = "SELECT StudentID, StudentFirstName, StudentLastName, StudentPersonNumber, ClassID FROM Students";

                    if (filter?.ToLower() == "class")
                    {
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
                    else if (filter?.ToLower() == "all")
                    {
                        DisplayStudents();
                    }
                    else
                    {
                        Console.WriteLine("Not a valid syntax, try again.");
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
