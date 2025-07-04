using CommandRunner;

namespace task08tests
{
    public class FileSystemCommandsTests
    {
        [Fact]
        public void DirectorySizeCommand_ShouldCalculateSize()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
            File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");

            var command = new DirectorySizeCommand(testDir);
            command.Execute();
            Assert.Equal(10, command.Size);

            Directory.Delete(testDir, true);
        }

        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");

            var command = new FindFilesCommand(testDir, "*.txt");
            command.Execute();
            Assert.Single(command.FoundFiles);
            Assert.Contains("file1.txt", command.FoundFiles[0]);

            Directory.Delete(testDir, true);
        }

        [Fact]
        public void CommandRunner_PrintsCorrectInfo()
        {
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);
            
            Program.Main();

            string[] result = writer.ToString().Split("\n\r ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal(2, result.Length);
            Assert.Equal("7", result[0]);
            Assert.Equal(Path.Combine(Path.GetTempPath(), "TestDir", "file1.txt"), result[1]);
        }
    }
}
