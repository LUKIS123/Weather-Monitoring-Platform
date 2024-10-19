import { FormControl } from '@angular/forms';

export interface RegisterDeviceFormControl {
  deviceUsername: FormControl<string | null>;
  googleMapsPlusCode: FormControl<string | null>;
  deviceExtraInfo: FormControl<string | null>;
}
