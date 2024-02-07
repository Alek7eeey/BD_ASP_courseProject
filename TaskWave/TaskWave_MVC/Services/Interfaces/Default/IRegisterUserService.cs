using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;

namespace TaskWave_MVC.Services.Interfaces.Default
{
    public interface IRegisterUserService
    {
        public User RegisterNewUser(RegisterUserDTO registerUser);

    }
}
