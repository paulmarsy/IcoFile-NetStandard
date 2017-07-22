using System.IO;

namespace  Icons.NetStandard.FolderCustomisation
{
    public class WindowsFolderCustomizer : IFolderCustomisation
    {
        public const string FolderCustomizationFile = "desktop.ini";

        public void SetFolderIcon(string folderPath, string iconPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);
            if (!File.Exists(iconPath))
                throw new FileNotFoundException(iconPath);

            ResetFileSystemObject(folderPath);
            var desktopIniFile = Path.Combine(folderPath, FolderCustomizationFile);
            if (File.Exists(desktopIniFile))
            {
                ResetFileSystemObject(desktopIniFile);
                File.Delete(desktopIniFile);
            }

            File.WriteAllText(desktopIniFile, $"[.ShellClassInfo]\r\nConfirmFileOp=0\r\nIconFile={iconPath}\r\nIconIndex=0\r\nIconResource={iconPath},0");

            File.SetAttributes(folderPath, FileAttributes.System);
            File.SetAttributes(desktopIniFile, FileAttributes.System | FileAttributes.Hidden);
        }

        private static void ResetFileSystemObject(string path)
        {
            var attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.System) || attributes.HasFlag(FileAttributes.ReadOnly) || attributes.HasFlag(FileAttributes.Hidden))
                File.SetAttributes(path, attributes & ~FileAttributes.System & ~FileAttributes.ReadOnly & ~FileAttributes.Hidden);
        }
    }
}