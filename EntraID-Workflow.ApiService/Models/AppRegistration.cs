using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntraID.Workflow.ApiService.Models
{
    public class AppRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppName { get; set; }

        [Required]
        [EmailAddress]
        public string OwnerEmail { get; set; }

        [Required]
        public int AppTypeId { get; set; }

        [ForeignKey("AppTypeId")]
        public AppType AppType { get; set; }

        [Required]
        public string RedirectUrl { get; set; }

        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public Status Status { get; set; }
    }

    public class AppType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public AppType(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}