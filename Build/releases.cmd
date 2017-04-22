@ECHO OFF

ECHO Nuget
nuget pack Our.Umbraco.PublishQueue.nuspec

ECHO Copy to local git
copy *.nupkg c:\source\localgit /y

Echo Package File

COPY ..\Our.Umbraco.PublishQueue\bin\Release\Our.Umbraco.PublishQueue.dll ..\releases\Our.Umbraco.PublishQueue\ /y
COPY ..\Our.Umbraco.PublishQueue\App_Plugins\PublishQueue\*.* ..\releases\Our.Umbraco.PublishQueue\ /y

Echo Creating package.
"c:\Program Files\7-Zip\7z.exe" a ..\Releases\Our.Umbraco.PublishQueue.zip ..\Releases\Our.Umbraco.PublishQueue\