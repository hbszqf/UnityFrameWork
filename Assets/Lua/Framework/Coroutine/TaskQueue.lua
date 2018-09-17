--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-06-30 19:08:40
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.Coroutine.TaskQueue#M]
function M.New()
    return M()
end

function M:ctor()
    M.super.ctor(self)

    self.tlTask = {}

    coroutine.start(function()
        self.corUtil = CorUtil.New()
        while true do
            if self.isDisposed then
                return
            end
            self:Loop()
            self.corUtil:Yield()
        end
    end)

end

function M:dispose()
    M.super.dispose(self)
end

function M:Loop()
    while true do
        local realTask = self:Dequeue()

        if realTask then
            self.runningTask = realTask.task
            realTask.resolve(realTask.task(unpack(realTask.params)))
            self.runningTask = realTask.task
        else
            break
        end
    end
end

function M:GetIsRunning()
    return #self.tlTask > 0 or self.runningTask
end

--@desc插入一个task (function对象) priority 优先级, 越大越优先执行
--@return [Framework.Utils.Promise#M]
function M:Queue(task, ...)
    
    if not self.corUtil then
        assert(false, "任务队列没准备好")
        return
    end

    if self.isDisposed then
        return
    end
    local params = {...}

    return Promise.New(function(resolve, reject)
        local realTask = {
            task = task,
            params = params,
            resolve = resolve,
            reject = reject,
        }

        local tlTask = self.tlTask
        tlTask[#tlTask + 1] = realTask

        if self.corUtil then
            if self.corUtil.isYield then
                self.corUtil:Resume()
            end
        end
    end)
end

--
function M:Dequeue()
    local tlTask = self.tlTask
    local task = tlTask[#tlTask]
    tlTask[#tlTask] = nil
    return task
end

function M:Clear()
    for k, v in pairs(self.tlTask) do
        self.tlTask[k] = nil
    end
end

function M:GetTlTask()
    return self.tlTask
end


return M