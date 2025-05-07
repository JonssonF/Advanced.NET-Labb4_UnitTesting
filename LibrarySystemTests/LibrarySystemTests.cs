using Advanced.NET_Labb4_UnitTesting;

namespace LibrarySystemTests
{
    [TestClass]
    public class LibrarySystemTests
    {
        private readonly LibrarySystem _librarySystem;

        public LibrarySystemTests()
        {
            _librarySystem = new LibrarySystem();
        }


        [TestMethod]
        [DataRow("Test Title", "Test Author", "0123456789", 2025)]
        [DataRow("Test Title1", "Test Author1", "0123456780", 2025)]
        public void AddBook_ShouldAdd_ValidBook(string title, string author, string isbn, int year)
        {
            //Given: a book with ubnique ISBN
            var book = new Book(title, author, isbn, year);

            //When: try to add the book to the library
            bool result = _librarySystem.AddBook(book);

            //Then: the book should be added successfully and retrievable by ISBN
            Assert.IsTrue(result, "Book should be added successfully.");
            Assert.IsNotNull(_librarySystem.SearchByISBN(isbn), "Book should be found in the library.");
        }

        [TestMethod]
        public void AddBook_ShouldNotAdd_DuplicateISBN()
        {
            //Given: two books with the same ISBN.
            string isbn = "0123456789";
            var duplicateBook = new Book("Test Title", "Test Author", isbn, 2025);

            //When: adding the first book(successfully) and the second book(unsuccessfully)
            _librarySystem.AddBook(new Book("Test Title", "Test Author", isbn, 2025));
            bool actual = _librarySystem.AddBook(duplicateBook);

            //Then: the second book should not be added.
            Assert.IsFalse(actual, "Expected: Adding a book with duplicate ISBN should fail.");

        }

        [TestMethod]
        public void RemoveBook_ShouldRemoveBook()
        {
            //Given: a book to remove
            var book = new Book("Test Title", "Test Author", "0123456789", 2025);
            _librarySystem.AddBook(book);
            //When: removing the book
            bool actual = _librarySystem.RemoveBook(book.ISBN);

            //Then: the book should be removed successfully
            Assert.IsTrue(actual, "Book was not removed successfully.");

        }

        [TestMethod]
        public void RemoveBook_ShouldNotRemoveBook_IsBorrowed()
        {
            //Given: a book that is borrowed
            var book = new Book("Test Title", "Test Author", "0123456789", 2025);
            _librarySystem.AddBook(book);
            book.IsBorrowed = true; // Simulate that the book is borrowed

            //When: trying to remove the borrowed book
            bool actual = _librarySystem.RemoveBook(book.ISBN);

            //Then: the book should not be removed
            Assert.IsFalse(actual, "Expected: Removing a borrowed book should fail.");
        }

        [TestMethod]
        public void SearchByISBN_ShouldReturnBook()
        {
            //Given: a book in the library
            var book = new Book("Test Title", "Test Author", "0123456789", 2025);
            _librarySystem.AddBook(book);
            //When: searching for the book by ISBN
            var actual = _librarySystem.SearchByISBN(book.ISBN);
            //Then: the correct book should be returned
            Assert.IsNotNull(actual, "Expected: Book should be found.");
            Assert.AreEqual(book.Title, actual.Title, "Expected: Book title should match.");
        }

        [TestMethod]
        public void SearchByISBN_ShouldReturnNull_NotFound()
        {
            //Given: a book that is not in the library
            string isbn = "0123456789";
            //When: searching for the book by ISBN
            var actual = _librarySystem.SearchByISBN(isbn);
            //Then: null should be returned
            Assert.IsNull(actual, "Expected: Book should not be found.");
        }

        [TestMethod]
        public void SearchByTitle_PartialTitleAndLowerCases_ReturnTrue() 
        {

            //Given: a book in the library
            _librarySystem.AddBook(new Book("Test This", "Test Author", "0123456789", 2025));
            
            //When: searching for the book by partial title
            var result = _librarySystem.SearchByTitle("this");

            //Then: the correct book should be returned
            Assert.AreEqual(1, result.Count, "Expected: One book should be found.");

        }

        [TestMethod]
        public void BorrowBook_StatusIsBorrowed_ReturnTrue()
        {
            //Given: a book to loan
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            _librarySystem.AddBook(book);

            //When: book is loaned
            _librarySystem.BorrowBook(isbn);
            
            //Then: IsBorrowed = true;
            Assert.IsTrue(book.IsBorrowed, "Expected: book should be borrowed.");

        }

        [TestMethod]
        public void BorrowBook_BorrowLoanedBook_ReturnFalse()
        {
            //Given: a book that is already borrowed
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            _librarySystem.AddBook(book);
            _librarySystem.BorrowBook(isbn);

            //When: trying to loan borrowed book
            bool actual = _librarySystem.BorrowBook(isbn);

            //Then: it should return false
            Assert.IsFalse(actual, "Expected: false when book is already borrowed.");
        }

        [TestMethod]
        public void BorrowBook_CheckBorrowDate_ReturnTrue()
        {
            //Given: a book that is added to the system.
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            _librarySystem.AddBook(book);

            //When: book is borrowed
            var borrowStart = DateTime.Now;
            bool result = _librarySystem.BorrowBook(isbn); 
            var borrowCheck = DateTime.Now;
            var borrowedBook = _librarySystem.SearchByISBN(isbn);

            //Then: it should return false
            Assert.IsTrue(result, "Expected: Book should be borrowed successfully.");
            Assert.IsTrue(
                borrowedBook.BorrowDate >= borrowStart && borrowedBook.BorrowDate <= borrowCheck,
                "Expected: BorrowDate should be between the time interval");

        }
        //TESTING
        
        [TestMethod]
        public void ReturnBook_ShouldResetBorrowDate()
        {
            //Given: a book that is borrowed
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            _librarySystem.AddBook(book);
            _librarySystem.BorrowBook(isbn);

            //When: returning the book
            bool result = _librarySystem.ReturnBook(isbn);

            //Then: the book should be returned successfully and IsBorrowed should be false
            Assert.IsTrue(result, "Expected: Book should be returned successfully.");
            //Assert.IsFalse(book.IsBorrowed, "Expected: Book should not be borrowed after return.");
            Assert.IsNull(book.BorrowDate, "Expected: BorrowDate should be reset to null after return.");
        }
    }
}
