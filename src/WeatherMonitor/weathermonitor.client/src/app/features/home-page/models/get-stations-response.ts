export interface GetStationResponse {
  username: string;
  deviceId: number;
  googleMapsPlusCode: string;
  deviceExtraInfo?: string | null;
  isActive: boolean;
  temperature: number;
  humidity: number;
  airPressure: number;
  altitude: number;
  pM10?: number | null;
  pM1_0?: number | null;
  pM2_5?: number | null;
  receivedAt?: Date | null;
}
