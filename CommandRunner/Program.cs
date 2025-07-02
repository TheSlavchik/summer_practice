using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    public class Program
    {
        static void Main()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

            Assembly assembly = Assembly.LoadFrom("FileSystemCommands.dll");

            var commandTypes = assembly.GetTypes().Where(t => typeof(ICommand).IsAssignableFrom(t));
            string mask = "*.txt";
            object[] args = [testDir, mask];
            ICommand? command;

            foreach (var commandType in commandTypes)
            {
                if (commandType.Name == "DirectorySizeCommand")
                {
                    command = (ICommand?)Activator.CreateInstance(commandType, args[0]);
                }
                else if (commandType.Name == "FindFilesCommand")
                {
                    command = (ICommand?)Activator.CreateInstance(commandType, args);
                }
                else
                {
                    return;
                }

                if (command != null)
                {
                    command.Execute();
                }
                else
                {
                    Console.WriteLine("Null Command");
                }
            }

            Directory.Delete(testDir, true);
        }
    }
}
