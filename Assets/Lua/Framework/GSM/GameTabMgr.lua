--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-24 22:17:22
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)


--@region 构造析构

--@return [Framework.GSM.GameTabMgr#M]
function M.New(...)
    return M(...)
end

--@context: [Framework.GSM.GameActivityWithTabMgr#M]
--@tlTabParam: [Framework.GSM.GameTabParam#M]
function M:ctor(context, tlTabParam)
    M.super.ctor(self)
    assert(context.com, "必须先初始化完this.com后 才能添加tabMgr")

    --@RefType [Framework.GSM.GameActivityWithTabMgr#M]
    self.context = context
    --@RefType [luaIde#FairyGUI.GComponent]
    self.com = self.context.com
    --@RefType [Framework.GSM.GameTab#M]
    self.tmGameTab = {}
    --@RefType [Framework.GSM.GameTab#M]
    self.runningGameTab = nil
    --默认名字
    self.defaultTabName = nil

    for _, tabParam in ipairs(tlTabParam)  do
        local tabName = tabParam.tabName
        if not self.defaultTabName then
            self.defaultTabName = tabName
        end

        if self.tmGameTab[tabName] then
            assert(false, "重复添加GameTab")
        end
        --@RefType [Framework.GSM.GameTab#M]
        local gameTab = tabParam.class(self, tabParam)
        self.com:AddChild(gameTab:GetGComponent())
        self.tmGameTab[tabName] = gameTab
    end
end

function M:dispose()
    for _, gameTab in pairs(self.tmGameTab) do
        LuaObject.Destroy(gameTab)
    end
    self.tmGameTab = nil
    M.super.dispose(self)
end

--@endregion
--@return [Framework.GSM.GameTab#M]
function M:GetGameTab(tabName)
    return self.tmGameTab[tabName]
end

--@return [Framework.GSM.GameTab#M]
function M:GetRunningGameTab()
    return self.runningGameTab
end

--@return [Framework.Utils.Promise#M]
function M:SwitchTo(tabName, enterParam) 
    return self.context:RunTask(self.SwitchTo_cor, self, tabName, enterParam)
end

--@return [Framework.GSM.GameTab#M]
function M:SwitchTo_cor(tabName, enterParam)
    local enterParam = enterParam
    local gameTab = self:GetGameTab(tabName)
    if not gameTab then
        assert(false, "切换不存在的GameTab")
    end
    if self.runningGameTab == gameTab then
        return gameTab
    end

    --处理
    local oldGameTab = self.runningGameTab
    if  oldGameTab~=nil then
        if oldGameTab.isEnter then
            oldGameTab:ExitWrap()
        end
    end

    self.runningGameTab = gameTab

    --没init
    if not gameTab.isInit then
        gameTab:InitWrap(gameTab.tabParam.initParam)
    end

    --没enter
    if not gameTab.isEnter then
        gameTab:EnterWrap(enterParam)
    end

    --回调
    if gameTab.tabParam.fCallback then
        gameTab.tabParam.fCallback(gameTab)
    end

    --返回
    return gameTab
end

--@return [Framework.Utils.Promise#M]
function M:SwitchToDefault(...)
    return self:SwitchTo(self.defaultTabName,...)
end

function M:Release()
    for _, gameTab in ipairs(self.tmGameTab) do
        if gameTab.isEnter then
            gameTab:ExitWrap()
        end
        if gameTab.isInit then
            gameTab:ReleaseWrap()
        end
    end
end
return M