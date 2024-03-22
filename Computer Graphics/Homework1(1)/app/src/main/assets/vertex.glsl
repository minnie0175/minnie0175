#version 300 es

uniform mat4 worldMat, viewMat, projMat;
uniform vec3 lightPos;

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texCoord;

out vec3 v_normal;
out vec2 v_texCoord;
out vec3 v_lightDir;

void main() {
    float scale = 1.0;

    //////////////////////////////
    /* TODO: Problem 1.
    *  Fill in the lines below.
    *  Scale the part of the teapot below XZ plane.
    */

    vec4 pos = worldMat * vec4(position, 1.0);

    if(pos.y < 0.0)
    {
        pos.x = 1.5 * pos.x;
        pos.y = 1.5 * pos.y;
        pos.z = 1.5 * pos.z;
    }

    gl_Position = projMat * viewMat * pos;
    v_normal = normalize(transpose(inverse(mat3(worldMat))) * normal);
    v_texCoord = texCoord;

    //////////////////////////////

    vec3 posWS = (worldMat * vec4(position, 1.0)).xyz;
    v_lightDir = normalize(lightPos - posWS);
}
