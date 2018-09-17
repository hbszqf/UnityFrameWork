--
-- classic
--
-- Copyright (c) 2014, rxi
--
-- This module is free software; you can redistribute it and/or modify it under
-- the terms of the MIT license. See LICENSE for details.
--

local M = {}
M.__index = M

--@return [Framework.LuaObject#M]
function M.New()
    return M.__call(M)
end

--构造函数
function M:ctor()
end

--析构函数
function M:dispose()
    self.isDisposed = true
    setmetatable(self, nil)
end

--构造器
function M:__call(...)
    local obj = setmetatable({}, self)
    obj:ctor(...)
    return obj
end

--删除一个对象
function M.Destroy(obj)
    if not obj.isDisposed then
        obj:dispose()
    end
end


function M.IsNull(obj)
    return obj == nil or obj.isDisposed 
end


function M:Extend(classname)
    local cls = {}
    for k, v in pairs(self) do
        if k:find("__") == 1 then
            cls[k] = v
        end
    end
    local tlName = string.split(classname, ".")
    cls.__index = cls
    cls.super = self
    cls.classname = classname
    cls.shortname = tlName[#tlName]

    setmetatable(cls, self)
    return cls
end

function M:Implement(...)
    for _, cls in pairs({...}) do
        for k, v in pairs(cls) do
            if self[k] == nil and type(v) == "function" then
                self[k] = v
            end
        end
    end
end

function M:Is(T)
    local mt = getmetatable(self)
    while mt do
        if mt == T then
            return true
        end
        mt = getmetatable(mt)
    end
    return false
end

return M
