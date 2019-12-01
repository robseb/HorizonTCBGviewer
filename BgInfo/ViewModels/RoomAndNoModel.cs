using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BgInfo.NativeMethods;
using System.Windows;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
using BgInfo.Models;
using System.Net.NetworkInformation;

namespace BgInfo.ViewModels
{
    /// <summary>
    /// Wrapper to handle the chnages inside the backround image
    /// </summary>
    class RoomAndNoModel : BindableBase
    {
        MonitorInfo _monitor;
        public Settings Settings { get; }

        public RoomAndNoModel(MonitorInfo monitor, Settings settings)
        {
            _monitor = monitor;
            Settings = settings;

            Refresh(false);
        }

        public string ComputerName => DynamicVals.ComputerName;

        public  string PCNo   => DynamicVals.PCNo;
        public  string RoomNo => DynamicVals.RoomNo;

        public string Drive_Freespace => DynamicVals.Drive_Freespace;

        public String RunTimeMin => DynamicVals.RunTimeMin;

        public String NetworkIPs => DynamicVals.IP_Addrs;
        

        public  string Resolution => $"{_monitor.rcMonitor.Width} X {_monitor.rcMonitor.Height}";



        /// <summary>
        /// Dispaced Timer event to update the values on the Backround image 
        /// </summary>
        /// <param name="raiseChanges"></param>
        public void Refresh(bool raiseChanges = true)
        {
            DynamicVals.UdpateDriveData("C:\\");
            DynamicVals.UpdateRunTime();

            if (raiseChanges)
            {
                RaisePropertyChanged(nameof(Resolution));
                RaisePropertyChanged(nameof(PCNo));
                RaisePropertyChanged(nameof(RoomNo));
                RaisePropertyChanged(nameof(Drive_Freespace));
                RaisePropertyChanged(nameof(RunTimeMin));
            }
        }

        /// <summary>
        /// Configuration of an event, that is triggert my network chaneges to 
        /// update the network status and the iPv4 address 
        /// </summary>
        public  void setNetworkChangeEvent()
        {
              DynamicVals.GetIPaddrs();
              RaisePropertyChanged(nameof(NetworkIPs));

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

        }

        /// <summary>
        /// Network Chnage event to upate the iPv4 address inside the backround Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            DynamicVals.GetIPaddrs();
            RaisePropertyChanged(nameof(NetworkIPs));

        }
    }
}
