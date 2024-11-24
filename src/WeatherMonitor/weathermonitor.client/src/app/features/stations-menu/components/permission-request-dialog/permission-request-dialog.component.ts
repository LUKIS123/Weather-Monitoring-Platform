import {
  Component,
  computed,
  inject,
  Inject,
  OnInit,
  signal,
} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { StationWithAddress } from '../../models/station-with-address';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { ToastService } from '../../../../shared/services/toast.service';
import { GetStationPermissionStatusService } from '../../services/get-station-permission-status.service';
import { StationPermissionStatusResponse } from '../../models/station-permission-status';
import { finalize } from 'rxjs';
import { FormatDateService } from '../../../../shared/services/format-date.service';
import { AuthorizationService } from '../../../authorization/services/authorization-service';
import { SendPermissionRequestService } from '../../services/send-permission-request.service';

@Component({
  selector: 'app-permission-request-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule, TranslateModule],
  templateUrl: './permission-request-dialog.component.html',
})
export class PermissionRequestDialogComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly formatDateService = inject(FormatDateService);
  private readonly getStationPermissionService = inject(
    GetStationPermissionStatusService
  );
  private readonly sendPermissionRequestService = inject(
    SendPermissionRequestService
  );
  private authService = inject(AuthorizationService);
  isLoggedIn = this.authService.isAuthorized;

  #weatherStationData = signal<StationWithAddress | null>(null);
  public readonly weatherStationData = this.#weatherStationData.asReadonly();
  #stationStatusResponse = signal<StationPermissionStatusResponse | null>(null);
  public readonly stationStatusResponse =
    this.#stationStatusResponse.asReadonly();

  #isLoading = signal(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #disable = signal(false);
  buttonDisable = computed(
    () =>
      this.#disable() || !this.#stationStatusResponse()?.canRequestPermission
  );

  constructor(
    @Inject(MAT_DIALOG_DATA) data: { station: StationWithAddress },
    private dialogRef: MatDialogRef<StationWithAddress>
  ) {
    this.#weatherStationData.set(data.station);
  }

  ngOnInit(): void {
    const deviceId = this.#weatherStationData()?.station.id;
    if (deviceId) {
      this.loadStatus(deviceId);
    } else {
      this.toastService.openError(
        this.translateService.instant('AvailableStations.RequestDialog.Error')
      );
    }
  }

  private loadStatus(deviceId: number): void {
    this.getStationPermissionService
      .getStationPermissions(deviceId)
      .pipe(finalize(() => this.#isLoading.set(false)))
      .subscribe({
        next: (status) => {
          this.#stationStatusResponse.set(status);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant(
              'AvailableStations.RequestDialog.Error'
            )
          );
        },
      });
  }

  public getUserRole(): string {
    if (this.stationStatusResponse()?.stationUserPermission?.userRole === 2) {
      return this.translateService.instant('UserAccount.Admin');
    } else if (
      this.stationStatusResponse()?.stationUserPermission?.userRole === 1
    ) {
      return this.translateService.instant('UserAccount.User');
    }
    return this.translateService.instant('UserAccount.Unknown');
  }

  public getDateFormatted(): string {
    return this.formatDateService.formatDate(
      this.stationStatusResponse()?.stationUserPermission?.changeDate ?? ''
    );
  }

  public getPermissionStatus(): string {
    const status =
      this.stationStatusResponse()?.stationUserPermission.permissionStatus;
    switch (status) {
      case 1:
        return this.translateService.instant('PermissionStatus.NotRequested');
      case 2:
        return this.translateService.instant('PermissionStatus.Pending');
      case 3:
        return this.translateService.instant('PermissionStatus.Granted');
      case 4:
        return this.translateService.instant('PermissionStatus.Denied');
      default:
        return this.translateService.instant('PermissionStatus.Unknown');
    }
  }

  cancel() {
    this.dialogRef.close();
  }

  sendPermissionRequest() {
    const stationId = this.#weatherStationData()?.station.id;
    if (stationId === null || stationId === undefined) {
      this.toastService.openError(
        this.translateService.instant('AvailableStations.SendRequest.Error')
      );
      return;
    }

    this.#isLoading.set(true);
    this.#disable.set(true);

    this.sendPermissionRequestService
      .sendRequest(stationId)
      .pipe(
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
      .subscribe({
        next: () => {
          this.toastService.openSuccess(
            this.translateService.instant(
              'AvailableStations.SendRequest.Success'
            )
          );
          setTimeout(() => {
            this.dialogRef.close();
          }, 2000);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('AvailableStations.SendRequest.Error')
          );
          this.#disable.set(false);
        },
      });
  }
}
