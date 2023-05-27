/**************************************************************************
*                                                                         *
*  File:        Logger.cs                                                 *
*  Copyright:   (c) Onofrei Grigore                                       *
*               @Kakerou_CLUB                                             *
*  Description: Module to record events and output them as scripts.       *
*                                                                         *
**************************************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using InputListener;

namespace Recorder
{
    public class Logger
    {
        private static string _stopKey = "q";

        /// <summary>
        /// Key used to stop a running script (default: 'q')
        /// </summary>
        public static string StopKey 
        { 
            get { return _stopKey; }
            set { _stopKey = value; } 
        }

        /// <summary>
        /// This flag is used to know when to stop recording
        /// </summary>
        private bool _stop = false;

        /// <summary>
        /// Mutable list of writers
        /// </summary>
        private List<IWriter> _writers = new List<IWriter>();

        /// <summary>
        /// Queue for listening events when recording
        /// </summary>
        private ConcurrentQueue<LLEventData> _eventQueue = new ConcurrentQueue<LLEventData>();

        //private Key[] specialKeys = { Key.LWin, Key.RWin, Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift, Key.LeftAlt, Key.RightAlt };

        public Logger(ConcurrentQueue<LLEventData> eventQueue)
        {
            this._eventQueue = eventQueue;
        }

        /// <summary>
        /// Destructor for properly closing writers
        /// </summary>
        ~Logger()
        {
            _writers.Clear();
        }

        public void AddWriter(IWriter writer)
        {
            _writers.Add(writer);
        }

        public void RemoveWriter(IWriter writer)
        {
            _writers.Remove(writer);
        }

        public void RemoveWriter(int index)
        {
            _writers.RemoveAt(index);
        }

        public void ClearWriters()
        {
            foreach (IWriter writer in _writers)
                writer.Close();

            _writers.Clear();
        }

        private void SendToWriters(string command)
        {
            foreach (IWriter w in _writers)
            {
                w.Write(command);
            }
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

                    SendToWriters(cmd);

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

                    SendToWriters(cmd);

                    break;
                case 2://special keys combination
                    cmd = "SendInput(";

                    foreach(uint k in action.cEvent.modifers)
                    {
                        cmd += (VirtualKeys)k + " ";
                    }

                    //if the last key is a digit(not from numpad)
                    if(action.cEvent.key.vkCode >= 48 && action.cEvent.key.vkCode <= 57)
                    {
                        cmd += (action.cEvent.key.vkCode - 48) + ")";
                    }
                    else
                    {
                        cmd += action.cEvent.key.uChar + ")";
                    }

                    //write the same command how many times last key was pressed
                    for(int i = 0; i < action.cEvent.key.count; i++)
                    {
                        SendToWriters(cmd);
                    }

                    break;
            }
        }

        public void StartRecording()
        {
            //while stop flag is set to false
            while(!_stop)
            {
                LLEventData action;
                    
                if(_eventQueue.TryDequeue(out action))
                {
                    Record(action);
                }
            }

            ClearWriters();
        }

        public void StopRecording()
        {
            _stop = true;
        }
    }
}
