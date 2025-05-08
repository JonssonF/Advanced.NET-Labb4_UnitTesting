using Advanced.NET_Labb4_UnitTesting;

namespace LibrarySystemTests
{
    [TestClass]
    public class LibrarySystemTests
    {
        private LibrarySystem? _librarySystem;
        public TestContext? TestContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            _librarySystem = new LibrarySystem(new List<Book> ());
        }

        [TestMethod]
        [TestCategory("Adding")]
        public void AddBook_ShouldAdd_ValidBook()
        {
            var library = new LibrarySystem(new List<Book>());
            
            //Given: a book with unique ISBN
            var book = new Book("Test Title", "Test Author", "0123456789", 2025);

            //When: try to add the book to the library
            bool result = library.AddBook(book);

            //Then: the book should be added successfully and retrievable by ISBN
            Assert.IsTrue(result, "Book couldn't be added successfully.");
        }


        [TestMethod]
        [TestCategory("Adding")]
        public void AddBook_ShouldNotAdd_DuplicateISBN()
        {
            var library = new LibrarySystem(new List<Book>());

            //Given: two books with the same ISBN.
            string isbn = "0123456789";
            var duplicateBook = new Book("Test Title", "Test Author", isbn, 2025);

            //When: adding the first book(successfully) and the second book(unsuccessfully)
            library.AddBook(new Book("Test Title", "Test Author", isbn, 2025));
            bool actual = library.AddBook(duplicateBook);

            //Then: the second book should not be added.
            Assert.IsFalse(actual, "Expected: Adding a book with different ISBN should fail.");

        }

        [TestMethod]
        [TestCategory("Removing")]
        public void RemoveBook_ShouldRemoveBook()
        {
            //Given: a book to remove
            var book = new Book("Test Title", "Test Author", "0123456789", 2025);
            var library = new LibrarySystem(new List<Book> { book }); //Adding the book to the library

            //When: removing the book
            bool actual = library.RemoveBook(book.ISBN);

            //Then: the book should be removed successfully
            Assert.IsTrue(actual, "Book was not removed successfully.");
        }

        [TestMethod]
        [TestCategory("Removing")]
        public void RemoveBook_ShouldNotRemoveBook_IsBorrowed()
        {
            //Given: a book that is borrowed
            var book = new Book("Test Title", "Test Author", "0123456789", 2025);
            book.IsBorrowed = true; // Simulate that the book is borrowed
            var library = new LibrarySystem(new List<Book> { book });

            //When: trying to remove the borrowed book
            bool actual = _librarySystem.RemoveBook(book.ISBN);

            //Then: the book should not be removed
            Assert.IsFalse(actual, "Expected: Removing a borrowed book should fail.");
        }

        [TestMethod]
        [TestCategory("Searching")]
        public void SearchExactISBN_ShouldReturnBook()
        {
            //Given: a book in the library
            var book = new Book("Test Title", "Test Author", "0123456789", 2025);
            var library = new LibrarySystem(new List<Book> { book });

            //When: searching for the book by ISBN
            var actual = library.FindExactISBN(book.ISBN);

            //Then: the correct book should be returned
            Assert.IsNotNull(actual, "Expected: Book should be found.");
            Assert.AreEqual(book.Title, actual.Title, "Expected: Book title should match.");
        }

        [TestMethod]
        [TestCategory("Searching")]
        public void SearchExactISBN_ShouldReturnNull_NotFound()
        {
            //CTFT
            /*
            string existentISBN = "0123456789";
            var book = new Book("Test Title", "Test Author", existentISBN, 2025);
            var library = new LibrarySystem(new List<Book> { book });
            */

            //Given: a book that is not in the library
            string nonExistentISBN = "9876543210";

            //When: searching for the book by ISBN
            var actual = _librarySystem.FindExactISBN(nonExistentISBN);

            //Then: null should be returned
            Assert.IsNull(actual, "Expected: Book should not be found.");
        }

        [TestMethod]
        [TestCategory("Searching")]
        public void SearchByTitle_PartialTitleAndLowerCases_ReturnTrue()
        {
            //Given: a book in the library
            var book = new Book("Test This", "Test Author", "0123456789", 2025);
            var library = new LibrarySystem(new List<Book> { book });

            //When: searching for the book by partial title
            var result = library.SearchByTitle("thi");

            //Then: the correct book should be returned
            Assert.AreEqual(1, result.Count, "Expected: One book should be found.");
        }

        [TestMethod]
        [TestCategory("Searching")]
        public void SearchByAuthor_PartialAuthorAndLowerCases_ReturnExpectedResult()
        {
            //Given: some books to add to the library
            var book = new Book("Test This", "Bellman", "0123456779", 2025);
            var book1 = new Book("Test Thoose", "Batman", "0123456789", 2025);
            var book2 = new Book("Test That", "The Man", "0123456799", 2025);
            var library = new LibrarySystem(new List<Book> { book, book1, book2 });

            string splitAuthor = "man";

            //When: searching for the book by partial author
            var result = library.SearchByAuthor(splitAuthor);

            //Then: the matching books should be returned
            int expected = library.GetAllBooks().Count;

            TestContext!.WriteLine($"Searched for {splitAuthor}");
            foreach(var b in result)
            {
                TestContext.WriteLine($"Found: {b.Author}");
            }
            TestContext!.WriteLine($"Expected: {expected} Actual: {result.Count}.");
            Assert.AreEqual(expected, result.Count, $"Expected: {expected} Actual: {result.Count}.");

        }

        [TestMethod]
        [TestCategory("Searching")]
        public void SearchByISBN_PartialISBN_ReturnMachingResults()
        {
            //Given: a book to add to the library
            var book = new Book("Test This", "Bellman", "0123333333", 2025);
            var book1 = new Book("Test Thoose", "Batman", "0123444444", 2025);
            var book2 = new Book("Test That", "The Man", "0123555555", 2025);
            var library = new LibrarySystem(new List<Book> { book, book1, book2 });

            string splitISBN = "0123";

            //When: searching for the book by partial author
            var result = library.SearchByISBN(splitISBN);
            int expected = library.GetAllBooks().Count;

            //Then: the matching books should be returned
            TestContext!.WriteLine($"Searched for {splitISBN}");
            foreach (var b in result)
            {
                TestContext.WriteLine($"Found: {b.Title} - {b.ISBN}");
            }
            Assert.AreEqual(expected, result.Count, $"Expected: {expected} Actual: {result.Count}.");
        }


        [TestMethod]
        [TestCategory("Borrowing")]
        public void BorrowBook_StatusIsBorrowed_ReturnTrue()
        {
            //Given: a book to loan
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            var library = new LibrarySystem(new List<Book> { book });

            //When: book is loaned
            library.BorrowBook(isbn);

            //Then: IsBorrowed = true;
            Assert.IsTrue(book.IsBorrowed, "Expected: book should be borrowed.");

        }

        [TestMethod]
        [TestCategory("Borrowing")]
        public void BorrowBook_BorrowLoanedBook_ReturnFalse()
        {
            //Given: a book that is already borrowed
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            var library = new LibrarySystem(new List<Book> { book });
            book.IsBorrowed = true;

            //When: trying to loan borrowed book
            bool actual = library.BorrowBook(isbn);

            //Then: it should return false
            Assert.IsFalse(actual, "Expected: false when book is already borrowed.");
        }

        [TestMethod]
        [TestCategory("Borrowing")]
        public void BorrowBook_CheckBorrowDate_ReturnTrue()
        {
            //Given: a book that is added to the system.
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            var library = new LibrarySystem(new List<Book> { book });
            
            //When: book is borrowed
            var borrowStart = DateTime.Now.AddSeconds(-1);
            bool result = library.BorrowBook(isbn);
            var borrowCheck = DateTime.Now.AddSeconds(1);


            //Then: it should return true and be between the time interval
            Console.WriteLine($"Start: {borrowStart.ToString()}");
            Console.WriteLine($"Actual: {book.BorrowDate.ToString()}");
            Console.WriteLine($"End: {borrowCheck.ToString()}");
            Assert.IsTrue(result, "Expected: Book should be borrowed successfully.");
            Assert.IsTrue(
                book.BorrowDate >= borrowStart && book.BorrowDate <= borrowCheck,
                "Expected: BorrowDate should be between the time interval");

        }

        [TestMethod]
        [TestCategory("Returning")]
        public void ReturnBook_ShouldResetBorrowDate()
        {

            //Given: a book that is borrowed
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            var library = new LibrarySystem(new List<Book> { book });
            book.IsBorrowed = true; // Simulate that the book is borrowed
            book.BorrowDate = DateTime.Now.AddMinutes(-5); // Simulate that the book has a borrow date

            //When: returning the book
            bool result = library.ReturnBook(isbn);

            //Then: expected results should be fulfilled
            Assert.IsTrue(result, "Expected: Book should be returned successfully.");
            Assert.IsFalse(book.IsBorrowed, "Expected: Book should not be borrowed after return.");
            Assert.IsNull(book.BorrowDate, "Expected: BorrowDate should be reset to null after return.");
        }

        [TestMethod]
        [TestCategory("Returning")]
        public void ReturnBook_ShouldNotReturn_NotBorrowed()
        {
            //Given: a book that is not borrowed
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            var library = new LibrarySystem(new List<Book> { book });
            book.IsBorrowed = false; // Simulate that the book is not borrowed

            //When: trying to return the book
            bool result = library.ReturnBook(isbn);

            //Then: the book should not be returned
            TestContext!.WriteLine($"Trying to return book with ISBN: {isbn}");
            TestContext.WriteLine($"Book status: Borrowed = {book.IsBorrowed}");
            TestContext.WriteLine($"Cannot return a book that isn't borrowed.");
            Assert.IsFalse(result, "Expected: Book should not be returned if it is not borrowed.");
        }

        [TestMethod]
        [TestCategory("Overdue")]
        public void IsBookOverdue_ShouldReturnTrue_Overdue()
        {
            //Given: a book that is borrowed
            string isbn = "0123456789";
            var book = new Book("Test Title", "Test Author", isbn, 2025);
            _librarySystem.AddBook(book);
            _librarySystem.BorrowBook(isbn);
            //When: checking if the book is overdue
            bool result = _librarySystem.IsBookOverdue(isbn, 7);
            //Then: it should return true
            Assert.IsTrue(result, "Expected: Book should be overdue.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up resources after each test
            _librarySystem = null;
        }
        
        }
    }
