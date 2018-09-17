--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-23 23:21:48
--

--@SuperType [Framework.GSM.GameActivityMgr#M]
local M = class(..., require("Framework.GSM.GameActivityMgr"))

--@region 构造析构

--@return [Framework.GSM.GameSceneMgr#M]
function M.New(...)
    return M(...)
end
function M:ctor(...)
    M.super.ctor(self, ...)

    --@RefType [Framework.GSM.GameScene#M]
    self.gameScene = nil
end

--@endregion

--@return [Framework.Utils.Promise#M]
function M:LoadGameScene(classOrName, params)
    return self.taskQueue:Queue(self.LoadGameScene_cor,self, classOrName, params)
end

function M:LoadGameScene_cor(classOrName, params)
    local gameScene_old = self.gameScene
    if gameScene_old then
        gameScene_old:ReleaseWrap()
        self:RemoveGameActivity(gameScene_old)
    end
    local gameScene = self:CreateGameActivity(classOrName)
    self:AddGameActivity(gameScene)
    self.gameScene = gameScene
    gameScene:InitWrap(params)
    return gameScene
end


return M