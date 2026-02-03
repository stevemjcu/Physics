using OpenTK.Windowing.Desktop;
using Physics.Demo;

var gameWindowSettings = new GameWindowSettings()
{
    UpdateFrequency = 144
};

var nativeWindowSettings = new NativeWindowSettings()
{
    Title = "LearnOpenTK",
    ClientSize = new(800, 600),
};

using var window = new Window(gameWindowSettings, nativeWindowSettings);
window.Run();