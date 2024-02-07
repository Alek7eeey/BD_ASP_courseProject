using TaskWave_MVC.Data.Repository;
using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;

namespace TaskWave_MVC.Services.Implementation.Default
{
    public class UserService
    {

        private readonly ProjectRepository _projectRepository;
        private readonly UserRepository _repoUser;
        public UserService(ProjectRepository projectRepository, UserRepository userRep)
        {
            this._projectRepository = projectRepository;
            this._repoUser = userRep;
        }

        public UserDescription GetDescriptionAccount(int userId)
        {
            UserDescription userDescription = _projectRepository.GetDescriptionAccount(userId);

            if (userDescription.image != null && userDescription.image.Length > 0)
            {
                string path = SaveImageToTempFolder(userDescription.image, userDescription.titlePhoto);
            }

            return userDescription;
        }

        private static string SaveImageToTempFolder(byte[] imageBytes, string fileName)
        {
            try
            {
                string tempFolder = Path.GetTempPath();
                string tempFilePath = Path.Combine(tempFolder, fileName);

                File.WriteAllBytes(tempFilePath, imageBytes);

                return tempFilePath;
            }
            catch (Exception ex)
            {
                // Обработка ошибок, например, запись в лог
                Console.WriteLine($"Error saving image to temp folder: {ex.Message}");
                return null;
            }
        }

        public bool ChangeDescriptionAccount(SignInDTO user, int idUser)
        {
            return _repoUser.ChangeUser(user, idUser);
        }

        public bool ChangeDescriptionAccount(UserDescriptionDTO user, int idUser)
        {
            return _repoUser.ChangeDescriptionAccount(user, idUser);
        }

        public List<User> GetAllUsers()
        {
            return _repoUser.GetAllUsers();
        }

        public bool DeleteUser(int id)
        {
            return _repoUser.DeleteUser(id);
        }

        public List<User> GetAllUnaffectedUser()
        {
            return _repoUser.GetAllUnaffectedUser();
        }

        public bool ConfirmUser(int id)
        {
            return _repoUser.ConfirmUser(id);
        }
    }
}
