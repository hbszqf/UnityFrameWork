--
-- 管理注册到c#的Delegate的 LuaFunction, 防止内存泄漏
--版权所有:{company}
-- Author:{author}
-- Date: 2018-07-01 24:44:14
--

--@TODO 2018-08-14 14:54:45

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

local instance = nil

--@return [重要:输入对应类型]
function M.GetInstance()
    if not instance then
        instance = M()
    end
    return instance
end

function M:ctor()
    M.super.ctor(self)
end

function M:dispose()
    M.super.dispose(self)
end

return M