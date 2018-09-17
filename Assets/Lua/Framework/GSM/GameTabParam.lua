--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-24 22:40:55
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@region 构造析构

--@return [Framework.GSM.GameTabParam#M]
function M.New(class, tabName, initParam, fCallback)
    return M(class, tabName, initParam, fCallback)
end
function M:ctor(class, tabName, initParam, fCallback)
    M.super.ctor(self)
    self.class = class
    self.tabName = tabName
    self.initParam = initParam
    self.fCallback = fCallback
end

--@endregion


return M