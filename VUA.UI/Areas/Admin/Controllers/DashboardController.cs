﻿
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using VUA.Core.IRepositories;
using VUA.Core.Models;
using VUA.Core.Models.ViewModels;
using VUA.Core.Models.ViewModels.Admin;
using VUA.EF;
using VUA.EF.Repositories;

namespace VUA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        #region configration
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<AppllicationUser> _userManager;
        private IBaseRepository<Course> _courseRepository;
        private IAccountRepositorory _accountRepositorory;
        private IWebHostEnvironment _webHostEnvironment;
       


        public DashboardController(RoleManager<IdentityRole> roleManager,
            UserManager<AppllicationUser> userManager,
            IBaseRepository<Course> courseRepository,
            IAccountRepositorory accountRepositorory,
            IWebHostEnvironment webHostEnvironment
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _courseRepository = courseRepository;
            _accountRepositorory = accountRepositorory;
            _webHostEnvironment = webHostEnvironment;
           
        }
        #endregion

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetStudants()
        {
            return View(_userManager.Users.Where(x => x.StudantOrTeacher == "Studant" && x.IsCompleted == true && x.IsApprovedUser==false));
        }
        [HttpGet]
        public IActionResult GetApprovedStudants()
        {
            return View(_userManager.Users.Where(x => x.StudantOrTeacher == "Studant" && x.IsCompleted == true && x.IsApprovedUser == true));
        }
        #region Rols
        [HttpGet]
        public IActionResult RolesList()
        {
            return View(_roleManager.Roles);
        }
        [HttpGet]
        public IActionResult CreatRole() { return View(); }

        [HttpPost]
        public async Task<IActionResult> CreatRole(CreatRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);

                }
                return View(model);

            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var model = new EditRoleViewModel
            {
                RoleId = role!.Id,
                RoleName = role.Name
            };
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name!))
                {
                    model.Users!.Add(user.Email!);
                }
            }
            return View(model);
        }
        [HttpPost]

        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId!);
                role!.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }
            return View(model);
        }
        [HttpGet]

        public async Task<IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }
        [HttpPost]

        public async Task<IActionResult> DeleteRole(Guid id)
        {
            if (id.ToString() == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id.ToString());
            var result = await _roleManager.DeleteAsync(role!);
            if (result.Succeeded)
            {
                return RedirectToAction("RolesList");
            }
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError(err.Code, err.Description);
            }
            return View(role);
        }
        [HttpGet]
        public async Task<IActionResult> UserRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            var userRoleViewModels = new List<UserRoleViewModel>();
            var HiÇ = _userManager.Users.Where(u => u.StudantOrTeacher == "Other");
            if(role!.Name == "Admin")
            {
                foreach (var user in HiÇ)
                {
                    var model = new UserRoleViewModel
                    {
                        UserName = user.UserName,
                        UserId = user.Id
                    };
                    if (await _userManager.IsInRoleAsync(user, role!.Name!))
                    {
                        model.IsSelected = true;
                    }
                    else
                    {
                        model.IsSelected = false;
                    }
                    userRoleViewModels.Add(model);
                }
                return View(userRoleViewModels);
            }
            foreach (var user in _userManager.Users)
            {
                var model = new UserRoleViewModel
                {
                    UserName = user.UserName,
                    UserId = user.Id
                };
                if (await _userManager.IsInRoleAsync(user, role!.Name!))
                {
                    model.IsSelected = true;
                }
                else
                {
                    model.IsSelected = false;
                }
                userRoleViewModels.Add(model);
            }
            return View(userRoleViewModels);
        }
        [HttpPost]
        public async Task<IActionResult> UserRole(string id, List<UserRoleViewModel> models)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            IdentityResult result = null!;
            for (var i = 0; i < models.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(models[i].UserId!);

                if (models[i].IsSelected && !(await _userManager.IsInRoleAsync(user!, role.Name!)))
                {
                    result = await _userManager.AddToRoleAsync(user!, role.Name!);
                }
                else if (!(models[i].IsSelected) && await _userManager.IsInRoleAsync(user!, role.Name!))
                {
                    result = await _userManager.RemoveFromRoleAsync(user!, role.Name!);
                }
                return View(models);
            }
            if (result!.Succeeded)
            {
                return RedirectToAction("EditRole", new { id = id });
            }
            return View(models);
        }
        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.CourseImage != null)
                {
                    string folder = "uploads/Courses/";
                    var course = new Course
                    {
                        CourseName = model.CourseName,
                        CourseDuration = model.CourseDuration,
                        CourseDescription = model.CourseDescription,
                        CoursePrice = model.CoursePrice,
                    };
                    course!.CourseImage = await _accountRepositorory.UploadImag(folder, model.CourseImage, _webHostEnvironment.WebRootPath);
                    _courseRepository.Add(course);
                    ViewBag.Added = true;
                    return View("flotingScreen");
                }
                return View(model);
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult flotingScreen()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Courses()
        {
            return View(_courseRepository.GetAll().Where(c => c.CourseId != 1));
        }
        [HttpPost]
        public IActionResult Courses(string term)
        {
            if (term != null)
            {
                var result = _courseRepository.GetAll().Where(c => c.CourseName!.Contains(term));
                return View(result);
            }
            return View(_courseRepository.GetAll().Where(c => c.CourseId != 1));
        }


        [HttpGet]
        public IActionResult EditCourse(int id)
        {
            var course = _courseRepository.Find(id);
            var model = new CreateCourseViewModel
            {
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                CourseDuration = course.CourseDuration,
                CoursePrice = course.CoursePrice,
                SubDescription = course.SubDescription,


            };
            return View(model);
        }
        public async Task<IActionResult> EditCourse(EditCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                string folder = "uploads/ProfilsPuctuers/Chenged/";
                var course = _courseRepository.Find(model.Id);
                course.CourseName = model.CourseName;
                course.CourseDescription = model.CourseDescription;
                course.CourseDuration = model.CourseDuration!.TrimStart();
                course.CoursePrice = model.CoursePrice;
                course.SubDescription = model.SubDescription;

                if (model.CourseImage != null)
                {
                    course.CourseImage = await _accountRepositorory.UploadImag(folder, model.CourseImage, _webHostEnvironment.WebRootPath);
                }
                _courseRepository.Update(course);
                return RedirectToAction("Courses");
            }
            return View(model);


            #endregion
        }
        [HttpGet]
        public  IActionResult DeleteCourse(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = _courseRepository.Find(id);
            return View(course);
        }

        [HttpPost]
        public  IActionResult DeleteCourse(int id , int d)
        {
            if (id.ToString() == null)
            {
                return NotFound();
            }
            var course = _courseRepository.Find(id);
             _courseRepository.Delete(course!);
            return RedirectToAction("Courses");
        }
        [HttpGet]
        public IActionResult MoreCourse(int id)
        {
            var course = _courseRepository.Find(id);
            
            return View(course);
        }
        [HttpGet]
        public IActionResult UserDetilse(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            return View(user);
            
        }
		public async Task<IActionResult> Logout()
		{
			await _accountRepositorory.SignOutAcync();
			return RedirectToAction("Login", "Account", new { area = "Identity" });
		}
        [HttpGet]
        public IActionResult GetTeachers()
        {
			return View(_userManager.Users.Where(x => x.StudantOrTeacher == "Teacher" && x.IsCompleted == true && x.IsApprovedUser == false));
		}
        [HttpGet]
        public IActionResult GetApprovedTeachers()
        {
            return View(_userManager.Users.Where(x => x.StudantOrTeacher == "Teacher" && x.IsCompleted == true && x.IsApprovedUser == true));
        }

        public async Task<IActionResult> ApproveUser(string id)
        {
            var user = _accountRepositorory.FindUserByIdAsync(id).Result;
            
            user.IsApprovedUser = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (user.StudantOrTeacher == "Studant")
                {
                    return RedirectToAction("GetStudants");
                }
                else
                {
                    return RedirectToAction("GetTeachers");
                }
               
            }
            return View();
            
        }

        public async Task<IActionResult> RejecteUser(string id)
        {
            var user = _accountRepositorory.FindUserByIdAsync(id).Result;

            user.IsApprovedUser = false;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (user.StudantOrTeacher == "Studant")
                {
                    return RedirectToAction("GetStudants");
                }
                else
                {
                    return RedirectToAction("GetTeachers");
                }

            }
            return View();

        }
        [HttpGet]
        public IActionResult Add_Edit_Instructor(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = _courseRepository.Find(id);
            var InstracoterCourseViewModel = new List<InstracoterCourseViewModel>();
            var teachers = _userManager.Users.Where(u => u.StudantOrTeacher == "Teacher" &&u.IsApprovedUser == true);
            foreach (var teacher in teachers)
            {
                var model = new InstracoterCourseViewModel
                {
                    InstructorId = teacher.Id,
                    InstructorUserName = teacher.UserName
                   
                };
                if (course.InstructorId == teacher.Id)
                {
                    model.IsSelected = true;
                }
                else
                {
                    model.IsSelected = false;
                }
                InstracoterCourseViewModel.Add(model);
            }
            return View(InstracoterCourseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add_Edit_Instructor(int id, List<InstracoterCourseViewModel> models)
        {
            
            var course = _courseRepository.Find(id);
            if (course == null)
            {
                return NotFound();
            }
            bool result = true;
            for (var i =  0; i < models.Count; i++)
            {
               
                var user = await _userManager.FindByIdAsync(models[i].InstructorId!);

                if (models[i].IsSelected && !(course.InstructorId == user!.Id))
                {
                    course.InstructorId = models[i].InstructorId;
                    result=  _courseRepository.UpdateWithIdentityResult(course);
                    
                }
                else if (!(models[i].IsSelected) && course.InstructorId == user.Id)
                {
                    course.InstructorId = "";
                    result = _courseRepository.UpdateWithIdentityResult(course);
                }
            }
            if (result)
            {
                return RedirectToAction("MoreCourse", new { id = id });
            }
            return View(models);
        }

    }
}
