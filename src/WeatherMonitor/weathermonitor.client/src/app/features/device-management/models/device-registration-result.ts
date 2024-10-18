import { CreateDeviceResponse } from './create-device-response';

export interface DeviceRegistrationResult {
  registered: boolean;
  createdResponse: CreateDeviceResponse | null;
}
