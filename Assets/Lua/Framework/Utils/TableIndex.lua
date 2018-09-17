--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-13 24:32:42
-- 为一个table创建索引 加快其查询速度

local _log = Log.New(true):SetName(...)

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@region 构造析构

--@return [Framework.Utils.TableIndex#M]
function M.New(...)
    return M(...)
end
function M:ctor(tl, tlKey, keyIndex)
    M.super.ctor(self)
    keyIndex = keyIndex or 1

    local keyName = tlKey[keyIndex]

    local nextKeyIndex = keyIndex + 1
    local nextKeyName = tlKey[nextKeyIndex] 


    --初始化cache
    local tmCache = {}

    for _, obj in ipairs(tl) do
        local keyValue = obj[keyName]
        local tlObj = tmCache[keyValue]
        if not tlObj then
            tlObj = {}
            tmCache[keyValue] = tlObj
        end
        tlObj[#tlObj+1] = obj
    end

    if nextKeyName then
        for _, tlObj in pairs(tmCache) do
            tmCache[_] = M.New(tlObj, tlKey, nextKeyIndex)
        end
    end

    self.tmCache = tmCache
    self.tl = tl
end



--@endregion

--@region 公有方法

--@desc 根据索引获取其中一项, 不存在会报异常
function M:GetData(key1, key2, key3)
    local data = self:TryGet(key1,key2,key3)
    if data == nil then
        --@TODO 2018-08-13 19:31:46
        assert(false)
    end
    return data
end

--@desc 根据索引获取其中一项, 不存在会报返回nil
function M:TryGet(key1, key2, key3)
    local tlData = self:TryGetTlData(key1, key2, key3)
    if not tlData then
        return nil
    end
    return tlData[1]
end

--@desc 根据索引获取若干项, 不存在会报异常
function M:GetTlData(key1, key2, key3)
    local tlData = self:TryGetTlData(key1, key2, key3)
    if not tlData then
        --@TODO 2018-08-13 19:31:46
        assert(false)
    end
    return tlData
end

--@desc 根据索引获取若干项, 不存在会返回nil
function M:TryGetTlData(key1, key2, key3)
    local cache = self.tmCache[key1]
    if not cache then
        return nil
    end

    if not key2 then
        if cache.tmCache then 
            --@TODO 2018-08-13 19:31:46
            assert(false)
        end
        return cache
    end

    local tableIndex = cache
    if not tableIndex.tmCache then
        --@TODO 2018-08-13 19:31:46
        assert(false)
    end
    
    return tableIndex:TryGetTlData(key2, key3)
end

--@endregion

--@region 私有方法

--@endregion

--@region 可覆盖重写方法

--@endregion

return M