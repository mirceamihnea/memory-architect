#!/bin/bash
# ci/activate_license.sh
# Activează licența Unity din variabila de mediu UNITY_LICENSE
# Rulează automat înainte de orice pas Unity în CI.

set -e

echo "--- Activating Unity License ---"

if [ -z "$UNITY_LICENSE" ]; then
  echo "ERROR: UNITY_LICENSE variable is not set!"
  echo "Go to: GitLab → Settings → CI/CD → Variables and add UNITY_LICENSE"
  exit 1
fi

# Scrie licența pe disk (variabila trebuie să fie de tip 'File' în GitLab)
mkdir -p /root/.local/share/unity3d/Unity/
echo "$UNITY_LICENSE" > /root/.local/share/unity3d/Unity/Unity_lic.ulf

echo "--- License file written successfully ---"
