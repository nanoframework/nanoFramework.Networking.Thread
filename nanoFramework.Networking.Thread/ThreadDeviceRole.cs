//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// Role of current device.
    /// </summary>
    public enum ThreadDeviceRole
    {
        /// <summary>
        /// Thread stack not started.
        /// </summary>
        Disabled,

        /// <summary>
        /// Thread stack started but not connected to an existing mesh network. 
        /// </summary>
        Detached,

        /// <summary>
        /// Thread stack started and connected to an existing mesh network as a child/End device. 
        /// </summary>
        Child,

        /// <summary>
        /// Thread stack started and connected to an existing mesh network as a router. 
        /// </summary>
        Router,

        /// <summary>
        /// Thread stack started and connected to an existing mesh network as a leader. 
        /// </summary>
        Leader
    }
}
