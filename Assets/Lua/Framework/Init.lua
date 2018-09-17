--初始化一些扩展
require("Framework.Extends.Init")

--@region Unity全局变量
GameObject = UnityEngine.GameObject
UObject = UnityEngine.Object
KeyCode = UnityEngine.KeyCode
Input = UnityEngine.Input


--@endregion

--@region  Fairy全局变量
GComponent = FairyGUI.GComponent
GRoot = FairyGUI.GRoot.inst
GGraph = FairyGUI.GGraph

--@endregion


--@region c#全局变量
PlayableProxy = FW.PlayableProxy


--@endregion


--所有Lua对象的基类
LuaObject = require("Framework.LuaObject")



--面向对象系统
class = function(classname, Super)
    local ret = (Super or LuaObject):Extend(classname)
    return ret
end

function IsNull(obj)
    if obj == nil then
        return true
    end

    if type(obj) == "table" then
        return LuaObject.IsNull(obj)
    end

    if type(obj) == "userdata" then
        return tolua.isnull(obj)
    end
end

--日志系统
Log = require("Framework.Utils.Log")

--luatable快速索引
TableIndex = require("Framework.Utils.TableIndex")

--事件代理
EventProxy = require("Framework.Event.EventProxy")

--Promise
Promise = require("Framework.Utils.Promise")

--内存泄露检测
MemLeak = require("Framework.Utils.MemLeak").GetInstance()

--协程小工具
CorUtil = require("Framework.Coroutine.CorUtil")

--任务队列
TaskQueue = require("Framework.Coroutine.TaskQueue")

--FairyMgr
FairyMgr = require("Framework.Fairy.FairyMgr").GetInstance()

--配置表代理
RefProxy = require("Framework.Utils.RefProxy")

--资源管理
ResMgr = require("Framework.Res.ResMgr").GetInstance()

--ECS
ECS = require("Framework.ECS.ECS")

--预置物管理
PrefabMgr = require("Framework.Prefab.PrefabMgr").GetInstance()

--有限状态机
FSM = require("Framework.FSM.FSM")

--GameActivity管理
GSM = require("Framework.GSM.GSM")

















				

