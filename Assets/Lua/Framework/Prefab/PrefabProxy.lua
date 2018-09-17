--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-18 23:20:24
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@region 构造析构

--@reuturn [Framework.Prefab.PrefabProxy#M]
function M.New(...)
    return M(...)
end
function M:ctor(prefabMgr, assetPath)
    M.super.ctor(self)
    self.prefabMgr = prefabMgr
    self.assetPath = assetPath

    local tl = string.split(assetPath,"/")

    --
    self.go = UnityEngine.GameObject.New(tl[#tl])
    self.go:SetActive(false)
    self.trans = self.go.transform
    self.trans:SetParent(self.prefabMgr:GetTransform())
    

    --加载asset相关
    self.asset = nil
    self.promise_asset = self:LoadAssetAsync()
    
    --缓存中的
    self.tlCache = {}
    --使用中的
    self.tlInUse = {}


end
--@endregion

--@return [Framework.Utils.Promise#M]
function M:LoadAssetAsync()
    local function fTask(resolve, reject)
        CsProxy.LoadAsset(self.assetPath, function(asset)
            if asset then
                self.asset = nil
                --解决
                resolve(asset)
            else
                --拒绝
                reject(asset)

            end
            self.promise_asset = nil
        end)  
    end

    return Promise.New(fTask)
end

function M:GetFromPoolAsync()
    --@TODO 2018-08-19 10:12:27 lua gc 性能有待优化
    local function fTask(resolve, reject)
        --处理GO
        local function HandleGO(go)
            if not go then
                reject()
            else
                --插入使用中的队列
                self.tlInUse[#self.tlInUse+1] = go
                local ret = resolve(go)
                if not ret then
                    self:ReturnToPool()
                end
            end    
        end

        --处理Asset
        local function HandleAsset(asset)
            if not asset then
                HandleGO(nil)
                return 
            end
            local go = UnityEngine.GameObject.Instantiate(asset)
            HandleGO(go)
        end

        --主逻辑
        do
            --缓存中还存在
            local go = self:GetFromPool()
            if go then
                HandleGO(go)
                return
            end

            --正在加载asset
            if self.promise_asset then
                self.promise_asset:AddListener(function(ret, asset) HandleAsset(asset) end)
                return
            end
            
            --asset加载完成
            HandleAsset( self.asset )
        end
    end
    return Promise.New(fTask)
end

function M:GetFromPool()
    --缓存中是否有
    local tlCache = self.tlCache
    local cacheCount = #tlCache
    if cacheCount<=0 then
        return nil
    end

    --从Cache移动到InUse
    local go = tlCache[cacheCount]
    tlCache[cacheCount] = nil
    local tlInUse = self.tlInUse
    tlInUse[#tlInUse+1] = go

    --返回
    return go
end

function M:ReturnToPool(go)
    if not self:TryReturnToPool() then
        assert(false, "回池对象不存在")
    end
end

function M:TryReturnToPool(go)
    --删除使用中的
    local tlInUse = self.tlInUse
    

    local index = table.indexof(tlInUse, go)
    if not index then
        return false
    end
    table.remove(tlInUse, index)

    --放入缓存池
    local tlCache = self.tlCache
    tlCache[#tlCache+1] = go

    --@RefType [luaIde#UnityEngine.Transform]
    local transform = go.transform
    transform:SetParent(self.trans)
    transform.localPosition = Vector3.zero
    transform.localEulerAngles = Vector3.zero
    transform.localScale = Vector3.one
    return true
end


return M