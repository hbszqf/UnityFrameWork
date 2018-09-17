--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-23 23:00:19
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@region 构造析构

--@return [Framework.GSM.GameActivityMgr#M]
function M.New(...)
    return M(...)
end
function M:ctor(sortingOrder, parent, taskQueue)
    M.super.ctor(self)

     if not parent then
        parent = FairyGUI.GRoot.inst
    end
    if not sortingOrder then
        sortingOrder = 0
    end
    if not taskQueue then
        taskQueue =  TaskQueue.New()
    end

    --@RefType [Framework.Coroutine.TaskQueue#M]
    self.taskQueue = taskQueue

    --@RefType [luaIde#FairyGUI.GComponent]
    self.gcomponent = GComponent.New()
    self.gcomponent.container.gameObject.name = self.shortname
    self.gcomponent.sortingOrder = sortingOrder
    
    parent:AddChild(self.gcomponent)

    --@RefType [Framework.GSM.GameActivity#M]
    self.tmGameActivity = {}
end

function M:dispose()
    self.gcomponent:Dispose()
    self.gcomponent = nil
    assert(false, "GameActivity暂时无法销毁")
    M.super.dispose(self)
end

--@endregion

--@return [luaIde#FairyGUI.GComponent]
function M:GetGComponent()
    return self.gcomponent
end

--@return [Framework.Coroutine.TaskQueue#M]
function M:GetTaskQueue()
    return self.taskQueue
end

--@return [Framework.GSM.GameActivity#M]
function M:CreateGameActivity(classOrName)
    local gameActivityClass = self:GetGameActivityClass(classOrName)
    --gameScene
    local gameActivity = gameActivityClass(self)
    return gameActivity
end

function M:GetGameActivityClass(classOrName)
    local GameActivityX = classOrName
    if type(GameActivityX) == "string" then
        GameActivityX = require(classOrName)
    end
    return GameActivityX
end

function M:AddGameActivity(gameActivity)
    self.tmGameActivity[gameActivity] = gameActivity
    self.gcomponent:AddChild(gameActivity:GetGComponent())
end

function M:RemoveGameActivity(gameActivity)
    self.tmGameActivity[gameActivity] = nil
    self.gcomponent:RemoveChild(gameActivity:GetGComponent())
end

function M:GetGameActivityByClass(classOrName)
    local class = self:GetGameActivityClass(classOrName)
    for _, gameActivity in pairs(self.tmGameActivity) do
        if gameActivity.__index == class then
            return gameActivity
        end
    end
end


return M