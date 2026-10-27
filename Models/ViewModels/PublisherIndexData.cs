using Mesaros_David_Lab2.Models;

namespace Mesaros_David_Lab2.ViewModels
{
    public class PublisherIndexData
    {
        public IEnumerable<Publisher> Publishers { get ; set ; }
        public IEnumerable<Book> Books { get ; set ; }
    }
}