html {
    head {
        title('Parrot parser')
        link[href='/content/site.css' rel='stylesheet' type='text/css']
    }
    body {
        form[action='/' method='post'] {
            table[style='width:100%;'] {
                tr {
                    td[style='width:50%'] {
                        h3('Model')
                        textarea[
                                name='model' 
                                style='width:100%; height:300px;'
                        ](Item2)
                    }
                    td[style='width:50%'] {
                        h3('Template')
                        textarea[name='template' style='width:100%; height:300px;'](Item1)
                    }
                }
            }
            input[type='submit' value='Parse']
        }
        table[style='width:100%;'] {
            tr {
                td[style='width:50%; vertical-align:top;'] {
                    h3('Output')
                    pre {
                        :Item3
                    }
                }
                td[style='width:50%; vertical-align:top;'] {
                    h3('Render')
                    =Item3
                }
            }
        }
    }
}