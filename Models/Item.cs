namespace Zaliczeniowy4.Models
{
    public class Item
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? Quantity { get; set; } = 1;
        public string? Unit { get; set; } = "szt";
        public string? Photo { get; set; }
        public bool? Bought { get; set; } = false;

    }
}
