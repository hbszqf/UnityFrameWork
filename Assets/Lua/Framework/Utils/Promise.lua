--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-07-14 11:00:57
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

local _log = Log.New(true):SetName(...)

--@return [Framework.Utils.Promise#M]
function M.New(...)
    return M(...)
end

--fTask: 执行函数
--fBreak: Promise没完成的时候, 被销毁时候的回调
function M:ctor(fTask, fBreak)
    M.super.ctor(self)
    
    --是否完成
    self.isDone = false
    --是否失败
    self.isFail = false
    
    --异步过程销毁回调
    self.fBreak = fBreak

    --是否中断
    self.isBreak = false

    --结果
    self.result = nil

    --监听数组
    self.tlListener = nil

    --执行任务
    fTask(
        function(...)
            return self:OnResolve(...)
        end,
        function()
            self:OnReject()
        end
    )
end

--@region 对外接口

function M:Break()
    --任务未完成的时候 触发break逻辑
    if not self:GetIsDone() then
        --
        self.isFail = true
        self.isBreak = true

        if self.fBreak then
            self.fBreak()
        end

        --处理所有回调
        self:HandleListener()

        --成功中断
        return true
    end
    --已经完成了 没法中断
    return false

end

--返回是否完成
function M:GetIsDone()
    return self.isDone
end

--返回是否失败
function M:GetIsFail()
    return self.isFail
end

--返回是否成功
function M:GetIsSuc()
    return not self.isFail
end

--返回结果
function M:GetResult()
    return self.result
end

--添加监听
function M:AddListener(f)

    if type(f) ~= "function" then 
        return
    end

    if self.isBreak or self.isDisposed or self:GetIsDone() then
        f(not self:GetIsFail(), unpack(self:GetResult() or {}))
        return
    end

    if self.tlListener == nil then
        self.tlListener = f
        return
    else
        if type(self.tlListener) == "function" then
            self.tlListener = {self.tlListener}
        end

        if type(self.tlListener) == "table" then
            self.tlListener[#self.tlListener + 1] = f
            return
        end
    end

end


--@endregion

--@region 私有接口
--私有方法
function M:OnResolve(...)
    --已经销毁
    if self.isBreak or self.isDisposed then
        return false
    end

    --已经完成
    if self.isDone then
        return false
    end

    --修改状态
    self.isDone = true
    self.isFail = false

    --保存结果
    self.result = {...}
    
    --处理所有监听 false
    self:HandleListener()

    return true
end

--处理失败逻辑
function M:OnReject()
    --已经销毁
    if self.isDisposed then
        return
    end

    --已经完成
    if self.isDone then
        return
    end

    --修改状态
    self.isDone = true
    self.isFail = true

    --保存结果
    self.result = nil

    --处理所有监听
    self:HandleListener()
end

--处理所有监听
function M:HandleListener()
    
    local tlListener = self.tlListener

    local result = self.result or {}

    --回调所有的
    if tlListener == nil then
        return
    elseif type(tlListener) == "function" then
        tlListener(self:GetIsSuc(), unpack(result))
    elseif type(self.tlListener) == "table" then
        local isSuc = self:GetIsSuc()
        for _, listener in ipairs(tlListener) do
            listener(isSuc, unpack(result))
        end
    end
end

--@endregion










return M