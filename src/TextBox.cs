using System.Numerics;
using Raylib_cs;

namespace Packing;

public class TextBox
{
    private int key;
    
    public string output = "";
    private char[] outputkeys;
    public int letters;
    
    public Rectangle placement;

    private float thick;
    public int size;

    public bool disabled = false;
    public bool angry = false;

    public TextBox(Rectangle placement, float thick, int size)
    {
        this.placement = placement;
        this.thick = thick;
        outputkeys = new char[100];
        this.size = size;
        //thick = placement.width * placement.height / 500;
    }

    public void Play()
    {
        key = Raylib.GetCharPressed();
    }
    
    public void Work()
    {
        if (disabled)
        {
            output = "/";
            letters = 0;
            return;
        }
        bool outofrange = Raylib.MeasureText(output, size) > placement.width - 80;
        
        while (key > 0)
        {
            if ((key >= 32) && (key <= 252) && !outofrange)
            {
                outputkeys[letters] = (char)key;
                letters++;
            }
            key = Raylib.GetCharPressed();
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
        {
            letters -= 1;
            if (letters < 0)
            {
                letters = 0;
            }
            outputkeys[letters] = '\0';
        }

        /*
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && letters != 0)
        {
            Delete();
        }
        */
        output = new string(outputkeys).Replace("\u0000", string.Empty);
    }
    
    public void Draw(Color colour, Color disabledcolour)
    {

        if (!angry)
        {
            Raylib.DrawRectangleRoundedLines(placement, 0.3f,2,thick, colour);
        }
        else
        {
            Raylib.DrawRectangleRoundedLines(placement, 0.3f,2,thick, disabledcolour);
        }
        Raylib.DrawText(output, (int)(placement.x + placement.width / 2 - Raylib.MeasureText(output, size) / 2), (int)(placement.y + placement.height / 2 - size / 2.1f), size, colour);

        int countsize = 30;
        
        Raylib.DrawText(letters.ToString(), (int)(placement.x + countsize / 1.5f - Raylib.MeasureText(letters.ToString(), countsize) / 2),(int)(placement.y + countsize / 3), countsize, colour);
    }

    public void Delete()
    {
        letters = 0;
        outputkeys = new char[100];
    }
}