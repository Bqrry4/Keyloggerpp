using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InputListener;
using Interpreter;
using Recorder;
using Logface;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
            listener.StopListening();

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

        [TestMethod]
        public void TestTest()
        {
            Assert.IsTrue(true);
        }
    }
}
