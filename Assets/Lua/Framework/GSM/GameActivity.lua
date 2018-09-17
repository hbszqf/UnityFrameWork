--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-23 22:55:46
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@region 构造析构

--@return [Framework.GSM.GameActivity#M]
function M.New(...)
    return M(...)
end
function M:ctor(gameActivityMgr)
    M.super.ctor(self)

    --@RefType [luaIde#FairyGUI.GComponent]
    self.gcomponent = FairyGUI.GComponent.New()
    self.gcomponent.fairyBatching = true
    self.gcomponent.container.gameObject.name = self.shortname

    --@RefType [Framework.GSM.GameActivityMgr#M]
    self.gameActivityMgr = gameActivityMgr
    self.gameActivityMgr.gcomponent:AddChild(self.gcomponent)

    self.com = nil
end

function M:dispose()
    self.gcomponent:Dispose()
    self.gcomponent = nil
    M.super.dispose(self)
end

--@endregion



--@region 可覆盖重写方法
function M:Init()
end

function M:Release()
end
--@endregion

function M:ReleaseWrap(...)
    self:Release(...)
end

function M:InitWrap(...)
    self:Init(...)
end

--@return [luaIde#FairyGUI.GComponent]
function M:GetGComponent()
    return self.gcomponent
end

--@return [Framework.Utils.Promise#M]
function M:RunTask(task, ...)
    return self.gameActivityMgr.taskQueue:Queue(function(...)
        if self.isDisposed then
            return
        end
        return task(...)
    end,...)
end




return M