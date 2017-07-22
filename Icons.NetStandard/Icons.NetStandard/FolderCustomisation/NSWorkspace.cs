using System.Runtime.InteropServices;

namespace  Icons.NetStandard.FolderCustomisation
{
    internal static class NSWorkspace
    {
        private const string NSWorkspaceBridge = "NSWorkspaceBridge.dylib";

        [DllImport(NSWorkspaceBridge, EntryPoint = nameof(NSWorkspace) + nameof(SetIconForFile))]
        internal static extern bool SetIconForFile([In] string setIcon, [In] string forFile);
    }
}