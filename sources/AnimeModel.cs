using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using License_Manager;
using System.Windows;

namespace Anime_Manager
{
    public class AnimeModel : List<Anime>
    {
        private const string DB_FILE = "anime.sqlite";

        private string file;
        public Anime FAKE_ANIME { get; private set; }
        private AnimeModel() 
        {
            FAKE_ANIME = new Anime("NO ANIME", "Saison 0", "Aucun", "Aucun", 42, 42, "Klingon", "Elfique", "C'est l'histoire d'un BD vide. Un jour, elle est chargee et cela change completement la vie de l'utilisateur. Deception, rage, on ne peut rien faire a part redemarrer le programme.", "NONE", "DTC");
        }

        /// <summary>
        /// Récuperation de toutes les données de la base de données. Si il n'existe pas de bases de données, elle sera créée. 
        /// </summary>
        /// <param name="dbFolder">Le dossier de création de la base de données</param>
        /// <returns>Une liste d'animes</returns>
        public static AnimeModel getInstance(string dbFolder)
        {
            if (dbFolder == "")
            {
                AnimeModel a = new AnimeModel();
                a.Add(a.FAKE_ANIME);
                return a;
            }
            string dbPath = Path.Combine(dbFolder, DB_FILE);
            if (!File.Exists(dbPath))
            {
                createDB(dbPath);
                // TODO : Verifier si on peut ecrire dans le dossier courant, si oui, db file = anime.sqlite sinon le appdata
                //        Faire la verification pour savoir si les fhichiers sont les memes
                //        Copier dans appdata si n'existe pas. Utiliser le local en priorite
            }
            return getAll(dbPath);            
        }

        /// <summary>
        /// Renvoie la liste d'animes de la base de données
        /// </summary>
        /// <param name="dbFile">le fichier de base de données</param>
        /// <returns>Une instance de AnimeModel avec tous les animes de la base de données</returns>
        private static AnimeModel getAll(string dbFile)
        {
            try
            {
                
                SQLiteConnection cnx;
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                AnimeModel all = new AnimeModel();
                all.file = dbFile;
                cnx = new SQLiteConnection("Data Source=" + dbFile + "; Version=3");
                string sqlStmnt = "SELECT a.name, a.season, a.studio, r.fansubs, a.year, r.episodes, r.language, r.sub, a.synopsis, a.type, r.path FROM anime AS a JOIN release AS r ON a.name = r.name AND a.season = r.season";
                cnx.Open();
                cmd = new SQLiteCommand(sqlStmnt, cnx);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                    all.Add(new Anime(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetInt32(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9), reader.GetString(10)));
                reader.Close();
                cnx.Close();
                if (all.Count == 0)
                    all.Add((new AnimeModel()).FAKE_ANIME);
                //MessageBox.Show(all.Count.ToString());
                return all;
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
                return null;
            }
        }

        /// <summary>
        /// Crée une base de donnée vide pour la gestion des animes
        /// </summary>
        /// <param name="dbFolder">Le dossier de la base de donnée</param>
        private static void createDB(string dbFile)
        {
            try
            {
                //string dbFile = Path.Combine(dbFolder, DB_FILE); // Le chemin vers le fichier de la base de données
                File.Delete(dbFile); // Supprime le fichier de base de données existant (Sécurité)
                SQLiteConnection.CreateFile(dbFile); // Création du fichier de base de donées
                SQLiteConnection cnx; // Permet d'effectuer les transactions avec la base de données
                SQLiteCommand cmd; // Transaction de la base de données
                string sqlStmnt; // Requete SQL de la transaction
                cnx = new SQLiteConnection("Data Source=" + dbFile + "; Version=3"); // Insataciation de la connesion à la base de données
                // Création de la table des animes
                sqlStmnt = "CREATE TABLE anime (name VARCHAR(150), season VARCHAR (40), studio VARCHAR(70), year INTEGER, type VARCHAR(200), synopsis VARCHAR(1000), PRIMARY KEY(name, season))";
                cnx.Open(); // Ouverture de la connection à la base de données
                cmd = new SQLiteCommand(sqlStmnt, cnx);
                cmd.ExecuteNonQuery(); // Execution de la requête
                // Création de la table des sorties des épisodes 
                sqlStmnt = "CREATE TABLE release (name VARCHAR(150), season VARCHAR(40), language VARCHAR(40), sub VARCHAR(40), fansubs VARCHAR(200), episodes INTEGER, path VARCHAR(500), PRIMARY KEY(name, season, language, sub), FOREIGN KEY(name) REFERENCES anime(name), FOREIGN KEY(season) REFERENCES anime(season))";
                cmd = new SQLiteCommand(sqlStmnt, cnx);
                cmd.ExecuteNonQuery(); // Execution de la requete de création de table
                cnx.Close();
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
            }
        }

        /// <summary>
        /// Insere un anime dans la base de données et dans la liste d'animes courante
        /// </summary>
        /// <param name="a">L'anime à inserer</param>
        public void insert(Anime a)
        {
            try
            {
                SQLiteConnection cnx = new SQLiteConnection("Data Source=" + file + "; Version=3"); // Instance de connexion à la base de données
                // Requete d'insertion de l'anime dans la base de données
                // L'insertion se fait forcement danas l'ordre suivant pour respecter la contrainte FOREIGN KEY
                string sqlStmnt = "INSERT INTO anime VALUES ('" + Helper.AdaptStringToSql(a.Name) + "', '" 
                    + Helper.AdaptStringToSql(a.Season) + "', '" 
                    + Helper.AdaptStringToSql(a.Studio) + "', " 
                    + a.Year + ", '" 
                    + Helper.AdaptStringToSql(a.Type) + "', '" 
                    + Helper.AdaptStringToSql(a.Synopsis) + "')";
                cnx.Open();
                (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
                sqlStmnt = "INSERT INTO release VALUES ('" + Helper.AdaptStringToSql(a.Name) + "', '" 
                    + Helper.AdaptStringToSql(a.Season) + "', '" 
                    + Helper.AdaptStringToSql(a.Language) + "', '" 
                    + Helper.AdaptStringToSql(a.Sub) + "', '" 
                    + Helper.AdaptStringToSql(a.Fansub) + "', " 
                    + a.NumberOfEpisode + ", '" 
                    + Helper.AdaptStringToSql(a.Directory) + "')";

                (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
                cnx.Close();
                this.Add(a);
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
            }
        }

        public List<string[]> AsStringArraysList()
        {
            List<string[]> l = new List<string[]>();
            foreach (Anime a in this)
            {
                l.Add(a.AsArray());
            }
            return l;
        }

        public AnimeModel get(Anime a)
        {
            AnimeModel filtered = new AnimeModel();
            foreach (Anime b in this)
                if (b.Name.Contains(a.Name) && a.Name.Length != 0 ||
                    b.Type.Contains(a.Type) && a.Type.Length != 0 ||
                    b.Language.Contains(a.Language) && a.Language.Length != 0 ||
                    b.Sub.Contains(a.Sub) && a.Sub.Length != 0 ||
                    a.Name.Length == 0 && a.Type.Length == 0 && a.Language.Length == 0 && a.Sub.Length == 0)
                {
                    filtered.Add(b);
                    //MessageBox.Show(b.toString());
                }
            if (filtered.Count == 0)
                filtered.Add(FAKE_ANIME);
            //MessageBox.Show(filtered.Count.ToString());
            return filtered;
        }


        public Anime getByPrimary(string name, string season, string lang, string sub)
        {
            Anime a = new Anime(name, season, "", "", 0, 0, lang, sub, "", "", "");
            AnimeModel filtered = new AnimeModel();
            foreach (Anime b in this)
                if (b.Name.Contains(a.Name) &&
                    b.Type.Contains(a.Type) &&
                    b.Language.Contains(a.Language) &&
                    b.Sub.Contains(a.Sub))
                    return b;
            return FAKE_ANIME;
        }

        public void update(Anime previous, Anime next)
        {
            SQLiteConnection cnx = new SQLiteConnection("Data Source=" + file + "; Version=3");
            cnx.Open();
            try
            {
                string sqlStmnt;   
                //MessageBox.Show(
                sqlStmnt = "UPDATE anime SET studio='" + Helper.AdaptStringToSql(next.Studio) + "', year=" + next.Year + ", type='" + Helper.AdaptStringToSql(next.Type) + "', synopsis='" + Helper.AdaptStringToSql(next.Synopsis) + "' WHERE name='" + Helper.AdaptStringToSql(next.Name) + "' AND season='" + Helper.AdaptStringToSql(next.Season) + "'";
                //);
                (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
                //MessageBox.Show(
                sqlStmnt = "UPDATE release SET fansubs='" + Helper.AdaptStringToSql(next.Fansub) + "' WHERE name='" + Helper.AdaptStringToSql(next.Name) + "' AND season='" + Helper.AdaptStringToSql(next.Season) + "' AND language='" + Helper.AdaptStringToSql(next.Language) + "' AND sub='" + Helper.AdaptStringToSql(next.Sub) + "'" ;
                //);
                (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
                cnx.Close();
                for (int i = 0; i < this.Count; i++)
                    if (this[i].Equals(previous))
                        this[i] = next;
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
            }
            cnx.Close();
        }

        public void AddEpisode(Anime a)
        {
            SQLiteConnection cnx = new SQLiteConnection("Data Source=" + file + "; Version=3");
            cnx.Open();
            try
            {
                string sqlStmnt = "UPDATE release SET episodes=" + ( ++a.NumberOfEpisode ) + " WHERE name='" + Helper.AdaptStringToSql(a.Name) + "' AND season='" + Helper.AdaptStringToSql(a.Season) + "' AND language='" + Helper.AdaptStringToSql(a.Language) + "' AND sub='" + Helper.AdaptStringToSql(a.Sub) + "'";
                (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
                for (int i = 0; i < this.Count; i++)
                    if (this[i].Equals(a))
                        this[i] = a;
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
            }
            cnx.Close();
        }

        public void delete(Anime a)
        {
            SQLiteConnection cnx = new SQLiteConnection("Data Source=" + file + "; Version=3");
            string sqlStmnt = "Delete from release where name='" + a.Name + "' AND season='" + a.Season + "' AND language='" + a.Language + "' AND sub='" + a.Sub + "'";
            cnx.Open();
            (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
            sqlStmnt = "Delete from anime where name='" + a.Name + "' AND season='" + a.Season + "'";
            (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
            cnx.Close();
            for (int i = 0; i < this.Count; i++)
                if (this[i].Equals(a))
                {
                    this.RemoveAt(i);
                    break;
                }
        }

        public void removeEpisode(Anime a)
        {
            SQLiteConnection cnx = new SQLiteConnection("Data Source=" + file + "; Version=3");
            cnx.Open();
            try
            {
                string sqlStmnt = "UPDATE release SET episodes=" + (--a.NumberOfEpisode) + " WHERE name='" + Helper.AdaptStringToSql(a.Name) + "' AND season='" + Helper.AdaptStringToSql(a.Season) + "' AND language='" + Helper.AdaptStringToSql(a.Language) + "' AND sub='" + Helper.AdaptStringToSql(a.Sub) + "'";
                (new SQLiteCommand(sqlStmnt, cnx)).ExecuteNonQuery();
                for (int i = 0; i < this.Count; i++)
                    if (this[i].Equals(a))
                        this[i] = a;
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
            }
            cnx.Close();
        }
    }
}
