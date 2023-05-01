using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using VeryBigShoeCompany.Application;
using VeryBigShoeCompany.Shared;

namespace VeryBigShoeCompany.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("Receive")]
    public IActionResult ReceiveOrder(IFormFile file)
    {
        try
        {
            return Ok(_orderService.SaveOrder(file));
        }
        catch (ArgumentException argumentException)
        {
            return BadRequest(argumentException.Message);
        }
        catch (ValidationException argumentException)
        {
            return BadRequest(argumentException.Message);
        }
    }
}