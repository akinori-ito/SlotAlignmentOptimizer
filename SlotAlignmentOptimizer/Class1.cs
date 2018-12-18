using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotAlignmentOptimizer
{
    // 参加者のクラス
    public class Attendee
    {
        string name;
        int id;
        static int seq = 0;
        public Attendee(string name)
        {
            this.name = name;
            this.id = seq;
            seq++;
        }
        public string getName()
        {
            return name;
        }
        public int getId()
        {
            return id;
        }
        public bool equals(Attendee a)
        {
            return id == a.id;
        }
    }
    // イベントのクラス
    // イベントはイベント名と複数の参加者からなる
    public class Event
    {
        public string name;
        public List<Attendee> attendees;
        public static Event nullEvent = null;
        public Event(string name)
        {
            this.name = name;
            attendees = new List<Attendee>();
            if (nullEvent == null)
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
    }
    // 部屋のクラス
    // 部屋は複数の時間スロットからなり、１つのスロットはイベントに対応する
    public class Room
    {
        public string name;
        public Event[] events;
        public int max_events;
        int tail;
        bool changable;
        public Room(string name, int max)
        {
            this.name = name;
            max_events = max;
            events = new Event[max];
            tail = 0;
            changable = true;
        }
        public void addEvent(Event e)
        {
            if (tail == max_events)
            {
                throw new Exception();
            }
            events[tail] = e;
            tail++;
        }
        public Event get(int i)
        {
            if (i >= max_events || events[i] == null)
            {
                return Event.nullEvent;
            }
            return events[i];
        }
        public void unchangable()
        {
            changable = false;
        }
    }
    // 複数の部屋のクラス
    // 同じ時間スロットで複数の部屋でイベントが開催される
    public class Rooms
    {
        public List<Room> rooms;
        int max_events;
        const double OVERLAP_PENALTY = 10000;
        const double CHANGE_PENALTY = 10;
        public Rooms(int max)
        {
            max_events = max;
            rooms = new List<Room>();
        }
        // 同時刻に開催されるイベントで参加者が重なっている数 * OVERLAP_PENALTY をペナルティとして返す
        double exist_constraint(int t, int i, int j)
        {
            Event event1 = rooms[i].get(t);
            Event event2 = rooms[j].get(t);
            return OVERLAP_PENALTY * event1.overlap(event2);
        }
        // 連続する2イベントで異なる参加者数 * CHANGE_PENALTY をペナルティとして返す
        double change_constraint(int t, int i)
        {
            if (t == max_events-1)
            {
                return 0;
            }
            Room room = rooms[i];
            int overlap = room.get(t).overlap(room.get(t + 1));
            int changenum = room.get(t).numberOfAttendees() + room.get(t + 1).numberOfAttendees() - overlap;
            return changenum * CHANGE_PENALTY;
        }
        public double loss()
        {
            double val = 0;
            for (int i = 0; i < rooms.Count-1; i++)
            {
                for (int t = 0; t < max_events; t++)
                {
                    for (int j = i + 1; j < rooms.Count; j++)
                    {
                        val += exist_constraint(t, i, j);
                    }
                    val += change_constraint(t, i);
                }
            }
            return val;
        }
    }
}
