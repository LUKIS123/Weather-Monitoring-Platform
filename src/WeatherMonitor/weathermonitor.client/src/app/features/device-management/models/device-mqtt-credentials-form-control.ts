import { FormControl } from '@angular/forms';

export interface DeviceMqttCredentialsFormControl {
  id: FormControl<number | null>;
  username: FormControl<string | null>;
  password: FormControl<string | null>;
  clientId: FormControl<string | null>;
  topic: FormControl<string | null>;
}
