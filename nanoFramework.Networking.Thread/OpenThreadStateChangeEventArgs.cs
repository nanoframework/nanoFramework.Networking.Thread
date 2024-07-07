//
//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Runtime.Events;

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// This class represents the event arguments for OnStatusChanged.
    /// </summary>
    public class OpenThreadStateChangeEventArgs : BaseEvent
    {
        private int _currentState;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenThreadStateChangeEventArgs() : base() 
        {
        }

        /// <summary>
        /// Current state of OpenThread stack.
        /// </summary>
        public int currentState { get => _currentState; set => _currentState = value; }
    }
}
