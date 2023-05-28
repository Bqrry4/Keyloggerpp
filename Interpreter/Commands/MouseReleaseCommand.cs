/**************************************************************************
 *                                                                        *
 *  File:        MouseReleaseCommand.cs                                   *
 *  Copyright:   (c) Olăreț Radu                                          *
 *               @Kakerou_CLUB                                            *
 *  Description: Command that releases the given mouse button at given    *
 *               location.                                                *
 **************************************************************************/

using InputListener;
using System;
using System.Runtime.InteropServices;

namespace Interpreter
{
    /// <summary>
    ///     Command that releases the given mouse button at given location.
    ///     <para>
    ///         Implemented mouse buttons: 
    ///             <list type="bullet">
    ///                 <item>1: Left mouse button</item>
    ///                 <item>2: Right mouse button</item>
    ///             </list>
    ///     </para>
    /// </summary>
    /// <remarks> The button MUST be in a pressed state. </remarks>
    internal class MouseReleaseCommand : IKlppCommand
    {
        private readonly ushort _posX;
        private readonly ushort _posY;
        private readonly byte _mouseButton;

        /// <summary>
        ///     Command that releases the given mouse button at given location.
        ///     <para>
        ///         Implemented mouse buttons: 
        ///             <list type="bullet">
        ///                 <item>1: Left mouse button</item>
        ///                 <item>2: Right mouse button</item>
        ///             </list>
        ///     </para>
        /// </summary>
        /// <remarks> The button MUST be in a pressed state. </remarks>
        /// <param name="posX">X screen coordinate in pixels</param>
        /// <param name="posY">Y screen coordinate in pixels</param>
        /// <param name="mouseButton">Mouse button to be released. Implemented options:
        ///     <list type="bullet">
        ///         <item>1: Left mouse button</item>
        ///         <item>2: Right mouse button</item>
        ///     </list>
        /// </param>
        public MouseReleaseCommand(ushort posX, ushort posY, byte mouseButton)
        {
            int width = LLInput.GetSystemMetrics(0);
            int height = LLInput.GetSystemMetrics(1);
            _posX = (ushort)(((float)posX / width) * 65536);
            _posY = (ushort)(((float)posY / height) * 65536);
            _mouseButton = mouseButton;
        }

        /// <summary>
        /// Sends a mouse release event for the given button
        /// </summary>
        /// <exception cref="ArgumentException">When the command holds an invalid mouseButton value</exception>
        public void Execute()
        {
            INPUT[] input = new INPUT[1];

            int mouseEvent = 0x0004;

            if (_mouseButton != 1 && _mouseButton != 2)
            {
                throw new ArgumentException("Error executing MouseRelease: Invalid mouse button: " + _mouseButton);
            }

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
                    dwFlags = (uint)(mouseEvent | 0x8000 | 0x0001) //ABSOLUTE and MOVE
                }
            };

            if (LLInput.SendInput(1, input, Marshal.SizeOf(typeof(INPUT))) == 0)
            {
                throw new AggregateException("Error executing MouseRelease: SendInput failed, error code " + LLInput.GetLastError().ToString());
            }
        }
    }
}
