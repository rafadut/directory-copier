using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DirectoryCopier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string DESTINATION_PATH = @"C:\Users\rafae\Desktop\";
        const byte DESTINATION_QUANTITY = 7;
        
        public MainWindow()
        {
            InitializeComponent();
            CreateCheckBoxes();
        }

        private void CreateCheckBoxes()
        {
            CheckBox objCheckBox;
            for (int i = 1; i <= DESTINATION_QUANTITY; i++)
            {
                objCheckBox = new CheckBox();
                objCheckBox.Content = "Teste" + i;
                objCheckBox.Margin = new Thickness(10, 0, 0, 10);
                spServers.Children.Add(objCheckBox);
            }
        }

        private void txtSource_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnPublish.IsEnabled = txtSource.Text != "";
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.SelectedPath = txtSource.Text;
                fbd.ShowDialog();
                txtSource.Text = fbd.SelectedPath;
            }
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            string sourcePath = txtSource.Text;
            DeleteWebConfig(sourcePath);
            string[] directories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            string[] files = Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories);
            List<string> DestinationFolders = GetSelectedDestinationFolders();
            try
            {
                foreach (string destinationFolder in DestinationFolders)
                {
                    CopyFilesToServer(sourcePath, directories, files, DESTINATION_PATH + destinationFolder);
                }
                MessageBox.Show("Processamento concluído com sucesso.", String.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, String.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteWebConfig(string sourcePath)
        {
            string webConfigPath = sourcePath + @"\web.config";
            if (File.Exists(webConfigPath))
            {
                File.Delete(webConfigPath);
            }
        }

        private List<string> GetSelectedDestinationFolders()
        {
            List<string> DestinationFolders = new List<string>();
            foreach (CheckBox Check in spServers.Children)
            {
                if ((bool)Check.IsChecked)
                {
                    DestinationFolders.Add(Check.Content.ToString());
                }
            }
            return DestinationFolders;
        }

        private void CopyFilesToServer(string sourcePath, string[] directories, string[] files, string destinationPath)
        {
            foreach (var directory in directories)
            {
                Directory.CreateDirectory(directory.Replace(sourcePath, destinationPath));
            }
            foreach (string file in files)
            {
                File.Copy(file, file.Replace(sourcePath, destinationPath), true);
            }
        }
    }
}
