#include "scene.h"
#include "binary/teapot.h"
#include "binary/rgb.h"
#include "binary/cloud.h"
#include "binary/tex_flower.h"
#include "checker.h"

Shader* Scene::vertexShader = nullptr;
Shader* Scene::fragmentShader = nullptr;
Program* Scene::program = nullptr;
Camera* Scene::camera = nullptr;
Object* Scene::teapot = nullptr;
Texture* Scene::diffuse = nullptr;
Texture* Scene::dissolve = nullptr;
Material* Scene::material = nullptr;
Light* Scene::light = nullptr;

int Scene::width = 0;
int Scene::height = 0;

// Arcball variables
float lastMouseX = 0, lastMouseY = 0;
float currentMouseX = 0, currentMouseY = 0;

void Scene::setup(AAssetManager* aAssetManager) {
    Asset::setManager(aAssetManager);

    Scene::vertexShader = new Shader(GL_VERTEX_SHADER, "vertex.glsl");
    Scene::fragmentShader = new Shader(GL_FRAGMENT_SHADER, "fragment.glsl");

    Scene::program = new Program(Scene::vertexShader, Scene::fragmentShader);

    Scene::camera = new Camera(Scene::program);
    Scene::camera->eye = vec3(20.0f, 30.0f, 20.0f);

    Scene::light = new Light(program);
    Scene::light->position = vec3(15.0f, 15.0f, 0.0f);

    //////////////////////////////
    /* TODO: Problem 2 : Change the texture of the teapot
     *  Modify and fill in the lines below.
     */
    vector<Texel> n_rgbTexels = rgbTexels;

    for (GLsizei i=0; i<1600; ++i)
    {
        GLubyte a =  n_rgbTexels[i].red;
        n_rgbTexels[i].red = n_rgbTexels[i].green;
        n_rgbTexels[i].green = n_rgbTexels[i].blue;
        n_rgbTexels[i].blue = a;

    }

    Scene::diffuse  = new Texture(Scene::program, 0, "textureDiff", n_rgbTexels, rgbSize);
    //////////////////////////////

    Scene::material = new Material(Scene::program, diffuse, dissolve);
    Scene::teapot = new Object(program, material, teapotVertices, teapotIndices);
}

void Scene::screen(int width, int height) {
    Scene::camera->aspect = (float) width/height;
    Scene::width = width;
    Scene::height = height;
}

void Scene::update(float deltaTime) {
    static float time = 0.0f;

    Scene::program->use();

    Scene::camera->update();
    Scene::light->update();

    Scene::teapot->draw();

    time += deltaTime;
}

void Scene::mouseDownEvents(float x, float y) {
    lastMouseX = currentMouseX = x;
    lastMouseY = currentMouseY = y;
}

void Scene::mouseMoveEvents(float x, float y) {
    //////////////////////////////
    /* TODO: Problem 3 : Implement Phong lighting
     *  Fill in the lines below.
     */


    currentMouseX = x;
    currentMouseY = y;

    vec3 arc_proj_c;
    vec3 arc_proj_p;

    float x_p_c = 2.0 * x / width - 1;
    float y_p_c = -(2.0 * y / height - 1);

    if(pow(x_p_c,2) + pow(y_p_c,2) > 1)
        arc_proj_c = normalize(vec3(x_p_c, y_p_c, 0.0));
    else
        arc_proj_c = vec3(x_p_c, y_p_c, sqrt(1- pow(x_p_c,2) - pow(y_p_c, 2)));

    float x_p_p = 2.0 * lastMouseX / width - 1;
    float y_p_p = -(2.0 * lastMouseY / height - 1);

    if(pow(x_p_p,2) + pow(y_p_p,2) > 1)
        arc_proj_p = normalize(vec3(x_p_p, y_p_p, 0.0));
    else
        arc_proj_p = vec3(x_p_p, y_p_p, sqrt(1- pow(x_p_p,2) - pow(y_p_p, 2)));

    vec3 r_axis = cross(arc_proj_p, arc_proj_c);

    vec3 n = normalize(Scene::camera->eye - Scene::camera->at);
    vec3 u = normalize(cross(Scene::camera->up, n));
    vec3 v = normalize(cross(n,u));
    mat4 inverseVT = glm::mat4(mat3(u,v,n));

    vec3 axis = vec3(inverseVT * vec4(r_axis, 1.0));

    float r_angle = acos(dot(arc_proj_c, arc_proj_p));

    glm::mat4 r_mat = glm::rotate(r_angle, axis);

    vec3 cur_pos = Scene::light->position;
    Scene::light->position = vec3( r_mat * vec4(Scene::light->position, 1.0f));

    if(isnan(Scene::light->position.x))
        Scene::light->position = cur_pos;

    lastMouseX = x;
    lastMouseY = y;

    LOG_PRINT_DEBUG("pos_c: %f, %f, %f", Scene::light->position[0], Scene::light->position[1], Scene::light->position[2]);
    //////////////////////////////
}