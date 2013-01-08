var Locals = (function () {
    function Locals(host) {
        this._host = host;
        //check for locals
        if(this._host["__locals"] != undefined && this._host["__locals"] != null) {
            this._locals = this._host["__locals"];
        } else {
            this._locals = [];
        }
    }
    Locals.prototype.push = function (value) {
        this._locals.push(value);
        this._host["__locals"] = this._locals;
    };
    Locals.prototype.pop = function () {
    };
    return Locals;
})();
//@ sourceMappingURL=Locals.js.map
