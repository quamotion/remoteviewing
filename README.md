# RemoteViewing (Quamotion branch)
[![Build status](https://ci.appveyor.com/api/projects/status/cglx3gjok6a8w6mv?svg=true)](https://ci.appveyor.com/project/qmfrederik/remoteviewing)

RemoteViewing is a .NET-native VNC client and server library.

You can use RemoteViewing to write your own VNC server or client.

It supports Raw, Hextile, Copyrect, and Zlib encodings, and includes a Windows Forms control to make embedding VNC in your program extremely easy.

RemoteViewing uses the BSD license.

## Installation

Install using the command line:

```
Install-Package Quamotion.RemoteViewing
```

## Samples

This repository contains three sample projects:

- `RemoteViewing.NoVncExample` implements a noVNC server using RemoteViewing. The server itself is implemented using ASP.NET Core
- `RemoteViewing.ServerExample` implements a VNC server which allows you to share your desktop
- `RemoteViewing.Example` implements a VNC client using Windows Forms.

## Credits
The code in this repository is based on the download available at
http://www.zer7.com/software/remoteviewing
