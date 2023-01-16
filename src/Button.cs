using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace Packing
{
    public class Button
    {
        public Rectangle box;
        public bool isPressed = false;
        public bool isHovered = false;
        
        int size;
        Texture2D icon;
        Vector2 iconsize;
        float rotation;

        private Sound presssound;
        
        public Button(Rectangle box, Texture2D icon, Vector2 iconsize, float rotation)
        {
            this.box = box;
            this.icon = icon;
            this.iconsize = iconsize;
            this.rotation = rotation;
            presssound = Raylib.LoadSound("assets/button.wav");
        }
        public void Play()
        {
            isPressed = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), box) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
            isHovered = Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), box);
            if (isPressed)
            {
                Raylib.PlaySound(presssound);
            }
        }
        public void Draw(Color colour,Color iconcolour,float thick)
        {
            Raylib.DrawRectangleRoundedLines(box, 0.07f,2,thick, colour);
            
            Raylib.DrawTexturePro(icon, new Rectangle(0,0,iconsize.X,iconsize.Y), new Rectangle(box.x + box.width / 2, box.y + box.height / 2, box.width, box.height), new Vector2(box.width / 2, box.height / 2), rotation, iconcolour);
        }
    }
}
