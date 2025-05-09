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

TEST_DIR="$PROJ_ROOT/tests/MateMachine.Extensions.Tests/TestResults"
TEST_DIR=$(cd "$TEST_DIR"; echo `pwd`/`ls -d */|head -n 1`)
TEST_FILE="$TEST_DIR/coverage.cobertura.xml"

OUTPUT=$("$FILE_PATH/get-code-coverage.ps1" "$TEST_FILE")
echo "Coverage is $OUTPUT"
if [ $? -eq 0 ]; then
    COMPARE=$(awk -v num1="$OUTPUT" -v num2="0.8" '
        BEGIN {
            print num1 < num2 ? "0" : "1"
        }
    ')

    if [ $OUTPUT == "1" ]; then
        cecho "GREEN" "Coverage is above 80%% "
        exit 0;
    else
        cecho "RED" "Coverage is below 80%% "
        exit 1;
    fi
else
    cecho "RED" "Unable to get coverage"
    exit 1;
fi
