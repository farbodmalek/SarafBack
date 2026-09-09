namespace GirlyShopBackend.Core.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string ReceiverFullName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
