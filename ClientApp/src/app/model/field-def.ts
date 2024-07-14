declare var $: any;

export class MatTableColDef {
    public PropertyName: string = '';
    public SortProperty?: string = '';
    public IsSelected?: boolean = true;
    public Title: string = '';
    public IsEditable?: boolean = true; // this is for action column
    public CssClassProp?: string = '';
    public CssStyleProp?: any = '';
    public BindProperty?: any = '';
    public hoverTitle?: any = ''
    public CustomTemplateName?: string = '';
    public FooterValue?: any;
    public ExpandableSubColModel?: MatTableColDef[] = null;
    public ExpandableDataProp?: string = '';
    public filterdata?: string[] = [];
    public colFilterData?: any[] = [];
    public enableColumnSearch?: boolean = false;
    //public FooterValue?:string;
    // private _footerVal?:string;

    // public get FooterValue(): string{
    //   return this._footerVal;
    // }

    // public set FooterValue(val){
    //   this._footerVal = val.toString();
    // }

    public mapServiceObj?(matTableColDef: MatTableColDef) {
        $.extend(this, matTableColDef);
    }
}

export class FieldDef<T> {
    public PropertyName: keyof T;
    public Title: string;
    public BindFunction: (t: T) => any;
    public showOnUI?: boolean = true;

    public mapServiceObj?(matTableColDef: MatTableColDef) {
        $.extend(this, matTableColDef);
    }
}
