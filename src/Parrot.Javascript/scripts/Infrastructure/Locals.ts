class Locals {
    _host: any[];
    _locals: any[];
    constructor(host: any[]) {
        this._host = host;

        //check for locals
        if (this._host["__locals"] != undefined && this._host["__locals"] != null) {
            this._locals = this._host["__locals"];
        } else {
            this._locals = [];
        }
    }

    push(value: any) {
        this._locals.push(value);
        this._host["__locals"] = this._locals;
    }

    pop() {

    }
}