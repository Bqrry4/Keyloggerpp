using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InputListener;
using Interpreter;
using Recorder;
using Logface;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

namespace KeyloggerUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        #region Interpreter Parse Tests

        [TestMethod] 
        public void ParseTest_Good()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Test Scripts\\test_good.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTest_BadSyntax()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Test Scripts\\test_bad_syntax.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_UnrecognizedCommand()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Test Scripts\\test_unrecognized_command.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_MalformedLine()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Test Scripts\\test_malformed_line.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_BadUsage()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Test Scripts\\test_bad_usage.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }


        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_BadButton()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Test Scripts\\test_bad_button.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void ParseTest_BadKey()
        {
            ScriptInterpreter interpreter = new ScriptInterpreter();
            StreamReader reader = new StreamReader("Interpreter Test Scripts\\test_bad_key.klpp");
            string script = reader.ReadToEnd();
            interpreter.Parse(script, out _);

            Assert.AreEqual(0, 0);
        }
        #endregion
    }
}
