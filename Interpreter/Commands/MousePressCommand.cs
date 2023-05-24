using System;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using InputListener;

namespace Interpreter
{
    /// <summary>
    ///     Command that presses the given mouse button at given location.
    ///     <para>
    ///         Implemented mouse buttons: 
    ///             <list type="bullet">
    ///                 <item>1: Left mouse button</item>
    ///                 <item>2: Right mouse button</item>
    ///             </list>
    ///     </para>
    /// </summary>
    /// <remarks> The button MUST be in a released state. </remarks>
    ///
    internal class MousePressCommand : IKlppCommand
    {
        private readonly ushort _posX;
        private readonly ushort _posY;
        private readonly byte _mouseButton;

        /// <summary>
        /// Command that presses the given mouse button at given location.
        ///     <para>
        ///         Implemented mouse buttons: 
        ///             <list type="bullet">
        ///                 <item>1: Left mouse button</item>
        ///                 <item>2: Right mouse button</item>
        ///             </list>
        ///     </para>
        /// </summary>
        /// <param name="posX">X screen coordinate in pixels</param>
        /// <param name="posY">Y screen coordinate in pixels</param>
        /// <param name="mouseButton">Mouse button to be pressed. Implemented options: 
        ///     <list type="bullet">
        ///         <item>1: Left mouse button</item>
        ///         <item>2: Right mouse button</item>
        ///     </list>
        /// </param>
        public MousePressCommand(ushort posX, ushort posY, byte mouseButton)
        {
            _posX = posX;
            _posY = posY;

            _mouseButton = mouseButton;
        }

        public void Execute()
        {
            //TODO: press
            INPUT[] input = new INPUT[1];

            int mouseEvent = 0x0004;

            if (_mouseButton == 2)
                mouseEvent <<= 2;

            input[0] = new INPUT
            {
                type = InputType.INPUT_MOUSE,
                mi = new MOUSEINPUT
                {
                    dx = _posX,
                    dy = _posY,
                    mouseData = 0,
                    dwFlags = mouseEvent | 0x8000 //ABSOLUT
                }
            };

            LLInput.SendInput(1, input, Marshal.SizeOf(typeof(INPUT)));
            //throw new NotImplementedException();
        }
    }
}
