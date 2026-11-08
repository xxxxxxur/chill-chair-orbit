using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ChillChairOrbit
{
    internal static class MiddleMouse
    {
        private const int MiddleButton = 0x04;
        private static readonly uint GameProcessId = (uint)Process.GetCurrentProcess().Id;

        internal static bool TryRead(out Vector2 mouse)
        {
            mouse = default;
            if ((GetAsyncKeyState(MiddleButton) & 0x8000) == 0 || !GetCursorPos(out Point cursor))
                return false;

            // Restrict polling to the game's visible window, including when Unity has no focus.
            var window = WindowFromPoint(cursor);
            if (window == IntPtr.Zero || GetWindowThreadProcessId(window, out uint processId) == 0 ||
                processId != GameProcessId)
                return false;

            mouse = new Vector2(cursor.X, -cursor.Y);
            return true;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int key);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);
    }
}
