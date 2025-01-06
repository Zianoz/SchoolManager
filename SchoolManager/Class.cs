using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolManager
{
    public class Class
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }

        // Navigation property
        public ICollection<Student> Students { get; set; }

        public static void AllCourses()
        {
            using (var context = new SchoolContext())
            {
                var classes = context.Classes.ToList();

                Console.WriteLine("ClassID | ClassName");
                Console.WriteLine(new string('-', 30));

                foreach (var cls in classes)
                {
                    Console.WriteLine($"{cls.ClassID,-8} | {cls.ClassName}");
                }

                
            }
        }
    }
}
