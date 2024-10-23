#!/bin/bash

# Usage: ./update_version.sh RetackAI_SDK_V_4.csproj 1.0.1

csproj_file=$1
new_version=$2

if [ -z "$csproj_file" ] || [ -z "$new_version" ]; then
    echo "Usage: $0 <csproj-file> <new-version>"
    exit 1
fi

# Update the version in the .csproj file
sed -i "s/<Version>.*<\/Version>/<Version>${new_version}<\/Version>/" $csproj_file

echo "Updated version to $new_version in $csproj_file"
