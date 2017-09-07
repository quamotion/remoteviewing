#region License
/*
RemoteViewing VNC Client/Server Library for .NET
Copyright (c) 2017 Quamotion bvba
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace RemoteViewing.Vnc
{
    /// <summary>
    /// <para>
    ///   keysym codes are 29-bit integer values identify characters or
    ///   functions associated with each key (e.g., via the visible
    ///   engraving) of a keyboard layout.
    /// </para>
    /// <para>
    ///   For most ordinary keys, the keysym is the same as the corresponding
    ///   ASCII value.
    /// </para>
    /// <para>
    ///   Modern versions of the X Window System handle keysyms for Unicode
    ///   characters, consisting of the Unicode character with the hex
    ///   1000000 bit set.
    /// </para>
    /// </summary>
    /// <seealso href="https://cgit.freedesktop.org/xorg/proto/x11proto/tree/keysymdef.h"/>
    /// <seealso href="https://tools.ietf.org/pdf/rfc6143.pdf"/>
    [CLSCompliant(false)]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300", Justification = "Match actual key values.", Scope = "type")]
    public enum KeySym
    {
        /// <summary>
        /// The void symbol.
        /// </summary>
        VoidSymbol = 0xffffff,

        /*
         * TTY function keys, cleverly chosen to map to ASCII, for convenience of
         * programming, but could have been arbitrary (at the cost of lookup
         * tables in client code).
         */

        /// <summary>
        /// Back space key
        /// </summary>
        Backspace = 0xff08,

        /// <summary>
        /// The tab key
        /// </summary>
        Tab = 0xff09,

        /// <summary>
        /// The line feed key
        /// </summary>
        LineFeed = 0xff0a,  /* Linefeed, LF */

        /// <summary>
        /// The clear key.
        /// </summary>
        Clear = 0xff0b,

        /// <summary>
        /// The return (enter) key.
        /// </summary>
        Return = 0xff0d,  /* Return, enter */

        /// <summary>
        /// The pause key
        /// </summary>
        Pause = 0xff13,  /* Pause, hold */

        /// <summary>
        /// The scroll lock key.
        /// </summary>
        ScrollLock = 0xff14,

        /// <summary>
        /// The Sys Req key.
        /// </summary>
        SysReq = 0xff15,

        /// <summary>
        /// The escape key.
        /// </summary>
        Escape = 0xff1b,

        /// <summary>
        /// The delete key.
        /// </summary>
        Delete = 0xffff,  /* Delete, rubout */

        /* Cursor control & motion */

        /// <summary>
        /// The home key
        /// </summary>
        Home = 0xff50,

        /// <summary>
        /// The left arrow key.
        /// </summary>
        Left = 0xff51,  /* Move left, left arrow */

        /// <summary>
        /// The up arrow key.
        /// </summary>
        Up = 0xff52,  /* Move up, up arrow */

        /// <summary>
        /// The right arrow key.
        /// </summary>
        Right = 0xff53,  /* Move right, right arrow */

        /// <summary>
        /// The down key.
        /// </summary>
        Down = 0xff54,  /* Move down, down arrow */

        /// <summary>
        /// The previous key.
        /// </summary>
        Prior = 0xff55,  /* Prior, previous */

        /// <summary>
        /// The page up key.
        /// </summary>
        PageUp = 0xff55,

        /// <summary>
        /// The next key.
        /// </summary>
        Next = 0xff56,  /* Next */

        /// <summary>
        /// The page down key.
        /// </summary>
        PageDown = 0xff56,

        /// <summary>
        /// The end key.
        /// </summary>
        End = 0xff57,  /* EOL */

        /// <summary>
        /// The begin key.
        /// </summary>
        Begin = 0xff58,  /* BOL */

        /* Misc functions */

        /// <summary>
        /// The select key.
        /// </summary>
        Select = 0xff60,  /* Select, mark */

        /// <summary>
        /// The print screen key.
        /// </summary>
        Print = 0xff61,

        /// <summary>
        /// The execute key.
        /// </summary>
        Execute = 0xff62,  /* Execute, run, do */

        /// <summary>
        /// The insert key.
        /// </summary>
        Insert = 0xff63,  /* Insert, insert here */

        /// <summary>
        /// The undo key.
        /// </summary>
        Undo = 0xff65,

        /// <summary>
        /// The redo key.
        /// </summary>
        Redo = 0xff66,  /* Redo, again */

        /// <summary>
        /// The menu key.
        /// </summary>
        Menu = 0xff67,

        /// <summary>
        /// The find key.
        /// </summary>
        Find = 0xff68,  /* Find, search */

        /// <summary>
        /// The cancel key.
        /// </summary>
        Cancel = 0xff69,  /* Cancel, stop, abort, exit */

        /// <summary>
        /// The help key.
        /// </summary>
        Help = 0xff6a,  /* Help */

        /// <summary>
        /// The break key.
        /// </summary>
        Break = 0xff6b,

        /// <summary>
        /// The switch key.
        /// </summary>
        ModeSwitch = 0xff7e,  /* Character set switch */

        /// <summary>
        /// The num lock key.
        /// </summary>
        Num_Lock = 0xff7f,

        /* Keypad functions, keypad numbers cleverly chosen to map to ASCII */

        /// <summary>
        /// The space key on the numeric keypad.
        /// </summary>
        NumPadSpace = 0xff80, /* Space */

        /// <summary>
        /// The tab key on the numeric keypad.
        /// </summary>
        NumPadTab = 0xff89,

        /// <summary>
        /// The enter key on the numeric keypad.
        /// </summary>
        NumPadEnter = 0xff8d,  /* Enter */

        /// <summary>
        /// The F1 key on the numeric keypad.
        /// </summary>
        NumPadF1 = 0xff91,  /* PF1, NumPadA, ... */

        /// <summary>
        /// The F2 key on the numeric keypad.
        /// </summary>
        NumPadF2 = 0xff92,

        /// <summary>
        /// The F3 key on the numeric keypad.
        /// </summary>
        NumPadF3 = 0xff93,

        /// <summary>
        /// The F4 key on the numeric keypad.
        /// </summary>
        NumPadF4 = 0xff94,

        /// <summary>
        /// The home key on the numeric keypad.
        /// </summary>
        NumPadHome = 0xff95,

        /// <summary>
        /// The left key on the numeric keypad.
        /// </summary>
        NumPadLeft = 0xff96,

        /// <summary>
        /// The up key on the numeric keypad.
        /// </summary>
        NumPadUp = 0xff97,

        /// <summary>
        /// The right key on the numeric keypad.
        /// </summary>
        NumPadRight = 0xff98,

        /// <summary>
        /// The down key on the numeric keypad.
        /// </summary>
        NumPadDown = 0xff99,

        /// <summary>
        /// The prior key on the numeric keypad.
        /// </summary>
        NumPadPrior = 0xff9a,

        /// <summary>
        /// The up key on the numeric keypad.
        /// </summary>
        NumPadPageUp = 0xff9a,

        /// <summary>
        /// The next key on the numeric keypad.
        /// </summary>
        NumPadNext = 0xff9b,

        /// <summary>
        /// The page down key on the numeric keypad.
        /// </summary>
        NumPadPageDown = 0xff9b,

        /// <summary>
        /// The end key on the numeric keypad.
        /// </summary>
        NumPadEnd = 0xff9c,

        /// <summary>
        /// The begin key on the numeric keypad.
        /// </summary>
        NumPadBegin = 0xff9d,

        /// <summary>
        /// The insert key on the numeric keypad.
        /// </summary>
        NumPadInsert = 0xff9e,

        /// <summary>
        /// The delete key on the numeric keypad.
        /// </summary>
        NumPadDelete = 0xff9f,

        /// <summary>
        /// The equals key on the numeric keypad.
        /// </summary>
        NumPadEqual = 0xffbd, /* Equals */

        /// <summary>
        /// The multiply key on the numeric keypad.
        /// </summary>
        NumPadMultiply = 0xffaa,

        /// <summary>
        /// The addition key on the numeric keypad.
        /// </summary>
        NumPadAdd = 0xffab,

        /// <summary>
        /// The separator key on the numeric keypad.
        /// </summary>
        NumPadSeparator = 0xffac, /* Separator, often comma */

        /// <summary>
        /// The substract key on the numeric keypad.
        /// </summary>
        NumPadSubtract = 0xffad,

        /// <summary>
        /// The decimal divider key on the numeric keypad.
        /// </summary>
        NumPadDecimal = 0xffae,

        /// <summary>
        /// The divide key on the numeric keypad.
        /// </summary>
        NumPadDivide = 0xffaf,

        /// <summary>
        /// The 0 key on the numeric keypad.
        /// </summary>
        NumPad0 = 0xffb0,

        /// <summary>
        /// The 1 key on the numeric keypad.
        /// </summary>
        NumPad1 = 0xffb1,

        /// <summary>
        /// The 2 key on the numeric keypad.
        /// </summary>
        NumPad2 = 0xffb2,

        /// <summary>
        /// The 3 key on the numeric keypad.
        /// </summary>
        NumPad3 = 0xffb3,

        /// <summary>
        /// The 4 key on the numeric keypad.
        /// </summary>
        NumPad4 = 0xffb4,

        /// <summary>
        /// The 5 key on the numeric keypad.
        /// </summary>
        NumPad5 = 0xffb5,

        /// <summary>
        /// The 6 key on the numeric keypad.
        /// </summary>
        NumPad6 = 0xffb6,

        /// <summary>
        /// The 7 key on the numeric keypad.
        /// </summary>
        NumPad7 = 0xffb7,

        /// <summary>
        /// The 8 key on the numeric keypad.
        /// </summary>
        NumPad8 = 0xffb8,

        /// <summary>
        /// The 9 key on the numeric keypad.
        /// </summary>
        NumPad9 = 0xffb9,

        /*
         * Auxiliary functions; note the duplicate definitions for left and right
         * function keys;  Sun keyboards and a few other manufacturers have such
         * function key groups on the left and/or right sides of the keyboard.
         * We've not found a keyboard with more than 35 function keys total.
         */

        /// <summary>
        /// The F1 key.
        /// </summary>
        F1 = 0xffbe,

        /// <summary>
        /// The F2 key.
        /// </summary>
        F2 = 0xffbf,

        /// <summary>
        /// The F3 key.
        /// </summary>
        F3 = 0xffc0,

        /// <summary>
        /// The F4 key.
        /// </summary>
        F4 = 0xffc1,

        /// <summary>
        /// The F5 key.
        /// </summary>
        F5 = 0xffc2,

        /// <summary>
        /// The F6 key.
        /// </summary>
        F6 = 0xffc3,

        /// <summary>
        /// The F7 key.
        /// </summary>
        F7 = 0xffc4,

        /// <summary>
        /// The F8 key.
        /// </summary>
        F8 = 0xffc5,

        /// <summary>
        /// The F9 key.
        /// </summary>
        F9 = 0xffc6,

        /// <summary>
        /// The F10 key.
        /// </summary>
        F10 = 0xffc7,

        /// <summary>
        /// The F11 key.
        /// </summary>
        F11 = 0xffc8,

        /// <summary>
        /// The F12 key.
        /// </summary>
        F12 = 0xffc9,

        /// <summary>
        /// The F13 key.
        /// </summary>
        F13 = 0xffca,

        /// <summary>
        /// The F14 key.
        /// </summary>
        F14 = 0xffcb,

        /// <summary>
        /// The F15 key.
        /// </summary>
        F15 = 0xffcc,

        /// <summary>
        /// The F16 key.
        /// </summary>
        F16 = 0xffcd,

        /// <summary>
        /// The F17 key.
        /// </summary>
        F17 = 0xffce,

        /// <summary>
        /// The F18 key.
        /// </summary>
        F18 = 0xffcf,

        /// <summary>
        /// The F19 key.
        /// </summary>
        F19 = 0xffd0,

        /// <summary>
        /// The F20 key.
        /// </summary>
        F20 = 0xffd1,

        /// <summary>
        /// The F21 key.
        /// </summary>
        F21 = 0xffd2,

        /// <summary>
        /// The F22 key.
        /// </summary>
        F22 = 0xffd3,

        /// <summary>
        /// The F23 key.
        /// </summary>
        F23 = 0xffd4,

        /// <summary>
        /// The F24 key.
        /// </summary>
        F24 = 0xffd5,

        /* Modifiers */

        /// <summary>
        /// The left shift key.
        /// </summary>
        ShiftLeft = 0xffe1,  /* Left shift */

        /// <summary>
        /// The right shift key
        /// </summary>
        ShiftRight = 0xffe2,  /* Right shift */

        /// <summary>
        /// The left control key.
        /// </summary>
        ControlLeft = 0xffe3,  /* Left control */

        /// <summary>
        /// The right control key.
        /// </summary>
        ControlRight = 0xffe4,  /* Right control */

        /// <summary>
        /// The caps lock key.
        /// </summary>
        CapsLock = 0xffe5,  /* Caps lock */

        /// <summary>
        /// The shift lock key.
        /// </summary>
        ShiftLock = 0xffe6,  /* Shift lock */

        /// <summary>
        /// The left meta key.
        /// </summary>
        MetaLeft = 0xffe7,  /* Left meta */

        /// <summary>
        /// The right meta key.
        /// </summary>
        MetaRight = 0xffe8,  /* Right meta */

        /// <summary>
        /// The left alt key.
        /// </summary>
        AltLeft = 0xffe9,  /* Left alt */

        /// <summary>
        /// The right alt key.
        /// </summary>
        AltRight = 0xffea,  /* Right alt */

        /// <summary>
        /// The left super key.
        /// </summary>
        SuperLeft = 0xffeb,  /* Left super */

        /// <summary>
        /// The right super key.
        /// </summary>
        SuperRight = 0xffec,  /* Right super */

        /// <summary>
        /// The left hyper key.
        /// </summary>
        HyperLeft = 0xffed,  /* Left hyper */

        /// <summary>
        /// The right hyper key.
        /// </summary>
        HyperRight = 0xffee,  /* Right hyper */

        /*
         * Latin 1
         * (ISO/IEC 8859-1 = Unicode U+0020..U+00FF)
         * Byte 3 = 0
         */

        /// <summary>
        /// The space key.
        /// </summary>
        Space = 0x0020,  /* U+0020 SPACE */

        /// <summary>
        /// The exclamation mark (!) key.
        /// </summary>
        Exclamation = 0x0021,  /* U+0021 EXCLAMATION MARK */

        /// <summary>
        /// The double quotation (") key.
        /// </summary>
        Quote = 0x0022,  /* U+0022 QUOTATION MARK */

        /// <summary>
        /// The number sign (#) key.
        /// </summary>
        NumberSign = 0x0023,  /* U+0023 NUMBER SIGN */

        /// <summary>
        /// The dollar sign ($) key.
        /// </summary>
        Dollar = 0x0024,  /* U+0024 DOLLAR SIGN */

        /// <summary>
        /// The precent sign (%) key.
        /// </summary>
        Percent = 0x0025,  /* U+0025 PERCENT SIGN */

        /// <summary>
        /// The ampersand (&amp;) key.
        /// </summary>
        Ampersand = 0x0026,  /* U+0026 AMPERSAND */

        /// <summary>
        /// The apostrophe (') key.
        /// </summary>
        Apostrophe = 0x0027,  /* U+0027 APOSTROPHE */

        /// <summary>
        /// The left parenthesis (() key.
        /// </summary>
        ParenthesisLeft = 0x0028,  /* U+0028 LEFT PARENTHESIS */

        /// <summary>
        /// The right parenthesis ()) key.
        /// </summary>
        ParenthesisRight = 0x0029,  /* U+0029 RIGHT PARENTHESIS */

        /// <summary>
        /// The aserisk (*) key.
        /// </summary>
        Asterisk = 0x002a,  /* U+002A ASTERISK */

        /// <summary>
        /// The plus (+) key.
        /// </summary>
        Plus = 0x002b,  /* U+002B PLUS SIGN */

        /// <summary>
        /// The comma (,) key.
        /// </summary>
        Comma = 0x002c,  /* U+002C COMMA */

        /// <summary>
        /// The minus sign key.
        /// </summary>
        Minus = 0x002d,  /* U+002D HYPHEN-MINUS */

        /// <summary>
        /// The full stop (.) key.
        /// </summary>
        Period = 0x002e,  /* U+002E FULL STOP */

        /// <summary>
        /// The slash (/) key.
        /// </summary>
        Slash = 0x002f,  /* U+002F SOLIDUS */

        /// <summary>
        /// The 0 key.
        /// </summary>
        D0 = 0x0030,  /* U+0030 DIGIT ZERO */

        /// <summary>
        /// The 1 key.
        /// </summary>
        D1 = 0x0031,  /* U+0031 DIGIT ONE */

        /// <summary>
        /// The 2 key.
        /// </summary>
        D2 = 0x0032,  /* U+0032 DIGIT TWO */

        /// <summary>
        /// The 3 key.
        /// </summary>
        D3 = 0x0033,  /* U+0033 DIGIT THREE */

        /// <summary>
        /// The 4 key.
        /// </summary>
        D4 = 0x0034,  /* U+0034 DIGIT FOUR */

        /// <summary>
        /// The 5 key.
        /// </summary>
        D5 = 0x0035,  /* U+0035 DIGIT FIVE */

        /// <summary>
        /// The 6 key.
        /// </summary>
        D6 = 0x0036,  /* U+0036 DIGIT SIX */

        /// <summary>
        /// The 7 key.
        /// </summary>
        D7 = 0x0037,  /* U+0037 DIGIT SEVEN */

        /// <summary>
        /// The 8 key.
        /// </summary>
        D8 = 0x0038,  /* U+0038 DIGIT EIGHT */

        /// <summary>
        /// The 9 key.
        /// </summary>
        D9 = 0x0039,  /* U+0039 DIGIT NINE */

        /// <summary>
        /// The colon (:) key.
        /// </summary>
        Colon = 0x003a,  /* U+003A COLON */

        /// <summary>
        /// The semicolon (;) key.
        /// </summary>
        Semicolon = 0x003b,  /* U+003B SEMICOLON */

        /// <summary>
        /// The less than (&lt;) key.
        /// </summary>
        Less = 0x003c,  /* U+003C LESS-THAN SIGN */

        /// <summary>
        /// The equals (=) key.
        /// </summary>
        Equal = 0x003d,  /* U+003D EQUALS SIGN */

        /// <summary>
        /// The greather than (&gt;) key.
        /// </summary>
        Greater = 0x003e,  /* U+003E GREATER-THAN SIGN */

        /// <summary>
        /// The question mark (?) key.
        /// </summary>
        Question = 0x003f,  /* U+003F QUESTION MARK */

        /// <summary>
        /// The at (@) key.
        /// </summary>
        At = 0x0040,  /* U+0040 COMMERCIAL AT */

        /// <summary>
        /// The capital letter A key.
        /// </summary>
        A = 0x0041,  /* U+0041 LATIN CAPITAL LETTER A */

        /// <summary>
        /// The capital letter B key.
        /// </summary>
        B = 0x0042,  /* U+0042 LATIN CAPITAL LETTER B */

        /// <summary>
        /// The capital letter C key.
        /// </summary>
        C = 0x0043,  /* U+0043 LATIN CAPITAL LETTER C */

        /// <summary>
        /// The capital letter D key.
        /// </summary>
        D = 0x0044,  /* U+0044 LATIN CAPITAL LETTER D */

        /// <summary>
        /// The capital letter E key.
        /// </summary>
        E = 0x0045,  /* U+0045 LATIN CAPITAL LETTER E */

        /// <summary>
        /// The capital letter F key.
        /// </summary>
        F = 0x0046,  /* U+0046 LATIN CAPITAL LETTER F */

        /// <summary>
        /// The capital letter G key.
        /// </summary>
        G = 0x0047,  /* U+0047 LATIN CAPITAL LETTER G */

        /// <summary>
        /// The capital letter H key.
        /// </summary>
        H = 0x0048,  /* U+0048 LATIN CAPITAL LETTER H */

        /// <summary>
        /// The capital letter I key.
        /// </summary>
        I = 0x0049,  /* U+0049 LATIN CAPITAL LETTER I */

        /// <summary>
        /// The capital letter J key.
        /// </summary>
        J = 0x004a,  /* U+004A LATIN CAPITAL LETTER J */

        /// <summary>
        /// The capital letter K key.
        /// </summary>
        K = 0x004b,  /* U+004B LATIN CAPITAL LETTER K */

        /// <summary>
        /// The capital letter L key.
        /// </summary>
        L = 0x004c,  /* U+004C LATIN CAPITAL LETTER L */

        /// <summary>
        /// The capital letter M key.
        /// </summary>
        M = 0x004d,  /* U+004D LATIN CAPITAL LETTER M */

        /// <summary>
        /// The capital letter N key.
        /// </summary>
        N = 0x004e,  /* U+004E LATIN CAPITAL LETTER N */

        /// <summary>
        /// The capital letter O key.
        /// </summary>
        O = 0x004f,  /* U+004F LATIN CAPITAL LETTER O */

        /// <summary>
        /// The capital letter P key.
        /// </summary>
        P = 0x0050,  /* U+0050 LATIN CAPITAL LETTER P */

        /// <summary>
        /// The capital letter Q key.
        /// </summary>
        Q = 0x0051,  /* U+0051 LATIN CAPITAL LETTER Q */

        /// <summary>
        /// The capital letter R key.
        /// </summary>
        R = 0x0052,  /* U+0052 LATIN CAPITAL LETTER R */

        /// <summary>
        /// The capital letter S key.
        /// </summary>
        S = 0x0053,  /* U+0053 LATIN CAPITAL LETTER S */

        /// <summary>
        /// The capital letter T key.
        /// </summary>
        T = 0x0054,  /* U+0054 LATIN CAPITAL LETTER T */

        /// <summary>
        /// The capital letter U key.
        /// </summary>
        U = 0x0055,  /* U+0055 LATIN CAPITAL LETTER U */

        /// <summary>
        /// The capital letter V key.
        /// </summary>
        V = 0x0056,  /* U+0056 LATIN CAPITAL LETTER V */

        /// <summary>
        /// The capital letter W key.
        /// </summary>
        W = 0x0057,  /* U+0057 LATIN CAPITAL LETTER W */

        /// <summary>
        /// The capital letter X key.
        /// </summary>
        X = 0x0058,  /* U+0058 LATIN CAPITAL LETTER X */

        /// <summary>
        /// The capital letter Y key.
        /// </summary>
        Y = 0x0059,  /* U+0059 LATIN CAPITAL LETTER Y */

        /// <summary>
        /// The capital letter Z key.
        /// </summary>
        Z = 0x005a,  /* U+005A LATIN CAPITAL LETTER Z */

        /// <summary>
        /// The left square bracket ([) key.
        /// </summary>
        BracketLeft = 0x005b,  /* U+005B LEFT SQUARE BRACKET */

        /// <summary>
        /// The backslash (\) key.
        /// </summary>
        Backslash = 0x005c,  /* U+005C REVERSE SOLIDUS */

        /// <summary>
        /// The right square bracket (]) key.
        /// </summary>
        Bracketright = 0x005d,  /* U+005D RIGHT SQUARE BRACKET */

        /// <summary>
        /// The cirumflex accent (^) key.
        /// </summary>
        AsciiCircum = 0x005e,  /* U+005E CIRCUMFLEX ACCENT */

        /// <summary>
        /// The underscore (_) key.
        /// </summary>
        Underscore = 0x005f,  /* U+005F LOW LINE */

        /// <summary>
        /// The grave accent (`) sign.
        /// </summary>
        Grave = 0x0060,  /* U+0060 GRAVE ACCENT */

        /// <summary>
        /// The small letter a key.
        /// </summary>
        a = 0x0061,  /* U+0061 LATIN SMALL LETTER A */

        /// <summary>
        /// The small letter b key.
        /// </summary>
        b = 0x0062,  /* U+0062 LATIN SMALL LETTER B */

        /// <summary>
        /// The small letter c key.
        /// </summary>
        c = 0x0063,  /* U+0063 LATIN SMALL LETTER C */

        /// <summary>
        /// The small letter d key.
        /// </summary>
        d = 0x0064,  /* U+0064 LATIN SMALL LETTER D */

        /// <summary>
        /// The small letter e key.
        /// </summary>
        e = 0x0065,  /* U+0065 LATIN SMALL LETTER E */

        /// <summary>
        /// The small letter f key.
        /// </summary>
        f = 0x0066,  /* U+0066 LATIN SMALL LETTER F */

        /// <summary>
        /// The small letter g key.
        /// </summary>
        g = 0x0067,  /* U+0067 LATIN SMALL LETTER G */

        /// <summary>
        /// The small letter h key.
        /// </summary>
        h = 0x0068,  /* U+0068 LATIN SMALL LETTER H */

        /// <summary>
        /// The small letter i key.
        /// </summary>
        i = 0x0069,  /* U+0069 LATIN SMALL LETTER I */

        /// <summary>
        /// The small letter j key.
        /// </summary>
        j = 0x006a,  /* U+006A LATIN SMALL LETTER J */

        /// <summary>
        /// The small letter k key.
        /// </summary>
        k = 0x006b,  /* U+006B LATIN SMALL LETTER K */

        /// <summary>
        /// The small letter l key.
        /// </summary>
        l = 0x006c,  /* U+006C LATIN SMALL LETTER L */

        /// <summary>
        /// The small letter m key.
        /// </summary>
        m = 0x006d,  /* U+006D LATIN SMALL LETTER M */

        /// <summary>
        /// The small letter n key.
        /// </summary>
        n = 0x006e,  /* U+006E LATIN SMALL LETTER N */

        /// <summary>
        /// The small letter o key.
        /// </summary>
        o = 0x006f,  /* U+006F LATIN SMALL LETTER O */

        /// <summary>
        /// The small letter p key.
        /// </summary>
        p = 0x0070,  /* U+0070 LATIN SMALL LETTER P */

        /// <summary>
        /// The small letter q key.
        /// </summary>
        q = 0x0071,  /* U+0071 LATIN SMALL LETTER Q */

        /// <summary>
        /// The small letter r key.
        /// </summary>
        r = 0x0072,  /* U+0072 LATIN SMALL LETTER R */

        /// <summary>
        /// The small letter s key.
        /// </summary>
        s = 0x0073,  /* U+0073 LATIN SMALL LETTER S */

        /// <summary>
        /// The small letter t key.
        /// </summary>
        t = 0x0074,  /* U+0074 LATIN SMALL LETTER T */

        /// <summary>
        /// The small letter u key.
        /// </summary>
        u = 0x0075,  /* U+0075 LATIN SMALL LETTER U */

        /// <summary>
        /// The small letter v key.
        /// </summary>
        v = 0x0076,  /* U+0076 LATIN SMALL LETTER V */

        /// <summary>
        /// The small letter w key.
        /// </summary>
        w = 0x0077,  /* U+0077 LATIN SMALL LETTER W */

        /// <summary>
        /// The small letter x key.
        /// </summary>
        x = 0x0078,  /* U+0078 LATIN SMALL LETTER X */

        /// <summary>
        /// The small letter y key.
        /// </summary>
        y = 0x0079,  /* U+0079 LATIN SMALL LETTER Y */

        /// <summary>
        /// The small letter z key.
        /// </summary>
        z = 0x007a,  /* U+007A LATIN SMALL LETTER Z */

        /// <summary>
        /// The left curly bracket ({) key.
        /// </summary>
        BraceLeft = 0x007b,  /* U+007B LEFT CURLY BRACKET */

        /// <summary>
        /// The vertical bar (|) key.
        /// </summary>
        Bar = 0x007c,  /* U+007C VERTICAL LINE */

        /// <summary>
        /// The right curly bracket (}) key.
        /// </summary>
        BraceRight = 0x007d,  /* U+007D RIGHT CURLY BRACKET */

        /// <summary>
        /// The tilde (~) key.
        /// </summary>
        AsciiTilde = 0x007e,  /* U+007E TILDE */
    }
}
