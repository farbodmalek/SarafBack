

namespace CommonLibrary.Core.Domain.Interfaces
{
    public interface IJwtManager
    {
        public string GenerateToken(int userID, string userName, string guid = null, int expireMinutes = 300);
    }
}
