using TaskWave_MVC.Data.Repository;
using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;
using TaskWave_MVC.Models.ENUMs;
using TaskWave_MVC.Services.Interfaces.Default;

namespace TaskWave_MVC.Services.Implementation.Default
{
    public class RegisterUserService : IRegisterUserService
    {
        private UserRepository _repoUser;
        public RegisterUserService(UserRepository repo)
        {
            this._repoUser = repo;
        }

        public User RegisterNewUser(RegisterUserDTO registerUser)
        {

            switch (registerUser.Role.ToLower())
            {
                case "user":
                    {
                        return RegisterStandartUser(registerUser);
                    }

                case "superuser":
                    {
                        return RegisterSuperUser(registerUser);

                    }

                default:
                    {
                        return null;
                    }
            }

        }

        private User RegisterStandartUser(RegisterUserDTO registerUser)
        {
            string descr = "";

            if (registerUser.Description != null && registerUser.Description != "")
            {
                descr = registerUser.Description;
            }

            User user = new User()
            {
                Login = registerUser.Login,
                Password = registerUser.Password,
                Email = registerUser.Email,
                Description = descr,
                Role = RoleENUM.USER
            };

            return _repoUser.CreateStandartUser(user);
        }

        private User RegisterSuperUser(RegisterUserDTO registerUser)
        {

            if (registerUser.Description == null)
            {
                return null;
            }

            User user = new User()
            {
                Login = registerUser.Login,
                Password = registerUser.Password,
                Email = registerUser.Email,
                Description = registerUser.Description,
                Role = RoleENUM.SUPERUSER
            };

            return _repoUser.CreateSuperUser(user);
        }
    }
}
