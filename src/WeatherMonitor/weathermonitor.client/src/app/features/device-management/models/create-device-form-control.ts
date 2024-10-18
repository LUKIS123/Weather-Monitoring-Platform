import { FormControl } from '@angular/forms';

export interface CreateDeviceDeviceFormControl {
  id: FormControl<number | null>;
  googleMapsPlusCode: FormControl<string | null>;
  isActivate: FormControl<string | null>;
  username: FormControl<string | null>;
  password: FormControl<string | null>;
  clientId: FormControl<string | null>;
  topic: FormControl<string | null>;
}
