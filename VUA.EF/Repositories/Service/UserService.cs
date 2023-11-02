using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VUA.Core.IRepositories;
using VUA.Core.IRepositories.IService;
using VUA.Core.Models;

namespace VUA.EF.Repositories.Service
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        private IBaseRepository<Course> _courseRepository;
        public UserService(IHttpContextAccessor httpContext, IBaseRepository<Course> courseRepository)
        {
            _httpContext = httpContext;
            _courseRepository = courseRepository;
        }
        public string GetUserId()
        {

            return _httpContext.HttpContext!.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }
        public IEnumerable<Course> GetCourses()
        {
            return _courseRepository.GetAll();
        }


       
    }
}
