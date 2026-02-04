#version 330 core
in float depth;

out vec4 color;

uniform vec4 base_color;

void main()
{
    color = base_color / (depth * 1.5);
}