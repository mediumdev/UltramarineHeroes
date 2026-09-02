GREEN='\033[0;32m'
RED='\033[0;33m'
NC='\033[0m' # No Color

export COMMIT=$(git log -1 --oneline --no-merges)

echo -e "${GREEN}BundleId: ${BUNDLE}${NC}"

# Запускаем сборку Unity
/Applications/Unity/Hub/Editor/2019.4.39f1/Unity.app/Contents/MacOS/Unity \
    -batchmode \
    -projectPath ./Heroes \
    -quit \
    -buildTarget ios \
    -executeMethod ProjectBuilder.BuildIos \
    -logfile stdout \
    -username ultramarin05@megion-group.ru \
    -password Solo0715CaptainNazgilias
    
echo "unity build finished"    

fastlane match adhoc --force_for_new_devices true

echo "fastlane gym start"

fastlane gym --scheme "Unity-iPhone" \
    -p "./Heroes/Heroes_iOS/Heroes/Unity-iPhone.xcodeproj" \
    --export_method ad-hoc \
    --include_bitcode false \
    --include_symbols false \
    --clean \
 
echo "fastlane gym finished"