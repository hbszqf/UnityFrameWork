-- 用指定字符或字符串分割输入字符串，返回包含分割结果的数组--
function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter == "") then
        return false
    end
    local pos, arr = 0, {}
    for st, sp in function()
        return string.find(input, delimiter, pos, true)
    end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end

-- 去除输入字符串头部的空白字符，返回结果--
function string.ltrim(input)
    return string.gsub(input, "^[ \t\n\r]+", "")
end

-- 去除输入字符串尾部的空白字符，返回结果--
function string.rtrim(input)
    return string.gsub(input, "[ \t\n\r]+$", "")
end

-- 去掉字符串首尾的空白字符，返回结果--
function string.trim(input)
    input = string.gsub(input, "^[ \t\n\r]+", "")
    return string.gsub(input, "[ \t\n\r]+$", "")
end

-- 将字符串的第一个字符转为大写，返回结果--
function string.ucfirst(input)
    return string.upper(string.sub(input, 1, 1)) .. string.sub(input, 2)
end

-- 将字符串的第一个字符转为小写，返回结果--
function string.lcfirst(input)
    return string.lower(string.sub(input, 1, 1)) .. string.sub(input, 2)
end

local function urlencodechar(char)
    return "%" .. string.format("%02X", string.byte(char))
end

-- 将字符串转换为符合 URL 传递要求的格式，并返回转换结果--
function string.urlencode(input)
    input = string.gsub(tostring(input), "\n", "\r\n")
    input = string.gsub(input, "([^%w%.%- ])", urlencodechar)
    return string.gsub(input, " ", "+")
end

-- 将 URL 中的特殊字符还原，并返回结果--
function string.urldecode(input)
    input = string.gsub(input, "+", " ")
    input =
        string.gsub(
        input,
        "%%(%x%x)",
        function(h)
            return string.char(checknumber(h, 16))
        end
    )
    input = string.gsub(input, "\r\n", "\n")
    return input
end

-- 计算 UTF8 字符串的长度，每一个中文算一个字符--
function string.utf8len(input)
    local len = string.len(input)
    local left = len
    local cnt = 0
    local arr = {0, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc}
    while left ~= 0 do
        local tmp = string.byte(input, -left)
        local i = #arr
        while arr[i] do
            if tmp >= arr[i] then
                left = left - i
                break
            end
            i = i - 1
        end
        cnt = cnt + 1
    end
    return cnt
end

function string.utf8lenByByte(charByte)
    local arr = {0, 0xc0, 0xe0, 0xf0, 0xf8, 0xfc}
    local i = #arr
    local cnt = 0
    while arr[i] do
        if charByte >= arr[i] then
            break
        end
        i = i - 1
    end
    return i
end

-- 分割字符串00，可分割包含中英文的字符串，
function string.utf8sub(str, startIndex, endIndex)
    local index_start = 1
    while startIndex > 1 do
        local char = string.byte(str, index_start)
        index_start = index_start + string.utf8lenByByte(char)
        startIndex = startIndex - 1
    end

    local currentIndex = index_start

    while endIndex > 0 and currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + string.utf8lenByByte(char)
        endIndex = endIndex - 1
    end
    return str:sub(index_start, currentIndex - 1)
end

-- 将数值格式化为包含千分位分隔符的字符串--
function string.formatnumberthousands(num)
    local formatted = tostring(checknumber(num))
    local k
    while true do
        formatted, k = string.gsub(formatted, "^(-?%d+)(%d%d%d)", "%1,%2")
        if k == 0 then
            break
        end
    end
    return formatted
end
