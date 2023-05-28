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
        private bool _batching = false;

        /// <summary>
        /// Script stop key
        /// </summary>
        private static string _stopKey = "Ctrl q";

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

        /// <summary>
        /// Adds a new writer and write first lines to it
        /// </summary>
        /// <param name="writer">The new writer</param>
        public void AddWriter(IWriter writer)
        {
            _writers.Add(writer);

            writer.Write(StopKey + "::\r\n{\r\n");
        }

        /// <summary>
        /// Close and remove writer if exists
        /// </summary>
        /// <param name="writer">Writer to be romoved</param>
        public void RemoveWriter(IWriter writer)
        {
            writer.Close();
            _writers.Remove(writer);
        }

        /// <summary>
        /// Close and remove writer if exists
        /// </summary>
        /// <param name="index">Index of writer</param>
        public void RemoveWriter(int index)
        {
            try 
            {
                IWriter writer = _writers[index];
                writer.Close();
                _writers.Remove(writer);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Writes an "}" at the end of outputs and close writers
        /// </summary>
        public void ClearWriters()
        {
            foreach (IWriter writer in _writers)
            {
                writer.Write("}\r\n");
                writer.Close();
            }

            _writers.Clear();
        }

        /// <summary>
        /// Sent to writers a command
        /// </summary>
        /// <param name="command">Command to be written</param>
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
                    //if was presed a modifier alone
                    if(action.kEvent.uChar.Equals(""))
                    {
                        if (_batching)
                        {
                            SendToWriters("\")\r\n");
                            _batching = false;
                        }

                        cmd = "\tSendInput(\"";
                        string modifier = ((VirtualKeys)action.kEvent.vkCode).ToString();
                        if (modifier.StartsWith("Left"))
                        {
                            cmd += modifier.TrimStart("Left".ToCharArray());
                        }
                        else if (modifier.StartsWith("Right"))
                        {
                            cmd += modifier.TrimStart("Right".ToCharArray());
                        }
                        else
                        {
                            cmd += modifier;
                        }

                        SendToWriters(cmd + "\")\r\n");
                        break;
                    }

                    //if Enter was pressed
                    if(action.kEvent.vkCode == (uint)VirtualKeys.Enter)
                    {
                        if (_batching)
                        {
                            SendToWriters("\")\r\n");
                            _batching = false;
                        }

                        SendToWriters("\tSendInput(\"" + VirtualKeys.Enter + "\")\r\n");
                        break;
                    }

                    //if is batching append new keys
                    if(_batching)
                    {
                        for (int i = 0; i < action.kEvent.count; i++)
                        {
                            SendToWriters(action.kEvent.uChar);
                        }
                    }
                    else//start batching
                    {
                        cmd = "\tSend(\"";
                        for (int i = 0; i < action.kEvent.count; i++)
                        {
                            cmd += action.kEvent.uChar;
                        }
                        SendToWriters(cmd);

                        _batching = true;
                    }
                    
                    break;
                case 1:// mouse event
                    //end Send command, if there is one started, before loggin other event type
                    if (_batching)
                    {
                        SendToWriters("\")\r\n");
                        _batching = false;
                    }

                    int x = action.mEvent.coords.X, y = action.mEvent.coords.Y;

                    if (action.mEvent.status)
                    {
                        cmd = "\tMouseRelease(" + x + ", " + y + ", " + (++action.mEvent.buttonID) + ")\r\n";
                    }
                    else
                    {
                        cmd = "\tMousePress(" + x + ", " + y + ", " + (++action.mEvent.buttonID) + ")\r\n";
                    }

                    SendToWriters(cmd);

                    break;
                case 2://special keys combination
                    //end Send command, if there is one started, before loggin other event type
                    if (_batching)
                    {
                        SendToWriters("\")\r\n");
                        _batching = false;
                    }

                    cmd = "\tSendInput(\"";

                    foreach(uint k in action.cEvent.modifers)
                    {
                        string modifier = (VirtualKeys)k + " ";
                        if(modifier.StartsWith("Left"))
                        {
                            cmd += modifier.TrimStart("Left".ToCharArray());
                        }
                        else if(modifier.StartsWith("Right"))
                        {
                            cmd += modifier.TrimStart("Right".ToCharArray());
                        }
                        else
                        {
                            cmd += modifier;
                        }
                    }

                    //if the last key is a digit(not from numpad)
                    if(action.cEvent.key.vkCode >= 48 && action.cEvent.key.vkCode <= 57)
                    {
                        cmd += (action.cEvent.key.vkCode - 48) + "\")\r\n";
                    }
                    else
                    {
                        cmd += (VirtualKeys)action.cEvent.key.vkCode + "\")\r\n";
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
            _stop = false;

            //while stop flag is set to false
            while(!_stop)
            {
                LLEventData action;
                    
                if(_eventQueue.TryDequeue(out action))
                {
                    Record(action);
                }
            }

            //end Send command, if there is one started, before finishing script
            if (_batching)
            {
                SendToWriters("\")\r\n");
                _batching = false;
            }

            ClearWriters();
        }

        public void StopRecording()
        {
            _stop = true;
        }
    }
}
