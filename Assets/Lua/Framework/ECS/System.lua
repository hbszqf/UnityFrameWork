--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-07 22:50:54
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.ECS.System#M]
function M.New(...)
    return M(...)
end
function M:ctor(ecs)
    M.super.ctor(self)

    

    --@RefType [Framework.ECS.ECS#M]
    self.ecs = ecs
    --@RefType [Framework.ECS.EntityCollectionMgr#M]
    self.entityCollectionMgr = ecs.entityCollectionMgr
    --@RefType [Framework.ECS.EntityCollection#M]
    self.entityCollection = self.entityCollectionMgr:GetEntityCollection(self:GetTlComName())

    --监听添加 这里监听回调的次序不确定 以后再优化
    self.entityCollection.eventProxy:AddListener("add",self.OnAdd,self)
    self.entityCollection.eventProxy:AddListener("remove", self.OnRemove, self)

    --@RefType [Framework.ECS.Sub#M]
    self.tmSub = {}
end

function M:dispose()
    self.entityCollection.eventProxy:RemoveListener("add", self.OnAdd, self)
    self.entityCollection.eventProxy:RemoveListener("remove", self.OnRemove, self)

    for _, sub in pairs(self.tmSub) do
        sub:ReleaseWrap()
        LuaObject.Destroy(sub)
    end
    self.tmSub = nil
    M.super.dispose(self)
end

function M:GetECS()
    return self.ecs
end

function M:GetSystem(shortname)
    return self.ecs:GetSystem(shortname)
end

function M:GetTlEntity()
    return self.entityCollection:GetTlEntity()
end

function M:GetEntity(id)
    return self.entityCollection:GetEntity(id)
end

function M:TryGetEntity(id)
    return self.entityCollection:TryGetEntity(id)
end

function M:GetSub(id)
    local sub = self.tmSub[id]
    if not sub then
        assert(false, "没有此sub:"..id)
    end
    return sub
end

function M:TryGetSub(id)
    return self.tmSub[id]
end

--@region sub
function M:OnAdd(entity)
    local subClass = self:GetSubClass(entity)
    if subClass then
        local sub = subClass.New(self, entity)
        self.tmSub[entity.id] = sub
    end
end
function M:OnRemove(entity)
    local sub = self.tmSub[entity.id]
    if sub~=nil then
        self.tmSub[entity.id] = nil
        sub:ReleaseWrap()
        LuaObject.Destroy(sub)
    end
end

--@endregion



--@region 生命周期
function M:PeformSub(funcName)
    local tlEntity = self:GetTlEntity()

    if #tlEntity > 0 then
        for _, entity in ipairs(tlEntity) do
            local sub = self.tmSub[entity.id]
            if sub then
                local func = sub[funcName]

                if func then
                    func(sub)
                end
            end
        end
    end

end
function M:BeforeUpdate()
    self:PeformSub("BeforeUpdate")
end

function M:Update()
    self:PeformSub("InitWrap")
    self:PeformSub("Update")
end

function M:LateUpdate()
    self:PeformSub("LateUpdate")
end

--@endregion


--@region 重要需要重写的函数
function M:GetTlComName()
    return {}
end

function M:GetSubClass(entity)
    return nil
end

--@endregion


return M