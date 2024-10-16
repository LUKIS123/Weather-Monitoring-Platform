export interface GetDeviceResponse {
  googleMapsPlusCode: string;
  isActive: boolean;
  deviceExtraInfo: string | null;
  mqttUsername: string;
  mqttBrokerClientId: string;
}
