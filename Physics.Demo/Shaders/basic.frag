#version 330 core
in vec4 displacement;

out vec4 color;

uniform vec4 base_color;

void main()
{
    float depth = length(displacement) / 25;
    color = mix(base_color, vec4(0), depth);
}