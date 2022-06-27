using Godot;
using Grate.Services;
using Grate.Services.Physics;
using Grate.Services.Input;

public class Game : Node2D
{
    private IServiceLocator serviceLocator;

    public override void _EnterTree()
    {
        serviceLocator = Services.CreateLocator(locator => locator
            .WithService<IPhysicsService, PhysicsService>(new PhysicsServiceParameters(){
                World2d = GetWorld2d()
            })
            .WithService<IInputService, InputService>()
            .WithService<IEventService, EventService>(new EventServiceParameters(){
                EventListener = GetNode<EventListener>("EventListener")
            })
        );
    }

    public override void _Ready()
    {
    }
}
