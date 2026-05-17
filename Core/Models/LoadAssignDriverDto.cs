namespace Core.Models;

using System.ComponentModel.DataAnnotations;

public class LoadAssignDriverDto
{
    public Guid DriverId { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DriverCharge { get; set; }
}