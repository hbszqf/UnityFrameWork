--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-06-30 19:07:57
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.Fairy.UIPackageProxy#M]
function M.New(...)
    return M(...)
end
function M:ctor(packageName)
    M.super.ctor(self)
    self.packageName = packageName
    self.loaded = false
    self.uiPackage = nil
    --事件
    self.tlTask = {}

    --是否常驻
    self.isResident = false

    --所有使用此Package创建的gobject, 使用弱表
    self.tmGObject = {}
    setmetatable(self.tmGObject, {__mode = "kv"})


    CsProxy.LoadFairyUIPackage(packageName,function ( uiPackage )
        if LuaObject.IsNull(self) then
            CsProxy.UnloadFairyUIPackage(packageName)
        end
        self.uiPackage = uiPackage
        self.loaded = true
        for _, task in ipairs(self.tlTask) do
            self:CreateObject2(task[1],task[2])
        end
    end)
    
end

function M:dispose()
    if self.uiPackage then
        CsProxy.UnloadFairyUIPackage(packageName)
    end

    M.super.dispose(self)
end

function M:CreateObject(objectName)
    if not self.loaded then return end

    local gobject = self.uiPackage:CreateObject(objectName)
    
    assert(gobject, string.format("%s:不存在", objectName))
    self:AddGObject(gobject, objectName)

    return gobject
end

function M:CreateObject2(objectName, fCallback)
    if self.loaded then
        fCallback(self:CreateObject(objectName))
    else
        self.tlTask[#self.tlTask + 1] = {objectName, fCallback}
    end  
end

--添加一个关联GObject,
function M:AddGObject(gobject, objectName)
    --记录引用
    self.tmGObject[gobject] = objectName
    --记录时间
    self.lastUseTime = os.time()
end

--获取所有TmGObject
function M:GetTmGObject()
    ---[[ 判断内部是否全部被dispose
    -- fuck 缺少是否被Dispose的接口
    for gobject, _ in pairs(self.tmGObject) do
        if IsNull(gobject) then
            self.tmGObject[gobject] = nil
        end
    end
    return self.tmGObject
end

--是否在使用
function M:IsInUse()
    if self.uiPackage == nil then
        return true
    end
    if self.loadingNum > 0 then
        return true
    end
    if self.isResident then
        return true
    end
    local tmGObject = self:GetTmGObject()
    if next(tmGObject) == nil then
        return false
    end
    return true
end




return M