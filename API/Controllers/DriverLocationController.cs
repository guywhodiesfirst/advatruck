using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/drivers/{driverId:guid}/location")]
    [ApiController]
    public class DriverLocationController : ControllerBase
    {
    }
}
