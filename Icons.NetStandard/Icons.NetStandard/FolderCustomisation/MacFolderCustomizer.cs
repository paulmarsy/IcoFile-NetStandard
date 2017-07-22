using System.IO;

namespace  Icons.NetStandard.FolderCustomisation
{
    public class MacFolderCustomizer : IFolderCustomisation
    {
        public void SetFolderIcon(string folderPath, string iconPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);
            if (!File.Exists(iconPath))
                throw new FileNotFoundException(iconPath);

            var success = NSWorkspace.SetIconForFile(iconPath, folderPath);
            if (!success)
                throw new IOException(nameof(NSWorkspace) + nameof(SetFolderIcon));
        }
    }
}