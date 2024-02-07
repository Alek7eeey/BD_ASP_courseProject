using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;
using TaskWave_MVC.Models.ENUMs;

namespace TaskWave_MVC.Data.Repository
{
    public class UserRepository
    {
        private readonly Connection _dbConnection;
        public UserRepository(Connection connection)
        {
            _dbConnection = connection;
        }
        public User CreateStandartUser(User user)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.USER))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.CreateStandartUser";
                        command.CommandType = CommandType.StoredProcedure;

                        // Устанавливаем параметры процедуры
                        command.Parameters.Add(new OracleParameter("user_login", OracleDbType.NVarchar2)).Value = user.Login;
                        command.Parameters.Add(new OracleParameter("user_password", OracleDbType.NVarchar2)).Value = user.Password;
                        command.Parameters.Add(new OracleParameter("user_email", OracleDbType.NVarchar2)).Value = user.Email;
                        command.Parameters.Add(new OracleParameter("user_description", OracleDbType.NVarchar2)).Value = user.Description;

                        command.ExecuteNonQuery();
                    }
                }

                SignInDTO currentUser = new SignInDTO()
                {
                    Login = user.Login,
                    Password = user.Password
                };

                user = SignInUser(currentUser);
            }
            catch (OracleException oe)
            {
                Console.WriteLine(oe.Message);
                return null;
            }

            return user;
        }

        public User CreateSuperUser(User user)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.CreateSuperUser";
                        command.CommandType = CommandType.StoredProcedure;

                        // Устанавливаем параметры процедуры
                        command.Parameters.Add(new OracleParameter("user_login", OracleDbType.NVarchar2)).Value = user.Login;
                        command.Parameters.Add(new OracleParameter("user_password", OracleDbType.NVarchar2)).Value = user.Password;
                        command.Parameters.Add(new OracleParameter("user_email", OracleDbType.NVarchar2)).Value = user.Email;
                        command.Parameters.Add(new OracleParameter("user_description", OracleDbType.NVarchar2)).Value = user.Description;

                        command.ExecuteNonQuery();
                    }
                }

                SignInDTO currentUser = new SignInDTO()
                {
                    Login = user.Login,
                    Password = user.Password
                };

                user = GetUnaffectedUser(currentUser);
            }
            catch (OracleException oe)
            {
                Console.WriteLine(oe.Message);
                return null;
            }
            return user;
        }

        public User GetUserById(int id)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.getUserById(:param1); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.Int32).Value = id;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            User user = new User();
                            while (reader.Read())
                            {
                                // Обрабатываем результат
                                user.id = int.Parse(reader["id"].ToString());
                                user.Login = reader["login"].ToString();
                                user.Password = reader["password"].ToString();
                                user.Email = reader["gmail_address"].ToString();
                                user.Description = reader["description"].ToString();
                                string role = reader["type"].ToString();
                                switch (role)
                                {
                                    case "user":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.USER;
                                            break;
                                        }
                                    case "admin":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.ADMIN;
                                            break;
                                        }
                                    case "superUser":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.SUPERUSER;
                                            break;
                                        }
                                }
                            }

                            if (user.id == null)
                            {
                                return null;
                            }

                            return user;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }

            return null;
        }

        public User GetUserByLogin(string login)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.getUserByLogin(:param1); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.NVarchar2).Value = login;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            User user = new User();
                            while (reader.Read())
                            {
                                // Обрабатываем результат
                                user.id = int.Parse(reader["id"].ToString());
                                user.Login = reader["login"].ToString();
                                user.Password = reader["password"].ToString();
                                user.Email = reader["gmail_address"].ToString();
                                user.Description = reader["description"].ToString();
                                string role = reader["type"].ToString();
                                switch (role)
                                {
                                    case "user":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.USER;
                                            break;
                                        }
                                    case "admin":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.ADMIN;
                                            break;
                                        }
                                    case "superUser":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.SUPERUSER;
                                            break;
                                        }
                                }
                            }

                            if (user.id == null)
                            {
                                return null;
                            }

                            return user;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }

            return null;
        }

        public User SignInUser(SignInDTO userDTO)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.getUser(:param1, :param2); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.NVarchar2).Value = userDTO.Password;
                        command.Parameters.Add("param2", OracleDbType.NVarchar2).Value = userDTO.Login;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            User user = new User();
                            while (reader.Read())
                            {
                                // Обрабатываем результат
                                user.id = int.Parse(reader["id"].ToString());
                                user.Login = reader["login"].ToString();
                                user.Password = reader["password"].ToString();
                                user.Email = reader["gmail_address"].ToString();
                                user.Description = reader["description"].ToString();
                                string role = reader["type"].ToString();
                                switch (role)
                                {
                                    case "user":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.USER;
                                            break;
                                        }
                                    case "admin":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.ADMIN;
                                            break;
                                        }
                                    case "superUser":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.SUPERUSER;
                                            break;
                                        }
                                }
                            }

                            if (user.id == null)
                            {
                                return null;
                            }

                            return user;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }

            return null;
        }

        public User GetUnaffectedUser(SignInDTO userDTO)
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.getUnaffectedUser(:param1, :param2); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.NVarchar2).Value = userDTO.Password;
                        command.Parameters.Add("param2", OracleDbType.NVarchar2).Value = userDTO.Login;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            User user = new User();
                            while (reader.Read())
                            {
                                // Обрабатываем результат
                                user.id = int.Parse(reader["id"].ToString());
                                user.Login = reader["login"].ToString();
                                user.Password = reader["password"].ToString();
                                user.Email = reader["gmail_address"].ToString();
                                user.Description = reader["description"].ToString();
                                string role = reader["type"].ToString();
                                switch (role)
                                {
                                    case "user":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.USER;
                                            break;
                                        }
                                    case "admin":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.ADMIN;
                                            break;
                                        }
                                    case "superUser":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.SUPERUSER;
                                            break;
                                        }
                                }
                            }

                            if (user.id == null)
                            {
                                return null;
                            }

                            return user;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }

            return null;
        }

        public bool ChangeUser(SignInDTO us, int idUser)
        {
            User user = GetUserById(idUser);

            if (us.Login != user.Login && GetUserByLogin(us.Login) != null)
            {
                return false;
            }

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "ADMIN_TASK_WAVE.ChangeUserInformation";
                            command.CommandType = CommandType.StoredProcedure;

                            // Устанавливаем параметры процедуры
                            command.Parameters.Add(new OracleParameter("user_login", OracleDbType.NVarchar2)).Value = us.Login;
                            command.Parameters.Add(new OracleParameter("user_password", OracleDbType.NVarchar2)).Value = us.Password;
                            command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32)).Value = idUser;

                            command.ExecuteNonQuery();
                        }
                    }

                    return true;
                }

                catch (OracleException oe)
                {
                    Console.WriteLine(oe.Message);
                    return false;
                }

            }

            return false;
        }

        public bool ChangeDescriptionAccount(UserDescriptionDTO us, int idUser)
        {
            User user = GetUserById(idUser);
            byte[]? photo = null;
            string? title = null;

            if (us.image != null && us.image.Length > 0)
            {
                byte[] fileBytes;

                using (var stream = new MemoryStream())
                {
                    us.image.CopyTo(stream);
                    fileBytes = stream.ToArray();
                }

                photo = fileBytes;
                title = us.image.FileName;
            }

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "ADMIN_TASK_WAVE.ChangeDescriptionAccount";
                            command.CommandType = CommandType.StoredProcedure;

                            // Устанавливаем параметры процедуры
                            command.Parameters.Add(new OracleParameter("userid", OracleDbType.Int32)).Value = idUser;
                            command.Parameters.Add(new OracleParameter("user_email", OracleDbType.NVarchar2)).Value = us.email;
                            command.Parameters.Add(new OracleParameter("user_company", OracleDbType.NVarchar2)).Value = us.company;
                            command.Parameters.Add(new OracleParameter("user_telegram", OracleDbType.NVarchar2)).Value = us.telegram;
                            command.Parameters.Add(new OracleParameter("user_city", OracleDbType.NVarchar2)).Value = us.city;
                            command.Parameters.Add(new OracleParameter("user_image", OracleDbType.Blob)).Value = photo;
                            command.Parameters.Add(new OracleParameter("user_title", OracleDbType.NVarchar2)).Value = title;

                            command.ExecuteNonQuery();
                        }
                    }

                    return true;
                }

                catch (OracleException oe)
                {
                    Console.WriteLine(oe.Message);
                    return false;
                }

            }

            return false;
        }

        public List<User> GetAllUsers()
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetAllUsers(); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            User user = null;
                            List<User> users = new List<User>();

                            while (reader.Read())
                            {
                                user = new User();
                                // Обрабатываем результат
                                user.id = int.Parse(reader["id"].ToString());
                                user.Login = reader["login"].ToString();
                                user.Password = reader["password"].ToString();
                                user.Email = reader["gmail_address"].ToString();
                                user.Description = reader["description"].ToString();
                                string role = reader["type"].ToString();
                                switch (role)
                                {
                                    case "user":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.USER;
                                            break;
                                        }
                                    case "admin":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.ADMIN;
                                            break;
                                        }
                                    case "superUser":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.SUPERUSER;
                                            break;
                                        }
                                }
                                users.Add(user);
                            }

                            if (users.Count == 0)
                            {
                                return null;
                            }

                            return users;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }

            return null;
        }

        public bool DeleteUser(int idUser)
        {

            if (idUser != 0)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(RoleENUM.ADMIN))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "ADMIN_TASK_WAVE.DeleteUser";
                            command.CommandType = CommandType.StoredProcedure;

                            // Устанавливаем параметры процедуры
                            command.Parameters.Add(new OracleParameter("userId", OracleDbType.Int32)).Value = idUser;

                            command.ExecuteNonQuery();
                        }
                    }

                    return true;
                }

                catch (OracleException oe)
                {
                    Console.WriteLine(oe.Message);
                    return false;
                }

            }

            return false;
        }

        public List<User> GetAllUnaffectedUser()
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(Models.ENUMs.RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetAllUnaffectedUser(); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            User user = null;
                            List<User> users = new List<User>();

                            while (reader.Read())
                            {
                                user = new User();
                                // Обрабатываем результат
                                user.id = int.Parse(reader["id"].ToString());
                                user.Login = reader["login"].ToString();
                                user.Password = reader["password"].ToString();
                                user.Email = reader["gmail_address"].ToString();
                                user.Description = reader["description"].ToString();
                                string role = reader["type"].ToString();
                                switch (role)
                                {
                                    case "user":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.USER;
                                            break;
                                        }
                                    case "admin":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.ADMIN;
                                            break;
                                        }
                                    case "superUser":
                                        {
                                            user.Role = Models.ENUMs.RoleENUM.SUPERUSER;
                                            break;
                                        }
                                }
                                users.Add(user);
                            }

                            if (users.Count == 0)
                            {
                                return null;
                            }

                            return users;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }

            return null;
        }

        public bool ConfirmUser(int idUser)
        {

            if (idUser != 0)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(RoleENUM.ADMIN))
                    {
                        connection.Open();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "ADMIN_TASK_WAVE.ConfirmUser";
                            command.CommandType = CommandType.StoredProcedure;

                            // Устанавливаем параметры процедуры
                            command.Parameters.Add(new OracleParameter("user_Id", OracleDbType.Int32)).Value = idUser;

                            command.ExecuteNonQuery();
                        }
                    }

                    return true;
                }

                catch (OracleException oe)
                {
                    Console.WriteLine(oe.Message);
                    return false;
                }

            }

            return false;
        }
    }
}
