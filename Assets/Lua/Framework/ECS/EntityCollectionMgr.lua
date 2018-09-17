--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-07 22:47:13
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

local EntityCollection = require("Framework.ECS.EntityCollection")

--@return [Framework.ECS.EntityCollectionMgr#M]
function M.New(...)
    return M(...)
end
function M:ctor()
    M.super.ctor(self)
    self.tmEntity = {}
    self.tmEntityCollection= {}
    self.tlEntityCollection = {}
end

function M:dispose()
    for _, collection in pairs(self.tmEntityCollection) do
        LuaObject.Destroy(collection)
    end
    self.tmEntityCollection = nil
    for _, entity in pairs(self.tmEntity) do
        LuaObject.Destroy(entity)
    end
    self.tmEntity = nil
    M.super.dispose(self)
end

--@region entity


function M:AddEntity(entity)
    assert(self.tmEntity[entity.id] == nil)
    
    self.tmEntity[entity.id] = entity

    for _, collection in ipairs(self.tlEntityCollection) do
        collection:AddEntity(entity)
    end

end

function M:RemoveEntity(id)
    local entity = self.tmEntity[id]
    assert(entity~=nil)

    self.tmEntity[id] = nil

    for _, collection in ipairs(self.tlEntityCollection) do
        collection:RemoveEntity(id)
    end

    LuaObject.Destroy(entity)
end

function M:GetEntity(id)
    local entity = self.tmEntity[id]
    if not entity then
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
--@endregion

--@region collection
function M:GetEntityCollection(tlComName)
    table.sort(tlComName)
    local finalKey = ""
    for _, comName in ipairs(tlComName) do
        finalKey = finalKey..comName
    end

    local collection = self.tmEntityCollection[finalKey]
    if collection==nil then
        collection = EntityCollection.New(tlComName)        
        self.tmEntityCollection[finalKey] = collection
        self.tlEntityCollection[#self.tlEntityCollection + 1] = collection
    end

    return collection
end

function M:GetEntityCollectionByCheckFunction(fCheckFunction)
    local collection = self.tmEntityCollection[fCheckFunction]
    if collection==nil then
        collection = EntityCollection.New(nil, fCheckFunction)        
        self.tmEntityCollection[fCheckFunction] = collection
        self.tlEntityCollection[#self.tlEntityCollection + 1] = collection
    end

    return collection
end

--@endregion


return M