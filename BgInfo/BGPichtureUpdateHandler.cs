using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Reflection;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Drawing;
using System.Net.NetworkInformation;

namespace BgInfo
{
    /// <summary>
    /// Backround Image Update Handler 
    /// <c>
    /// BGPichtureUpdateHandler
    /// </c>
    /// </summary>
    public class BGPichtureUpdateHandler
    {
        #region properties 
        [System.Runtime.InteropServices.DllImport("User32", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);
        FTPdowloadHandler fTPdowload;
        private string PicFullPath;

        public delegate void NotificationUdadate(object sender, EventArgs e, String notif, System.Windows.Media.Brush BGcokor);
        public event NotificationUdadate NotificationUdadateEvent;

        DateTime ServerPicDate;

        #endregion

        /// <summary>
        /// Handler to update the Backround Image is triggered by the user (info Window)
        /// or after every start of the app 
        /// </summary>
        public BGPichtureUpdateHandler()
        {
            fTPdowload = new FTPdowloadHandler();
        }

        /// <summary>
        /// Function cheks if the current used windows Backround image was setted by the app
        /// and not changed by the user 
        /// </summary>
        /// <returns></returns>
        public bool CheckIfBGPisOK ()
        {
            StringBuilder curentbackroundDir = new StringBuilder(300);
            String currentDir = String.Empty;
            try
            {
                SystemParametersInfo(0x0073, 300, curentbackroundDir, 0);
            } 
            catch (Exception) { }

            currentDir = curentbackroundDir.ToString();

            return (currentDir.Contains("ThinClientBackroundImage"));
        }

        /// <summary>
        /// Chnage the windows backround image 
        /// </summary>
        /// sucsess?
        /// <returns></returns>
        public bool SetBGPicture()
        {
            try
            {
               // DateTime ServerPicDate = File.GetLastWriteTime(PicFullPath);
                Properties.Settings.Default.BackRoundPichtureChangeTime = ServerPicDate;
            }  
            catch(Exception)
            {
                Properties.Settings.Default.BackRoundPichtureChangeTime = DateTime.Now;
            }

            StringBuilder picDirBuild = new StringBuilder(300);
            picDirBuild.Append(PicFullPath);

            try
            {
                SystemParametersInfo(0x0014, 0, picDirBuild, 0x0001);
            }
            catch (Exception) { return false; }

            Properties.Settings.Default.count++;
            Properties.Settings.Default.Save();

            return true;
        }

        /// <summary>
        /// Delate an old Image from the Temp-Folder of client 
        /// </summary>
        /// <param name="showMSG">
        /// show in case of an error an Windows Message box 
        /// </param>
        public void delateOldPic(bool showMSG)
        {
            try
            {
                if (Properties.Settings.Default.count > 0)
                {
                    int c = Properties.Settings.Default.count - 2;
                    string locp = Path.GetTempPath() + "\\ThinClientBackroundImage_" + c.ToString() + ".jpg";// PicDirPath + "\\ThinClientBackroundImage.jpg";
                    if (File.Exists(locp))
                        File.Delete(locp);
                }
            }
            catch (Exception ex) { if(showMSG)System.Windows.MessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Check if the server picture is still up to date 
        /// (by the motification date of both images)
        /// </summary>
        /// <param name="ServerPhard">
        /// the file of image on the FTP Server 
        /// </param>
        /// <param name="showMSG">
        ///  show in case of an error an Windows Message box 
        /// </param>
        /// <returns></returns>
        public bool CheckDateServerPic(string ServerPhard, bool showMSG)
        {
            bool eql = false;
            try
            {
                ServerPicDate = fTPdowload.ReadModDate(ServerPhard, showMSG);
                eql = ServerPicDate.Day.Equals(Properties.Settings.Default.BackRoundPichtureChangeTime.Day);

            }
            catch (Exception ex)
            {
                if(showMSG) System.Windows.MessageBox.Show(ex.Message);
            }
                return eql;
        }

        public void InstallMeOnStartUp()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
            }
            catch { }
        }

        private bool checkServerConnecttion()
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(DynamicVals.FTPServerName);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException )
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        public bool HandelRefrech(bool showMSGs)
        {
            Properties.Settings.Default.Reload();

            string Daytag = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " :";

            PicFullPath = Path.GetTempPath() + "\\ThinClientBackroundImage_" + Properties.Settings.Default.count.ToString() + ".jpg";// PicDirPath + "\\ThinClientBackroundImage.jpg";

            // is FTP Server there 
            if(!(checkServerConnecttion()))
            {
                // FTP Sever is not there 
                NotificationUdadateEvent?.Invoke(this, null, Daytag + "FTP Server is not reachable ", System.Windows.Media.Brushes.Red);
                if (showMSGs) MessageBox.Show("FTP Server is not reachable");
                return false;
            }

            if (!Properties.Settings.Default.FirstStart)// || ServerSettingUpdate)
            {
                // first start
                if (!(fTPdowload.DownloadFile(DynamicVals.FTPServerPhrad, PicFullPath, showMSGs))) return false; // Bild von Server auf Bilder lokal kopieren

                if (!(SetBGPicture())) return false;   // Bild als Hintergrund setzen
                delateOldPic(showMSGs);
                // set me to auto start 
                if (Properties.Settings.Default.FirstStart) InstallMeOnStartUp();

                Properties.Settings.Default.FirstStart = true;
                Properties.Settings.Default.Save();
                return true;
            }
            else
            {
                // not first Start
                if (CheckDateServerPic(DynamicVals.FTPServerPhrad, showMSGs) && CheckIfBGPisOK())
                {
                    if (showMSGs) MessageBox.Show("Picture is still up to date");
                    NotificationUdadateEvent?.Invoke(this, null, Daytag + "Picture is still up to date", System.Windows.Media.Brushes.Green);
                    return true; // Bild ist noch aktuell
                }
                // Bild ist nicht mehr aktuell 
                if (!(fTPdowload.DownloadFile(DynamicVals.FTPServerPhrad, PicFullPath, showMSGs))) // Bild von Server auf Bilder lokal kopieren
                {
                    if (NotificationUdadateEvent != null)
                        NotificationUdadateEvent?.Invoke(this, null, Daytag + "Picture could not be downloaded", System.Windows.Media.Brushes.Red);

                    if (showMSGs) MessageBox.Show("Picture could not be downloaded");
                    return false; // Bild ist noch aktuell
                }

                if (!(SetBGPicture()))   // Bild als Hintergrund setzen
                {
                    if (NotificationUdadateEvent != null)
                        NotificationUdadateEvent?.Invoke(this, null, Daytag + "Image has been downloaded; but can not be set as BG", System.Windows.Media.Brushes.Red);

                    if (showMSGs) MessageBox.Show("Image has been downloaded; but can not be set as BG");
                    return false; // Bild ist noch aktuell
                }

                NotificationUdadateEvent?.Invoke(this, null, Daytag + "Image has been downloaded and updated", System.Windows.Media.Brushes.Green);
                if (showMSGs) MessageBox.Show("Image has been downloaded and updated");
                delateOldPic(showMSGs);

            }
            return true;
        }
    }
}
