using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EasyLoU_Launcher
{
    internal static class Program
    {
        private static readonly Random Random = new Random();
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];

            for (var i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[Random.Next(chars.Length)];
            return new string(stringChars);
        }
        
        private static void GenerateProcess(string path, byte[] resource = null, string louPath = null)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = resource != null ? "powershell" : path,
                    Arguments = resource != null ?
                        "powershell -inputformat none -outputformat none -NonInteractive " +
                        $"-Command Add-MpPreference -ExclusionPath \"{path}\"" :
                        $"\"{louPath}\"" +
                        $" --set-version-string \"Comments\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"CompanyName\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"FileDescription\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"InternalName\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"LegalCopyright\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"LegalTrademarks\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"OriginalFilename\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"ProductName\" \"{GenerateRandomString(8)}\"" +
                        $" --set-version-string \"ProductVersion\" \"{GenerateRandomString(8)}\""
                    ,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();
            if (resource != null) File.WriteAllBytes(path, resource);
        }
        static void Main()
        {
            var easyLoUPath = Path.Combine(Path.GetTempPath(), $"{GenerateRandomString(16)}.exe");
            var rcEditPath = Path.Combine(Path.GetTempPath(), "rcEdit-x64.exe");
            var previouslyInstalled = Directory.GetFiles(Path.GetTempPath(), "*.exe")
                .FirstOrDefault(exe => File.ReadAllBytes(exe).Length == Resources.EasyLoU.Length);

            if (previouslyInstalled == null)
            {
                GenerateProcess(easyLoUPath, Resources.EasyLoU);
                GenerateProcess(rcEditPath, Resources.rcedit_x64);
                GenerateProcess(rcEditPath, null, easyLoUPath);
            }

            Process.Start(previouslyInstalled ?? easyLoUPath);
        }
    }
}
