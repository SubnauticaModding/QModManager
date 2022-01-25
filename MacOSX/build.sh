#!/bin/bash

# Variables
CONFIGURATION="SN.STABLE"
VERSION="4.0.2.3"
echo "[$CONFIGURATION]"

# Clean up build and dist folder
DEPS="../Dependencies"
BUILD="../Build/$CONFIGURATION"
DIST="./Build/MacOSX/$CONFIGURATION"
PACK="../packages/AssetsTools.NET.2.0.9/lib/net40/"
rm -rf ./Build
rm -rf $DIST
rm -rf $PACK
mkdir -p $DIST
mkdir -p "./$DIST/BepInEx/patchers/QModManager/"
mkdir -p "./$DIST/BepInEx/plugins/QModManager/"
mkdir -p "./$DIST/doorstop_libs"
mkdir -p $PACK

# Download QModManager.exe
echo "~> downloading QModManager.exe"
wget -q -O "$DIST/QModManager.zip" "https://github.com/SubnauticaModding/QModManager/archive/refs/tags/v$VERSION.zip"
unzip -qq "$DIST/QModManager.zip" -d "$DIST/QModManager"


# Download doorstop libs
echo "~> downloading doorstop libs"
wget -q -O "./$DIST/doorstop_libs/libdoorstop_x64.dylib" https://github.com/SphereII/DMTBridgeLoaderPlugin/raw/main/doorstop_libs/libdoorstop_x64.dylib
wget -q -O "./$DIST/doorstop_libs/libdoorstop_x86.dylib" https://github.com/SphereII/DMTBridgeLoaderPlugin/raw/main/doorstop_libs/libdoorstop_x86.dylib


# Download AssetsTools
echo "~> downloading AssetTools"
wget -q -O "$DIST/netcoreapp3.1.zip" "https://github.com/nesrak1/AssetsTools.NET/releases/download/v18/netcoreapp3.1.zip"
unzip -qq "$DIST/netcoreapp3.1.zip" -d $PACK


# Build projects
echo "~> building OculusNewtonsoftRedirect"
msbuild ../OculusNewtonsoftRedirect/QModManager.OculusNewtonsoftRedirect.csproj -property:Configuration=$CONFIGURATION #-verbosity:quiet 2>&1 > /dev/null
echo "~> building QModManager"
msbuild ../QModManager/QModManager.csproj -property:Configuration=$CONFIGURATION #-verbosity:quiet 2>&1 > /dev/null
echo "~> building UnityAudioFixer"
msbuild ../UnityAudioFixer/QModManager.UnityAudioFixer.csproj -property:Configuration=$CONFIGURATION #-verbosity:quiet 2>&1 > /dev/null
echo "~> building QModPluginGenerator"
msbuild ../QModPluginEmulator/QModManager.QModPluginGenerator.csproj -property:Configuration=$CONFIGURATION -property:PostBuildEvent= #-verbosity:quiet 2>&1 > /dev/null

# Copy Build Files
echo "~> creating distribution package"
cp -r "$DEPS/BepInEx/BepInEx" "./$DIST"
cp "$DIST/QModManager/QModManager-$VERSION/Build/QModManager.exe" "./$DIST/BepInEx/patchers/QModManager/QModManager.exe"
cp "$DIST/QModManager/QModManager-$VERSION/Build/QModManager.exe.config" "./$DIST/BepInEx/patchers/QModManager/QModManager.exe.config"
cp "$DEPS/BepInEx/doorstop_config.ini" "./$DIST/"
cp "$DEPS/BepInEx/winhttp.dll" "./$DIST"
cp "$DEPS/cldb.dat" "./$DIST/BepInEx/patchers/QModManager/"
cp "$PACK/AssetsTools.NET.dll" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModManager.QModPluginGenerator.dll" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModManager.UnityAudioFixer.dll" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModManager.UnityAudioFixer.xml" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModInstaller.dll" "./$DIST/BepInEx/plugins/QModManager/"
cp "$BUILD/QModInstaller.xml" "./$DIST/BepInEx/plugins/QModManager/"

# Copy Build Files for SN.STABLE
if [ "$CONFIGURATION" != "SN.STABLE" ]; then
  echo "~> copying Oculus Newtonsoft dlls for Subzero"
  cp "$DEPS/Oculus.Newtonsoft.Json.dll" "./$DIST/BepInEx/patchers/QModManager/"
  cp "$BUILD/QModManager.OculusNewtonsoftRedirect.dll" "./$DIST/BepInEx/patchers/QModManager/"
fi

#Cleanup zips
rm "$DIST/QModManager.zip"
rm "$DIST/netcoreapp3.1.zip"
rm -rf "$DIST/QModManager"

# Copy Launcher
cp "./dist/QModManager.sh" "./$DIST/QModManager.sh"
chmod +x "./$DIST/QModManager.sh"
echo "~> distribution package created at $DIST"

# Build Installer Package
chmod -R 755 $DIST
PKG_BUILD="./MacOSX/PKG.$CONFIGURATION"
mkdir -p $PKG_BUILD
if [ "$CONFIGURATION" != "SN.STABLE" ]; then
  pkgbuild --identifier org.QModManager.$VERSION --version $VERSION --install-location "/Library/Application Support/Steam/steamapps/common/SubnauticaZero" --root "$DIST" $PKG_BUILD/QModManager.pkg 
else
  pkgbuild --identifier org.QModManager.$VERSION --version $VERSION --install-location "/Library/Application Support/Steam/steamapps/common/Subnautica" --root "$DIST" $PKG_BUILD/QModManager.pkg 
fi
productbuild --distribution ./dist/Distribution --resources ./dist/Resources --package-path $PKG_BUILD "./Build/MacOSX/QModManager-$VERSION.$CONFIGURATION.pkg"
#rm -rf $PKG_BUILD
echo "~> installer package created at Build/MacOSX/QModManager-$VERSION.$CONFIGURATION.pkg"
