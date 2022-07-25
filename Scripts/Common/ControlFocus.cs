using Godot;

namespace Grate.Utils
{
    public class ControlFocus : Reference
    {
        public bool IsMouseOver { get; private set; }

        public ControlFocus(Control node)
        {
            node.Connect("mouse_entered", this, nameof(Focus));
            node.Connect("mouse_exited", this, nameof(Unfocus));
        }

        private void Focus() => IsMouseOver = true;
        private void Unfocus() => IsMouseOver = false;
    }
}
