using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftp.Browser.Models
{
    public class MovieFile : IFile
    {
        public string Id { get; }
        public string Title { get; }
        public string Path { get; }
        public List<Subtitle> Subtitles { get; set; }
        public int ReleaseYear { get; }
        public DateTime ReleaseDate { get; }
        public decimal IMDBRating { get; private set; }
        public int IMdbUsers { get; }
        public decimal RottenTomatoRating { get; private set; }
        public string Director { get; private set; }
        public string Producers { get; private set; }
        public string Writer { get; set; }
        public string Stars { get; set; }
        public string Genres { get; private set; }
        public int Duration { get; }
    }

    public class Subtitle
    {
        public string MovieName { get; set; }
        public string FilePath { get; set; }
        public string Language { get; set; }
    }

    public interface IFile
    {
        string Id { get; }
        string Title { get; }
        string Path { get; }
    }
}
