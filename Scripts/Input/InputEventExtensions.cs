using System;
using Godot;

namespace Grate.CustomInput
{
    public static class InputEventExtensions
    {
        public static void Process(
                this InputEvent e,
                Action<InputEventMouseButton>? onLeftMouseButtonUp = null,
                Action<InputEventMouseMotion>? onMouseMove = null
        )
        {
            switch (e)
            {
                case InputEventMouseMotion mm:
                    onMouseMove?.Invoke(mm);
                    return;
                case InputEventMouseButton mb:
                    if (mb.IsLeftMouseUp())
                        onLeftMouseButtonUp?.Invoke(mb);
                    return;
                default:
                    return;
            }
        }

        public static bool IsLeftMouseUp(this InputEventMouseButton e) =>
            !e.Pressed && e.ButtonIndex == MouseButton.Left;
    }
}
