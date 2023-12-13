/*************************************************************************
*                                                                        *
*  File:        LogFace.cs                                               *
*  Copyright:   (c)Paniș Alexandru                                       *
*               @Kakerou_CLUB                                            *
*  Description: A facade pattern that encapsulate the logic on how       * 
*               the rest of modules are working together                 *
*                                                                        *
**************************************************************************/



using InputListener;
using Recorder;
using Interpreter;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Threading;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace IntermediaryFacade
{
    /// <summary>
    /// A Facade pattern over independent modules to encapsulate how they operate together
    /// </summary>
    public class LogFace
    {
        private LLListener _listener;
        private Logger _logger;
        private ScriptInterpreter _interpreter;
        private HotKeyListener _hkListener;

        public LogFace()
        {
            ConcurrentQueue<LLEventData> _llEventsQueue = new ConcurrentQueue<LLEventData>();
            //Binding the listener and logger to the same queue, as one is a producer and other a consumer
            _listener = new LLListener(_llEventsQueue);
            _logger = new Logger(_llEventsQueue);

            _interpreter = new ScriptInterpreter();
            _hkListener = new HotKeyListener();

            //Binding interpretor to consume the hotKeys events
            _hkListener.Subscribe(_interpreter);
        }



        private Thread _loggerThread = null;
        public void StartRecording()
        {

            _loggerThread = new Thread(new ThreadStart(delegate
            {
                _logger.StartRecording();
            }));

            _loggerThread.Start();

            _listener.StartListening();
        }

        public void StopRecording()
        {
            _listener.StopListening();

            _logger.StopRecording();

            //Wait the thread to stop
            _loggerThread.Join();
        }

        /// <summary>
        /// Set where the output will be written
        /// </summary>
        /// <param name="output"></param>
        public void setOutput(IWriter output)
        {
            _logger.AddWriter(output);
        }


        public void StartRunning(string script)
        {
            try
            {
                List<string> hotKeys = new List<string>();
                _interpreter.Parse(script, out hotKeys);

                //Register the recieved hotKeys from the interpreter
                hotKeys.ForEach(hKey =>
                {
                    _hkListener.Register(hKey);
                });

                _hkListener.StartListening();

            }
            catch (Exception ex)
            {
                //pass it to the heavens
                throw ex;
            }
        }

        public void StopRunning()
        {
            _hkListener.StopListening();
            _hkListener.Clear();
            _interpreter.Clear();
        }

    }
}
