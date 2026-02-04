#version 330 core
in float depth;

out vec4 color;

uniform vec4 static_color;

void main()
{
    color = static_color / depth;
}