#!/bin/bash

cd "$(dirname "$0")/../.."

set -x
PROJECTS="libpalaso chorus wesay"

for PROJ in $PROJECTS
do
  cd $PROJ
  git ls-files -z \*AssemblyInfo.cs | xargs -0 git checkout --
  git checkout -- lib/Debug
  rm -rf output externals
  cd ..
done
