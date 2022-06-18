using Godot;

namespace Grate.Utils
{
    public static class GodotConsole
    {
        public static void PrintInfo(object message)
        {
            GD.Print($"<INFO> : {message}");
        }
        public static void PrintError(object message)
        {
            GD.PushError($"<ERROR> : {message}");
        }
        public static void PrintWarning(object message)
        {
            GD.PushWarning($"<WARNING> : {message}");
        }
    }
}
