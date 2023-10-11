#include "scene.h"

#include "obj_teapot.h"
#include "tex_flower.h"


Shader* Scene::vertexShader = nullptr;
Shader* Scene::fragmentShader = nullptr;
Program* Scene::program = nullptr;
Camera* Scene::camera = nullptr;
Light* Scene::light = nullptr;
Object* Scene::teapot = nullptr;
Material* Scene::flower = nullptr;
float Scene::time = 0.0f;

void Scene::setup(AAssetManager* aAssetManager) {

    // set asset manager
    Asset::setManager(aAssetManager);

    // create shaders
    vertexShader = new Shader(GL_VERTEX_SHADER, "vertex.glsl");
    fragmentShader = new Shader(GL_FRAGMENT_SHADER, "fragment.glsl");

    // create program
    program = new Program(vertexShader, fragmentShader);

    // create camera
    camera = new Camera(program);
    camera->eye = vec3(60.0f, 60.0f, 60.0f);
    camera->cameraN = glm::normalize(camera->eye - camera->at);
    camera->cameraU = glm::normalize(glm::cross(camera->up, camera->cameraN));
    camera->cameraV = glm::normalize(glm::cross(camera->cameraN, camera->cameraU));

    // create light
    light = new Light(program);
    light->position = vec3(100.0f, 0.0f, 0.0f);

    // create floral texture
    flower = new Material(program, texFlowerData, texFlowerSize);

    // create teapot object
    teapot = new Object(program, flower, objTeapotVertices, objTeapotIndices,
                        objTeapotVerticesSize, objTeapotIndicesSize);
}

void Scene::screen(int width, int height) {

    // set camera aspect ratio
    camera->aspect = (float) width / height;
}

void Scene::update(float deltaTime) {
    static float radius = 15.0f;

    // use program
    program->use();

    // scale teapot
    float scale = 0.5;
    mat4 scaleM;
    scaleM = transpose(mat4(scale, 0.0f, 0.0f, 0.0f,
                            0.0f, scale, 0.0f, 0.0f,
                            0.0f, 0.0f, scale, 0.0f,
                            0.0f, 0.0f, 0.0f, 1.0f));
    teapot->worldMatrix = scaleM;

    //////////////////////////////
    /* TODO: Problem 2.
    *  Fill in the lines below.
    */
    // TODO : Make the teapot orbit on YZ plane

    mat4 orbitA = transpose( mat4 (1.0f ,0.0f, 0.0f, 0.0f,
                                   0.0f, 1.0f, 0.0f, radius*sin(time/40),
                                   0.0f, 0.0f, 1.0f, radius*cos(time/40),
                                   0.0f, 0.0f, 0.0f, 1.0f)) ;

    // TODO : Rotate the teapot around its own y-axis
    mat4 orbitI = transpose( mat4 (cos(time/20), 0.0f, sin(time/20), 0.0f,
                                   0.0f, 1.0f, 0.0f, 0.0f,
                                   -sin(time/20), 0.0f, cos(time/20), 0.0f,
                                   0.0f, 0.0f, 0.0f, 1.0f));

    mat4 result2 = orbitI *teapot->worldMatrix;

    teapot->worldMatrix = result2;

    mat4 result1 = orbitA *teapot->worldMatrix;

    teapot->worldMatrix = result1;
    //////////////////////////////

    camera->updateViewMatrix();
    camera->updateProjectionMatrix();

    teapot->viewMatrix = camera->viewMatrix;
    teapot->projMatrix = camera->projMatrix;

    light->setup();

    // increase time
    time = time + deltaTime * 100;

    // draw teapot
    teapot->draw();
}

void Scene::dragScreen(float dx,float dy)
{
    float Sensitivity = 0.1;

    float thetaYaw=glm::radians(Sensitivity*dx);

    moveCamera(thetaYaw);
}
void Scene::moveCamera(float theta)
{
    //////////////////////////////
    /* TODO: Problem 3.
     *  Note that u,v,n should always be orthonormal.
     *  The u vector can be accessed via camera->cameraU.
     *  The v vector can be accessed via camera->cameraV.
     *  The n vector can be accessed via camera->cameraN.
     *  === HINT ===
     *  Initial camera position is (60.0f, 60.0f, 60.0f)
     *  Argument theta is amount of camera rotation in radians
     */

/*
    camera->cameraN.x = cos(theta) * camera->cameraN.x - sin(theta) * camera->cameraN.z;
    camera->cameraN.z = cos(theta) * camera->cameraN.z + sin(theta) * camera->cameraN.x;
*/

    mat3 camera_r = transpose( mat3 (cos(-theta), 0.0f, sin(-theta),
                                   0.0f, 1.0f, 0.0f,
                                   -sin(-theta), 0.0f, cos(-theta)));

    camera->eye = camera_r * camera->eye;
    camera->cameraN = glm::normalize(camera->eye - camera->at);
    camera->cameraU = glm::normalize(glm::cross(camera->up, camera->cameraN));
    camera->cameraV = glm::normalize(glm::cross(camera->cameraN, camera->cameraU));


    //////////////////////////////
}






































