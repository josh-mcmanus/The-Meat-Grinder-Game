using Godot;
using System;

public partial class ConnectiveTissue : Node2D
{
	[Export] Marker2D[] StartPoints;
	[Export] Marker2D[] EndPoints;
	
	[Export] private float TissueWidth = 2f;
	[Export] private float StartDistance = 5f;
	[Export] private Color TissueColor = Colors.DarkRed;
	
	private Line2D[] lines;
	private int numLines;
	
	private RandomNumberGenerator rng;
	
    public override void _Ready()
    {
		numLines = Math.Min(StartPoints.Length, EndPoints.Length);
		
        lines = new Line2D[numLines];
		
		rng = new RandomNumberGenerator();
		
		for (int i = 0; i < numLines; i++)
		{
            Line2D line = new Line2D
            {
                Width = TissueWidth,
                DefaultColor = TissueColor
            };
			
            AddChild(line);
			lines[i] = line;
			
		}
		
    }
	
	
    public override void _Process(double delta)
    {
        for (int i = 0; i < numLines; i++)
		{
			lines[i].Visible = true;
			
			Vector2 startPos = StartPoints[i].GlobalPosition;
			Vector2 endPos   = EndPoints[i].GlobalPosition;
			
			Vector2 localStart = ToLocal(startPos);
			Vector2 localEnd   = ToLocal(endPos);
			
			lines[i].Points = new Vector2[]{localStart, localEnd};
		}
    }
	
}
