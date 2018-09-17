--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-18 23:24:08
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@region 构造析构

--@return [Framework.Prefab.PrefabMgrConfig#M]
function M.New(...)
    return M(...)
end
function M:ctor()
    M.super.ctor(self)
end


--@endregion


--@desc 回池
function M:GetIsDeactiveWhenReturnToPool(assetPath)
    --assert(false, "子类需要重写此函数")
    return true
end

--@desc 至少保留多少个 不垃圾回收
function M:GetMinCount(assetPath)
    --assert(false, "子类需要重写此函数")
    return 1
end


return M