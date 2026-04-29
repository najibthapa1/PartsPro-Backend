using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/partrequest")]
public class PartRequestController : ControllerBase
{
    static List<PartRequest> list = new();

    [HttpPost]
    public IActionResult Create(PartRequestDto dto)
    {
        var p = new PartRequest
        {
            Id = list.Count + 1,
            CustomerId = dto.CustomerId,
            PartName = dto.PartName
        };

        list.Add(p);
        return Ok(p);
    }
}