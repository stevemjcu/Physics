#version 330 core
layout(location = 0) in vec3 position;

out float depth;

uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(position, 1.0) * view * projection;
    depth = gl_Position.z + 1;
}