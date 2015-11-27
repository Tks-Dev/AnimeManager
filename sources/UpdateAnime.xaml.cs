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
using System.Windows.Shapes;
using System.IO;
using License_Manager;

namespace Anime_Manager
{
    /// <summary>
    /// Interaction logic for UpdateAnime.xaml
    /// </summary>
    public partial class UpdateAnime : Window
    {
        private MainWindow main;
        private Anime previous;

        public UpdateAnime(MainWindow main)
        {
            this.main = main;
            previous = main.animeShown;
            InitializeComponent();
            setBackground();
            loadFields();
        }

        private void loadFields()
        {
            tbox_name.Text = previous.Name;
            tbox_season.Text = previous.Season;
            tbox_studio.Text = previous.Studio;
            tbox_fansubs.Text = previous.Fansub;
            tbox_type.Text = previous.Type;
            tbox_year.Text = previous.Year.ToString();
            tbox_synopsis.Text = previous.Synopsis;
            cbox_language.Text = previous.Language;
            cbox_sub.Text = previous.Sub;
        }

        private void tbox_synopsis_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbl_count.Content = (1000 - tbox_synopsis.Text.Length).ToString();
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            main.loadDgrid();
            main.loadCboxes();
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

        private void GroupBox_MouseEnter(object sender, MouseEventArgs e)
        {
            rect.Fade(0.8, 0.1, 0.2);
        }

        private void GroupBox_MouseLeave(object sender, MouseEventArgs e)
        {
            rect.Fade(0.3, 0.8, 0.2);
        }

        private void GroupBox_GotFocus(object sender, RoutedEventArgs e)
        {
            rect.Fade(0.2, 0.1, 0.2);
        }

        private void btn_update_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_update.Fade(1, 0.1, 0.2);
        }

        private void btn_update_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_update.Fade(0.3, 0.8, 0.2);
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            string s = tbox_year.Text;
            int year = s.Trim() == "" ? 42 : Convert.ToInt32(s);
            Anime next = new Anime(tbox_name.Text, tbox_season.Text, tbox_studio.Text, tbox_fansubs.Text, year, previous.NumberOfEpisode, cbox_language.Text, cbox_sub.Text, tbox_synopsis.Text, tbox_type.Text, "Anime/" + tbox_name + " - " + tbox_season);
            if (!next.StrictEquals(previous))
            {
                main.animes.update(previous, next);
                MessageBox.Show("Mise à jour effectuée !", "OK");
            }
            this.Close();
        }

        private void tbox_year_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbox_year.Text.Length > 0 && (tbox_year.Text[tbox_year.Text.Length - 1] > '9' || tbox_year.Text[tbox_year.Text.Length - 1] < '0'))
                tbox_year.Text = tbox_year.Text.Remove(tbox_year.Text.Length - 1);
            if (tbox_year.Text.Length > 4)
                tbox_year.Text = tbox_year.Text.Remove(tbox_year.Text.Length - 1);
        }


    }
}
