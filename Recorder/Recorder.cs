/**************************************************************************
*                                                                         *
*  File:        Recorder.cs                                               *
*  Copyright:   (c) Onofrei Grigore                                       *
*               @Kakerou_CLUB                                             *
*  Description: Module to record events and output them as scripts.       *
*                                                                         *
**************************************************************************/

using System;
using System.Collections.Generic;
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

        //private Key[] specialKeys = { Key.LWin, Key.RWin, Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift, Key.LeftAlt, Key.RightAlt };

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
        /// <param name="action"> Action to be recorded </param>
        public void Record(LLEventData action)
        {
            switch(action.eType)
            {
                case 0:
                    foreach(IWriter w in writers)
                    {
                        string cmd = "Send(\"";
                        for(int i = 0; i < action.kEvent.count; i++)
                        {
                            cmd += action.kEvent.uChar;
                        }
                        cmd += "\")\r\n";
                        w.Write(cmd);
                    }
                    break;
                case 1:
                    foreach(IWriter w in writers)
                    {
                        int x = action.mEvent.coords.X, y = action.mEvent.coords.Y;
                        if (action.mEvent.status)
                        {
                            w.Write("ClickRelease(\"" + x + ", " + y + ", " + action.mEvent.buttonID + "\")\r\n");
                        }
                        else
                        {
                            w.Write("ClickPress(\"" + x + ", " + y + ", " + action.mEvent.buttonID + "\")\r\n");
                        }
                    }
                    break;
                case 2:
                    throw new NotImplementedException();
                    break;
            }
        }
    }
}
