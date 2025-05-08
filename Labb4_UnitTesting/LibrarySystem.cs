using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advanced.NET_Labb4_UnitTesting
{
    public class LibrarySystem
    {
        private List<Book> books;

        public LibrarySystem(List<Book> testBookList)
        {
            books = testBookList ?? GetDefaultBooks();
        }

        private List<Book> GetDefaultBooks()
        {
            return new List<Book>
            {
                new Book("1984", "George Orwell", "9780451524935", 1949),
                new Book("To Kill a Mockingbird", "Harper Lee", "9780061120084", 1960),
                new Book("The Great Gatsby", "F. Scott Fitzgerald", "9780743273565", 1925),
                new Book("The Hobbit", "J.R.R. Tolkien", "9780547928227", 1937),
                new Book("Pride and Prejudice", "Jane Austen", "9780141439518", 1813),
                new Book("The Catcher in the Rye", "J.D. Salinger", "9780316769488", 1951),
                new Book("Lord of the Flies", "William Golding", "9780399501487", 1954),
                new Book("Brave New World", "Aldous Huxley", "9780060850524", 1932)
            };
        }

        public int BookCount => books.Count;

        /*------------------------------------------------------------------------------------*/
        public bool AddBook(Book book)
        {
            //BUG 1 - Added if-statement so books can't be duplicate.
            if (books.Any(b => b.ISBN == book.ISBN)) 
            {
                return false;
            }
            //BUG 2 - Added if-statement so books can't be whitespace or null.
            if (string.IsNullOrWhiteSpace(book.ISBN))
            {
                return false;
            }
            //BUG 5 - Added if-statement to check if book is null.
            if (book == null)
            {
                return false;
            }

            books.Add(book);
            return true;
        }
        /*------------------------------------------------------------------------------------*/
        //BUG 3 - Altered if-statement to include IsBorrowed and to see if the book is null.
        //And checking if the match get several hits.
        public bool RemoveBook(string isbn)
        {
            var matches = SearchByISBN(isbn);

            if (matches.Count() == 0)
            {
                Console.WriteLine("Could not find a book with matching ISBN.");
                return false;
            }

            if (matches.Count > 1)
            {
                Console.WriteLine("We found several matches for your search, be more specific.");
                foreach(var b in matches)
                {
                    Console.WriteLine($"-{b.Title} - ({b.ISBN})");
                }
                return false;
            }
            var book = matches.First();
            if (book.IsBorrowed)
            {
                Console.WriteLine("\nThe book is already borrowed and cannot be removed.");
                return false;
            }
            books.Remove(book);
            return true;
        }
        /*------------------------------------------------------------------------------------*/
        //BUG 6 - Changed the method to return a list of books instead of a single book.
        public List<Book> SearchByISBN(string isbn)
        {
            return books
            .Where(b => b.ISBN.Contains(isbn, StringComparison.OrdinalIgnoreCase))
            .ToList();
        }
        /*------------------------------------------------------------------------------------*/
        //Bug 7 - Added a method to match exact ISBN to avoid false positives.
        public Book? FindExactISBN(string isbn)
        {
            return books
                .FirstOrDefault(b => b.ISBN.Equals(isbn, StringComparison.OrdinalIgnoreCase));
        }
        /*------------------------------------------------------------------------------------*/
        //BUG 4 - Added StringComparison.OrdinalIgnoreCase to make the search case-insensitive.
        public List<Book> SearchByTitle(string title)
        {
            return books
                .Where(b => b.Title
                .Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        /*------------------------------------------------------------------------------------*/
        public List<Book> SearchByAuthor(string author)
        {
            return books
                .Where(b => b.Author
                .Contains(author, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        /*------------------------------------------------------------------------------------*/
        //BUG 8 - Changed the method to return a list of books instead of a single book.
        //Also added a check to see if the book is null / borrowed.
        public bool BorrowBook(string isbn)
        {
            var matches = SearchByISBN(isbn);

            if (matches.Count == 0)
            {
                Console.WriteLine("Couldn't find a book with matching ISBN.");
                return false;
            }

            if (matches.Count > 1)
            {
                Console.WriteLine($"Provided ISBN:{isbn} had several matches.");
                Console.WriteLine($"Please be more specific if u want to borrow.\n");
                foreach (var match in matches)
                {
                    Console.WriteLine($"- {match.Title} - ({match.ISBN})");
                }
                return false;
            }

            var book = matches.First();
            if (book.IsBorrowed)
            {
                Console.WriteLine($"The book {book.Title} is already borrowed.");
                return false;
            }

            book.IsBorrowed = true;
            book.BorrowDate = DateTime.Now;
            Console.WriteLine($"\nYou have succesfully borrowed book with provided ISBN: {book.ISBN}");
            Console.WriteLine($"-{book.Title} / {book.Author} / {book.BorrowDate}");
            return true;
        }
        /*------------------------------------------------------------------------------------*/
        // BUG 10 - Added a reset to the borrow date when returning a book.
        //Implemented new method to match exact ISBN to avoid false positives.
        public bool ReturnBook(string isbn)
        {
            var book = FindExactISBN(isbn);
            if (book != null && book.IsBorrowed)
            {
                book.IsBorrowed = false;
                book.BorrowDate = null;
                Console.WriteLine($"We hope you enjoyed reading {book.Title}.");
                return true;
            }
            return false;
        }
        /*------------------------------------------------------------------------------------*/
        public List<Book> GetAllBooks()
        {
            return books;
        }
        /*------------------------------------------------------------------------------------*/
        public decimal CalculateLateFee(string isbn, int daysLate)
        {
            if (daysLate <= 0)
                return 0;

            var book = FindExactISBN(isbn);
            if (book == null)
                return 0;
            //Added multiplying instead of adding.
            decimal feePerDay = 0.5m;
            return daysLate * feePerDay;
        }
        /*------------------------------------------------------------------------------------*/
        public bool IsBookOverdue(string isbn, int loanPeriodDays)
        {
            var book = FindExactISBN(isbn);
            if (book != null && book.IsBorrowed && book.BorrowDate.HasValue)
            {
                TimeSpan borrowedFor = DateTime.Now - book.BorrowDate.Value;
                return borrowedFor.Days > loanPeriodDays;
            }
            return false;
        }
        /*------------------------------------------------------------------------------------*/
    }
}
