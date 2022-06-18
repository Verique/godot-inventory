
using System;
using Godot;
using Grate.Services.Physics;

namespace Grate.Services.Input
{
    public interface IInputService : IService
    {
        event Action<Vector2> MouseMoved;
        event Action<Vector2> LeftMouseUp;
        event Action<Vector2, CollisionObject2D> LeftMouseUpOn;
        // TODO : Add clicks on colliders;
    }

    public class InputService : IInputService
    {
        public event Action<Vector2> MouseMoved;
        public event Action<Vector2> LeftMouseUp;
        public event Action<Vector2, CollisionObject2D> LeftMouseUpOn;

        private readonly IPhysicsService _physicsService;

        public InputService(IPhysicsService physicsService, IEventService eventService)
        {
            eventService.MouseMoved += ProcessMouseMotion;
            eventService.MouseClicked += ProcessMouseClick;
            _physicsService = physicsService;
        }

        private void ProcessMouseMotion(InputEventMouseMotion mouseMotion)
        {
            MouseMoved?.Invoke(mouseMotion.Position);
        }

        private void ProcessMouseClick(InputEventMouseButton mouseClick)
        {
            if (mouseClick.ButtonIndex == (int)ButtonList.Left)
            {
                if (!mouseClick.IsPressed())
                {
                    LeftMouseUp?.Invoke(mouseClick.Position);
                    GD.Print(_physicsService.TryGetCollisionObjectAtPoint(mouseClick.Position).GetPath());
                    LeftMouseUpOn?.Invoke(mouseClick.Position, _physicsService.TryGetCollisionObjectAtPoint(mouseClick.Position));
                }
            }
        }
    }
}
