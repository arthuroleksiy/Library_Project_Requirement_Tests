using Library_Project.Model;
using Library_Project.Models;
using Library_Project.Services;
using Library_Project.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Library_Project.Services.Interfaces;
namespace Library_Project_Tests
{
    public class LibraryServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepo = new();
        private readonly Mock<IMemberService> _member = new();
        private readonly Mock<INotificationService> _notification = new();
        private readonly Mock<IAuditService> _audit = new();
        private readonly Mock<IBorrowRepository> _borrowRepo = new();
        private readonly Mock<IFineService> _fineService = new();

        private LibraryService CreateService()
        {
            return new LibraryService(
                _bookRepo.Object,
                _member.Object,
                _notification.Object,
                _audit.Object,
                _borrowRepo.Object,
                _fineService.Object
            );
        }

    }
}
