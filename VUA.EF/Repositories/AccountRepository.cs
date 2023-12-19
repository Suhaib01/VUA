using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VUA.Core.IRepositories;
using VUA.Core.IRepositories.IService;
using VUA.Core.Models;
using VUA.Core.Models.ViewModels;
using VUA.EF.Repositories.Service;

namespace VUA.EF.Repositories
{
    public class AccountRepository : IAccountRepositorory
    {
        private readonly UserManager<AppllicationUser> _userManager;
        private readonly SignInManager<AppllicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IBaseRepository<Course> _courseRepository;
        private readonly IBaseRepository<UserCourse> _UserCourseRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IBaseRepository<AppllicationUser> _applicationUserRepository;
        
        public AccountRepository(UserManager<AppllicationUser> userManager,
            SignInManager<AppllicationUser> signInManager,
            IUserService userService,
            IBaseRepository<Course> courseRepository,
            IEmailService emailService,
            IConfiguration configuration,
            IBaseRepository<UserCourse> UserCourseRepository,
            IBaseRepository<AppllicationUser> applicationUserRepository
            )

        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _emailService = emailService;
            _configuration = configuration;
            _UserCourseRepository = UserCourseRepository;
            _courseRepository = courseRepository;
            _applicationUserRepository = applicationUserRepository;
            
        }
        //public async Task<IdentityResult> CreateUserAsync(AppllicationUser model)
        //{
        //    var user = new AppllicationUser()
        //    {
        //        FullName = $"{model.FirstName} {model.LastName}",
        //        Email = model.Email,
        //        UserName = model.Email,
        //        StudantOrTeacher = model.StudantOrrTeacher,
        //        IsCompleted = false,
        //        CourseId = 1
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password!);
        //    //if (result.Succeeded)
        //    //{
        //    //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    //if (!string.IsNullOrEmpty(token))
        //    //{
        //    //    await SendEmailConfirmtion(user, token);
        //    //}
        //    //}
        //    return result;
        //}
        public async Task<AppllicationUser> FindUserByIdAsync(string id)
        {
            var result = await _userManager.FindByIdAsync(id);
            return result!;
        }
        public async Task<AppllicationUser> FindUserByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            return result!;
        }
        public async Task<bool> IsInRoleAsync(AppllicationUser user, string RoleName)
        {
            var result = await _userManager.IsInRoleAsync(user,RoleName);
            return result;
        }
        public async Task<Microsoft.AspNetCore.Identity.SignInResult> PasswordUserSignInAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, model.RememberMe, false);
            return result;
        }
        public async Task SignOutAcync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<AppllicationUser> GetApplicationUser()
        {
            var AppUserID = _userService.GetUserId();
            return await _userManager.FindByIdAsync(AppUserID);
        }
        public async Task<IdentityResult> UpdateUserAsync(AppllicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result;
        }
        public async Task<IdentityResult> CompleteUserProfile(CompleteProfileModelView model,string WRP)
        {

            var studant = await GetApplicationUser();
            studant!.DateOfBirth = model.DateOfBirth;
            studant.PhoneNumber = model.PhoneNumber;
           // studant.Gender = model.Gender;
            studant.Country = model.City;
            studant.PostalCode = model.PostalCode;
            studant.Address = $"{model.State} {model.StreetName}";
            studant.IdType = model.IdType;
            studant.IdNumber = model.IdNumber;
            studant.ExpiryDate = model.ExpiryDate;
            studant.ProfileImage = await UploadImag("uploads/ProfilsPuctuers/", model.profilePictuer!, WRP);
            studant.IDFrontImage = await UploadImag("uploads/ProfilsPuctuers/Id/", model.IdFrontSidePictuer!, WRP);
            if(model.IdBackSidePictuer != null) { studant.IDbackImage = await UploadImag("uploads/ProfilsPuctuers/Id/", model.IdBackSidePictuer!, WRP); }
            studant.IsCompleted = true;
            var result = await _userManager.UpdateAsync(studant);
            return result;
        }
        public async Task<IdentityResult> CompleteTeacherUserProfile(CompleatTeacherProfileViewModel model, string WRP)
        {

            var Teacher = await GetApplicationUser();
            Teacher!.DateOfBirth = model.DateOfBirth;
            Teacher.PhoneNumber = model.PhoneNumber;
           // Teacher.Gender = model.Gender;
            Teacher.Country = model.City;
            Teacher.PostalCode = model.PostalCode;
            Teacher.Address = $"{model.State} {model.StreetName}";
            Teacher.IdType = model.IdType;
            Teacher.IdNumber = model.IdNumber;
            Teacher.ExpiryDate = model.ExpiryDate;
            Teacher.Major = model.Major;
            Teacher.Description= model.Description;
            Teacher.ProfileImage = await UploadImag("uploads/ProfilsPuctuers/", model.profilePictuer!, WRP);
            Teacher.IDFrontImage = await UploadImag("uploads/ProfilsPuctuers/Id/", model.IdFrontSidePictuer!, WRP);
            Teacher.CertificateImage = await UploadImag("uploads/Certificate/", model.CertificateImage!, WRP);
            if (model.IdBackSidePictuer != null) { Teacher.IDbackImage = await UploadImag("uploads/ProfilsPuctuers/Id/", model.IdBackSidePictuer!, WRP); }
            Teacher.IsCompleted = true;
            var result = await _userManager.UpdateAsync(Teacher);
            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViweModel model)
        {
           return await _userManager.ChangePasswordAsync(GetApplicationUser().Result,model.CurrenPassword!,model.NewPassword!);
            
        }
        public async Task<string> UploadImag(string folderPath, IFormFile file ,string WebRootPath)
        {
            folderPath += $"{Guid.NewGuid().ToString()}_{file.FileName}";
            string serverFolder = Path.Combine(WebRootPath, folderPath);
            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            return "/"+folderPath;
        }
        public async Task<string> UploadVideo(IFormFile model)
        {
            if (model != null && model.Length > 0)
            {
                // Generate a unique filename to avoid overwriting existing files
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.FileName);

                // Define the path where you want to save the uploaded video
                var filePath = Path.Combine("wwwroot", "uploads", fileName);

                // Save the video file to the server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }

                return fileName;
            }

            return null; // or some default value indicating no file was uploaded
        }
        public async Task<string> UploadPowerPoint(IFormFile powerPointFile,string Web)
        {
            if (powerPointFile != null && powerPointFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(powerPointFile.FileName);
                // Save the PowerPoint file to a location (e.g., server's file system)
                var filePath = Path.Combine(Web, "Uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    powerPointFile.CopyTo(stream);
                }

                // Optionally, you can store the file path in a database or perform additional actions

                return fileName;
            }

            return null;
        }



            private bool IsVideoFile(string fileExtension)
        {
            // You can extend this method to check for valid video file extensions
            // For simplicity, this example allows common video file formats (you might need to update this list)
            string[] allowedExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };
            return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public void AddCourse(int courseId)
        {
            var studentId = _userService.GetUserId();
            var userCourse = new UserCourse
            {
                CourseId = courseId,
                UserId = studentId
            };

            _UserCourseRepository.Add(userCourse);
           
        }

        public async Task<IdentityResult> AddUserToRoleAsync(AppllicationUser Studant, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(Studant, roleName);
            return result;
        }

        public async Task<AppllicationUser> GetApplicationUser(ClaimsPrincipal user)
        {
            var stuEmail = user.Identity!.Name;
            AppllicationUser studant = await _userManager.FindByEmailAsync(stuEmail);
            return studant!;
        }
        public AppllicationUser IsSignInUser(ClaimsPrincipal user)
        {

            if (_signInManager.IsSignedIn(user))
            {
                AppllicationUser studant = GetApplicationUser(user).Result;
                return studant;
            }
            return null!;
        }
        public  bool ChekPhoneNumper(string phone)
        {
            if(phone == null) { return false; }
            var chkPhone = _applicationUserRepository.GetAll().Where(x => x.PhoneNumber == phone);
            if (chkPhone.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task SendEmailConfirmtion(AppllicationUser user, string token)
        {
            string appDomain = _configuration.GetSection("Application:AppDomain").Value!;
            string confirmationLink = _configuration.GetSection("Application:EmailConfirmation").Value!;
            UserEmailOptions options = new UserEmailOptions
            {
                ToEmails = new List<string> { user.Email! },
                PlaceHolders = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.FullName!),
                    new KeyValuePair<string, string>("{{Link}}", string.Format(appDomain + confirmationLink, user.Id, token))
                }
            };
            await _emailService.SendEmailForConfirmation(options);
        }

        Task<Microsoft.AspNetCore.Identity.SignInResult> IAccountRepositorory.PasswordUserSignInAsync(LoginViewModel model)
        {
            throw new NotImplementedException();
        }

        
    }
}
