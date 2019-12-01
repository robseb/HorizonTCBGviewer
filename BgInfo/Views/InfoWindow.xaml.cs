using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace BgInfo.Views
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public delegate void RefrechBGPicture(object sender, EventArgs e);
        public event RefrechBGPicture RefrechBGPictureEvent;

        public string chageBGinfoLabel
        {
            set
            {
                BGmesLabel.Content = value;
            }
        }

        public System.Windows.Media.Brush chageBGinfoLabelBGcolor
        {
            set
            {
                BGmesLabel.Background = value;
            }
        }

        public InfoWindow()
        {

            InitializeComponent();

            BGPichtureUpdateHandler bgInfo = new BGPichtureUpdateHandler();
            TMP_Dir.Content = System.IO.Path.GetTempPath();
            Server_Dir.Content = DynamicVals.FTPServerPhrad;

            Properties.Settings.Default.Reload();

            mod_date.Content = Properties.Settings.Default.BackRoundPichtureChangeTime.ToLongDateString();


            Version.Content = DynamicVals.VersionStr;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (RefrechBGPictureEvent != null)
                RefrechBGPictureEvent(this, e);

        }
    }
}
