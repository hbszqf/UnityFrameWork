--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-06-30 19:12:48
--

local running = coroutine.running
local yield = coroutine.yield
local resume = coroutine.resume
local unpack = unpack


--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.Coroutine.CorUtil#M]
function M.New()
    return M()
end

function M:ctor()
    M.super.ctor(self)
    --当前正在运行的携程
    self.cor = coroutine.running( )
    assert(self.cor, "当前函数只能运行在协程中")
    --是否Resume
    self.isResume = false
    --是否Yield
    self.isYield = false
    --Resume时的返回值
    self.tlRet = nil

end

function M:Yield()
    self.isYield = true
    if not self.isResume then
        self.tlRet = {yield()}
    end

    local tlRet = self.tlRet
    self.isResume = false
    self.isYield = false
    self.tlRet = nil
    return unpack(tlRet or {})

end

function M:Resume(...)
    self.isResume = true
    if self.isYield then
        local flag, msg = resume(self.cor, ...)
        if not flag then
            msg = debug.traceback(self.cor, msg)
            error(msg)
        end
    else
        self.tlRet = {...}
    end
end

function M:WaitPromise(promise)

    promise:AddListener(function(suc, ...)
        local tl = promise:GetResult()
        self:Resume(suc, unpack(tl or {}))
    end)
    return self:Yield()
end

return M