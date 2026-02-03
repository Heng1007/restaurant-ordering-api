using FoodDeliveryServer.Models;

namespace FoodDeliveryServer.Services
{
    // 📜 这是一个接口 (Interface)
    // 它只定义“我们要干什么”，不定义“怎么干”
    public interface IFoodService
    {
        // 1. 获取所有食物
        Task<List<FoodItem>> GetAllFoods();

        // 2. 添加食物
        Task<FoodItem> AddFood(FoodItem food);

        // 4. 删除食物
        Task DeleteFood(int id);

        // 3. 根据ID找食物 (为了配合 Order 使用)
        Task<FoodItem?> GetFoodById(int id);
    }
}