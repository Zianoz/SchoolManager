﻿using Microsoft.Data.SqlClient;

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

        // Options for fetching employees
        public static void FetchEmployees(string connectionString)
        {
            Console.WriteLine("Select an option");
            Console.WriteLine("1. View all employees");
            Console.WriteLine("2. View employees in a role");
            Console.WriteLine("3. View how many employees in each department");
            Console.WriteLine("4. Edit employees");
            string choice = Console.ReadLine();
            Console.Clear();

            switch (choice)
            {
                case "1":
                    ViewAllEmployees(connectionString);
                    break;
                case "2":
                    ViewRole(connectionString);
                    break;
                case "3":
                    FetchTeacherDepartments(); //EF
                    break;
                case "4":
                    EditEmployee(connectionString);
                    break;
            }

        }
        // Method to handle department salary information options
        public static void DepartmentSalaryInfo(string connectionString)
        {
            Console.WriteLine("Select an option");
            Console.WriteLine("1. View how much each department pays each month");
            Console.WriteLine("2. View average salary for each department");
            string choice = Console.ReadLine();
            Console.Clear();

            switch (choice)
            {
                case "1":
                    DepartmentSalaryPerMonth(connectionString); // View total salary per month
                    break;

                case "2":
                    AverageDepartmentSalary(connectionString); // View average salary
                    break;
            }
        }

        // Method to display average salary for each department
        public static void AverageDepartmentSalary(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = @"
                        SELECT EmployeeRole, AVG(Salary) AS AverageSalary
                        FROM Employees
                        GROUP BY EmployeeRole";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("Department | Average Salary");
                            Console.WriteLine(new string('-', 30));

                            while (reader.Read())
                            {
                                Console.WriteLine($"{reader["EmployeeRole"]} | {reader["AverageSalary"]}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Connection to database failed");
                }
            }
        }
        // Method to display total salary per month for each department
        public static void DepartmentSalaryPerMonth(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = @"
                        SELECT EmployeeRole, SUM(Salary) AS TotalSalaryPerMonth
                        FROM Employees
                        GROUP BY EmployeeRole";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("Department | Total Monthly Salary");
                            // Additional code to display the results
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Connection to database failed");
                }
            }
        }

        // Method to fetch teacher departments
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

        // Method to view all employees
        public static void ViewAllEmployees(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = "SELECT EmployeeID, EmployeeRole, EmployeeFirstName, EmployeeLastName, Salary, EmployeeStart FROM Employees";

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

            }

        }

        // Method to edit employee details
        public static void EditEmployee(string connectionString)
        {
            Console.WriteLine("Select an option");
            Console.WriteLine("1. Add new employee");
            Console.WriteLine("2. Remove existing employee");
            string choice = Console.ReadLine();
            Console.Clear();

            if (choice == "1")
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

            else if (choice == "2")
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

        // Method to view roles of employees
        public static void ViewRole(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = "SELECT EmployeeID, EmployeeRole, EmployeeFirstName, EmployeeLastName, Salary, EmployeeStart FROM Employees";

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

                else
                {
                    Console.WriteLine("Connection to database failed");
                }

            }
        }
        

    }
}


