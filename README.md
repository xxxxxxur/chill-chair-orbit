# Chill Chair Orbit

《Chill with You : Lo-Fi Story》的独立 Windows BepInEx 5 插件。在房间画面中按住鼠标中键拖动，主摄像机便会绕座面上方的固定点旋转。松开中键后保留当前角度。水平旋转范围为 ±35°，垂直为 ±18°。

## 安装

在已安装 BepInEx 5 的游戏目录中解压 Releases 提供的压缩包，然后启动游戏。插件可与外观 Mod 一起使用。

## 构建

运行 `dotnet build ChillChairOrbit.csproj -c Release -p:GamePath="<游戏安装目录>"`。生成的 `ChillChairOrbit.dll` 放在 `BepInEx/plugins/ChillChairOrbit/`。
