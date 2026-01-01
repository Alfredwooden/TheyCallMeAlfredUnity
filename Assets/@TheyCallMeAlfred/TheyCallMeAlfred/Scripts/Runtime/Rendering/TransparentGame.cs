using System;
using System.Runtime.InteropServices;
using UnityEngine;

/*
    How to make a window transparent in Unity:
    1 - Add the script to an object in the scene
    2 - Camera with solid color and alpha set to 0, Post processing and HDR rendering disabled
    3 - In player project settings, set the fullscreenmode to Windowed and disable "Use DXGI flip model swapchain for d3d11"
    4 - In other settings disable the Auto Graphics API for windows and move the Direct3D11 out of the list
*/
namespace TheyCallMeAlfredUnity.Rendering
{
    public class TransparentGame : MonoBehaviour
    {
        //#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

        private const int GWL_STYLE = -16;
        private const int WS_BORDER = 0x00800000;
        private const int WS_DLGFRAME = 0x00400000;
        private const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

        void Start()
        {
            IntPtr hwnd = GetActiveWindow();
            int style = GetWindowLong(hwnd, GWL_STYLE);
            style &= ~WS_CAPTION;
            SetWindowLong(hwnd, GWL_STYLE, style);
            var margins = new MARGINS
            {
                cxLeftWidth = -1,
                cxRightWidth = 0,
                cyTopHeight = 0,
                cyBottomHeight = 0
            };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }

        //#endif
    }
}
