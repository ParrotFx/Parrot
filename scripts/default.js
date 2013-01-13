        $(document).ready(function () {
            // Page scrolling
            $(function () {
                $('a').bind('click', function (e)
                {
                    e.preventDefault();

                    var $anchor = $(this);

                    $("nav ul li a.active").removeClass("active");
                    $anchor.addClass("active");                    

                    $('html, body').stop().animate({
                        scrollTop: $($anchor.attr('href')).offset().top - 100
                    }, 1500, 'easeInOutExpo', function () {
                        window.location.hash = $anchor.attr("href");
                    });
                });
            });

            //// Sticky Nav
            $(window).scroll(function (e) {
                $("section").each(function (e) {
                    var section = $(this);
                    var top = section.offset().top - $(window).scrollTop();

                    if (top > 0 && top < ($(window).height() / 3)) {
                        
                        var id = section.attr("id");
                        $("nav ul li a.active").removeClass("active");
                        $("nav ul li a[href='#" + id + "']").addClass("active");
                    }
                });
            });

            //setup textareas
            $("#template").val(["div#firstname.nameInfo {",
"    label { 'First name' }",
"    span > @FirstName",
"}",
"div#lastname.nameInfo {",
"    label { 'Last name' }",
"    span > @LastName",
"}",
"div#age.nameInfo {",
"    label { 'Age' }",
"    span > @Age",
"}",
"div#address.addressInfo {",
"    div(Address) {",
"        div > =StreetAddress",
"        div > '=City, =State =PostalCode'",
"    }",
"}",
"ul#phoneNumbers.phone(PhoneNumber) {",
"    li {",
"        span > =Type",
"        span > =Number",
"    }",
"}"].join("\n"));

            $("#model").val(["{",
"     \"FirstName\": \"John\",",
"     \"LastName\" : \"Smith\",",
"     \"Age\"      : 25,",
"     \"Address\"  :",
"     {",
"         \"StreetAddress\": \"21 2nd Street\",",
"         \"City\"         : \"New York\",",
"         \"State\"        : \"NY\",",
"         \"PostalCode\"   : \"10021\"",
"     },",
"     \"PhoneNumber\":",
"     [",
"         {",
"           \"Type\"  : \"home\",",
"           \"Number\": \"212 555-1234\"",
"         },",
"         {",
"           \"Type\"  : \"fax\",",
"           \"Number\": \"646 555-4567\"",
"         }",
"     ]",
" }"].join("\n"));

            $("button").click(function () {
                var model = $.parseJSON($("#model").val());
                var template = $("#template").val();

                var parser = new Parser();
                var host = [];
                host["Model"] = model;
                var view = new ParrotDocumentView(parser.parse(template));

                var result = view.render(host, model);

                //$("#output-html").text(result);
                $("#output").html(result);
            });

        });