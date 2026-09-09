
namespace CommonLibrary.Core.Domain.Dto.Customers
{
    public class CustomerAddressVM
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CustomerNumber { get; set; }
        public int CustomerAddressTypeId { get; set; }
        public string CustomerAddressTypeDesc { get; set; }
        public string PostalCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Address
        {
            get
            {
                return $"{AddressLine1} {AddressLine2} {AddressLine3}";
            }
        }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
    }
}
