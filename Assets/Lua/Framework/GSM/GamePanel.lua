--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-24 20:53:26
--

--@SuperType [Framework.GSM.GameActivityWithTabMgr#M]
local M = class(..., require("Framework.GSM.GameActivityWithTabMgr"))

--@region 构造析构

--@return [Framework.GSM.GamePanel#M]
function M.New(...)
    return M(...)
end

function M:ctor(...)
    M.super.ctor(self, ...)
    self.gcomponent.size = GRoot.size
    self.gcomponent.opaque = true
    self.hasBackground = true
    self.isFull= true    --是否全屏
    self.isUnique= true --是否唯一
    self.hickness=0 --厚度
    --@RefType [Framework.Event.EventProxy#M]
    self.eventProxy = nil
end

--@endregion


--@region 可重写
function  M:BecomeTop()
end

function M:ResignTop()
end
--@endregion

function M:SetIsOpaque(opaque)
    self.gcomponent.opaque = opaque
end

function M:GetIsOpaque()
    return self.gcomponent.opaque
end

function M:SetHasBackground( hasBackground )
    self.hasBackground = true
    self.gameActivityMgr:UpdatePanelsOrder()
end
function M:GetHasBackground(hasBackground)
    return self.hasBackground
end

function M:SetIsUnique(isUnique)
    self.isUnique = isUnique
end
function M:GetIsUnique()
    return self.isUnique
end

function M:GetIsFull()
    return self.isFull
end
function M:SetIsFull(isFull)
    self.isFull = isFull
    self.gameActivityMgr:UpdatePanelsOrder()
end

function M:SetHickness(hickness)
    self.hickness = hickness
    self.gameActivityMgr:UpdatePanelsOrder()
end
function M:GetHickness()
    return self.hickness
end

return M