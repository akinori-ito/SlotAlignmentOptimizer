using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SlotAlignmentOptimizer
{
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
            Event event1 = rooms[i].Get(t);
            Event event2 = rooms[j].Get(t);
            return OVERLAP_PENALTY * event1.overlap(event2);
        }
        // 連続する2イベントで異なる参加者数 * CHANGE_PENALTY をペナルティとして返す
        double change_constraint(int t, int i)
        {
            if (t == max_events - 1)
            {
                return 0;
            }
            Room room = rooms[i];
            int overlap = room.Get(t).overlap(room.Get(t + 1));
            int changenum = room.Get(t).numberOfAttendees() + room.Get(t + 1).numberOfAttendees() - overlap;
            return changenum * CHANGE_PENALTY;
        }
        public double loss()
        {
            double val = 0;
            for (int i = 0; i < rooms.Count - 1; i++)
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
