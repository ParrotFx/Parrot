doctype('html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"')
html[xmlns="http://www.w3.org/1999/xhtml"] {
	head {
		title("Home Page")
		link[href="http://hattanshobokshi.com/content/site.css" rel="stylesheet" type="text/css"]
		link[rel="alternate" type="application/rss+xml" title="Hattan Shobokshi's RSS Feed" href="http://feeds.feedburner.com/theCodeHobo"]
	}
	body {
		div.holder {
			div.top_curve
			div.left_shade1
			div.center_content {
				div.header {
					div[style="float:left;border:0px solid black;width:200px"] {
						a[href="/"] {
							img[src="http://hattanshobokshi.com/content/images/hattanlogo.png" alt="logo" border="0"]
						}
					}
					
					div.navigation {
						a.nav[href="http://hattanshobokshi.com/"]("home")
						a.nav[href="http://hattanshobokshi.com/Home/About"]("about")
						a.nav[href="http://hattanshobokshi.com/Home/Resources"]("resources")
						a.nav[href="http://hattanshobokshi.com/delicious.html"]("Links")
						a.nav[href="http://hattanshobokshi.com/Home/Contact"]("contact")
					}
				}

				div.mainContent(this) {
					div {
						h2 {
							a[href=Url target="_blank"](Title)
						}

						h1(DatePosted)
						div.blogDescription {
							:Description
						}
					}
					div.hr
				}
			}
			div.right_shade1
			br[clear="all"]
		}
		div.bottom_curve
		div.footer("&copy2010 Hattan Shobokshi.")
	}
}