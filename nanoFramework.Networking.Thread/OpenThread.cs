//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

// Ignore Spelling: nano

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Net;

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// OpenThread main class for constructing and starting the OpenThread mesh network.
    /// </summary>
    public class OpenThread : IDisposable
    {
        private bool _disposedValue;
        private ThreadDeviceRole _currentDeviceRole = ThreadDeviceRole.Disabled;
        private OpenThreadDataset _dataset;
        private bool _started = false;
        private ThreadDeviceType _threadDeviceType;
        private RadioConnection _radioType;
        private int _port;
        private int _speed;
        private AutoResetEvent _consoleOutputAvailable = new AutoResetEvent(false);
        private AutoResetEvent _joinerResultAvailable = new AutoResetEvent(false);
        private int _openThreadErrorCode = 0;
        private string[] _consoleCommandResult = new string[0];
        private bool _commandInProgress = false;

        internal static readonly OpenThreadEventListener _openThreadEventManager = new();

        /// <summary>
        /// Delegate for Status Changed events.
        /// </summary>
        /// <param name="sender">Sender OpenThread object.</param>
        /// <param name="args"></param>
        public delegate void OpenThreadStatusChangedEventHandler(OpenThread sender, OpenThreadStateChangeEventArgs args);

        /// <summary>
        /// Delegate for Role Changed events.
        /// </summary>
        /// <param name="sender">Sender OpenThread object.</param>
        /// <param name="args"></param>
        public delegate void OpenThreadRoleChangedEventHandler(OpenThread sender, OpenThreadRoleChangeEventArgs args);

        /// <summary>
        /// Delegate for Role Changed events.
        /// </summary>
        /// <param name="sender">Sender OpenThread object.</param>
        /// <param name="args"></param>
        public delegate void OpenThreadConsoleOutputAvailableEventHandler(OpenThread sender, OpenThreadConsoleOutputAvailableArgs args);

        /// <summary>
        /// This is an event that is triggered when the status of OpenThread changes.
        /// </summary>
        public event OpenThreadStatusChangedEventHandler OnStatusChanged;

        /// <summary>
        /// This is an event that is triggered when the device role changes.
        /// </summary>
        public event OpenThreadRoleChangedEventHandler OnRoleChanged;

        /// <summary>
        /// This is an event that is triggered when there is output available on the console as
        /// a result of a command or as an unsolicited message.
        /// </summary>
        public event OpenThreadConsoleOutputAvailableEventHandler OnConsoleOutputAvailable;

        /// <summary>
        /// Create an Open Thread stack using radio connection and Dataset. 
        /// </summary>
        /// <param name="radioConnection">The radio connection type to use.</param>
        /// <param name="threadDeviceType">
        /// The type of the device in the Thread mesh network. An end device or router. 
        /// </param>
        /// <param name="deviceNumber">
        /// <para>
        /// The device number to use for the connection. For Native radio connection this is unused.
        /// </para>
        /// <para>
        /// For a UART connection, the number is the COM port to use for connection to radio. 
        /// The UART parameters are currently fixed at 8 data bit, no parity, 460800 baud. 
        /// Make sure the RX and TX pins used are configured for the associated COM port before calling constructor.
        /// </para>
        /// <para>
        /// For SPI connection the number is the SPI bus port to use for connection to radio. 
        /// Make sure the MISO, MOSI and Clock pins used are configured for the associated SPI port before calling constructor.
        /// </para>
        /// </param>
        /// <param name="speed">
        /// For UART connections this is the serial baud rate which defaults to 460800 if not specified.
        /// </param>
        /// <remarks>
        /// After constructing the OpenThread object then call the Start method to configure and start the OpenTHread stack.
        /// </remarks>
        internal OpenThread(RadioConnection radioConnection, ThreadDeviceType threadDeviceType, int deviceNumber = 1, int speed = 460800)
        {
            _radioType = radioConnection;
            _threadDeviceType = threadDeviceType;
            _port = deviceNumber;
            _speed = speed;

            _openThreadEventManager.OpenThread = this;

            NativeCreateStack();
        }

        /// <summary>
        /// Create an OpenThread object for use with inbuilt radio.
        /// </summary>
        /// <param name="threadDeviceType">
        /// The type of the device in the Thread mesh network. An end device or router. 
        /// </param>
        /// <returns>OpenThread object.</returns>
        public static OpenThread CreateThreadWithNativeRadio(ThreadDeviceType threadDeviceType = ThreadDeviceType.Router)
        {
            return new OpenThread(RadioConnection.Native, threadDeviceType);
        }

        /// <summary>
        /// Create an OpenThread object for use with a RCP radio connected via a UART.
        /// </summary>
        /// <param name="threadDeviceType">
        /// The type of the device in the Thread mesh network. An end device or router. </param>
        /// <param name="comPortNumber">The COM port number to use for radio. 
        /// If required the pins used by COM port must be configured before this call.</param>
        /// <param name="speed">The serial speed of connection. Defaults to 460800 bps.</param>
        /// <returns>OpenThread Object.</returns>
        public static OpenThread CreateThreadWithUartRadio(ThreadDeviceType threadDeviceType, int comPortNumber, int speed = 460800)
        {
            return new OpenThread(RadioConnection.Uart, threadDeviceType, comPortNumber, speed);
        }

        /// <summary>
        /// Create an OpenThread object for use with a RCP radio connected via a SPI port.
        /// </summary>
        /// <param name="threadDeviceType">
        /// The type of the device in the Thread mesh network. An end device or router. </param>
        /// <param name="spiDeviceNumber">The SPI number where radio is connected.
        /// If required the pins used by SPI port must be set up before this call.</param>
        /// <returns>OpenThread object.</returns>
        public static OpenThread CreateThreadWithSpiRadio(ThreadDeviceType threadDeviceType, int spiDeviceNumber)
        {
            return new OpenThread(RadioConnection.Spi, threadDeviceType, spiDeviceNumber);
        }

        /// <summary>
        /// Return devices current role. 
        /// </summary>
        public ThreadDeviceRole Role => _currentDeviceRole;

        /// <summary>
        /// Gets or Sets the active dataset. 
        /// </summary>
        public OpenThreadDataset Dataset
        {
            get
            {
                NativeGetActiveDataset();
                return _dataset;
            }
            set
            {
                _dataset = value;
                NativeSetActiveDataset();
            }
        }

        /// <summary>
        /// Returns the Local Mesh IPV6 address.
        /// </summary>
        public IPAddress MeshLocalAddress { get => new IPAddress(NativeGetMeshLocalAddress()); }

        /// <summary>
        /// Start OpenThread stack running. 
        /// The device will try to connect to existing thread network based on defined dataset.  
        /// </summary>
        /// <exception cref="InvalidOperationException">OpenThread stack already started.</exception>

        public void Start()
        {
            if (!_started)
            {
                NativeStartThread();
                _started = true;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Stop OpenThread stack running.
        /// </summary>
        /// <exception cref="InvalidOperationException">OpenThread stack not running.</exception>
        public void Stop()
        {
            if (_started)
            {
                NativeStopThread();
                _started = false;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Start the Joiner role for commissioning the device. This can only be called before the OpenThread has been started.
        /// Will return when the commissioning has completed or failed.
        /// </summary>
        /// <param name="pskc">Pre-shared key for the commissioner.</param>
        /// <returns>Returns true is commissioning has completed successfully. If an error the
        /// OpenThread error code is stored in the property JoinerStartError</returns>
        public bool JoinerStart(string pskc)
        {
            _joinerResultAvailable.Reset();

            // Start the Joiner role
            NativeJoinerStart(pskc);

            // Wait for Joiner result via event 
            if (_joinerResultAvailable.WaitOne(30000, false))
            {
                if (_openThreadErrorCode == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The OpenThread error result of last JoinerStart command.
        /// </summary>
        public int OpenThreadError => _openThreadErrorCode;

        /// <summary>
        /// Send a Command line input (CLI command) and wait for result.
        /// </summary>
        /// <param name="input">Command to send</param>
        public string[] CommandLineInputAndWaitResponse(string input)
        {
            _consoleOutputAvailable.Reset();
            _consoleCommandResult = new string[0];

            _commandInProgress = true;
            NativeSendConsoleInput(input, true);

            // Wait for command to complete
            _consoleOutputAvailable.WaitOne(30000, true);

            // Return console output strings excluding "Done"
            return _consoleCommandResult;
        }

        /// <summary>
        /// Send a Command line input (CLI command) and don't wait for response. All results will be 
        /// fired as OnConsoleOutputAvailable event.
        /// </summary>
        /// <param name="input">Command to send</param>
        public void CommandLineInput(string input)
        {
            NativeSendConsoleInput(input, false);
        }

        internal void FireStatusChangedEvent(OpenThreadStateChangeEventArgs e)
        {
            OnStatusChanged?.Invoke(this, e);
        }

        internal void FireRoleChangedEvent(OpenThreadRoleChangeEventArgs e)
        {
            OnRoleChanged?.Invoke(this, e);
        }

        internal void FireJoinerResultEvent(OpenThreadJoinerStartCompleteEventArgs e)
        {
            _openThreadErrorCode = e.error;
            _joinerResultAvailable.Set();
        }

        internal void FireConsoleOutputAvailableEvent(OpenThreadConsoleOutputAvailableArgs e)
        {
            if (_commandInProgress)
            {
                _consoleCommandResult = e.consoleLines;
                _consoleOutputAvailable.Set();
                _commandInProgress = false;
                System.Threading.Thread.Sleep(1);
            }
            else
            {
                OnConsoleOutputAvailable?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_started)
                    {
                        NativeDispose();
                    }
                }

                _dataset = null;
                _disposedValue = true;

                _openThreadEventManager.OpenThread = null;
            }
        }

        /// <summary>
        /// OpenThread finalize r
        /// </summary>
        ~OpenThread()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <summary>
        /// Dispose OpenThread object and release native memory.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region Native

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeCreateStack();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSetActiveDataset();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeGetActiveDataset();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeJoinerStart(string pskc);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStartThread();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeStopThread();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeDispose();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeSendConsoleInput(string input, bool collectResponse);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern string[] NativeGetConsoleOutput();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Byte[] NativeGetMeshLocalAddress();

        #endregion Native
    }
}