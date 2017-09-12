using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alarm_Clock
{
    class AlarmRing
    {
        public event EventHandler<AlarmEventArgs> AlarmRings;

        protected virtual void OnAlarmRings(AlarmEventArgs e)
        {
            EventHandler<AlarmEventArgs> handler = AlarmRings;
            if(handler != null)
            {
                handler(this, e);
            }

        }

        public void compareTime(UserAlarm userAl, String timeStr, DateTime currDate)
        {
            if (currDate.ToString("h:mm tt") == timeStr && userAl.getAlarm().dismissed == false)
            {
                if (!this.getDays(userAl, currDate))
                {
                    if (!userAl.getAlarm().getRepeating())
                    {
                        AlarmEventArgs args = new AlarmEventArgs();
                        args.currAl = userAl;
                        OnAlarmRings(args);
                    }
                }
                else
                {
                    AlarmEventArgs args = new AlarmEventArgs();
                    args.currAl = userAl;
                    OnAlarmRings(args);
                }
              
            }
        }
        public bool getDays(UserAlarm userAl, DateTime currDate)
        {
            bool[] currDays = userAl.getAlarm().getDays();
            return currDays[(int)currDate.DayOfWeek];
        }
    }
}
