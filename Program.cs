using System;
using System.Net;
using System.IO;
using Parsing;

public class HtmlDownloader
{
    static async Task Main(string[] args)
    {
        BookParser parser = new BookParser();
        string baseUrl = "https://www.amazon.com/s?k=python&i=stripbooks-intl-ship&ref=nb_sb_noss"; 
        int count = 10; 

        List<BookSample> books = await parser.GetRangeBooksAsync(count, baseUrl);

        foreach (var book in books)
        {
            Console.WriteLine(book);
        }
    }
}
