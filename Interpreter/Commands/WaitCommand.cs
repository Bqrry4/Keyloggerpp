using System.Threading;

namespace Interpreter.Commands
{
    /// <summary>
    /// Command that makes the execution wait for specified milliseconds
    /// </summary>
    internal class WaitCommand : IKlppCommand
    {
        private readonly uint _time;
        public WaitCommand(uint time)
        {
            _time = time;
        }
        public void Execute()
        {
            Thread.Sleep((int)_time);
        }
    }
}
