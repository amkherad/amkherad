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

echo "Running pre-commit hooks"

cecho "CYAN" "Running tests"

rm -r "$PROJ_ROOT/tests/MateMachine.Extensions.Tests/TestResults/" &>/dev/null

dotnet test "$PROJ_ROOT/MateMachine.Extensions.sln" --collect:"XPlat Code Coverage"

Result=$?

if [ $? -eq 0 ]; then
  cecho "GREEN" "Tests were successful"
else
  cecho "RED" "Tests have failed"
  exit 1;
fi

"$FILE_PATH/code-coverage.bash"

COVERAGE_OUTPUT=$?

rm -r "$PROJ_ROOT/tests/MateMachine.Extensions.Tests/TestResults/" &>/dev/null

exit $COVERAGE_OUTPUT
