--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-24 22:34:45
--

--@SuperType [Framework.GSM.GameActivity#M]
local M = class(..., require("Framework.GSM.GameActivity"))


--@region 构造析构

--@return [Framework.GSM.GameActivityWithTabMgr#M]
function M.New(...)
    return M(...)
end

function M:ctor(...)
    M.super.ctor(self, ...)
    --@RefType [Framework.GSM.GameTabMgr#M]
    self.tmGameTabMgr = {}
end

function M:dispose()
    for _, tabMgr in pairs(self.tmGameTabMgr) do
        LuaObject.Destroy(tabMgr)
    end
    self.tmGameTabMgr = nil
    M.super.dispose(self)
end

--@endregion

--@tlGameTabMgr: [Framework.GSM.GameTabParam#M]
function M:AddTabMgr(tlGameTabParam, mgrName)
    mgrName = mgrName or "default"
    if self.tmGameTabMgr[mgrName] then
        assert(false, "重复添加tabMgr")
    end

    local gameTabMgr = require("Framework.GSM.GameTabMgr").New(self, tlGameTabParam)
    self.tmGameTabMgr[mgrName] = gameTabMgr
end

--@return [Framework.GSM.GameTabMgr#M]
function M:GetTabMgr(mgrName)
    return self.tmGameTabMgr[mgrName or "default"]
end

--@return [Framework.Utils.Promise#M]
function M:SwitchTo(tabName, enterParam, mgrName)
    local tabMgr = self:GetTabMgr(mgrName)
    return tabMgr:SwitchTo(tabName,enterParam)
end

--@desc 连续切换 self:ChainSwitchTo({ {tabName = "武器"}, {tabName = "金色"} })
function M:ChainSwitchTo(tlSwichParam)
    return self:RunTask(function()
        --@RefType [Framework.GSM.GameActivityWithTabMgr#M]
        local gawtm = self
        for swithParam in ipairs(tlSwichParam) do
            local tabName = swithParam.tabName
            local enterParam = swithParam.enterParam
            local mgrName = swithParam.mgrName
            local fCallback = swithParam.fCallback
            if not gawtm then
                break
            end

            local tabMgr = gawtm:GetTabMgr(mgrName)
            if not tabMgr then
                break
            end

            gawtm = tabMgr:SwitchTo_cor(tabName,enterParam)
            if gawtm and fCallback then
                fCallback(gawtm)
            end
        end
    end)
end

function M:ReleaseWrap()
    for _, tabMgr in pairs(self.tmGameTabMgr) do
        tabMgr:Release()
    end
    M.super.ReleaseWrap()
end



return M