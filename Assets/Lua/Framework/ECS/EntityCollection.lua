--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-07 23:40:56
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.ECS.EntityCollection#M]
function M.New(...)
    return M(...)
end
function M:ctor(tlComName, fCheckFunc)
    M.super.ctor(self)
    self.eventProxy = EventProxy.New({"add","remove"})
    self.tmEntity = {}
    self.tlEntity = {}
    self.tlComName = tlComName
    self.fCheckFunc = fCheckFunc
end

function M:dispose()
    self.tlComName = nil
    self.tlEntity = nil
    self.tmEntity = nil
    LuaObject.Destroy(self.eventProxy)
    self.eventProxy = nil
    M.super.dispose(self)
end

function M:CheckEntity(entity)
    if self.fCheckFunc then
        return self.fCheckFunc(entity)
    end

    if #self.tlComName == 0 then
        return false
    end
    for _, comName in ipairs(self.tlComName) do
        if entity:GetCom(comName) == nil then
            return false
        end
    end
    return true
end

function M:AddEntity(entity)
    assert(self.tmEntity[entity.id]==nil)
    if self:CheckEntity(entity) then
        self.tmEntity[entity.id] = entity
        self.tlEntity[#self.tlEntity+1] = entity
        self.eventProxy:Broacast("add",entity)
    end
end

function M:RemoveEntity(id)
    local entity = self.tmEntity[id]
    if entity ~= nil then
        self.tmEntity[entity.id] = nil
        table.removebyvalue(self.tlEntity,entity)
        self.eventProxy:Broacast("remove",entity)
    end
end

function M:GetEntity(id)
    local entity = self.tmEntity[id]
    if entity == nil then
        assert(false, "没有此entity:"..id)
    end
    return entity
end

function M:TryGetEntity(id)
    return self.tmEntity[id]
end

function M:GetTmEntity()
    return self.tmEntity
end

function M:GetTlEntity()
    return self.tlEntity
end

return M