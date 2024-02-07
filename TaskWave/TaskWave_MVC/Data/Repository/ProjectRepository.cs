using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using TaskWave_MVC.Models;
using TaskWave_MVC.Models.ENUMs;
using Task = TaskWave_MVC.Models.Task;

namespace TaskWave_MVC.Data.Repository
{
    public class ProjectRepository
    {
        private readonly Connection _dbConnection;
        private readonly UserRepository _userRepository;
        public ProjectRepository(Connection connection, UserRepository userRepository)
        {
            _dbConnection = connection;
            _userRepository = userRepository;
        }

        public bool CreateProject(Project proj)
        {
            RoleENUM currentRole;
            int resultValue;

            if (proj.type == Models.ENUMs.ProjectENUM.GROUP)
            {
                currentRole = RoleENUM.SUPERUSER;
            }
            else
            {
                currentRole = RoleENUM.USER;
            }

            try
            {
                using (var connection = _dbConnection.GetConnection(currentRole))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.CreateProject";
                        command.CommandType = CommandType.StoredProcedure;

                        // Set the parameters for the stored procedure
                        command.Parameters.Add(new OracleParameter("creator_id", OracleDbType.Int32)).Value = proj.creatorId;
                        command.Parameters.Add(new OracleParameter("project_name", OracleDbType.NVarchar2)).Value = proj.name;
                        command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = new DateTime(proj.toDate.Year, proj.toDate.Month, proj.toDate.Day);
                        command.Parameters.Add(new OracleParameter("project_description", OracleDbType.NVarchar2)).Value = proj.description;
                        if (proj.photo != null)
                        {
                            command.Parameters.Add(new OracleParameter("photo_name", OracleDbType.NVarchar2)).Value = proj.photo.fileName;
                            command.Parameters.Add(new OracleParameter("photo_data", OracleDbType.Blob)).Value = proj.photo.photo;
                        }
                        else
                        {
                            command.Parameters.Add(new OracleParameter("photo_name", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("photo_data", OracleDbType.Blob)).Value = null;
                        }

                        if (proj.document != null)
                        {
                            command.Parameters.Add(new OracleParameter("document_name", OracleDbType.NVarchar2)).Value = proj.document.title;
                            command.Parameters.Add(new OracleParameter("document_type", OracleDbType.NVarchar2)).Value = proj.document.type;
                            command.Parameters.Add(new OracleParameter("document_data", OracleDbType.Blob)).Value = proj.document.data;
                        }
                        else
                        {
                            command.Parameters.Add(new OracleParameter("document_name", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("document_type", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("document_data", OracleDbType.Blob)).Value = null;

                        }

                        // OUT parameter
                        OracleParameter resultParameter = new OracleParameter("result", OracleDbType.Int32);
                        resultParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParameter);

                        command.ExecuteNonQuery();

                        // Retrieve the value of the OUT parameter
                        resultValue = Convert.ToInt32(Convert.ToString(resultParameter.Value));
                    }

                    if (proj.tasks != null)
                    {
                        foreach (var t in proj.tasks)
                        {
                            if (t.description == null)
                            {
                                continue;
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "ADMIN_TASK_WAVE.CreateTask";
                                command.CommandType = CommandType.StoredProcedure;

                                // Set the parameters for the stored procedure
                                command.Parameters.Add(new OracleParameter("project_id ", OracleDbType.Int32)).Value = resultValue;
                                command.Parameters.Add(new OracleParameter("description", OracleDbType.NVarchar2)).Value = t.description;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (OracleException oe)
            {
                Console.WriteLine(oe.Message);
                return false;
            }
            return true;
        }

        public IEnumerable<Project> GetMyProjects(int userId)
        {
            User user = _userRepository.GetUserById(userId);

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetOwnProjects(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = user.id;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {
                                List<Project> projects = new List<Project>();
                                ProjectPhoto projectPhoto = null;
                                ProjectDocument projectDocument = null;

                                while (reader.Read())
                                {
                                    string? idPhotoValue = reader["idPh"].ToString();
                                    if (idPhotoValue != null && idPhotoValue != "")
                                    {
                                        projectPhoto = new ProjectPhoto()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            fileName = reader["titlePh"].ToString(),
                                            photo = (byte[])reader["photoPh"]
                                        };
                                    }

                                    string? idDocValue = reader["idDoc"].ToString();
                                    if (idDocValue != null && idDocValue != "")
                                    {
                                        projectDocument = new ProjectDocument()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            title = reader["titleDoc"].ToString(),
                                            type = reader["typeDoc"].ToString(),
                                            data = (byte[])reader["dataDoc"]
                                        };
                                    }

                                    string? idProjValue = reader["ID"].ToString();
                                    Project project = null;
                                    if (idProjValue != null && idProjValue != "")
                                    {
                                        project = new Project()
                                        {
                                            id = int.Parse(reader["ID"].ToString()),
                                            creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                            name = reader["NAME"].ToString(),
                                            toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                            fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                            description = reader["DESCRIPTION"].ToString(),
                                            type = ProjectENUM.INDIVIDUAL
                                        };
                                    }


                                    if (projectPhoto != null && project != null)
                                    {
                                        project.photo = projectPhoto;
                                    }

                                    if (projectDocument != null && project != null)
                                    {
                                        project.document = projectDocument;
                                    }

                                    projects.Add(project);
                                }

                                if (projects.Count() == 0)
                                {
                                    return null;
                                }

                                return projects;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public IEnumerable<Project> GetGroupProjects(int userId)
        {
            User user = _userRepository.GetUserById(userId);

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GETTEAMPROJECTS(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = user.id;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {
                                List<Project> projects = new List<Project>();
                                ProjectPhoto projectPhoto = null;
                                ProjectDocument projectDocument = null;

                                while (reader.Read())
                                {
                                    string? idPhotoValue = reader["idPh"].ToString();
                                    if (idPhotoValue != null && idPhotoValue != "")
                                    {
                                        projectPhoto = new ProjectPhoto()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            fileName = reader["titlePh"].ToString(),
                                            photo = (byte[])reader["photoPh"]
                                        };
                                    }

                                    string? idDocValue = reader["idDoc"].ToString();
                                    if (idDocValue != null && idDocValue != "")
                                    {
                                        projectDocument = new ProjectDocument()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            title = reader["titleDoc"].ToString(),
                                            type = reader["typeDoc"].ToString(),
                                            data = (byte[])reader["dataDoc"]
                                        };
                                    }

                                    string? idProjValue = reader["ID"].ToString();
                                    Project project = null;
                                    if (idProjValue != null && idProjValue != "")
                                    {
                                        project = new Project()
                                        {
                                            id = int.Parse(reader["ID"].ToString()),
                                            creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                            name = reader["NAME"].ToString(),
                                            toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                            fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                            description = reader["DESCRIPTION"].ToString(),
                                            type = ProjectENUM.INDIVIDUAL
                                        };
                                    }

                                    if (projectPhoto != null && project != null)
                                    {
                                        project.photo = projectPhoto;
                                    }

                                    if (projectDocument != null && project != null)
                                    {
                                        project.document = projectDocument;
                                    }

                                    projects.Add(project);
                                }

                                if (projects.Count() == 0)
                                {
                                    return null;
                                }

                                return projects;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public IEnumerable<SendProject> GetSendProjects(int userId)
        {
            User user = _userRepository.GetUserById(userId);

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetSendProjects(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = user.id;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {

                                List<SendProject> sendProjects = new List<SendProject>();
                                ProjectPhoto projectPhoto = null;
                                ProjectDocument projectDocument = null;
                                SendProject readyProject = null;

                                while (reader.Read())
                                {
                                    string? idPhotoValue = reader["idPh"].ToString();
                                    if (idPhotoValue != null && idPhotoValue != "")
                                    {
                                        projectPhoto = new ProjectPhoto()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            fileName = reader["titlePh"].ToString(),
                                            photo = (byte[])reader["photoPh"]
                                        };
                                    }

                                    string? idDocValue = reader["idDoc"].ToString();
                                    if (idDocValue != null && idDocValue != "")
                                    {
                                        projectDocument = new ProjectDocument()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            title = reader["titleDoc"].ToString(),
                                            type = reader["typeDoc"].ToString(),
                                            data = (byte[])reader["dataDoc"]
                                        };
                                    }

                                    int? idSendProject = int.Parse(reader["ID_SEND_PROJECT"].ToString());

                                    if (idSendProject != null)
                                    {
                                        readyProject = new SendProject()
                                        {
                                            id = idSendProject,
                                            sender_user_id = int.Parse(reader["SENDER_USER_ID"].ToString()),
                                            dateSend = DateOnly.FromDateTime(DateTime.Parse(reader["DATE_SEND_PROJECT"].ToString())),
                                            description = reader["SEND_PROJECT_DESCRIPTION"].ToString(),
                                            project_id = int.Parse(reader["ID"].ToString()),
                                            creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                            project_name = reader["NAME"].ToString(),
                                            toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                            fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                        };

                                    }


                                    if (projectPhoto != null && readyProject != null)
                                    {
                                        readyProject.photo = projectPhoto;
                                    }

                                    if (projectDocument != null && readyProject != null)
                                    {
                                        readyProject.document = projectDocument;
                                    }

                                    sendProjects.Add(readyProject);
                                }

                                if (sendProjects.Count() == 0)
                                {
                                    return null;
                                }

                                return sendProjects;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public IEnumerable<ReadyProject> GetReadyProjects(int userId)
        {
            User user = _userRepository.GetUserById(userId);

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetReadyProjects(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = user.id;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {

                                List<ReadyProject> readyProjects = new List<ReadyProject>();
                                ProjectPhoto projectPhoto = null;
                                ProjectDocument projectDocument = null;
                                ReadyProject readyProject = null;

                                while (reader.Read())
                                {
                                    string? idPhotoValue = reader["idPh"].ToString();
                                    if (idPhotoValue != null && idPhotoValue != "")
                                    {
                                        projectPhoto = new ProjectPhoto()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            fileName = reader["titlePh"].ToString(),
                                            photo = (byte[])reader["photoPh"]
                                        };
                                    }

                                    string? idDocValue = reader["idDoc"].ToString();
                                    if (idDocValue != null && idDocValue != "")
                                    {
                                        projectDocument = new ProjectDocument()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            title = reader["titleDoc"].ToString(),
                                            type = reader["typeDoc"].ToString(),
                                            data = (byte[])reader["dataDoc"]
                                        };
                                    }
                                    int? idReadyProject = null;
                                    if (reader["ID_READY_PROJECT"].ToString() != null && reader["ID_READY_PROJECT"].ToString() != "")
                                    {
                                        idReadyProject = int.Parse(reader["ID_READY_PROJECT"].ToString());
                                    }

                                    if (idReadyProject != null)
                                    {
                                        readyProject = new ReadyProject()
                                        {
                                            id = idReadyProject,
                                            perfomer_user_id = int.Parse(reader["perfomer_USER_ID"].ToString()),
                                            dateComplete = DateOnly.FromDateTime(DateTime.Parse(reader["DATE_COMPLETE_PROJECT"].ToString())),
                                            project_id = int.Parse(reader["ID"].ToString()),
                                            creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                            project_name = reader["NAME"].ToString(),
                                            toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                            fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                            description = reader["DESCRIPTION"].ToString()
                                        };

                                    }

                                    if (projectPhoto != null && readyProject != null)
                                    {
                                        readyProject.photo = projectPhoto;
                                        projectPhoto = null;
                                    }

                                    if (projectDocument != null && readyProject != null)
                                    {
                                        readyProject.document = projectDocument;
                                        projectDocument = null;
                                    }

                                    if(readyProject != null)
                                    {
                                        readyProjects.Add(readyProject);
                                        readyProject = null;
                                    }
                                }

                                if (readyProjects.Count() == 0)
                                {
                                    return null;
                                }

                                return readyProjects;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public IEnumerable<Stat> GetStat(int userId)
        {
            User user = _userRepository.GetUserById(userId);

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetStat(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = user.id;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {

                                List<Stat> stats = new List<Stat>();

                                while (reader.Read())
                                {
                                    string? month = reader["Month"].ToString();
                                    int count = int.Parse(reader["ProjectCount"].ToString());

                                    stats.Add(new Stat() { month = month, count = count });
                                }

                                return stats;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public UserDescription GetDescriptionAccount(int userId)
        {
            User user = _userRepository.GetUserById(userId);
            UserDescription userDescr = null;
            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetDescription(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = user.id;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {

                                while (reader.Read())
                                {
                                    int idDescr = -1;
                                    if (reader["ID"].ToString() != null && reader["ID"].ToString() != "")
                                    {
                                        idDescr = int.Parse(reader["ID"].ToString());
                                    }

                                    string? gmail = reader["GMAIL"].ToString();
                                    string? telegram = reader["TELEGRAM"].ToString();
                                    string? company = reader["COMPANY"].ToString();
                                    string? city = reader["CITY"].ToString();

                                    string? login = reader["LOGIN"].ToString();
                                    string? password = reader["PASSWORD"].ToString();

                                    string titlePhoto = reader["TITLEPHOTO"].ToString();
                                    byte[]? image = null;

                                    if (titlePhoto != "" && titlePhoto != null)
                                    {
                                        image = (byte[])reader["IMAGE"];
                                    }

                                    userDescr = new UserDescription()
                                    {
                                        id = idDescr,
                                        user_id = userId,
                                        email = gmail,
                                        telegram = telegram,
                                        company = company,
                                        city = city,
                                        login = login,
                                        password = password,
                                        titlePhoto = titlePhoto,
                                        image = image
                                    };
                                }
                            }


                        }

                        if (userDescr.id == -1)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "ADMIN_TASK_WAVE.CreateDefaultDescription ";
                                command.CommandType = CommandType.StoredProcedure;

                                // Устанавливаем параметры процедуры
                                command.Parameters.Add(new OracleParameter("user_id", OracleDbType.Int32)).Value = userId;
                                command.ExecuteNonQuery();
                            }
                        }

                        return userDescr;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public Project GetCurrentProject(int projectId, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            Project project = null;
            ProjectPhoto projectPhoto = null;
            ProjectDocument projectDocument = null;
            List<Task> tasks = new List<Task>();
            Task task = null;

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentProject(:param1); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            while (reader.Read())
                            {
                                string? idPhotoValue = reader["idPh"].ToString();
                                if (idPhotoValue != null && idPhotoValue != "")
                                {
                                    projectPhoto = new ProjectPhoto()
                                    {
                                        id = int.Parse(reader["idPh"].ToString()),
                                        fileName = reader["titlePh"].ToString(),
                                        photo = (byte[])reader["photoPh"]
                                    };
                                }

                                string? idDocValue = reader["idDoc"].ToString();
                                if (idDocValue != null && idDocValue != "")
                                {
                                    projectDocument = new ProjectDocument()
                                    {
                                        id = int.Parse(reader["idPh"].ToString()),
                                        title = reader["titleDoc"].ToString(),
                                        type = reader["typeDoc"].ToString(),
                                        data = (byte[])reader["dataDoc"]
                                    };
                                }

                                string? idProjValue = reader["ID"].ToString();
                                if (idProjValue != null && idProjValue != "")
                                {
                                    project = new Project()
                                    {
                                        id = int.Parse(reader["ID"].ToString()),
                                        creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                        name = reader["NAME"].ToString(),
                                        toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                        fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                        description = reader["DESCRIPTION"].ToString(),
                                        type = ProjectENUM.INDIVIDUAL
                                    };
                                }

                                if (projectPhoto != null)
                                {
                                    project.photo = projectPhoto;
                                }

                                if (projectDocument != null)
                                {
                                    project.document = projectDocument;
                                }
                            }
                        }
                    }

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentTasks(:param1); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            while (reader.Read())
                            {
                                string? idTaskValue = reader["idTask"].ToString();
                                if (idTaskValue != null && idTaskValue != "")
                                {
                                    task = new Task()
                                    {
                                        id = int.Parse(reader["idTask"].ToString()),
                                        description = reader["TaskDescription"].ToString(),
                                        projectId = projectId
                                    };

                                    tasks.Add(task);
                                }
                            }

                            if (tasks.Count != 0)
                            {
                                project.tasks = tasks;
                            }
                        }
                    }

                    return project;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return null;
            }
        }

        public bool CompleteProject(int projectId, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.CompleteProject";
                        command.CommandType = CommandType.StoredProcedure;

                        // Устанавливаем параметры процедуры
                        command.Parameters.Add(new OracleParameter("projectId", OracleDbType.Int32)).Value = projectId;
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return false;
            }
        }

        public bool DeleteProject(int projectId, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.DeleteProject";
                        command.CommandType = CommandType.StoredProcedure;

                        // Устанавливаем параметры процедуры
                        command.Parameters.Add(new OracleParameter("projectId", OracleDbType.Int32)).Value = projectId;
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return false;
            }
        }

        public bool UpdateProject(Project proj, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.UpdateProject";
                        command.CommandType = CommandType.StoredProcedure;

                        // Set the parameters for the stored procedure
                        command.Parameters.Add(new OracleParameter("proj_id", OracleDbType.Int32)).Value = proj.id;
                        command.Parameters.Add(new OracleParameter("project_name", OracleDbType.NVarchar2)).Value = proj.name;
                        command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = new DateTime(proj.toDate.Year, proj.toDate.Month, proj.toDate.Day);
                        command.Parameters.Add(new OracleParameter("project_description", OracleDbType.NVarchar2)).Value = proj.description;

                        if (proj.photo != null)
                        {
                            command.Parameters.Add(new OracleParameter("photo_name", OracleDbType.NVarchar2)).Value = proj.photo.fileName;
                            command.Parameters.Add(new OracleParameter("photo_data", OracleDbType.Blob)).Value = proj.photo.photo;
                        }
                        else
                        {
                            command.Parameters.Add(new OracleParameter("photo_name", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("photo_data", OracleDbType.Blob)).Value = null;
                        }

                        if (proj.document != null)
                        {
                            command.Parameters.Add(new OracleParameter("document_name", OracleDbType.NVarchar2)).Value = proj.document.title;
                            command.Parameters.Add(new OracleParameter("document_type", OracleDbType.NVarchar2)).Value = proj.document.type;
                            command.Parameters.Add(new OracleParameter("document_data", OracleDbType.Blob)).Value = proj.document.data;
                        }
                        else
                        {
                            command.Parameters.Add(new OracleParameter("document_name", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("document_type", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("document_data", OracleDbType.Blob)).Value = null;

                        }

                        command.ExecuteNonQuery();
                    }

                    foreach (var t in proj.tasks)
                    {
                        if (t.id != 0)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "ADMIN_TASK_WAVE.UpdateTask";
                                command.CommandType = CommandType.StoredProcedure;

                                // Set the parameters for the stored procedure
                                command.Parameters.Add(new OracleParameter("task_id", OracleDbType.Int32)).Value = t.id;
                                command.Parameters.Add(new OracleParameter("descript", OracleDbType.NVarchar2)).Value = t.description;

                                command.ExecuteNonQuery();
                            }
                        }

                        else
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "ADMIN_TASK_WAVE.CreateTask";
                                command.CommandType = CommandType.StoredProcedure;

                                // Set the parameters for the stored procedure
                                command.Parameters.Add(new OracleParameter("project_id", OracleDbType.Int32)).Value = proj.id;
                                command.Parameters.Add(new OracleParameter("description", OracleDbType.NVarchar2)).Value = t.description;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return false;
            }
        }

        public Project GetCurrentTeamProject(int projectId, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            Project project = null;
            ProjectPhoto projectPhoto = null;
            ProjectDocument projectDocument = null;
            List<Task> tasks = new List<Task>();
            Task task = null;

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentTeamProject(:param1); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            while (reader.Read())
                            {
                                string? idPhotoValue = reader["idPh"].ToString();
                                if (idPhotoValue != null && idPhotoValue != "")
                                {
                                    projectPhoto = new ProjectPhoto()
                                    {
                                        id = int.Parse(reader["idPh"].ToString()),
                                        fileName = reader["titlePh"].ToString(),
                                        photo = (byte[])reader["photoPh"]
                                    };
                                }

                                string? idDocValue = reader["idDoc"].ToString();
                                if (idDocValue != null && idDocValue != "")
                                {
                                    projectDocument = new ProjectDocument()
                                    {
                                        id = int.Parse(reader["idPh"].ToString()),
                                        title = reader["titleDoc"].ToString(),
                                        type = reader["typeDoc"].ToString(),
                                        data = (byte[])reader["dataDoc"]
                                    };
                                }

                                string? idProjValue = reader["ID"].ToString();
                                if (idProjValue != null && idProjValue != "")
                                {
                                    project = new Project()
                                    {
                                        id = int.Parse(reader["ID"].ToString()),
                                        creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                        name = reader["NAME"].ToString(),
                                        toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                        fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                        description = reader["DESCRIPTION"].ToString(),
                                        type = ProjectENUM.INDIVIDUAL
                                    };
                                }

                                if (projectPhoto != null)
                                {
                                    project.photo = projectPhoto;
                                }

                                if (projectDocument != null)
                                {
                                    project.document = projectDocument;
                                }
                            }
                        }
                    }

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentTasks(:param1); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            while (reader.Read())
                            {
                                string? idTaskValue = reader["idTask"].ToString();
                                if (idTaskValue != null && idTaskValue != "")
                                {
                                    task = new Task()
                                    {
                                        id = int.Parse(reader["idTask"].ToString()),
                                        description = reader["TaskDescription"].ToString(),
                                        projectId = projectId
                                    };

                                    tasks.Add(task);
                                }
                            }

                            if (tasks.Count != 0)
                            {
                                project.tasks = tasks;
                            }
                        }
                    }

                    return project;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return null;
            }
        }

        public bool SendProject(int projectId, int userId, string description)
        {
            User user = _userRepository.GetUserById(userId);

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.SendProject";
                        command.CommandType = CommandType.StoredProcedure;

                        // Устанавливаем параметры процедуры
                        command.Parameters.Add(new OracleParameter("projectId", OracleDbType.Int32)).Value = projectId;
                        command.Parameters.Add(new OracleParameter("descrip", OracleDbType.NVarchar2)).Value = description;
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return false;
            }
        }

        public SendProject GetCurrentSendProject(int userId, int projectId)
        {
            User user = _userRepository.GetUserById(userId);

            ProjectPhoto projectPhoto = null;
            ProjectDocument projectDocument = null;
            List<Task> tasks = new List<Task>();
            Task task = null;
            SendProject sendProject = null;

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentSendProject(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {
                                while (reader.Read())
                                {
                                    string? idPhotoValue = reader["idPh"].ToString();
                                    if (idPhotoValue != null && idPhotoValue != "")
                                    {
                                        projectPhoto = new ProjectPhoto()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            fileName = reader["titlePh"].ToString(),
                                            photo = (byte[])reader["photoPh"]
                                        };
                                    }

                                    string? idDocValue = reader["idDoc"].ToString();
                                    if (idDocValue != null && idDocValue != "")
                                    {
                                        projectDocument = new ProjectDocument()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            title = reader["titleDoc"].ToString(),
                                            type = reader["typeDoc"].ToString(),
                                            data = (byte[])reader["dataDoc"]
                                        };
                                    }

                                    int? idSendProject = int.Parse(reader["ID_SEND_PROJECT"].ToString());

                                    if (idSendProject != null)
                                    {
                                        sendProject = new SendProject()
                                        {
                                            id = idSendProject,
                                            sender_user_id = int.Parse(reader["SENDER_USER_ID"].ToString()),
                                            dateSend = DateOnly.FromDateTime(DateTime.Parse(reader["DATE_SEND_PROJECT"].ToString())),
                                            description = reader["DESCRIPTION"].ToString(),
                                            project_id = int.Parse(reader["ID"].ToString()),
                                            creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                            project_name = reader["NAME"].ToString(),
                                            toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                            fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                            sendProjectDescription = reader["SEND_PROJECT_DESCRIPTION"].ToString()
                                        };

                                    }


                                    if (projectPhoto != null && sendProject != null)
                                    {
                                        sendProject.photo = projectPhoto;
                                    }

                                    if (projectDocument != null && sendProject != null)
                                    {
                                        sendProject.document = projectDocument;
                                    }
                                }
                            }
                        }
                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentTasks(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {
                                while (reader.Read())
                                {
                                    string? idTaskValue = reader["idTask"].ToString();
                                    if (idTaskValue != null && idTaskValue != "")
                                    {
                                        task = new Task()
                                        {
                                            id = int.Parse(reader["idTask"].ToString()),
                                            description = reader["TaskDescription"].ToString(),
                                            projectId = projectId
                                        };

                                        tasks.Add(task);
                                    }
                                }

                                if (tasks.Count != 0)
                                {
                                    sendProject.tasks = tasks;
                                }
                            }
                        }

                        return sendProject;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public ReadyProject GetCurrentReadyProject(int userId, int projectId)
        {
            User user = _userRepository.GetUserById(userId);
            ProjectPhoto projectPhoto = null;
            ProjectDocument projectDocument = null;
            List<Task> tasks = new List<Task>();
            Task task = null;
            ReadyProject readyProject = null;

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentReadyProject(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {

                                while (reader.Read())
                                {
                                    string? idPhotoValue = reader["idPh"].ToString();
                                    if (idPhotoValue != null && idPhotoValue != "")
                                    {
                                        projectPhoto = new ProjectPhoto()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            fileName = reader["titlePh"].ToString(),
                                            photo = (byte[])reader["photoPh"]
                                        };
                                    }

                                    string? idDocValue = reader["idDoc"].ToString();
                                    if (idDocValue != null && idDocValue != "")
                                    {
                                        projectDocument = new ProjectDocument()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            title = reader["titleDoc"].ToString(),
                                            type = reader["typeDoc"].ToString(),
                                            data = (byte[])reader["dataDoc"]
                                        };
                                    }

                                    int? idReadyProject = int.Parse(reader["ID_READY_PROJECT"].ToString());

                                    if (idReadyProject != null)
                                    {
                                        readyProject = new ReadyProject()
                                        {
                                            id = idReadyProject,
                                            perfomer_user_id = int.Parse(reader["COMPLETE_USER_ID"].ToString()),
                                            dateComplete = DateOnly.FromDateTime(DateTime.Parse(reader["DATE_COMPLETE_PROJECT"].ToString())),
                                            project_id = int.Parse(reader["ID"].ToString()),
                                            creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                            project_name = reader["NAME"].ToString(),
                                            toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                            fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                            description = reader["DESCRIPTION"].ToString()
                                        };

                                    }


                                    if (projectPhoto != null && readyProject != null)
                                    {
                                        readyProject.photo = projectPhoto;
                                    }

                                    if (projectDocument != null && readyProject != null)
                                    {
                                        readyProject.document = projectDocument;
                                    }
                                }
                            }
                        }

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetCurrentTasks(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = projectId;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {
                                while (reader.Read())
                                {
                                    string? idTaskValue = reader["idTask"].ToString();
                                    if (idTaskValue != null && idTaskValue != "")
                                    {
                                        task = new Task()
                                        {
                                            id = int.Parse(reader["idTask"].ToString()),
                                            description = reader["TaskDescription"].ToString(),
                                            projectId = projectId
                                        };

                                        tasks.Add(task);
                                    }
                                }

                                if (tasks.Count != 0)
                                {
                                    readyProject.tasks = tasks;
                                }
                            }
                        }

                        return readyProject;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public bool CreateTeamProject(Project proj, string login)
        {

            User artistUser = _userRepository.GetUserByLogin(login);

            RoleENUM currentRole;
            int resultValue;

            if (proj.type == Models.ENUMs.ProjectENUM.GROUP)
            {
                currentRole = RoleENUM.SUPERUSER;
            }
            else
            {
                currentRole = RoleENUM.USER;
            }

            try
            {
                using (var connection = _dbConnection.GetConnection(currentRole))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.CreateTeamProject ";
                        command.CommandType = CommandType.StoredProcedure;

                        // Set the parameters for the stored procedure
                        command.Parameters.Add(new OracleParameter("creator_id", OracleDbType.Int32)).Value = proj.creatorId;
                        command.Parameters.Add(new OracleParameter("project_name", OracleDbType.NVarchar2)).Value = proj.name;
                        command.Parameters.Add(new OracleParameter("toDate", OracleDbType.Date)).Value = new DateTime(proj.toDate.Year, proj.toDate.Month, proj.toDate.Day);
                        command.Parameters.Add(new OracleParameter("project_description", OracleDbType.NVarchar2)).Value = proj.description;
                        if (proj.photo != null)
                        {
                            command.Parameters.Add(new OracleParameter("photo_name", OracleDbType.NVarchar2)).Value = proj.photo.fileName;
                            command.Parameters.Add(new OracleParameter("photo_data", OracleDbType.Blob)).Value = proj.photo.photo;
                        }
                        else
                        {
                            command.Parameters.Add(new OracleParameter("photo_name", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("photo_data", OracleDbType.Blob)).Value = null;
                        }

                        if (proj.document != null)
                        {
                            command.Parameters.Add(new OracleParameter("document_name", OracleDbType.NVarchar2)).Value = proj.document.title;
                            command.Parameters.Add(new OracleParameter("document_type", OracleDbType.NVarchar2)).Value = proj.document.type;
                            command.Parameters.Add(new OracleParameter("document_data", OracleDbType.Blob)).Value = proj.document.data;
                        }
                        else
                        {
                            command.Parameters.Add(new OracleParameter("document_name", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("document_type", OracleDbType.NVarchar2)).Value = null;
                            command.Parameters.Add(new OracleParameter("document_data", OracleDbType.Blob)).Value = null;
                        }
                        if (artistUser != null)
                        {
                            command.Parameters.Add(new OracleParameter("artistId", OracleDbType.Int32)).Value = artistUser.id;
                        }
                        else
                        {
                            command.Parameters.Add(new OracleParameter("artistId", OracleDbType.Int32)).Value = -1;
                        }

                        // OUT parameter
                        OracleParameter resultParameter = new OracleParameter("result", OracleDbType.Int32);
                        resultParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(resultParameter);

                        command.ExecuteNonQuery();

                        // Retrieve the value of the OUT parameter
                        resultValue = Convert.ToInt32(Convert.ToString(resultParameter.Value));
                    }

                    if (proj.tasks != null)
                    {
                        foreach (var t in proj.tasks)
                        {
                            if (t.description == null)
                            {
                                continue;
                            }

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = "ADMIN_TASK_WAVE.CreateTask";
                                command.CommandType = CommandType.StoredProcedure;

                                // Set the parameters for the stored procedure
                                command.Parameters.Add(new OracleParameter("project_id ", OracleDbType.Int32)).Value = resultValue;
                                command.Parameters.Add(new OracleParameter("description", OracleDbType.NVarchar2)).Value = t.description;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (OracleException oe)
            {
                Console.WriteLine(oe.Message);
                return false;
            }
            return true;
        }

        public IEnumerable<SendProject> GetReviewProjects(int userId)
        {
            User user = _userRepository.GetUserById(userId);

            if (user != null)
            {
                try
                {
                    using (var connection = _dbConnection.GetConnection(user.Role))
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetReviewProjects(:param1); END;", connection))
                        {
                            command.CommandType = CommandType.Text;

                            // Устанавливаем параметры
                            command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add("param1", OracleDbType.Int32).Value = user.id;

                            command.ExecuteNonQuery();

                            // Получаем результат вызова функции
                            using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                            {

                                List<SendProject> sendProjects = new List<SendProject>();
                                ProjectPhoto projectPhoto = null;
                                ProjectDocument projectDocument = null;
                                SendProject readyProject = null;

                                while (reader.Read())
                                {
                                    string? idPhotoValue = reader["idPh"].ToString();
                                    if (idPhotoValue != null && idPhotoValue != "")
                                    {
                                        projectPhoto = new ProjectPhoto()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            fileName = reader["titlePh"].ToString(),
                                            photo = (byte[])reader["photoPh"]
                                        };
                                    }

                                    string? idDocValue = reader["idDoc"].ToString();
                                    if (idDocValue != null && idDocValue != "")
                                    {
                                        projectDocument = new ProjectDocument()
                                        {
                                            id = int.Parse(reader["idPh"].ToString()),
                                            title = reader["titleDoc"].ToString(),
                                            type = reader["typeDoc"].ToString(),
                                            data = (byte[])reader["dataDoc"]
                                        };
                                    }

                                    int? idSendProject = int.Parse(reader["ID_SEND_PROJECT"].ToString());

                                    if (idSendProject != null)
                                    {
                                        readyProject = new SendProject()
                                        {
                                            id = idSendProject,
                                            sender_user_id = int.Parse(reader["SENDER_USER_ID"].ToString()),
                                            dateSend = DateOnly.FromDateTime(DateTime.Parse(reader["DATE_SEND_PROJECT"].ToString())),
                                            description = reader["SEND_PROJECT_DESCRIPTION"].ToString(),
                                            project_id = int.Parse(reader["ID"].ToString()),
                                            creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                            project_name = reader["NAME"].ToString(),
                                            toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                            fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                        };

                                    }


                                    if (projectPhoto != null && readyProject != null)
                                    {
                                        readyProject.photo = projectPhoto;
                                    }

                                    if (projectDocument != null && readyProject != null)
                                    {
                                        readyProject.document = projectDocument;
                                    }

                                    sendProjects.Add(readyProject);
                                }

                                if (sendProjects.Count() == 0)
                                {
                                    return null;
                                }

                                return sendProjects;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            return null;
        }

        public bool AcceptProject(int projectId, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.AcceptProject";
                        command.CommandType = CommandType.StoredProcedure;

                        // Устанавливаем параметры процедуры
                        command.Parameters.Add(new OracleParameter("projectId", OracleDbType.Int32)).Value = projectId;
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return false;
            }
        }

        public bool RejectProject(int projectId, int userId)
        {
            User user = _userRepository.GetUserById(userId);

            try
            {
                using (var connection = _dbConnection.GetConnection(user.Role))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ADMIN_TASK_WAVE.RejectProject";
                        command.CommandType = CommandType.StoredProcedure;

                        // Устанавливаем параметры процедуры
                        command.Parameters.Add(new OracleParameter("projectId", OracleDbType.Int32)).Value = projectId;
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return false;
            }
        }

        public IEnumerable<Project> GetAllProjects()
        {
            try
            {
                using (var connection = _dbConnection.GetConnection(RoleENUM.ADMIN))
                {
                    connection.Open();

                    using (OracleCommand command = new OracleCommand("BEGIN :result := ADMIN_TASK_WAVE.GetAllProjects(); END;", connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Устанавливаем параметры
                        command.Parameters.Add("result", OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue;

                        command.ExecuteNonQuery();

                        // Получаем результат вызова функции
                        using (OracleDataReader reader = ((OracleRefCursor)command.Parameters["result"].Value).GetDataReader())
                        {
                            List<Project> projects = new List<Project>();
                            ProjectPhoto projectPhoto = null;
                            ProjectDocument projectDocument = null;

                            while (reader.Read())
                            {
                                string? idPhotoValue = reader["idPh"].ToString();
                                if (idPhotoValue != null && idPhotoValue != "")
                                {
                                    projectPhoto = new ProjectPhoto()
                                    {
                                        id = int.Parse(reader["idPh"].ToString()),
                                        fileName = reader["titlePh"].ToString(),
                                        photo = (byte[])reader["photoPh"]
                                    };
                                }

                                string? idDocValue = reader["idDoc"].ToString();
                                if (idDocValue != null && idDocValue != "")
                                {
                                    projectDocument = new ProjectDocument()
                                    {
                                        id = int.Parse(reader["idPh"].ToString()),
                                        title = reader["titleDoc"].ToString(),
                                        type = reader["typeDoc"].ToString(),
                                        data = (byte[])reader["dataDoc"]
                                    };
                                }

                                string? idProjValue = reader["ID"].ToString();
                                Project project = null;
                                if (idProjValue != null && idProjValue != "")
                                {
                                    project = new Project()
                                    {
                                        id = int.Parse(reader["ID"].ToString()),
                                        creatorId = int.Parse(reader["CREATOR_ID"].ToString()),
                                        name = reader["NAME"].ToString(),
                                        toDate = DateOnly.FromDateTime(DateTime.Parse(reader["TO_DATE"].ToString())),
                                        fromDate = DateOnly.FromDateTime(DateTime.Parse(reader["FROM_DATE"].ToString())),
                                        description = reader["DESCRIPTION"].ToString()
                                    };

                                    if (reader["TYPE"].ToString() == "individual")
                                    {
                                        project.type = ProjectENUM.INDIVIDUAL;
                                    }
                                    else
                                    {
                                        project.type = ProjectENUM.GROUP;
                                    }
                                }


                                if (projectPhoto != null && project != null)
                                {
                                    project.photo = projectPhoto;
                                }

                                if (projectDocument != null && project != null)
                                {
                                    project.document = projectDocument;
                                }

                                projects.Add(project);

                                projectPhoto = null;
                                projectDocument = null;
                            }

                            if (projects.Count() == 0)
                            {
                                return null;
                            }

                            return projects;
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
    }
}
