using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CommonLibrary.Core.Domain.Entities.Customers
{
    [Table("Customers", Schema = "Core")]
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        public int CustomerNumber { get; set; }
        public bool IsCompany { get; set; }
        public int BranchId { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }

        public string? EnFirstName { get; set; }
        public string? EnLastName { get; set; }
        public string EnFatherName { get; set; }

        public string? NationalNumber { get; set; }
        public int NationalityTypeId { get; set; }

        public bool IsFamilySupervisor { get; set; }
        public int GroupCode { get; set; }

        public string? LegalNationalityNumber { get; set; }
        public string? LegalRegistrationNumber { get; set; }
        public string? LegalRegistrationLocation { get; set; }
        public string? LegalEconomicCode { get; set; }

        public string? MobileNo { get; set; }

        public bool Gender { get; set; }
        public bool IsMarried { get; set; }

        public DateTime BirthDate { get; set; }
        public string? BirthLocation { get; set; }

        public string? IdNumber { get; set; }
        public string? BirthCertificateNo { get; set; }
        public string? BirthCertificateSerial { get; set; }

        public int? EdicationLeveTypelId { get; set; }

        public bool IsEnable { get; set; }
        public DateTime RegisterDate { get; set; }

        public string FullName { get; set; }
        public bool IsDeath { get; set; }

        public DateOnly? BirtDateStandard { get; set; } // احتمالاً اشتباه تایپی دارد
        public string? ShamsiDate { get; set; }

        public int? SuppInsuranceTypeId { get; set; }
    }
}
