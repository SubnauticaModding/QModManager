### MirrorInternalLogs

This preloader patcher allows to capture and mirror Unity internal debug logs (i.e. the contents of `output_log.txt`).  
Preloader patch provides a public event one can listen to which will receive all Unity logs, including internal debug logs that are only output to `output_log.txt`.

Unlike output log, which can be disabled in the game, this mirror will always capture said debug logs. If Unity already outputs `output_log.txt`, 
this plugin will simply create a copy of it in a more accessible place that `%APPDATA%`.

**How to use:** This is a preloader patcher. Put the compiled DLL into `BepInEx/patchers`.  


### Pre - Configured
By default, logs are output to `unity_log.txt` but we have included a configuration file to name it `{process}-Modding-Log.txt`. 


