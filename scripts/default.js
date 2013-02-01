(function ($, undefined) {
    var parrot = {
        parse: function (params) {
            var defaults = { host: [], model: null, source: "" };
            $.extend(defaults, params);
            var parser = new Parser();
            var view = new ParrotDocumentView(parser.parse(params.source));

            params.host["Model"] = params.model;

            return view.render(params.host, params.model);
        }
    };

    $.fn.parrot = function (model, host) {
        var template = $(this).html();
        return parrot.parse({
            host: host ? host : [],
            model: model,
            source: template
        });
    };

    window.parrot = parrot;
})(jQuery);

var modal = (function ($, undefined) {
    var modal = {
        hideWindow: function (element) {
            if ($(element).length) {
                $(element).removeClass("modal").hide();
            } else {
                $(".modal").removeClass("modal").hide();
            }

            $("body .mask").animate({ opacity: 0, duration: 1000 }, function () { $(this).remove(); });
        },

        showWindow: function (element) {
            console.log("showing", element);
            this.showMask();

            var modal = $(element);
            var position = this.calculatePosition(modal);
            modal.addClass("modal");
            modal.css({
                top: position.top,
                left: position.left,
                zIndex: 50001,
                position: 'absolute'
            });
            modal.show();
            setTimeout(function () {
                modal.find("input:first").focus();
            }, 1000);
        },

        showMask: function () {
            var mask = $("body .mask");
            if (!mask.length) {
                console.log("creating mask");
                mask = $("<div />").addClass("mask").css({ opacity: 0 });
                $("body").append(mask);
            }

            mask.css({
                opacity: 0,
                backgroundColor: '#000',
                position: 'fixed',
                top: 0,
                left: 0,
                right: 0,
                bottom: 0,
                background: '#000',
                opacity: 0,
                zIndex: 5000,
            }).animate({ opacity: .3, duration: 1000 });
        },

        calculatePosition: function (element, properWidth, properHeight) {
            var w = $(window);
            var e = $(element);

            if (!properWidth) {
                properWidth = e.outerWidth();
            }

            if (!properHeight) {
                properHeight = e.outerHeight();
            }

            var t = ((w.height() - properHeight) / 2) + w.scrollTop();
            var l = ((w.width() - properWidth) / 2) + w.scrollLeft();
            t = t < 0 ? 0 : t;
            l = l < 0 ? 0 : l;

            return {
                top: t,
                left: l
            };
        }
    };

    $(document).on("click", ".mask", function (e) {
        modal.hideWindow();
    });

    return modal;
})(jQuery);

(function ($, undefined) {

    $(document).ready(function () {
        // Page scrolling
        $(function () {
            $('nav a').bind('click', function (e) {
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
        $("#template").val([
            "div#firstname.nameInfo {",
            "    label > 'First name'",
            "    span > @FirstName",
            "}",
            "div#lastname.nameInfo {",
            "    label > 'Last name'",
            "    span > @LastName",
            "}",
            "div#age.nameInfo {",
            "    label > 'Age'",
            "    span > @Age",
            "}",
            "div#address.addressInfo >",
            "    div(Address) {",
            "        div > =StreetAddress",
            "        div > '=City, =State =PostalCode'",
            "    }",
            "ul#phoneNumbers.phone(PhoneNumber) > ",
            "    li {",
            "        span > =Type",
            "        span > =Number",
            "    }"].join("\n")
        );

        $("#model").val([
            "{",
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
            " }"].join("\n")
        );

        $("#parse").click(function () {
            var model = $.parseJSON($("#model").val());
            var template = $("#template").val();

            var parser = new Parser();
            var host = [];
            host["Model"] = model;
            var view = new ParrotDocumentView(parser.parse(template));

            var result = view.render(host, model);

            //$("#output-html").text(result);
            $("#output").html(result);
            modal.showWindow("#output-container");
        });

        $("#clear").click(function () {
            modal.hideWindow();
            $("#output").html("");
        });

        //$(document).on("click", "#view-source", function (e) {
        //    e.preventDefault();
        //    var source = $("#homepage").html();
        //    $("div.source pre").html(source);
        //    modal.showWindow("div.source");
        //    return false;
        //});

        var pre = document.createElement('pre');
        pre.id = "view-source"

        // private scope to avoid conflicts with demos
        $(document).on("click", function (event) {
            if (event.target.hash == '#view-source') {
                // event.preventDefault();
                if (!document.getElementById('view-source')) {
                    // pre.innerHTML = ('<!DOCTYPE html>\n<html>\n' + document.documentElement.innerHTML + '\n</html>').replace(/[<>]/g, function (m) { return {'<':'&lt;','>':'&gt;'}[m]});
                    var xhr = new XMLHttpRequest();

                    // original source - rather than rendered source
                    xhr.onreadystatechange = function () {
                        if (this.readyState == 4 && this.status == 200) {
                            console.log(this.responseText);
                            pre.innerHTML = this.responseText.replace(/[<>]/g, function (m) { return { '<': '&lt;', '>': '&gt;' }[m] });
                        }
                    };

                    document.body.appendChild(pre);
                    // really need to be sync? - I like to think so
                    xhr.open("GET", window.location, true);
                    xhr.send();
                }
                document.body.className = 'view-source';

                var sourceTimer = setInterval(function () {
                    if (window.location.hash != '#view-source') {
                        clearInterval(sourceTimer);
                        document.body.className = '';
                    }
                }, 200);
            }
        });

    });
})(jQuery);