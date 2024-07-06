[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.Device.Bluetooth&metric=alert_status)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.Device.Bluetooth) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=nanoframework_nanoFramework.Networking.Thread&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=nanoframework_nanoFramework.Networking.Thread) [![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE) [![NuGet](https://img.shields.io/nuget/dt/nanoFramework.Networking.Thread.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Networking.Thread/) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/nanoframework/Home/blob/main/CONTRIBUTING.md) [![Discord](https://img.shields.io/discord/478725473862549535.svg?logo=discord&logoColor=white&label=Discord&color=7289DA)](https://discord.gg/gCyBu8T)

![nanoFramework logo](https://raw.githubusercontent.com/nanoframework/Home/main/resources/logo/nanoFramework-repo-logo.png)

-----
document language: [English](README.md) | [简体中文](README.zh-cn.md)

# Welcome to the .NET **nanoFramework** nanoFramework.Networking.Thread Library repository

## Build status

| Component | Build Status | NuGet Package |
|:-|---|---|
| nanoFramework.Networking.Thread | [![Build Status](https://dev.azure.com/nanoframework/nanoFramework.Networking.Thread/_apis/build/status/nanoFramework.Networking.Thread?repoName=nanoframework%2FnanoFramework.Networking.Thread&branchName=main)](https://dev.azure.com/nanoframework/nanoFramework.Networking.Thread/_build/latest?definitionId=85&repoName=nanoframework%2FnanoFramework.Networking.Thread&branchName=main) | [![NuGet](https://img.shields.io/nuget/v/nanoFramework.Networking.Thread.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/nanoFramework.Networking.Thread/) |

# nanoFramework.Networking.Thread

 .NET nanoFramework library for working with OpenThread networking. 
 
 The OpenThread network is based on IPV6 and requires a firmware which has IPV6 and Thread enabled.
 Currently there 3 different firmwares available.

 - ESP32_C6_Thread
 - ESP32_H2_Thread
 - ESP32_PSRAM_REV3_IPV6
 
 The ESP32_PSRAM_REV3_IPV6 firmware requires a OpenThread radio Co-Processor using the RCP firmware. This 
 firmware is available in the Espressif IDF SDK under examples/openthread/ot_rcp
 
 This library is the initial version and is a work in progress. More work needs to be done to add direct support for CoAP.
 This version enables access to the OpenThread CLI which allows most options to be accessed if not available directly in C#.
 
 For more information on OpenThread see the website https://openthread.io/

 ## Creating the Thread Network stack object

 When the openThread stack is created the node device type needs to be specified. 

 This can be one of the types.
	
- Router
- End device
- Sleepy end device

The **Router** and **End device** are for a powered devices. 
The **End device** will stay in the **child** role and won't ever be promoted to a router.
The **Router** type will start with the child role and be promoted to the **router** or **leader** roles as required.

The **Sleepy end device** are for battery operated devices and don't stay connected to
the mesh network to save on battery. It will regularly poll its connected router for any messages. Routers connected
to a sleepy end device will store and forward messages. 

The devices can take on a number of roles as already mentioned. 
These are child, Router or Leader. 

### Devices with a built-in 802.15.4 radio

Creating openThread object for a device with a built-in 802.15.4 radio.

 ```c#
OpenThread ot = OpenThread.CreateThreadWithNativeRadio(ThreadDeviceType.Router);
 ```

### Devices with a Co-Processor with radio (RCP) 

#### Connected via a UART port
 
Create the openThread object specifying the COM port connected to the co-processor. 
Before calling this, the pins used by COM port may need to be set up. 
This depends on the target device. (i.e. ESP32)

This code creates the stack using the network processor connected by UART on device COM1.

```c#
OpenThread ot = OpenThread.CreateThreadWithUartRadio(ThreadDeviceType.Router, 1);
```    

#### Connected via a SPI port.**

Create the openThread object specifying the SPI port connected to the co-processor. 
Before calling this, the pins used by SPI port may need to be set up. 
This depends on the target device. (i.e. ESP32)

This code creates the stack using the network processor connected by SPI on device SPI1.

```c#
OpenThread ot = OpenThread.CreateThreadWithSpiRadio(ThreadDeviceType.Router, 1);
```     


## Connecting to an existing mesh network

To connect to an existing Thread network you need to specify the minimum of the network name, network key and panId.
Supplying other parameters like the channel number reduces the time it take to re-connect as it doesn't need to scan 
all channels for network.

 ```c#
 // Create dataset for existing network
OpenThreadDataset data = new OpenThreadDataset()
{
    NetworkName = "nanoFramework",
    NetworkKey = new byte[16] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
    Channel = 15
};

// Set the Active Dataset
ot.Dataset = data;

// Start Thread network
ot.Start();
```

If an existing network already exists it will attach to it. 
If not it will wait about 2 minutes and create a new network using credentials in dataset.

Monitor the state change events to know when the device has attached to the mesh network.
See later section on events raised by the OpenThread class.

## Joining a existing network using the commissioner.
 
 To attach a device to an existing network where the network credentials are unknown. 
 A commissioner from another device on network(border router) or an external device will need to be used.

 This is done using a pre-shared key between the device and the commissioner.
 Once the device is connected to network the network credentials can be saved so that next time it can attach to the mesh directly.
 
 Start the joiner using the pre-shared key (PSKd)
 When JoinerStart() return true the commissioning will be complete.

 Loop forever until device is commissioned.

 ```c#
 while (ot.JoinerStart("J01NME") == false)
 {
     // Wait for Join to work
     Thread.Sleep(2000);
 }

 // Start Thread network
ot.Start();
 ```

Once the device is in an attached state the current dataset 
can be read using the **Dataset** property of the openThread object and saved for next time.

## Monitoring events to check state.

The **OpenThread** object has 3 events that can be monitored. 
 
 ```c#
ot.OnStatusChanged += Ot_OnStatusChanged;
ot.OnRoleChanged += Ot_OnRoleChanged;
ot.OnConsoleOutputAvailable += Ot_OnConsoleOutputAvailable;
```

### OnStatusChanged

This event is fired when the state of the Thread stack changes. 
The main states that need to be monitored are the Detached and Attached events.
There will also be events for network interface up/down, an IPV6 address has been assigned and when the stack has been started or stopped.

Here is a example of handling the Attached event. It uses an event to signal main thread when connected to
mesh network. The main thread can wait on the event after starting the openThread.

See samples for full code.

```c#
static AutoResetEvent WaitNetAttached = new AutoResetEvent(false);

private static void Ot_OnStatusChanged(OpenThread sender, OpenThreadStateChangeEventArgs args)
{
    switch ((ThreadDeviceState)args.currentState)
    {
	    case ThreadDeviceState.OPENTHREAD_EVENT_ATTACHED:
            WaitNetAttached.Set();
            break;
    }
}
```

### OnRoleChanged

This event is fired when the role of the device changes. 

The role can be one of the following:

| Role | Description |
| ----- | ----- |
| Disabled | OpenThread stack not started. |
| Detached | OpenThread stack started but not connected to mesh. |
| Child | The device is currently a child, connected to a router. |
| Router | The device is a router and has 0 or more child devices. |
| Leader | The device is a router that has been promoted to leader. |

### OnConsoleOutputAvailable  

This event is fired when unsolicited messages are received on Command Line Interface (CLI).
The event will supply an array of strings from the CLI to the application. See CLI section for 
more information.

## Communicating over mesh network

With Thread the main means of communication is via UDP due to its low resource overhead. With
UDP error/retries needs to be done at the application level.  For that reason CoAP is the recommended way
to transfer information between devices as it has mechanisms to guarantee message delivery.

In OpenThread, multi cast addresses are used to communicate with multiple devices simultaneously within the network. 
For example the IPV6 multi cast address **FC03::1** can be used to communicate with all nodes except sleepy end devices in the mesh network.
Sleepy end devices need to be addressed directly.

For more information on OpenThread IPV6 addressing see: https://openthread.io/guides/thread-primer/ipv6-addressing.
These addresses are crucial for sending messages to multiple devices efficiently in an OpenThread network.

### Communicating with CoAP

The openThread stack includes coAP support. Currently this is not been implemented from C# but
it can be accessed using the CLI interface although this method is limited.

Another way is to use a third party library.

### Communicating via UDP using CLI

The CLI supports limited UDP communication but the best method now is to use the normal UDP socket 
communication built in to nanoFramework. The samples show how this can be done using sockets.

### Communicating via UDP using sockets

For examples on using sockets for communication see the Thread sockets samples

### Communicating via TCP/IP.

As the OpenThread is based on an IP networking it also supports communication via TCP/IP. All the 
normal methods of communicating with TCP/IP will work. 

## OpenThread CLI interface.

The OpenThread library has a large number of different APIs.  
We can't implement all of these in this library but we have created an API to call the Command Line Interface(CLI) which
gives access to all the OpenThread APIs available in the build.

For details on available CLI commands see: https://openthread.io/reference/cli/commands

There are 2 different APIs to send commands to the CLI. One where the result of the command is returned as a return value 
of API and another API where all results are fired on OnConsoleOutputAvailable event.

If you have unsolicited messages being outputted on the CLI at the same time as a command is returning results then
it will be better to just use the OnConsoleOutputAvailable event for all output.

### CommandLineInputAndWaitResponse

This will send a command to the CLI and wait for all strings returned until a 'done' string is seen.
These strings will be returned directly by the command excluding the 'done' message. 

CLI command to return all the IPV6 addresses assigned to the interface.

```c#
string[] results = ot.CommandLineInputAndWaitResponse("ipaddr");
```

If you want to know the IPV6 address for local communication then just use the property MeshLocalAddress.

```c#
IPAddress meshLocal = ot.MeshLocalAddress;
```

### CommandLineInput

The **CommandLineInput** method sends the command to the CLI and returns. All the results for 
the command are returned via the **OnConsoleOutputAvailable** event.

---

## Feedback and documentation

For documentation, providing feedback, issues and finding out how to contribute please refer to the [Home repo](https://github.com/nanoframework/Home).

Join our Discord community [here](https://discord.gg/gCyBu8T).

## Credits

The list of contributors to this project can be found at [CONTRIBUTORS](https://github.com/nanoframework/Home/blob/main/CONTRIBUTORS.md).

## License

The **nanoFramework** Class Libraries are licensed under the [MIT license](LICENSE.md).

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](https://dotnetfoundation.org/code-of-conduct).

## .NET Foundation

This project is supported by the [.NET Foundation](https://dotnetfoundation.org).
