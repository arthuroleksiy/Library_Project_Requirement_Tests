namespace Library_Project.Services.Interfaces
{
    public interface IAuditService
    {
        void LogBorrow(int memberId, string title);
    }
}
