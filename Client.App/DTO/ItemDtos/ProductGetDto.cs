namespace Client.App.DTO.ItemDtos;

public class ProductGetDTO
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Guid CategoryId { get; set; }

    public double? Price { get; set; }

    public string OwnerId { get; set; }
}
