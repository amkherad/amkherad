#!/usr/bin/env bash

FILE_PATH=$(cd `dirname "$0"`; pwd)
PROJ_ROOT=$(dirname "$FILE_PATH")

cecho(){
  RED="\033[0;31m"
  GREEN="\033[0;32m"  # <-- [0 means not bold
  YELLOW="\033[1;33m" # <-- [1 means bold
  CYAN="\033[1;36m"
  # ... Add more colors if you like

  NC="\033[0m" # No Color

  # printf "${(P)1}${2} ${NC}\n" # <-- zsh
  printf "${!1}${2} ${NC}\n" # <-- bash
}

############################

GIT_DIR=$(git rev-parse --git-dir)

echo "Installing hooks..."
echo "$FILE_PATH/pre-commit.bash" "$PROJ_ROOT/.git/hooks/pre-commit"
ln -s "$FILE_PATH/pre-commit.bash" "$PROJ_ROOT/.git/hooks/pre-commit"

cecho "GREEN" Done!
