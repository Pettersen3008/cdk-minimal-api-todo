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

And finally, you will need to install Docker. You can download Docker [here](https://www.docker.com/products/docker-desktop).

## Running the project

The project can be run both localhost and be deployed to AWS. To run the project locally, you can use the following command:

```bash
docker-compose up -d
```
This will spin up a postgres database and a pgAdmin instance. You can access pgAdmin by navigating to `localhost:5050` in your browser. The default username is `postgres@postgres.com` and the default password is `postgres`.

Then you can run the following command to start the project:

```bash
dotnet run --project src/TodoApp.Api
```

To deploy the project to AWS, you can use the following command:

```bash
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


To check if everything is working as expected, you can go to our API Gateway URL / Localhost url and check if you can access the API at <a href="http://localhost:5257/scalar/v1">http://localhost:5257/scalar/v1</a>.
