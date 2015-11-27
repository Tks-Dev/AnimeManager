using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using License_Manager;
using System.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Reflection;

namespace Anime_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public AnimeModel animes { get; set; }
        public Anime animeShown { get; private set; }
        public string pathStart { get; set; }
        public const string FILE_NAME = "info.animan";
        private string configFile = "";
        private string appDataErrorFolder;

        public MainWindow()
        {
            
            configFile = getConfigFile();
            /*
             * TODO :
             * Instancier les animes après la récupération du dossier de stockage
             */
            
            //while (!(bool)this.IsEnabled) ;
            
            InitializeComponent();
            if (configFile != "")
                initialisationP2();
            setBackground();
            
            //Process.Start("explorer.exe", "-p");
        }

        public void initialisationP2()
        {
            pathStart = getPathFromConfigFile();
            appDataErrorFolder = getErrorFolder();
            animes = AnimeModel.getInstance(pathStart);
            loadDgrid();
            loadCboxes();
        }

        private string getErrorFolder()
        {
            string folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnimeManager");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            folder = System.IO.Path.Combine(folder, "logs");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;

        }

        private string getPathFromConfigFile()
        {
            try
            {
                StreamReader configReader = new StreamReader(configFile);
                string animeFolder = configReader.ReadLine();
                configReader.Dispose();
                while( !(Directory.Exists(animeFolder) && Helper.checkFolderWriteEnable(animeFolder)) )
                {
                    MessageBox.Show("Impossible d'accéder au dossier de sauvegarde des animes. \nVeuillez le sélectionner ou en sélectionner un autre...", "Dossier inaccessible", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    animeFolder = folderFromBrowser("Selectionnez le dossier de stockage de vos animes");
                    updateConfigFile(animeFolder);
                }
                return animeFolder;
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
                string animeFolder = "";
                do
                {
                    MessageBox.Show("Impossible d'accéder au dossier de sauvegarde des animes. \nVeuillez le sélectionner ou en sélectionner un autre...", "Dossier inaccessible", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    animeFolder = folderFromBrowser("Selectionnez le dossier de stockage de vos animes");
                    updateConfigFile(animeFolder);
                }
                while (!(Directory.Exists(animeFolder) && Helper.checkFolderWriteEnable(animeFolder)));
                return animeFolder;
            }
        }

        private void updateConfigFile(string animeFolder)
        {
            string configFilePath = getConfigFile();
            try
            {
                StreamWriter infoAnimanager = new StreamWriter(configFilePath);
                infoAnimanager.Flush();
                infoAnimanager.Write(animeFolder);
                infoAnimanager.Dispose();
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
            }
        }

        public string getConfigFileName()
        {
            return FILE_NAME;
        }

        /// <summary>
        /// Récupère le chemin 
        /// </summary>
        /// <returns>La chaine de caractère correspondant au chemin absolu du ficher de configuration</returns>
        private string getConfigFile()
        {
            string path;
            try
            {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // chemin vers Appdata
                string animeManagerFolder = System.IO.Path.Combine(folder, "AnimeManager"); // Chemin vers le dossier de l'application
                bool exist; // existence du fichier de configuration dans le dossier d'installation
                bool inAppData; // existence du fichier de configuration dans le dossier Appdata
                exist = File.Exists(FILE_NAME); // True si le fichier existe dans le dossire d'installation
                inAppData = File.Exists(path = animeManagerFolder + "\\" + FILE_NAME); // True si le ficher existe dans le dossier AppData

                // Cas 1 : le fichier n'èxiste pas dans le dossier courant mais le fichier existe dans AppData
                if (!exist && inAppData)
                {
                    StreamReader sReader = new StreamReader(path);
                    // On vérifie que le chemin du dossier contenu dans le fichier existe
                    if (Directory.Exists(sReader.ReadLine()))
                    {
                        sReader.Dispose();
                        return path; // renvoie le chemin du fichier de configuration (appdata->AnimeManager)
                    }
                    sReader.Dispose();
                    StreamWriter sWriter = new StreamWriter(path); // Créé un flux d'écriture dans le fichier de configuartion dans Appdata
                    string storageFolder = folderFromBrowser("Selectionnez le dossier où les animes seront stockés");
                    sWriter.WriteLine(storageFolder); // On ecrit le chemin du dossier de stockage dans le fichier de configuration
                    sWriter.Dispose();
                    try
                    {
                        File.Copy(path, FILE_NAME); // on essaie de copier le fichier de configuration dans le dossier d'installation
                    }
                    catch (Exception err) { }
                    return path;
                }

                // Cas 2 : Le fichier de configuration exite dans le dossier d'installation mais n'existe pas dans AppData
                if (exist && !inAppData)
                {
                    try
                    {
                        StreamReader sReader = new StreamReader(FILE_NAME); // Flux de lecture du fichier de configuration dans le dossier d'installation 
                        string folderName = sReader.ReadLine(); // Récupere le dossier de sauvegarde des animes
                        sReader.Dispose(); // Fermeture du fulx de lecture
                        if (Directory.Exists(folderName))
                        {
                            if (!Directory.Exists(animeManagerFolder))
                                Directory.CreateDirectory(animeManagerFolder);
                            File.Copy(FILE_NAME, path); // On copie le fichier dans appdata
                            return path;
                        }

                        //Cas où le dossier de sauvegarde n'existe pas
                        MessageBox.Show("Le dossier de stockage indiqué dans le fichier de configuration est inexistant. \nVeuillez en selectionner un autre.", "Dossier de stockage inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        string storageFolder = folderFromBrowser("Selectionnez le dossier où les animes seront stockés");
                        if (!Directory.Exists(animeManagerFolder)) // On creer le dossier de l'application dans Appdata si il n'existe pas déjà
                            Directory.CreateDirectory(animeManagerFolder);
                        StreamWriter sWriter =  File.CreateText(path); // on creer le fichier de configuration
                        sWriter.Write(storageFolder);
                        sWriter.Dispose();
                        try
                        {
                            File.Copy(path, FILE_NAME);
                        }
                        catch (Exception error) { }

                        return path;
                    }
                    catch (Exception err)
                    {
                        Helper.ShowError(err);
                        return FILE_NAME;
                    }
                }

                if (!exist && !inAppData)
                {
                    if (!Directory.Exists(animeManagerFolder)) 
                        Directory.CreateDirectory(animeManagerFolder);
                    StreamWriter appDataFile = File.CreateText(path);
                    string storageFolder = folderFromBrowser("Selectionnez le dossier où les animes seront stockés");
                    appDataFile.WriteLine(storageFolder);
                    appDataFile.Dispose();
                    try
                    {
                        File.Copy(path, FILE_NAME);
                    }
                    catch (Exception err) { }
                    return path;
                }

                if (exist && inAppData)
                {
                    StreamReader local = new StreamReader(FILE_NAME);
                    StreamReader appdata = new StreamReader(path);
                    string localConfig = local.ReadLine();
                    string appDataConfig = appdata.ReadLine();
                    local.Dispose();
                    appdata.Dispose();
                    if (localConfig == appDataConfig)
                        return path;
                    (new ChooseWindow(this, localConfig, appDataConfig)).Show();
                    this.IsEnabled = false;
                    return "";
                }


                    // if local == appdata
                    //     return path;
                    // new ChooseWindow(this, local, appdata)
                    /*
                     * utiliser local
                     * utiliser appdata
                     * utiliser local et remplacer appdata
                     * supprimer local et utiliser appdata
                     */
                    /*
                    info2 = new StreamReader("info.animan");
                    string s = info2.ReadLine();
                    if (Directory.Exists(s))
                    {
                        info2.Dispose();
                        return s;
                    }
                    info2.Dispose();
                    File.Delete("info.animan");
                }
                info = File.CreateText("info.animan");
                
                /*info.WriteLine(f.SelectedPath);
                info.Dispose();
                return f.SelectedPath;*/

                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Helper.ShowError(e);
                return null;
            }
        }

        private string folderFromBrowser(string windowTitle)
        {
            System.Windows.Forms.FolderBrowserDialog f = new System.Windows.Forms.FolderBrowserDialog();
            f.Description = windowTitle ;
            f.ShowDialog();
            return f.SelectedPath;
        }

        private string fileFromBrowser(string windowTitle, string initialDirectory)
        {
            System.Windows.Forms.OpenFileDialog f = new System.Windows.Forms.OpenFileDialog();
            f.InitialDirectory = initialDirectory;
            f.Title = windowTitle;
            f.ShowDialog();
            return f.FileName;
        }

        public void loadCboxes()
        {
            AnimeController.fill(cbox_searchCategory, AnimeController.PROP_TYPE, animes, true);
            AnimeController.fill(cbox_searchLanguage, AnimeController.PROP_LANG, animes, true);
            AnimeController.fill(cbox_searchSub, AnimeController.PROP_SUB, animes, true);
        }

        public void loadDgrid()
        {
            string lang = cbox_searchLanguage.SelectedItem != null ? cbox_searchLanguage.SelectedItem.ToString() : "";
            string sub = cbox_searchSub.SelectedItem != null ? cbox_searchSub.SelectedItem.ToString() : "";
            string type = cbox_searchCategory.SelectedItem != null ? cbox_searchCategory.SelectedItem.ToString() : "";
            AnimeController.fill(dGrid_anime, animes.get(new Anime(tbox_searchName.Text, "", "", "", 0, 0, lang, sub , "", type, "")));
            dGrid_anime.SelectedItem = null;
            btn_addEp.IsEnabled = btn_addSeason.IsEnabled = btn_update.IsEnabled = false;
        }

        private void setBackground()
        {
            string[] backgrounds = Directory.GetFiles("Backgrounds");
            //Xceed.Wpf.Toolkit.MessageBox.Show(backgrounds.Length.ToString());
            Random r = new Random(Helper.dateTimeToMillis(DateTime.Now));
            int value = r.Next(backgrounds.Length);
            for (int i = 0; i < value; i++)
                r.Next(backgrounds.Length);
            BitmapImage bimg = new BitmapImage();
            bimg.BeginInit();
            bimg.UriSource = new Uri(backgrounds[r.Next(backgrounds.Length)], UriKind.Relative);
            bimg.CacheOption = BitmapCacheOption.OnLoad;
            bimg.EndInit();
            
            try
            {
                img_background.ChangeSource(bimg, 0.5, 0.5);
                //img_background.Source = bimg;
                //img_background.Refresh();
                //MessageBox.Show(backgrounds[value]);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n============================================\n" + e.StackTrace + "\n============================================\nPath :" + backgrounds[value], "Error - Loading Image failed");

            }
        }

        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            setBackground();
        }

        private void dGrid_anime_MouseEnter(object sender, MouseEventArgs e)
        {
            dGrid_anime.Fade(1, 0.1, 0.5);
            /*var animation = new DoubleAnimation
            {
                To = 1,
                BeginTime = TimeSpan.FromSeconds(0.1),
                Duration = TimeSpan.FromSeconds(0.5),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => dGrid_anime.Opacity = 1;

            dGrid_anime.BeginAnimation(UIElement.OpacityProperty, animation);*/
        }

        private void dGrid_anime_MouseLeave(object sender, MouseEventArgs e)
        {
            dGrid_anime.Fade(0.1, 0.1, 0.5);
            /*var animation = new DoubleAnimation
            {
                To = 0.1,
                BeginTime = TimeSpan.FromSeconds(0.1),
                Duration = TimeSpan.FromSeconds(0.5),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => dGrid_anime.Opacity = 0.1;

            dGrid_anime.BeginAnimation(UIElement.OpacityProperty, animation);*/
            //dGrid_anime.Opacity = 0.1;
        }

        private void filter_MouseEnter(object sender, MouseEventArgs e)
        {
            gbox_filter.Fade(1, 0.01, 0.5);
            rect_filter.Fade(1, 0.01, 0.5);
        }

        private void details_MouseEnter(object sender, MouseEventArgs e)
        {
            gbox_details.Fade(1, 0.01, 0.5);
            rect_detail.Fade(1, 0.01, 0.5);
        }

        private void details_MouseLeave(object sender, MouseEventArgs e)
        {
            gbox_details.Fade(0.3, 2, 0.5);
            rect_detail.Fade(0.3, 2, 0.5);
        }

        private void filter_MouseLeave(object sender, MouseEventArgs e)
        {
            gbox_filter.Fade(0.3, 2, 0.5);
            rect_filter.Fade(0.3, 2, 0.5);
        }

        private void btn_load_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_load.Fade(1, 0.1, 0.3);
        }

        private void btn_load_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_load.Fade(0.05, 0.1, 0.3);
        }

        private void reloadGrid(object sender, TextChangedEventArgs e)
        {
            loadDgrid();
        }

        private void reloadGrid(object sender, SelectionChangedEventArgs e)
        {
            loadDgrid();
        }

        private void showDetails(object sender, SelectionChangedEventArgs e)
        {
            if (dGrid_anime.SelectedItem == null)
            {
                tbloc_synopsis.Text = tbox_episodes.Text = tbox_fansub.Text = tbox_language.Text = tbox_name.Text = tbox_season.Text = tbox_studio.Text = tbox_sub.Text = tbox_type.Text = tbox_year.Text = "";
                btn_delete.IsEnabled = btn_deleteEp.IsEnabled = btn_openFolder.IsEnabled = btn_addEp.IsEnabled = btn_addSeason.IsEnabled = btn_update.IsEnabled = false;
                return;
            }
            animeShown = animes.getByPrimary(Helper.getDataGridValueAt(dGrid_anime, "Nom"), Helper.getDataGridValueAt(dGrid_anime, "Saison"), Helper.getDataGridValueAt(dGrid_anime, "Langue"), Helper.getDataGridValueAt(dGrid_anime, "Sous_Titres"));
            //if (animeShown == null)
            //    MessageBox.Show("NO ANIME SELECTED")
            tbox_name.Text = animeShown.Name;
            tbox_season.Text = animeShown.Season;
            tbox_fansub.Text = animeShown.Fansub;
            tbox_language.Text = animeShown.Language;
            tbox_type.Text = animeShown.Type;
            tbox_sub.Text = animeShown.Sub;
            tbox_studio.Text = animeShown.Studio;
            tbox_year.Text = animeShown.Year.ToString();
            tbox_episodes.Text = animeShown.NumberOfEpisode.ToString();
            tbloc_synopsis.Text = animeShown.Synopsis;
            btn_delete.IsEnabled =  btn_addEp.IsEnabled = btn_addSeason.IsEnabled = btn_update.IsEnabled = !(animeShown.Equals(animes.FAKE_ANIME));
            btn_openFolder.IsEnabled = btn_deleteEp.IsEnabled = !(animeShown.NumberOfEpisode == 0 || animeShown.Equals(animes.FAKE_ANIME));
        }

        private void btn_add_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_add.Fade(1, 0.1, 0.2);
        }

        private void btn_add_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_add.Fade(0.3, 1, 0.2);
        }

        private void btn_addSeason_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_addSeason.Fade(1, 0.1, 0.2);
        }

        private void btn_addSeason_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_addSeason.Fade(0.3, 1, 0.2);
        }

        private void btn_update_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_update.Fade(1, 0.1, 0.2);
        }

        private void btn_update_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_update.Fade(0.3, 1, 0.2);
        }

        private void btn_addEp_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_addEp.Fade(1, 0.1, 0.2);
        }

        private void btn_addEp_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_addEp.Fade(0.3, 1, 0.2);
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            (new AddAnime(this)).Show();
        }

        private void btn_addSeason_Click(object sender, RoutedEventArgs e)
        {
            (new AddSeason(this)).Show();
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            (new UpdateAnime(this)).Show();
        }

        private void btn_addEp_Click(object sender, RoutedEventArgs e)
        {
            (new AddEpisode(this)).Show();
        }

        private void btn_openFolder_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_openFolder.Fade(1, 0.1, 0.2);
        }

        private void btn_openFolder_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_openFolder.Fade(0.3, 0.1, 0.2);
        }

        private void btn_openFolder_Click(object sender, RoutedEventArgs e)
        {
            
            string path = pathStart + "\\" + animeShown.Directory.Split('/')[0] + "\\";
            path.Replace('/', '\\');
            path.Replace('/', '\\');
            path = "\"" + path + "\\\"";
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                Helper.ShowError(ex);
                MessageBox.Show(path);
                //ex.WriteErrorLog(appDataErrorFolder);
                //MessageBox.Show("Path : " + path);
            }
        }

        private void btn_delete_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_delete.Fade(1, 0.1, 0.2);
        }

        private void btn_delete_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_delete.Fade(0.3, 0.1, 0.2);
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Voulez-vous vraiment supprimer " + animeShown.Name + " - " + animeShown.Season + " et tous les épisodes ? (Attention aucun retour possible)",
                    "Supprimer tout les animes ?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (string file in Directory.EnumerateFiles(pathStart + "\\" + animeShown.Directory))
                        File.Delete(file);
                    Directory.Delete(pathStart + "\\" + animeShown.Directory);
                    animes.delete(animeShown);
                    MessageBox.Show("Suppressions éffectuées !!", "OK !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Helper.ShowError(ex);
            }
            loadDgrid();
        }

        private void btn_deleteEp_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_deleteEp.Fade(1, 0.1, 0.2);
        }

        private void btn_deleteEp_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_deleteEp.Fade(0.3, 0.1, 0.2);
        }

        private void btn_deleteEp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog f = new System.Windows.Forms.OpenFileDialog();            
            f.Title = "Sélectionnez un fichier";
            f.RestoreDirectory = true;
            f.Multiselect = false;
            string path = pathStart + "\\" + animeShown.Directory.Split('/')[0]; //+ "\\" + animeShown.Directory.Split('/')[1];
            path.Replace('/', '\\');
            path.Replace('/', '\\');

            f.InitialDirectory = path;
            MessageBox.Show(path);
            
            f.ShowDialog();
            if (f.FileName == "" || MessageBox.Show("Voulez-vraiment supprimer cet épisode ? (Définitif)", "Supprimer l'épisode ?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;
            try
            {
                File.Delete(f.FileName);
                animes.removeEpisode(animeShown);
                MessageBox.Show("Suppression éffectuée !!", "OK !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Helper.ShowError(ex);
                MessageBox.Show("Erreur : fichier non supprimé", "## ERROR ##", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            loadDgrid();
        }

        private void btn_chooseBg_Click(object sender, RoutedEventArgs e)
        {
            string nextBackgroundPath = fileFromBrowser("Choisissez un fond", System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)+"\\Backgrounds" );
            //MessageBox.Show(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)); //+ "\\Backgrounds");
            if (nextBackgroundPath == "")
                return;
            BitmapImage bimg = new BitmapImage();
            bimg.BeginInit();
            bimg.UriSource = new Uri(nextBackgroundPath, UriKind.Relative);
            bimg.CacheOption = BitmapCacheOption.OnLoad;
            bimg.EndInit();
            img_background.ChangeSource(bimg, 0.1, 0.5);
        }

        private void btn_chooseBg_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_chooseBg.Fade(1, 0.1, 0.2);
        }

        private void btn_chooseBg_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_chooseBg.Fade(0.05, 0.1, 0.2);
        }
    }
}
