using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Upsilon.Tools.Updater
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                throw new IOException($"Missing argument : 'args[0] - ProcessName'.");
            }
            string processName = args[0];

            if (args.Length < 2)
            {
                throw new IOException($"Missing argument : 'args[1] - WorkingDirectory'.");
            }
            string workingDirectory = args[1];

            if (args.Length < 3)
            {
                throw new IOException($"Missing argument : 'args[2] - UpdateDirectory'.");
            }
            string updateDirectory = args[2];

            if (args.Length < 4)
            {
                throw new IOException($"Missing argument : 'args[3] - MainExecutable'.");
            }
            string mainExecutable = args[3];

            int timeout = 0;
            if (args.Length > 5)
            {
                int.TryParse(args[4], out timeout);
            }

            int delay = 0;
            var processes = Process.GetProcessesByName(processName);
            while ((timeout == 0 || delay < timeout)
                && processes.Length != 0)
            {
                Thread.Sleep(500);
                delay += 500;
                processes = Process.GetProcessesByName(processName);
            }

            foreach (var process in processes)
            {
                process.Kill();
            }

            string[] toMove = Directory.GetFiles(updateDirectory)
                .Union(Directory.GetDirectories(updateDirectory))
                .ToArray();

            foreach (var path in toMove)
            {
                _move(path, workingDirectory);
            }

            Directory.Delete(updateDirectory, true);

            Process.Start(mainExecutable);
        }

        private static void _move(string sourcePath, string destinationDirectory)
        {
            if (File.Exists(sourcePath))
            {
                Directory.CreateDirectory(destinationDirectory);

                try
                {
                    File.Move(sourcePath, Path.Combine(destinationDirectory, Path.GetFileName(sourcePath)), true);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    throw;
                }
            }
            else if (Directory.Exists(sourcePath))
            {
                DirectoryInfo dir = new(sourcePath);

                string[] dirs = dir.GetDirectories().Select(x => x.FullName).ToArray();
                string[] files = dir.GetFiles().Select(x => x.FullName).ToArray();

                destinationDirectory = Path.Combine(destinationDirectory, Path.GetFileName(sourcePath));
                Directory.CreateDirectory(destinationDirectory);

                foreach (string source in dirs.Union(files))
                {
                    _move(source, destinationDirectory);
                }
            }
            else
            {
                throw new IOException($"'{sourcePath}' does not exists.");
            }
        }
    }
}
