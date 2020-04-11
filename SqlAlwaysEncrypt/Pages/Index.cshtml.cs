using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SqlAlwaysEncrypt.Data;
using SqlAlwaysEncrypt.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlAlwaysEncrypt.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext context;

        public IndexModel(
            IConfiguration configuration,
            SchoolContext context)
        {
            Configuration = configuration;
            this.context = context;
        }

        public IConfiguration Configuration { get; }

        public List<Student> Students { get; set; }

        public string CourseJson { get; set; }

        public void OnGet()
        {
            //context.Database.EnsureCreated();

            CourseJson = JsonConvert.SerializeObject(context.Course.ToArray(), Formatting.Indented);

            Students = new List<Student>();
        }

        public IActionResult OnPostGetStudents()
        {
            Students = context.Student.ToList();
            return Page();
        }

        public IActionResult OnPostAddStudent()
        {
            var s = new Student
            {
                FirstMidName = "太郎",
                LastName = "日本語",
                EnrollmentDate = DateTime.Now,
            };
            context.Student.Add(s);
            context.SaveChanges();

            Students = context.Student.ToList();
            return Page();
        }

        public IActionResult OnPostAddStudents()
        {
            // Look for any students.
            if (context.Student.Any())
            {
                return Page();   // DB has been seeded
            }

            var students = new Student[]
            {
                new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2019-09-01")},
                new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2017-09-01")},
                new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2018-09-01")},
                new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2017-09-01")},
                new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2017-09-01")},
                new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2016-09-01")},
                new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2018-09-01")},
                new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2019-09-01")}
            };
            foreach (Student s in students)
            {
                context.Student.Add(s);
            }
            context.SaveChanges();

            Students = context.Student.ToList();
            return Page();
        }
    }
}
