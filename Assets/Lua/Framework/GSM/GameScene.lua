--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-23 23:25:06
--

--@SuperType [Framework.GSM.GameActivity#M]
local M = class(..., require("Framework.GSM.GameActivity"))

--@region 构造析构

--@return [Framework.GSM.GameScene#M]
function M.New(...)
    return M(...)
end

--@endregion

return M