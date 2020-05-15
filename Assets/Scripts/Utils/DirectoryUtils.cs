using System.IO;

public static class DirectoryUtils
{
    public static string GetPreviousDirectory(string directory)
    {
        int lastFolderIndex = directory.LastIndexOf(Path.DirectorySeparatorChar);
        if (lastFolderIndex > 0 && lastFolderIndex < directory.Length)
        {
            return directory.Substring(0, lastFolderIndex);
        }
        return string.Empty;
    }

    public static string GetDirectoryShortName(string fullPath)
    {
        int lastFolderIndex = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
        if (lastFolderIndex >= 0)
        {
            return fullPath.Substring(lastFolderIndex + 1);
        }

        return string.Empty;
    }
}
