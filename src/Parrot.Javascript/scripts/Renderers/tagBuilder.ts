enum TagRenderMode {
    startTag,
    endTag,
    selfClosing,
    normal
}

class TagBuilder {
    name: string;
    innerHtml: string;
    attributes: any[];

    constructor(name: string) {
        this.name = name;
        this.innerHtml = "";
        this.attributes = [];
    }

    toString(renderMode: TagRenderMode): string {
        switch (renderMode) {
            case TagRenderMode.startTag:
                return "<" + this.name + this.appendAttributes() + ">";
            case TagRenderMode.endTag:
                return "</" + this.name + ">";
            case TagRenderMode.selfClosing:
                return "<" + this.name + this.appendAttributes() + " />";
            default:
                return "<" + this.name + this.appendAttributes() + this.innerHtml + "</" + this.name + ">";
        }
    }

    appendAttributes(): string {
        var render: string = "";
        for (var i in this.attributes) {
            var attribute = this.attributes[i];
            var key: string = i;
            var value = attribute;

            if (key == "id" && value == null || value.length == 0) {
                continue;
            }

            if (value != null) {
                value = this.htmlAttributeEncode(value);
            } else {
                value = key;
            }

            render += " " + key + "=\"" + value + "\"";
        }

        return render;
    }

    htmlAttributeEncode(value: string): string {
        return value.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;").replace("'", "&#39;");
    }

    mergeAttribute(key: string, value: string, replaceExisting: bool) {
        if (key == null || key.length == 0) {
            throw new ArgumentException("key");
        }

        if (replaceExisting || !this.containsKey(this.attributes, key)) {
            this.attributes[key] = value;
        }
    }

    containsKey(source: any[], key: string) {
        for (var i in source) {
            if (i == key) {
                return true;
            }
        }

        return false;
    }

    addCssClass(value: string): void {
        if (this.attributes["class"] != undefined && this.attributes["class"] != null) {
            this.attributes["class"] = value + " " + this.attributes["class"];
        } else {
            this.attributes["class"] = value;
        }
    }
}