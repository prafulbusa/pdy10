using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Instructor
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }
        [Required]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + " " + FirstMidName; }
        }

        // navigational properties - virtual to take advantage of lazy loading
        public virtual ICollection<Course> Courses { get; set; }

        // v.08 - or could do this instead of controller code to make a new empty collections of 
        // courses when doing instructor::create
        // we will leave this capability and checking in the controller
        //private ICollection<Course> _courses;
        //public virtual ICollection<Course> Courses
        //{
        //    get
        //    {
        //        return _courses ?? (_courses = new List<Course>());
        //    }
        //    set
        //    {
        //        _courses = value;
        //    }
        //} 
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}