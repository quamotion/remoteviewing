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
#endif

#include <stdlib.h>
#include <stdio.h>
#include <stdarg.h>

typedef void (*LogProc)(int level, char* message, int length);

DllExport LogProc logCallback = NULL;

static void Log(int level, const char* format, va_list args)
{
    int length = vsnprintf(NULL, 0, format, args) + 1;

    char* buffer = (char*)malloc(length * sizeof(char));

    if (buffer != NULL)
    {
        vsprintf(buffer, format, args);

        logCallback(level, buffer, length);

        free(buffer);
    }
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
