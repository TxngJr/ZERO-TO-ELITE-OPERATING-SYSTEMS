#!/usr/bin/env bash
set -euo pipefail

projects=(
  01-os-introduction/examples/Chapter01.csproj
  02-process-context-I/examples/Chapter02.csproj
  03-process-context-II/examples/Chapter03.csproj
  04-concurrency-I/examples/Chapter04.csproj
  05-concurrency-II/examples/Chapter05.csproj
  06-synchronization-I/examples/Chapter06.csproj
  07-synchronization-II/examples/Chapter07.csproj
  08-synchronization-III/examples/Chapter08.csproj
  09-scheduling/examples/Chapter09.csproj
  10-address-translation/examples/Chapter10.csproj
  11-virtual-memory/examples/Chapter11.csproj
)

for project in "${projects[@]}"; do
  echo "==> dotnet build $project"
  dotnet build "$project" -c Release --nologo
done
