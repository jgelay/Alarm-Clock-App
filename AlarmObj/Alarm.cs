using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlarmObj
{
    [Serializable]
    public class Alarm : ISerializable
    {
        public int id { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int ampm { get; set; }
        public int origHour { get; set; }
        public int origMinute { get; set; }
        public int origAmpm { get; set; }
        public bool dismissed { get; set; }
        public bool repeating { get; set; }
        public string descript { get; set; }
        public bool snooze { get; set; }
        public int rang { get; set; }
        public string ringerPath { get; set; }

        private bool[] _days = { false, false, false, false, false, false, false };

        public bool[] days
        {
            get
            {
                return _days;
            }
            set
            {
                _days = value;
            }
        }
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
        }

        public String getString()
        {

            String tempMin = minute.ToString();
            if (minute < 10)
            {
                tempMin = "0" + tempMin;
            }

            String temp = this.hour + ":" + tempMin;

            if (this.ampm == 0)
            {
                temp += " AM";
            }
            else
            {
                temp += " PM";
            }
            return temp;
        }
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
        }
    }
}
