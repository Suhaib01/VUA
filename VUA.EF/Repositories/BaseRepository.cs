using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VUA.Core.IRepositories;
using VUA.Core.Models;

namespace VUA.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
       
        private readonly AppDbContext _db;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        public BaseRepository(AppDbContext db,
            IDbContextFactory<AppDbContext> dbFactory)
        {
            _db = db;
            _dbFactory = dbFactory;
        }
        public List<T> GetValues(string col)
        {
            var dbCon = _dbFactory.CreateDbContext();
            var valluse =  dbCon.Set<T>().Include(col).ToList();
            return valluse;
        }

        public void Add(T entity)
        {
            _db.Set<T>().Add(entity);
            _db.SaveChanges();
        }
        public async void AddAsync(T entity)
        {
             await _db.Set<T>().AddAsync(entity);
             _db.SaveChanges();
            
            
        }
        public void Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
            _db.SaveChanges();

        }

        public T Find(int id)
        {
            return _db.Set<T>().Find(id)!;
        }

      

        public async Task<T> GetByIdAsync(int id)
        {
            
            return await _db.Set<T>().FindAsync(id)!;
        }

        public async Task Update(T entity)
        {

            _db.Set<T>().Update(entity);
            _db.SaveChanges();
        }


        IEnumerable<T> IBaseRepository<T>.GetAll()
		{
			return _db.Set<T>().ToList();

           
		}

        public bool UpdateWithIdentityResult(T entity)
        {
            var result = _db.Set<T>().Update(entity);
            _db.SaveChanges();
            if (result != null)
            {

                return true;
            }
            else
            {
                return false;
            }
        }
        public void deleteCourseFromUser(string studentId, int courseId)
        {
            var student = _db.ApplicationUsers.Include(s => s.UserCourses).FirstOrDefault(s => s.Id == studentId);
            var course = _db.Courses.FirstOrDefault(c => c.CourseId == courseId);

            if (student != null && course != null)
            {
                // Remove the course from the student's courses
                var studentCourse = student.UserCourses!.FirstOrDefault(sc => sc.CourseId == courseId);
                if (studentCourse != null)
                {
                    student.UserCourses!.Remove(studentCourse);
                    _db.SaveChanges();
                }
            }
        }
        public void deleteWeekFromCourse(int weekId, int courseId)
        {
            var course = _db.Courses.Include(s => s.CourseWeeks).FirstOrDefault(s => s.CourseId == courseId);
            var week = _db.Weeks.FirstOrDefault(c => c.WeekId == weekId);

            if (week != null && course != null)
            {
                // Remove the course from the student's courses
                var CourseWeek = course.CourseWeeks!.FirstOrDefault(sc => sc.WeekId == weekId);
                if (CourseWeek != null)
                {
                    course.CourseWeeks!.Remove(CourseWeek);
                    _db.SaveChanges();
                }
            }
        }
    }
}
