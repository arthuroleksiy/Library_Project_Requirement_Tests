using Library_Project.Models;

namespace Library_Project.Services.Interfaces
{
    public interface IBorrowRepository
    {
        void SaveBorrow(BorrowRecord record);
        void UpdateBorrow(BorrowRecord record);
        BorrowRecord GetBorrowRecord(int memberId, string title);
        List<BorrowRecord> GetActiveBorrows(int memberId);
    }
}
