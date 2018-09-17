--------------------------------------------------------------------------------------------
-- @description 该文件存放一些全局的Table操作函数
-- @author zhongliang
-- @coryright 蜂鸟工作室
-- @release 2016-08-01
--------------------------------------------------------------------------------------------

-- 计算表格包含的字段数量--
function table.count(t)
    local count = 0
    for k, v in pairs(t) do
        count = count + 1
    end
    return count
end

-- 返回指定表格中的所有键--
function table.keys(hashtable)
    local keys = {}
    if isTable(hashtable) then
        for k, v in pairs(hashtable) do
            keys[#keys + 1] = k
        end
    end
    return keys
end

-- 返回指定表格中的所有值--
function table.values(hashtable)
    local values = {}
    if isTable(hashtable) then
        for k, v in pairs(hashtable) do
            values[#values + 1] = v
        end
    end
    return values
end

-- 将来源表格中所有键及其值复制到目标表格对象中，如果存在同名键，则覆盖其值--
function table.merge(dest, src)
    for k, v in pairs(src) do
        dest[k] = v
    end
end

-- 在目标表格的指定位置插入来源表格，如果没有指定位置则连接两个表格--
function table.insertto(dest, src, begin)
    begin = begin or 0
    if begin <= 0 then
        begin = #dest + 1
    end

    local len = #src
    for i = 0, len - 1 do
        dest[i + begin + #src] = dest[i + begin]
        dest[i + begin] = src[i + 1]
    end
end

-- 从表格中查找指定值，返回其索引，如果没找到返回 false--
function table.indexof(array, value, begin)
    for i = begin or 1, #array do
        if array[i] == value then
            return i
        end
    end
    return false
end

-- 从表格中查找指定值，返回其 key，如果没找到返回 nil--
function table.keyof(hashtable, value)
    if isTable(hashtable) then
        for k, v in pairs(hashtable) do
            if v == value then
                return k
            end
        end
    end
    return nil
end

-- 从表格中删除指定值，返回删除的值的个数--
function table.removebyvalue(array, value, removeall)
    local c, i, max = 0, 1, #array
    while i <= max do
        if array[i] == value then
            table.remove(array, i)
            c = c + 1
            i = i - 1
            max = max - 1
            if not removeall then
                break
            end
        end
        i = i + 1
    end
    return c
end

-- 对表格中每一个值执行一次指定的函数，并用函数返回值更新表格内容--
function table.map(t, fn)
    t = checktable(t)
    for k, v in pairs(t) do
        t[k] = fn(v, k)
    end
end

-- 对表格中每一个值执行一次指定的函数，但不改变表格内容--
function table.walk(t, fn)
    t = checktable(t)
    for k, v in pairs(t) do
        fn(v, k)
    end
end

-- 对表格中每一个值执行一次指定的函数，如果该函数返回false，则对应的值会从表格中删除--
function table.filter(t, fn)
    for k, v in pairs(t) do
        if not fn(v, k) then
            t[k] = nil
        end
    end
end

-- 遍历表格，确保其中的值唯一--
function table.unique(t)
    local check = {}
    local n = {}
    for k, v in pairs(t) do
        if not check[v] then
            n[k] = v
            check[v] = true
        end
    end
    return n
end

-- 判断表格中是否包含指定的key--
function table.containKey(t, key)
    for k, v in pairs(t) do
        if key == k then
            return true
        end
    end
    return false
end

-- 判断表格中是否包含指定的value--
function table.containValue(t, value)
    for k, v in pairs(t) do
        if v == value then
            return true
        end
    end
    return false
end

-- 清空一个表格--
function table.clear(t)
    if type(t) == "table" then
        table.map(
            t,
            function()
                return nil
            end
        )
    end
end
