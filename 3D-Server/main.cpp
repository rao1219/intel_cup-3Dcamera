#include "CClient.h"
#include "CServer.h"
#include "Config.h"
#include "Enums.h"
#include <string>
#include <iostream>
#include <math.h>

using namespace std;
#define SET_RESULE(err)\
    {\
        info[INFO_ERROR_CODE] = err;\
        noWrong=false;\
    }
#define INFO_SIZE 8
#define JOINTDATA_SIZE 12
static Config config;
static float z[JOINTDATA_SIZE],x[JOINTDATA_SIZE],y[JOINTDATA_SIZE];
static float info[INFO_SIZE];

unsigned int Encode(float*x, float*y, float*z, Config*config, float*info, int jointsize, int infosize, char*lpmsg = 0);
unsigned int Decode(float*x, float*y, float*z, Config*config, float*info, int jointsize, int infosize, char*lpmsg = 0);
void Analyze(float*x, float*y, float*z, Config &config, float*info);
int main(){

    CServer *server =new CServer();
    server->init(9000);

    char msg[1024];
    while(1)
    {
        server->getMsg();
        int msgsize=Decode(x,y,z,&config,info,JOINTDATA_SIZE,INFO_SIZE,server->recvbuf);
        Analyze(x,y,z,config,info);
        Encode(x,y,z,&config,info,JOINTDATA_SIZE,INFO_SIZE,msg);
        server->sendMsg(msg,msgsize);
    }

    return 0;
}

unsigned int Encode(float*x, float*y, float*z, Config*config, float*info,int jointsize,int infosize,char*lpmsg/*=0*/)
{
    char*msg,*temp;
    msg = new char[1024];
    temp = msg;
    if (lpmsg != 0)
        msg = lpmsg;
    unsigned int msgindex = 0;
    for (int i = 0; i < jointsize*(sizeof(float) / sizeof(char)); i++)
        msg[msgindex++] = ((char*)x)[i];
    for (int i = 0; i < jointsize*(sizeof(float) / sizeof(char)); i++)
        msg[msgindex++] = ((char*)y)[i];
    for (int i = 0; i < jointsize*(sizeof(float) / sizeof(char)); i++)
        msg[msgindex++] = ((char*)z)[i];
    for (int i = 0; i < (sizeof(Config) / sizeof(char)); i++)
        msg[msgindex++] = ((char*)config)[i];
    for (int i = 0; i < infosize*(sizeof(float) / sizeof(char)); i++)
        msg[msgindex++] = ((char*)info)[i];
//    for(unsigned int i=0;i<msgindex;i++)
//        msg[i]+=1;
    msg[msgindex]=0;
    delete[] temp;
    return msgindex;
}

unsigned int Decode(float*x, float*y, float*z, Config*config, float*info, int jointsize, int infosize, char*lpmsg /*= 0*/)
{
    char*msg,*temp;
    msg = new char[1024];
    temp = msg;
    if (lpmsg != 0)
        msg = lpmsg;
    unsigned int msgsize=strlen(msg);
//    for(unsigned int i=0;i<msgsize;i++)
//        msg[i]-=1;
    unsigned int getindex = 0;
    for (int i = 0; i < jointsize*(sizeof(float) / sizeof(char)); i++)
        ((char*)x)[i] = msg[getindex++];
    for (int i = 0; i < jointsize*(sizeof(float) / sizeof(char)); i++)
        ((char*)y)[i] = msg[getindex++];
    for (int i = 0; i < jointsize*(sizeof(float) / sizeof(char)); i++)
        ((char*)z)[i] = msg[getindex++];
    for (int i = 0; i < (sizeof(Config) / sizeof(char)); i++)
        ((char*)config)[i] = msg[getindex++];
    for (int i = 0; i < infosize*(sizeof(float) / sizeof(char)); i++)
        ((char*)info)[i] = msg[getindex++];
    delete[] temp;
    return getindex;
}

void Analyze(float*x, float*y, float*z, Config&config, float*info)
{
    AnlyzeMode mode;
    bool noWrong = true;

    float headangle = ((y[HEAD] - y[NECK]) / (z[HEAD] - z[NECK]));
    float headangle2 = ((y[HEAD] - y[NECK]) / (x[HEAD] - x[NECK]));
    float torsoangle2 = (y[HEAD] - y[TORSO]) / (x[HEAD] - x[TORSO]);
    float torsoangle3 = (y[NECK] - y[TORSO]) / (x[NECK] - x[TORSO]);
    float hipangle4;
    float hipangle5;
    float hipangle6;
    if (z[RIGHT_HIP] < z[LEFT_HIP])
    {
        hipangle4 = (y[HEAD] - y[RIGHT_HIP]) / (x[HEAD] - x[RIGHT_HIP]);
        hipangle5 = (y[NECK] - y[RIGHT_HIP]) / (x[NECK] - x[RIGHT_HIP]);
        hipangle6 = (y[SHOULDER_ASIDE] - y[RIGHT_HIP]) / (x[SHOULDER_ASIDE] - x[RIGHT_HIP]);
    }
    else
    {
        hipangle4 = (y[HEAD] - y[LEFT_HIP]) / (x[HEAD] - x[LEFT_HIP]);
        hipangle5 = (y[NECK] - y[LEFT_HIP]) / (x[NECK] - x[LEFT_HIP]);
        hipangle6 = (y[SHOULDER_ASIDE] - y[LEFT_HIP]) / (x[SHOULDER_ASIDE] - x[LEFT_HIP]);
    }
    float hipangle7 = hipangle6;
    float shoulderangle1 = (y[SHOULDER_ASIDE] - y[HEAD_ASIDE]) / (x[SHOULDER_ASIDE] - x[HEAD_ASIDE]);
    //m_angle = GetAngle(0, 1, 0, x[HEAD] - x[NECK], y[HEAD] - y[NECK], z[HEAD] - z[NECK]);
    float shoulder_distance = abs(z[LEFT_SHOULDER] - z[RIGHT_SHOULDER]);
    info[INFO_SHOULDER_DISTANCE] = shoulder_distance;
    info[INFO_HEAD_FB] = headangle;
    info[INFO_HEAD_LR] = headangle2;
    info[INFO_TORSO] = hipangle7;
    info[INFO_HEAD_ASIDE] = shoulderangle1;
    if (shoulder_distance >= config.GetConfig(Config::CONFIG_SHOULDER_MAX))
    {
        mode = ANLYZEMODE_ASIDE;
        info[INFO_POSITION] = 0;
    }
    else if (shoulder_distance <= config.GetConfig(Config::CONFIG_SHOULDER_MIN))
    {
        mode = ANLYZEMODE_FRONT;
        info[INFO_POSITION] = 1;
    }
    if (mode == ANLYZEMODE_FRONT)
    {
        if (config.GetConfig(Config::CONFIG_HEAD_ANGLE_A) >= 0)
        {
            if (headangle <= config.GetConfig(Config::CONFIG_HEAD_ANGLE_A))
                SET_RESULE(1);
        }
        else
        {
            if (headangle >= config.GetConfig(Config::CONFIG_HEAD_ANGLE_A) && headangle<0)
                SET_RESULE(1);
        }
        if (headangle2 >= config.GetConfig(Config::CONFIG_HEAD_ANGLE_B_MIN) && headangle2 <= config.GetConfig(Config::CONFIG_HEAD_ANGLE_B_MAX))
            SET_RESULE(2);

        if (headangle2 >= -1 * config.GetConfig(Config::CONFIG_HEAD_ANGLE_B_MAX) && headangle2 <= config.GetConfig(Config::CONFIG_HEAD_ANGLE_B_MIN))
            SET_RESULE(3);
    }
    else
    {
        if (config.GetStatus(Config::CONBIT_ASIDE_BODY_TEST))
        {
            if (abs(hipangle7) <= config.GetConfig(Config::CONFIG_ABDOMEN_ANGLE))
                SET_RESULE(4);
            if (abs(shoulderangle1) <= config.GetConfig(Config::CONFIG_ASIDE_HEAD_ANGLE_MIN) && abs(hipangle7) >= config.GetConfig(Config::CONFIG_ASIDE_HEAD_ANGLE_MAX))
                SET_RESULE(5);
        }
        else
        {
            if (abs(shoulderangle1) <= config.GetConfig(Config::CONFIG_ASIDE_HEAD_ANGLE_MIN))
                SET_RESULE(5);
        }
    }

    if (noWrong)
    {
        info[INFO_ERROR_CODE] = 0;
    }
    if (config.GetStatus(Config::CONBIT_TIME))
    {
        info[INFO_ERROR_CODE] = -1;
    }

}
