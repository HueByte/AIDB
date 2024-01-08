using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIDB.App.ViewModels
{
    public class TeacherViewModel
    {
        public string? Name { get; set; }
        public string? MiddleName { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Title { get; set; }
        public virtual List<SubjectViewModel>? Subjects { get; set; }

    }
}