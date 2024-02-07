using System.ComponentModel.DataAnnotations;

namespace TaskWave_MVC.DTOs
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Поле 'Login' обязательно для заполнения")]
        [MaxLength(60, ErrorMessage = "Максимальная длина 'Login' составляет 60 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Поле 'Password' обязательно для заполнения")]
        [MaxLength(100, ErrorMessage = "Максимальная длина 'Password' составляет 100 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле 'Email' обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
        public string Email { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальная длина 'Description' составляет 100 символов")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Поле 'Role' обязательно для заполнения")]
        public string Role { get; set; }
    }
}
