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
using System.Security.Cryptography;
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
        private void Record(LLEventData action)
        {
            string cmd;

            // create command string
            switch (action.eType)
            {
                case 0:// key event
                    cmd = "Send(\"";

                    for (int i = 0; i < action.kEvent.count; i++)
                    {
                        cmd += action.kEvent.uChar;
                    }

                    cmd += "\")";
                    break;
                case 1:// mouse event
                    int x = action.mEvent.coords.X, y = action.mEvent.coords.Y;

                    if (action.mEvent.status)
                    {
                        cmd = "ClickRelease(\"" + x + ", " + y + ", " + action.mEvent.buttonID + "\")";
                    }
                    else
                    {
                        cmd = "ClickPress(\"" + x + ", " + y + ", " + action.mEvent.buttonID + "\")";
                    }
                    break;
                case 2:
                    cmd = "SendInput(";

                    foreach(uint k in action.cEvent.modifers)
                    {
                        cmd += (VirtualKeys)k + " ";
                    }

                    cmd += action.cEvent.key.uChar + ")";
                    break;
                default:
                    cmd = null;
                    break;
            }

            // write in every writer if created
            if(cmd != null)
            {
                foreach (IWriter w in writers)
                {
                    w.Write(cmd);
                }
            }
        }

        public void StartRecording()
        {

        }
    }
}
