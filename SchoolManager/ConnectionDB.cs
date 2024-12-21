using Microsoft.Data.SqlClient;
using System;

namespace SchoolManager
{
    public class ConnectionDB
    {
        public static SqlConnection GetDatabaseConnection(string connectionString)
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
