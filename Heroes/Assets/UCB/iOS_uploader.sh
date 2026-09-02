#!/bin/sh

set -x

export IOS_BUILD_NAME="$(find . -name '*.ipa')"
export IOS_BUILD_PATH="$(realpath ${IOS_BUILD_NAME})"

xcrun altool --upload-app -f ${IOS_BUILD_PATH} -u ${ITUNES_USERNAME}  -p ${FASTLANE_APPLE_APPLICATION_SPECIFIC_PASSWORD}