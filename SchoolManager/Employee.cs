using Microsoft.Data.SqlClient;
using System;

namespace SchoolManager
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeRole { get; set; }
        public int Salary { get; set; }
        public DateTime startDate { get; set; }

        public static void FetchTeacherDepartments()
        {
            using (var context = new SchoolContext())
            {
                var departmentCounts = context.Employees
                    .GroupBy(e => e.EmployeeRole)
                    .Select(group => new
                    {
                        Department = group.Key,
                        EmployeeCount = group.Count()
                    })
                    .ToList();

                Console.WriteLine("Department | Employee Count");
                Console.WriteLine(new string('-', 30));

                foreach (var department in departmentCounts)
                {
                    Console.WriteLine($"{department.Department} | {department.EmployeeCount}");
                }
            }
        }
        public static void FetchEmployees(string connectionString)
        {
            Console.WriteLine("Do you want to VIEW/EDIT employees or (check how many employees in each department? EF)");
            string filter = Console.ReadLine();

            if (filter.ToLower() == "view")
            {
                Console.Clear();
                Console.WriteLine("Would you like to view ALL employees or ROLE?");
                filter = Console.ReadLine();

                using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
                {
                    if (connection != null)
                    {
                        string query = "SELECT EmployeeID, EmployeeRole, EmployeeFirstName, EmployeeLastName, Salary, EmployeeStart FROM Employees";

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
                                    Console.WriteLine("Employees:");
                                    while (reader.Read())
                                    {
                                        DateTime employeeStart = reader.GetDateTime(reader.GetOrdinal("EmployeeStart"));
                                        TimeSpan timeEmployed = DateTime.Now - employeeStart;
                                        Console.WriteLine($"ID: {reader["EmployeeID"]}, NAME: {reader["EmployeeFirstName"]} {reader["EmployeeLastName"]}, SALARY: {reader["Salary"]}, ROLE: {reader["EmployeeRole"]}, STARTDATE: {reader["EmployeeStart"]}, TIME EMPLOYED: {timeEmployed.Days} days");
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
                                    Console.WriteLine("Employees:");
                                    while (reader.Read())
                                    {
                                        DateTime employeeStart = reader.GetDateTime(reader.GetOrdinal("EmployeeStart"));
                                        TimeSpan timeEmployed = DateTime.Now - employeeStart;
                                        Console.WriteLine($"ID: {reader["EmployeeID"]}, NAME: {reader["EmployeeFirstName"]} {reader["EmployeeLastName"]}, SALARY: {reader["Salary"]}, ROLE: {reader["EmployeeRole"]}, STARTDATE: {reader["EmployeeStart"]}, TIME EMPLOYED: {timeEmployed.Days} days");
                                    }
                                }
                            }
                        }

                        else if (filter.ToLower() == "Departments")
                        {
                            FetchTeacherDepartments();
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

            if (filter.ToLower() == "edit")
            {
                Console.WriteLine("Would you like to ADD or REMOVE an employee?");
                filter = Console.ReadLine();

                if (filter.ToLower() == "add")
                {
                    using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
                    {
                        if (connection != null)
                        {
                            Console.WriteLine("Enter the new employee's first name: ");
                            string firstName = Console.ReadLine();

                            Console.WriteLine("Enter the new employee's last name: ");
                            string lastName = Console.ReadLine();

                            Console.WriteLine("Enter the new employee's role: ");
                            string role = Console.ReadLine();

                            Console.WriteLine("Enter the new employee's salary: ");
                            int salary = int.Parse(Console.ReadLine());

                            DateTime startDate = DateTime.Now;

                            string query = "INSERT INTO Employees (EmployeeFirstName, EmployeeLastName, EmployeeRole, Salary, EmployeeStart) VALUES (@EmployeeFirstName, @EmployeeLastName, @EmployeeRole, @Salary, @StartDate)";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@EmployeeFirstName", firstName);
                                command.Parameters.AddWithValue("@EmployeeLastName", lastName);
                                command.Parameters.AddWithValue("@EmployeeRole", role);
                                command.Parameters.AddWithValue("@Salary", salary);
                                command.Parameters.AddWithValue("@StartDate", startDate);

                                try
                                {
                                    command.ExecuteNonQuery();
                                    Console.WriteLine("Employee added successfully!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to connect to the database.");
                        }
                    }
                }

                else if (filter.ToLower() == "remove")
                {
                    using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
                    {
                        if (connection != null)
                        {
                            Console.WriteLine("Which employee would like to remove?");
                            Console.WriteLine("Please state the ID to remove them");
                            Console.WriteLine("Here is a list of all the current employees in the school");
                            Console.Clear();
                            string query = "SELECT EmployeeID, EmployeeFirstName, EmployeeLastName, EmployeeRole, Salary, EmployeeStart FROM Employees";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    Console.WriteLine("Employees:");
                                    while (reader.Read())
                                    {
                                        Console.WriteLine($"ID: {reader["EmployeeID"]}, NAME: {reader["EmployeeFirstName"]} {reader["EmployeeLastName"]}, ROLE: {reader["EmployeeRole"]}, SALARY: {reader["Salary"]}, STARTDATE: {reader["EmployeeStart"]}");
                                    }
                                }
                            }
                            Console.WriteLine("Enter the ID of the employee to remove:");
                            int employeeId = int.Parse(Console.ReadLine());

                            string deleteQuery = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                            using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                            {
                                deleteCommand.Parameters.AddWithValue("@EmployeeID", employeeId);
                                try
                                {
                                    deleteCommand.ExecuteNonQuery();
                                    Console.WriteLine("Employee removed successfully!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to connect to the database.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Wrong syntax, try again.");
                }
            }
        }
    }
}
