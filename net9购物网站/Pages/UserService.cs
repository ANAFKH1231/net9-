using Microsoft.Data.SqlClient;
using net9购物网站.MODEL;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

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

        public static User? GetUserByEmail(string email, string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var command = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    Password = reader.GetString(2),
                    Email = reader.IsDBNull(3) ? null : reader.GetString(3)
                };
            }

            return null;
        }

        public static string SendVerificationCode(string email, IConfiguration configuration)
        {
            // 验证邮箱格式
            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            {
                throw new ArgumentException("无效的邮箱地址。");
            }

            // 生成随机验证码
            var code = new Random().Next(100000, 999999).ToString();

            try
            {
                // 从配置中读取 SMTP 信息
                var smtpConfig = configuration.GetSection("Smtp");
                var host = smtpConfig["Host"];
                var portString = smtpConfig["Port"];
                var username = smtpConfig["Username"];
                var password = smtpConfig["Password"];
                var enableSslString = smtpConfig["EnableSsl"];

                // 检查配置值是否为 null
                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portString) ||
                    string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(enableSslString))
                {
                    throw new InvalidOperationException("SMTP 配置信息不完整。");
                }

                var port = int.Parse(portString);
                var enableSsl = bool.Parse(enableSslString);

                // 配置 SMTP 客户端
                var smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new System.Net.NetworkCredential(username, password),
                    EnableSsl = enableSsl
                };

                // 创建邮件消息
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(username),
                    Subject = "验证码",
                    Body = $"您的验证码是: {code}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(email);

                // 发送邮件
                smtpClient.Send(mailMessage);

                Console.WriteLine($"验证码已发送到邮箱: {email}");
                return code; // 返回验证码
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送邮件失败: {ex.Message}");
                throw; // 重新抛出异常
            }
        }


    }
}
