using Library_Project.Models;

namespace Library_Project.Services.Interfaces
{
    public interface IMemberService
    {
        Member GetMember(int memberId);
        bool IsValidMember(int memberId);
    }

}
