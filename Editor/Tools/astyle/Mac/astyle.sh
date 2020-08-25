#!/bin/sh

### Set up astyle affected folders
CURRENT_DIR=$(pwd -P)
DIR="${CURRENT_DIR}/Assets"

init(){
  if [ ! -x "$(which astyle)" ]; then
      echo "Did not find astyle, please install it before continuing."
      exit 1
  fi

  if [ ! -d ${CURRENT_DIR}/.git/hooks ]; then
      mkdir ${CURRENT_DIR}/.git/hooks
  fi
  cp -i $1/pre-commit.git ${CURRENT_DIR}/.git/hooks/pre-commit
  cp -i $1/astyle.config ${CURRENT_DIR}/
  cp -i $1/astyle-ignore ${CURRENT_DIR}/
  chmod 777 ${CURRENT_DIR}/.git/hooks/pre-commit
  echo "SUCCESS: File copied."
}

run(){
    echo "--- running astyle ---"
    for file in $(find ${DIR} -name "*.cs"); do
        format=true
        while read line
        do
            if [[ $file == *"$line"* ]]; then
                format=false
                break
            fi
        done < "$(pwd -P)/astyle-ignore"
        if $format; then
            astyle --options="$(pwd -P)/astyle.config" $file
        else
            echo "Skipping format for: $file"
        fi
    done
    echo "--- running astyle done ---"
}

clean(){
    for folder in $DIR; do
        find $folder -name "*.bak" -type f -delete
    done
}

case "$1" in
    init)
        init $2
        ;;
    run)
        run
        ;;
    clean)
        clean
        ;;
    *)
        echo "Usage: $0 {init|run|clean}"
esac

exit 0
