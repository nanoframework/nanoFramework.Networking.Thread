//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using nanoFramework.Runtime.Events;

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// This class represents the event arguments for OnRoleChanged.
    /// </summary>
    public class OpenThreadRoleChangeEventArgs : BaseEvent
    {
        private ThreadDeviceRole _previousRole;
        private ThreadDeviceRole _currentRole;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OpenThreadRoleChangeEventArgs() : base()
        {
        }

        /// <summary>
        /// The previous thread device role before current change.
        /// </summary>
        public ThreadDeviceRole previousRole { get => _previousRole; set => _previousRole = value; }

        /// <summary>
        /// Current role of device.
        /// </summary>
        public ThreadDeviceRole currentRole { get => _currentRole; set => _currentRole = value; }
    }
}
