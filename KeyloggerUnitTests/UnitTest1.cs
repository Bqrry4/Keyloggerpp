using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InputListener;
using Interpreter;
using Recorder;
using Logface;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace KeyloggerUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ListenerTestMethod()
        {
            //listener to be tested
            ConcurrentQueue<LLEventData> queue = new ConcurrentQueue<LLEventData>();
            LLListener listener = new LLListener(queue);

            //interpreter for generating an event
            ScriptInterpreter interpreter = new ScriptInterpreter();
            List<string> hots;

            listener.StartListening();
            interpreter.Parse("Ctrl J::\n{\nSend(\"a\")\n}",out hots);
            interpreter.Run(hots[0]);
            listener.StopListening();

            /*LLEventData testAction = new LLEventData
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
            }*/
        }

        [TestMethod]
        public void TestTest()
        {
        }

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
