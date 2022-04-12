#version 330

layout(location=0) in vec3 inPos;
layout(location=1) in vec3 inNormal;
layout(location=2) in vec2 inTextCoords;

uniform mat4 projectionMat;
uniform mat4 viewMat;
uniform mat4 modelMat;

out vec3 FragPos;  
out vec3 Normal;
  
void main()
{
    gl_Position = projectionMat * viewMat * modelMat * vec4(inPos, 1.0);
    FragPos = vec3(modelMat * vec4(inPos, 1.0));
    Normal = mat3(transpose(inverse(modelMat))) * inNormal;
}