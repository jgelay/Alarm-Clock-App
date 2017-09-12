using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AlarmClockMobile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer dispatchTimer;
        public MainPage()
        {

            this.InitializeComponent();
            dispatchTimer = new DispatcherTimer();
            dispatchTimer.Tick += DispatchTimer_Tick;

            dispatchTimer.Interval = new TimeSpan(0, 0, 1);
            dispatchTimer.Start();
        }

        private void DispatchTimer_Tick(object sender, object e)
        {
            DateTime myDate = DateTime.Now;

            digitalTime.Text = myDate.ToString("hh:dd:ss tt");
            dateTime.Text = myDate.ToString("MMM dd, yyyy");
        }
    }
}
