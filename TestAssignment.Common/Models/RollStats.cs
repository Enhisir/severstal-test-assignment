namespace TestAssignment.Common.Models;

public class RollStats
{
    public int CountAdded { get; set; }
    
    public int CountDeleted { get; set; }

    public double AverageLength { get; set; }
    
    public double AverageWeight { get; set; }
    
    // минимальный промежуток между добавлением и удалением рулона
    public int MinGap { get; set; }
    
    // максимальный промежуток между добавлением и удалением рулона
    public int MaxGap { get; set; }

    
    public static RollStats Empty => new();
}