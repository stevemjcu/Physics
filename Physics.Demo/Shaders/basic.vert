#version 330 core
layout(location = 0) in vec3 position;

out vec4 displacement;

uniform mat4 view;
uniform mat4 projection;

void main()
{
    displacement = vec4(position, 1.0) * view;
    gl_Position = displacement * projection;
}