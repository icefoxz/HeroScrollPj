echo off
.\XlsxToJson1.01\xlsxtojson.exe HeroDataTable.xlsx ..\Uploader\jsons
xcopy Uploader\jsons\*.* ..\SanGuoHuiJuanPj\Assets\Resources\Jsons\
pause
