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

namespace Anime_Manager
{
    /// <summary>
    /// Interaction logic for ChooseWindow.xaml
    /// </summary>
    public partial class ChooseWindow : Window
    {
        private MainWindow mainWindow;
        private string localConfig;
        private string appDataConfig;

        public ChooseWindow()
        {
            InitializeComponent();
        }

        public ChooseWindow(MainWindow mainWindow, string localConfig, string appDataConfig)
        {
            // TODO: Complete member initialization
            this.mainWindow = mainWindow;
            mainWindow.IsEnabled = false;
            this.localConfig = localConfig;
            this.appDataConfig = appDataConfig;
            InitializeComponent();
            tbox_appData.Text = appDataConfig;
            tbox_localConfig.Text = localConfig;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.IsEnabled = true;
            mainWindow.initialisationP2();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Êtes-vous sûr de votre choix ?", "C'est votre dernier mot ?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                return ;

            if ((bool)rb_useAppData.IsChecked)
            {
                mainWindow.pathStart = appDataConfig;
                Close();
            }

            if ((bool)rb_useLocal.IsChecked)
            {
                mainWindow.pathStart = localConfig;
                Close();
            }

            if ((bool)rb_suppLocal.IsChecked)
            {
                mainWindow.pathStart = appDataConfig;
                try
                {
                    File.Delete(mainWindow.getConfigFileName());
                    try
                    {                        
                        StreamWriter w = new StreamWriter(File.Create(mainWindow.getConfigFileName()));
                        w.WriteLine(appDataConfig);
                        w.Dispose();
                    }
                    catch (Exception ex) { }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Impossible de supprimer le fichier local", "Oops !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Close();
            }

            if ((bool)rb_suppAppData.IsChecked)
            {
                mainWindow.pathStart = localConfig;
                try
                {
                    string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string animeManagerFolder = System.IO.Path.Combine(folder, "AnimeManager");
                    File.Delete(animeManagerFolder + "/" + mainWindow.getConfigFileName());
                    try
                    {
                        StreamWriter w = new StreamWriter(File.Create(System.IO.Path.Combine(animeManagerFolder, mainWindow.getConfigFileName())));
                        w.WriteLine(localConfig);
                        w.Dispose();
                    }
                    catch (Exception ex) { }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Impossible de supprimer le fichier de session", "Oops !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Close();
            }
        }
    }
}
