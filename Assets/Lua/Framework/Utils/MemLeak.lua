--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-06-29 13:18:50
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

M.log = Log.New(true):SetName(...)

local instance = nil

--@return [Framework.Utils.MemLeak#M]
function M.GetInstance()
    if not instance then
        instance = M()
    end
    return instance
end


function M:ctor()
    M.super.ctor(self)
    --一张若表
    self.tm = {}
    self.index = 0
    setmetatable(self.tm, {__mode = "kv"})
end

function M:Add(object)
    self.index = self.index+1
    self.tm[self.index] = object
end

function M:GCAndDump(isMono, isLua)
    coroutine.start(
        function()
            if isMono then
                coroutine.wait(0.1)
                System.GC.Collect()
                System.GC.Collect()
                coroutine.wait(0.1)
                System.GC.Collect()
                System.GC.Collect()
                coroutine.wait(0.1)
            end

            if isLua then
                collectgarbage("collect")
                collectgarbage("collect")
                coroutine.wait(0.1)
                collectgarbage("collect")
                collectgarbage("collect")
                coroutine.wait(0.1)
            end

            M.log:Dump(self.tm)
        end
    )
end






return M