namespace FoodDeliveryServer.Dtos
{
    // <T> 的意思是：这盒子能装 Order，也能装 FoodItem，随你定
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new(); // 这一页的数据 (比如 10 个订单)
        public int TotalCount { get; set; }         // 数据库里总共有多少条 (比如 1000 条)
        public int PageNumber { get; set; }         // 当前是第几页
        public int PageSize { get; set; }           // 每页显示几条

        // 👇 一个自动计算的属性：总页数 = 总数 / 每页大小 (向上取整)
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}