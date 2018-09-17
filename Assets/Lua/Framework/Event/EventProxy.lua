--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-06-28 13:52:43
--

local ListenerMap = require("Framework.Event.ListenerMap")
--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

M.log = Log.New(true)

--@return [Framework.Event.EventProxy]
function M.New()
    return M()
end

function M:ctor()
    M.super.ctor(self)

    --监听者的map
    self.tmListenerMap = {}
    --是否正在广播
    self.isBroacasting = false
end

function M:dispose()
    M.super.dispose(self)
end

--添加一个监听
function M:AddListener(eventName, listener, luaObject)
	assert(not self.isBroacasting, "派发过程中, 暂时不支持增加监听者")

	local key = luaObject == nil and listener or luaObject
	local autoremove = luaObject ~= nil

	local listenerMap = self.tmListenerMap[key]
	if not listenerMap then
		listenerMap = ListenerMap.New(key, autoremove)
		self.tmListenerMap[key] = listenerMap
	end

	listenerMap:SetListener(eventName, listener)
end

--删除一个监听者
function M:RemoveListener(eventName, listener, luaObject)
	assert(not self.isBroacasting, "派发过程中, 暂时不支持增加监听者")

	--没有luaObject时候, listener 作为key
	local key = luaObject == nil and listener or luaObject

	--不存在
	local listenerMap = self.tmListenerMap[key]
	if not listenerMap then
		return
	end
	if listenerMap[eventName] ~= listener then
		return
	end

	--删除监听
	listenerMap:RemoveListener(eventName)

	--已经没有监听了
	if not listenerMap:GetHasListener() then
		self.tmListenerMap[key] = nil
	end
	
end

function M:RemoveAllListener()
	for key, listenerMap in pairs(self.tmListenerMap) do
		self.tmListenerMap[key] = nil
	end
end

--广播事件
function M:Broacast(eventName, ...)
	assert(not self.isBroacasting, "派发过程中, 暂时不支持增加监听者")

	--标记为派发中
	self.isBroacasting = true

	--将要删除的事件
	local tlRemoveKey = nil

	--派发事件
	for key, listenerMap in pairs(self.tmListenerMap) do
		local willremove = listenerMap:Broacast(eventName,...)
		if willremove then
			if not tlRemoveKey then
				tlRemoveKey = {}
			end
			tlRemoveKey[#tlRemoveKey + 1] = key
		end
	end

	--结束派发
	self.isBroacasting = false

	--删除
	if tlRemoveKey then
		for _, key in ipairs(tlRemoveKey) do
			self.tmListenerMap[key] = nil
		end
	end

end

if false then
	local eventProxy = M.New()
	
	local lo = LuaObject.New()
	
	
	eventProxy:AddListener("eventName",function(...)
		M.log:Print(...)
	end,lo)

	eventProxy:Broacast("eventName","123")

	LuaObject.Destroy(lo)

	eventProxy:Broacast("eventName", "123")


end



return M