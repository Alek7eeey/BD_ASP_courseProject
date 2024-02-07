using Microsoft.AspNetCore.Mvc;
using TaskWave_MVC.DTOs;
using TaskWave_MVC.Models;
using TaskWave_MVC.Services;
using TaskWave_MVC.Services.Implementation.Default;

namespace TaskWave_MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly JwtService _jwtService;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;
        public UserController(JwtService service, ProjectService projectService, UserService userService)
        {
            this._jwtService = service;
            this._projectService = projectService;
            this._userService = userService;
        }

        [HttpGet]
        public IActionResult MainPage()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("../User/MainPage");
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetTeamProjects()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                IEnumerable<Project>? pr = _projectService.GetTeamProjects(Convert.ToInt32(id));
                if (pr != null)
                {
                    pr = pr.OrderByDescending(p => p.fromDate);
                }
                return Ok(pr);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult CreateProject()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("../User/CreateProject");
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult CreateProject([FromForm] CreateProjectDTO project)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (project.name == null || project.description == null)
            {
                return BadRequest();
            }

            if (role == "USER")
            {
                if (_projectService.CreateProject(project, Convert.ToInt32(id)))
                {
                    return View("../User/CreateProject");
                }
                else
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult TeamProjects()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("TeamProject");
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult SendProjects()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("SendProject");
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetMyProjects()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                IEnumerable<Project>? pr = _projectService.GetMyProjects(Convert.ToInt32(id));
                if (pr != null)
                {
                    pr = pr.OrderByDescending(p => p.fromDate);
                }
                return Ok(pr);

            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetMySendProjects()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                IEnumerable<SendProject>? pr = _projectService.GetMySendProjects(Convert.ToInt32(id));
                if (pr != null)
                {
                    pr = pr.OrderByDescending(p => p.dateSend);
                }
                return Ok(pr);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult ReadyProjects()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("ReadyProject");
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetMyReadyProjects()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                IEnumerable<ReadyProject>? pr = _projectService.GetMyReadyProjects(Convert.ToInt32(id));
                if (pr != null)
                {
                    pr = pr.OrderByDescending(p => p.dateComplete);
                }
                return Ok(pr);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult Stat()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("Stat");
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetStat()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                List<Stat>? stat = (List<Stat>)_projectService.GetStat(Convert.ToInt32(id));
                List<Stat>? firstFiveStats = stat?.Take(6).ToList();

                return Ok(firstFiveStats);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult SettingAccount()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                return View("SettingAccount");
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetDescriptionAccount()
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                UserDescription user = _userService.GetDescriptionAccount(Convert.ToInt32(id));

                return Ok(user);
            }

            return Unauthorized();
        }

        [Route("User/GetUserImage/{fileName}")]
        public IActionResult GetUserImage(string fileName)
        {
            var imagePath = Path.Combine("C:\\Users\\aleks\\AppData\\Local\\Temp\\", fileName);

            if (System.IO.File.Exists(imagePath))
            {
                var imageBytes = System.IO.File.ReadAllBytes(imagePath);
                return File(imageBytes, "image/jpeg"); // Измените "image/jpeg" на соответствующий тип изображения
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult ChangePasswordLogin([FromForm] SignInDTO user)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                if (_userService.ChangeDescriptionAccount(user, Convert.ToInt32(id)))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult ChangeUserDescription([FromForm] UserDescriptionDTO userDescription)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                if (_userService.ChangeDescriptionAccount(userDescription, Convert.ToInt32(id)))
                {
                    return Ok();
                }

                else
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetMyProjectDescription(int idPr)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                Project pr = _projectService.GetProject(idPr, Convert.ToInt32(id));
                return View("MyProjectDescr", pr);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult CompleteProject(int idPr)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                if (_projectService.CompleteProject(idPr, Convert.ToInt32(id)))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpDelete]
        public IActionResult DeleteProject(int idPr)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                if (_projectService.DeleteProject(idPr, Convert.ToInt32(id)))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult UpdateOwnProject(int idPr)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                Project pr = _projectService.GetProject(idPr, Convert.ToInt32(id));
                return View("UpdateOwnProject", pr);
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult UpdateMyProject([FromForm] UpdateProjectDTO project)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (project.name == null || project.description == null)
            {
                return BadRequest();
            }

            if (role == "USER")
            {
                if (_projectService.UpdateProject(project, Convert.ToInt32(id)))
                {
                    return View("MainPage");
                }
                else
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetTeamProjectDescription(int idPr)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                Project pr = _projectService.GetTeamProject(idPr, Convert.ToInt32(id));
                return View("TeamProjectDescr", pr);
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult SendTeamProject([FromForm] int idPr, string description)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                if (_projectService.SendProject(idPr, Convert.ToInt32(id), description))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetSendProjectDescription(int idPr)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                SendProject pr = _projectService.GetSendProject(idPr, Convert.ToInt32(id));
                return View("SendProjectDescr", pr);
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetCompleteProjectDescription(int idPr)
        {
            var id = HttpContext.Items["UserId"] as string;
            var role = HttpContext.Items["UserRole"] as string;

            if (role == "USER")
            {
                ReadyProject pr = _projectService.GetReadyProject(idPr, Convert.ToInt32(id));
                return View("ReadyProjectDescr", pr);
            }

            return Unauthorized();
        }
    }
}
