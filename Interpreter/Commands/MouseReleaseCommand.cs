using System;

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
            _posX = posX;
            _posY = posY;
            _mouseButton = mouseButton;
        }

        public void Execute()
        {
            //TODO: CLICK
            throw new NotImplementedException();
        }
    }
}
