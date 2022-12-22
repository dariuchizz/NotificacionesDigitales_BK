@echo OFF
echo Stopping old service version...
net stop "NotificacionesService"
echo Uninstalling old service version...
sc delete "NotificacionesService"

echo Installing service...
sc create "NotificacionesService" binpath= "%CD%\NotificacionesService.exe" start= auto
sc start "NotificacionesService"
echo Starting server complete
pause