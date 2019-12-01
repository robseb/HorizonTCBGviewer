using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BgInfo
{
    /// <summary>
    ///  Class <c>FTPdowloadHandler</c> FTP Client to download, updating and update checks 
    ///  for Backuround Images
    /// </summary>
    public class FTPdowloadHandler
    {
        /// <summary>
        ///  Change here the FTP User Name and Password 
        ///  for the FTP Server connection to the Backround image download
        /// </summary>
        /// 
        private const string ServerUser =  "FTPServerUser";
        private const string SeverPassword= "FTPServerPassword";
# warning "Chnage the FTP Server USER and PASSWORD"


        /// <summary>
        /// FTP Client to downlaod the new backround image from a FTP Server 
        /// </summary>
        /// <param name="source">
        /// Backround Image Location on the FTP Server
        /// (global defined by "DynamicVals.FTPServerPhrad") 
        /// </param>
        /// <param name="destination"></param>
        /// the location to copy the Backround file on the clinet
        /// <param name="showMessages"></param>
        /// True enableds in case of an error a error message box
        /// <returns></returns>
        public bool DownloadFile(string source, string destination, bool showMessages)
        {
            try
            {
                using (WebClient request = new WebClient())
                {
                    request.Credentials = new NetworkCredential(ServerUser, SeverPassword);
                  
                    byte[] fileData = request.DownloadData(source);

                    using (FileStream file = File.Create(destination))
                    {
                        file.Write(fileData, 0, fileData.Length);
                        file.Close();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                if(showMessages) 
                    MessageBox.Show("FTP Backround Image downolad failed " + ex.Message, "FTP Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// to read the motification date of the backround image on the FTP Server 
        /// </summary>
        /// <param name="source">#
        ///  Backround Image Location on the FTP Server
        /// (global defined by "DynamicVals.FTPServerPhrad") 
        /// </param>
        /// <param name="showMessages">
        /// True enableds in case of an error a error message box
        /// </param>
        /// the motification date of the backround image file on the server
        /// <returns></returns>
        public DateTime ReadModDate(string source, bool showMessages)
        {
            DateTime dateTime = new DateTime();
            try
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(source);
                request.Credentials = new NetworkCredential(ServerUser, SeverPassword);
                request.Timeout = 10000;

                request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return dateTime = response.LastModified;
            }
            catch (Exception ex)
            {
                if (showMessages) MessageBox.Show("Failed to read the Backround Image on the FTP Server" 
                            + ex.Message, "FTP Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return dateTime;
            }
        }
    }
}
