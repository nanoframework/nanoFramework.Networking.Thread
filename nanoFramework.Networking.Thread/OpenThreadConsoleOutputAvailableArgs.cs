//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Runtime.Events;

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// Event arguments for the OnConsoleOutputAvailable Event.
    /// </summary>
    public class OpenThreadConsoleOutputAvailableArgs : BaseEvent
    {
        private string[] _consoleLines;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenThreadConsoleOutputAvailableArgs() : base()
        {
            _consoleLines = new string[0];
        }

        /// <summary>
        /// Currently available console lines. 0 or more strings.
        /// </summary>
        public string[] consoleLines { get => _consoleLines; set => _consoleLines = value; }
    }
}
