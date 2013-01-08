var TagRenderMode;
(function (TagRenderMode) {
    TagRenderMode._map = [];
    TagRenderMode._map[0] = "startTag";
    TagRenderMode.startTag = 0;
    TagRenderMode._map[1] = "endTag";
    TagRenderMode.endTag = 1;
    TagRenderMode._map[2] = "selfClosing";
    TagRenderMode.selfClosing = 2;
    TagRenderMode._map[3] = "normal";
    TagRenderMode.normal = 3;
})(TagRenderMode || (TagRenderMode = {}));
var TagBuilder = (function () {
    function TagBuilder(name) {
        this.name = name;
        this.innerHtml = "";
        this.attributes = [];
    }
    TagBuilder.prototype.toString = function (renderMode) {
        switch(renderMode) {
            case TagRenderMode.startTag: {
                return "<" + this.name + this.appendAttributes() + ">";

            }
            case TagRenderMode.endTag: {
                return "</" + this.name + ">";

            }
            case TagRenderMode.selfClosing: {
                return "<" + this.name + this.appendAttributes() + " />";

            }
            default: {
                return "<" + this.name + this.appendAttributes() + this.innerHtml + "</" + this.name + ">";

            }
        }
    };
    TagBuilder.prototype.appendAttributes = function () {
        var render = "";
        for(var i in this.attributes) {
            var attribute = this.attributes[i];
            var key = i;
            var value = attribute;
            if(key == "id" && value == null || value.length == 0) {
                continue;
            }
            if(value != null) {
                value = this.htmlAttributeEncode(value);
            } else {
                value = key;
            }
            render += " " + key + "=\"" + value + "\"";
        }
        return render;
    };
    TagBuilder.prototype.htmlAttributeEncode = function (value) {
        return value.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;").replace("'", "&#39;");
    };
    TagBuilder.prototype.mergeAttribute = function (key, value, replaceExisting) {
        if(key == null || key.length == 0) {
            throw new ArgumentException("key");
        }
        if(replaceExisting || !this.containsKey(this.attributes, key)) {
            this.attributes[key] = value;
        }
    };
    TagBuilder.prototype.containsKey = function (source, key) {
        for(var i in source) {
            if(i == key) {
                return true;
            }
        }
        return false;
    };
    TagBuilder.prototype.addCssClass = function (value) {
        if(this.attributes["class"] != undefined && this.attributes["class"] != null) {
            this.attributes["class"] = value + " " + this.attributes["class"];
        } else {
            this.attributes["class"] = value;
        }
    };
    return TagBuilder;
})();
//@ sourceMappingURL=tagBuilder.js.map
