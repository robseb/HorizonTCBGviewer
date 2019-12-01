using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Zodiacon.WPF;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace BgInfo
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		Mutex _oneInstanceMutex;

        BgInfoManager _mgr;
        BGPichtureUpdateHandler BGhandler;
        BgInfo.Views.InfoWindow infoWindow;

        private String BGnotifString;
        private System.Windows.Media.Brush BGnotifcolor;

        System.Windows.Forms.NotifyIcon nicon = new System.Windows.Forms.NotifyIcon();

        DispatcherTimer _timer;
        private static bool NETnotifcationOneTime = false;

        /// <summary>
        /// Application start up (MAIN) event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
		{
            /// load the Windows Minibar Icon 
            DynamicVals.DecodeComputerName();

            nicon.Icon = new System.Drawing.Icon("EITlogo.ico");
            nicon.Visible = true;
            nicon.Click += nIcon_Click;

            base.OnStartup(e);
            nicon.Text = "Robin Sebastian 2018:  HorizonTCBGviewer";
          
            // set of a mutex to allow to the app only once at the same time
            bool createNew;
			_oneInstanceMutex = new Mutex(false, "BgInfo_OneInstanceMutex", out createNew);
			if(!createNew) {
				Shutdown();
				return;
			}
            // configure a Upate Event timer to upate the Backround image after 5mins
            // this should help slower computer to start the seoson complite before the backround 
            // image is changed 
            BGhandler = new BGPichtureUpdateHandler();
            BGhandler.NotificationUdadateEvent += BGhandler_NotificationUdadateEvent;

            if (!(BGhandler.HandelRefrech(false)))
            {
                _timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(5) };
                _timer.Tick += _timer_Tick;
                _timer.Start();
            }

            _mgr = new BgInfoManager();
            _mgr.CreateWindows();

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            
        }

        #region events 


        /// <summary>
        /// Event to update the backround image 5mins after the start of the computer 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            if ((BGhandler.HandelRefrech(false)))
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// this event ist triggerd after a update try to show in an passiv way the 
        /// startus of the attempt on the Windows Minibar icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="notif"></param>
        /// <param name="BGcokor"></param>
        private void BGhandler_NotificationUdadateEvent(object sender, EventArgs e, string notif, System.Windows.Media.Brush BGcokor)
        {
            if (infoWindow != null)
            {
                infoWindow.chageBGinfoLabel = notif;
                infoWindow.chageBGinfoLabelBGcolor = BGcokor;
            }
            BGnotifString = notif;
            BGnotifcolor = BGcokor;
            if(BGcokor == Brushes.Green)
             nicon.Text = "Robin Sebastian 2018: HorizonTCBGviewer [OKAY]";
            else
             nicon.Text = "Robin Sebastian 2018: HorizonTCBGviewer [ERROR]";
        }

        /// <summary>
        /// this event is triggert by an click on the windows minibar icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void nIcon_Click(object sender, EventArgs e)
        {
            if (infoWindow == null)
            {
                infoWindow = new Views.InfoWindow();

                infoWindow.chageBGinfoLabel = BGnotifString;
                infoWindow.chageBGinfoLabelBGcolor = BGnotifcolor;
                infoWindow.RefrechBGPictureEvent -= InfoWindow_RefrechBGPictureEvent;
                infoWindow.RefrechBGPictureEvent += InfoWindow_RefrechBGPictureEvent;
                infoWindow.Closed -= InfoWindow_Closed;
                infoWindow.Closed += InfoWindow_Closed;
                infoWindow.Show();
            }
        }

        /// <summary>
        /// this event is called after the info window was closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoWindow_Closed(object sender, EventArgs e)
        {
            infoWindow = null;
        }

        /// <summary>
        /// this event is called after the reface button inside the 
        /// info window was pressed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoWindow_RefrechBGPictureEvent(object sender, EventArgs e)
        {
            if (BGhandler != null)
                BGhandler.HandelRefrech(true);
        }

        /// <summary>
        /// this event is called after no network connection is avelibil 
        /// to trigger a Windows notification 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            DynamicVals.GetIPaddrs();
            if (DynamicVals.IP_Addrs == null || DynamicVals.IP_Addrs == String.Empty || DynamicVals.IP_Addrs == "NO CONNECTION") 
            {
                if (!NETnotifcationOneTime)
                {
                    nicon.ShowBalloonTip(5, "No Network Connection", "Please check the network connection of the client ", System.Windows.Forms.ToolTipIcon.Error);
                    NETnotifcationOneTime = true;
                }
            }
            else NETnotifcationOneTime = false;
        }

        #endregion
    }
}


