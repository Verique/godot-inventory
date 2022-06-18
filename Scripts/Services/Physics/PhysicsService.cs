using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Grate.Services.Physics
{
    public interface IPhysicsService : IService
    {
        CollisionObject2D TryGetCollisionObjectAtPoint(Vector2 point);
    }

    public class PhysicsService : IPhysicsService
    {
        private World2D _world;

        public PhysicsService(World2D world)
        {
            _world = world;
        }

        public CollisionObject2D TryGetCollisionObjectAtPoint(Vector2 point)
        {
            Godot.Collections.Array godotArr = _world.DirectSpaceState.IntersectPoint(point);

            List<CollisionObject2D> hitObjs = new List<CollisionObject2D>();
            foreach (var obj in godotArr)
            {
                var dict = obj as Godot.Collections.Dictionary;
                var item = dict["collider"] as CollisionObject2D;
                hitObjs.Add(item);
                GD.Print($"{item.ZIndex} - {item.GetIndex()}");
            }

            return hitObjs.OrderBy(o => o.ZIndex).ThenBy(o => o.GetIndex()).FirstOrDefault();
        }
    }
}
