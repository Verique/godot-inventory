using Godot;
using Grate.Services;
using Grate.Inventory;
using Grate.Services.Physics;
using Grate.Services.Input;

public class Game : Node2D
{
    private IServiceLocator serviceLocator;

    public override void _EnterTree()
    {
        serviceLocator = Services.CreateLocator(locator => locator
            .WithService<IPhysicsService, PhysicsService>()
            .WithService<IInputService, InputService>()
            .WithService<IItemPickingService, ItemPickingService>()
            .WithService<IEventService, EventService>()
            .WithParameter(GetNode<EventListener>("EventListener"))
            .WithParameter(GetWorld2d())
        );
    }

    public override void _Ready()
    {
        // test
        serviceLocator.Get<IItemPickingService>().RegisterItem(GetNode<IInventoryItem>("/root/Game/Inventory/InverntoryGrid/InventoryItem"));
        serviceLocator.Get<IItemPickingService>().RegisterItem(GetNode<IInventoryItem>("/root/Game/Inventory/InverntoryGrid/InventoryItem2"));
        serviceLocator.Get<IItemPickingService>().RegisterItem(GetNode<IInventoryItem>("/root/Game/Inventory/InventoryItem"));
    }
}
