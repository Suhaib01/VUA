using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUA.Core.Models.ViewModels
{
     public class CreateCourseContantViewModel
    {
        public string? WelcomeViduoUrl { get; set; }
        [Required]
        public string? SubjectName { get; set; }
        [Required]
        public string? ViduoUrl { get; set; }
        public string? Subjectfile { get; set; }

        public string? WhatStudantShouldDo { get; set; }

        public string? EndOfThisWeek { get; set; }
    }
}
