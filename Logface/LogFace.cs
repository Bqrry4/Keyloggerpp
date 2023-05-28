using InputListener;
using Recorder;
using Interpreter;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.Threading;

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
        }

        private Thread loggerThread = null;
        public void StartRecording()
        {

            loggerThread = new Thread(new ThreadStart(() =>
            {
                _logger.StartRecording();
            }));

            loggerThread.Start();

            _listener.StartListening();

        }

        public void StopRecording()
        {
            _listener.StopListening();

            _logger.StopRecording();

            //Wait the thread to stop
            loggerThread.Join();

            Console.WriteLine("asf");
        }

        public void setOutput(IWriter output)
        {
            _logger.AddWriter(output);
        }

        public void StartRunning(string script)
        {
            try
            {
                List<string> hotKeys;
                _interpreter.Parse(script, out hotKeys);

                //Register the recieved hotKeys from the interpreter
                hotKeys.ForEach(hKey =>
                {
                    _hkListener.Register(hKey);
                });



            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public void StopRunning()
        {

        }

    }
}
