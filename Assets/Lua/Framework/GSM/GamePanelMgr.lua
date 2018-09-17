--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-24 21:11:35
--

--@SuperType [Framework.GSM.GameActivityMgr#M]
local M = class(..., require("Framework.GSM.GameActivityMgr"))

--@region 构造析构

--@return [Framework.GSM.GamePanelMgr#M]
function M.New(...)
    return M(...)
end
function M:ctor(...)
    M.super.ctor(self,...)
    local size = GRoot.size
    self.hasFullPanel = false
    self.graph_bg = GGraph.New()
    self.graph_bg:DrawRect(size.x, size.y, 0, Color.white, Color.New(0, 0, 0, 0.7))
    self.gcomponent:AddChild(self.graph_bg)
    self.graph_bg.visible = false

    --@RefType [Framework.GSM.GamePanel#M]
    self.tlGamePanel = {}
end
--@endregion

--@return [Framework.Utils.Promise#M]
function M:AddGamePanel(class, params)
    local params = params

    return self.taskQueue:Queue(function()
        --@RefType [Framework.GSM.GamePanel#M]
        local panel = self:GetGameActivityByClass(class)
        if panel and panel:GetIsUnique() then
            local index1 = table.indexof(self.tlGamePanel, panel)
            local index2 = #self.tlGamePanel
            if index1 == index2 then
                panel:BecomeTop(params)
                return panel
            end
            local temp = self.tlGamePanel[index1]
            self.tlGamePanel[index1] = self.tlGamePanel[index2]
            self.tlGamePanel[index2] = temp
            this.UpdatePanelsOrder()
            self.tlGamePanel[index1]:ResignTop()
            self.tlGamePanel[index2]:BecomeTop(params)
        else
            local runningPanel = self:GetRunningGamePanel()
            panel = self:CreateGameActivity(class)
            panel:InitWrap(params)
            self:AddGameActivity(panel)
            self.tlGamePanel[#self.tlGamePanel + 1] = panel
            self:UpdatePanelsOrder()
            if runningPanel  then
                runningPanel:ResignTop()
            end
            panel:BecomeTop(params)
        end
        return panel
    end)
end

--@return [Framework.Utils.Promise#M]
function M:RemoveGamePanel(gamePanel)
    return self.taskQueue:Queue(function()
        if IsNull(gamePanel) then
            return
        end
        
        local index = table.indexof(self.tlGamePanel, gamePanel)
        if not index then
            return
        end

        table.removebyvalue(self.tlGamePanel,gamePanel)
        gamePanel:ReleaseWrap()
        self:RemoveGameActivity(gamePanel)
        LuaObject.Destroy(gamePanel)
        self:UpdatePanelsOrder()

        local running = self:GetRunningGamePanel()
        if running then
            running:BecomeTop()
        end
    end)
end

--获取当前最上层的GamePnael
--@return [Framework.GSM.GamePanel#M]
function M:GetRunningGamePanel()
    local tlGamePanel = self.tlGamePanel
    return tlGamePanel[#tlGamePanel]
end

--是否有全屏界面
function M:GetHasFullPanel()
    return self.hasFullPanel
end

--更新Panel的显示层级
function M:UpdatePanelsOrder()
    local tlGamePanel = self.tlGamePanel
    local lastHickness = 0
    local sortingOrder = 0
    --是否存在全屏界面
    self.hasFullPanel = false


    self.graph_bg.visible = false
    for i = 1, #tlGamePanel, 1 do
        local gamePanel = tlGamePanel[i]

        local gamePanelCom = gamePanel:GetGComponent()
        -- 往后移动
        local hickness = -gamePanel:GetHickness()
        gamePanelCom:SetPosition(gamePanelCom.x, gamePanelCom.y, lastHickness)

        if gamePanel:GetHasBackground() then

            self.graph_bg.visible = true
            self.graph_bg.z = lastHickness

            sortingOrder = sortingOrder + 1
            self.graph_bg.sortingOrder = sortingOrder
        end

        sortingOrder = sortingOrder + 1
        gamePanelCom.sortingOrder = sortingOrder

        lastHickness = lastHickness + hickness
        if gamePanel:GetIsFull() then
            self.hasFullPanel = true
        end
    end
end

return M