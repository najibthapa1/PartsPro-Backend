using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/appointment")]
public class AppointmentController : ControllerBase
{
    static List<Appointment> list = new();

    [HttpPost]
    public IActionResult Create(AppointmentDto dto)
    {
        var a = new Appointment
        {
            Id = list.Count + 1,
            CustomerId = dto.CustomerId,
            Service = dto.Service
        };

        list.Add(a);
        return Ok(a);
    }
}