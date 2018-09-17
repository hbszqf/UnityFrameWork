--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)

--@return [Framework.Utils.Log#M]
function M.New(isEnabled)
    return M(isEnabled)
end

function M:ctor()
    self.isEnabled = true
    self.name = ""
end

function M:dispose()
    M.super.dispose(self)
end

--@return [Framework.Utils.Log#M]
function M:SetEnabled(isEnabled)
    self.isEnabled = isEnabled
    return self
end

--@return [Framework.Utils.Log#M]
function M:SetName(name)
    self.name = name
    return self
end



function M:Print(...)
    
    if self.isEnabled then
        local tl = {self.name, ":", ...}
        tl[#tl+1] = "\n"
        tl[#tl+1] = debug.traceback()
        print( unpack(tl) )
    end
end


-- 输出值的内容--
-- @param mixed value 要输出的值
-- @param [string desciption] 输出内容前的文字描述
-- @parma [integer nesting] 输出时的嵌套层级，默认为 3
function M:Dump(value, desciption, nesting)
    if type(nesting) ~= "number" then
        nesting = 3
    end

    local tlMsg = {}
    local print = function(...)
        table.insertto(tlMsg, {...})
    end

    local lookupTable = {}
    local result = {}

    local function _v(v)
        if type(v) == "string" then
            v = '"' .. v .. '"'
        end
        return tostring(v)
    end

    local traceback = string.split(debug.traceback("", 2), "\n")
    print("dump from: " .. string.trim(traceback[3]))
    local spc
    local function _dump(value, desciption, indent, nest, keylen)
        desciption = desciption or "<var>"
        spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(_v(desciption)))
        end
        if type(value) ~= "table" then
            result[#result + 1] = string.format("%s%s%s = %s", indent, _v(desciption), spc, _v(value))
        elseif lookupTable[value] then
            result[#result + 1] = string.format("%s%s%s = *REF*", indent, desciption, spc)
        else
            lookupTable[value] = true
            if nest > nesting then
                result[#result + 1] = string.format("%s%s = *MAX NESTING*", indent, desciption)
            else
                result[#result + 1] = string.format("%s%s = {", indent, _v(desciption))
                local indent2 = indent .. "    "
                local keys = {}
                local keylen = 0
                local values = {}
                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = _v(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then
                        keylen = vkl
                    end
                    values[k] = v
                end
                table.sort(
                    keys,
                    function(a, b)
                        if type(a) == "number" and type(b) == "number" then
                            return a < b
                        else
                            return tostring(a) < tostring(b)
                        end
                    end
                )
                for i, k in ipairs(keys) do
                    _dump(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result + 1] = string.format("%s}", indent)
            end
        end
    end
    _dump(value, desciption, "- ", 1)

    for i, line in ipairs(result) do
        print(line)
    end

    self:Print(table.concat(tlMsg, "\n"))
end


return M
