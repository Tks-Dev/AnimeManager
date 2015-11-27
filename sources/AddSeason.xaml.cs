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
    /// Interaction logic for AddSeason.xaml
    /// </summary>
    public partial class AddSeason : Window
    {
        MainWindow main;

        public AddSeason(MainWindow main)
        {
            this.main = main;
            InitializeComponent();
            setBackground();
            loadFields();
        }

        private void loadFields()
        {
            tbox_name.Text = main.animeShown.Name;
            tbox_fansubs.Text = main.animeShown.Fansub;
            tbox_studio.Text = main.animeShown.Studio;
            cbox_language.Text = main.animeShown.Language;
            cbox_sub.Text = main.animeShown.Sub;
            tbox_type.Text = main.animeShown.Type;

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
            main.IsEnabled = true;
            main.loadDgrid();
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

        private void btn_ajouter_MouseEnter(object sender, MouseEventArgs e)
        {
            btn_ajouter.Fade(1, 0.1, 0.2);
        }

        private void btn_ajouter_MouseLeave(object sender, MouseEventArgs e)
        {
            btn_ajouter.Fade(0.3, 0.5, 0.2);
        }

        private void tbox_synopsis_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbl_count.Content = (1000 - tbox_synopsis.Text.Length).ToString();
        }

        private void btn_ajouter_Click(object sender, RoutedEventArgs e)
        {
            if (tbox_name.Text.Trim() == "" || tbox_season.Text.Trim() == "" || cbox_language.Text.Trim() == "" || cbox_sub.Text.Trim() == "")
            {
                MessageBox.Show("Bakka ! Tu as oublié le plus important !", "BAKKA !!", MessageBoxButton.OK, MessageBoxImage.Error);
                tbox_name.BorderBrush = tbox_name.Text.Trim() == "" ? Brushes.Red : Brushes.Green;
                tbox_season.BorderBrush = tbox_season.Text.Trim() == "" ? Brushes.Red : Brushes.Green;
                cbox_language.BorderBrush = cbox_language.Text.Trim() == "" ? Brushes.Red : Brushes.Green;
                cbox_sub.BorderBrush = cbox_sub.Text.Trim() == "" ? Brushes.Red : Brushes.Green;
            }
            else
            {
                if (!main.animes.getByPrimary(tbox_name.Text, tbox_season.Text, cbox_language.Text, cbox_sub.Text).Equals(new Anime("NO ANIME", "Saison 0", "Aucun", "Aucun", 42, 42, "Klingon", "Elfique", "C'est l'histoire d'un BD vide", "NONE", "DTC")))
                {
                    if (MessageBox.Show("Chottomatte ! Cet anime existe déjà. Veux-tu changer les donnée ? (Si tu veux pas, je quitte)", "Chottomatte !", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
                        return;
                    else
                        Close();
                }
                main.animes.insert(new Anime(tbox_name.Text, tbox_season.Text, tbox_studio.Text, tbox_fansubs.Text, Convert.ToInt32(tbox_year.Text), 0, cbox_language.Text, cbox_sub.Text, tbox_synopsis.Text, tbox_type.Text, "Anime\\" + tbox_name + " - " + tbox_season));
                this.Close();
            }
        }
    }
}
