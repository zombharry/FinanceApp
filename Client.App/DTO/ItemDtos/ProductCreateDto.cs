namespace Client.App.DTO.ItemDtos
{
    public class ProductCreateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public double? Price { get; set; }

        public Guid CategoryId { get; set; }

        public string OwnerId { get; set; }
    }
}
