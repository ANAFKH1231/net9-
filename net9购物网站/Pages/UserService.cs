using Microsoft.Data.SqlClient;
using net9购物网站.MODEL;

namespace net9购物网站.Services
{
    public static class UserService
    {

        public static User? Login(string username, string password, string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        SELECT Id, Username, Password, Email
                        FROM Users
                        WHERE Username = @Username AND Password = @Password";
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    Console.WriteLine($"Executing SQL: {command.CommandText}");
                    Console.WriteLine($"Parameters: Username={username}, Password={password}");

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("User found in database.");
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Email = reader.IsDBNull(3) ? null : reader.GetString(3)
                            };
                        }
                        else
                        {
                            Console.WriteLine("No user found with the provided credentials.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
            }

            return null; // Login failed
        }


        public static bool Register(User user, string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO Users (Username, Password, Email)
                        VALUES (@Username, @Password, @Email)";
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);

                    command.ExecuteNonQuery();
                }

                return true; // Registration successful
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration failed: {ex.Message}");
                return false; // Registration failed
            }
        }
    }
}
