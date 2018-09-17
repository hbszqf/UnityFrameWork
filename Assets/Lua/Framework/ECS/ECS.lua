--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-07 24:02:35
--
local EntityCollectionMgr = require("Framework.ECS.EntityCollectionMgr")
--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)
M.Entity = require("Framework.ECS.Entity")
M.System = require("Framework.ECS.System")
M.Sub = require("Framework.ECS.Sub")

--@region ComProperty

local ComProperty = {}
ComProperty.optional = false
ComProperty.type = "number"
ComProperty.name = nil --name 为nil 代表此字段直接赋值为com本身

--@endregion

--@region 构造函数

--@return [Framework.ECS.ECS#M]
function M.New(...)
    return M(...)
end
function M:ctor(tlClass)
    M.super.ctor(self)

    --待添加实体
    self.tlEntity_add= {}
    self.tmEntity_add = {}

    --带删除实体
    self.tlEntity_remove = {}
    self.tmEntity_remove = {}

    --实体集合管理器
    self.entityCollectionMgr = EntityCollectionMgr.New()

    --系统
    self.tlSystem = {}
    self.tmSystem = {}

    --
    self.genId = 0

    --当前时间 单位毫秒
    self.nowTime = 0

    --和上一帧 间隔多少
    self.deltaTime = 0
    
    --构造所有系统
    for index, systemClass in ipairs(tlClass) do
        local system = systemClass.New(self)
        self.tlSystem[index] = system
        self.tmSystem[systemClass.shortname] = system
    end
end

function M:dispose()
    --销毁所有系统
    for i = #self.tlSystem, 1, -1 do
        local system = self.tlSystem[i]
        LuaObject.Destroy(system)
    end

    --销毁所有实体
    LuaObject.Destroy(self.entityCollectionMgr)
    self.entityCollectionMgr = nil

    --系统清空
    self.tlSystem = nil
    self.tmSystem = nil

    --清理 临时实体
    for _, entity in ipairs(self.tlEntity_add) do
        LuaObject.Destroy(entity)
    end
    for _, entity in ipairs(self.tlEntity_remove) do
        LuaObject.Destroy(entity)
    end

    self.tlEntity_add = nil
    self.tmEntity_add = nil
    self.tlEntity_remove = nil
    self.tmEntity_remove = nil

    --基类销毁
    M.super.dispose(self)
end
--@endregion

--@return [Framework.ECS.System#M]
function M:GetSystem(shortname)
    return self.tmSystem[shortname]
end

--@return [Framework.ECS.System#M]
function M:GetTlSystem()
    return self.tlSystem
end

--@desc 获取下一个id
function M:GetNextId()
    self.genId = self.genId + 1
    return self.genId
end

--@desc 获取当前时间
function M:GetNowTime()
    return self.nowTime
end

--@desc 获取间隔时间
function M:GetDeltaTime()
    return self.deltaTime
end

--添加当前时间
function M:AddNowTime(deltaTime)
    deltaTime = math.floor(deltaTime)
    self.nowTime = self.nowTime + deltaTime
    self.deltaTime = deltaTime
end

--@return [Framework.ECS.ECS#ComProperty]
function M:GetTlComProperty(name)
    assert(false, "GetTlComProperty")
end

--@region Entity


function M:AddEntity(entity, imm)
    local id = entity.id

    if self.tmEntity_add[id] or self.entityCollectionMgr:TryGetEntity(id) then
        assert(false, "AddEntity重复添加")
    end

    if imm then
        self.entityCollectionMgr:AddEntity(entity)
    else
        self.tlEntity_add[#self.tlEntity_add+1] = entity
        self.tmEntity_add[id] = entity
    end
    return entity
end

function M:RemoveEntity(id, imm)
    if self.tmEntity_add[id] ~= nil and self.entityCollectionMgr:TryGetEntity(id) ~= nil then
        assert(false, "RemoveEntity删除不存在对象")
    end

    if imm then
        self.entityCollectionMgr:RemoveEntity(id)
    else
        --已经存在删除列表
        if self.tmEntity_remove[id] then
            return
        end

        --存在添加列表
        if self.tmEntity_add[id] then
            local entity_add = self.tmEntity_add[id]
            self.tmEntity_add[id] = nil
            table.removebyvalue(self.tlEntity_add, entity_add)
            LuaObject.Destroy(entity_add)


        else
            local entity = self.entityCollectionMgr:GetEntity(id)
            self.tlEntity_remove[#self.tlEntity_remove + 1] = entity
            self.tmEntity_remove[id] = entity
        end
    end
end

function M:GetEntity(id)
    return self.entityCollectionMgr:GetEntity(id)
end

function M:GetTmEntity()
    return self.entityCollectionMgr:GetTmEntity()
end
--@endregion


--@region pipeline


function M:AddOrRemovePipeline()
    local tlEntity_add = self.tlEntity_add
    if #tlEntity_add > 0 then
        for _, entity in ipairs(tlEntity_add) do
            self.entityCollectionMgr:AddEntity(entity)
        end
        self.tlEntity_add = {}
        self.tmEntity_add = {}
    end

    local tlEntity_remove = self.tlEntity_remove
    if #tlEntity_remove > 0 then
        for _, entity in ipairs(tlEntity_remove) do
            self.entityCollectionMgr:RemoveEntity(entity.id)
        end
        self.tlEntity_remove = {}
        self.tmEntity_remove = {}
    end
end

function M:BeforeUpdatePipeline()
    for _, system in ipairs(self.tlSystem) do
        system:BeforeUpdate()
    end
end

function M:UpdatePipeline()
    for _, system in ipairs(self.tlSystem) do
        system:Update()
    end
end

function M:LateUpdatePipeline()
    for _, system in ipairs(self.tlSystem) do
        system:LateUpdate()
    end
end

function M:DefaultPipeline()
    self:AddOrRemovePipeline()

end
--@endregion

return M