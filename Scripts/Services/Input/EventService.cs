using System;
using Godot;

namespace Grate.Services.Input
{
    public interface IEventService : IService
    {
        event Action<InputEventMouseMotion> MouseMoved;
        event Action<InputEventMouseButton> MouseClicked;
    }

    public class EventService : IEventService
    {
        public EventService(EventListener eventListener)
        {
            eventListener.UnhandledEventOccured += ProcessEvent;
        }

        public event Action<InputEventMouseMotion> MouseMoved;
        public event Action<InputEventMouseButton> MouseClicked;

        private void ProcessEvent(InputEvent inputEvent)
        {
            switch (inputEvent)
            {
                case InputEventMouseMotion e:
                    MouseMoved?.Invoke(e);
                    break;
                case InputEventMouseButton e:
                    MouseClicked?.Invoke(e);
                    break;
            }
        }
    }
}
