# Welcome to your CDK C# project!

This is a blank project for CDK development with C#.

The `cdk.json` file tells the CDK Toolkit how to execute your app.

It uses the [.NET CLI](https://docs.microsoft.com/dotnet/articles/core/) to compile and execute your project.

## Useful commands

* `dotnet build src` compile this app
* `cdk deploy`       deploy this stack to your default AWS account/region
* `cdk diff`         compare deployed stack with current state
* `cdk synth`        emits the synthesized CloudFormation template

## Getting Started

To get this project up and running, you will need to install the AWS CDK CLI. You can do this by running the following command:

```bash
npm install -g aws-cdk
```

Next, you will need to the .NET Core SDK. You can download the SDK [here](https://dotnet.microsoft.com/download).

Once you have the CDK CLI and the .NET Core SDK installed, you can run the following commands to deploy the stack:

```bash
export AWS_ACCESS_KEY_ID=<your-access-key-id>
export AWS_SECRET_ACCESS_KEY=<your-secret-access-key>
export AWS_SESSION_TOKEN=<your-session-token>
```

```bash
chmod +x deploy.sh
```

```bash
./deploy.sh
```

This will build the project and deploy the stack to your AWS account.