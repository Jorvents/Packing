using System.Numerics;
using Packing;
using Raylib_cs;

namespace Packing;

public class Lesson
{
    public string tag;
    public string lessonname;
    public Color colour;
    public Vector2 loc;

    public Lesson(string tag, Color colour)
    {
        this.tag = tag;
        this.colour = colour;
    }

    public Lesson()
    {
        tag = "";
        lessonname = "";
        colour = Color.BLANK;
    }
}