//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Runtime.Events;

namespace nanoFramework.Networking.Thread
{
    internal class OpenThreadJoinerStartCompleteEventArgs : BaseEvent
    {
        private int _error;

        internal OpenThreadJoinerStartCompleteEventArgs() : base() 
        {
            _error = 0;
        }

        /// <summary>
        /// Joiner start error or 0 if successful.
        /// </summary>
        public int error { get => _error; set => _error = value; }
    }
}
