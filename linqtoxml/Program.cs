using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;

namespace LinqToXml
{
    class Program
    {
        static XElement _movies;

        static void Main(string[] args)
        {
            // Filvägen
            var path = Application.StartupPath + "/../../Movies.xml";
            // Ladda in xml-dokumentet i minnet
            _movies = XElement.Load(path);

            //Uppgift 1
            Console.WriteLine("Uppgift 1 - All Titles: ");
            var allTitles = AllTitles();
            foreach (XElement title in allTitles)
            {
                Console.WriteLine(title);
            }

            //Uppgift 2
            Console.WriteLine("Uppgift 2 - All Categories: ");
            var allCategories = AllCategories();
            foreach (String category in allCategories)
            {
                Console.WriteLine(category);
            }

            //Uppgift 3
            Console.WriteLine("Uppgift 3 - Movies from Year: " + MoviesFromYear(1992));

            //Uppgift 4
            Console.WriteLine("Uppgift 4 - Movies from Category" + MoviesFromCategory("Music"));

            //Uppgift 5
            Console.WriteLine("Uppgift 5 - Average UserVotes: " + Math.Round(AverageUserVotes(), 2));

            //Uppgift 6
            Console.WriteLine("Uppgift 6 - Top Movies: " + TopMovies(3));

            //Uppgift 7
            Console.WriteLine("Uppgift 7 - Movies With Title Containing: " + MoviesWithTitleContaining("Good"));

            //Uppgift 8
            Console.WriteLine("Uppgift 8 - Movies In Category: " + MoviesInCategory("Drama"));

            //Uppgift 9
            Console.WriteLine("Uppgift 9 - Single Category Movies: " + SingleCategoryMovies("Drama"));

            Console.ReadLine();
        }

        /// <summary>
        /// Uppgift 1
        /// </summary>
        /// <returns>Alla "Title"-noder</returns>
        public static IEnumerable<XElement> AllTitles()
        {
            IEnumerable<XElement> allTitles = from movie in _movies.Descendants("Movie")
                                              select movie.Element("Title");
            return allTitles;
        }

        /// <summary>
        /// Uppgift 2
        /// </summary>
        /// <returns>En lista med alla kategorier i klartext.</returns>
        public static IEnumerable<string> AllCategories()
        {
            IEnumerable<string> allCategories = (from movie in _movies.Descendants("Categories")
                                                 select (string)movie.Element("Category")).ToList().Distinct();
            return allCategories;
        }

        /// <summary>
        /// Uppgift 3
        /// </summary>
        /// <returns>Alla movie-noder (och de noder som Movie omsluter) från ett angivet årtal.</returns>
        public static XElement MoviesFromYear(int year)
        {
            XElement moviesFromYear =
                new XElement("MoviesFromYear",
                    new XElement("Year", year),
                    new XElement("Movies",
                        new XElement("Movie",
                        from movie in _movies.Descendants("Movie")
                        where (int)movie.Element("Year") == year
                        select movie)));

            return moviesFromYear;
        }

        /// <summary>
        /// Uppgift 4
        /// </summary>
        /// <returns>Alla movie-noder (och dess titel) från en angiven kategori.</returns>
        public static XElement MoviesFromCategory(string name)
        {
            XElement moviesFromCategory =
                new XElement("CategorySearch",
                    new XElement("Category", name),
                    new XElement("Movies",
                        from movie in _movies.Descendants("Movie")
                        where (string)movie.Element("Categories").Element("Category") == name
                        select movie));

            return moviesFromCategory;
        }

        /// <summary>
        /// Uppgift 5
        /// </summary>
        /// <returns>Medelvärdet av UserVotes i XML:en.</returns>
        public static double AverageUserVotes()
        {
            var allUserVotes = from movie in _movies.Descendants("Movie")
                                      select (double)movie.Element("UserVotes");

            return allUserVotes.Average();
        }

        /// <summary>
        /// Uppgift 6
        /// </summary>
        /// <returns>De movie-noder (och de noder som Movie omsluter) som är "bäst". Dvs. har högst rating.</returns>
        public static XElement TopMovies(int numberOfMovies)
        {
            string timeNow = DateTime.Now.ToString();
            XElement topMovies =
                new XElement("TopMovies",
                    new XElement("TimeOfRequest", timeNow),
                    new XElement("NumberOfMovies", numberOfMovies),
                    new XElement("Movies", 
                                (from movie in _movies
                                .Descendants("Movie")
                                .OrderByDescending(m => (double)m.Element("Rating"))
                                select movie).Take(10)));

            return topMovies;
        }

        /// <summary>
        /// Uppgift 7
        /// </summary>
        /// <returns>De movie-noder (och de noder som Movie omsluter) som innehåller nyckelordet i dess titel.</returns>
        public static XElement MoviesWithTitleContaining(string keyword)
        {
            XElement moviesWithTitleContainingKeyWord =
                new XElement("TitleSearch",
                    new XElement("SearchString", keyword),
                    new XElement("Movies", 
                                from movie in _movies.Descendants("Movie")
                                where ((string)movie.Element("Title")).Contains(keyword)
                                select movie));

            return moviesWithTitleContainingKeyWord;
        }

        /// <summary>
        /// Uppgift 8
        /// </summary>
        /// <returns>Antalet filmer som tillhör den angivna kategorin i XML-format.</returns>
        public static XElement MoviesInCategory(string cat)
        {
            XElement moviesInCategory = 
                new XElement("SingleCategoryMovies", 
                    new XElement("Category", cat),
                    new XElement("NumberOfMovies",
                                (from movie in _movies.Descendants("Movie")
                                where (string)movie.Element("Categories").Element("Category") == cat
                                select movie.Element("Title")).Count()));

            return moviesInCategory;
        }

        /// <summary>
        /// Uppgift 9
        /// </summary>
        /// <returns>Titlarna för alla filmer som har en och endast en kategori. Denna kategori är den som anges som parameter.</returns>
        public static XElement SingleCategoryMovies(string cat)
        {
            XElement singleCategoryMovies =
                new XElement("SingleCategoryMovies",
                    new XElement("Category", cat),
                    new XElement("Titles",
                                from movie in _movies.Descendants("Movie")
                                where movie.Descendants("Category").Count() == 1 &&
                                     (string)movie.Element("Categories").Element("Category") == cat
                                select movie.Element("Title")));

            return singleCategoryMovies;
        }
    }
}
