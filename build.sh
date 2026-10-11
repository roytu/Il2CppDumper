#!/usr/bin/env bash
# Builds Il2CppDumper as a single self-contained executable into out/ using the newest .NET SDK installed
set -e
cd "$(dirname "$0")"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet is not installed. Grab the .NET SDK from https://dotnet.microsoft.com/download (or brew install --cask dotnet-sdk) and run this again."
  exit 1
fi
if ! sdk="$(dotnet --version 2>/dev/null)"; then
  echo "dotnet is installed but only the runtime, not the SDK. Grab the SDK from https://dotnet.microsoft.com/download and run this again."
  exit 1
fi

tfm="net$(echo "$sdk" | cut -d. -f1).0"
case "$(uname -s)-$(uname -m)" in
  Darwin-arm64)  rid=osx-arm64 ;;
  Darwin-*)      rid=osx-x64 ;;
  Linux-aarch64) rid=linux-arm64 ;;
  Linux-*)       rid=linux-x64 ;;
  *)             rid=win-x64 ;;
esac

dotnet publish Il2CppDumper/Il2CppDumper.csproj -c Release -f "$tfm" -r "$rid" --self-contained -p:TargetFrameworks="$tfm" -p:PublishSingleFile=true -o out
