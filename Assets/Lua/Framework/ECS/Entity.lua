--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-07 22:51:23
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.ECS.Entity#M]
function M.New(ecs, id)
    return M(ecs, id)
end
function M:ctor(ecs, id)
    M.super.ctor(self)
    --@RefType [Framework.ECS.ECS#M]
    self.ecs = ecs
    self.id = id or self.ecs:GetNextId()
    self.log = nil
end

function M:dispose()
    M.super.dispose(self)
end

function M:GetCom(name)
    return self[name]
end

--@desc 添加com
function M:AddCom(name, com)
    local tlComProperty = self.ecs:GetTlComProperty(name)
    if not tlComProperty then
        assert(false, "没有此类型com:"..name)
    end
    if self[name] then
        assert(false, "已经存在类型com:" .. name)
    end
    if com == nil then
        assert(false, "com为空:" .. name)
    end

    for i = 1, #tlComProperty do
        local comProperty = tlComProperty[i]
        if type(com[comProperty.name]) ~= comProperty.type then
            assert(false, "属性类型错误:".. comProperty.name)
        end
    end
    self[name] = com
end

--@desc 更新一个com
function M:UpdateCom(name, com)
    local tlComProperty = self.ecs:GetTlComProperty(name)
    if not tlComProperty then
        assert(false, "没有此类型com:" .. name)
    end
    if not self[name] then
        assert(false, "不存在此类型com:" .. name)
    end
    if com == nil then
        assert(false, "com为空:" .. name)
    end
   
    if #tlComProperty <= 0 then
        self[name] = com
    else
        local oldCom = self[name]
        for i = 1, #tlComProperty do
            local comProperty = tlComProperty[i]
            local name = comProperty.name
            local newValue = com[name]
            if newValue then
                if type(newValue) ~= comProperty.type then
                    assert(false, "属性类型错误:" .. comProperty.name)
                end
                oldCom[name] = newValue
            end
            
        end
    end

end

function M:Dump()
    local ecs = self.ecs 
    local log = self.log
    self.ecs = nil
    NYLog:Dump(self)
    self.ecs = ecs
    self.log = log
end

function M:EnableLog(enabled)
    if enabled then
        self.log = Log.New(true):SetName(tostring(self.id))
    else
        self.log = nil
    end
end

function M:Print(...)
    if self.log then
        self.log:Print(...)
    end
end

return M