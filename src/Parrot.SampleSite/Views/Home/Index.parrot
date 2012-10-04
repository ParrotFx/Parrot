doctype
html {
	head {
		meta[charset='utf-8']
		meta[http-equiv="X-UA-Compatible" content="chrome=1"]
		meta[name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1"]
		link[href='https://fonts.googleapis.com/css?family=Architects+Daughter' rel='stylesheet' type='text/css']
		link[rel="stylesheet" type="text/css" href="http://thisisparrot.com/stylesheets/stylesheet.css" media="screen"]
		link[rel="stylesheet" type="text/css" href="http://thisisparrot.com/stylesheets/pygment_trac.css" media="screen"]
		link[rel="stylesheet" type="text/css" href="http://thisisparrot.com/stylesheets/print.css" media="print"]

		style[type='text/css'] > |label { margin-right: .5em; font-weight: bold; }

		title > |Parrot Test Drive
	}
	
    body {
        header[style="padding: 10px 0 10px 0;"] {
            div.inner {
                h1 > |Parrot Test Drive

                h2 > |Try out Parrot right here in your browser!
                a.button[href="https://github.com/Buildstarted/Parrot"] {
	                small > |View project on
	                |GitHub
                }
            }
        }

        div#content-wrapper {
            div.inner.clearfix[style='width:100%'] {
	            section#main-content[style='width:100%;'] {
		            form[action='/' method='post'] {
			            table[style='width:100%;'] {
				            tr {
					            td[style='width:45%'] {
						            h3('Model')
						            textarea[
								            name='model' 
								            style='width:95%; height:300px;'
						            ] > =Item2
					            }
					            td[style='width:45%'] {
						            h3('Template')
						            textarea[name='template' style='width:95%; height:300px;'] > =Item1
					            }
				            }
			            }
			            input[type='submit' value='Parse']
		            }
		            table[style='width:100%; text-align:left;'] {
			            tr {
				            td[style='width:50%; vertical-align:top; text-align:left; max-width:50%;'] {
					            h2 > "Output"
					            pre {
						            :Item3
					            }
				            }
				            td[style='width:50%; vertical-align:top; text-align:left;'] {
					            h2 > "Render"   
                                =Item3
				            }
			            }
		            }
	            }
            }	  
        }
    }
}