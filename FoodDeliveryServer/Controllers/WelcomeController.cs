using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This make the url to be /api/welcome
    public class WelcomeController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello Heng! The server is up and running.";
        }

        [HttpGet("me")]
        public object getMyInfo()
        {
            return new
            {
                Name = "Heng",
                Role = "Developer",
                City = "Johor Bahru",
                Skills = new string[] { "C#", "ASP.NET Core", "Blazor", "Entity Framework Core" }
            };
        }

        [HttpGet("food")]
        public object getFoodInfo()
        {
            return new
            {
                FavoriteFood = "Nasi Lemak",
                Cuisine = "Malaysian",
                SpicinessLevel = "Medium",
                Ingredients = new string[] { "Rice", "Coconut Milk", "Anchovies", "Eggs", "Peanuts", "Cucumber" }
            };
        }
    }
}
