using Godot;

public class TestNode : TextureRect
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    public TestNode(string ss){

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("gui_input", this, "Process");
    }

    private void Process(InputEvent e)
    {
        if (e is InputEventMouseButton a
                && a.IsPressed() == false)
            GD.Print(e.AsText());
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
