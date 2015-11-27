using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anime_Manager
{
    public class Anime
    {
        //public string Id { get; private set; }
        public string Name { get; set; }
        public string Season { get; set; }
        public string Studio { get; set; }
        public string Fansub { get; set; }
        public int Year { get; set; }
        public int NumberOfEpisode { get; set; }
        public string Language { get; set; }
        public string Sub { get; set; }
        public string Synopsis { get; set; }
        public string Type { get; set; }
        public string Directory { get; set; }

        public Anime(string name, string season, string studio, string fansub, int year, int episodes, string language, string sub, string synopsis, string type, string directory)
        {
            //Id = id;
            Name = name;
            Season = season;
            Studio = studio;
            Fansub = fansub;
            Year = year;
            NumberOfEpisode = episodes;
            Language = language;
            Sub = sub;
            Synopsis = synopsis;
            Type = type;
            Directory = directory;
        }

        public string[] AsArray()
        {
            return new string[] { Name, Season, Type, Studio, Fansub, Year.ToString(), NumberOfEpisode.ToString(), Language, Sub, Synopsis, Directory};
        }

        public bool Equals(Anime a)
        {
            return Name == a.Name && Season == a.Season && Language == a.Language && Sub == a.Sub;
        }

        public bool StrictEquals(Anime a)
        {
            return a.toString() == this.toString();
        }

        public string toString()
        {
            string s = "{";
            foreach (string str in this.AsArray())
                s += str + ",";
            s = s.Remove(s.Length - 1);
            s += "}";
            return s;
        }
    }
}
