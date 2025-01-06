using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManager
{
    public class Interface
    {

        public static void Options(string connectionString)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. View/Edit Employees");
            Console.WriteLine("2. View/Edit Students");
            Console.WriteLine("3. View grades");
            Console.WriteLine("4. View courses");
            Console.WriteLine("5. View departments salary info");
            Console.WriteLine("6. Exit");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Employee.FetchEmployees(connectionString);
                    WaitForX();
                    break;

                case "2":
                    Console.Clear();
                    Student.FetchStudents(connectionString);
                    WaitForX();
                    break;

                case "3":
                    Console.Clear();
                    Grades.GradeOption(connectionString);
                    WaitForX();
                    break;

                case "4":
                    Console.Clear();
                    Class.AllCourses();
                    WaitForX();
                    break;
                
                case "5":
                    Console.Clear();
                    Employee.DepartmentSalaryInfo(connectionString);
                    WaitForX();
                    break;
                case "6":
                    Console.Clear();
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Wrong syntax try again");
                    break;

            }
        }

        static ConsoleKey WaitForReadkey()
        {
            // Reads a key press from the console without displaying it
            // (intercept: true hides the key from appearing on the console)
            return Console.ReadKey(intercept: true).Key;
        }

        public static void WaitForX()
        {

            Console.WriteLine("Press X to go back to the menu");

            while (true)
            {
                // Captures the keypress
                ConsoleKey keyInfo = WaitForReadkey();

                // Check if the key pressed was 'X'
                if (keyInfo == ConsoleKey.X)
                {
                    Console.Clear();
                    return;
                }
                else
                {
                    Console.WriteLine("Try again");
                }

            }

        }

    }
}
