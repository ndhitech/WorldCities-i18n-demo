import { Component, OnInit, OnChanges, ViewChild, inject, Input, DoCheck } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';

import { Country } from './country';
import { CountryService } from './country.service';
import { AuthService } from '../auth/auth.service';

import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { TranslateParser, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.scss']
})


export class CountriesComponent implements OnInit, OnChanges, DoCheck {
  [x: string]: any;

  //@Input()
  //language: string = "en";

  public displayedColumns: string[] = ['id', 'name', 'isO2', 'isO3',  'totCities'];
  public countries: MatTableDataSource<Country> = new MatTableDataSource<Country>();
  public authService: AuthService = inject(AuthService);
  defaultPageIndex: number = 0;
  defaultPageSize: number = 10;
  //pageSize?: number;
  public defaultSortColumn: string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";

  defaultFilterColumn: string = "name";
  filterQuery?: string;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  filterTextChanged: Subject<string> = new Subject<string>();

  constructor(
    private translateService: TranslateService,
    private countryService: CountryService
  ) {
    //this.setPainatorLables(translateService);
    //this.paginator._intl.itemsPerPageLabel = "ZXD";
    console.log("Country constructor called.");
   
  }
  
  ngOnInit() {
    this.loadData();
    console.log("Country Laoddata called.")
    this.translateService.onLangChange.subscribe((lang) => { this.setPainatorLables(); });

  }
  ngOnChanges() {
    this.setPainatorLables();
    console.log("Country ngOnChanges called.");

    // Initialize the translations once at construction time
    //this.setPainatorLables();
  }
  ngDoCheck(){
  }
  ngAfterViewInit() {
  }
  setPainatorLables()
  {
    //this.paginator._intl.firstPageLabel = translateService.instant('globle.paginator.first.page');
    //this.paginator._intl.itemsPerPageLabel = this.translateService.instant('globle.paginator.items.per.page');
    //this.paginator._intl.getRangeLabel()
    console.log('setPainatorLables called!.');
    //this.paginator.la
    //this.paginator._intl.lastPageLabel = translateService.instant('globle.paginator.last.page');
    //this.paginator._intl.nextPageLabel = translateService.instant('globle.paginator.next.page');
    //this.paginator._intl.previousPageLabel = translateService.instant('globle.paginator.previous.page');
   // this.paginator.lastPage = this.translateService.instant('globle.paginator.range');
  }
  // debounce filter text changes
  onFilterTextChanged(filterText: string) {
    console.log("Country onFilter Text Change called.")
    if (!this.filterTextChanged.observed) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        });
    }
    this.filterTextChanged.next(filterText);
  }
  loadData(query?: string) {
    var pageEvent = new PageEvent();
    pageEvent.pageIndex = this.defaultPageIndex;
    pageEvent.pageSize = this.defaultPageSize;   
    this.filterQuery = query;
    this.getData(pageEvent);
  }

  getData(event: PageEvent) {
    var sortColumn = (this.sort)
      ? this.sort.active
      : this.defaultSortColumn;
    var sortOrder = (this.sort)
      ? this.sort.direction
      : this.defaultSortOrder;
    var filterColumn = (this.filterQuery)
      ? this.defaultFilterColumn
      : null;
    var filterQuery = (this.filterQuery)
      ? this.filterQuery
      : null;
    this.countryService.getData(
      event.pageIndex,
      event.pageSize!,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery)
      .subscribe({
        next: (result) => {
          this.paginator.length = result.totalCount;
          this.paginator.pageIndex = result.pageIndex;
          this.paginator.pageSize = result.pageSize;
          this.paginator._intl.lastPageLabel = "Last";
          //this.paginator._intl.itemsPerPageLabel = this.translateService.instant('globle.paginator.items.per.page');
          //this.paginator._intl.firstPageLabel = this.translateService.instant('globle.paginator.first.page');
          this.paginator._intl.itemsPerPageLabel = this.translateService.instant('globle.paginator.first.page');
          //this.paginator._intl.lastPageLabel = this.translateService.instant('globle.paginator.last.page');
          //this.paginator._intl.nextPageLabel = this.translateService.instant('globle.paginator.next.page');
          //this.paginator._intl.previousPageLabel = this.translateService.instant('globle.paginator.previous.page');
          //this.paginator. = this.translateService.instant('globle.paginator.range');
          this.countries = new MatTableDataSource<Country>(result.data);
        },
        error: (error) => console.error(error)
      });
  }
}
