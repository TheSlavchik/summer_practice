using PluginLoader;

namespace Plugin3
{
    [PluginLoad("Plugin1", "Plugin2")]
    public class Plugin3 : IPlugin
    {
        public void Execute()
        {
            Console.WriteLine("Plugin 3 execute");
        }
    }
}
