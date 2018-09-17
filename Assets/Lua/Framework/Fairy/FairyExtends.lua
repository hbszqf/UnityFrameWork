local getpeer = tolua.getpeer
local setpeer = tolua.setpeer

--@region 扩展GObject

do
    local mt = getmetatable(FairyGUI.GComponent)
    rawset(mt, "GetPeer", function(self)
        local peer = getpeer(self)
        if peer == nil then
            peer = {}
            peer.owner = self
            setpeer(self, peer)
        end
        return peer
    end)

    rawset(mt, "GetUserData", function(self, key)
        local peer = self:GetPeer()
        peer.userData = peer.userData or {}
        return peer.userData[key]
    end)

    rawset(mt, "SetUserData", function(self, key, value)
        local peer = self:GetPeer()
        peer.userData = peer.userData or {}
        peer.userData[key] = value
    end
)

end
--@endregion

--@region 扩展 GComponent
do
    local mt = getmetatable(FairyGUI.GComponent)

    -- 缓存子节点 加速子节点查找
    rawset(mt, "tm", function(self, name1, name2, name3)

        local child = self:_tm(name1)
        if not name2 then
            return child
        end

        local child = self:_tm(name2)
        if not name3 then
            return child
        end

        return child:_tm(name3)

    end)

    rawset(mt, "_tm", function(self, name)
        local peer = self:GetPeer()
        peer.tmChild = peer.tmChild or {}
        local child = peer.tmChild
        if not child  then
            child = self:GetChild(name)
            peer.tmChild[name] = child
        end
        return child

    end)

end

--@endregion
