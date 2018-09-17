﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class FairyGUI_GProgressBarWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(FairyGUI.GProgressBar), typeof(FairyGUI.GComponent));
		L.RegFunction("TweenValue", TweenValue);
		L.RegFunction("Update", Update);
		L.RegFunction("Setup_AfterAdd", Setup_AfterAdd);
		L.RegFunction("Dispose", Dispose);
		L.RegFunction("New", _CreateFairyGUI_GProgressBar);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("titleType", get_titleType, set_titleType);
		L.RegVar("max", get_max, set_max);
		L.RegVar("value", get_value, set_value);
		L.RegVar("reverse", get_reverse, set_reverse);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateFairyGUI_GProgressBar(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				FairyGUI.GProgressBar obj = new FairyGUI.GProgressBar();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: FairyGUI.GProgressBar.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int TweenValue(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)ToLua.CheckObject<FairyGUI.GProgressBar>(L, 1);
			double arg0 = (double)LuaDLL.luaL_checknumber(L, 2);
			float arg1 = (float)LuaDLL.luaL_checknumber(L, 3);
			FairyGUI.GTweener o = obj.TweenValue(arg0, arg1);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Update(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)ToLua.CheckObject<FairyGUI.GProgressBar>(L, 1);
			double arg0 = (double)LuaDLL.luaL_checknumber(L, 2);
			obj.Update(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Setup_AfterAdd(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)ToLua.CheckObject<FairyGUI.GProgressBar>(L, 1);
			FairyGUI.Utils.ByteBuffer arg0 = (FairyGUI.Utils.ByteBuffer)ToLua.CheckObject<FairyGUI.Utils.ByteBuffer>(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
			obj.Setup_AfterAdd(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Dispose(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)ToLua.CheckObject<FairyGUI.GProgressBar>(L, 1);
			obj.Dispose();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_titleType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			FairyGUI.ProgressTitleType ret = obj.titleType;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index titleType on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_max(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			double ret = obj.max;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index max on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_value(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			double ret = obj.value;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index value on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_reverse(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			bool ret = obj.reverse;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index reverse on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_titleType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			FairyGUI.ProgressTitleType arg0 = (FairyGUI.ProgressTitleType)ToLua.CheckObject(L, 2, typeof(FairyGUI.ProgressTitleType));
			obj.titleType = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index titleType on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_max(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			double arg0 = (double)LuaDLL.luaL_checknumber(L, 2);
			obj.max = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index max on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_value(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			double arg0 = (double)LuaDLL.luaL_checknumber(L, 2);
			obj.value = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index value on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_reverse(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			FairyGUI.GProgressBar obj = (FairyGUI.GProgressBar)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.reverse = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index reverse on a nil value");
		}
	}
}

