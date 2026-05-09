namespace Core.Models;

using Core.Enums;

public class LoadStatusUpdateDto
{
    public Guid LoadId { get; set; }

    public LoadStatus LoadStatus { get; set; }
}