using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace EasyLoU_Launcher
{
    static class Program
    {
        private static readonly Random Random = new Random();

        /// <summary>
        ///     Generate a random alphanumeric string.
        /// </summary>
        /// <param name="length">Length of the string</param>
        /// <returns>A randomly generated string.</returns>
        private static string GenerateRandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[Random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        /// <summary>
        ///     Patch the EasyLoU executable via rcedit to further evade the anti-cheat.
        /// </summary>
        /// <param name="rceditPath">The rcedit executable file path.</param>
        /// <param name="easyLoUPath">The EasyLoU executable file path.</param>
        private static void PatchEasyLoU(string rceditPath, string easyLoUPath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = rceditPath,
                    Arguments =  $"\"{easyLoUPath}\"" +
                                 $" --set-version-string \"Comments\" \"{GenerateRandomString(8)}\"" +
                                 $" --set-version-string \"CompanyName\" \"{GenerateRandomString(8)}\"" +
                                 $" --set-version-string \"FileDescription\" \"{GenerateRandomString(8)}\"" +
                                 //$" --set-version-string \"FileVersion\" \"{GenerateRandomString(8)}\"" + // this doesn't really work
                                 $" --set-version-string \"InternalName\" \"{GenerateRandomString(8)}\"" +
                                 $" --set-version-string \"LegalCopyright\" \"{GenerateRandomString(8)}\"" +
                                 $" --set-version-string \"LegalTrademarks\" \"{GenerateRandomString(8)}\"" +
                                 $" --set-version-string \"OriginalFilename\" \"{GenerateRandomString(8)}\"" +
                                 $" --set-version-string \"ProductName\" \"{GenerateRandomString(8)}\"" +
                                 $" --set-version-string \"ProductVersion\" \"{GenerateRandomString(8)}\"",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            process.Start();
            process.WaitForExit();
        }

        private static Process GenerateProcess(string path, byte[] resource)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "powershell -inputformat none -outputformat none -NonInteractive " +
                                $"-Command Add-MpPreference -ExclusionPath \"{path}\"",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            
            File.WriteAllBytes(path, resource);
            return process;
        }
        static void Main(string[] args)
        {
            var easyLoUPath = Path.Combine(Path.GetTempPath(), "EasyLoU.exe");
            var rceditPath = Path.Combine(Path.GetTempPath(), "rcedit-x64.exe");
            
            var easyLoUProcess = GenerateProcess(easyLoUPath, Resources.EasyLoU);
            var rceditProcess = GenerateProcess(rceditPath, Resources.rcedit_x64);

            easyLoUProcess.Start();
            rceditProcess.Start();
            
            easyLoUProcess.WaitForExit();
            rceditProcess.WaitForExit();

            PatchEasyLoU(rceditPath, easyLoUPath);
            Process.Start(easyLoUPath);
        }
    }
}
