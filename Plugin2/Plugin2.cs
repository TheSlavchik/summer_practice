using PluginLoader;

namespace Plugin2
{
    [PluginLoad("Plugin1")]
    public class DatabasePlugin : IPlugin
    {
        public void Execute()
        {
            Console.WriteLine("Plugin 2 execute");
        }
    }
}
