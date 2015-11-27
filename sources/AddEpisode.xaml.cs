using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using License_Manager;
using System.IO;

namespace Anime_Manager
{
    /// <summary>
    /// Interaction logic for AddEpisode.xaml
    /// </summary>
    public partial class AddEpisode : Window
    {
        private MainWindow main;
        private Anime selected;
        string path;

        public AddEpisode(MainWindow main)
        {
            this.main = main;
            path = main.pathStart + "/" + main.animeShown.Directory;
            selected = main.animeShown;
            InitializeComponent();
            setBackground();
        }

        private void fadeOne(object sender, MouseEventArgs e)
        {
            rct.Fade(1, 0.1, 0.2);
            lbl.Fade(1, 0.1, 0.2);
            tbox_num.Fade(1, 0.1, 0.2);
        }

        private void fadeTwo(object sender, MouseEventArgs e)
        {
            rct.Fade(0.5, 0.1, 0.2);
            lbl.Fade(0.5, 0.1, 0.2);
            tbox_num.Fade(0.5, 0.1, 0.2);
        }

        private void setBackground()
        {
            string[] backgrounds = Directory.GetFiles("AddBacks");
            //Xceed.Wpf.Toolkit.MessageBox.Show(backgrounds.Length.ToString());
            Random r = new Random(Helper.dateTimeToMillis(DateTime.Now));
            int value = r.Next(backgrounds.Length);
            BitmapImage bimg = new BitmapImage();
            bimg.BeginInit();
            bimg.UriSource = new Uri(backgrounds[value], UriKind.Relative);
            bimg.CacheOption = BitmapCacheOption.OnLoad;
            bimg.EndInit();

            try
            {
                img_background.Source = bimg;
                img_background.Refresh();
                //MessageBox.Show(backgrounds[value]);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n============================================\n" + e.StackTrace + "\n============================================\nPath :" + backgrounds[value], "Error - Loading Image failed");

            }
        }

        private void tbox_path_MouseEnter(object sender, MouseEventArgs e)
        {
            tbox_path.Fade(1, 0.1, 0.2);
        }

        private void tbox_path_MouseLeave(object sender, MouseEventArgs e)
        {
            tbox_path.Fade(0.5, 0.1, 0.2);
        }

        private void btn_search_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_search.Fade(1, 0.1, 0.2);
        }

        private void btn_search_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_search.Fade(0.5, 0.1, 0.2);
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog f = new System.Windows.Forms.OpenFileDialog();
            f.RestoreDirectory = true;
            f.Multiselect = false;
            f.Title = "Selectionnez un fichier";
            f.ShowDialog();
            tbox_path.Text = f.FileName;
        }

        private void btn_ajouter_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_ajouter.Fade(1, 0.1, 0.2);
        }

        private void btn_ajouter_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_ajouter.Fade(0.5, 0.1, 0.2);
        }

        private void btn_ajouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                //File.Copy(tbox_path.Text, path + "\\" + selected.Name + " - " + selected.Season + " - " + tbox_num.Text + "." + tbox_path.Text.Split('.')[tbox_path.Text.Split('.').Length - 1]);
                //File.Delete(tbox_path.Text);
                (new CustomFileCopier(tbox_path.Text, path + "\\" + selected.Name + " - " + selected.Season + " - " + tbox_num.Text + "." + tbox_path.Text.Split('.')[tbox_path.Text.Split('.').Length - 1])).Copy();
                main.animes.AddEpisode(selected);
                MessageBox.Show("L'ajout a bien été effectué !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                Helper.ShowError(ex);
            }
            this.Close();
        }

        private void btn_changeBack_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_changeBack.Fade(1, 0.1, 0.2);
        }

        private void btn_changeBack_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_changeBack.Fade(0.1, 0.1, 0.2);
        }

        private void btn_changeBack_Click(object sender, RoutedEventArgs e)
        {
            setBackground();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.loadDgrid();
        }
    }
}
