using MQTTnet;
using MQTTnet.Client;

namespace WeatherMonitorCore.Shared.MqttClient.Features.MqttFactoryHelpers;

internal interface IMqttFactoryWrapper
{
    MqttClientSubscribeOptionsBuilder CreateSubscribeOptionsBuilder();
    MqttClientUnsubscribeOptionsBuilder CreateUnsubscribeOptionsBuilder();
    IMqttClient CreateMqttClient();
}

internal class MqttFactoryWrapper : IMqttFactoryWrapper
{
    private readonly MqttFactory _mqttFactory = new();

    public MqttClientSubscribeOptionsBuilder CreateSubscribeOptionsBuilder() =>
        _mqttFactory.CreateSubscribeOptionsBuilder();

    public MqttClientUnsubscribeOptionsBuilder CreateUnsubscribeOptionsBuilder() =>
        _mqttFactory.CreateUnsubscribeOptionsBuilder();

    public IMqttClient CreateMqttClient() =>
        _mqttFactory.CreateMqttClient();
}
