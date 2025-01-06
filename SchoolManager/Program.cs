using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.EntityFrameworkCore.Storage;
namespace SchoolManager
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=SchoolManager;Trusted_Connection=True";

            while (true)
            {
                Interface.Options(connectionString);
            }
        }
    }
}
