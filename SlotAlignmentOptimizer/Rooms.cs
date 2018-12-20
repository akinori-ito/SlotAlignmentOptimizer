using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SlotAlignmentOptimizer
{
    public class Eventpair
    {
        int[] room;
        int[] slot;
        Rooms rooms;
        public Eventpair(Rooms rs)
        {
            this.rooms = rs;
            room = new int[2];
            slot = new int[2];
            do
            {
                do
                {
                    room[0] = rs.rand_room();
                    room[1] = rs.rand_room();
                } while (!rooms.rooms[room[0]].changable || 
                         !rooms.rooms[room[1]].changable);
                slot[0] = rs.rand_slot();
                slot[1] = rs.rand_slot();
            } while (room[0] == room[1] && slot[0] == slot[1]);
        }
        public Event GetEvent(int i)
        {
            return rooms.rooms[room[i]].Get(slot[i]);
        }
        public void swap()
        {
            Event e0 = rooms.rooms[room[0]].Get(slot[0]);
            Event e1 = rooms.rooms[room[1]].Get(slot[1]);
            rooms.rooms[room[0]].Set(slot[0], e1);
            rooms.rooms[room[1]].Set(slot[1], e0);
        }
    }

    // 複数の部屋のクラス
    // 同じ時間スロットで複数の部屋でイベントが開催される
    public class Rooms
    {
        public List<Room> rooms;
        int max_events;
        Random rand = new Random();
        const double OVERLAP_PENALTY = 10;
        const double CHANGE_PENALTY = 0.01;
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
        public Room AddRoom(string name)
        {
            var r = new Room(name, max_events);
            rooms.Add(r);
            return r;
        }
        public int rand_room()
        {
            return rand.Next(rooms.Count);
        }
        public int rand_slot()
        {
            return rand.Next(max_events);
        }
        public override string ToString()
        {
            string str = "<rooms>";
            foreach (var r in rooms)
            {
                str += r.ToString();
            }
            return str + "</rooms>";
        }
        // とりあえず書いておく
        public void anneal(double inittemp, int Nepoch, int Niter)
        {
            double loss = this.loss();
            double temp = inittemp;
            for (int j = 0; j < Nepoch; j++)
            {
                for (int i = 0; i < Niter; i++)
                {
                    Eventpair p = new Eventpair(this);
                    p.swap();
                    double newloss = this.loss();
                    if (newloss < loss)
                    {
                        loss = newloss;
                    }
                    else
                    {
                        double prob = Math.Exp((loss - newloss) / temp);
                        if (rand.NextDouble() < prob)
                        {
                            loss = newloss;
                        }
                        else
                        {
                            p.swap();
                        }
                    }
                }
                temp /= 2;
            }
        }

    }
}
