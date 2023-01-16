using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Raylib_cs;

namespace Packing;

public class Schedule // && lessons controller
{
    public bool done;
    
    Rectangle textboxrec;
    TextBox textbox;

    Rectangle grid1;
    Rectangle grid2;

    float thick;
    float thick2 = 5.0f;

    public float lessonsize;
    int textsize;

    int grid2columns;
    int grid2count;

    Lesson creating = new Lesson();
    Rectangle lessonstudio;
    List<Lesson> lessons = new List<Lesson>();
    List<Color> colours = new List<Color>();
    
    int selectedindex = -1;

    public List<Lesson> schedule = new List<Lesson>();

    private Button trashcanoffice;
    private Button continuedoor;

    private Sound trashsound;
    private Sound programsound;
    private Sound selectsound;
    private Sound addsound;
    private Sound disabledsound;
    private Sound deselectsound;

    Lesson hovering = new Lesson();
    float hoverspeed = 1.4f;
    float hoverbetween;
    int hoversize;
    
    public Schedule(int spacing, float thick)
    {
        float height = Raylib.GetScreenHeight() - spacing * 3;
        lessonsize = height / 6f;
        
        textsize = (int)(lessonsize / 2.3f);
        hoversize = (int)(lessonsize / 4f);
        hoverbetween = (int)(lessonsize / 20f);
        
        if (lessonsize * 12 > Raylib.GetScreenWidth() - spacing * 3)
        {
            lessonsize = (Raylib.GetScreenWidth() - spacing * 3) / 12f;
            height = lessonsize * 5;
        }
        
        float width = lessonsize * 8;
        
        float xloc1 = Raylib.GetScreenWidth() - width - spacing;

        grid1 = new Rectangle(xloc1, spacing, width, height);

        grid2columns = (int)Math.Truncate((grid1.x - spacing) / lessonsize);
        grid2count = grid2columns * 5;
        
        float width2 = grid2columns * lessonsize;
        
        grid2 = new Rectangle(spacing, spacing, width2,grid1.height);
        
        this.thick = thick;
        
        trashsound = Raylib.LoadSound("assets/delete.wav");
        programsound = Raylib.LoadSound("assets/program.wav");
        selectsound = Raylib.LoadSound("assets/select.wav");
        addsound = Raylib.LoadSound("assets/add.wav");
        disabledsound = Raylib.LoadSound("assets/disabled.wav");
        deselectsound = Raylib.LoadSound("assets/deselect.wav");

        trashcanoffice =
            new Button(new Rectangle(spacing, Raylib.GetScreenHeight() - lessonsize - spacing, 
                    lessonsize, lessonsize),
                Raylib.LoadTexture("assets/bin.png"), new Vector2(512), 0f
                );

        continuedoor =
            new Button(
                new Rectangle(Raylib.GetScreenWidth() - lessonsize - spacing,
                    Raylib.GetScreenHeight() - lessonsize - spacing, lessonsize, lessonsize),
                Raylib.LoadTexture("assets/arrow.png"), new Vector2(512), 0f
                ); 

        int xwidth = (int)lessonsize * 8;
        int yheight = (int)lessonsize;
        int xloc = Raylib.GetScreenWidth() - xwidth - spacing - (int)continuedoor.box.width - spacing;
        
        textboxrec = new Rectangle(xloc,Raylib.GetScreenHeight() - yheight - spacing,xwidth,yheight);
        textbox = new TextBox(textboxrec, thick, textsize);

        lessonstudio = new Rectangle((textboxrec.x - trashcanoffice.box.x + trashcanoffice.box.width + spacing * 2) / 2 - textboxrec.height / 2,textboxrec.y,textboxrec.height,textboxrec.height);
        
        colours = new List<Color>()
        {
            new Color(14, 196, 52,255),
            new Color(34, 140, 104,255),
            new Color(138, 216, 232,255),
            new Color(35, 91, 84,255),
            new Color(41, 189, 171,255),
            new Color(57, 152, 245,255),
            new Color(55, 41, 79,255),
            new Color(39, 125, 167,255),
            new Color(55, 80, 219,255),
            new Color(242, 32, 32,255),
            new Color(153, 25, 25,255),
            new Color(255, 203, 165,255),
            new Color(230, 143, 102,255),
            new Color(197, 97, 51,255),
            new Color(150, 52, 28,255),
            new Color(99, 40, 25,255),
            new Color(255, 196, 19,255),
            new Color(244, 122, 34,255),
            new Color(47, 42, 160,255),
            new Color(183, 50, 204,255),
            new Color(119, 43, 157,255),
            new Color(240, 124, 171,255),
            new Color(211, 11, 148,255),
            new Color(195, 165, 180,255),
            new Color(148, 106, 162,255),
            new Color(93, 76, 134,255)
        };
        rndColour();
    }

    public void Play()
    {
        textbox.Play();
        trashcanoffice.Play();
        continuedoor.Play();

        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), grid1))
        {
            int xloc = (int)Math.Truncate((Raylib.GetMouseX() - grid1.x) / lessonsize);
            int yloc = (int)Math.Truncate((Raylib.GetMouseY() - grid1.y) / lessonsize);
            
            if (selectedindex != -1)
            {
                Lesson adding = new Lesson();
            
                adding.tag = lessons[selectedindex].tag;
                adding.lessonname = lessons[selectedindex].lessonname;
                adding.colour = lessons[selectedindex].colour;
                    
                Lesson removing = schedule.Find(x => x.loc == new Vector2(xloc, yloc));
                if (removing != null)
                {
                    if (adding.lessonname != removing.lessonname)
                    {
                        Raylib.PlaySound(trashsound);
                        schedule.Remove(removing);
                    }
                }
                else
                {
                    Raylib.PlaySound(programsound);
                    
                    adding.loc = new Vector2(xloc, yloc);
            
                    schedule.Add(adding);
                }
            }
            else
            {
                if (schedule.Remove(schedule.Find(x => x.loc == new Vector2(xloc, yloc))))
                {
                    Raylib.PlaySound(trashsound);
                }
            }
            
        }

        if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), textbox.placement))
        {
            Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_IBEAM);
        }
        else if (selectedindex == -1 && trashcanoffice.isHovered)
        {
            Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_NOT_ALLOWED);
        }
        else if (trashcanoffice.isHovered || continuedoor.isHovered)
        {
            Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_POINTING_HAND);
        }
        else
        {
            Raylib.SetMouseCursor(MouseCursor.MOUSE_CURSOR_ARROW);
        }
        
    }

    public void Work()
    {
        Shorten(textbox.output);
        
        textbox.Work();
        
        if (Raylib.GetKeyPressed() == 0)
        {
            string[] lessonnames = lessons.Select(l => l.lessonname).ToArray();
            lessonnames = lessonnames.Where(x => x != "").ToArray();
            lessonnames = lessonnames.Select(x => x.ToLower()).ToArray();
            if (!textbox.disabled)
            {
                textbox.angry = lessonnames.Contains(textbox.output.ToLower());
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && !textbox.disabled && !textbox.angry && textbox.letters != 0)
        {
            if (textbox.letters != 0 && !textbox.disabled && !(lessons.Count >= grid2count))
            {
                Raylib.PlaySound(addsound);
                creating.lessonname = textbox.output;
                textbox.Delete();
                lessons.Add(creating);
                creating = new Lesson();
                rndColour();
            }
            
            if (lessons.Count >= grid2count)
            {
                textbox.disabled = true;
                textbox.angry = true;
                textbox.Delete();
                creating = new Lesson();
            }

            if (colours.Count == 0)
            {
                textbox.disabled = true;
                textbox.angry = true;
                textbox.Delete();
                creating = new Lesson();
            }
        }
        if (textbox.angry && Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
        {
            Raylib.PlaySound(disabledsound);
        }

        if (selectedindex != -1 && trashcanoffice.isPressed)
        {
            Raylib.PlaySound(trashsound);
            if (textbox.disabled)
            {
                textbox.disabled = false;
                creating = new Lesson();
                rndColour();
            }
            colours.Insert(Raylib.GetRandomValue(0,colours.Count),lessons[selectedindex].colour);
            Lesson removing = lessons[selectedindex];
            lessons.Remove(removing);
            
            schedule.RemoveAll(l => l.lessonname == removing.lessonname);
            selectedindex = -1;
        }
        if (continuedoor.isPressed)
        {
            done = true;
        }
    }

    public void Draw(Color colour, Color badground, Color goodground)
    {
        textbox.Draw(colour, badground);
        
        if (lessons.Count != 0)
        {
            hovering = new Lesson();
            for (int i = 0; i <= lessons.Count - 1; i++)
            {
                int xloc = (int)(grid2.x + lessonsize * (i + Math.Truncate((double)(i / grid2columns)) * -grid2columns)); //this took 4h to figure out

                int yloc = (int)(grid2.y + lessonsize * Math.Truncate((double)(i / grid2columns)));
                
                Raylib.DrawRectangle(xloc, yloc, (int)lessonsize, (int)lessonsize, lessons[i].colour);
                
                Raylib.DrawText(lessons[i].tag, (int)(xloc + lessonsize / 2 - Raylib.MeasureText(lessons[i].tag, textsize) / 2), (int)(yloc + lessonsize / 2 - textsize / 2.1f),textsize, colour);
                
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(xloc,yloc, lessonsize, lessonsize))&& i != selectedindex) //wrong place but what ever
                {
                    Raylib.PlaySound(selectsound);
                    selectedindex = i;
                }

                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT) && selectedindex != -1)
                {
                    Raylib.PlaySound(deselectsound);
                    selectedindex = -1;
                }
                if (selectedindex == i)
                {
                    Raylib.DrawRectangle(xloc,yloc,(int)lessonsize,(int)lessonsize, new Color(0,0,0,100));
                }
                
                float mousespeed = (Raylib.GetMouseDelta().X + Raylib.GetMouseDelta().Y) / 2f;
                
                if (Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(xloc,yloc, lessonsize, lessonsize)) && mousespeed < hoverspeed && mousespeed > -hoverspeed && hovering.lessonname != lessons[i].lessonname )
                {
                    hovering.lessonname = lessons[i].lessonname;
                    hovering.colour = lessons[i].colour;
                }
            }
        }
        
        if (schedule.Count != 0)
        {
            for (int i = 0; i <= schedule.Count - 1; i++)
            {
                int xloc = (int)(grid1.x + schedule[i].loc.X * lessonsize);

                int yloc = (int)(grid1.y + schedule[i].loc.Y * lessonsize);
                
                Raylib.DrawRectangle(xloc, yloc, (int)lessonsize, (int)lessonsize, schedule[i].colour);
                
                Raylib.DrawText(schedule[i].tag, (int)(xloc + lessonsize / 2 - Raylib.MeasureText(schedule[i].tag, textsize) / 2), (int)(yloc + lessonsize / 2 - textsize / 2.1f),textsize, colour);
            }
        }
        for (int i = 1; i <= 4; i++)
        {
            Vector2 grid1left = new Vector2(grid1.x, lessonsize * i + grid1.y);
            Vector2 grid1right = new Vector2(grid1.x + grid1.width, lessonsize * i + grid1.y);

            Vector2 grid2left = new Vector2(grid2.x, (int)(grid2.y + lessonsize * i));
            Vector2 grid2right = new Vector2(grid2.x + grid2.width , grid2.y + lessonsize * i);
                
            Raylib.DrawLineEx(grid1left, grid1right, thick2, colour);
            Raylib.DrawLineEx(grid2left, grid2right, thick2, colour);
        }
        
        Raylib.DrawRectangleRoundedLines(grid1, 0.009f,2,thick, colour);
        Raylib.DrawRectangleRoundedLines(grid2, 0.013f,2,thick, colour);
        
        for (int i = 1; i <= 7; i++)
        {
            Vector2 grid1up = new Vector2(grid1.width - lessonsize * i + grid1.x , grid1.y);
            Vector2 grid1down = new Vector2(grid1.width - lessonsize * i + grid1.x, grid1.y + grid1.height);
            
            Raylib.DrawLineEx(grid1up, grid1down, thick2, colour);
            
            if (grid2columns - 1 > i - 1)
            {
                Vector2 grid2up = new Vector2(grid2.width - lessonsize * i + grid2.x, grid2.y);
                Vector2 grid2down = new Vector2(grid2.width - lessonsize * i + grid2.x, grid2.y + grid2.height);
                
                Raylib.DrawLineEx(grid2up, grid2down, thick2, colour);
            }
        }
        
        Raylib.DrawRectangleRec(lessonstudio, creating.colour);
        Raylib.DrawText(creating.tag, (int)(lessonstudio.x + lessonstudio.width / 2 - Raylib.MeasureText(creating.tag, textsize) / 2), (int)(lessonstudio.y + lessonstudio.height / 2 - textsize / 2.1f), textsize, colour);
        Raylib.DrawRectangleRoundedLines(lessonstudio, 0.07f,2,thick, colour);
        
        //Raylib.DrawText(schedule.Count.ToString(), 50, 820, 50, colour);
        //Raylib.DrawText(schedule.Count.ToString(), 50, 900, 50, colour);
        //Raylib.DrawText(schedule.Count.ToString(), 50, 900, 50, colour);
        
        trashcanoffice.Draw(colour, badground, thick);
        
        continuedoor.Draw(colour, goodground, thick);
        
        if (hovering.lessonname != "")
        {
            Rectangle hoverbox = new Rectangle(Raylib.GetMouseX(), Raylib.GetMouseY() - hoversize - hoverbetween * 2, Raylib.MeasureText(hovering.lessonname, hoversize) + hoverbetween * 2, hoversize + hoverbetween * 2);
            Raylib.DrawRectangleRec(hoverbox, new Color(0,0,0,160));
            Raylib.DrawRectangleRoundedLines(hoverbox,0.1f, 2, thick2, new Color(255,255,255,180));
            Raylib.DrawText(hovering.lessonname, (int)(Raylib.GetMouseX() + hoverbetween), (int)(hoverbox.y + hoverbetween), hoversize, colour);
        }
    }

    void Shorten(string input)
    {
        if (Raylib.MeasureText(input, textsize) < lessonstudio.width - 15)
        {
            creating.tag = input;
        }
        else
        {
            string[] words = input.Split(' ');
            words = words.Where(w => w.Trim() != "").ToArray();
            
            if (words.Length < 2)
            {
                return;
            }

            char[] firstchars = words.Select(w => w[0]).ToArray();
            string firstletters = new string(firstchars);
            
            if (Raylib.MeasureText(firstletters, textsize) < lessonstudio.width - 25)
            {
                creating.tag = firstletters;
            }
        }
    }

    void rndColour()
    {
        if (colours.Count == 0)
        {
            creating.colour = Color.BLANK;
            return;
        }
        int index = Raylib.GetRandomValue(0, colours.Count - 1);
        creating.colour = colours[index];
        colours.RemoveAt(index);
    }
}