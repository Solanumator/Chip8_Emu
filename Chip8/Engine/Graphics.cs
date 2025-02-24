using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Chip8.Engine
{
    public class Graphics : GameWindow
    {

        public Graphics(int width, int height, string title)
            : base(GameWindowSettings.Default, new NativeWindowSettings()
            { 
                ClientSize = (width, height),
                Title = title 
            }) 
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0f, 0f, 0f, 1f);
        }

        public override void Run()
        {
            base.Run();
        }

        public void Test()
        {
            Console.ReadLine();
        }
    }
}
