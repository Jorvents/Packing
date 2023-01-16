using System.Diagnostics;
using System.Numerics;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Raylib_cs;

namespace Packing;

public class Show
{
    public bool done = false;
    
    private Texture2D extraicon;
    private Texture2D neededicon;
    
    private List<string> extra =  new List<string>();
    private List<string> needed = new List<string>();
    
    private int spacing;
    private float thick;
    private int size;

    private string everythingpacked = "You have everything packed";
    private int size2;
    private Vector2 loc2;

    private string test;

    private Vector2 linestart;
    private Vector2 lineend;
    
    private JsonDocument info;

    private Button backroom;
    public Show(List<Lesson> schedule,int spacing, float thick,float lessonsize)
    {
        this.spacing = spacing;
        this.thick = thick;

        extraicon = Raylib.LoadTexture("assets/extra.png");
        neededicon = Raylib.LoadTexture("assets/needed.png");
        
        backroom =
            new Button(new Rectangle(spacing, Raylib.GetScreenHeight() - lessonsize - spacing,
                    lessonsize, lessonsize),
                Raylib.LoadTexture("assets/arrow.png"), new Vector2(512), 180f
            );
        
        size = (Raylib.GetScreenWidth() / 2 - spacing * 2) / 19;
        
        size2 = (Raylib.GetScreenWidth() - spacing * 2) / 19;
        loc2 = new Vector2(Raylib.GetScreenWidth() / 2 - Raylib.MeasureText(everythingpacked,size2) / 2, Raylib.GetScreenHeight() / 2 - size2 / 2.1f);

        linestart = new Vector2(Raylib.GetScreenWidth() / 2, spacing / 3);
        lineend = new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() - spacing / 3);
    }

    public void Play()
    {
        backroom.Play();
        if (backroom.isHovered)
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
        if (backroom.isPressed)
        {
            done = true;
        }
    }

    public void Draw(Color colour, Color badground, Color goodground)
    {
        backroom.Draw(colour, badground, thick);
        
        int yloc = spacing;
        foreach (string lesson in needed)
        {
            if (lesson != null)
            {
                Raylib.DrawText(lesson, spacing * 3, yloc, size, colour);
                Raylib.DrawTexturePro(neededicon, new Rectangle(0,0,512,512), new Rectangle(spacing, yloc, size, size), new Vector2(0), 0f, goodground);
                yloc += size + size / 2;
            }
        }
        
        int yloc2 = spacing;
        foreach (var lesson in extra)
        {
            if (lesson != null)
            {
                Raylib.DrawText(lesson, (int)linestart.X + spacing * 3, yloc2, size, colour);
                Raylib.DrawTexturePro(extraicon, new Rectangle(0,0,512,512), new Rectangle((int)linestart.X + spacing, yloc2, size, size), new Vector2(0), 0f, badground);
                yloc2 += size + size / 2;
            }
        }
        
        if (!needed.Any() && !extra.Any())
        {
            Raylib.DrawText(everythingpacked, (int)loc2.X,(int)loc2.Y,size2,colour);
            return;
        }
        
        Raylib.DrawLineEx(linestart, lineend, thick, colour);
    }

    public void Calculate(List<Lesson> schedule)
    {
        string[] Monday = new string[8];
        string[] Tuesday = new string[8];
        string[] Wednesday = new string[8];
        string[] Thursday = new string[8];
        string[] Friday = new string[8];
        
        foreach (var lesson in schedule) //parsing
        {
            switch ((int)lesson.loc.Y)
            {
                case 0:
                    Monday[(int)lesson.loc.X] = $"{lesson.lessonname}";
                    break;
                case 1:
                    Tuesday[(int)lesson.loc.X] = $"{lesson.lessonname}";
                    break;
                case 2:
                    Wednesday[(int)lesson.loc.X] = $"{lesson.lessonname}";
                    break;
                case 3:
                    Thursday[(int)lesson.loc.X] = $"{lesson.lessonname}";
                    break;
                case 4:
                    Friday[(int)lesson.loc.X] = $"{lesson.lessonname}";
                    break;
            }
        }

        ScheduleParse scheduleparse = new ScheduleParse()
        {
            Monday = Monday,
            Tuesday = Tuesday,
            Wednesday = Wednesday,
            Thursday = Thursday,
            Friday = Friday
        };
        
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string jsonString = JsonSerializer.Serialize(scheduleparse, options);
        File.WriteAllText("schedule/info.json", jsonString);
        string savedjsonstring = File.ReadAllText("schedule/info.json");
        ScheduleParse jsonData = JsonSerializer.Deserialize<ScheduleParse>(savedjsonstring);
        
        Calculate(jsonData);
    }

    public void Calculate(ScheduleParse schedule)
    {
        ScheduleParse jsonData = schedule;
        
        switch (DateTime.Now.DayOfWeek)
        {
            case DayOfWeek.Monday:
                needed = jsonData.Tuesday.ToList();
                extra = jsonData.Monday.ToList();
        
                foreach (var lesson in jsonData.Monday)
                {
                    needed.Remove(lesson);
                }
                foreach (var lesson in jsonData.Tuesday)
                {
                    extra.Remove(lesson);
                }
                break;
            case DayOfWeek.Tuesday:
                needed = jsonData.Wednesday.ToList();
                extra = jsonData.Tuesday.ToList();
        
                foreach (var lesson in jsonData.Tuesday)
                {
                    needed.Remove(lesson);
                }
                foreach (var lesson in jsonData.Wednesday)
                {
                    extra.Remove(lesson);
                }
                break;
            case DayOfWeek.Wednesday:
                needed = jsonData.Thursday.ToList();
                extra = jsonData.Wednesday.ToList();
        
                foreach (var lesson in jsonData.Wednesday)
                {
                    needed.Remove(lesson);
                }
                foreach (var lesson in jsonData.Thursday)
                {
                    extra.Remove(lesson);
                }
                break;
            case DayOfWeek.Thursday:
                needed = jsonData.Friday.ToList();
                extra = jsonData.Thursday.ToList();
        
                foreach (var lesson in jsonData.Thursday)
                {
                    needed.Remove(lesson);
                }
                foreach (var lesson in jsonData.Friday)
                {
                    extra.Remove(lesson);
                }
                break;
            default:
                needed = jsonData.Monday.ToList();
                extra = jsonData.Friday.ToList();
        
                foreach (var lesson in jsonData.Friday)
                {
                    needed.Remove(lesson);
                }
                foreach (var lesson in jsonData.Monday)
                {
                    extra.Remove(lesson);
                }
                break;
        }

        extra = extra.Distinct().ToList();
        needed = needed.Distinct().ToList();
    }
}