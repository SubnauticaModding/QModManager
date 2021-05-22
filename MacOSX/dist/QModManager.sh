#!/bin/sh
EXECUTABLE_NAME="SubnauticaZero.app"

export DOORSTOP_ENABLE=TRUE
export DOORSTOP_INVOKE_DLL_PATH="${PWD}/BepInEx/core/BepInEx.Preloader.dll"

ARCH=""
EXECUTABLE_NAME=`basename "${EXECUTABLE_NAME}" .app`
REAL_EXECUTABLE_NAME=`defaults read "${PWD}/${EXECUTABLE_NAME}.app/Contents/Info" CFBundleExecutable`
EXECUTABLE_PATH="${PWD}/${EXECUTABLE_NAME}.app/Contents/MacOS/${REAL_EXECUTABLE_NAME}"
EXECUTABLE_TYPE=`LD_PRELOAD="" file -b "${EXECUTABLE_PATH}"`;

case $EXECUTABLE_TYPE in
    *64-bit*)
        arch="x64"
        ;;
    *32-bit*|*i386*)
        arch="x86"
        ;;
    *)
        echo "Cannot identify executable type (got ${EXECUTABLE_TYPE})!"
        echo "Please create an issue at https://github.com/BepInEx/BepInEx/issues."
        exit 1
        ;;
esac

DOORSTOP_LIBS="${PWD}/doorstop_libs"
DOORSTOP_LIBNAME=libdoorstop_${arch}.dylib

export LD_LIBRARY_PATH="${DOORSTOP_LIBS}":${LD_LIBRARY_PATH}
export LD_PRELOAD=$DOORSTOP_LIBNAME:$LD_PRELOAD
export DYLD_LIBRARY_PATH="${DOORSTOP_LIBS}"
export DYLD_INSERT_LIBRARIES="${DOORSTOP_LIBS}/$DOORSTOP_LIBNAME"

"${EXECUTABLE_PATH}"
