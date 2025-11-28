namespace Library_Project.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Status { get; set; } = "Active"; // "Active" or "Suspended"
        public bool HasOverdueBooks { get; set; }
    }

}
