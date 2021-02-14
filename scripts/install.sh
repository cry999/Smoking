#!/bin/sh -eu

set -x

SETTING_DIR=$HOME/.smoking

mkdir -p $SETTING_DIR/publish
dotnet publish -c Release -r osx-x64 -o $SETTING_DIR/bin
if [ -e "/usr/local/bin/smoking" ]; then
    rm -f /usr/local/bin/smoking
fi
ln -s $SETTING_DIR/bin/Smoking.Console /usr/local/bin/smoking
