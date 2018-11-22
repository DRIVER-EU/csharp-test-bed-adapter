REM Call the avrogen.exe to convert all schemas [-s] located in this folder [file path] and output in folder [folder path]
if exist ..\..\schemas\eu (
rmdir ..\..\schemas\eu /s /q
)

for %%a in (*.avsc) do (
.\avrogen.exe -s %%a ..\..\schemas
)
@pause