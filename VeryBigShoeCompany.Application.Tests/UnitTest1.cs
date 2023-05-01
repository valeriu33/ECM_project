using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using VeryBigShoeCompany.Shared;

namespace VeryBigShoeCompany.Application.Tests;

public class Tests
{
    private OrderService _orderService;
    [SetUp]
    public void Setup()
    {
        _orderService = new OrderService(new AppSettings{XsdPath = "../../../../VeryBigShoeCompany.SolutionFiles/OrderImport.xsd"});
    }

    [Test]
    public void SaveOrder_MissingName_ThrowsException()
    {
        // Arrange
        //Setup mock file using a memory stream
        var content = 
	        $@"<?xml version=""1.0""?>
			<BigShoeDataImport>
				<Order 
					
					CustomerEmail = ""test@test.com""
					Quantity = ""2000""
					Notes = ""This is a test note""
					Size = ""11.5""
					DateRequired = ""{DateTime.Now.AddDays(20):yyyy-MM-dd}""/>
			</BigShoeDataImport>";
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;
        
        //create FormFile with desired data
        IFormFile file = new FormFile(stream, 0, stream.Length, "test", fileName);

        // Assert & Act
        Assert.Throws<ValidationException>(() => _orderService.SaveOrder(file));
    }
    
    [Test]
    public void SaveOrder_DateRequiredTooSoon_ThrowsException()
    {
	    // Arrange
	    //Setup mock file using a memory stream
	    var content = 
		    $@"<?xml version=""1.0""?>
			<BigShoeDataImport>
				<Order 
					CustomerName = ""1""
					CustomerEmail = ""test@test.com""
					Quantity = ""2000""
					Notes = ""This is a test note""
					Size = ""11.5""
					DateRequired = ""{DateTime.Now.AddDays(5):yyyy-MM-dd}""/>
			</BigShoeDataImport>";
	    var fileName = "test.pdf";
	    var stream = new MemoryStream();
	    var writer = new StreamWriter(stream);
	    writer.Write(content);
	    writer.Flush();
	    stream.Position = 0;
        
	    //create FormFile with desired data
	    IFormFile file = new FormFile(stream, 0, stream.Length, "test", fileName);

	    // Assert & Act
	    Assert.Throws<ValidationException>(() => _orderService.SaveOrder(file));
    }
    
    // TODO: Add more unit tests
}