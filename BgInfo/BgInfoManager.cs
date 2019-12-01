using BgInfo.ViewModels;
using BgInfo.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Controls;
using static BgInfo.NativeMethods;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reflection;
using Zodiacon.WPF;
using System.ComponentModel.Composition.Hosting;
using BgInfo.Models;
using System.Windows.Threading;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.IO;


namespace BgInfo {
    class BgInfoManager {
        ObservableCollection<RoomAndNoModel> _screens = new ObservableCollection<RoomAndNoModel>();
        DispatcherTimer _timer;

        public Settings Settings { get; } = new Settings();

        public BgInfoManager() {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }


        private void _timer_Tick(object sender, EventArgs e) {
            Refresh();
        }


        public int CreateWindows() {
            var windows = 0;

                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr data) => {
                Debug.WriteLine($"monitor: {hMonitor}");

                var info = new MonitorInfo();
                info.Init();
                GetMonitorInfo(hMonitor, ref info);

                var vm = new RoomAndNoModel(info, Settings);
      
                var win = new MainView {
                    Left = info.rcWork.Left,
                    Top = info.rcWork.Top,
                    Width = info.rcWork.Width,
                    Height = info.rcWork.Height,
                    DataContext = vm
              //      Background = new ImageBrush(new BitmapImage(new Uri("ThinClientBackroundImage.jpg", UriKind.Relative)))
            };
                _screens.Add(vm);

                vm.setNetworkChangeEvent();
                win.Show();
                windows++;
                return true;
            }, IntPtr.Zero);


            return windows;
        }
        public void Refresh() {
            foreach(var screen in _screens)
                screen.Refresh();
        }
    }
}
