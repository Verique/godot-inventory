using System;
using Godot;

namespace Grate.Services.Input
{
    public class EventListener : Node2D
    {
        public event Action<InputEvent> UnhandledEventOccured;

        public override void _UnhandledInput(InputEvent e)
        {
            UnhandledEventOccured?.Invoke(e);
        }
    }
}
