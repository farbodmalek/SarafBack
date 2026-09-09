using CommonLibrary.Core.Domain.Dto.Legal;

namespace CommonLibrary.Core.Services.Interfaces
{
    public interface ILegalProxy
    {
        Task<bool> SetPrimaryLegalDoc(PrimaryLegalDocDto primaryLegalDocDto);
        Task<bool> SetGuarantorsLegal(int PmId);
        Task<bool> SetCustomerLegal(int PmId);
        Task<bool> SetGuarantessLegal(int PmId);
    }
}
