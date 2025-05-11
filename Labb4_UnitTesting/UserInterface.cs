using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advanced.NET_Labb4_UnitTesting
{
    public static class UserInterface
    {
        public static void DisplayMenu(LibrarySystem library)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Library Management System ===");
                Console.WriteLine("1. Add a book");
                Console.WriteLine("2. Remove a book");
                Console.WriteLine("3. Search for a book");
                Console.WriteLine("4. Borrow a book");
                Console.WriteLine("5. Return a book");
                Console.WriteLine("6. Display all books");
                Console.WriteLine("7. Calculate late fees");
                Console.WriteLine("8. Exit");
                Console.Write("Enter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            UserInterface.AddBookUI(library);
                            break;
                        case 2:
                            UserInterface.RemoveBookUI(library);
                            break;
                        case 3:
                            UserInterface.SearchBookUI(library);
                            break;
                        case 4:
                            UserInterface.BorrowBookUI(library);
                            break;
                        case 5:
                            UserInterface.ReturnBookUI(library);
                            break;
                        case 6:
                            UserInterface.DisplayAllBooksUI(library);
                            break;
                        case 7:
                            UserInterface.CalculateLateFeeUI(library);
                            break;
                        case 8:
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        public static void AddBookUI(LibrarySystem library)
        {
            Console.Clear();
            Console.WriteLine("=== Add a Book ===");

            Console.Write("Enter title: ");
            string title = Console.ReadLine();

            Console.Write("Enter author: ");
            string author = Console.ReadLine();

            Console.Write("Enter ISBN: ");
            string isbn = Console.ReadLine();

            Console.Write("Enter publication year: ");
            if (int.TryParse(Console.ReadLine(), out int year))
            {
                Book book = new Book(title, author, isbn, year);
                bool success = library.AddBook(book);

                if (success)
                {
                    Console.WriteLine("Book added successfully!");
                }
                else
                {

                    Console.WriteLine("Failed to add book. ISBN might already exist or is null.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Invalid year format.");
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void RemoveBookUI(LibrarySystem library)
        {
            Console.Clear();
            Console.WriteLine("=== Remove a Book ===");

            Console.Write("Enter ISBN of book to remove: ");
            string isbn = Console.ReadLine();

            bool success = library.RemoveBook(isbn);

            if (success)
                Console.WriteLine("Book removed successfully!");
            else
                Console.WriteLine("Failed to remove book.");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void SearchBookUI(LibrarySystem library)
        {
            Console.Clear();
            Console.WriteLine("=== Search for a Book ===");
            Console.WriteLine("1. Search by Title");
            Console.WriteLine("2. Search by Author");
            Console.WriteLine("3. Search by ISBN");
            Console.Write("Enter your choice: ");
            string searchTerm = "";
            //BUG 9 - Removed the if-statement to check if ISBN is null when searching for a book.
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                List<Book> results = new List<Book>();

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter title: ");
                        searchTerm = Console.ReadLine();
                        results = library.SearchByTitle(searchTerm);
                        break;
                    case 2:
                        Console.Write("Enter author: ");
                        searchTerm = Console.ReadLine();
                        results = library.SearchByAuthor(searchTerm);
                        break;
                    case 3:
                        Console.Write("Enter ISBN: ");
                        searchTerm = Console.ReadLine();
                        results = library.SearchByISBN(searchTerm);
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                if (results.Count > 1)
                {
                    Console.WriteLine($"\nProvided search term /{searchTerm}/ gave several matches:\n");
                    foreach (var book in results)
                    {
                        Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, ISBN: {book.ISBN}, Year: {book.PublicationYear}, Available: {!book.IsBorrowed}");
                    }

                    Console.WriteLine("Would you like to narrow your search?");
                    Console.WriteLine("1. Yes");
                    Console.WriteLine("2. No");
                    Console.Write("Enter your choice: ");
                    if (int.TryParse(Console.ReadLine(), out int narrowChoice))
                    {
                        if (narrowChoice == 1)
                        {
                            Console.Write("Please enter specific ISBN: ");
                            string newSearchTerm = Console.ReadLine();
                            var book = library.FindExactISBN(newSearchTerm);

                            if (results.Count > 0)
                            {
                                Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, ISBN: {book.ISBN}, Year: {book.PublicationYear}, Available: {!book.IsBorrowed}");
                            }
                            else
                            {
                                Console.WriteLine($"No matches found for {newSearchTerm}.");
                            }
                        }
                        else if (narrowChoice == 2)
                        {
                            Console.WriteLine("Search cancelled.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice.");
                        }
                    }
                }
                else if (results.Count == 1)
                {
                    var book = results[0];
                    Console.WriteLine($"\nWe found one match for {searchTerm}:\n");
                    Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, ISBN: {book.ISBN}, Year: {book.PublicationYear}, Available: {!book.IsBorrowed}\n");
                }
                else
                {
                    Console.WriteLine($"No books found matching your search /{searchTerm}/.");
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void BorrowBookUI(LibrarySystem library)
        {
            Console.Clear();
            Console.WriteLine("=== Borrow a Book ===");

            Console.Write("Enter ISBN of book to borrow: ");
            string isbn = Console.ReadLine();

            bool success = library.BorrowBook(isbn);

            if (success)
                Console.WriteLine("\nBook borrowed successfully!");
            else
                Console.WriteLine("\nFailed to borrow book.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
        }

        public static void ReturnBookUI(LibrarySystem library)
        {
            Console.Clear();
            Console.WriteLine("=== Return a Book ===");

            Console.Write("Enter ISBN of book to return: ");
            string isbn = Console.ReadLine();

            bool success = library.ReturnBook(isbn);

            if (success)
                Console.WriteLine("Book returned successfully!");
            else
                Console.WriteLine("Failed to return book. ISBN might not exist or book is not currently borrowed.");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void DisplayAllBooksUI(LibrarySystem library)
        {
            Console.Clear();
            Console.WriteLine("=== All Books ===");

            var books = library.GetAllBooks();

            if (books.Count > 0)
            {
                foreach (var book in books)
                {
                    Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, ISBN: {book.ISBN}, Year: {book.PublicationYear}, Available: {!book.IsBorrowed}");
                }
            }
            else
            {
                Console.WriteLine("No books in the library.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        public static void CalculateLateFeeUI(LibrarySystem library)
        {
            Console.Clear();
            Console.WriteLine("=== Calculate Late Fee ===");

            Console.Write("Enter ISBN of book: ");
            string isbn = Console.ReadLine();

            Console.Write("Enter number of days late: ");
            if (int.TryParse(Console.ReadLine(), out int daysLate))
            {
                decimal fee = library.CalculateLateFee(isbn, daysLate);
                Console.WriteLine($"Late fee: ${fee}");
            }
            else
            {
                Console.WriteLine("Invalid number of days.");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
