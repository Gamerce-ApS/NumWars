#!/bin/bash
mydir="$(dirname "$BASH_SOURCE")"
cd $mydir/../out/myproject
pod install
open "Unity-iPhone.xcworkspace"
