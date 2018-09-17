--@SuperType [Framework.LuaObject#M]
local M = class(...)

--@return [Framework.FSM.FSMState#M]
function M.New(...)
    return M(...)
end

function M:ctor(fsm, name)
    M.super.ctor(self)
    self.fsm = fsm
    self.name = name
end

function M:GetName()
    return self.name
end

--@region 子类重写
function M:OnEnter()
end

function M:OnExit()
end

function M:OnUpdate()
end
--@endregion


return M