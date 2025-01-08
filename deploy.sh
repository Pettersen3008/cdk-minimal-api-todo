echo $GITHUB_REF_NAME

cd src 
cd TodoApp.Api
dotnet publish --configuration Release -o ../pubapi --runtime linux-arm64 --framework net8.0
cd ..

exitCode=$?
echo $exitCode
if [ $exitCode -ne 0 ]; then 
    echo Failed with $exitCode
    exit $exitCode
fi

find ./pubapi/runtimes -mindepth 1 ! -regex '^./pubapi/runtimes/linux-arm64\(/.*\)?' -delete
rm -rf ./pubapi/datadog/osx-x64
rm -rf ./pubapi/datadog/win-x64
rm -rf ./pubapi/datadog/net461
rm -rf ./pubapi/datadog/linux-x64
rm -rf ./pubapi/runtimes/linux-musl-x64
rm -rf ./pubapi/runtimes/osx-x64
rm -rf ./pubapi/runtimes/unix
rm -rf ./pubapi/runtimes/win
rm -rf ./pubapi/runtimes/win-arm64
rm -rf ./pubapi/runtimes/win-x64
rm -rf ./pubapi/runtimes/win-x86
rm -rf ./pubapi/Magick.Native-Q16-arm64.dll.so

cd ../

cdk bootstrap
cdk deploy --require-approval never
exitCode=$?
echo $exitCode
if [ $exitCode -ne 0 ]; then 
    echo Failed with $exitCode
    exit $exitCode
fi
