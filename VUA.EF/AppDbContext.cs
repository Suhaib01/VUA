﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection.Emit;
using VUA.Core.Models;


namespace VUA.EF
{
    public class AppDbContext : IdentityDbContext<AppllicationUser>
    {
        public DbSet<WeekVideoUrls> weekVideoUrls { get; set; }
        public DbSet<WeekFileUrl> weekFileUrls { get; set; }
        public DbSet<ViduoUrl> ViduoUrls { get; set; }
        public DbSet<FileUrl> fileUrls { get; set; }
        public DbSet<UserPayment> UserPayments { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<News> Newss { get; set; }
        public DbSet<WhyAcademicsWorks> WAWS { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AppllicationUser> ApplicationUsers { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<Week> Weeks { get; set; }
        public DbSet<CourseWeeks> CourseWeeks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.Entity<WeekVideoUrls>()
                .HasKey(wf => new { wf.WeekId, wf.id });
                 
            modelBuilder.Entity<WeekVideoUrls>()
                .HasOne(wf => wf.Week)
                .WithMany(w => w.WeekVideoUrls)
                .HasForeignKey(wf => wf.WeekId);

            modelBuilder.Entity<WeekVideoUrls>()
                .HasOne(wf => wf.Url)
                .WithMany()
                .HasForeignKey(wf => wf.id);


            modelBuilder.Entity<WeekFileUrl>()
               .HasKey(wf => new { wf.WeekId, wf.id });

            modelBuilder.Entity<WeekFileUrl>()
                .HasOne(wf => wf.Week)
                .WithMany(w => w.WeekFileUrls)
                .HasForeignKey(wf => wf.WeekId);

            modelBuilder.Entity<WeekFileUrl>()
                .HasOne(wf => wf.Url)
                .WithMany()
                .HasForeignKey(wf => wf.id);





            modelBuilder.Entity<UserCourse>()
            .HasKey(u => new { u.UserId, u.CourseId });

            modelBuilder.Entity<UserCourse>()
                .HasOne(u => u.User)
                .WithMany(uc => uc.UserCourses)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<UserCourse>()
                .HasOne(c => c.Course)
                .WithMany(uc => uc.UserCourses)
                .HasForeignKey(c => c.CourseId);





            modelBuilder.Entity<UserPayment>()
                .HasKey(u => new { u.UserId, u.PaymentId });

            modelBuilder.Entity<UserPayment>()
                .HasOne(u => u.User)
                .WithMany(ap => ap.UserPayments)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<UserPayment>()
                .HasOne(p => p.Payment)
                .WithMany(up => up.UserPayments)
                .HasForeignKey(p => p.PaymentId);





			modelBuilder.Entity<CourseWeeks>()
				.HasKey(c => new { c.CourseId, c.WeekId });

			modelBuilder.Entity<CourseWeeks>()
				.HasOne(w => w.Course)
				.WithMany(cw => cw.CourseWeeks)
				.HasForeignKey(c => c.CourseId);

			modelBuilder.Entity<CourseWeeks>()
				.HasOne(w => w.Week)
				.WithMany(cw => cw.CourseWeeks)
				.HasForeignKey(w => w.WeekId);
		}
        
    }
        
        
    
}
