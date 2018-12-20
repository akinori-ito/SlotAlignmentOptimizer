﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SlotAlignmentOptimizer
{
    // 部屋のクラス
    // 部屋は複数の時間スロットからなり、１つのスロットはイベントに対応する
    public class Room
    {
        public string name { get; private set; }
        public Event[] events { get; private set; }
        public int max_events { get; private set; }
        int tail;
        bool changable;
        public Room(string name, int max)
        {
            this.name = name;
            max_events = max;
            events = new Event[max];
            for (int i = 0; i < max; i++)
            {
                events[i] = Event.nullEvent;
            }
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
        public Event Get(int i)
        {
            if (i >= max_events || events[i] == null)
            {
                return Event.nullEvent;
            }
            return events[i];
        }
        public void Set(int i, Event e)
        {
            events[i] = e;
        }
        public void unchangable()
        {
            changable = false;
        }
    }
}
