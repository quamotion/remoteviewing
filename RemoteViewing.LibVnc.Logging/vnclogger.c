// RemoteViewing VNC Client/Server Library for .NET
// Copyright (c) 2020 Quamotion bvba
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// libvncserver defines the logging functions like this:
// typedef void (*rfbLogProc)(const char *format, ...);
//
// .NET has the __arglist keyword which is equivalent to ...,
// but this does not work in callbacks, see
// https://github.com/dotnet/runtime/issues/9316#issuecomment-353790995
//
// As a workaround, this library defines a function which takes care
// of message formatting in C and then invokes a delegate (which can
// be in managed code) with the formatted message.

#if defined (WIN32)
#define _CRT_SECURE_NO_WARNINGS
#define DllExport   __declspec( dllexport )
#else
#define DllExport
#define _GNU_SOURCE
#endif

#include <stdlib.h>
#include <stdio.h>
#include <stdarg.h>
#include "rfb/rfb.h"

typedef void (*LogProc)(int level, char* message, int length);

DllExport LogProc logCallback = NULL;

static void Log(int level, const char* format, va_list args)
{
#if defined(_MSC_VER)
    int length = vsnprintf(NULL, 0, format, args) + 1;

    char* buffer = (char*)malloc(length * sizeof(char));

    if (buffer != NULL)
    {
        vsprintf(buffer, format, args);

        logCallback(level, buffer, length - 1);

        free(buffer);
    }
#else
   char* buffer = NULL;
   int length = 0;

   length = vasprintf(&buffer, format, args);

   if (buffer != NULL)
   {
       logCallback(level, buffer, length);

       free(buffer);
   }
#endif
}

DllExport void
LogMessage(const char* format, ...)
{
    va_list args;
    va_start(args, format);

    Log(0, format, args);

    va_end(args);
}

DllExport void
LogError(const char* format, ...)
{
    va_list args;
    va_start(args, format);

    Log(1, format, args);

    va_end(args);
}

DllExport int
rfbScreenInfo_get_width(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->width;
}

DllExport void
rfbScreenInfo_set_width(rfbScreenInfoPtr rfbScreen, int width)
{
    rfbScreen->width = width;
}

DllExport int
rfbScreenInfo_get_height(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->height;
}

DllExport void
rfbScreenInfo_set_height(rfbScreenInfoPtr rfbScreen, int height)
{
    rfbScreen->height = height;
}

DllExport void*
rfbScreenInfo_get_screenData(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->screenData;
}

DllExport void
rfbScreenInfo_set_screenData(rfbScreenInfoPtr rfbScreen, void* screenData)
{
    rfbScreen->screenData = screenData;
}

DllExport rfbPixelFormat
rfbScreenInfo_get_serverFormat(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->serverFormat;
}

DllExport void
rfbScreenInfo_set_serverFormat(rfbScreenInfoPtr rfbScreen, rfbPixelFormat serverFormat)
{
    rfbScreen->serverFormat = serverFormat;
}

DllExport rfbBool
rfbScreenInfo_get_autoPort(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->autoPort;
}

DllExport void
rfbScreenInfo_set_autoPort(rfbScreenInfoPtr rfbScreen, rfbBool autoPort)
{
    rfbScreen->autoPort = autoPort;
}

DllExport int
rfbScreenInfo_get_port(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->port;
}

DllExport void
rfbScreenInfo_set_port(rfbScreenInfoPtr rfbScreen, int port)
{
    rfbScreen->port = port;
}

DllExport in_addr_t
rfbScreenInfo_get_listenInterface(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->listenInterface;
}

DllExport void
rfbScreenInfo_set_listenInterface(rfbScreenInfoPtr rfbScreen, in_addr_t listenInterface)
{
    rfbScreen->listenInterface = listenInterface;
}

DllExport char*
rfbScreenInfo_get_frameBuffer(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->frameBuffer;
}

DllExport void
rfbScreenInfo_set_frameBuffer(rfbScreenInfoPtr rfbScreen, char* frameBuffer)
{
    rfbScreen->frameBuffer = frameBuffer;
}

DllExport rfbKbdAddEventProcPtr
rfbScreenInfo_get_kbdAddEvent(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->kbdAddEvent;
}

DllExport void
rfbScreenInfo_set_kbdAddEvent(rfbScreenInfoPtr rfbScreen, rfbKbdAddEventProcPtr kbdAddEvent)
{
    rfbScreen->kbdAddEvent = kbdAddEvent;
}

DllExport rfbPtrAddEventProcPtr
rfbScreenInfo_get_ptrAddEvent(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->ptrAddEvent;
}

DllExport void
rfbScreenInfo_set_ptrAddEvent(rfbScreenInfoPtr rfbScreen, rfbPtrAddEventProcPtr ptrAddEvent)
{
    rfbScreen->ptrAddEvent = ptrAddEvent;
}

DllExport rfbNewClientHookPtr
rfbScreenInfo_get_newClientHook(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->newClientHook;
}

DllExport void
rfbScreenInfo_set_newClientHook(rfbScreenInfoPtr rfbScreen, rfbNewClientHookPtr newClientHook)
{
    rfbScreen->newClientHook = newClientHook;
}

DllExport void*
rfbScreenInfo_get_authPasswdData(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->authPasswdData;
}

DllExport void
rfbScreenInfo_set_authPasswdData(rfbScreenInfoPtr rfbScreen, void* authPasswdData)
{
    rfbScreen->authPasswdData = authPasswdData;
}

DllExport rfbPasswordCheckProcPtr
rfbScreenInfo_get_passwordCheck(rfbScreenInfoPtr rfbScreen)
{
    return rfbScreen->passwordCheck;
}

DllExport void
rfbScreenInfo_set_passwordCheck(rfbScreenInfoPtr rfbScreen, rfbPasswordCheckProcPtr passwordCheck)
{
    rfbScreen->passwordCheck = passwordCheck;
}