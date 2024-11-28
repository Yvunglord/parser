using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Leaf.xNet;

namespace Parsing
{
    public class BookParser
    {
        private string responce;
        public BookParser() 
        {
            responce = string.Empty;
        }

        private async Task GetResponseAsync(string url)
        {
            HttpRequest request = new HttpRequest();
            request.KeepAlive = true;
            request.UserAgent = Http.IEUserAgent();
            request.ConnectTimeout = 500;
            request.KeepAliveTimeout = 5000;

            responce = request.Get(url).ToString();
        }

        public async Task<List<BookSample>> GetRangeBooksAsync(int count, string baseUrl)
        {
            List<BookSample> books = new List<BookSample>();
            int p = 1;
            int i = 0;
            baseUrl = baseUrl.Replace("{1}", p.ToString());

            while (i < count)
            {
                var task = GetBooksByUrlAsync(baseUrl);
                var res = await task;

                books.AddRange(res);

                p += 1;
                i += res.Length;
            }

            return books;
        }

            private async Task<BookSample[]> GetBooksByUrlAsync(string url)
        {
            await GetResponseAsync(url);

            IHtmlParser parser = new HtmlParser();

            var body = parser.ParseDocument(responce).Body;

            var root = body.Children
                .Where(el => el.Id != null && el.Id.ToLower().Contains("a-page")).FirstOrDefault()?.Children
                .Where(el => el.Id != null && el.Id.ToLower().Contains("search")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("desktop-width")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("20-of-24")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("section")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("s-main-slot")).FirstOrDefault()?.Children
                .Where(el => el.HasAttribute("data-uuid") && el.HasAttribute("data-component-type"));

            return root.Select(el => GetBooksFromItem(el)).ToArray();
        }

        private BookSample GetBooksFromItem(IElement item)
        {
            var root = item.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("span")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("puisg-row")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("list-col-right")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault();

            var nameAndhref = root?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("list-col-right")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("h2")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("a")).FirstOrDefault();

            var href = nameAndhref?.GetAttribute("href");

            var name = nameAndhref?.Children
                .Where(el => el.TagName.ToLower().Equals("span")).FirstOrDefault()?.TextContent;

            var authorAndDate = root?.Children
                .Where(el => el.ClassName.ToLower().Contains("title-recipe")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("a-row")).FirstOrDefault();

            var author = authorAndDate?.Children
                .Where(el => el.TagName.ToLower().Equals("a")).FirstOrDefault()?.TextContent;

            var date = authorAndDate?.Children
                .Where(el => el.TagName.ToLower().Equals("span") && el.ClassName.ToLower().Contains("a-size-base a-color-secondary a-text-normal")).FirstOrDefault()?.TextContent;

            var rating = root?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("top-micro")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => (el.ClassName == null || el.ClassName == string.Empty) && el.TagName.ToLower().Equals("span"))
                .FirstOrDefault()?.GetAttribute("aria-label");

            var coast = root?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("puisg-row")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => !el.HasAttribute("data-cy")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.TagName.ToLower().Equals("div")).FirstOrDefault()?.Children
                .Where(el => el.ClassName != null && el.ClassName.ToLower().Contains("price")).FirstOrDefault()?.TextContent;

            string c = root.InnerHtml.ToString();
            string r = Regex.Match(c, @"\$\d+\.\d+").Value;
            if (coast == null) coast = "";
            return new BookSample(name, author, rating, r, date);
        }
    }
}
