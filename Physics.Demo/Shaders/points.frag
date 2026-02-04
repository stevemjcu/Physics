#version 330 core
out vec4 color;

uniform vec4 static_color;

void main()
{
    color = static_color;
}