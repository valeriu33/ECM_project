using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using VeryBigShoeCompany.Domain;
using VeryBigShoeCompany.Shared;

namespace VeryBigShoeCompany.Application;

public class OrderService : IOrderService
{
    private readonly AppSettings _appSettings;

    public OrderService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public List<Order> SaveOrder(IFormFile file)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(file.OpenReadStream());
        xmlDoc.Schemas.Add(null, _appSettings.XsdPath);
        
        // Validate against the provided XSD file
        xmlDoc.Validate(OrderImportSettingsValidationEventHandler);

        var orders = new List<Order>();
        var orderNodes = xmlDoc.SelectNodes("//Order");
        if (orderNodes == null) throw new ArgumentException("Couldn't find Orders in your xml file");
        foreach (XmlNode orderNode in orderNodes)
        {
            var order = new Order
            {
                CustomerName = orderNode.Attributes["CustomerName"].Value,
                CustomerEmail = orderNode.Attributes["CustomerEmail"].Value,
                Quantity = short.Parse(orderNode.Attributes["Quantity"].Value),
                Size = float.Parse(orderNode.Attributes["Size"].Value, CultureInfo.InvariantCulture.NumberFormat),
                DateRequired = DateTime.Parse(orderNode.Attributes["DateRequired"].Value),
                Notes = orderNode.Attributes["Notes"]?.Value
            };
            ValidateOrder(order);
            orders.Add(order);
        }

        return orders;
    }

    private void ValidateOrder(Order order)
    {
        if (!Regex.IsMatch(order.CustomerEmail, Constants.ValidEmailRegEx))
            throw new ValidationException($"In order {order.CustomerName}: Email not Valid");

        if (order.DateRequired.CompareTo(AddBusinessDays(DateTime.Now , 10)) < 0)
            throw new ValidationException($"In order {order.CustomerName}: Date must be at least 10 days into the future");

        if (order.Size < 11.5 || order.Size > 15 || order.Size % .5 != 0)
            throw new ValidationException($"In order {order.CustomerName}: Size must be 11.5 to 15 including half sizes");

        if (order.Quantity % 1000 != 0)
            throw new ValidationException($"In order {order.CustomerName}: Quantity must be in multiples of 1000");
    }
    
    public static DateTime AddBusinessDays(DateTime date, int days)
    {
        switch (days)
        {
            case < 0:
                throw new ArgumentException("days cannot be negative", "days");
            case 0:
                return date;
        }

        switch (date.DayOfWeek)
        {
            case DayOfWeek.Saturday:
                date = date.AddDays(2);
                days -= 1;
                break;
            case DayOfWeek.Sunday:
                date = date.AddDays(1);
                days -= 1;
                break;
        }

        date = date.AddDays(days / 5 * 7);
        var extraDays = days % 5;

        if ((int)date.DayOfWeek + extraDays > 5)
            extraDays += 2;

        return date.AddDays(extraDays);
    }
    
    static void OrderImportSettingsValidationEventHandler(object? sender, ValidationEventArgs e)
    {
        // TODO: find a way to get the source object name for the exception message
        throw new ValidationException($"The xml didn't pass the validation:\n {e.Message}");
    }
}
