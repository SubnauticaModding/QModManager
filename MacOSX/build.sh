#!/bin/bash

# Variables
CONFIGURATION="BZ.STABLE"
VERSION="4.0.2.3"
echo "[$CONFIGURATION]"

# Clean up build and dist folder
DEPS="./Dependencies"
BUILD="./Build/$CONFIGURATION"
DIST="./Build/MacOSX/$CONFIGURATION"
rm -rf ./Build
rm -rf $DIST
mkdir -p $DIST
mkdir -p "./$DIST/BepInEx/patchers/QModManager/"
mkdir -p "./$DIST/BepInEx/plugins/QModManager/"
mkdir -p "./$DIST/doorstop_libs"

# Download QModManager.exe
echo "~> downloading QModManager.exe"
wget -q -O "$DIST/QModManager.zip" "https://github.com/SubnauticaModding/QModManager/archive/refs/tags/v$VERSION.zip"
unzip -qq "$DIST/QModManager.zip" -d "$DIST/QModManager"
cp "$DIST/QModManager/QModManager-$VERSION/Build/QModManager.exe" "./$DIST/BepInEx/patchers/QModManager/QModManager.exe"
cp "$DIST/QModManager/QModManager-$VERSION/Build/QModManager.exe.config" "./$DIST/BepInEx/patchers/QModManager/QModManager.exe.config"
rm "$DIST/QModManager.zip"
rm -rf "$DIST/QModManager"

# Download doorstop libs
echo "~> downloading doorstop libs"
wget -q -O "./$DIST/doorstop_libs/libdoorstop_x64.dylib" https://github.com/SphereII/DMTBridgeLoaderPlugin/raw/main/doorstop_libs/libdoorstop_x64.dylib
wget -q -O "./$DIST/doorstop_libs/libdoorstop_x86.dylib" https://github.com/SphereII/DMTBridgeLoaderPlugin/raw/main/doorstop_libs/libdoorstop_x86.dylib

# Build projects
echo "~> building OculusNewtonsoftRedirect"
msbuild ./OculusNewtonsoftRedirect/QModManager.OculusNewtonsoftRedirect.csproj -property:Configuration=$CONFIGURATION -verbosity:quiet 2>&1 > /dev/null
echo "~> building QModManager"
msbuild ./QModManager/QModManager.csproj -property:Configuration=$CONFIGURATION -verbosity:quiet 2>&1 > /dev/null
echo "~> building UnityAudioFixer"
msbuild ./UnityAudioFixer/QModManager.UnityAudioFixer.csproj -property:Configuration=$CONFIGURATION -verbosity:quiet 2>&1 > /dev/null
echo "~> building QModPluginGenerator"
msbuild ./QModPluginEmulator/QModManager.QModPluginGenerator.csproj -property:Configuration=$CONFIGURATION -property:PostBuildEvent= -verbosity:quiet 2>&1 > /dev/null

# Copy Build Files
echo "~> creating distribution package"
cp -r "$DEPS/BepInEx/BepInEx" "./$DIST"
cp "$DEPS/BepInEx/doorstop_config.ini" "./$DIST"
cp "$DEPS/BepInEx/winhttp.dll" "./$DIST"
cp "$DEPS/cldb.dat" "./$DIST/BepInEx/patchers/QModManager/"
cp "./packages/AssetsTools.NET.2.0.3/lib/net35/AssetsTools.NET.dll" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModManager.QModPluginGenerator.dll" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModManager.UnityAudioFixer.dll" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModManager.UnityAudioFixer.xml" "./$DIST/BepInEx/patchers/QModManager/"
cp "$BUILD/QModInstaller.dll" "./$DIST/BepInEx/plugins/QModManager/"
cp "$BUILD/QModInstaller.xml" "./$DIST/BepInEx/plugins/QModManager/"

# Copy Build Files for SN.SABLE
if [ "$CONFIGURATION" != "SN.STABLE" ]; then
  cp "$DEPS/Oculus.Newtonsoft.Json.dll" "./$DIST/BepInEx/patchers/QModManager/"
  cp "$BUILD/QModManager.OculusNewtonsoftRedirect.dll" "./$DIST/BepInEx/patchers/QModManager/"
fi

# Copy Launcher
cp "./MacOSX/dist/QModManager.sh" "./$DIST/QModManager.sh"
chmod +x "./$DIST/QModManager.sh"
echo "~> distribution package created at $DIST"

# Build Installer Package
chmod -R 755 $DIST
PKG_BUILD="./MacOSX/PKG.$CONFIGURATION"
mkdir -p $PKG_BUILD
pkgbuild --identifier org.QModManager.$VERSION --version $VERSION --install-location "/Library/Application Support/Steam/steamapps/common/SubnauticaZero" --root "$DIST" $PKG_BUILD/QModManager.pkg > /dev/null 2>&1
productbuild --distribution ./MacOSX/dist/Distribution --resources ./MacOSX/dist/Resources --package-path $PKG_BUILD "./Build/MacOSX/QModManager-$VERSION.$CONFIGURATION.pkg" > /dev/null 2>&1
rm -rf $PKG_BUILD
echo "~> installer package created at Build/MacOSX/QModManager-$VERSION.$CONFIGURATION.pkg"
