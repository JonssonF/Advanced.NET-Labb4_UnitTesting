namespace Advanced.NET_Labb4_UnitTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LibrarySystem library = new LibrarySystem(null);
            UserInterface.DisplayMenu(library);
        }
    }
}
