using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUA.Core.Models
{
	public class Week
	{
		[Key]
        public int WeekId { get; set; }

        public string? Description { get; set; }

        public string? SubjectName { get; set; }


		public string? ViduoUrl { get; set; }

		public string? Subjectfile { get; set; }
		
		public string? WhatStudantShouldDo { get; set; }
		
        public string? EndOfThisWeek { get; set; }
		public ICollection<CourseWeeks>? CourseWeeks { get; set; }

	}
}
