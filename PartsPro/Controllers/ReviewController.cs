using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/review")]
public class ReviewController : ControllerBase
{
    static List<Review> list = new();

    [HttpPost]
    public IActionResult Create(ReviewDto dto)
    {
        var r = new Review
        {
            Id = list.Count + 1,
            CustomerId = dto.CustomerId,
            Comment = dto.Comment
        };

        list.Add(r);
        return Ok(r);
    }
}