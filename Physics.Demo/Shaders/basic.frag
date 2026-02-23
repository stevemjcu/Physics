#version 330 core
in float depth;

out vec4 color;

uniform vec4 base_color;

void main()
{
    color = (0.25 * base_color) + (0.75 * base_color / (depth / 2));
}