using UnityEngine;
using System.Collections;

namespace FW
{
    public class Log 
    {
        /// <summary>
        /// 通用trace函数。一般请不要使用这个函数来进行记录，除非确认前端同事都需要相同调试信息。
        /// </summary>
        /// <param name='msg'>
        /// Message.
        /// </param>
        public static void Print(object msg)
        {
            UnityEngine.Debug.Log(string.Format("<color=#23A8FF>[Public]{0:0.00}:</color>{1}", Time.realtimeSinceStartup, msg));   
        }
    }

}
