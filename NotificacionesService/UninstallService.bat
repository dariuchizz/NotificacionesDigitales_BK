﻿@echo OFF
echo Stopping service...
net stop "NotificacionesService"
echo Uninstalling service...
sc delete "NotificacionesService"
pause