using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SlotAlignmentOptimizer
{
    // イベントのクラス
    // イベントはイベント名と複数の参加者からなる
    public class Event
    {
        public string name { get; private set;  }
        public List<Attendee> attendees { get; private set;  }
        public static Event nullEvent = null;
        public Event(string name)
        {
            this.name = name;
            attendees = new List<Attendee>();
            if (nullEvent == null && name != "<NULL>")
            {
              nullEvent = new Event("<NULL>");
            }
        }
        // ２つのイベントで参加者が何人オーバーラップしているか調べる
        public int overlap(Event e)
        {
            int ov = 0;
            foreach (Attendee a1 in attendees)
            {
                foreach (Attendee a2 in e.attendees)
                {
                    if (a1.equals(a2))
                        ov++;
                }
            }
            return ov;
        }
        public int numberOfAttendees()
        {
            return attendees.Count;
        }
        override public string ToString()
        {
            string s = "<event>";
            s = "<name>" + name + "</name><attendees>";
            foreach (Attendee a in attendees)
            {
                s += a.ToString();
            }
            return s + "</attendees></event>";
        }
    }
}
