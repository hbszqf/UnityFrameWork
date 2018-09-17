--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-12 11:27:16
--

local TableIndex = require("Framework.Utils.TableIndex")

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)



--@return [Framework.Utils.RefProxy#M]
function M.New(...)
    return M(...)
end
function M:ctor( tlRefX, __index)
    M.super.ctor(self)
    
    self.tlRefX = tlRefX
    self.tmRefX = {}

    local mainKey = self:GetMainKey()
    local meta = {
        __index = __index
    }
    for _, refX in ipairs(tlRefX) do
        self:ProcessRef(refX)
        setmetatable(refX, meta)
        if refX[mainKey] then
            self.tmRefX[refX[mainKey]] = refX
        end
    end
end

--@desc 用主键索引Ref
function M:GetRef(mainKey)
    local ref = self:TryGetRef(mainKey)
    if not ref then
        --@TODO 2018-08-13 21:47:59
        assert(false)
    end
    return ref
end

--@desc 尝试用主键索引ref, 索引失败 返回nil
function M:TryGetRef(mainKey)
    return self.tmRefX[mainKey]
end

--@desc 获取列表
function M:GetTlRef()
    return self.tlRefX
end

--@desc 获取Map
function M:GetTmRef()
    return self.tmRefX
end

--@desc 构建一个(RefIndex)索引, 加快一些复杂查询
--@return [Framework.Ref.RefIndex#M]
function M:CreateTableIndex(tlKey)
    return TableIndex.New(self.tlRefX, tlKey)
end
--@endregion


--@region 子类重写函数
function M:ProcessRef(ref)

end
function M:GetMainKey()
    return "refId"
end
--@endregion


return M