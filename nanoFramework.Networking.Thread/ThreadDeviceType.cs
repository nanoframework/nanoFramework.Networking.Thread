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
    public enum ThreadDeviceType
    {
        /// <summary>
        /// The device is a powered End Device.  
        /// </summary>
        EndDevice,

        /// <summary>
        /// The device is a battery power end Device. With this type of end device it will poll its router for new information.
        /// The device can go to sleep.
        /// </summary>
        SleepyEndDevice,

        /// <summary>
        /// The device is a powered device which can take on the role of End Device, Router or Leader. 
        /// When first connecting to network it will start as a end device then get promoted to Router or Leader.
        /// </summary>
        Router,
    }
}
