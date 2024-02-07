using Microsoft.AspNetCore.Mvc;
using TaskWave_MVC.Models;
using TaskWave_MVC.Services.Implementation.Default;
using X.PagedList;

namespace TaskWave_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserService _userService;
        private readonly ProjectService _projectService;
        public AdminController(UserService userService, ProjectService projectService)
        {
            this._userService = userService;
            _projectService = projectService;
        }
        public IActionResult MainPage(int? page)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "ADMIN")
            {
                List<User> users = _userService.GetAllUsers();

                if (users != null)
                {
                    users = users.OrderBy(u => u.id).ToList();
                    // Устанавливаем количество элементов на странице
                    int pageSize = 10;

                    // Получаем номер текущей страницы из параметра запроса
                    int pageNumber = (page ?? 1);

                    // Преобразовываем список в PagedList
                    IPagedList<User> pagedUsers = users.ToPagedList(pageNumber, pageSize);

                    return View("MainPage", pagedUsers);
                }
            }

            return Unauthorized();
        }

        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            var idUser = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "ADMIN")
            {
                if (_userService.DeleteUser(id))
                {
                    return Ok();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult AllProjects(int? page)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "ADMIN")
            {
                List<Project> pr = (List<Project>)_projectService.GetAllProjects();

                if (pr != null)
                {
                    pr = pr.OrderBy(u => u.id).ToList();

                    // Устанавливаем количество элементов на странице
                    int pageSize = 5;

                    // Получаем номер текущей страницы из параметра запроса
                    int pageNumber = (page ?? 1);

                    // Преобразовываем список в PagedList
                    IPagedList<Project> pagedUsers = pr.ToPagedList(pageNumber, pageSize);

                    return View("AllProjects", pagedUsers);
                }
                else
                {
                    return View("AllProjects", null);
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult Exit()
        {
            HttpContext.Items["UserId"] = null;
            HttpContext.Items["UserRole"] = null;

            return Ok("Good");
        }

        [HttpGet]
        public IActionResult ConfirmUser(int? page)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "ADMIN")
            {
                List<User> users = _userService.GetAllUnaffectedUser();

                if (users != null)
                {
                    users = users.OrderBy(u => u.id).ToList();

                    // Устанавливаем количество элементов на странице
                    int pageSize = 5;

                    // Получаем номер текущей страницы из параметра запроса
                    int pageNumber = (page ?? 1);

                    // Преобразовываем список в PagedList
                    IPagedList<User> pagedUsers = users.ToPagedList(pageNumber, pageSize);

                    return View("ConfirmUser", pagedUsers);
                }
                else
                {
                    return View("ConfirmUser", null);
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult Confirm(int id)
        {
            var idUser = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "ADMIN")
            {
                if (_userService.ConfirmUser(id))
                {
                    List<User> users = _userService.GetAllUnaffectedUser();
                    if (users != null)
                    {
                        users = users.OrderBy(u => u.id).ToList();
                        return View("ConfirmUser", users);
                    }
                    else
                    {
                        return View("ConfirmUser", null);
                    }
                }
            }

            return Unauthorized();
        }
    }
}
