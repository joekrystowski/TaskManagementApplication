using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementApplication.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(Config.MaxTaskNameLength)]
        [Display(Name = "Task Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [MaxLength(Config.MaxTaskDescriptionLength)]
        [Display(Name = "Task Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Task Status")]
        public Constants.Status Status { get; set; }

        [Required]
        [Display(Name = "Task Due Date")]
        [DataType(DataType.DateTime)]
        public DateTime DueDate{ get; set;}

        [Display(Name = "Task Priority")]
        public Constants.Priority Priority { get; set; }

        // User reference
        [Display(Name = "User Id")]
        public string? UserId { get; set; }
    }
}
