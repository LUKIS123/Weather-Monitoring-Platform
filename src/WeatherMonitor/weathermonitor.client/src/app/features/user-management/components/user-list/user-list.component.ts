import { Component, inject, OnInit, signal } from '@angular/core';
import { User } from '../../models/user';
import {
  MatPaginatorIntl,
  MatPaginatorModule,
  PageEvent,
} from '@angular/material/paginator';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { debounceTime, finalize, Subject } from 'rxjs';
import { PageResult } from '../../../../shared/models/page-result';
import { ToastService } from '../../../../shared/services/toast.service';
import { GetUsersService } from '../../services/get-users.service';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../../shared/material.module';
import { MatPaginatorIntlPl } from '../../../../shared/components/paginator/MatPaginatorIntlPl';
import { UserListElementComponent } from '../user-list-element/user-list-element.component';
import { ErrorStateMatcher } from '@angular/material/core';
import {
  FormControl,
  FormGroup,
  FormGroupDirective,
  FormsModule,
  NgForm,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export class SearchInputErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(
    control: FormControl | null,
    form: FormGroupDirective | NgForm | null
  ): boolean {
    const isSubmitted = form && form.submitted;
    return !!(
      control &&
      control.invalid &&
      (control.dirty || control.touched || isSubmitted)
    );
  }
}

export interface NicknameSeachFormControl {
  nicknameSearchPhrase: FormControl<string | null>;
}

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    TranslateModule,
    MatPaginatorModule,
    UserListElementComponent,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
  ],
  providers: [{ provide: MatPaginatorIntl, useClass: MatPaginatorIntlPl }],
  templateUrl: './user-list.component.html',
})
export class UserListComponent implements OnInit {
  private readonly translateService = inject(TranslateService);
  private readonly toastService = inject(ToastService);
  private readonly getUsersService = inject(GetUsersService);

  matcher = new SearchInputErrorStateMatcher();
  public formGroup = new FormGroup<NicknameSeachFormControl>({
    nicknameSearchPhrase: new FormControl(null, [
      Validators.minLength(1),
      Validators.maxLength(255),
    ]),
  });

  private inputSubject = new Subject<string>();

  #usersPageResult = signal<PageResult<User>>({
    items: [],
    totalPages: 0,
    itemsFrom: 0,
    itemsTo: 0,
    totalItemsCount: 0,
    pageSize: 0,
  });
  public usersPageResult = this.#usersPageResult.asReadonly();

  #isLoading = signal<boolean>(true);
  public readonly isLoading = this.#isLoading.asReadonly();
  #currentPage = signal<number>(0);
  public readonly currentPage = this.#currentPage.asReadonly();

  ngOnInit(): void {
    this.#isLoading.set(true);
    const nickname = this.formGroup.get('nicknameSearchPhrase')?.value ?? null;
    this.loadUsers(this.#currentPage(), nickname);
    this.inputSubject.pipe(debounceTime(300)).subscribe(() => {
      if (this.isFormValid) {
        this.submit();
      }
    });
  }

  public get isFormValid() {
    return this.formGroup.valid;
  }

  onInputChange($event: Event) {
    const input = ($event.target as HTMLInputElement).value;
    this.inputSubject.next(input);
  }

  submit() {
    const nickname = this.formGroup.get('nicknameSearchPhrase')?.value ?? null;
    this.#isLoading.set(true);
    this.loadUsers(this.#currentPage(), nickname);
  }

  pageEvent!: PageEvent;
  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.#currentPage.set(e.pageIndex);
    const nickname = this.formGroup.get('nicknameSearchPhrase')?.value ?? null;
    this.loadUsers(this.#currentPage(), nickname);
  }

  public refresh(): void {
    const nickname = this.formGroup.get('nicknameSearchPhrase')?.value ?? null;
    this.#isLoading.set(true);
    this.loadUsers(this.#currentPage(), nickname);
  }

  private loadUsers(
    pageNumber: number,
    nicknameSearch: string | null = null
  ): void {
    this.getUsersService
      .getUsers(pageNumber + 1, nicknameSearch)
      .pipe(
        finalize(() => {
          this.#isLoading.set(false);
        })
      )
      .subscribe({
        next: (result) => {
          this.#usersPageResult.set(result);
        },
        error: () => {
          this.toastService.openError(
            this.translateService.instant('UserManagement.Users.Error')
          );
        },
      });
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onChangeDetected(change: boolean, _index: number) {
    if (change) {
      this.refresh();
    }
  }
}
