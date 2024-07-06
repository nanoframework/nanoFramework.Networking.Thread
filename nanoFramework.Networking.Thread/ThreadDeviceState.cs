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
        Start = 0,

        /// <summary>
        /// Thread network stack has stopped.
        /// </summary>
        Stop = 1,

        /// <summary>
        /// Thread detached from any mesh network.
        /// </summary>
        Detached = 2,

        /// <summary>
        /// THread attached to mesh network.
        /// </summary>
        Attached = 3,

        /// <summary>
        /// Network interface is up.
        /// </summary>
        InterfaceUp = 5,

        /// <summary>
        /// Network interface is down.
        /// </summary>
        InterfaceDown = 6,

        /// <summary>
        /// Network interface received IPV6 address.
        /// </summary>
        GotIpv6 = 7,
    }
}
