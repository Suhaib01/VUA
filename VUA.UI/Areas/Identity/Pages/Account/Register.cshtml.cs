// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using VUA.Core.Models;
using VUA.EF.Repositories;
using VUA.EF.Repositories.Service;
using IEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

namespace WebApplication1.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppllicationUser> _signInManager;
        private readonly UserManager<AppllicationUser> _userManager;
        private readonly IUserStore<AppllicationUser> _userStore;
        private readonly IUserEmailStore<AppllicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailService _emailSender;
        private readonly IAccountRepositorory _accountRepositories;

        public RegisterModel(
            UserManager<AppllicationUser> userManager,
            IUserStore<AppllicationUser> userStore,
            SignInManager<AppllicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailService emailSender,
            IAccountRepositorory accountRepositorory)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _accountRepositories = accountRepositorory;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {

            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
           



            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			string selectedValue = Request.Form["options"].ToString();
			string gender = Request.Form["gender"].ToString();
            string condtions = Request.Form["condtions"].ToString();
			if (ModelState.IsValid && selectedValue != "" &&
                gender !=""
                && condtions !="")
            {
                
                var user = CreateUser();
                user.IsCompleted = false;
                user.IsRejectedUser = false;
                user.FullName = Input.FirstName + " " + Input.LastName;
                
                user.Gender = gender;
                user.StudantOrTeacher = selectedValue;
               
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        if(user.StudantOrTeacher=="Studant"||user.StudantOrTeacher == "Teacher")
                        {
							bool isInRole = await _accountRepositories.IsInRoleAsync(user, "AppUsers");

							if (!isInRole)
							{
								await _accountRepositories.AddUserToRoleAsync(user, "AppUsers");

								return RedirectToAction("Login", "Account", new { area = "Identity" });
							}
                        }
                        else
                        {
                            bool isInRole = await _accountRepositories.IsInRoleAsync(user, "Admin");

                            if (!isInRole)
                            {
                                await _accountRepositories.AddUserToRoleAsync(user, "Admin");

                                return RedirectToAction("Login", "Account", new { area = "Identity" });
                            }
                        }
                        return LocalRedirect("~/Identity/Account/Login");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            if(selectedValue == "") { TempData["sot"] = "*slecte If you studant or Teacher"; return Page(); }
            if(gender == "") { TempData["gander"] = "*slecte gander"; return Page(); }
            if(condtions == "") { TempData["con"] = "*You must agree to the terms and conditions"; return Page(); }

            // If we got this far, something failed, redisplay form
			return Page();
        }

        private AppllicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppllicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppllicationUser)}'. " +
                    $"Ensure that '{nameof(AppllicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppllicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppllicationUser>)_userStore;
        }
    }
}
