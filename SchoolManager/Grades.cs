using Microsoft.Data.SqlClient;
using System;

namespace SchoolManager
{
    internal class Grades
    {
        public static void GradeOption(string connectionString)
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("1: See all current courses");
                Console.WriteLine("2: Set a grade for a student in a course");
                Console.WriteLine("3: View grades");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Clear();
                    ViewCourses(connectionString);
                }
                else if (choice == "2")
                {
                    Console.Clear();
                    SetGrade(connectionString);
                }
                else if (choice == "3")
                {
                    Console.Clear();
                    ViewGrades(connectionString);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Wrong syntax, try again\n");
                }
            }
        }

        internal static void ViewCourses(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = "SELECT CourseID, CourseName FROM Courses";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("Courses:");
                            while (reader.Read())
                            {
                                Console.WriteLine($"CourseID: {reader["CourseID"]}, CourseName: {reader["CourseName"]}");
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

        internal static void SetGrade(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    // List all students
                    Console.WriteLine("Select a Student ID from the following list:");
                    string studentQuery = "SELECT StudentID, StudentFirstName, StudentLastName FROM Students";
                    using (SqlCommand studentCommand = new SqlCommand(studentQuery, connection))
                    {
                        using (SqlDataReader studentReader = studentCommand.ExecuteReader())
                        {
                            while (studentReader.Read())
                            {
                                Console.WriteLine($"StudentID: {studentReader["StudentID"]}, Name: {studentReader["StudentFirstName"]} {studentReader["StudentLastName"]}");
                            }
                        }
                    }

                    Console.Write("Enter StudentID: ");
                    int studentId;
                    while (!int.TryParse(Console.ReadLine(), out studentId))
                    {
                        Console.Write("Invalid input. Please enter a valid StudentID: ");
                    }

                    // List all courses
                    Console.WriteLine("\nSelect a Course ID from the following list:");
                    string courseQuery = "SELECT CourseID, CourseName FROM Courses";
                    using (SqlCommand courseCommand = new SqlCommand(courseQuery, connection))
                    {
                        using (SqlDataReader courseReader = courseCommand.ExecuteReader())
                        {
                            while (courseReader.Read())
                            {
                                Console.WriteLine($"CourseID: {courseReader["CourseID"]}, CourseName: {courseReader["CourseName"]}");
                            }
                        }
                    }

                    Console.Write("Enter CourseID: ");
                    int courseId;
                    while (!int.TryParse(Console.ReadLine(), out courseId))
                    {
                        Console.Write("Invalid input. Please enter a valid CourseID: ");
                    }

                    // List all teachers (employees)
                    Console.WriteLine("\nSelect an Employee ID (Teacher) from the following list:");
                    string employeeQuery = "SELECT EmployeeID, EmployeeFirstName, EmployeeLastName FROM Employees WHERE EmployeeRole = 'Teacher'";
                    using (SqlCommand employeeCommand = new SqlCommand(employeeQuery, connection))
                    {
                        using (SqlDataReader employeeReader = employeeCommand.ExecuteReader())
                        {
                            while (employeeReader.Read())
                            {
                                Console.WriteLine($"EmployeeID: {employeeReader["EmployeeID"]}, Name: {employeeReader["EmployeeFirstName"]} {employeeReader["EmployeeLastName"]}");
                            }
                        }
                    }

                    Console.Write("Enter EmployeeID: ");
                    int employeeId;
                    Console.Clear();
                    while (!int.TryParse(Console.ReadLine(), out employeeId))
                    {
                        Console.Write("Invalid input. Please enter a valid EmployeeID: ");
                    }

                    // Enter Grade
                    Console.Write("Enter Grade (e.g., A+, A, B, etc.): ");
                    string grade = Console.ReadLine();

                    DateTime assignedDate = DateTime.Now;

                    // Insert into Grades table
                    string insertQuery = "INSERT INTO Grades (StudentID, EmployeeID, CourseID, Grade, AssignedDate) VALUES (@StudentID, @EmployeeID, @CourseID, @Grade, @AssignedDate)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@StudentID", studentId);
                        insertCommand.Parameters.AddWithValue("@EmployeeID", employeeId);
                        insertCommand.Parameters.AddWithValue("@CourseID", courseId);
                        insertCommand.Parameters.AddWithValue("@Grade", grade);
                        insertCommand.Parameters.AddWithValue("@AssignedDate", assignedDate);

                        try
                        {
                            insertCommand.ExecuteNonQuery();
                            Console.WriteLine("Grade assigned successfully!");
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

        internal static void ViewGrades(string connectionString)
        {
            using (SqlConnection connection = ConnectionDB.GetDatabaseConnection(connectionString))
            {
                if (connection != null)
                {
                    string query = @"
                        SELECT 
                            g.GradeID,
                            s.StudentFirstName,
                            s.StudentLastName,
                            c.CourseName,
                            g.Grade,
                            e.EmployeeFirstName,
                            e.EmployeeLastName,
                            g.AssignedDate
                        FROM Grades g
                        INNER JOIN Students s ON g.StudentID = s.StudentID
                        INNER JOIN Courses c ON g.CourseID = c.CourseID
                        INNER JOIN Employees e ON g.EmployeeID = e.EmployeeID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("Grades:");
                            while (reader.Read())
                            {
                                int gradeId = (int)reader["GradeID"];
                                string studentName = $"{reader["StudentFirstName"]} {reader["StudentLastName"]}";
                                string courseName = reader["CourseName"].ToString();
                                string grade = reader["Grade"].ToString();
                                string teacherName = $"{reader["EmployeeFirstName"]} {reader["EmployeeLastName"]}";
                                DateTime assignedDate = (DateTime)reader["AssignedDate"];

                                Console.WriteLine($"GradeID: {gradeId}, Student: {studentName}, Course: {courseName}, Grade: {grade}, Teacher: {teacherName}, Date Assigned: {assignedDate.ToShortDateString()}");
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
    }
}
