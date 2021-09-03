using System.ComponentModel.DataAnnotations;

namespace MinimalApi.Models.Requests
{
    public record PageModel([Range(0, int.MaxValue)] int Offset, [Range(1, 50)] int Limit);
}