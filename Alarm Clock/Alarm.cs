using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Alarm_Clock
{
    [Serializable]
    public class Alarm : ISerializable
    {
        private int id;
        private int hour;
        private int minute;
        private int ampm;
        private int origHour;
        private int origMinute;
        private int origAmpm;
        public bool dismissed;
        private bool repeating;
        private String descript;

        //variables for snooze
        private bool snooze = false;
        private int rang = 0;

        // Arrray for which days to ring, STARTS ON SUNDAY
        // SUN MON TUES WED THURS SAT
        private bool[] days = { false, false, false, false, false, false, false };

        // path for the ringer sound file
        private String ringerPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

        // Linnking to user conrol (user alarm)
        private UserAlarm  userAlarm =  null;
        
        

        // Constructor initializes the time when loading from file
        protected Alarm(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new System.ArgumentNullException("info");
            this.id = (int)info.GetValue("AlarmID", typeof(int));
            this.hour = (int)info.GetValue("AlarmHour", typeof(int));
            this.minute = (int)info.GetValue("AlarmMin", typeof(int));
            this.ampm = (int)info.GetValue("AlarmAMPM", typeof(int));
            this.ringerPath = (string)info.GetValue("AlarmRingerPath", typeof(String));
            this.origHour = (int)info.GetValue("AlarmOriginalHour", typeof(int));
            this.origMinute = (int)info.GetValue("AlarmOriginalMin", typeof(int));
            this.origAmpm = (int)info.GetValue("AlarmOriginalAMPM", typeof(int));
            this.descript = (string)info.GetValue("AlarmDescription", typeof(string));
            this.repeating = (bool)info.GetValue("AlarmRepeat", typeof(bool));
            this.days = (bool[])info.GetValue("AlarmDays", typeof(bool[]));
            this.dismissed = (bool)info.GetValue("AlarmDismissed", typeof(bool));
        }
        public Alarm(int hour, int minute, int ampm, bool repeating, string words,bool[]days)
        {
            this.hour = hour;
            this.minute = minute;
            this.ampm = ampm;
            this.origHour = hour;
            this.origMinute = minute;
            this.origAmpm = ampm;
            this.repeating = repeating;

            this.descript = words;
            this.days = days;

        }


        // Constructor that initializes the time as well as days 
        public Alarm(int hour, int minute, int ampm, bool repeating, bool[] days)
        {
            this.hour = hour;
            this.minute = minute;
            this.ampm = ampm;
            this.origHour = hour;
            this.origMinute = minute;
            this.origAmpm = ampm;

            this.repeating = repeating;
            this.days = days;
        }


        // Constructor that initializes the time as well as days 
        public Alarm(int hour, int minute, int ampm, bool repeating, bool[] days, String ringerPath)
        {
            this.hour = hour;
            this.minute = minute;
            this.ampm = ampm;

            this.repeating = repeating;
            this.days = days;
            this.ringerPath = ringerPath;
        }


        // Getters
        public int getOrigHour()
        {
            return this.origHour;
        }
        public UnmanagedMemoryStream getActualRinger(String ringer)
        {
            UnmanagedMemoryStream ringerPath = null;
            if (ringer.Equals(@"Default.wav")){
                ringerPath = Alarm_Clock.Properties.Resources.Default;
            }

            else if (ringer.Equals(@"metal_crunch.wav"))
            {
                ringerPath = Alarm_Clock.Properties.Resources.metal_crunch;
            }

            else if (ringer.Equals(@"gun.wav"))
            {
                ringerPath = Alarm_Clock.Properties.Resources.gun;
            }

            else if (ringer.Equals(@"neon_light.wav"))
            {
                ringerPath = Alarm_Clock.Properties.Resources.neon_light;
            }

            else if (ringer.Equals(@"hammer_anvil2.wav"))
            {
                ringerPath = Alarm_Clock.Properties.Resources.hammer_anvil2;
            }

            else if (ringer.Equals(@"kiss_x.wav"))
            {
                ringerPath = Alarm_Clock.Properties.Resources.kiss_x;
            }

            else if (ringer.Equals(@"laser_x.wav"))
            {
                ringerPath = Alarm_Clock.Properties.Resources.laser_x;
            }

            return ringerPath;
        }
        public int getOrigMinute()
        {
            return this.origMinute;
        }
        public int getorigAmpm()
        {
            return this.origAmpm;
        }
        public int getRang()
        {
            return this.rang;
        }
        public bool getSnooze()
        {
            return this.snooze;
        }
        public int getID()
        {
            return id;
        }

        public int getHour()
        {
            return hour;
        }

        public String getName()
        {
            return descript;
        }

        public int getMin()
        {
            return minute;
        }

        public int getAMPM()
        {
            return ampm;
        }

        public bool getRepeating()
        {
            return repeating;
        }

        public bool[] getDays()
        {
            return days;
        }

        public string getDescription()
        {
            return this.descript;
        }
        public String getRingerPath()
        {
            return this.ringerPath;
        }

        public UserAlarm getUAlarm()
        {
            return userAlarm;
        }

        public void setName(String words)
        {
            this.descript = words;
        }

        public String getString()
        {

            String tempMin = minute.ToString();
            if (minute < 10)
            {
                tempMin = "0" + tempMin;
            }
           
            String temp = this.getHour() + ":" + tempMin;
               
            if (this.getAMPM() == 0)
            {
                temp += " AM";
            }
            else
            {
                temp += " PM";
            }
            return temp;
        }

        // Setters
        public void setOrigHour(int hour)
        {
             this.origHour = hour;
        }

        public void setDays(bool[] day)
        {
            this.days = day;
        }
        public void setOrigMinute(int mint)
        {
            this.origMinute = mint;
        }
        public void setorigAmpm(int ampm)
        {
            this.origAmpm  = ampm;
        }
        public void setSnooze(bool snooze)
        {
            this.snooze = snooze;
        }
        public void setUserAlarm(UserAlarm userAlarm)
        {
            this.userAlarm = userAlarm;
        }

        public void setID(int id)
        {
            this.id = id;
        }

        public void setRepeat(bool repe)
        {
            this.repeating = repe;
        }

        public void setHour(int hour)
        {
            this.hour = hour;
        }

        public void setMin(int mins)
        {
            this.minute = mins;
        }
        public void setAMPM(int amopm)
        {
            this.ampm = amopm;
        }
        //....................
        public void setRingerPath(String path)
        {
            this.ringerPath = path;
        }

        // This method deals with the serialization of each alarm object
        // it serializes all the fields needed for alarm.
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AlarmID", this.id);
            info.AddValue("AlarmHour", this.hour);
            info.AddValue("AlarmMin", this.minute);
            info.AddValue("AlarmAMPM", this.ampm);
            info.AddValue("AlarmRingerPath", this.ringerPath);
            info.AddValue("AlarmOriginalHour", this.origHour);
            info.AddValue("AlarmOriginalMin", this.origMinute);
            info.AddValue("AlarmOriginalAMPM", this.origAmpm);
            info.AddValue("AlarmDescription", this.descript);
            info.AddValue("AlarmRepeat", this.repeating);
            info.AddValue("AlarmDays", this.days);
            info.AddValue("AlarmDismissed", this.dismissed);
        }

        internal void setRingerPath(UnmanagedMemoryStream @default)
        {
            throw new NotImplementedException();
        }
    }
}
