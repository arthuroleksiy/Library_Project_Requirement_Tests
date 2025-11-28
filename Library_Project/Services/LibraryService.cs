using Library_Project.Model;
using Library_Project.Models;
using Library_Project.Services.Interfaces;



    namespace Library_Project.Services
    {
        public class LibraryService
        {
            private readonly IBookRepository _bookRepo;
            private readonly IMemberService _memberService;
            private readonly INotificationService _notification;
            private readonly IAuditService _audit;
            private readonly IBorrowRepository _borrowRepo;
            private readonly IFineService _fineService;

    
        public LibraryService(
                IBookRepository repo,
                IMemberService memberService,
                INotificationService notification,
                IAuditService audit,
                IBorrowRepository borrowRepo,
                IFineService fineService)
            {
                _bookRepo = repo;
                _memberService = memberService;
                _notification = notification;
                _audit = audit;
                _borrowRepo = borrowRepo;
                _fineService = fineService;
            }



        // R1, R2, R3
        public void AddBook(string title, int copies, bool isReferenceOnly = false)
            {
                if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
                    throw new ArgumentException("Title must be at least 3 characters.");

                if (copies <= 0 || copies > 100)
                    throw new ArgumentException("Copies must be between 1 and 100.");

                int totalBooks = _bookRepo.GetAllBooks().Sum(b => b.Copies);
                if (totalBooks + copies > 500)
                    throw new InvalidOperationException("Library capacity exceeded.");

                var existing = _bookRepo.FindBook(title);

                if (existing == null)
                {
                    _bookRepo.SaveBook(new Book
                    {
                        Title = title,
                        Copies = copies,
                        IsReferenceOnly = isReferenceOnly
                    });
                }
                else
                {
                    existing.Copies += copies;
                    _bookRepo.SaveBook(existing);
                }
            }

            // R4, R5, R7, R8, R9, R11, R12, R13
            public bool BorrowBook(int memberId, string title)
            {
                var member = _memberService.GetMember(memberId);

                if (member == null || member.Status == "Suspended")
                    throw new InvalidOperationException("Member cannot borrow.");

                if (member.HasOverdueBooks)
                    throw new InvalidOperationException("Member has overdue books.");

                var book = _bookRepo.FindBook(title);
                if (book == null || book.Copies <= 0)
                    return false;

                if (book.IsReferenceOnly)
                    throw new InvalidOperationException("Reference books cannot be borrowed.");

                var activeBorrows = _borrowRepo.GetActiveBorrows(memberId);
                if (activeBorrows.Count >= 5)
                    throw new InvalidOperationException("Borrow limit exceeded.");

                if (activeBorrows.Any(b => b.Title == title))
                    throw new InvalidOperationException("Cannot borrow the same book twice.");

                // Create borrow record
                var borrow = new BorrowRecord
                {
                    MemberId = memberId,
                    Title = title,
                    BorrowDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14)
                };

                _borrowRepo.SaveBorrow(borrow);

                // Reduce book copies
                book.Copies--;
                _bookRepo.SaveBook(book);

                // Audit log
                _audit.LogBorrow(memberId, title);

                _notification.NotifyBorrow(memberId, title);
                return true;
            }

            // R6, R10, R14, R15
            public bool ReturnBook(int memberId, string title, bool signatureConfirmed)
            {
                if (!signatureConfirmed)
                    throw new InvalidOperationException("Return must be confirmed with signature.");

                var borrow = _borrowRepo.GetBorrowRecord(memberId, title);
                if (borrow == null)
                    return false;

                var book = _bookRepo.FindBook(title);
                if (book == null)
                    return false;

                // Increase copies
                book.Copies++;
                _bookRepo.SaveBook(book);

                // Mark record as returned
                borrow.ReturnDate = DateTime.Now;

                if (borrow.ReturnDate > borrow.DueDate)
                {
                    borrow.IsLate = true;
                    _fineService.ApplyFine(memberId, borrow.Title);
                }

                _borrowRepo.UpdateBorrow(borrow);

                // Notification
                _notification.NotifyReturn(memberId, title);

                // R14 – notify if no active books left
                if (!_borrowRepo.GetActiveBorrows(memberId).Any())
                    _notification.NotifyAllReturned(memberId);

                return true;
            }
        }
    }

