using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VUA.Core.IRepositories;
using VUA.Core.Models;

namespace VUA.EF.Repositories
{
    public class DashboardRipository : IDashboardRipository
    {
        private IBaseRepository<Course> _courseRepository;

        public DashboardRipository(IBaseRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public IEnumerable<Course> Search(string term)
        {
            var course = _courseRepository.GetAll().Where(x => x.CourseName!.Contains(term));
            return course;
        }
    }
}
