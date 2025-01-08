using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.RDS;
using Constructs;
using System.Collections.Generic;
using Amazon.CDK.AWS.APIGateway;
using InstanceType = Amazon.CDK.AWS.EC2.InstanceType;

namespace TodoApp
{
    public class TodoAppStack : Stack
    {
        internal TodoAppStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var vpc = new Vpc(this, "TodoAppVpc", new VpcProps
            {
                MaxAzs = 2,
                NatGateways = 1
            });
            
            var dbSecurityGroup = new SecurityGroup(this, "DatabaseSecurityGroup", new SecurityGroupProps
            {
                Vpc = vpc,
                Description = "Security group for RDS instance",
                AllowAllOutbound = true
            });
            
            var lambdaSecurityGroup = new SecurityGroup(this, "LambdaSecurityGroup", new SecurityGroupProps
            {
                Vpc = vpc,
                Description = "Security group for Lambda function",
                AllowAllOutbound = true
            });
            
            dbSecurityGroup.AddIngressRule(
                lambdaSecurityGroup,
                Port.Tcp(5432),
                "Allow Lambda to access PostgreSQL"
            );
            
            var rdsInstance = new DatabaseInstance(this, "TodoDatabase", new DatabaseInstanceProps
            {
                Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
                {
                    Version = PostgresEngineVersion.VER_15
                }),
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                Vpc = vpc,
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
                SecurityGroups = new[] { dbSecurityGroup },
                DatabaseName = "tododb",
                MaxAllocatedStorage = 20,
                AllocatedStorage = 20,
                RemovalPolicy = RemovalPolicy.DESTROY, // Change this when production
                DeletionProtection = false // Change this when production
            });
            
            var lambdaFunction = new Function(this, "TodoApiHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "TodoApp.Api",
                Code = Code.FromAsset("./src/pubapi"),
                MemorySize = 1024,
                Timeout = Duration.Seconds(60),
                Architecture = Architecture.ARM_64,
                Environment = new Dictionary<string, string>
                {
                    // Since i have the connection string i dont need to pass the individual values (i'am no sure but we can test it)
                    // ["POSTGRES_HOST"] = rdsInstance.InstanceEndpoint.Hostname,
                    // ["POSTGRES_PORT"] = "5432",
                    // ["POSTGRES_DB"] = "tododb",
                    // ["POSTGRES_USER"] = rdsInstance.Secret?.SecretValueFromJson("username").UnsafeUnwrap() ?? "",
                    // ["POSTGRES_PASSWORD"] = rdsInstance.Secret?.SecretValueFromJson("password").UnsafeUnwrap() ?? "",
                    ["DB_CONNECTION_STRING"] = $"Host={rdsInstance.InstanceEndpoint.Hostname};Port=5432;Database=tododb;User ID={rdsInstance.Secret?.SecretValueFromJson("username").UnsafeUnwrap()};Password={rdsInstance.Secret?.SecretValueFromJson("password").UnsafeUnwrap()};Pooling=true;",
                    ["ASPNETCORE_ENVIRONMENT"] = "Production",
                    ["AWS_LAMBDA_ASPNETCORE_HOSTBUILDER_BUILDER_KEY"] = "1"
                },
                Vpc = vpc,
                VpcSubnets = new SubnetSelection { SubnetType = SubnetType.PRIVATE_WITH_EGRESS },
                SecurityGroups = new[] { lambdaSecurityGroup }
            });
            
            rdsInstance.Secret?.GrantRead(lambdaFunction);

            _ = new LambdaRestApi(this, "TodoApi", new LambdaRestApiProps
            {
                Handler = lambdaFunction,
                Proxy = true,
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowOrigins = Cors.ALL_ORIGINS,
                    AllowMethods = Cors.ALL_METHODS
                }
            });
        }
    }
}