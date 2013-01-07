class Token {
    type: TokenType;
    content: string;
    index: number;

    constructor(index: number, content: string, type: TokenType) {
        this.index = index;
        this.content = content;
        this.type = type;
    }
}