using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
namespace FW
{
    class LuaMgr : BaseMgr
    {
        private static LuaMgr _inst = null;
        public static LuaMgr inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = Mgr._inst.AddMgr<LuaMgr>();
                }
                return _inst;
            }
        }

        public void StartUp() {

            LuaConst.luaResDir = ResHelper.GetLuaFolder();


            LuaState lua = new LuaState();

            //重新创建lua读取
            new LuaFileUtils();

            //设置lua搜索路径

#if UNITY_EDITOR
            //编辑器模式下永远使用 Assets下的lua
            lua.AddSearchPath(LuaConst.luaDir);
            lua.AddSearchPath(LuaConst.toluaDir);
#else
            //非编辑器模式下永远使用Res下的Lua
            lua.AddSearchPath(ResHelper.GetLuaFolder());
#endif



            lua.OpenLibs(LuaDLL.luaopen_pb);
            lua.OpenLibs(LuaDLL.luaopen_struct);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);
            //lua.OpenLibs(luaopen_protobuf_c);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            luaState.OpenLibs(LuaDLL.luaopen_bit);
#endif



            if (LuaConst.openLuaSocket)
            {
                lua.BeginPreLoad();
                lua.RegFunction("socket.core", LuaOpen_Socket_Core);
                lua.RegFunction("mime.core", LuaOpen_Mime_Core);
                lua.EndPreLoad();
            }

            lua.LuaSetTop(0);

            //启动lua
            lua.Start();
            LuaBinder.Bind(lua);
            DelegateFactory.Init();
            lua.DoFile("Main.lua");
            

            this.gameObject.AddComponent<LuaLooper>().luaState = lua;
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int LuaOpen_Socket_Core(IntPtr L)
        {
            return LuaDLL.luaopen_socket_core(L);
        }

        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int LuaOpen_Mime_Core(IntPtr L)
        {
            return LuaDLL.luaopen_mime_core(L);
        }


        #region pbc
#if !UNITY_EDITOR && UNITY_IPHONE
        const string LUADLL = "__Internal";
#else
        const string LUADLL = "tolua";
#endif
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_protobuf_c(IntPtr L);
        #endregion
    }
}
