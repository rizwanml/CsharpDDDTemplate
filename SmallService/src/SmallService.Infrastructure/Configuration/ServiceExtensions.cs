using AutoMapper.Extensions.ExpressionMapping;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using SmallService.Domain.InfrastructureContracts.Processors;
using SmallService.Domain.InfrastructureContracts.Repositories;
using SmallService.Infrastructure.Options;
using SmallService.Infrastructure.Abstractions.Messaging.Kafka;
using SmallService.Infrastructure.Abstractions.Messaging.Kafka.Models;
using SmallService.Infrastructure.Abstractions.Persistence.InMemory;
using SmallService.Infrastructure.Processors;
using SmallService.Infrastructure.Providers.ReqRes;
using SmallService.Infrastructure.Repositories;

namespace SmallService.Infrastructure.Configuration;

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

        //Add automapper profiles
        serviceCollection.AddAutoMapper(cfg => { cfg.AddProfile<InfrastructureAutoMapperProfile>(); cfg.AddExpressionMapping(); });

        //Add persistence
        serviceCollection.AddMemoryCache();
        serviceCollection.AddTransient<IInMemory, InMemory>();

        //Add repositories
        serviceCollection.AddTransient<IExamplePersonRepository, ExamplePersonRepository>();

        //Add provider implementations
        serviceCollection.AddHttpClient<IReqResProvider, ReqResProvider>();
        serviceCollection.TryAddSingleton<IReqResProvider, ReqResProvider>();

        //Add processors
        serviceCollection.AddTransient<IExampleProcessor, ExampleProcessor>();

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

        serviceCollection
         .AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(kafkaOptions.BoostrapServers)
                        .WithSchemaRegistry(schemaRegistry =>
                        {
                            schemaRegistry.BasicAuthCredentialsSource = AuthCredentialsSource.UserInfo;
                            schemaRegistry.BasicAuthUserInfo = kafkaOptions.SchemaRegBasicAuth;
                            schemaRegistry.Url = kafkaOptions.SchemaRegistryUrl.ToString();
                        })
                         .WithSecurityInformation(x =>
                         {
                             x.SaslMechanism = KafkaFlow.Configuration.SaslMechanism.Plain;
                             x.SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol.SaslSsl;
                             x.SaslUsername = kafkaOptions.SaslUsername;
                             x.SaslPassword = kafkaOptions.SaslPassword;
                         })
                         .AddConsumer(
                            consumer => consumer
                            .WithName("abc")
                                .Topic(kafkaOptions.CreatedPersonEventTopicName)
                                .WithGroupId(kafkaOptions.ConsumerGroupId)
                                .WithBufferSize(100)
                                .WithWorkersCount(20)
                                .WithAutoOffsetReset(KafkaFlow.AutoOffsetReset.Earliest)
                                .AddMiddlewares(
                                    middlewares => middlewares
                                    .Add<PauseConsumerOnExceptionMiddleware>()
                                        .AddSchemaRegistryJsonSerializer<CreatedPersonEvent>()
                                        .AddTypedHandlers(handlers => handlers
                                            .AddHandler<KafkaConsumerService>()
                                            .WhenNoHandlerFound(context =>
                                                Console.WriteLine("Message not handled > Partition: {0} | Offset: {1}",
                                                context.ConsumerContext.Partition,
                                                context.ConsumerContext.Offset)
                                            )
                                        )
                                )
                            )
                          .AddProducer<CreatedPersonEvent>(
                            producer => producer
                                .DefaultTopic(kafkaOptions.CreatedPersonEventTopicName)
                                .AddMiddlewares(
                                    middlewares => middlewares 
                                        .AddSchemaRegistryJsonSerializer<CreatedPersonEvent>(
                                            new JsonSerializerConfig
                                            {
                                               // SubjectNameStrategy = SubjectNameStrategy.TopicRecord
                                            })))
                        ));
    }
}