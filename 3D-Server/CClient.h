#ifndef CCLIENT_H
#define CCLIENT_H


#include <sys/types.h>
#include <sys/socket.h>
#include <stdio.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <string.h>
#include <stdlib.h>
#include <fcntl.h>
#include <sys/shm.h>
#include <pthread.h>
#include <semaphore.h>
#include <iostream>
#include <termios.h>
#include <assert.h>
#include <time.h>

using namespace std;

class CClient{
public:
    int sock_cli;
    int buflen;
    pthread_t receive,sendmessage;
    char recvbuf[10240];
    char sendbuf[10240];
    bool isvalid;
    int link;
    bool init(char *ipaddress,int port){
        sock_cli = socket(AF_INET,SOCK_STREAM, 0);
        buflen=0;
        link=0;
        struct sockaddr_in servaddr;
        memset(&servaddr, 0, sizeof(servaddr));
        servaddr.sin_family = AF_INET;
        servaddr.sin_port = htons(port);  ///服务器端口
        servaddr.sin_addr.s_addr = inet_addr(ipaddress);  ///服务器ip

        ///连接服务器，成功返回0，错误返回-1
        if (connect(sock_cli, (struct sockaddr *)&servaddr, sizeof(servaddr)) < 0)
        {
            perror("disconnect");
            exit(1);
            isvalid=false;
        }
        else{
            isvalid=true;
            link++;
            fputs("Server has connected successfully!\n", stdout);
        }
        sockaddr_in addrMy;
         memset(&addrMy,0,sizeof(addrMy));
         socklen_t len = sizeof(addrMy);
            getsockname(sock_cli,(struct sockaddr*)&addrMy,&len);
            return true;
    }

    void sendMsg(char *sendmsg){//发送消息
        send(sock_cli, sendmsg, strlen(sendmsg), 0);
        return;
    }

    void getMsg(){//获取消息
        unsigned int t=clock();
        while(1){
            if(clock()-t>100000)
            {
                isvalid=false;
                return;
            }
        buflen = recv(sock_cli, recvbuf, sizeof(recvbuf),0);
        if(buflen >= 1){
            return;
        }
        }
    }
};


#endif // CCLIENT_H
