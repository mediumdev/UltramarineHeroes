#!/bin/sh

CURRENT_DIR=$(pwd)
IPA_FILE=$(find $CURRENT_DIR -type f \( -iname \*.ipa \) -print -quit)
IPA_FOLDER=$(dirname "$IPA_FILE")

unzip "$IPA_FILE" -d "$IPA_FOLDER"

#change the certificate ename to yours. it's usually similar to this: "Apple Distribution: COMPANY NAME (TEAM_ID)"
CERTIFICATE_NAME=${CERTIFICATE_NAME}
PAYLOAD_FOLDER="$IPA_FOLDER/Payload"
APP_NAME=$(ls "$PAYLOAD_FOLDER")
APP_PATH="$PAYLOAD_FOLDER/$APP_NAME"

codesign -s "$CERTIFICATE_NAME" -f --preserve-metadata --generate-entitlement-der $APP_PATH

rm $IPA_FILE

cd "$IPA_FOLDER"

zip -r $IPA_FILE "Payload"

npm install -g appcenter-cli

set -x

export IOS_BUILD_NAME="$(find . -name '*.ipa')"
export IOS_BUILD_PATH="$(realpath ${IOS_BUILD_NAME})"

appcenter distribute release --release-notes '' --app ${APP_CENTER_USER}/${APP_NAME}  --file $IOS_BUILD_PATH --group ${GROUP_NAME} --token ${TOKEN} --silent --quiet --disable-telemetry

echo "Upload succes"

echo "Remove .ipa"

rm /Users/ultramarinestudio/UltramarineHeroes/Heroes.ipa

echo "Remove Archive"

rm -rf /Users/ultramarinestudio/Library/Developer/Xcode/Archives

echo "Complete"
