using Raylib_cs;
using System.Xml;
using Packing;

namespace Packing
{
    static class Program
    {
        public static void Main()
        {
            Raylib.InitWindow(Raylib.GetScreenWidth(), Raylib.GetScreenHeight(), "Packing");
            Raylib.SetTargetFPS(163);
            Raylib.InitAudioDevice();
            
            Raylib.SetWindowIcon(Raylib.LoadImage("assets/icon.png"));

            Controller controller = new Controller();

            /*

            XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.1", "utf-8", null);
            xdoc.AppendChild(dec);
            /*
            XmlElement Students = xdoc.CreateElement(null, "Students", null);
            xdoc.AppendChild(Students);

            XmlElement student = xdoc.CreateElement(null, "student1", null);
            XmlText studentName = xdoc.CreateTextNode("Jorvents");
            student.AppendChild(studentName);
            Students.AppendChild(student);
            */

            //xdoc.Save("schedule/info.xml");

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                
                controller.JustRun();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}