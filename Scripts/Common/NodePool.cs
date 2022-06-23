using System;
using System.Collections.Generic;
using Godot;

namespace Grate.Pooling
{
    public interface IPoolableNode {
        event Action<Node> MarkToDespawn;
    }

    public class NodePool<T> where T : Node, IPoolableNode
    {
        private int _size;
        private PackedScene _packedScene;
        private LinkedList<T> _pool;
        private Node _parent;

        public NodePool(PackedScene packedScene, int size, Node owner)
        {
            _pool = new LinkedList<T>();
            _packedScene = packedScene;
            _size = size;

            CreatePoolNode(owner);
            InitializePool();
        }

        public T Spawn()
        {
            var node = _pool.First;
            _pool.RemoveFirst();

            _parent.AddChild(node.Value);

            _pool.AddLast(node);

            return node.Value;
        }

        public void Despawn(T node)
        {
            var nodeToDelete = _parent.GetNodeOrNull<T>(node.GetPath());

            if (nodeToDelete != null) _parent.RemoveChild(node);
            else throw new Exception("Node you're trying to despawn isn't in pool");
            
            var success = _pool.Remove(node);

            if (!success) throw new Exception("Removing from pool went wrong");

            _pool.AddFirst(node);
        }

        private void CreatePoolNode(Node owner)
        {
            _parent = new Node();
            _parent.Name = "pool";
            owner.AddChild(_parent);
        }

        private void InitializePool()
        {
            for (int i = 0; i < _size; i++)
            {
                var node = _packedScene.Instance() as T;
                node.MarkToDespawn += n => Despawn(n as T);
                _pool.AddLast(node);
            }
        }
    }
}

