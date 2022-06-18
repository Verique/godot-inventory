using System;
using Godot;

namespace Grate.Input
{
    public interface IInputEvents<T>
    {
        event Action<T> OnLeftMouseUp;
        event Action<T, InputEventMouseMotion> OnMouseMoved;
    }

    public interface IHasInputEvents<T> where T : class
    {
        InputEvents<T> InputEvents { get; }
    }

    public class InputEvents<T> : Godot.Object, IInputEvents<T> where T : class
    {
        public event Action<T, InputEventMouseMotion> OnMouseMoved;
        public event Action<T> OnLeftMouseUp;

        private Node _node;

        public InputEvents(Node node)
        {
            _node = node;
            _node.Connect("input_event", this, nameof(ProcessInputEvent));
        }

        private void ProcessInputEvent(Node viewport, InputEvent e, int shapeIdx)
        {
            switch (e)
            {
                case InputEventMouseMotion mouseMotion:
                    ProcessMouseMotion(mouseMotion);
                    break;
                case InputEventMouseButton mouseClick:
                    ProcessMouseClick(mouseClick);
                    break;
            }
            viewport.GetTree().SetInputAsHandled();
        }

        private void ProcessMouseClick(InputEventMouseButton mouseClick)
        {
            if (mouseClick.ButtonIndex == (int)ButtonList.Left
                    && mouseClick.Pressed == false)
            {
                OnLeftMouseUp?.Invoke(_node as T);
            }
        }

        private void ProcessMouseMotion(InputEventMouseMotion mouseMotion)
        {
            OnMouseMoved?.Invoke(_node as T, mouseMotion);
        }
    }
}
