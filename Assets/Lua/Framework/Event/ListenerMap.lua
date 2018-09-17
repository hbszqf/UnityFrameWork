--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-06-28 13:57:50
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

M.log = Log.New(true):SetName(...)

--@return [Framework.Event.ListenerMap#M]
function M.New(luaObject,autoremove)
    return M(luaObject,autoremove)
end
function M:ctor(luaObject,autoremove )
    M.super.ctor(self)

    self.luaObject = luaObject
    self.autoRemove = autoremove
    self.tmListener = {}
end

function M:dispose()
    self.luaObject = nil
    self.autoRemove = nil
    self.tmListener = nil

    M.super.dispose(self)
end

--设置一个监听
function M:SetListener(eventName, listener)
    self.tmListener[eventName] = listener
end

--获取一个监听
function M:GetListener(eventName)
    return self.tmListener[eventName]
end

--删除一个监听
function M:RemoveListener(eventName)
    self.tmListener[eventName] = nil
end

--广播一个事件, 返回这个事件是否要删除
function M:Broacast(eventName, ...)
    
    local listener = self:GetListener(eventName)
    if not listener then
        return false
    end


    if self.autoRemove then
        if LuaObject.IsNull(self.luaObject) then
            return true
        end
    end
    if self.luaObject then
        listener(self.luaObject, ...)
    else
        listener(...)
    end
end

--是否有监听
function M:GetHasListener()
    return next( self.tmListener ) ~= nil
end

return M