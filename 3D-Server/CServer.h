#ifndef CSERVER_H
#define CSERVER_H


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

class CServer{
public:
    int conn;
    int buflen;
    int server_sockfd;
    int QUEUE;
    int link;
    pthread_t receive,sendmessage;
/*    struct recv_buf_t {
    int32_t TEMP;
    int32_t FHX;
    int32_t FHY;
    int32_t FHZ;
    int32_t NX;
    int32_t NY;
    int32_t NZ;
    int32_t LHX;
    int32_t LHY;
    int32_t LHZ;
    int32_t RHX;
    int32_t RHY;
    int32_t RHZ;
    int32_t LSX;
    int32_t LSY;
    int32_t LSZ;
    int32_t RSX;
    int32_t RSY;
    int32_t RSZ;
    int32_t HAX;
    int32_t HAY;
    int32_t HAZ;
    int32_t SAX;
    int32_t SAY;
    int32_t SAZ;
    int32_t LEX;
    int32_t LEY;
    int32_t LEZ;
    int32_t REX;
    int32_t REY;
    int32_t REZ;
    int32_t TX;
    int32_t TY;
    int32_t TZ;
    int32_t SIZE;
    int32_t SDI;
    int32_t SDA;
    int32_t SW;
    int32_t SL;
    int32_t HAFNB;
    int32_t HALNRI;
    int32_t HALNRA;
    int32_t WA;
    int32_t HASI;
    int32_t HASA;
    int32_t T;
    } recv_buf;
    struct post_{
    } recv_buf;*/

    char recvbuf[1024];
    char sendbuf[1024];
    bool isvalid;
    bool init(int port){
        link=0;
        buflen=0;
        QUEUE=20;
        ///定义sockfd
        server_sockfd = socket(AF_INET,SOCK_STREAM, 0);//socket描述字
        //AF_INET 协议域，又称为协议族（family）将ipv4地址和端口号组合赋给socket
        //SOCK_STREAM 指定socket类型
        //0 指定协议
        //返回的socket描述字它存在于协议族（address family，AF_XXX）空间中，但没有一个具体的地址。
        //如果想要给它赋值一个地址，就必须调用bind()函数，否则就当调用connect()、listen()时系统会自动随机分配一个端口。

        ///定义sockaddr_in
        struct sockaddr_in server_sockaddr;//ipv4对应的地址协议族
        server_sockaddr.sin_family = AF_INET;
        server_sockaddr.sin_port = htons(port);//端口号？8887
        server_sockaddr.sin_addr.s_addr = htonl(INADDR_ANY);//internet address
    //s_addr address in network byte order
        ///bind，成功返回0，出错返回-1
        if(bind(server_sockfd,(struct sockaddr *)&server_sockaddr,sizeof(server_sockaddr))==-1)
            //server_sockfd 即socket描述字，它是通过socket()函数创建了，唯一标识一个socket。
            //bind()函数就是将给这个描述字绑定一个名字
            //server_sockaddr 指向要绑定给sockfd的协议地址。
            //这个地址结构根据地址创建socket时的地址协议族的不同而不同
            //sizeof(server_sockaddr) 对应的是地址的长度
            //绑定一个众所周知的地址（如ip地址+端口号），用于提供服务，客户就可以通过它来接连服务器
        {
            perror("bind");
            exit(1);
        }

        ///listen，成功返回0，出错返回-1
        if(listen(server_sockfd,QUEUE) == -1)//define Queue 20
            //server_sockfd 要监听的socket描述字
            //QUEUE 相应socket可以排队的最大连接个数 这里是20
            //listen函数将socket变为被动类型的，等待客户的连接请求
        {
            perror("listen");//用来将上一个函数发生错误的原因输出到标准设备(stderr)。
            //参数 s 所指的字符串会先打印出，后面再加上错误原因字符串。
            exit(1);
        }

        ///客户端套接字

        struct sockaddr_in client_addr;//ipv4地址协议族
        socklen_t length = sizeof(client_addr);//对应地址长度

        ///成功返回非负描述字，出错返回-1
        conn = accept(server_sockfd, (struct sockaddr*)&client_addr, &length);
        //server_sockfd服务器socket描述字
        //client_addr指向struct sockaddr *的指针，用于返回客户端的协议地址
        //length协议地址的长度
        //如果accpet成功，那么其返回值是由内核自动生成的一个全新的描述字，代表与返回客户的TCP连接
        if(conn<0)
        {
            isvalid=false;
            perror("connect");
            exit(1);
        }

        if (conn>=0){
            link++;
            fputs("Client has connected successfully!\n", stdout);
            isvalid=true;
        }
//        pthread_create(&receive,NULL,Receive,&conn);
 //       cout<<inet_ntoa(client_addr.sin_addr)<<" Connect to Server!"<<endl;
        return true;
    }

    void* Receive(void *args){//接收消息
        int conn=(int)(*((int*)args));
        while(1){
    //    memset(buffer,0,sizeof(buffer));//将buffer中前buffer个字节的数据用0替代并写入buffer
        int leng = recv(conn, recvbuf, sizeof(recvbuf),0);//返回的是buffer长度len?
        //recv()用于已连接的数据报或流式套接口进行数据的接收
        //返回所有可用的信息，最大可达缓冲区的大小
        if(strcmp(recvbuf,"exit\n")==0)//如果字符串是exit则退出
            exit(0);
        fputs(recvbuf, stdout);//将buffer内容打印在控制台
        memset(recvbuf, 0, sizeof(recvbuf));
        }
    }

    void* SendMessage(void *args){//发送消息
        int conn=(int)(*((int*)args));
 //       char sendbuf[256]=(char)(*((char*)args));
        while(1){
    //        fflush(stdin);
    //        sendbuf[0] = getch();
        fgets(sendbuf, sizeof(sendbuf), stdin) ;
    //    cout << '\r';
        send(conn, sendbuf, strlen(sendbuf), 0);
        //send()用于向一个已经连接的socket发送数据，如果无错误，返回值为所发送数据的总数，否则返回SOCKET_ERROR
        memset(sendbuf, 0, sizeof(sendbuf));
        }
    }

    void sendMsg(char *sendmsg,int size=0){//发送消息
//        pthread_create(&sendmessage,NULL,SendMessage,send);
        if (size==0){
            size=strlen(sendmsg);
        }
        send(conn, sendmsg, size, 0);
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
        buflen = recv(conn, recvbuf, sizeof(recvbuf),0);
        if(buflen >= 1){
            return;
        }
        }
    }

};

#endif // CSERVER_H
