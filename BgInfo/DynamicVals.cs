using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;

namespace BgInfo
{
    /// <summary>
    /// Globale satic proteries 
    /// <c>DynamicVals</c>
    /// </summary>
    static class DynamicVals
    {
        /// <summary>
        /// the current Software Version Code
        /// </summary>
        public static string VersionStr = "1.51";

        /// <summary>
        /// the detected PC No
        /// </summary>
        public static string PCNo;

        /// <summary>
        /// the detected Room No
        /// </summary>
        public static string RoomNo;

# warning "Chnage Server path and addresses here "
        /// <summary>
        /// FTP Server directory with the backround image 
        /// (with the DNS or IPv4 Name of the FTP Server and the file name of the image as JPG)
        /// </summary>
        public static string ServerPhardDefault = "\\\\192.168.100.1\\HorizonBackundImage\\HorizonBackundImage.jpg";

        /// <summary>
        /// FTP Server Name as DNS- or iPv4-Name
        /// </summary>
        public static string FTPServerName = "192.168.100.1";

        /// <summary>
        /// the compite FTP Server directory as FTP path 
        /// (same as "ServerPhardDefault" but with "ftp://....")
        /// </summary>
        public static string FTPServerPhrad = "ftp://192.168.100.1\\HorizonBackundImage\\HorizonBackundImage.jpg";

        /// <summary>
        /// Setup here the computer name
        /// with this name will be the Comuter- and PC-Number decoded 
        /// </summary>
        public static string ComputerName  =>Environment.MachineName; // "TC1.74-DOZ";

        /// <summary>
        /// Information to write to the Backround image: free space on the hard drive of the client
        /// </summary>
        public static string Drive_Freespace;

        /// <summary>
        /// Information to write to the Backround image: the iPv4 address of the client
        /// </summary>
        public static string IP_Addrs;

        /// <summary>
        /// The uptime of the client in minutes
        /// </summary>
        public static String RunTimeMin;

        /// <summary>
        /// Handler to trigger a Windows notification
        /// </summary>
        private static System.Windows.Forms.NotifyIcon nicon = new System.Windows.Forms.NotifyIcon();

        /// <summary>
        /// the methode decodes the Computer name to read the 
        /// computer- and PC-No 
        /// </summary>
        /// <returns></returns>
        public static bool DecodeComputerName()
        {
            string RoomNameTemp = string.Empty;
            int startposAfterBuild = 0;
            string BuildingName= string.Empty;

#warning "Chnage decoding of the PC Name here"

            // check if the PC name is vailed as an TC Room Name
            if (!(ComputerName.IndexOf("T") == 0) && (ComputerName.IndexOf("C") == 1)) return false; // TC-D11-158-2

            int start_pos_Slach = ComputerName.IndexOf("-");
            if (start_pos_Slach < 1) return false;

            // Check of the room Number
            if ((((ComputerName.IndexOf("C", start_pos_Slach + 1) > 0) || (ComputerName.IndexOf("D", start_pos_Slach + 1) > 0))
                && (!(ComputerName.IndexOf("D", start_pos_Slach + 1) == ComputerName.IndexOf("DOZ", start_pos_Slach + 1) ))))
            {
                    

                int start_pos_Slach_next = ComputerName.IndexOf("-", start_pos_Slach + 1);
                if (start_pos_Slach_next < 1) return false;

                BuildingName = ComputerName.Substring(start_pos_Slach + 1, start_pos_Slach_next - (start_pos_Slach + 1));


                start_pos_Slach = start_pos_Slach_next;
                startposAfterBuild = start_pos_Slach;


                start_pos_Slach = ComputerName.IndexOf("-", start_pos_Slach + 1);
                if (start_pos_Slach < 1) return false;
            }
      

            int dotpos = ComputerName.IndexOf(".", startposAfterBuild);

            if (dotpos != -1) // Romm Format SS.RR 
            {
                if (startposAfterBuild == 0)
                    RoomNameTemp = ComputerName.Substring(2, dotpos - 2);
                else
                    RoomNameTemp = ComputerName.Substring(startposAfterBuild + 1, dotpos - (1 + startposAfterBuild));
                try
                {
                    Convert.ToUInt32(RoomNameTemp);
                }
                catch (FormatException) { return false; }
                catch (OverflowException) { return false; }
                
                string RoomNameTemp2= ComputerName.Substring(dotpos+1, start_pos_Slach - (dotpos + 1));
                try
                {
                    Convert.ToUInt32(RoomNameTemp2);
                }
                catch (FormatException) { return false; }
                catch (OverflowException) { return false; }


                if (BuildingName != String.Empty)
                    RoomNo = BuildingName + "/" + RoomNameTemp+"."+ RoomNameTemp2;
                else
                    RoomNo = RoomNameTemp + "." + RoomNameTemp2;

            }
            else
            {
                if(startposAfterBuild==0)
                    RoomNameTemp = ComputerName.Substring(2, start_pos_Slach - 2);
                else
                    RoomNameTemp = ComputerName.Substring(startposAfterBuild+1, start_pos_Slach -(1+ startposAfterBuild));
                try
                {
                    Convert.ToUInt32(RoomNameTemp);

                }
                catch (FormatException) { return false; }
                catch (OverflowException) { return false; }

                if (BuildingName != String.Empty)
                    RoomNo = BuildingName + "/" + RoomNameTemp;
                else
                    RoomNo = RoomNameTemp;
            }

            string PCNotemp = ComputerName.Substring(start_pos_Slach + 1, ComputerName.Length - (start_pos_Slach + 1));
            if (!(PCNotemp.Equals("DOZ")))
            {
                try
                {
                    Convert.ToUInt32(PCNotemp);

                }
                catch (FormatException) { return false; }
                catch (OverflowException) { return false; }
            }

            PCNo = PCNotemp;

            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////  Handlers to update the backround values IPv4-Adreesm,up time ... )                                                       /////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// help function to convert a Byte value to a MB- or GB-string
        /// </summary>
        /// <param name="size">
        /// Size value in Byte to convert
        /// </param>
        /// String with the converted Byte-value
        /// <returns></returns>
        private static string GetSize(long size)
        {
            if (size > 1 << 30)
                return $"{size >> 30} GB";
            return $"{size >> 20} MB";
        }

        /// <summary>
        /// Handler to reclalculate the free space on the Hardrive
        /// </summary>
        /// <param name="driveName">
        /// the drive Name (e.g.: C:\\\)"
        /// </param>
        /// the free drive size as an string
        /// <returns></returns>
        public static long UdpateDriveData(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    Drive_Freespace = GetSize(drive.TotalFreeSpace);
                    return drive.TotalFreeSpace >> 20;
                }
            }
            return -1;
        }

        /// <summary>
        ///  Handler to update the run time of the current season on the client computer
        /// </summary>
        public static void UpdateRunTime()
        {
            RunTimeMin = string.Format("{0:00}:{1:00} h", TimeSpan.FromMilliseconds(Environment.TickCount).Hours,
                TimeSpan.FromMilliseconds(Environment.TickCount).Minutes);
        }


        /// <summary>
        /// Handler to update the iPv4 address of the client computer  
        /// </summary>
        public static void GetIPaddrs()
        { 
            List<string> IP_AddrsList = new List<string>();

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            if (ni.Name.Contains("Blue")) continue;
                            if (ni.OperationalStatus != OperationalStatus.Up ) continue;
                            IP_AddrsList.Add(ni.Name.ToString() + " - IPv4: " + (ip.Address.ToString()));
                        }
                    }
                }
            }
            if (IP_AddrsList.Count > 0)
            {
                IP_Addrs = String.Empty;
                for (int i = 0; i < IP_AddrsList.Count && IP_AddrsList.Count < 4; i++)
                {
                    IP_Addrs = IP_AddrsList[i] + Environment.NewLine;
                }
            }
            else IP_Addrs = "NO CONNECTION";
        }
    }
}
