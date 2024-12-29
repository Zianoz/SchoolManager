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
    }
}
