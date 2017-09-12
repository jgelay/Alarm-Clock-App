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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Alarm_Clock
{
    /// <summary>
    /// Interaction logic for UserAlarm.xaml
    /// </summary>
    public partial class UserAlarm : UserControl
    {
        private int id;

        private Alarm al;
        public UserAlarm(int id, Alarm alarm)
        {
            InitializeComponent();
            this.id = id;
            al = alarm;

        }
        // ID Method
        public int getID()
        {
            return id;
        }

        public void setAlarm(Alarm alarm)
        {
            this.al = alarm;
        }

        public Alarm getAlarm()
        {
            return al;
        }

        // The method deals with the selected alarm and loads the information
        // on to the slide menu.
        private void alarm_button_Click(object sender, RoutedEventArgs e)
        {

            MainWindow win = (MainWindow)Window.GetWindow(this);
            //win.slideMenu.Visibility = Visibility.Visible;
            win.slideMenuToggle(win.slideMenu, win.menuTogg);

            win.setAlarm_save.Visibility = Visibility.Hidden;
            win.editAlarm_save.Visibility = Visibility.Visible;
            win.setAlarm_delete.Visibility = Visibility.Visible;

             win.setCurrentAlarm(this);
            // Finding the position of the alarm in the alarms linked list

           win.setAlarm_hours.Content = al.getHour();
           win.setCurrentHour(al.getHour());
           win.checkBox.IsChecked = al.getRepeating();
            // Loads previous alarm values
            int getmin = al.getMin();
           win.setCurrentMin(al.getMin());
           if (getmin < 10)
           {
                win.setAlarm_minutes.Content = "0" + al.getMin();
           }
           else
           {
                win.setAlarm_minutes.Content = al.getMin();
            }

           win.setCurrentAMPM(al.getAMPM());
           if (al.getAMPM() == 0)
           {
                win.setAlarm_amORpm.Content = " AM";
           }
           else
           {
               win.setAlarm_amORpm.Content = " PM";
           }

            alarm_title.Content = al.getName();
            win.alarm_name.Text = al.getName();

            int counter = 0;
            foreach(bool p in al.getDays())
            {
               
                if(counter == 0)
                {
                    changeColor(win.sun_button, p);
                }else if(counter == 1)
                {
                     changeColor(win.mon_button, p);
                }else if(counter == 2)
                {
                    changeColor(win.tues_button, p);
                }else if(counter == 3)
                {
                    changeColor(win.wed_button, p);

                }else if(counter == 4)
                {
                    changeColor(win.thurs_button, p);

                }else if(counter == 5)
                {
                    changeColor(win.fri_button, p);

                }else if(counter == 6){
                    changeColor(win.sat_button, p);

                }
                counter++;

            }

            //  set changed alarm values for the alrm in alarms linked list

            // }
        }
        
        // This method changes the days button color
        private void changeColor(Button name, bool value)
        {
            if (value)
            {
                name.Background = Brushes.LightSeaGreen;
            }
            else
            {
                name.Background = new SolidColorBrush(Color.FromRgb(216, 241, 228));
            }
        }

        // Method that snoozes the alarm
        private void snooze_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.setCurrentAlarm(this);
            
        }

        // Method that deletes the alarm
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = (MainWindow)Window.GetWindow(this);
            win.setCurrentAlarm(this);
            win.deleteAlarm();
        }
    }

    }

