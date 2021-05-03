@echo off
echo Starting..

:top
node bot.js
echo Bot crashed, restarting.
goto top