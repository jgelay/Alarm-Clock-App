using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AlarmLibrary
{
    public class AlarmViewModel
    {
        private int createAlarmHour = 12;
        private int createAlarmMin = 0;
        private int createAlarmAMPM = 0;
        public LinkedList<Alarm> alarms = new LinkedList<Alarm>();
        public LinkedList<UserAlarmViewModel> uAlarms = new LinkedList<UserAlarmViewModel>();

        public AlarmViewModel()
        {

        }
        public void setAlarm_decMinutes(Label label)
        {
            //if there are 0 mins in the label clicking up will change it to 59 mins
            if (createAlarmMin == 0)
            {
                createAlarmMin = 59;
                label.Content = createAlarmMin.ToString();
            }
            //if the minutes are inbetween or equal to 0 and 9 then add an extra "0" to format it 
            else if (createAlarmMin >= 0 && createAlarmMin < 11)
            {
                createAlarmMin -= 1;
                label.Content = "0" + createAlarmMin.ToString();
            }
            //otherwise just update the current minutes and update the label 
            else
            {
                createAlarmMin -= 1;
                label.Content = createAlarmMin.ToString();
            }
        }

        public void setAlarm_incMinutes(Label label)
        {
            //if there are 59 mins in the label clicking up will change it to 00 mins
            if (createAlarmMin == 59)
            {
                createAlarmMin = 0;
                label.Content = "0" + createAlarmMin.ToString();
            }
            //if the minutes are inbetween or equal to 0 and 9 then add an extra "0" to format it 
            else if (createAlarmMin >= 0 && createAlarmMin < 9)
            {
                createAlarmMin += 1;
                label.Content = "0" + createAlarmMin.ToString();
            }
            //otherwise just update the current minutes and update the label 
            else
            {
                createAlarmMin += 1;
                label.Content = createAlarmMin.ToString();
            }
        }

        public void setAlarm_decHours(Label label)
        {
            //if there are 0 hours in the label clicking down will change it to 12 mins
            if (createAlarmHour == 0 || createAlarmHour == 1)
            {
                createAlarmHour = 12;
                label.Content = createAlarmHour.ToString();
            }

            //otherwise just update the current hours and update the label 
            else
            {
                createAlarmHour -= 1;
                label.Content = createAlarmHour.ToString();
            }
        }

        public void setAlarm_incHours(Label label)
        {
            //if there are 12 hours in the label clicking up will change it to 00 mins
            if (createAlarmHour == 12)
            {
                createAlarmHour = 1;
                label.Content = createAlarmHour.ToString();
            }

            //if the hours are inbetween or equal to 0 and 9 then add an extra "0" to format it 
            else
            {
                createAlarmHour += 1;
                label.Content = createAlarmHour.ToString();
            }
        }

        public void setAlarm_amORpm(Label label)
        {
            //if the button is on AM it will change to PM
            if (createAlarmAMPM == 0)
            {
                createAlarmAMPM = 1;
                label.Content = "PM";
            }

            //otherwise the button is on PM and it will change to AM
            else
            {
                createAlarmAMPM = 0;
                label.Content = "AM";
            }
        }
        public void createAlarm()
        {
            UserAlarmViewModel uAlarm = new AlarmLibrary.UserAlarmViewModel();
            Alarm alarm = new Alarm(createAlarmHour, createAlarmMin, createAlarmAMPM, "test", false);
            alarm.dismissed = false;

            uAlarms.AddLast(uAlarm);



        }

    }
}
