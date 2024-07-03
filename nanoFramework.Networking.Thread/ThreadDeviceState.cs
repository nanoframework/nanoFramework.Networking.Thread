//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// Current OpenThread state.
    /// </summary>
    public enum ThreadDeviceState
    {
        /// <summary>
        /// Thread network stack has started.
        /// </summary>
        OPENTHREAD_EVENT_START = 0,
        /// <summary>
        /// Thread network stack has stopped.
        /// </summary>
        OPENTHREAD_EVENT_STOP = 1,
        /// <summary>
        /// Thread detached from any mesh network.
        /// </summary>
        OPENTHREAD_EVENT_DETACHED = 2,
        /// <summary>
        /// THread attached to mesh network.
        /// </summary>
        OPENTHREAD_EVENT_ATTACHED = 3,
        /// <summary>
        /// Network interface is up.
        /// </summary>
        OPENTHREAD_EVENT_IF_UP = 5,
        /// <summary>
        /// Network interface is down.
        /// </summary>
        OPENTHREAD_EVENT_IF_DOWN = 6,
        /// <summary>
        /// Network interface received IPV6 address.
        /// </summary>
        OPENTHREAD_EVENT_GOT_IP6 = 7,
    }
}
