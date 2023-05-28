using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InputListener;
using Interpreter;
using Recorder;
using IntermediaryFacade;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace KeyloggerUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        #region Listener tests
        [TestMethod]
        //[Ignore]
        public void ListenerTestMethod()
        {
            //listener to be tested
            ConcurrentQueue<LLEventData> queue = new ConcurrentQueue<LLEventData>();
            LLListener listener = new LLListener(queue);

            //interpreter for generating an event
            ScriptInterpreter interpreter = new ScriptInterpreter();
            List<string> hots;

            //thread for event listening
            LLEventData dd;
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (!queue.TryDequeue(out dd)){}
            }));
            t.Start();

            listener.StartListening();
            t.Join();

            //interpreter.Parse("Ctrl J::\n{\nSend(\"a\")\n}",out hots);
            //interpreter.Run("Ctrl J");
            listener.StopListening();
            //Application.Exit();

            LLEventData testAction = new LLEventData
            {
                eType = 0,
                kEvent = new KeyEventData
                {
                    count = 1,
                    uChar = "a",
                    vkCode = (uint)VirtualKeys.A
                }
            };

            LLEventData action;
            if(queue.TryDequeue(out action))
            {
                Assert.AreEqual(action, testAction);
            }
            else
            {
                Assert.Fail("Didn't receive event!");
            }
        }
        #endregion

        #region Recorder tests
        public class StringWriter : IWriter
        {
            private StringBuilder _output;

            public StringWriter(StringBuilder output)
            {
                _output = output;
                _output.Append("{");
            }

            void IWriter.Close()
            {
                _output.Append("}");
                Console.Write("sdfsdf");
            }

            void IWriter.Write(string value)
            {
                _output.Append(value);
            }
        }

        [TestMethod]
        public void LoggerWritersTest()
        {
            ConcurrentQueue<LLEventData> q = new ConcurrentQueue<LLEventData>();
            Logger logger = new Logger(q);

            StringBuilder test = new StringBuilder();
            StringWriter writer = new StringWriter(test);
            logger.AddWriter(writer);

            LLEventData action = new LLEventData
            {
                eType = 0,
                kEvent = new KeyEventData
                {
                    count = 1,
                    uChar = "a",
                    vkCode = (uint)VirtualKeys.A
                }
            };

            q.Enqueue(action);

            Task log = Task.Factory.StartNew(() =>
            {
                logger.StartRecording();
            });

            Thread.Sleep(2000);
            logger.StopRecording();

            log.Wait();
            Assert.AreEqual("{Send(\"a\")}", test.ToString());
        }
        #endregion

        #region Interpreter Parse Tests

        [TestMethod] 
        public void ParseTest_Good()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Tests\\test_good.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTest_BadSyntax()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Tests\\test_bad_syntax.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_UnrecognizedCommand()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Tests\\test_unrecognized_command.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_MalformedLine()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Tests\\test_malformed_line.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_BadUsage()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Tests\\test_bad_usage.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }


        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_BadButton()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Tests\\test_bad_button.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_BadKey()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Tests\\test_bad_key.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }
        #endregion
    }
}
          
