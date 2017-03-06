#!/bin/bash
BASE_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
pushd "$BASE_DIR"/..

function check_program() {
    PROGRAM=$1
    echo "Checking $PROGRAM"
    StyleCop.Baboon Automation/Settings.StyleCop "$PROGRAM"/ "$PROGRAM"/bin "$PROGRAM"/obj && rm StyleCopViolations.xml
    gendarme --ignore "Automation/gendarme_$PROGRAM.ignore" --html "Automation/gendarme_$PROGRAM.html" "$PROGRAM"/bin/Debug/"$PROGRAM".{exe,dll}
}

xbuild Xenosaga.sln

check_program XenoPacker
check_program XenoJavusk

popd
