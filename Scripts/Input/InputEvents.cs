using Godot;

namespace Grate.Input
{
    public static class InputEvents {
        public static bool IsLeftMouseUp(this InputEventMouseButton e) =>
            !e.Pressed && e.ButtonIndex == (int)ButtonList.Left;
    }
}
