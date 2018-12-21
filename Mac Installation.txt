Okay, so QMods on a Mac? Easy... Once you've tried it all and found the solution at last!
Getting right into things, you'll need three things:
  1. The .exe found here - https://www.nexusmods.com/subnautica/mods/16/
  2. Your built-in "Terminal" App (or any other CLI terminal)
  3. Mono Project, found here- https://www.mono-project.com/download/stable/

So, now that you have Mono Installed, you want to right-click Subnautica in Steam, then click "Prefrences at the bottom of the
pop-up menu. Go to the tab called "Local Files", and click "Browse Local Files", Put QMods.exe here.

Now, open "Terminal" (Cmd+Space; search Terminal) and enter the following command line.
cd ~/Library/"Application Support"/Steam/steamapps/common/Subnautica

Finally, enter:
mono QMods.exe


Now, if you go to the Subnautica folder you accessed at the beginning, you should see a folder titled QMods, put the mods you
want to run here and start Subnautica! 
To test your installation, try the "Custom Load Screen" mod found here: https://www.nexusmods.com/subnautica/mods/124
