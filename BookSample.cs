using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing
{
    public class BookSample
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Rating { get; set; }
        public string Date { get; set; }
        public string Coast { get; set; }
        public string Href { get; set; }

        public BookSample(string title, string author, string rating, string coast, string href, string date = "")
        {
            Title = title;
            Author = author;
            Rating = rating;
            Date = date;
            Coast = coast;
            Href = href;
        }
    }
}
