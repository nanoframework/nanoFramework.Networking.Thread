//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using nanoFramework.Runtime.Events;

namespace nanoFramework.Networking.Thread
{
    /// <summary>
    /// Event types sent from native code.
    /// </summary>
    enum OpenThreadEventType
    {
        StateChanged,
        RoleChanged,
        CommandOutputAvailable,
        JoinerComplete
    };

    internal class OpenThreadEventListener : IEventProcessor, IEventListener
    {
        // Reference to current open thread object for passing events.
        OpenThread _openThreadObject = null;

        public OpenThreadEventListener()
        {
            EventSink.AddEventProcessor(EventCategory.OpenThread, this);
            EventSink.AddEventListener(EventCategory.OpenThread, this);
        }

        public void InitializeForEventSource()
        {
        }

        public OpenThread OpenThread { get => _openThreadObject; set => _openThreadObject = value; }

        public BaseEvent ProcessEvent(uint data1, uint data2, DateTime time)
        {
            OpenThreadEventType otEventType = (OpenThreadEventType)(data1 & 0xff);
            switch (otEventType)
            {
                case OpenThreadEventType.RoleChanged:
                    return new OpenThreadRoleChangeEventArgs
                    {
                        currentRole = (ThreadDeviceRole)(data2 & 0xff),
                        previousRole = (ThreadDeviceRole)((data2 >> 8) & 0xff)
                    };

                case OpenThreadEventType.StateChanged:
                    return new OpenThreadStateChangeEventArgs
                    {
                        currentState = (int)(data2 & 0xff)
                    };

                case OpenThreadEventType.CommandOutputAvailable:
                    return new OpenThreadConsoleOutputAvailableArgs()
                    {
                        consoleLines = _openThreadObject.NativeGetConsoleOutput()
                    };

                default:
                case OpenThreadEventType.JoinerComplete:
                    return new OpenThreadJoinerStartCompleteEventArgs()
                    {
                        error = (int)(data2 & 0xff)
                    };
            }
        }

        public bool OnEvent(BaseEvent ev)
        {
            if (_openThreadObject != null)
            {
                if (ev.GetType() == typeof(OpenThreadRoleChangeEventArgs))
                {
                    _openThreadObject.FireRoleChangedEvent((OpenThreadRoleChangeEventArgs)ev);
                    return true;
                }
                else
                if (ev.GetType() == typeof(OpenThreadStateChangeEventArgs))
                {
                    _openThreadObject.FireStatusChangedEvent((OpenThreadStateChangeEventArgs)ev);
                    return true;
                }
                else
                if (ev.GetType() == typeof(OpenThreadConsoleOutputAvailableArgs))
                {
                    _openThreadObject.FireConsoleOutputAvailableEvent((OpenThreadConsoleOutputAvailableArgs)ev);
                    return true;
                }
                if (ev.GetType() == typeof(OpenThreadJoinerStartCompleteEventArgs))
                {
                    _openThreadObject.FireJoinerResultEvent((OpenThreadJoinerStartCompleteEventArgs)ev);
                    return true;
                }
            }
            // Not handled
            return false;
        }
    }
}
