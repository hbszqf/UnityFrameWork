local M = {}

M.GameTab = require("Framework.GSM.GameTab")
M.GameActivityWithTabMgr = require("Framework.GSM.GameActivityWithTabMgr")
M.GameTabParam = require("Framework.GSM.GameTabParam")

M.GameSceneMgr = require("Framework.GSM.GameSceneMgr")
M.GameScene = require("Framework.GSM.GameScene")
M.GamePanelMgr = require("Framework.GSM.GamePanelMgr")
M.GamePanel = require("Framework.GSM.GamePanel")


if false then
    local GSM = M
    GameSceneMgr = GSM.GameSceneMgr.New(0)
    GamePanelMgr = GSM.GamePanelMgr.New(1)
    GameSceneMgr:LoadGameScene(GSM.GameScene):AddListener(function(suc, gameScene)
        NYLog:Dump( "GameScene" )
    end)
    require("misc.strict")

    --@RefType [Framework.GSM.GamePanel#M]
    local GamePanel = class("GamePanel",GSM.GamePanel)
    function GamePanel:Init()
        self.com = GComponent.New()
        self.com.gameObjectName = "com"
        
        self.gcomponent:AddChild(self.com)

        self:AddTabMgr({
            GSM.GameTabParam.New(GSM.GameTab,"1",nil)
        })
    end

    GamePanelMgr:AddGamePanel(GamePanel):AddListener(function(ret, gamePanel)
        if not ret then
            return
        end
        gamePanel:SwitchTo("1")
    end)
end
return M