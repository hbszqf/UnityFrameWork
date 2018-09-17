--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-24 22:17:35
--

--@SuperType [Framework.GSM.GameActivityWithTabMgr#M]
local M = class(..., require("Framework.GSM.GameActivityWithTabMgr"))

--@region 构造析构

--@return [Framework.GSM.GameTab#M]
function M.New(...)
    return M(...)
end

--@gameTabMgr: [Framework.GSM.GameTabMgr#M]
--@tabParam: [Framework.GSM.GameTabParam#M]
function M:ctor(gameTabMgr,tabParam)
    M.super.ctor(self,gameTabMgr.context)

    self.isInit = false
    self.isEnter = false
    self.tabParam = tabParam
    self:GetGComponent().visible = false
end
--@endregion

--@region 可重写

function M:InitWrap(...)
    M.super.InitWrap(self, ...)
    self.isInit = true
end
function M:ReleaseWrap(...)
    M.super.ReleaseWrap(self, ...)
    self.isInit = false
end

function M:EnterWrap(...)
    self:Enter(...)
    self.isEnter = true
    self:GetGComponent().visible = true
end

function M:Enter()
end

function M:ExitWrap()
    self:Exit()
    self.isEnter = false
    self:GetGComponent().visible = false
end

function M:Exit()
end
--@endregion





return M