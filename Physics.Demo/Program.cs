using OpenTK.Windowing.Desktop;
using Physics.Demo;

var settings = new NativeWindowSettings()
{
    Title = "LearnOpenTK",
    ClientSize = new(800, 600)
};

using var window = new Window(GameWindowSettings.Default, settings);
window.Run();