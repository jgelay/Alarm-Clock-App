using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Alarm_Clock
{
    
    public partial class MainWindow : Window 
    {
        //these are the objects of the min, hour, and second hand for the analog clock
        private RotateTransform MinHandTr = new RotateTransform();
        private RotateTransform HourHandTr = new RotateTransform();
        private RotateTransform SecHandTr = new RotateTransform();

        public UserAlarm currAlarm;

        public bool[] currDays;

        private int createAlarmHour = 12;
        private int createAlarmMin = 0;
        private int createAlarmAMPM = 0;

        private System.Media.SoundPlayer player;
        private String alarmTitle;
        private double timeMult = 0;
        private static int idSet = 0;
        private Button[] daysList;

        public int menuTogg = 0;

        public LinkedList<Alarm> alarms = new LinkedList<Alarm>();
        public LinkedList<UserAlarm> uAlarms = new LinkedList<UserAlarm>();
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        AlarmRing ring = new AlarmRing();
        private IFormatter formatter;
        private Stream stream;
        DateTime myDate;
        private string currDay;

        public MainWindow()
        {
            //initializes the clock  
            InitializeComponent();

            //initalizes the combo-box for selecting the alarm sound 
            this.AlarmSelectSound.SelectedValuePath = "Key";
            this.AlarmSelectSound.DisplayMemberPath = "Value";
            //how works | value = string of the sound clip | text = what is displayed on the combo box | (Value, Text) 
            this.AlarmSelectSound.Items.Add(new KeyValuePair<String, string>(" ", "Default"));
            this.AlarmSelectSound.Items.Add(new KeyValuePair<String, string>(" ", "Metal Crunch"));
            this.AlarmSelectSound.Items.Add(new KeyValuePair<String, string>(" ", "Gun Shot"));
            this.AlarmSelectSound.Items.Add(new KeyValuePair<String, string>(" ", "Buzz"));
            this.AlarmSelectSound.Items.Add(new KeyValuePair<String, string>(" ", "Hammer"));
            this.AlarmSelectSound.Items.Add(new KeyValuePair<String, string>(" ", "Kiss"));
            this.AlarmSelectSound.Items.Add(new KeyValuePair<String, string>(" ", "Laser"));
            formatter = new BinaryFormatter();
            if (File.Exists("MyFile.bin") == true)
            {
                loadAlarmObjects();
            }

            
            formatter = new BinaryFormatter();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer(DispatcherPriority.Render);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            myDate = DateTime.Now;
            currDay = myDate.ToString("dddd");
            dispatcherTimer.Start();

            ring.AlarmRings += Ring_AlarmRings;

            daysList = new Button[7]{sun_button, mon_button, tues_button, wed_button, thurs_button,fri_button,sat_button};

            this.KeyUp += MainWindow_KeyUp;
        }

        // Loads the serialized objects from MyFile.bin
        private void loadAlarmObjects()
        {
            stream = new FileStream("MyFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
            while (stream.Position != stream.Length)
            {
                Alarm deserializedAlarm = (Alarm)formatter.Deserialize(stream);
                alarms.AddLast(deserializedAlarm);

                UserAlarm userAlarm = new UserAlarm(deserializedAlarm.getID(), deserializedAlarm);
                userAlarm.alarm_button.Content = deserializedAlarm.getString();
                userAlarm.alarm_title.Content = "";
                this.updateControl(userAlarm, userAlarm.getAlarm().getDays());

                uAlarms.AddLast(userAlarm);

                // Updating Stack Panel
                stacky.Children.Add(userAlarm);

                // Linking the user alarm to the alarm object
                deserializedAlarm.setUserAlarm(userAlarm);
            }

            stream.Close();
        }
        //Checks to see if the slide menu is visible,
        //if so then hide the menu, also reset values to default
        //If the slide menu is not visible, it will make the menu visible
        public void slideMenuToggle(Canvas slideMenu, int menuTogg)
        {
            moveSlideMenu(slideMenu, menuTogg);
            this.menuTogg = (menuTogg + 1) % 2;

            if (plusButton.Content.ToString() == "-")
            {
                plusButton.Content = "+";
                setAlarm_delete.Visibility = Visibility.Hidden;
                alarm_name.Text = "";
                checkBox.IsChecked = false;
                foreach(Button p in daysList)
                {
                    p.Background = new SolidColorBrush(Color.FromRgb(216, 241, 228));

                }
            }
            else
            {
                plusButton.Content = "-";
            }        
           
        }

        // Animates the slide menu.
        public static void moveSlideMenu(Canvas slideMenu, int menuTogg)
        {
            TranslateTransform trans = new TranslateTransform();
            slideMenu.RenderTransform = trans;
            DoubleAnimation anim = null;

            // Slide menu is hidden, so slide it out
            if (menuTogg == 0)
            {
                anim = new DoubleAnimation(0, -600, TimeSpan.FromSeconds(0.5));
            }

            // Slide menu is visible, so put it back
            else
            {
                anim = new DoubleAnimation(-600, 0, TimeSpan.FromSeconds(0.5));
            }

            trans.BeginAnimation(TranslateTransform.XProperty, anim);

        }

        private void Ring_AlarmRings(object sender, AlarmEventArgs e)
        {
            player = new System.Media.SoundPlayer(e.currAl.getAlarm().getActualRinger(e.currAl.getAlarm().getRingerPath()));
            player.Load();
            player.Play();
            currAlarm = e.currAl;

            this.alertCanvas1.Visibility = Visibility.Visible;
            this.alertCanvas2.Visibility = Visibility.Visible;
        }

        /* this method is an event driven system for analog and digital clock
         * event is called every "tick" which is one second
         * it also updates the clocks each second
         */
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            
            if (!myDate.ToString("dddd").Equals(currDay))
            {
                currDay = myDate.ToString("dddd");
                resetDismissed();
            }
            /* digital clock
             * displays the time, date and am
             */
            myDate = timeMultiplier(myDate);
            digitalTime.Content = myDate.ToString("hh:mm:ss"); //hours:minutes: seconds   
            amORpm.Content = myDate.ToString("tt"); //AM or PM 
            date.Content = myDate.ToString("MMM dd, yyyy"); //month day, year 

            //analog clock
            //calculates angles for the minute, hour and second hand 
            MinHandTr.Angle = (myDate.Minute * 6);
            HourHandTr.Angle = (myDate.Hour * 30) + (myDate.Minute * 0.5);
            SecHandTr.Angle = (myDate.Second * 6);

            //moves the minute, second and hour hand  
            MinuteHand.RenderTransform = MinHandTr;
            HourHand.RenderTransform = HourHandTr;
            SecondHand.RenderTransform = SecHandTr;
            alarmCheck(ring);
            myDate = myDate.AddSeconds(1);
            timeMult = 0;
        }

        // This method gets called whenever a new day begins
        private void resetDismissed()
        {
            if (uAlarms.Last() != null)
            {
                foreach(UserAlarm u in uAlarms)
                {
                    u.getAlarm().dismissed = false;
                }
            }
        }

        // For testing purposes this method adds 60 seconds to the current date time
        private DateTime timeMultiplier(DateTime myDate)
        {
            myDate = myDate.AddSeconds(timeMult);
            return myDate;
        }

        private void plusButton_Click(object sender, RoutedEventArgs e)
        {
            editAlarm_save.Visibility = Visibility.Hidden;
            setAlarm_save.Visibility = Visibility.Visible;
            if (menuTogg == 1)
            {
                //reset the create alarm to inital values
                createAlarmAMPM = 0;
                createAlarmHour = 12;
                createAlarmMin = 0;
                //update the visuals for the reset values
                setAlarm_minutes.Content = "0" + "0";
                setAlarm_hours.Content = 12;
                setAlarm_amORpm.Content = "AM";
            }

            slideMenuToggle(slideMenu, menuTogg);
        }
       
        /* This method closes the program down if the escape key is hit
         * adds a minute to the current date time when B key is hit
         * adds 24 hours to the current date time when C key is hit
         * 
         */
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                stream.Close();
                Application.Current.Shutdown();
            }
            else if (e.Key == Key.B)
            {
                timeMult = timeMult + 60;
            }
            else if (e.Key == Key.C)
            {
                timeMult = timeMult + 86400;
            }
        }


        //**When creating a new alarm** - decriment of the mins button  
        private void setAlarm_decMinutes_Click(object sender, RoutedEventArgs e)
        {
            //if there are 0 mins in the label clicking up will change it to 59 mins
            if (createAlarmMin == 0)
            {
                createAlarmMin = 59;
                setAlarm_minutes.Content = createAlarmMin.ToString();
            }
            //if the minutes are inbetween or equal to 0 and 9 then add an extra "0" to format it 
            else if (createAlarmMin >= 0 && createAlarmMin < 11)
            {
                createAlarmMin -= 1;
                setAlarm_minutes.Content = "0" + createAlarmMin.ToString();
            }
            //otherwise just update the current minutes and update the label 
            else
            {
                createAlarmMin -= 1;
                setAlarm_minutes.Content = createAlarmMin.ToString();
            }
        }


        //**When creating a new alarm** - increment of the the mins button
        private void setAlarm_incMinutes_Click(object sender, RoutedEventArgs e)
        {
            //if there are 59 mins in the label clicking up will change it to 00 mins
            if (createAlarmMin == 59)
            {
                createAlarmMin = 0;
                setAlarm_minutes.Content = "0" + createAlarmMin.ToString();
            }
            //if the minutes are inbetween or equal to 0 and 9 then add an extra "0" to format it 
            else if (createAlarmMin >= 0 && createAlarmMin < 9)
            {
                createAlarmMin += 1;
                setAlarm_minutes.Content = "0" + createAlarmMin.ToString();
            }
            //otherwise just update the current minutes and update the label 
            else
            {
                createAlarmMin += 1;
                setAlarm_minutes.Content = createAlarmMin.ToString();
            }
        }


        //**When creating a new alarm** - decriment of the the hours button 
        private void setAlarm_decHours_Click(object sender, RoutedEventArgs e)
        {
            //if there are 0 hours in the label clicking down will change it to 12 mins
            if (createAlarmHour == 0 || createAlarmHour == 1)
            {
                createAlarmHour = 12;
                setAlarm_hours.Content = createAlarmHour.ToString();
            }
            
            //otherwise just update the current hours and update the label 
            else
            {
                createAlarmHour -= 1;
                setAlarm_hours.Content = createAlarmHour.ToString();
            }
        }

        //**When creating a new alarm** - increment of the hours button
        private void setAlarm_incHours_Click_1(object sender, RoutedEventArgs e)
        {
            //if there are 12 hours in the label clicking up will change it to 00 mins
            if (createAlarmHour == 12)
            {
                createAlarmHour = 1;
                setAlarm_hours.Content = createAlarmHour.ToString();
            }

            //if the hours are inbetween or equal to 0 and 9 then add an extra "0" to format it 
            else
            {
                createAlarmHour += 1;
                setAlarm_hours.Content = createAlarmHour.ToString();
            }
        }

        //**When creating a new alarm** - changing from "am -> pm" or "pm -> am"
        private void setAlarm_amORpm_Click(object sender, RoutedEventArgs e)
        {
            //if the button is on AM it will change to PM
            if (createAlarmAMPM == 0)
            {
                createAlarmAMPM = 1;
                setAlarm_amORpm.Content = "PM";
            }

            //otherwise the button is on PM and it will change to AM
            else
            {
                createAlarmAMPM = 0;
                setAlarm_amORpm.Content = "AM";
            }
        }

        // This method saves all the current relevant information that the user selected when 
        // creating an alarm (such as the time, whether it is repeating or not, etc.

        public void setAlarm_save_Click(object sender, RoutedEventArgs e)
        {
            // ** Need to also check if it's repeating and send the last bool acordingly

            alarmTitle = alarm_name.Text;

            bool[] days = checkDays(daysList);

            bool repeating =false;
            //Checks the option for repeating
            if(checkBox.IsChecked == true)
            {
                repeating = true;
            }else
            {
                repeating = false;
            }

            Alarm myAlarm = new Alarm(createAlarmHour, createAlarmMin, createAlarmAMPM, repeating, alarmTitle, days);
            myAlarm.setID(idSet + 1);
            myAlarm.dismissed = false;
            

            //Getting the String and putting it in the linked list
            String temp = myAlarm.getString();
            alarms.AddLast(myAlarm);

            String s = AlarmSelectSound.Text;
            
            // Creating new User Alarm and adding it to linked list
            UserAlarm userAlarm = new UserAlarm(idSet, myAlarm);

            if (s.Equals("Default"))//if user selected default alarm sound 
            {
                userAlarm.getAlarm().setRingerPath(@"Default.wav");

            }

            else if (s.Equals("Metal Crunch"))
            {
                userAlarm.getAlarm().setRingerPath(@"metal_crunch.wav");

            }

            else if (s.Equals("Gun Shot"))
            {
                userAlarm.getAlarm().setRingerPath(@"gun.wav");

            }
            
            else if (s.Equals("Buzz"))
            {
                userAlarm.getAlarm().setRingerPath(@"neon_light.wav");

            }

            else if (s.Equals("Hammer"))
            {
                userAlarm.getAlarm().setRingerPath(@"hammer_anvil2.wav");

            }

            else if (s.Equals("Kiss"))
            {
                userAlarm.getAlarm().setRingerPath(@"kiss_x.wav");

            }

            else if (s.Equals("Laser"))
            {
                userAlarm.getAlarm().setRingerPath(@"laser_x.wav");

            }

            userAlarm.alarm_button.Content = temp;
            userAlarm.alarm_title.Content = alarm_name.Text;
            //Checks to see which days have been selected for the alarm to ring
            //Update the visual on the alarm to notify user of their days selection
            updateControl(userAlarm, days);
            //Update linked list that contains all alarms
            uAlarms.AddLast(userAlarm);

            // Updating Stack Panel
            stacky.Children.Add(userAlarm);

            // Linking the user alarm to the alarm object
            myAlarm.setUserAlarm(userAlarm);
            stream = new FileStream("MyFile.bin", FileMode.Append, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, myAlarm);
            stream.Close();
            slideMenuToggle(slideMenu, menuTogg);
        }
    
        //Takes in a boolean list of days according to which days the user has selected
        //Will update user control to bold days that where chosen
       private void updateControl(UserAlarm ala, bool[] date)
        {
            int counter = 0;
            TextBlock name = ala.sun_label;
            foreach (bool p in date)
            { 
            
                if (counter == 0)
                {
                    name = ala.sun_label;
                }
                else if (counter == 1)
                {
                    name = ala.mon_label;
                }
                else if (counter == 2)
                {
                    name = ala.tues_label;
                }
                else if (counter == 3)
                {
                    name = ala.wed_label;
                }
                else if (counter == 4)
                {
                    name = ala.thurs_label;
                }
                else if (counter == 5)
                {
                    name = ala.fri_label;
                }
                else if (counter == 6)
                {
                    name = ala.sat_label;
                }

                if (p)
                {
                    name.FontWeight = FontWeights.Bold;
                                 
                }else
                {
                    name.FontWeight = FontWeights.Regular;
                                       
                }
                counter++;
            }


        }
       
        //Will take in a list of buttons (day buttons from the slide menu) and
        //check if it was selected. Return a list of boolean of whether the days
        //were selected or not. Order: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
        private bool[] checkDays(Button[] list)
        {
            bool[] read = new bool[7];
            int x = 0;
            foreach(Button p in list){
                if(p.Background == Brushes.LightSeaGreen)
                {
                    read[x] = true;
                }else
                {
                    read[x] = false;
                }
                x++;
            }

            return read;
        }

        // This method deletes a selected alarm
        // this is the delete method for the button beside the change button
        // 
        private void setAlarm_delete_Click(object sender, RoutedEventArgs e)
        {
            this.deleteAlarm();
            slideMenuToggle(slideMenu, menuTogg);
        }
        public void deleteAlarm()
        {
            uAlarms.Remove(currAlarm);
            stacky.Children.Remove(currAlarm);

            stream = new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            foreach (UserAlarm uAlarm in uAlarms)
            {
                formatter.Serialize(stream, uAlarm.getAlarm());
            }
            stream.Close();
            
        }

        // This method calls the compare time in Alarm Rings class
        // it converts the time into from each alarm into a string 
        // and calls compare Time
        private void alarmCheck(AlarmRing ring)
        {
            // Getting the alarm itme in "hh:mm" format
            if (uAlarms.Last != null)
            {
                foreach (UserAlarm uAlarm in uAlarms)
                {
                    String ampm = null;
                    String min = null;
                    ampm = (uAlarm.getAlarm().getAMPM() == 1 ? "PM" : "AM");
                    if (uAlarm.getAlarm().getMin().ToString().Length == 1)
                    {
                        min = "0" + uAlarm.getAlarm().getMin().ToString();
                    }
                    else
                    {
                        min = uAlarm.getAlarm().getMin().ToString();
                    }
                    String checker = uAlarm.getAlarm().getHour().ToString() + ":" + min + " " + ampm;
                    ring.compareTime(uAlarm, checker, myDate);

                }
            }
        }

        // This method sets the current Useralarm to be the currAlarm
        public void setCurrentAlarm(UserAlarm al)
        {
            currAlarm = al;
        }

        // This method describes the edit functionality when a user
        // clicks a specific alarm that was already created
        // similar to creating an alarm.
        private void editAlarm_save_Click(object sender, RoutedEventArgs e)
        {
             currAlarm.getAlarm().setHour(createAlarmHour);
             currAlarm.getAlarm().setMin(createAlarmMin);
             currAlarm.getAlarm().setAMPM(createAlarmAMPM);
             currAlarm.getAlarm().setOrigHour(createAlarmHour);
             currAlarm.getAlarm().setOrigMinute(createAlarmMin);
             currAlarm.getAlarm().setorigAmpm(createAlarmAMPM);

            bool repeating = false;

            if (checkBox.IsChecked == true)
            {
                repeating = true;
            }
            else
            {
                repeating = false;
            }

            currAlarm.getAlarm().setRepeat(repeating);
            
            String s = AlarmSelectSound.Text;

            if (s.Equals("Default"))//if user selected default alarm sound 
            {
                currAlarm.getAlarm().setRingerPath(@"Default.wav");

            }

            else if (s.Equals("Metal Crunch"))
            {
                currAlarm.getAlarm().setRingerPath(@"metal_crunch.wav");

            }

            else if (s.Equals("Gun Shot"))
            {
                currAlarm.getAlarm().setRingerPath(@"gun.wav");

            }

            else if (s.Equals("Buzz"))
            {
                currAlarm.getAlarm().setRingerPath(@"neon_light.wav");

            }

            else if (s.Equals("Hammer"))
            {
                currAlarm.getAlarm().setRingerPath(@"hammer_anvil2.wav");

            }

            else if (s.Equals("Kiss"))
            {
                currAlarm.getAlarm().setRingerPath(@"kiss_x.wav");

            }

            else if (s.Equals("Laser"))
            {
                currAlarm.getAlarm().setRingerPath(@"laser_x.wav");

            }

            currAlarm.getAlarm().dismissed = false;
            currAlarm.getAlarm().setSnooze(false);
            currAlarm.alarm_button.Content = currAlarm.getAlarm().getString();
            currAlarm.alarm_title.Content = alarm_name.Text;
                                                  
            currAlarm.alarm_button.Content = currAlarm.getAlarm().getString();

            currAlarm.getAlarm().setDays(checkDays(daysList));

            updateControl(currAlarm, currAlarm.getAlarm().getDays());

            stream = new FileStream("MyFile.bin", FileMode.Create, FileAccess.Write, FileShare.None);
            foreach (UserAlarm uAlarm in uAlarms)
            {
                formatter.Serialize(stream, uAlarm.getAlarm());
            }
            stream.Close();
            slideMenuToggle(slideMenu, menuTogg);
        }

        // Deleting the alarm
        private void dismiss_Click(object sender, RoutedEventArgs e)
        {
            this.alarmEventCanvas.Visibility = Visibility.Hidden;
        }
        public void setCurrentHour(int hour)
        {
            createAlarmHour = hour;
        }
        public void setCurrentMin(int min)
        {
            createAlarmMin = min;
        }
        public void setCurrentAMPM(int ampm)
        {
            createAlarmAMPM = ampm;
        }

        //for dismissing the alarm
        private void dismiss1_Click(object sender, RoutedEventArgs e)
        {

            if (currAlarm != null)
            {
                currAlarm.getAlarm().dismissed = true;
            }
            player.Stop();
            this.alertCanvas1.Visibility = Visibility.Hidden;
            this.alertCanvas2.Visibility = Visibility.Hidden;
            bool s = currAlarm.getAlarm().getRepeating();

            if (s == false)
            {
                deleteAlarm();
            }
            

        }

        // Method that describes the actions taken when a user snooze an alarm
        // The method updates the time to be one minute after the original tiime
        // The original time is stored for visual purposes while the time that is checked by
        // the alarmCheck method is the new updated time

        public void snooze_Click(object sender, RoutedEventArgs e)
        {

            //now makes the new alarm that rings 5 mins later (this alarm is hidden from user)
            //get values of current alarm
            int newHour;
            int newMin = myDate.Minute + 1;
            //int newSec = DateTime.Now.Second + 10;
            int newAMPM;
            if (myDate.ToString("tt") == "AM" || myDate.Hour == 12)
            {
                //its am
                newHour = myDate.Hour;
            }

            else
            {
                //its pm
                newHour = myDate.Hour - 12;
            }
            if (myDate.ToString("tt") == "AM")
            {
                newAMPM = 0;
            }
            else
            {
                newAMPM = 1;
            }
            //IF THE CURRENT ALARM IS 59MINS THE NEW MIN SHOULD BE 00 WITH HOURS BEING +1
            if (newMin >= 60)
            {
                newMin = 0;
                newHour = newHour + 1;
                //if the hour is 12 and we +1 it check if its 13, if it is then we make it == 1:00 
                if(newHour >= 13)
                {
                    newHour = 1;
                }
            
                else
                {
                  //if previously pm change to am
                  newAMPM = 0;
                }
            }
            

            if (currAlarm != null)
            {
                currAlarm.getAlarm().setHour(newHour);
                currAlarm.getAlarm().setMin(newMin);
                currAlarm.getAlarm().setAMPM(newAMPM);  
            }    
            //dissmisses the inital alarm 
            player.Stop();
            this.alertCanvas1.Visibility = Visibility.Hidden;
            this.alertCanvas2.Visibility = Visibility.Hidden;

            }

        

        //digital clock checkbox if it is checked
        private void checkBoxDigital_Checked(object sender, RoutedEventArgs e)
        {
            HandleDig(sender as CheckBox);
        }

        //digital clock checkbox if it is unchecked
        private void checkBoxDigital_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleDig(sender as CheckBox);
        }

        //the handler for the digital clock checkbox
        void HandleDig(CheckBox checkBox)
        {
            // Use IsChecked.
            bool flag = checkBox.IsChecked.Value;

            //if the checkbox is checked then hide the digital clock
            if(flag == true)
            {
                this.date.Visibility = Visibility.Hidden;
                this.digitalTime.Visibility = Visibility.Hidden;
                this.amORpm.Visibility = Visibility.Hidden;

                this.checkBoxAnalog.IsEnabled = false;
                //this.checkBoxAnalog.Visibility = Visibility.Hidden;

            }

            //otherwise if the flag is false then display thedigital clock
            if(flag == false)
            {

                this.date.Visibility = Visibility.Visible;
                this.digitalTime.Visibility = Visibility.Visible;
                this.amORpm.Visibility = Visibility.Visible;

                this.checkBoxAnalog.IsEnabled = true;
                //this.checkBoxAnalog.Visibility = Visibility.Visible;

            }


        }

        //analog clock checkbox if it is checked
        private void checkBoxAnalog_Checked(object sender, RoutedEventArgs e)
        {
            HandleAn(sender as CheckBox);
        }

        //analog clock checkbox if it is not checked
        private void checkBoxAnalog_UnChecked(object sender, RoutedEventArgs e)
        {
            HandleAn(sender as CheckBox);
        }

        //the handler for the digital clock checkbox
        void HandleAn(CheckBox checkBox)
        {
            // Use IsChecked.
            bool flag = checkBox.IsChecked.Value;

            //if the checkbox is checked then hide the digital clock
            if (flag == true)
            {
                //make circles invisible
                this.Circle1.Visibility = Visibility.Hidden;
                this.Circle2.Visibility = Visibility.Hidden;
                this.Circle3.Visibility = Visibility.Hidden;
                this.Circle4.Visibility = Visibility.Hidden;

                //make labels invisible
                this.Label12.Visibility = Visibility.Hidden;
                this.Label3.Visibility = Visibility.Hidden;
                this.Label6.Visibility = Visibility.Hidden;
                this.Label9.Visibility = Visibility.Hidden;

                //make hands invisible
                this.SecondHand.Visibility = Visibility.Hidden;
                this.MinuteHand.Visibility = Visibility.Hidden;
                this.HourHand.Visibility = Visibility.Hidden;

                this.checkBoxDigital.IsEnabled = false;
               // this.checkBoxDigital.Visibility = Visibility.Hidden;
            }

            //otherwise if the flag is false then display thedigital clock
            if (flag == false)
            {
                //make circles visible 
                this.Circle1.Visibility = Visibility.Visible;
                this.Circle2.Visibility = Visibility.Visible;
                this.Circle3.Visibility = Visibility.Visible;
                this.Circle4.Visibility = Visibility.Visible;

                //make labels visible
                this.Label12.Visibility = Visibility.Visible;
                this.Label3.Visibility = Visibility.Visible;
                this.Label6.Visibility = Visibility.Visible;
                this.Label9.Visibility = Visibility.Visible;

                //make hands visible
                this.SecondHand.Visibility = Visibility.Visible;
                this.MinuteHand.Visibility = Visibility.Visible;
                this.HourHand.Visibility = Visibility.Visible;

                this.checkBoxDigital.IsEnabled = true;
             //   this.checkBoxDigital.Visibility = Visibility.Visible;

            }
        }


        //light/dark mode of the program 
        private void light_dark_Click(object sender, RoutedEventArgs e)
        {
            //hide button and call second  
            light_dark.Visibility = Visibility.Hidden;
            light_dark2.Visibility = Visibility.Visible;

            //change background to dark (FFAF8FC1)
            var bc = new BrushConverter();
            this.MainWin.Background = (Brush)bc.ConvertFrom("#FF594735");

            //☀ ☾    FF594735   FFF1E4D8

        }
        //dark #FFAF8FC1

        private void light_dark2_Click(object sender, RoutedEventArgs e)
        {
            //hide button and call second button 
            light_dark2.Visibility = Visibility.Hidden;
            light_dark.Visibility = Visibility.Visible;

            //change background to light (FFD7C8EA) 
            var bc = new BrushConverter();
            this.MainWin.Background = (Brush)bc.ConvertFrom("#FFF1E4D8");
            //#FFF1E4D8

        }
        //Takes in a button argument and check whether it was selected( color is light sea green)
        //If pressed again, it will revert to its original color
        private void changeColor(Button name)
        {
            if (name.Background != Brushes.LightSeaGreen)
            {
                name.Background = Brushes.LightSeaGreen;
            }
            else
            {
                name.Background = new SolidColorBrush(Color.FromRgb(216, 241, 228));
            }
        }

        private void butSnoozeErr_Click(object sender, RoutedEventArgs e)
        {
            this.SnoozeError.Visibility = Visibility.Hidden;
            this.butSnoozeErr.Visibility = Visibility.Hidden;
        }
        //Check if the days button were selected, if so then toggle between colors
        private void Sun_Select(object sender, RoutedEventArgs e)
        {
            changeColor(sun_button);
        }

        private void mon_click(object sender, RoutedEventArgs e)
        {
            changeColor(mon_button);
        }

        private void tues_click(object sender, RoutedEventArgs e)
        {
            changeColor(tues_button);
        }

        private void wed_click(object sender, RoutedEventArgs e)
        {
            changeColor(wed_button);
        }

        private void thurs_click(object sender, RoutedEventArgs e)
        {
            changeColor(thurs_button);
        }

        private void fri_click(object sender, RoutedEventArgs e)
        {
            changeColor(fri_button);
        }

        private void sat_click(object sender, RoutedEventArgs e)
        {
            changeColor(sat_button);
        }
        //Toggle between viewing the days of the week when the repeat checkbox is selected
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            sun_button.Visibility = Visibility.Visible;
            mon_button.Visibility = Visibility.Visible;
            tues_button.Visibility = Visibility.Visible;
            wed_button.Visibility = Visibility.Visible;
            thurs_button.Visibility = Visibility.Visible;
            fri_button.Visibility = Visibility.Visible;
            sat_button.Visibility = Visibility.Visible;
        }
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            sun_button.Visibility = Visibility.Hidden;
            mon_button.Visibility = Visibility.Hidden;
            tues_button.Visibility = Visibility.Hidden;
            wed_button.Visibility = Visibility.Hidden;
            thurs_button.Visibility = Visibility.Hidden;
            fri_button.Visibility = Visibility.Hidden;
            sat_button.Visibility = Visibility.Hidden;
        }

    }

}