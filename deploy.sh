cd src 
cd TodoApp.Api
dotnet publish --configuration Release --runtime linux-arm64 --framework net8.0
cd ..

exitCode=$?
if [ $exitCode -ne 0 ]; then 
    echo Failed with $exitCode
    exit $exitCode
fi

cd ../

cdk bootstrap
cdk deploy --require-approval never
exitCode=$?
if [ $exitCode -ne 0 ]; then 
    echo Failed with $exitCode
    exit $exitCode
fi
