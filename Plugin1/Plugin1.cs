using PluginLoader;

namespace Plugin1
{
    [PluginLoad]
    public class Plugin1 : IPlugin
    {
        public void Execute()
        {
            Console.WriteLine("Plugin 1 execute");
        }
    }
}
