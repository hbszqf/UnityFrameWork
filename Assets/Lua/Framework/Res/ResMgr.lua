--
--版权所有:{company}
-- Author:{author}
-- Date: 2018-08-14 14:58:50
--

--@SuperType [Framework.LuaObject#M]
local M = class(..., LuaObject)



--@region 构造析构
local M_instance = nil
--@return [Framework.Res.ResMgr#M]
function M.GetInstance()
    if not M_instance then
        M_instance = M()
    end
    return M_instance
end

function M:ctor()
    M.super.ctor(self)
    self.runningScene = nil
end


--@endregion

--@region 公有方法

--@return [Framework.Utils.Promise#M]
function M:LoadAssetAsync(assetPath)
    local function fTask(resolve, reject)
        CsProxy.LoadAsset(assetPath,function(uo)
            if uo then
                --解决
                local ret = resolve(uo)
                --没成功处理
                if not ret then
                    self:UnloadAsset(assetPath,uo)
                end
            else
                --拒绝
                reject(uo)
            end
        end)
    end
    return Promise.New(fTask)
end

function M:UnloadAsset(assetPath, uo)
    CsProxy.UnloadAsset(assetPath, uo)
end

--@return [Framework.Utils.Promise#M]
function M:LoadSceneAsync(assetPath)

    
    local function fTask(resolve, reject)
        if self:GetRunningScene() == assetPath then
            resolve(true)
            return 
        end
        self.runningScene = nil
        CsProxy.LoadScene(assetPath,function(suc)
            if suc then
                self.runningScene = assetPath
                --解决
                resolve(true)
            else
                --拒绝
                reject(suc)
            end
        end)
    end
    return Promise.New(fTask) 
end

--@return [Framework.Utils.Promise#M]
function M:UnloadSceneAsync()
    self.runningScene = nil
    local function fTask(resolve, reject)
        CsProxy.UnloadScene(function(suc)
            if suc then
                --解决
                resolve(gobject)
            else
                --拒绝
                reject(suc)
            end
        end)
    end
    return Promise.New(fTask) 
end

function M:GetRunningScene()
    return self.runningScene
end
--@endregion

--@region 私有方法

--@endregion

--@region 可覆盖重写方法
--@endregion

return M