#include "scene.h"
#include "binary/animation.h"
#include "binary/skeleton.h"
#include "binary/player.h"

Shader* Scene::vertexShader = nullptr;
Shader* Scene::fragmentShader = nullptr;
Program* Scene::program = nullptr;
Camera* Scene::camera = nullptr;
Object* Scene::player = nullptr;
Texture* Scene::diffuse = nullptr;
Material* Scene::material = nullptr;
Object* Scene::lineDraw = nullptr;
Texture* Scene::lineColor = nullptr;
Material* Scene::lineMaterial = nullptr;

bool Scene::upperFlag = true;
bool Scene::lowerFlag = true;
float elapsedTime_U = 0.0f;
float elapsedTime_L = 0.0f;

vector<Vertex> default_pose = playerVertices;
vector<mat4> M_pre;

void Scene::setup(AAssetManager* aAssetManager) {
    Asset::setManager(aAssetManager);

    Scene::vertexShader = new Shader(GL_VERTEX_SHADER, "vertex.glsl");
    Scene::fragmentShader = new Shader(GL_FRAGMENT_SHADER, "fragment.glsl");

    Scene::program = new Program(Scene::vertexShader, Scene::fragmentShader);

    Scene::camera = new Camera(Scene::program);
    Scene::camera->eye = vec3(0.0f, 0.0f, 80.0f);

    Scene::diffuse = new Texture(Scene::program, 0, "textureDiff", playerTexels, playerSize);
    Scene::material = new Material(Scene::program, diffuse);
    Scene::player = new Object(program, material, playerVertices, playerIndices);
    player->worldMat = scale(vec3(1.0f / 3.0f));

    Scene::lineColor = new Texture(Scene::program, 0, "textureDiff", {{0xFF, 0x00, 0x00}}, 1);
    Scene::lineMaterial = new Material(Scene::program, lineColor);
    Scene::lineDraw = new Object(program, lineMaterial, {{}}, {{}}, GL_LINES);
}

void Scene::screen(int width, int height) {
    Scene::camera->aspect = (float) width/height;
}

void Scene::update(float deltaTime) {
    Scene::program->use();
    Scene::camera->update();

    if(upperFlag)
        elapsedTime_U += deltaTime;

    if(lowerFlag)
        elapsedTime_L += deltaTime;


    if (elapsedTime_U >= 4.0f)
        elapsedTime_U = 0.0f;


    if (elapsedTime_L >= 4.0f)
        elapsedTime_L = 0.0f;

    // Create Mid & Mip
    vector<mat4> Mp, Md, Md_inv;

    for (int i = 0; i < 28; i++)
    {
        if (i == 0){
            Mp.push_back(mat4(1.0f));
            Md.push_back(mat4(1.0f));
        }
        else {
            Mp.push_back(translate(jOffsets[i]));
            Md.push_back(Md[jParents[i]]*Mp[i]);
        }
        Md_inv.push_back(inverse(Md[i]));
    }

    mat4 rotate_cur, rotate_next;

    int frame_cur_U = floor(elapsedTime_U);
    int frame_next_U = (frame_cur_U + 1) % 4;
    int frame_cur_L = floor(elapsedTime_L);
    int frame_next_L = (frame_cur_L + 1) % 4;
    float weight_frame;

    // Mil & Mia
    vector<mat4> Ma;

    for (int i = 0; i < 28; i++){
        if( i < 12 )
        {
            rotate_cur = rotate(radians(motions[frame_cur_L][3*i + 5]), vec3(0.0f, 0.0f, 1.0f))
                         * rotate(radians(motions[frame_cur_L][3*i + 3]), vec3(1.0f, 0.0f, 0.0f))
                         * rotate(radians(motions[frame_cur_L][3*i + 4]), vec3(0.0f, 1.0f, 0.0f));

            rotate_next = rotate(radians(motions[frame_next_L][3*i + 5]), vec3(0.0f, 0.0f, 1.0f))
                          * rotate(radians(motions[frame_next_L][3*i + 3]), vec3(1.0f, 0.0f, 0.0f))
                          * rotate(radians(motions[frame_next_L][3*i + 4]), vec3(0.0f, 1.0f, 0.0f));

            weight_frame = elapsedTime_L - frame_cur_L;
        }
        else
        {
            rotate_cur = rotate(radians(motions[frame_cur_U][3*i + 5]), vec3(0.0f, 0.0f, 1.0f))
                         * rotate(radians(motions[frame_cur_U][3*i + 3]), vec3(1.0f, 0.0f, 0.0f))
                         * rotate(radians(motions[frame_cur_U][3*i + 4]), vec3(0.0f, 1.0f, 0.0f));

            rotate_next = rotate(radians(motions[frame_next_U][3*i + 5]), vec3(0.0f, 0.0f, 1.0f))
                          * rotate(radians(motions[frame_next_U][3*i + 3]), vec3(1.0f, 0.0f, 0.0f))
                          * rotate(radians(motions[frame_next_U][3*i + 4]), vec3(0.0f, 1.0f, 0.0f));

            weight_frame = elapsedTime_U - frame_cur_U;
        }

        quat q_rotate_cur = quat_cast(rotate_cur);
        quat q_rotate_next = quat_cast(rotate_next);
        quat q_Mil = mix(q_rotate_cur, q_rotate_next, weight_frame);
        mat4 Mil = mat4_cast(q_Mil);

        if(i == 0)
            Ma.push_back(Mil);
        else
            Ma.push_back(Ma[jParents[i]]*Mp[i]*Mil);
    }

    // save default
    for (int i = 0; i < playerVertices.size(); i++) {
        vec3 position = default_pose[i].pos;
        vec3 normal = default_pose[i].nor;

        vec4 weight = default_pose[i].weight;
        vec4 bone = default_pose[i].bone;

        // final Matrix M(Mia * Mid_inv)
        mat4 M = mat4(0.0f);

        for (int j = 0; j < 4; j++) {
            if (bone[j] == -1)
                continue;
            M += weight[j] * Ma[bone[j]] * Md_inv[bone[j]];
        }
        playerVertices[i].pos = vec3(M * vec4(position, 1.0f));
        playerVertices[i].nor = transpose(inverse(mat3(M))) * normal;
    }

    if(upperFlag && lowerFlag) M_pre = Ma;


    // Line Drawer
    // glLineWidth(20);
    // Scene::lineDraw->load(vertices, indices);
    // Scene::lineDraw->draw();

    Scene::player->load(playerVertices, playerIndices);
    Scene::player->draw();

}

void Scene::setUpperFlag(bool flag)
{
    Scene::upperFlag = flag;
}

void Scene::setLowerFlag(bool flag)
{
    Scene::lowerFlag = flag;
}