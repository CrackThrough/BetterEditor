using static UnityModManagerNet.UnityModManager;

namespace BetterEditor
{
    static class Startup
    {
        static void Load(ModEntry modEntry)
        {
            BetterEditor.Init(modEntry);
        }
    }
}