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
		link[rel="stylesheet" type="text/css" href="~/content/site.css" media="screen"]

		style[type='text/css'] {
            |label { margin-right: .5em; font-weight: bold; }
            |h2 { margin-left: 50px; }
        }

		title > |Parrot Test Drive
	}
	
    body {
        header {
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
            div.inner.clearfix {
	            section#main-content {
		            form[action='/' method='post'] {
			            table {
				            tr {
					            td.half {
						            h3 > |Model
						            textarea[name='model'] > =Item2
					            }
					            td.half {
						            h3 > |Template
						            textarea[name='template' style='width:95%; height:300px;'] > =Item1
					            }
				            }
			            }
			            input[type='submit' value='Parse']
		            }
		            table[style='width:100%; text-align:left;'] {
                        tr {
                            td[colspan='2' style='text-align:left;'] > ul(Item4) > li > @Message
                        }
			            tr {
				            td.output {
					            h2 > "Output"
					            pre > @Item3
				            }
				            td.render {
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