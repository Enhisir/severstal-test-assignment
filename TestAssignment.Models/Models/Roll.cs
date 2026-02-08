using System.ComponentModel.DataAnnotations;

namespace TestAssignment.Models.Models;

public class Roll
{
    public Guid Id { get; init; }

    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double Length { get; init; }

    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double Weight { get; init; }

    [Required]
    public DateTime DateAdded { get; init; }
    
    public DateTime? DateDeleted { get; set; }
}