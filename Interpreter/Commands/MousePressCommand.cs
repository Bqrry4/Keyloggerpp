using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //TODO: RELEASE
            throw new NotImplementedException();
        }
    }
}
