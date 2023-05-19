using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using InputListener;

namespace Recorder
{
    public class Recorder
    {
        private static string stopKey = "q";

        /// <summary>
        /// Key used to stop a running script (default: 'q')
        /// </summary>
        public static string StopKey 
        { 
            get { return stopKey; }
            set { stopKey = value; } 
        }

        /// <summary>
        /// Mutable list of writers
        /// </summary>
        private List<IWriter> writers = new List<IWriter>();

        /// <summary>
        /// Queue for listening events when recording
        /// </summary>
        private Queue<LLEventData> eventQueue = new Queue<LLEventData>();

        private Key[] specialKeys = { Key.LWin, Key.RWin, Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift, Key.LeftAlt, Key.RightAlt };

        public Recorder(Queue<LLEventData> eventQueue)
        {
            this.eventQueue = eventQueue;
        }

        /// <summary>
        /// Destructor for properly closing writers
        /// </summary>
        ~Recorder()
        {
            writers.Clear();
        }

        public void AddWriter(IWriter writer)
        {
            writers.Add(writer);
        }

        public void RemoveWriter(IWriter writer)
        {
            writers.Remove(writer);
        }

        public void RemoveWriter(int index)
        {
            writers.RemoveAt(index);
        }

        public void ClearWriters()
        {
            writers.Clear();
        }

        /// <summary>
        /// Record an action to be written in every writer's destination
        /// </summary>
        /// <param name="action">Action to be recorded</param>
        public void Record(LLEventData action)
        {
            switch(action.type)
            {
                case 0:
                    KeyEventData keyAction = (KeyEventData)action;
                    foreach(IWriter w in writers)
                    {
                        if(Array.IndexOf(specialKeys, keyAction.key) == -1)
                        {
                            w.Write("\tSendInput(" + keyAction.key.ToString() + ")\n");
                        }
                    }
                    break;
                case 1:
                    MouseEventData mouseAction = (MouseEventData)action;
                    foreach(IWriter w in writers)
                    {
                        //w.Write("\tClick(" + mouseAction.choords.x);
                    }
                    break;
                case 2:

                    break;
            }
        }
    }
}
