using Library_Project.Model;

namespace Library_Project.Services.Interfaces
{
    public interface IBookRepository
    {
        List<Book> GetAllBooks();
        Book FindBook(string title);
        void SaveBook(Book book);
    }

}
