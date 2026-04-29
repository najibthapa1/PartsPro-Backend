using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/customer")]
public class CustomerController : ControllerBase
{
    static List<Customer> list = new();

    [HttpPost("register")]
    public IActionResult Register(CustomerDto dto)
    {
        var c = new Customer
        {
            Id = list.Count + 1,
            Name = dto.Name,
            Email = dto.Email
        };

        list.Add(c);
        return Ok(c);
    }
}