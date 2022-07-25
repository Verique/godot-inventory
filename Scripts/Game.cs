using Godot;
using Grate.Services;
using Grate.Services.Physics;
using Grate.Inventory;

public class Game : Node2D
{
    private IServiceLocator serviceLocator;

    public override void _EnterTree()
    {
        serviceLocator = Services.CreateLocator(locator => locator
            .WithOption(GetWorld2d())
            .WithOption(GetNode<InventoryNode>("GUICanvas/Inventory"))
            .WithOption(GetNode<Button>("GUICanvas/Button"))

            .WithService<IPhysicsService, PhysicsService>()
            .WithService<IInventoryService, InventoryService>()
        );
    }
}
