using TaskWave_MVC.Data.Repository;
using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;

namespace TaskWave_MVC.Services.Implementation.Default
{
    public class SignInUserService
    {
        private UserRepository _repoUser;
        public SignInUserService(UserRepository repo)
        {
            this._repoUser = repo;
        }

        public User SignInUser(SignInDTO user)
        {
            return _repoUser.SignInUser(user);
        }
    }
}
