using TaskWave_MVC.Data.Repository;
using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;

namespace TaskWave_MVC.Services.Implementation.Default
{
    public class ProjectService
    {
        private readonly ProjectRepository _projectRepository;
        private readonly UserRepository _repoUser;
        public ProjectService(ProjectRepository projectRepository, UserRepository userRep)
        {
            this._projectRepository = projectRepository;
            this._repoUser = userRep;
        }

        public bool CreateProject(CreateProjectDTO proj, int id)
        {
            ProjectPhoto projectPhoto = null;
            ProjectDocument projectDocument = null;
            List<Models.Task> tasks = null;

            if (proj.tasks != null && proj.tasks.Count > 0)
            {
                tasks = new List<Models.Task>();

                foreach (var task in proj.tasks)
                {
                    tasks.Add(new Models.Task()
                    {
                        description = task
                    });
                }
            }

            if (proj.document != null && proj.document.Length > 0)
            {
                byte[] fileBytes;

                using (var stream = new MemoryStream())
                {
                    proj.document.CopyTo(stream);
                    fileBytes = stream.ToArray();
                }

                projectDocument = new ProjectDocument()
                {
                    data = fileBytes,
                    title = proj.document.FileName,
                    type = Path.GetExtension(proj.document.FileName)
                };
            }

            if (proj.photo != null && proj.photo.Length > 0)
            {
                byte[] fileBytes;

                using (var stream = new MemoryStream())
                {
                    proj.photo.CopyTo(stream);
                    fileBytes = stream.ToArray();
                }

                projectPhoto = new ProjectPhoto()
                {
                    fileName = proj.photo.FileName,
                    photo = fileBytes
                };
            }

            Project project = new Project()
            {
                name = proj.name,
                toDate = proj.toDate,
                description = proj.description,
                creatorId = id,
                type = Models.ENUMs.ProjectENUM.INDIVIDUAL,
                photo = projectPhoto,
                document = projectDocument,
                tasks = tasks
            };

            return _projectRepository.CreateProject(project);
        }

        public IEnumerable<Project> GetMyProjects(int userId)
        {
            List<Project> projects = (List<Project>)_projectRepository.GetMyProjects(userId);
            return projects;
        }

        public IEnumerable<Project> GetTeamProjects(int userId)
        {
            List<Project> projects = (List<Project>)_projectRepository.GetGroupProjects(userId);
            return projects;
        }

        public IEnumerable<SendProject> GetMySendProjects(int userId)
        {
            List<SendProject> projects = (List<SendProject>)_projectRepository.GetSendProjects(userId);
            return projects;
        }

        public IEnumerable<ReadyProject> GetMyReadyProjects(int userId)
        {
            List<ReadyProject> projects = (List<ReadyProject>)_projectRepository.GetReadyProjects(userId);
            return projects;
        }

        public IEnumerable<Stat> GetStat(int userId)
        {
            return _projectRepository.GetStat(userId);
        }

        public Project GetProject(int idProj, int idUser)
        {
            return _projectRepository.GetCurrentProject(idProj, idUser);
        }

        public bool CompleteProject(int idProj, int idUser)
        {
            return _projectRepository.CompleteProject(idProj, idUser);
        }

        public bool DeleteProject(int idProj, int idUser)
        {
            return _projectRepository.DeleteProject(idProj, idUser);
        }

        public bool UpdateProject(UpdateProjectDTO proj, int id)
        {
            ProjectPhoto projectPhoto = null;
            ProjectDocument projectDocument = null;
            List<Models.Task> tasks = null;

            if (proj.tasks != null && proj.tasks.Count > 0)
            {
                tasks = new List<Models.Task>();

                foreach (var task in proj.tasks)
                {
                    tasks.Add(new Models.Task()
                    {
                        id = task.Id,
                        description = task.Description
                    });
                }
            }

            if (proj.document != null && proj.document.Length > 0)
            {
                byte[] fileBytes;

                using (var stream = new MemoryStream())
                {
                    proj.document.CopyTo(stream);
                    fileBytes = stream.ToArray();
                }

                projectDocument = new ProjectDocument()
                {
                    data = fileBytes,
                    title = proj.document.FileName,
                    type = Path.GetExtension(proj.document.FileName)
                };
            }

            if (proj.photo != null && proj.photo.Length > 0)
            {
                byte[] fileBytes;

                using (var stream = new MemoryStream())
                {
                    proj.photo.CopyTo(stream);
                    fileBytes = stream.ToArray();
                }

                projectPhoto = new ProjectPhoto()
                {
                    fileName = proj.photo.FileName,
                    photo = fileBytes
                };
            }

            Project project = new Project()
            {
                id = proj.id,
                name = proj.name,
                toDate = proj.toDate,
                description = proj.description,
                creatorId = id,
                type = Models.ENUMs.ProjectENUM.INDIVIDUAL,
                photo = projectPhoto,
                document = projectDocument,
                tasks = tasks
            };

            return _projectRepository.UpdateProject(project, id);
        }

        public Project GetTeamProject(int idProj, int idUser)
        {
            return _projectRepository.GetCurrentTeamProject(idProj, idUser);
        }

        public bool SendProject(int idProj, int idUser, string description)
        {
            return _projectRepository.SendProject(idProj, idUser, description);
        }

        public SendProject GetSendProject(int idProj, int idUser)
        {
            return _projectRepository.GetCurrentSendProject(idUser, idProj);
        }

        public ReadyProject GetReadyProject(int idProj, int idUser)
        {
            return _projectRepository.GetCurrentReadyProject(idUser, idProj);
        }

        public bool CreateTeamProject(CreateTeamProjectDTO proj, int id)
        {
            ProjectPhoto projectPhoto = null;
            ProjectDocument projectDocument = null;
            List<Models.Task> tasks = null;

            if (proj.tasks != null && proj.tasks.Count > 0)
            {
                tasks = new List<Models.Task>();

                foreach (var task in proj.tasks)
                {
                    tasks.Add(new Models.Task()
                    {
                        description = task
                    });
                }
            }

            if (proj.document != null && proj.document.Length > 0)
            {
                byte[] fileBytes;

                using (var stream = new MemoryStream())
                {
                    proj.document.CopyTo(stream);
                    fileBytes = stream.ToArray();
                }

                projectDocument = new ProjectDocument()
                {
                    data = fileBytes,
                    title = proj.document.FileName,
                    type = Path.GetExtension(proj.document.FileName)
                };
            }

            if (proj.photo != null && proj.photo.Length > 0)
            {
                byte[] fileBytes;

                using (var stream = new MemoryStream())
                {
                    proj.photo.CopyTo(stream);
                    fileBytes = stream.ToArray();
                }

                projectPhoto = new ProjectPhoto()
                {
                    fileName = proj.photo.FileName,
                    photo = fileBytes
                };
            }

            Project project = new Project()
            {
                name = proj.name,
                toDate = proj.toDate,
                description = proj.description,
                creatorId = id,
                type = Models.ENUMs.ProjectENUM.GROUP,
                photo = projectPhoto,
                document = projectDocument,
                tasks = tasks
            };

            return _projectRepository.CreateTeamProject(project, proj.userLogin);
        }

        public IEnumerable<SendProject> GetReviewProjects(int userId)
        {
            List<SendProject> projects = (List<SendProject>)_projectRepository.GetReviewProjects(userId);
            return projects;
        }

        public bool AcceptProject(int idProj, int idUser)
        {
            return _projectRepository.AcceptProject(idProj, idUser);
        }

        public bool RejectProject(int idProj, int idUser)
        {
            return _projectRepository.RejectProject(idProj, idUser);
        }

        public IEnumerable<Project> GetAllProjects()
        {
            return _projectRepository.GetAllProjects();
        }
    }
}
