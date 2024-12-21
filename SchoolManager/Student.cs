using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManager
{
    internal class Student
    {
        public int StudentID { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public int StudentPersonNumber { get; set; }
        public int ClassID { get; set; }

        public static void FetchStudents(string connectionString)
        {
            Console.WriteLine("Would you like to see all the students? filter by role? (all/role)");
            string filter = Console.ReadLine();

            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = "SELECT StudentID, StudentFirstName, StudentLastName, StudentPersonNumber, ClassID FROM Students";

                    if (filter?.ToLower() == "role")
                    {
                        using (SqlCommand command = new SqlCommand("SELECT DISTINCT EmployeeRole FROM Employees", connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                Console.WriteLine("Here are all the roles: ");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"{reader["EmployeeRole"]}");
                                }
                            }
                        }

                        Console.WriteLine("Enter a role");
                        string role = Console.ReadLine();
                        query += " WHERE EmployeeRole = @Role";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Role", role);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                Console.WriteLine("Students:");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"ID: {reader["EmployeeID"]}, NAME: {reader["EmployeeFirstName"]}, {reader["EmployeeLastName"]}, PERSONAL NUMBER: {reader["StudentPersonNumber"]}, CLASSID: {reader["ClassID"]}, STARTDATE: {reader["EmployeeStart"]}");
                                }
                            }
                        }
                    }
                    else if (filter?.ToLower() == "all")
                    {
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
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
