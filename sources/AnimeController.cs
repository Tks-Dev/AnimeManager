using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Anime_Manager
{
    class AnimeController
    {
        public const int PROP_TYPE = 1;
        public const int PROP_LANG = 2;
        public const int PROP_SUB = 3;

        public static void fill(DataGrid table, AnimeModel model)
        {
            table.AutoGenerateColumns = true;
            table.ItemsSource = (from tab in model.AsStringArraysList()
                                 select new
                                 {
                                     Nom = tab[0],
                                     Saison = tab[1],
                                     Année = tab[5],
                                     Langue = tab[7],
                                     Sous__Titres = tab[8],
                                     Episodes = tab[6]
                                 });
        }

        public static void fill(ComboBox cbox, int kind, AnimeModel model, bool emptyRow)
        {
            if (kind < PROP_TYPE || kind > PROP_SUB)
                return;
            List<string> l = new List<string>();
            if (kind == PROP_TYPE)
                foreach (Anime a in model)
                    foreach (string type in a.Type.Split(';'))
                        if (!l.Contains(type))
                            l.Add(type.Trim());
            if (kind == PROP_LANG)
                foreach (Anime a in model)
                    if (!l.Contains(a.Language))
                        l.Add(a.Language);
            if (kind == PROP_SUB)
                foreach (Anime a in model)
                    if (!l.Contains(a.Sub))
                        l.Add(a.Sub);
            cbox.ItemsSource = l;
            if (emptyRow)
                l.Add("");
        }
    }
}
