using InputListener;
using Recorder;
using Interpreter;
using System.Collections.Concurrent;

namespace Logface
{
    /// <summary>
    /// A Facade pattern over independent modules to encapsulate how they operate together
    /// </summary>
    public class LogFace
    {
        private LLListener _listener;
        private Logger _logger;
        private ScriptInterpreter _interpreter;

        public LogFace()
        {
            ConcurrentQueue<LLEventData> _llEventsQueue = new ConcurrentQueue<LLEventData>();
            //Binding the listener and logger to the same queue, as one is a producer and other a consumer
            _listener = new LLListener(_llEventsQueue);
            _logger = new Logger(_llEventsQueue);

            _interpreter = new ScriptInterpreter();


        }
    }
}
