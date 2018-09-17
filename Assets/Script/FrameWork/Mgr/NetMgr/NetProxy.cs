using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

namespace FW
{

    public class NetProxy 
    {
        public Action<bool> OnConnect;
        public Action OnDisconnect;
        public Action<byte[]> OnReceivedMessage;

        private Net socket;
        int slot;

        
        

        bool isApplicationQuit = false;

        Net SocketClient
        {
            get
            {
                if (socket == null)
                    socket = new Net(this.slot);
                return socket;
            }
        }

        public NetProxy(int slot)
        {
            this.slot = slot;
            this.socket = new Net(slot);
        }

        /// <summary>
        /// 发送链接请求
        /// </summary>
        public void Connect(string host, int port, bool noDelay)
        {
            SocketClient.Connect(host, port, noDelay,
            delegate (bool suc) {
                if (suc)
                {
                    OnConnect(true);
                }
                else
                {
                    OnConnect(false);
                    
                }
            },
            delegate () {
                OnDisconnect();
            });
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            SocketClient.Close();
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        public void Send(byte[] buffer)
        {
            SocketClient.Send(buffer);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        internal void OnDestroy()
        {
            if (isApplicationQuit)
            {
                SocketClient.SwitchToIdle();
            }
            else
            {
                SocketClient.Close();
            }

        }

        internal void OnApplicationQuit()
        {
            isApplicationQuit = true;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Update()
        {
            while (true)
            {
                byte[] buff = SocketClient.Receive();
                if (buff == null)
                {
                    break;
                }
                OnReceivedMessage(buff);
            }
            SocketClient.Update();
        }
    }

}