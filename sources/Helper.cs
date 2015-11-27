using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace License_Manager
{
    class Helper
    {
        /// <summary>
        /// Convertit une date sous en un entier sous la forme YYYYMMDD
        /// </summary>
        /// <param name="toConvert">La date a convertir</param>
        /// <returns>L'entier correspondant a la date</returns>
        public static int toInt(DateTime toConvert)
        {
            return toConvert.Year * 10000 + toConvert.Month * 10 + toConvert.Day;
        }

        /// <summary>
        /// Convertit un entier de la forme YYYYMMDD en date
        /// </summary>
        /// <param name="toConvert">L'entier a convertir</param>
        /// <returns>La date correspondante a l'entier</returns>
        public static DateTime toDate(int toConvert)
        {
            int year = toConvert / 10000;
            int month = toConvert % 10000 / 100;
            int day = toConvert % 100;
            return new DateTime(year, month, day);
        }

        /// <summary>
        /// Convertit une date sous la forme J/M/Y en date (aucune vérification sur la validité ne sera faite)
        /// </summary>
        /// <param name="date">La chaine de caractère contenant la date</param>
        /// <returns>La date correspondant à la chaine de caractère</returns>
        public static DateTime stringFrToDate(string date)
        {
            if (date.Trim() == "")
                return DateTime.MaxValue;
            return new DateTime(
                        Convert.ToInt32(date.Split('/')[2]),
                        Convert.ToInt32(date.Split('/')[1]),
                        Convert.ToInt32(date.Split('/')[0])
                            );
        }

        /// <summary>
        /// Convertit un DateTime en une chaine de caractere utilisable en SQLite pour inserer une date
        /// </summary>
        /// <param name="toAdapt">La date a convertir</param>
        /// <returns></returns>
        public static string toSQLiteModel(DateTime toAdapt)
        {
            return toAdapt.Year + "-" + (toAdapt.Month > 9 ? "" + toAdapt.Month : "0" + toAdapt.Month) + "-" + (toAdapt.Day > 9 ? "" + toAdapt.Day : "0" + toAdapt.Day);
        }

        /// <summary>
        /// Permet de récuperer la valeur d'une cellule d'un DataGrid dans la ligne selectionnée.
        /// ATTENTION : La valeur de la colonne est absolue : si le DataGrid est réorganisé par l'utilisateur, il faut changer l'index.
        /// </summary>
        /// <param name="dGrid">Le DataGrid où l'on fait la récupération</param>
        /// <param name="columnIndex">L'index de la valeur dans la ligne selectionnée</param>
        /// <returns>La valeur contenue dans la ligne selectionnée ou une chaine vide si aucune ligne n'est selectionnée</returns>
        public static string getDataGridValueAt(DataGrid dGrid, int columnIndex)
        {
            if (dGrid.SelectedItem == null) // si rien n'est selectionner, on renvoit une chaine vide
                return "";
            string str = dGrid.SelectedItem.ToString(); // Recupere la ligne selectionnee
            str = str.Replace("}", "").Trim().Replace("{", "").Trim(); // Enleve les caracteres superflus
            if (columnIndex < 0 || columnIndex >= str.Split(',').Length) // Cas ou l'index donne n'est pas dans l'ensemble des idex utilisables
                return "";
            str = str.Split(',')[columnIndex].Trim();
            str = str.Split('=')[1].Trim();
            return str;
        }

        /// <summary>
        /// Permet de récupérer la valeur d'une cellule d'un DataGrid dans la ligne sélectionnée.
        /// </summary>
        /// <param name="dGrid">Le DataGrid où l'on fait la récupération.</param>
        /// <param name="columnName">Le nom de la colonne de la valeur recherchée. ATTENTION, le paramètre doit être le même que celui AFFICHE.</param>
        /// <returns>La valeur contenue dans la ligne selectionnée ou une chaîne de caractères vide si aucune ligne n'est selectionnée ou si la colonne n'existe pas.</returns>
        public static string getDataGridValueAt(DataGrid dGrid, string columnName)
        {
            if (dGrid.SelectedItem == null)
                return "";
            for (int i = 0; i < columnName.Length; i++)
                if (columnName.ElementAt(i) == '_')
                {
                    columnName = columnName.Insert(i, "_"); // Formalise le nom de la colonne (il faut 2 '_' pour qu'il n'y en ai qu'un d'affiche)
                    i++;
                }
            string str = dGrid.SelectedItem.ToString(); // Recupere la ligne selectionnee
            str = str.Replace("}", "").Trim().Replace("{", "").Trim(); // Enleve les caracteres superflus
            for (int i = 0; i < str.Split(',').Length; i++)
                if (str.Split(',')[i].Trim().Split('=')[0].Trim() == columnName) // Verifie la correspondance entre la colonne demandee et celles presentes dans le DataGrid
                    return str.Split(',')[i].Trim().Split('=')[1].Trim();
            return str;
        }

        /// <summary>
        /// Recupere la date en millisecondes (overflow permanent)
        /// </summary>
        /// <param name="dateTime">La date a convertir</param>
        /// <returns>La date en milliseconde</returns>
        public static int dateTimeToMillis(DateTime dateTime)
        {
            return (int)((((((((dateTime.Year) * 12 + dateTime.Month - 1) * 30.41 + dateTime.Day) * 24 + dateTime.Hour) * 60 + dateTime.Minute) * 60 + dateTime.Second)*1000 + dateTime.Millisecond) % (double)Int32.MaxValue);
        }

        /// <summary>
        /// Affiche une boite de dialogue contenant les details de l'erreur
        /// </summary>
        /// <param name="e">L'erreur a afficher</param>
        public static void ShowError(Exception e)
        {
            MessageBox.Show(e.Message + "\n\t===========\n" + e.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Permet d'adapter une chaine de caratere pour eviter les injections SQL lors de l'insertion
        /// </summary>
        /// <param name="toAdapt">La chaine de caracteres a adapter</param>
        /// <returns>La chaine modifiee prete a etre inseree</returns>
        public static string AdaptStringToSql(string toAdapt)
        {
            for (int i = 0; i < toAdapt.Length; i++)
                if (toAdapt[i] == '\'')
                {
                    toAdapt = toAdapt.Insert(i, "'");
                    i++;
                }
            return toAdapt;
        }

        /// <summary>
        /// Predicat de creation de fichiers dans le dossier local
        /// </summary>
        /// <returns>True si il est possible de creer un fichier, false sinon</returns>
        public static bool checkLocalWriteEnable()
        {
            return checkFolderWriteEnable(".");
        }

        /// <summary>
        /// Predicat de creation de fichiers dans le dossier séléctionné
        /// </summary>
        /// <returns>True si il est possible de creer un fichier, false sinon</returns>
        public static bool checkFolderWriteEnable(string folderPath)
        {
            string path = Path.Combine(folderPath, "try");
            //MessageBox.Show(path, "path chemin à tester");
            try
            {
                File.Create(path); // Essai la création d'un fichier "try"
                //MessageBox.Show("Create ok");
                return File.Exists(path);    
            }
            catch (Exception e) { return false; }
        }
    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate() { };

        /// <summary>
        /// Permet de rafaichir un element graphique
        /// </summary>
        /// <param name="uiElement">L'element graphique a rafraichir</param>
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        /// <summary>
        /// Effectue un fondu en utilisant une DoubleAnimation
        /// </summary>
        /// <param name="uiElement">L'element this qui subira la transformation</param>
        /// <param name="opacityValue">La valeur d'opacite a atteindre</param>
        /// <param name="startTime">Le delai avant lq commencement de l'animation</param>
        /// <param name="duration">La lapse de temps durant lequel l'animation se deroule</param>
        public static void Fade(this UIElement uiElement, double opacityValue, double startTime, double duration)
        {
            var animation = new DoubleAnimation
            {
                To = opacityValue,
                BeginTime = TimeSpan.FromSeconds(startTime),
                Duration = TimeSpan.FromSeconds(duration),
                FillBehavior = FillBehavior.Stop
            };
            animation.Completed += (s, a) => uiElement.Opacity = opacityValue;
            uiElement.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        /// <summary>
        /// Change cette image avec une transition en fondu
        /// </summary>
        /// <param name="image">L'image à modifier</param>
        /// <param name="source">La source de la prochaine image</param>
        /// <param name="beginTime">Le délai en seconde après lequel la transition commence</param>
        /// <param name="fadeTime">Le durée de la transition</param>
        public static void ChangeSource(this Image image, ImageSource source, double beginTime, double fadeTime)
        {
            var fadeInAnimation = new DoubleAnimation 
            {
                To = 1, 
                BeginTime = TimeSpan.FromSeconds(fadeTime), 
                Duration = TimeSpan.FromSeconds(beginTime) 
            };

            if (image.Source != null)
            {
                var fadeOutAnimation = new DoubleAnimation
                {
                    To = 0,
                    BeginTime = TimeSpan.FromSeconds(0.05),
                    Duration = TimeSpan.FromSeconds(fadeTime)
                };

                fadeOutAnimation.Completed += (o, e) =>
                {
                    image.Source = source;
                    image.BeginAnimation(Image.OpacityProperty, fadeInAnimation);
                };

                image.BeginAnimation(Image.OpacityProperty, fadeOutAnimation);
            }
            else
            {
                image.Opacity = 0d;
                image.Source = source;
                image.BeginAnimation(Image.OpacityProperty, fadeInAnimation);
            }
        }

        public static void WriteErrorLog(this Exception err, string logFileFolder)
        {
            try
            {
                string fileName = DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToLocalTime() + " - AnimeManager Error.bug";
                fileName = Path.Combine(logFileFolder,fileName);
                StreamWriter logFile = File.CreateText(fileName);
                MessageBox.Show("Une erreur est survenue. Veuillez transmettre ce fichier au développeur : \n" + fileName, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            { Helper.ShowError(e); }
        }
    }

    public class ComboBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            if (this == null)
                return "";
            return Text;
        }

        public ComboBoxItem(string Text, object Value)
        {
            this.Text = Text;
            this.Value = Value;
        }
    }

    public delegate void ProgressChangeDelegate(double Persentage, ref bool Cancel);
    public delegate void Completedelegate();

    class CustomFileCopier
    {
        public CustomFileCopier(string Source, string Dest)
        {
            this.SourceFilePath = Source;
            this.DestFilePath = Dest;

            OnProgressChanged += delegate { };
            OnComplete += delegate { };
        }

        public void Copy()
        {
            byte[] buffer = new byte[1024 * 1024]; // 1MB buffer
            bool cancelFlag = false;

            using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
            {
                long fileLength = source.Length;
                using (FileStream dest = new FileStream(DestFilePath, FileMode.CreateNew, FileAccess.Write))
                {
                    long totalBytes = 0;
                    int currentBlockSize = 0;

                    while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytes += currentBlockSize;
                        double persentage = (double)totalBytes * 100.0 / fileLength;

                        dest.Write(buffer, 0, currentBlockSize);

                        cancelFlag = false;
                        OnProgressChanged(persentage, ref cancelFlag);

                        if (cancelFlag == true)
                        {
                            // Delete dest file here
                            break;
                        }
                    }
                }
            }

            OnComplete();
        }

        public string SourceFilePath { get; set; }
        public string DestFilePath { get; set; }

        public event ProgressChangeDelegate OnProgressChanged;
        public event Completedelegate OnComplete;
    }
}
