using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonita_A2
{
    [Serializable]
    public class Movie
    {
        public string movieName { get; set; }
        public DateTime releaseDate { get; set; }
        public string location { get; set; }
        public string genre { get; set; }
        public int rating { get; set; }
        int duration { get; set; }
        public double price { get; set; }

        public Movie(string movieName, DateTime releaseDate, string location, string genre, int rating, int duration, double price)
        {
            this.movieName = movieName;
            this.releaseDate = releaseDate;
            this.location = location;
            this.genre = genre;
            this.rating = rating;
            this.duration = duration;
            this.price = price;
        }

        public override string ToString()
        {
            return $"{movieName}, {releaseDate.ToShortDateString()}, {location}, {genre}, {rating}, {duration}, {price}";
        }
    }
}
