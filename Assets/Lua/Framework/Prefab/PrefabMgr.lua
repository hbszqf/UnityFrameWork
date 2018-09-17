--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-18 22:46:40
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

local PrefabProxy = require("Framework.Prefab.PrefabProxy")

--@region 构造析构
local M_instance = nil
--@return [Framework.Prefab.PrefabMgr#M]
function M.GetInstance()
    if not M_instance then
        M_instance = M()
    end
    return M_instance
end

function M:ctor()
    M.super.ctor(self)
    self.go = UnityEngine.GameObject.New("PrefabMgr")
    self.transform = self.go.transform
    --@RefType [Framework.Prefab.PrefabProxy#M]
    self.tmPrefabProxy = {}
    UnityEngine.GameObject.DontDestroyOnLoad(self.go)
end
--@endregion

function M:GetGO()
    return self.go
end

function M:GetTransform()
    return self.transform
end

--@return [Framework.Utils.Promise#M]
function M:GetFromPoolAsync(assetPath)
    local prefabProxy = self:GetPrefabProxy(assetPath)
    return prefabProxy:GetFromPoolAsync()
end

function M:ReturnToPool(go, assetPath)
    if assetPath then
        local prefabProxy = self:GetPrefabProxy(assetPath)
        prefabProxy:ReturnToPool(go)
    else
        local ret = false
        for _, prefabProxy in pairs(self.tmPrefabProxy) do
            if prefabProxy:TryReturnToPool(go) then
                ret = true
                break
            end
        end
        if not ret then
            assert(false, "回池失败")
        end
    end
end

--@return [Framework.Prefab.PrefabProxy#M]
function M:GetPrefabProxy(assetPath)
    local tmPrefabProxy = self.tmPrefabProxy
    local prefabProxy = tmPrefabProxy[assetPath]
    if not prefabProxy then
        prefabProxy = PrefabProxy.New(self, assetPath)
        tmPrefabProxy[assetPath] = prefabProxy
    end
    return prefabProxy
end
return M