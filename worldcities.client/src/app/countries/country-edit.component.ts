import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Country } from './country';

import { BaseFormComponent } from '../base/base-form.component';
import { CountryService } from './country.service';
import { TranslateService } from '@ngx-translate/core';

@Component(
  {
    selector: 'app-country-edit',
    templateUrl: './country-edit.component.html',
    styleUrls: ['./country-edit.component.scss']
  }
)
export class CountryEditComponent
  extends BaseFormComponent
  implements OnInit
{

  // the view title
  title?: string;
  // the form model
  //form!: FormGroup;
  // the country object to edit or create
  button?: string;
  country?: Country;
  // the country object id, as fetched from the active route:
  // It's NULL when we're adding a new country,
  // and not NULL when we're editing an existing one.
  id?: number;
  // the countries array for the select
  countries?: Country[];
  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private countryService: CountryService,
    translateService: TranslateService
  )
  {
    super(translateService);
  }
  ngOnInit() {
    this.form = this.fb.group({
      name: ['',
        Validators.required,
        this.isDupeField("name")
      ],
      isO2: ['',
        [
          Validators.required,
          Validators.pattern(/^[a-zA-Z]{2}$/)
        ],
        this.isDupeField("isO2")
      ],
      isO3: ['',
        [
          Validators.required,
          Validators.pattern(/^[a-zA-Z]{3}$/)
        ],
        this.isDupeField("isO3")
      ]
    });
    this.loadData();
  }
  loadData() {
    // retrieve the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDIT MODE
      // fetch the country from the server
      //var url = environment.baseUrl + "api/Countries/" + this.id;
      this.countryService.get(this.id).subscribe(
        {
        next: (result) => {
          this.country = result;
            this.title = this.country.name;
          // update the form with the country value
          this.form.patchValue(this.country);
        },
        error: (error) => console.error(error)
        }
      );
    }
    else {
      // ADD NEW MODE
      this.title = "";
    }
  }
  onSubmit() {
    var country = (this.id) ? this.country : <Country>{};
    if (country) {
      country.name = this.form.controls['name'].value;
      country.isO2 = this.form.controls['isO2'].value;
      country.isO3 = this.form.controls['isO3'].value;
      if (this.id) {
        // EDIT mode
        //var url = environment.baseUrl + 'api/Countries/' + country.id;
        this.countryService
          .put(country)
          .subscribe(
            {
              next: (result) => {
                  console.log("Country " + country!.id + " has been updated.");
                  // go back to countries view
                  this.router.navigate(['/countries']);
              },
                error: (error) => console.error(error)
            }
          );
      }
      else {
        // ADD NEW mode
        country.id = 0;
        this.countryService
          .post(country)
          .subscribe(
            {
              next: (result) => {
                console.log("Country " + result.id + " has been created.");
                // go back to countries view
                this.router.navigate(['/countries']);
              },
              error: (error) => console.error(error)
            }
          );
      }
    }
  }
  isDupeField(fieldName: string): AsyncValidatorFn {

    return (control: AbstractControl): Observable<{
      [key: string]: any
    } | null> => {
      return this.countryService.isDupeField(
        this.id ?? 0,
        fieldName,
        control.value)
        .pipe(map(result => {
          return (result ? { isDupeField: true } : null);
        }));
    }
  }
}
