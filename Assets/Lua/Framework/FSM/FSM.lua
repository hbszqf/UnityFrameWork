
--@SuperType [Framework.LuaObject#M]
local M = class(...)

--@return [Framework.FSM.FSM#M]
function M.New(...)
    return M(...)
end

function M:ctor()
    M.super.ctor(self)
    --这个状态机的所有状态
    self.tmFSMState = {}
    self.fsmState = nil --当前状态
end

function M:dispose()
    for _, state in pairs( self.tmFSMState ) do
        state:dispose()
    end
    self.tmFSMState = nil
    self.fsmState = nil
end

function M:AddState(name, class_fsmState)
    self.tmFSMState[name] = class_fsmState.New(self, name)
end

function M:SwitchTo(name, ...)
    
    local newState = self.tmFSMState[name]
    local oldState = self.fsmState

    if newState == oldState then
        return
    end

    if oldState ~= nil then
        oldState:OnExit(newState, newState and newState:GetName())
    end

    self.fsmState = newState

    if newState ~= nil then        
        newState:OnEnter()
    end

    self:OnSwitch(oldState and oldState:GetName(), newState and newState:GetName())   
end



function M:Update()
    if self.fsmState~=nil then
        self.fsmState:OnUpdate()
    end
end

function M:GetCurrentState()
    return self.fsmState
end

function M:GetCurrentStateName()
    return self.fsmState:GetName()
end

function M:GetFSMState(name)
    return self.tmFSMState[name]
end

--@region 子类可重写
function M:OnSwitch(lastName, nextName)
end

--@endregion


return M