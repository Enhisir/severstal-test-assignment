using System.ComponentModel.DataAnnotations;

namespace TestAssignment.Dtos;

public class CreateRollDto
{
    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double Length { get; set; }

    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double Weight { get; set; }
}