using System.Net;
using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RemoteProject.Shared.AspNet.Helpers;
using RemoteProject.Shared.AspNet.Jobs;
using RemoteProject.Shared.AspNet.Models;

namespace RemoteProject.Shared.AspNet.ServiceMesh;

public class ConsulRegistryService : BackgroundJobBase
{
    private readonly IConsulClient _consulClient;


    private readonly bool _isEnabled;

    private readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(3);
    private readonly TimeSpan _defaultInterval = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _defaultDeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1);
    private readonly bool _defaultSkipTlsVerify = true;


    private readonly AgentServiceRegistration _agentServiceRegistration;
    private readonly AgentCheckRegistration _agentCheckRegistration;

    private readonly ILogger<ConsulRegistryService> _logger;

    public ConsulRegistryService(
        IConsulClient consulClient,
        IOptions<ServiceDiscoveryConfig> serviceDiscoveryConfig,
        IServer? server,
        ILogger<ConsulRegistryService> logger
    ) : this(consulClient, serviceDiscoveryConfig.Value, server, logger)
    {
    }

    private ConsulRegistryService(
        IConsulClient consulClient,
        ServiceDiscoveryConfig serviceDiscoveryConfig,
        IServer? server,
        ILogger<ConsulRegistryService> logger
    ) : base(logger, serviceDiscoveryConfig.ReloadInterval, "ConsulRegistryJob")
    {
        _consulClient = consulClient;
        _logger = logger;
        var serviceDiscoveryConfig1 = serviceDiscoveryConfig ?? new ServiceDiscoveryConfig();
        var serverAddressesFeature = server?.Features?.Get<IServerAddressesFeature>()!;

        var address = serverAddressesFeature?.Addresses?.FirstOrDefault(x => x.StartsWith("http://"))
                      ?? serverAddressesFeature?.Addresses?.FirstOrDefault() ?? "http://localhost:5000";

        var config = serviceDiscoveryConfig1;
        _isEnabled = config.IsEnabled;

        var uri = new Uri(address);
        var serviceUri = InternalHelpers.BuildServiceUri(config.ThisServiceHost, config.ThisServicePort);
        var serviceHost = !string.IsNullOrWhiteSpace(config.ThisServiceHost) ? serviceUri.Host : uri.Host;
        var servicePort = config.ThisServicePort > 0 ? config.ThisServicePort : uri.Port;
        var serviceHealthCheckUri =
            InternalHelpers.BuildServiceUri(config.ThisServiceHealthCheckHost, config.ThisServiceHealthCheckPort);
        var serviceHealthCheckHost = !string.IsNullOrWhiteSpace(config.ThisServiceHealthCheckHost)
            ? serviceHealthCheckUri.Host
            : uri.Host;
        var serviceHealthCheckPort =
            config.ThisServiceHealthCheckPort > 0 ? config.ThisServiceHealthCheckPort : uri.Port;
        var healthCheckPath = config.ThisServiceHealthCheckPath ?? HealthChecksConfigurations.HealthCheckPath;
        var healthCheck = $"{serviceUri.Scheme}://{serviceHealthCheckHost}:{serviceHealthCheckPort}{healthCheckPath}";

        _agentCheckRegistration = new AgentCheckRegistration
        {
            // ID = $"{config.ServiceId}_check",
            Name = $"{config.ServiceId}_check",
            HTTP = healthCheck,
            Method = "GET",
            Notes = $"Checks {healthCheck}.",
            Timeout = config.RegistrationTimeout ?? _defaultTimeout,
            Interval = config.RegistrationInterval ?? _defaultInterval,
            DeregisterCriticalServiceAfter = config.RegistrationDeregisterCriticalServiceAfter ??
                                             _defaultDeregisterCriticalServiceAfter,
            TLSSkipVerify = config.RegistrationTLSSkipVerify ?? _defaultSkipTlsVerify
        };
        _agentServiceRegistration = new AgentServiceRegistration
        {
            ID = config.ServiceId,
            Name = config.ServiceId,
            Address = serviceHost,
            Port = servicePort,
            Tags = config.Tags,

            Checks = new[]
            {
                _agentCheckRegistration
            }
        };

        logger.LogInformation(
            "[SHA] @ConsulRegistryService.ctor, Instantiated AgentServiceRegistration, IsEnabled:{IsEnabled}, " +
            "ServiceId:{ServiceId}, Address:{Address}, Port:{Port}" +
            "serviceHealthCheckHost:{serviceHealthCheckHost}, serviceHealthCheckPort:{serviceHealthCheckPort}, " +
            "RegistrationInterval:{RegistrationInterval}, RegistrationTLSSkipVerify:{RegistrationTLSSkipVerify}",
            _isEnabled,
            config.ServiceId,
            serviceHost,
            servicePort,
            serviceHealthCheckHost,
            serviceHealthCheckPort,
            config.RegistrationInterval,
            config.RegistrationTLSSkipVerify
        );
    }

    protected override async Task Execute(int iterationCount, CancellationToken cancellationToken)
    {
        if (!_isEnabled)
        {
            return;
        }

        var node = await _consulClient.Agent.GetNodeName(cancellationToken);

        var result = await _consulClient.Agent.Reload(node, cancellationToken);

        var status = result.StatusCode == HttpStatusCode.OK
            ? "succeeded"
            : "failed";

        _logger.LogInformation(
            "[SHA] @ConsulRegistryService.Reload, Reloading registration for {AgentServiceId} have {status} with status of {statusCode}, iteration: {IterationCount}.",
            _agentServiceRegistration.ID,
            status,
            result.StatusCode,
            iterationCount
        );
    }

    public override async Task StartAsync(
        CancellationToken cancellationToken
    )
    {
        if (!_isEnabled)
        {
            return;
        }

        // await _consulClient.Agent.CheckDeregister(_agentCheckRegistration.ID, cancellationToken);
        // await _consulClient.Agent.CheckRegister(_agentCheckRegistration, cancellationToken);

        var deregisterResult =
            await _consulClient.Agent.ServiceDeregister(_agentServiceRegistration.ID, cancellationToken);

        if (deregisterResult.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogInformation(
                "[SHA] @ConsulRegistryService.Register, Got {status} when tried to deregister an old {AgentServiceId}.",
                deregisterResult.StatusCode,
                _agentServiceRegistration.ID
            );
        }

        var registerResult =
            await _consulClient.Agent.ServiceRegister(_agentServiceRegistration, true, cancellationToken);

        var status = registerResult.StatusCode == HttpStatusCode.OK
            ? "succeeded"
            : "failed";

        _logger.LogInformation(
            "[SHA] @ConsulRegistryService.Register, Registering {AgentServiceId} have {status} with status of {statusCode}.",
            _agentServiceRegistration.ID,
            status,
            registerResult.StatusCode
        );

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken
    )
    {
        if (!_isEnabled)
        {
            return;
        }

        var checkDeregisterResult =
            await _consulClient.Agent.CheckDeregister(_agentCheckRegistration.ID, cancellationToken);

        if (checkDeregisterResult.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogInformation(
                "[SHA] @ConsulRegistryService.Deregister, Got {status} when tried to deregister an old {AgentServiceId}.",
                checkDeregisterResult.StatusCode,
                _agentServiceRegistration.ID
            );
        }


        var serviceDeregisterResult =
            await _consulClient.Agent.ServiceDeregister(_agentServiceRegistration.ID, cancellationToken);

        var status = serviceDeregisterResult.StatusCode == HttpStatusCode.OK
            ? "succeeded"
            : "failed";

        _logger.LogInformation(
            "[SHA] @ConsulRegistryService.Register, Deregistering {AgentServiceId} have {status} with status of {statusCode}.",
            _agentServiceRegistration.ID,
            status,
            serviceDeregisterResult.StatusCode
        );

        await base.StopAsync(cancellationToken);
    }
}
