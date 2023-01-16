using System.Text.Json;
using System.Xml;
using Raylib_cs;

namespace Packing;

public class Controller
{
    private int spacing = 15;
    private int innerspacing;

    private float thick = 9.0f;

    Color background = new Color(31, 33, 47, 255);    //grey
    Color foreground = new Color(250, 248, 229,255);  //white

    Color goodground = new Color(124, 225, 112, 255); //green
    Color badground = new Color(226, 110, 104,255);   //red

    private Rectangle border;
    
    //XmlDocument xdoc = new();

    private Schedule schedule;
    private Show show;
    
    private enum Scene
    {
        scheduling,
        showing
        
    }

    private bool editing;
    
    Scene scene;

    public Controller()
    {
        innerspacing = spacing * 3;
        border = new Rectangle(spacing, spacing, Raylib.GetScreenWidth() - spacing * 2, Raylib.GetRenderHeight() - spacing * 2);
        
        schedule = new Schedule(innerspacing, thick);
        show = new Show(schedule.schedule, innerspacing, thick, schedule.lessonsize);
        
        scene = Scene.scheduling;
        try
        {
            string savedjsonstring = File.ReadAllText("schedule/info.json");
            ScheduleParse jsonData = JsonSerializer.Deserialize<ScheduleParse>(savedjsonstring);
            show.Calculate(jsonData);
            scene = Scene.showing;
        }
        catch (FileNotFoundException e)
        {
            scene = Scene.scheduling;
        }
        catch (ArgumentNullException e)
        {
            scene = Scene.scheduling;
        }
        catch (JsonException e)
        {
            scene = Scene.scheduling;
        }
    }

    public void JustRun()
    {
        if (Raylib.IsWindowFocused()) //BORDELESS WINDOW??
        {
            Raylib.SetWindowState(ConfigFlags.FLAG_FULLSCREEN_MODE);
        }
        else if (Raylib.IsWindowFullscreen())
        {
            Raylib.ToggleFullscreen();
        }
        
        if (schedule.done)
        {
            if (schedule.schedule.Count != 0)
            {
                show.Calculate(schedule.schedule);
            }
            
            scene = Scene.showing;
            schedule.done = false;
        }
        if (show.done)
        {
            scene = Scene.scheduling;
            show.done = false;
        }
        Play();
        Work();
        Draw();
    }

    private void Play()
    {
        switch (scene)
        {
            case 0:
                schedule.Play();
                break;
            case (Scene)1:
                show.Play();
                break;
        }
    }

    private void Work()
    {
        switch (scene)
        {
            case 0:
                schedule.Work();
                break;
            case (Scene)1:
                show.Work();
                break;
        }
    }

    private void Draw()
    {
        Raylib.DrawRectangleRoundedLines(border, 0.015f,2,thick, foreground);
        switch (scene)
        {
            case 0:
                schedule.Draw(foreground, badground, goodground);
                break;
            case (Scene)1:
                show.Draw(foreground, badground, goodground);
                break;
        }
        Raylib.ClearBackground(background);
    }
}