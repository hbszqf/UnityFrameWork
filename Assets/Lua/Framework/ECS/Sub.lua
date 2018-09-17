--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-07 22:51:37
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.ECS.Sub#M]
function M.New(...)
    return M(...)
end
function M:ctor(system, entity)
    M.super.ctor(self)
    --@RefType [Framework.ECS.System#M]
    self.system = system
    --@RefType [Framework.ECS.ECS#M]
    self.ecs = system.ecs
    --@RefType [Framework.ECS.Entity#M]
    self.entity = entity
    --id
    self.id = self.entity.id
    self.isStart = false
end

function M:dispose()
    M.super.dispose(self)
end

function M:GetSub(systemName)
    local sub = self:TryGetSub(systemName)
    if not sub then
        assert(false, string.format("不存在sub %s %s", systemName, self.shortname))
    end
    return sub
end

function M:TryGetSub(systemName)
    local system = self.ecs:GetSystem(systemName)
    return system:GetSub(self.id)
end


function M:InitWrap()
    if not self.isInit then
        self.isInit = true
        self:Init()
    end
end

function M:ReleaseWrap()
    if  self.isInit then
        self.isInit = false
        self:Release()
    end
end

function M:Init()
end

function M:Release()
end

return M