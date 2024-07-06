//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// Thread Radio connection type.
    /// How the THread radio is connected to this device.
    /// </summary>
    public enum RadioConnection
    {
        /// <summary>
        /// Connected via a native radio which is part of SOC.
        /// </summary>
        Native,

        /// <summary>
        /// Connected via a UART port.
        /// </summary>
        Uart,

        /// <summary>
        /// Connected via a SPI bus.
        /// </summary>
        Spi
    };
}
