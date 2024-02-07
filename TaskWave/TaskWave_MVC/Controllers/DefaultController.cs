using Microsoft.AspNetCore.Mvc;
using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;
using TaskWave_MVC.Services;
using TaskWave_MVC.Services.Implementation.Default;

namespace TaskWave_MVC.Controllers
{
    public class DefaultController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegisterUserService _register;
        private readonly SignInUserService _signIn;
        private readonly JwtService _jwtService;

        public DefaultController(ILogger<HomeController> logger, RegisterUserService register, SignInUserService signIn, JwtService jwt)
        {
            _logger = logger;
            _register = register;
            _signIn = signIn;
            _jwtService = jwt;
        }


        [HttpGet]
        public IActionResult SignIn()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("../User/MainPage");
            }

            return View("../Default/SignIn");
        }

        [HttpPost]
        public IActionResult SignIn([FromBody] SignInDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, enter correct input data in the fields!");
            }

            User authUser = _signIn.SignInUser(user);

            if (authUser == null)
            {
                return Unauthorized("Incorrect password or/and login!");
            }

            var token = _jwtService.GenerateJwtToken(authUser.id, authUser.Role);
            return Ok(new { token });

        }

        [HttpGet]
        public IActionResult Register()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("../User/MainPage");
            }

            return View("Register");

        }

        [HttpGet]
        public IActionResult Exit()
        {
            HttpContext.Items["UserId"] = null;
            HttpContext.Items["UserRole"] = null;

            return Ok("Good");
        }

        [HttpPost]
        public IActionResult Register([FromBody] DTOs.RegisterUserDTO newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please, enter correct input data in the fields!");
            }

            if (newUser.Description == null || newUser.Description == "")
            {
                return BadRequest("The 'Description' field is required for the superuser!");
            }

            User user = _register.RegisterNewUser(newUser);

            if (user == null)
            {
                return BadRequest("User with such login has already existed!");
            }

            var token = _jwtService.GenerateJwtToken(user.id, user.Role);
            return Ok(new { token });
        }
    }
}
