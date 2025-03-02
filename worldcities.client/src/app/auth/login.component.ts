import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn
} from '@angular/forms';
import { BaseFormComponent } from '../base/base-form.component';
import { AuthService } from './auth.service';
import { LoginRequest } from './login-request';
import { LoginResult } from './login-result';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent
  extends BaseFormComponent implements OnInit {
  title?: string;
  loginResult?: LoginResult;
  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    translateService: TranslateService
    ) {

    super(translateService);
  }
  ngOnInit() {
    this.form = new FormGroup({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }
  onSubmit() {
    var loginRequest = <LoginRequest>{};
    loginRequest.email = this.form.controls['email'].value;
    loginRequest.password = this.form.controls['password'].value;
    this.authService
      .login(loginRequest)
      .subscribe({
        next: (result) => {
          console.log(result);
          this.loginResult = result;
          if (result.success) {
            this.router.navigate(["/"]);
          }
        },
        error: (error) => {
          console.log(error);
          if (error.status == 401) {
            this.loginResult = error.error;
          }
        }
      });
  }
}
