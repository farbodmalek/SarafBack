
namespace CommonLibrary.Core.Domain.Dto
{
    public class UserDto : LoginUserDTO
    {
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
