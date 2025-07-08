using Amazon.SQS;
using AutoMapper.Extensions.ExpressionMapping;
using Azure.Messaging.ServiceBus;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using CqrsService.Domain.Orchestrators;
using CqrsService.Infrastructure.Options;
using CqrsService.Infrastructure.Orchestrators;
using CqrsService.Infrastructure.Persistence.InMemory;
using CqrsService.Infrastructure.Provider.External.ReqRes;
using CqrsService.Infrastructure.Provider.Messaging.AzureServiceBus;
using CqrsService.Infrastructure.Provider.Messaging.Kafka;
using CqrsService.Infrastructure.Provider.Messaging.Kafka.Models;
using CqrsService.Infrastructure.Provider.Messaging.MassTransit;
using CqrsService.Infrastructure.Provider.Messaging.Sqs;
using KafkaFlow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace CqrsService.Infrastructure.Configuration;

public static partial class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection, IConfiguration configuration, bool isLocalEnvironment)
    {
        if (isLocalEnvironment)
        {
            //Required if you need specific service setup for local development ie: AWS/Azure creds
            Log.Warning("Local Environment: Adding local services to the IoC Container");
        }
        else
        {
            Log.Warning("Deployed Environment: Adding standard services to the IoC Container");
        }

        //Add options
        serviceCollection.Configure<ReqResOptions>(configuration.GetSection(nameof(ReqResOptions))).AddOptions<ReqResOptions>();
        // serviceCollection.Configure<MongoDbOptions>(configuration.GetSection(nameof(MongoDbOptions))).AddOptions<MongoDbOptions>();
        // serviceCollection.Configure<OracleOptions>(configuration.GetSection(nameof(OracleOptions))).AddOptions<OracleOptions>();
        // serviceCollection.Configure<PostgreSqlOptions>(configuration.GetSection(nameof(PostgreSqlOptions))).AddOptions<PostgreSqlOptions>();
        // serviceCollection.Configure<RabbitMqOptions>(configuration.GetSection(nameof(RabbitMqOptions))).AddOptions<RabbitMqOptions>();
        // serviceCollection.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions))).AddOptions<RedisOptions>();
        // serviceCollection.Configure<AwsOptions>(configuration.GetSection(nameof(AwsOptions))).AddOptions<AwsOptions>();
        // serviceCollection.Configure<AzureServiceBusQueueOptions>(configuration.GetSection(nameof(AzureServiceBusQueueOptions))).AddOptions<AzureServiceBusQueueOptions>();

        //Add automapper profiles
        serviceCollection.AddAutoMapper(cfg => { cfg.AddProfile<InfrastructureAutoMapperProfile>(); cfg.AddExpressionMapping(); });

        //Add persistence setup
        // serviceCollection.AddTransient<OracleDbConnectionProvider>();
        // serviceCollection.AddTransient<PostgreSqlDbConnectionProvider>();
        // serviceCollection.AddTransient<MongoDbConnectionProvider>();

        //Redis setup
        // var redisOptions = configuration.GetSection(nameof(RedisOptions));
        // serviceCollection.TryAddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions[nameof(RedisOptions.HostName)]));

        //MongoDb setup
        /*serviceCollection.TryAddSingleton<IMongoClient>(options =>
        {
            var mongoOptions = options.GetService<IOptions<MongoDbOptions>>();

            var mongoDbConnectionString = String.Format(mongoOptions.Value.ConnectionString,
                                                        mongoOptions.Value.UserName,
                                                        mongoOptions.Value.Password,
                                                        mongoOptions.Value.ClusterEndpoint, // This must be the cluster endpoint name, not the friendly 'local' name
                                                        mongoOptions.Value.DatabaseName,
                                                        mongoOptions.Value.ReadPreference);

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoDbConnectionString);
            settings.LinqProvider = LinqProvider.V3;

            return new MongoClient(settings);
        });*/

        //Add persistence implementations
        serviceCollection.AddMemoryCache();
        serviceCollection.TryAddSingleton<IInMemoryPersistence, InMemoryPersistence>();
        // serviceCollection.TryAddSingleton<IMongoDbPersistence, MongoDbPersistence>();
        // serviceCollection.TryAddSingleton<IOraclePersistence, OraclePersistence>();
        // serviceCollection.TryAddSingleton<IPostgreSqlPersistence, PostgreSqlPersistence>();
        // serviceCollection.TryAddSingleton<IRedisPersistence, RedisPersistence>();

        //Add provider setup
        //Mass transit setup
        /*serviceCollection.AddMassTransit(x =>
        {
            Uri schedulerEndpoint = new Uri("queue:scheduler");
            x.AddMessageScheduler(schedulerEndpoint);
            x.AddConsumer<ExampleMessageConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                ConfigureQueueEndpoints(context, cfg, configuration, schedulerEndpoint);
            });

            x.UsingAmazonSqs((context, cfg) =>
            {
                var awsOptions = configuration.GetSection(nameof(AwsOptions)).Get<AwsOptions>();

                cfg.Host("eu-west-2", h =>
                {
                    h.AccessKey(awsOptions.AccessKey);
                    h.SecretKey(awsOptions.SecretKey);
                });

                ConfigureQueueEndpoints(context, cfg, configuration, schedulerEndpoint);
            });
        });*/

        //Azure service bus setup
        var clientOptions = new ServiceBusClientOptions { TransportType = ServiceBusTransportType.AmqpWebSockets };
        serviceCollection.TryAddSingleton(clientOptions);

        //Add provider implementations
        serviceCollection.AddHttpClient<IReqResProvider, ReqResProvider>();
        serviceCollection.TryAddSingleton<IReqResProvider, ReqResProvider>();
        serviceCollection.AddAWSService<IAmazonSQS>();
        serviceCollection.TryAddSingleton<ISqsProvider, SqsProvider>();
        serviceCollection.TryAddSingleton<IAzureServiceBusProvider, AzureServiceBusProvider>();
        serviceCollection.TryAddSingleton<IMassTransitProvider, MassTransitProvider>();

        //Add orchestrators
        serviceCollection.TryAddSingleton<IExamplePersonDomainOrchestrator, ExamplePersonOrchestrator>();

        return serviceCollection;
    }

    public static async void KafkaFlowStartConsuming(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var bus = serviceProvider.CreateKafkaBus();
        await bus.StartAsync();
    }
    public static void KafkaFlowSetup(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var kafkaOptions = new KafkaOptions();
        configuration.GetSection("KafkaOptions").Bind(kafkaOptions);

        serviceCollection.AddSingleton<IKafkaProducerService<CreatedPersonEvent>, KafkaProducerService>();

        //serviceCollection
        // .AddKafka(
        //    kafka => kafka
        //        .UseConsoleLog()
        //        .AddCluster(
        //            cluster => cluster
        //                .WithBrokers(kafkaOptions.BoostrapServers)
        //                .WithSchemaRegistry(schemaRegistry =>
        //                {
        //                    schemaRegistry.BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo;
        //                    schemaRegistry.BasicAuthUserInfo = kafkaOptions.SchemaRegBasicAuth;
        //                    schemaRegistry.Url = kafkaOptions.SchemaRegistryUrl.ToString();
        //                })
        //                 .WithSecurityInformation(x =>
        //                 {
        //                     x.SaslMechanism = KafkaFlow.Configuration.SaslMechanism.Plain;
        //                     x.SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol.SaslSsl;
        //                     x.SaslUsername = kafkaOptions.SaslUsername;
        //                     x.SaslPassword = kafkaOptions.SaslPassword;
        //                 })
        //                 .AddConsumer(
        //                    consumer => consumer
        //                    .WithName("abc")
        //                        .Topic(kafkaOptions.CreatedPersonEventTopicName)
        //                        .WithGroupId(kafkaOptions.ConsumerGroupId)
        //                        .WithBufferSize(100)
        //                        .WithWorkersCount(20)
        //                        .WithAutoOffsetReset(KafkaFlow.AutoOffsetReset.Earliest)
        //                        .AddMiddlewares(
        //                            middlewares => middlewares
        //                            .Add<PauseConsumerOnExceptionMiddleware>()
        //                                .AddSchemaRegistryJsonSerializer<CreatedPersonEvent>()
        //                                .AddTypedHandlers(handlers => handlers
        //                                    .AddHandler<KafkaConsumerService>()
        //                                    .WhenNoHandlerFound(context =>
        //                                        Console.WriteLine("Message not handled > Partition: {0} | Offset: {1}",
        //                                        context.ConsumerContext.Partition,
        //                                        context.ConsumerContext.Offset)
        //                                    )
        //                                )
        //                        )
        //                    )
        //                  .AddProducer<CreatedPersonEvent>(
        //                    producer => producer
        //                        .DefaultTopic(kafkaOptions.CreatedPersonEventTopicName)
        //                        .AddMiddlewares(
        //                            middlewares => middlewares
        //                                .AddSchemaRegistryJsonSerializer<CreatedPersonEvent>(
        //                                    new JsonSerializerConfig
        //                                    {
        //                                        // SubjectNameStrategy = SubjectNameStrategy.TopicRecord
        //                                    })))
        //                ));
    }

    //Add mass transit setup
    /*private static void ConfigureQueueEndpoints(IRegistrationContext context, IBusFactoryConfigurator<IReceiveEndpointConfigurator> cfg, IConfiguration configuration, Uri schedulerEndpoint)
    {
        cfg.UseMessageScheduler(schedulerEndpoint);

        cfg.ReceiveEndpoint("example-message-queue", endpoint =>
        {
            //Retry policy can be configured in a variety of ways
            cfg.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2)));

            endpoint.ConfigureConsumer<ExampleMessageConsumer>(context);
        });
    }*/
}