//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Text;

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// Represents a OpenThread Dataset.
    /// </summary>
    public class OpenThreadDataset
    {
        private UInt16 _channel;
        private UInt16 _panId;
        private byte[] _networkName = new byte[16];
        private byte[] _extendedPanId = new byte[8];
        private byte[] _networkKey = new byte[16];
        private byte[] _pskc = new byte[16];

        /// <summary>
        /// Used to flags properties in use.
        /// </summary>
        [Flags]
        private enum componentInUseFlags
        {
            None = 0,
            channel = 1,
            panId = 2,
            networkName = 4,
            extendedPanId = 8,
            networkKey = 16,
            pskc = 32
        };

        private componentInUseFlags _inUseFlags = componentInUseFlags.None;

        /// <summary>
        /// Construct an empty Thread Dataset.
        /// </summary>
        public OpenThreadDataset()
        {
        }

        /// <summary>
        /// Get or set the Thread Network Key.
        /// Must be a byte[8].
        /// </summary>
        /// <remarks>
        /// This is the only data required to connect to an existing network. Although having channel will
        /// speed up connection. Must be 8 bytes length.
        /// </remarks>
        public byte[] NetworkKey
        {
            get => _networkKey;
            set
            {
                if (value.Length != _networkKey.Length)
                {
                    throw new ArgumentException();
                }

                _networkKey = value;
                _inUseFlags |= componentInUseFlags.networkKey;
            }
        }

        /// <summary>
        /// Get or Set the PAN ID.
        /// </summary>
        public UInt16 PanId
        {
            get => _panId;
            set
            {
                _panId = value;
                _inUseFlags |= componentInUseFlags.panId;
            }
        }

        /// <summary>
        /// Get or Set the Extended PAN ID.
        /// Must be a byte[8].
        /// </summary>
        public byte[] ExtendedPanId
        {
            get => _extendedPanId;
            set
            {
                if (value.Length != _extendedPanId.Length)
                {
                    throw new ArgumentException();
                }

                _extendedPanId = value;
                _inUseFlags |= componentInUseFlags.extendedPanId;
            }
        }

        /// <summary>
        /// Get or Set the PSKc. The pre-shared key required for commissioner.
        /// Must be a byte[16].
        /// </summary>
        public byte[] PSKc
        {
            get => _pskc;
            set
            {
                if (value.Length != _pskc.Length)
                {
                    throw new ArgumentException();
                }

                _pskc = value;
                _inUseFlags |= componentInUseFlags.pskc;
            }
        }

        /// <summary>
        /// Get or Set the Channel.
        /// </summary>
        public UInt16 Channel
        {
            get => _channel;
            set
            {
                _channel = value;
                _inUseFlags |= componentInUseFlags.channel;
            }
        }

        /// <summary>
        /// Get or Set Network Name. 
        /// Must be less than or equal to 16 characters.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Network name is longer then 16 characters.</exception>
        public string NetworkName
        {
            get
            {
                return Encoding.UTF8.GetString(_networkName, 0, _networkName.Length);
            }
            set
            {
                byte[] name = Encoding.UTF8.GetBytes(value);
                if (name.Length > 16)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _networkName = name;
                _inUseFlags |= componentInUseFlags.networkName;
            }
        }
    }
}
