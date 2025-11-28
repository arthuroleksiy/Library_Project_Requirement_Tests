namespace Library_Project.Models
{
    public class BorrowRecord
    {
        public int MemberId { get; set; }
        public string Title { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public bool IsLate { get; set; }
        public bool Returned { get; set; }
    }

}
