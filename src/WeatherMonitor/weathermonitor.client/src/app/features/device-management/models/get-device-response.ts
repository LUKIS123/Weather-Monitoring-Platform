export interface GetDeviceResponse {
  id: number;
  googleMapsPlusCode: string | null;
  isActive: boolean;
  deviceExtraInfo: string | null;
  mqttUsername: string;
  mqttBrokerClientId: string;
}
