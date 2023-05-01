using Microsoft.AspNetCore.Http;
using VeryBigShoeCompany.Domain;

namespace VeryBigShoeCompany.Application;

public interface IOrderService
{
    List<Order> SaveOrder(IFormFile file);
}