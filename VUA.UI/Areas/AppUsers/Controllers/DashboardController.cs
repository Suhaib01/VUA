using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VUA.Core.Models;
using VUA.EF;
using System.Collections.Generic;
using VUA.Core.Models.ViewModels;
using VUA.Core.IRepositories;
using VUA.Core.IRepositories.IService;
using VUA.EF.Repositories;
using System.IO;
using System.Threading.Tasks;
using System.Web;



namespace VUA.UI.Areas.Studant.Controllers
{
    [Area("AppUsers")]
    [Authorize(Roles ="AppUsers")]
    public class DashboardController : Controller
    {

        private readonly IUserService _userService;
        private readonly IBaseRepository<Course> _cousreRepository;
        private readonly IBaseRepository <UserCourse> _userCourseRepository;
        private readonly IAccountRepositorory _accountRepositorory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBaseRepository<UserPayment> _userPaymentRepository;
        private readonly UserManager<AppllicationUser> _userManager;
        private readonly IBaseRepository<AppllicationUser> _appuserRepository;
		private readonly IBaseRepository<CourseWeeks> _courseWeeksRepository;
        private readonly IBaseRepository<Week> _weeksRepository;
        private readonly IWebHostEnvironment _webhostEnvauroment;
		public DashboardController(IAccountRepositorory accountRepositorory,
            IWebHostEnvironment webHostEnvironment,
			IUserService userService,
			IBaseRepository<Course> cousreRepository,
			IConfiguration configuration,
			IBaseRepository<UserPayment> userpaymentRepository,
			UserManager<AppllicationUser> userManager,
			IBaseRepository<UserCourse> userCourseRepository,
			IBaseRepository<AppllicationUser> appuserRepository,
			IBaseRepository<CourseWeeks> courseWeeksRepository,
            IBaseRepository<Week> weekRepository,
            IWebHostEnvironment webHost)
		{
			_webHostEnvironment = webHostEnvironment;
			_accountRepositorory = accountRepositorory;
			_userService = userService;
			_cousreRepository = cousreRepository;
			_userPaymentRepository = userpaymentRepository;
			_userManager = userManager;
			_userCourseRepository = userCourseRepository;
			_appuserRepository = appuserRepository;
			_courseWeeksRepository = courseWeeksRepository;
            _weeksRepository = weekRepository;
            _webhostEnvauroment = webHost;
		}
		public IActionResult Index()
        {
            var user = _accountRepositorory.IsSignInUser(User);
			var course = _userCourseRepository.GetValues("Course").Where(c => c.UserId == user.Id);
            if (course.Any())
            {
                return RedirectToAction("myCourse");
            }
            else
            {
                return View(user);
            }
            
			
              
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var user = _accountRepositorory.GetApplicationUser(User).Result;             
            var course = _userCourseRepository.GetValues("Course").Where(c => c.UserId == user.Id);
            var InsCourses = _cousreRepository.GetAll().Where(c=>c.InstructorId == user.Id);
            ViewBag.StudantCourse = course;
            ViewBag.Count = course.Count();
            ViewBag.InsCourse = InsCourses;
            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult CompleteTeacherProfile()
        {
            
            return View();
        }
        [HttpGet]
        public IActionResult CompleteProfile()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CompleteProfile(CompleteProfileModelView model)
        {
            if(_accountRepositorory.ChekPhoneNumper(model.PhoneNumber!) == true) { ViewBag.err = "● Exist Phone number!"; return View(model); }
            if (ModelState.IsValid)
            {
                if (model.profilePictuer != null)
                {
                    var result = await _accountRepositorory.CompleteUserProfile(model, _webHostEnvironment.WebRootPath);
                    if (result.Succeeded)
                    {
                        ViewBag.sucsses = true;
                        return View();
                    }
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return View(model);
                }
                return View(model);
            }
            return View(model);

        }
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CompleteTeacherProfile(CompleatTeacherProfileViewModel model)
        {
            if (_accountRepositorory.ChekPhoneNumper(model.PhoneNumber!) == true) { ViewBag.err = "● Exist Phone number!"; return View(model); }
            if (ModelState.IsValid)
            {
                if (model.profilePictuer != null)
                {
                    var result = await _accountRepositorory.CompleteTeacherUserProfile(model, _webHostEnvironment.WebRootPath);
                    if (result.Succeeded)
                    {
                        ViewBag.sucsses = true;
                        return View();
                    }
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return View(model);
                }
                return View(model);
            }
            return View(model);

        }
        [HttpGet]
        public IActionResult ChangePicture()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePicture(ChangePictuerViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.profilePictuer != null)
                {
                    string folder = "uploads/ProfilsPuctuers/Chenged/";
                    var studant = _accountRepositorory.GetApplicationUser().Result;

                    studant!.ProfileImage = await _accountRepositorory.UploadImag(folder, model.profilePictuer, _webHostEnvironment.WebRootPath);
                    var result = await _accountRepositorory.UpdateUserAsync(studant);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Profile", studant);
                    }
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                }
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult viewFImg()
        {
            var studant = _accountRepositorory.IsSignInUser(User);
            return View(studant);
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult CertificateImg()
        {
            var studant = _accountRepositorory.IsSignInUser(User);
            return View(studant);
        }
        [HttpGet]
        public IActionResult viewBImg()
        {

            var studant = _accountRepositorory.IsSignInUser(User);
            return View(studant);
        }
        [HttpGet]
        public IActionResult viewPImg()
        {

            var studant = _accountRepositorory.IsSignInUser(User);
            return View(studant);
        }
        [HttpGet]
        [Authorize(Roles = "Studant")]
        public IActionResult AddCourse()
        {
            var courses = _userService.GetCourses();
            var userId = _userService.GetUserId();
            var userCourses = _userCourseRepository.GetValues("Course").Where(uc => uc.UserId == userId);
            ViewBag.Course =courses.Where(c=>!userCourses.Any(uc=>uc.CourseId == c.CourseId)&&c.InstructorId!="").ToList();
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Studant")]
        public async Task<IActionResult> AddCourse(AddCourseViewModel course)
        {
            var co = _cousreRepository.Find(course.Id);
            TempData["ID"] = co.CourseId;
            TempData["Total"] = co.CoursePrice;
            TempData["Name"] = co.CourseName;
            return RedirectToPage("/Checkout");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViweModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepositorory.ChangePasswordAsync(model);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
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
        [Authorize(Roles = "Studant")]
        public  IActionResult MyCourse()
        {
            var userId = _userService.GetUserId();
            var courses = _userCourseRepository.GetValues("Course").Where(c => c.UserId == userId);
            return View(courses);
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult TeacherCourses()
        {
            var user = _accountRepositorory.GetApplicationUser().Result;
            var InsCourses = _cousreRepository.GetAll().Where(c => c.InstructorId == user.Id);
            ViewBag.InsCourse = InsCourses;
            return View();
        }
        [HttpGet]
        public IActionResult SubCoruses()
        {
            return View();
        }
        [Authorize(Roles = "Studant")]
        public IActionResult PaymentHistory()
        {
            var userid = _userService.GetUserId();
            

            var Payments = _userPaymentRepository.GetValues("Payment").Where(p=>p.UserId == userid);

            return View(Payments);

            
        }
       
        [HttpGet]
        public IActionResult CoursePage()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public  IActionResult CreateCourseContant(Course Course)
        {
            var course = _cousreRepository.Find(Course.CourseId);
            ViewBag.coId= Course.CourseId;
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public  IActionResult CreateCourseContant(CreateCourseContantViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                var course = _cousreRepository.Find(id);
                if (course == null)
                {
                    return NotFound();
                }
                var ids = new Week
                {
                    WhatStudantShouldDo = model.WhatStudantShouldDo,
                    EndOfThisWeek = model.EndOfThisWeek,
                    Subjectfile = model.Subjectfile,
                    ViduoUrl =  model.ViduoUrl,
                    WelcomeViduoUrl = model.WelcomeViduoUrl,
                    SubjectName = model.SubjectName
                };
                _weeksRepository.Add(ids);

                var courseWeek = new CourseWeeks
                {
                    CourseId = course.CourseId,
                    WeekId = ids.WeekId
                };
                _courseWeeksRepository.Add(courseWeek);

                return RedirectToAction("TeacherCourses");
            }
            return View(model);
        }
        
        public IActionResult Contant(int id)
        {
            
            var courseContant = _courseWeeksRepository.GetValues("Week").Where(c => c.CourseId ==id);

            if (courseContant.Any())
            {
                return View(courseContant);
            }
            else
            {
               return NoContent();
            }
            
        }
        [Authorize(Roles = "Studant")]
        public IActionResult DropeCourse(int courseId)
        {
            ViewBag.DropeCourse= _cousreRepository.Find(courseId);
            return RedirectToAction("MyCourse");
        }

       
        
    }

}


