﻿using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace gESilk.engine.misc;

internal static class GlDebug
{
    private static readonly DebugProc Dp = Debug;

    public static void Init()
    {
        GL.DebugMessageCallback(Dp, IntPtr.Zero);
        GL.Enable(EnableCap.DebugOutput);
    }

    private static void Debug(DebugSource source, DebugType type, int id, DebugSeverity severity, int length,
        IntPtr message, IntPtr userParam)
    {
        if (severity is not DebugSeverity.DebugSeverityHigh) Console.Write(Marshal.PtrToStringAnsi(message, length));
        Console.BackgroundColor = ConsoleColor.Red;
        if (severity is DebugSeverity.DebugSeverityHigh) throw new Exception(Marshal.PtrToStringAnsi(message, length));
        Console.BackgroundColor = default;
    }
}