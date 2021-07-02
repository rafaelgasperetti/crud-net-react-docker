/*!
*  Blink
* 
*/

$(function () {
    'use strict';

    if ($('[data-toggle="tooltip"]').length > 0) {
        $('[data-toggle="tooltip"]').tooltip();
    }

    if ($('#modalIndisponivel').length > 0) {
        $('#modalIndisponivel').modal('show');
    }


    if ($(window).width() >= 992) {

        var scrollValue = 0;

        $(window).scroll(function () {

            if ($(window).scrollTop() > 0) {
                $('body').addClass('menu-fixed');
                $('.header').addClass('fixed').addClass('small');
            } else {
                $('.header').removeClass('small');
                setTimeout(function () {
                    $('.header').removeClass('fixed');
                    $('body').removeClass('menu-fixed');
                }, 400);
            }

            if ($(window).scrollTop() > scrollValue) {
                // baixo
                $('.header').removeClass('open');
            } else {
                // cima
                $('.header').addClass('open');
            }

            scrollValue = $(window).scrollTop();

        });
    }


    $('[name="btn-checkout"]').click(function (e) {
        e.preventDefault()
        var sucess = $('.checkout--sucesso');
        var el = $(sucess).find('.icobutton');

        sucess.removeClass('d-none');
        $('.checkout').find('.link-texto').css('opacity', 0);

        setTimeout(function () {
            timeline.replay();
        }, 400);

        var scaleCurve = mojs.easing.path('M0,100 L25,99.9999983 C26.2328835,75.0708847 19.7847843,0 100,0');
        var elSpan = $(el).find('span');
        // mo.js timeline obj
        var timeline = new mojs.Timeline();

        // tweens for the animation:

        // burst animation
        var tween1 = new mojs.Burst({
            parent: el,
            duration: 2000,
            shape: 'circle',
            fill: '#008ed1',
            opacity: 1,
            childOptions: { radius: { 20: 0 } },
            radius: { 80: 160 },
            count: 8,
            isSwirl: true,
            easing: mojs.easing.bezier(0.1, 1, 0.3, 1)
        });
        // ring animation
        var tween2 = new mojs.Transit({
            parent: el,
            duration: 500,
            type: 'circle',
            radius: { 0: 70 },
            fill: '#FF8D0A',
            stroke: '#FF8D0A',
            strokeWidth: { 15: 0 },
            opacity: 1,
            easing: mojs.easing.bezier(0, 1, 0.5, 1)
        });
        // icon scale animation
        var tween3 = new mojs.Tween({
            duration: 900,
            onUpdate: function (progress) {
                var scaleProgress = scaleCurve(progress);

                $(elSpan).css('WebkitTransform', $(elSpan).css('transform', 'scale3d(' + scaleProgress + ',' + scaleProgress + ',1)'))
            }
        });

        // add tweens to timeline:
        timeline.add(tween1, tween2, tween3);
    })

    // if ($(window).width() < 992) {
    //     if ($('.cards.cards-produto').length > 0) {
    //         var width = ($(".cards.cards-produto").children().width() * $(".cards").children().length) / $(".cards-container > .overflow-auto").length + 50;
    //         $('.cards.cards-produto').width(width);
    //     }

    //     if ($('.cards.cards-social').length > 0) {
    //         var widthSocial = ($(".cards.cards-social").children().width() * $(".cards.cards-social").children().length) + 40;
    //         $('.cards.cards-social').width(widthSocial);
    //     }
    // }

    // DATE PICKER

    if ($('input[name="daterange"]').length > 0) {
        $('input[name="daterange"]').daterangepicker({
            autoApply: true,
            "locale": {
                "format": "DD/MM/YYYY",
                "separator": " a ",
                "daysOfWeek": [
                    "Dom",
                    "Seg",
                    "Ter",
                    "Qua",
                    "Qui",
                    "Sex",
                    "Sab"
                ],
                "monthNames": [
                    "Janeiro",
                    "Fevereiro",
                    "Março",
                    "Abril",
                    "Maio",
                    "Junho",
                    "Julho",
                    "Agosto",
                    "Setembro",
                    "Outubro",
                    "Novembro",
                    "Dezembro"
                ],
                "firstDay": 1,
            },
            "minDate": moment().subtract(180, 'days'),
            "maxDate": moment(),
            "startDate": moment().subtract(7, 'days'),
            "endDate": moment()
        });
    }

    /*if ($('input[class="datepicker"]').length > 0) {
        $('.datepicker').datepicker({
            autoApply: true,
            "locale": {
                "format": "DD/MM/YYYY",
                "separator": " a ",
                "daysOfWeek": [
                    "Dom",
                    "Seg",
                    "Ter",
                    "Qua",
                    "Qui",
                    "Sex",
                    "Sab"
                ],
                "monthNames": [
                    "Janeiro",
                    "Fevereiro",
                    "Março",
                    "Abril",
                    "Maio",
                    "Junho",
                    "Julho",
                    "Agosto",
                    "Setembro",
                    "Outubro",
                    "Novembro",
                    "Dezembro"
                ],
                "firstDay": 1,
            },
            "startDate": moment(),
            "format": 'dd/mm/yyyy',
            "changeYear": true,
            "changeMonth": true,
            "todayHighlight": true,
            "autoclose": true,
            "yearRange": "1930:2100"
        });
    }*/

    // SELECT

    function selected(wrapper, select) {

        if (!select[0].selected) {
            $(wrapper).addClass('selected');
        } else {
            $(wrapper).removeClass('selected');
        }
    }

    $('.select').each(function () {
        var select = $(this).find('select').children()

        selected(this, select);

        $(this).change(function () {
            selected(this, select)
        })

    });

    function invocarOverlay(div) {

        $(div).append('<div class="overlay"></div>');

        $('body').css('overflow', 'hidden');

        if ($(window).width() >= 992) {
            setTimeout(function () {
                $('.overlay').addClass('active');
            }, 10);
        } else {
            setTimeout(function () {
                $('.overlay').addClass('active');
            }, 400);
        }
    }

    $('.header_nav .nav-dropdown, .header_nav_breadcrumb a').click(function (e) {
        e.preventDefault();

        var that = this;

        if ($(this).next('.nav_sub-menu').length > 0) {

            if (!$(this).next('.nav_sub-menu').hasClass('active') && !$(this).parent().parent().hasClass('active')) {
                var otherLink = $('.nav_sub-menu--interno.active').prev();
                var otherNav = $('.nav_sub-menu--interno.active');

                $(otherLink).removeClass('active');
                $(otherNav).removeClass('active');
                setTimeout(function () {
                    $(otherNav).removeAttr('style');
                }, 400);
            } else if ($(this).parent().parent().hasClass('active') && $(this).parent().parent().find('.nav_sub-menu--interno.active').length > 0) {

                var otherLink = $(this).parent().parent().find('.nav_sub-menu--interno.active').prev();
                var otherNav = $(this).parent().parent().find('.nav_sub-menu--interno.active');

                $(otherLink).removeClass('active');
                $(otherNav).removeClass('active');
                setTimeout(function () {
                    $(otherNav).removeAttr('style');
                }, 400);
            }

            $(this).toggleClass('active');

            if ($(this).next('.nav_sub-menu').hasClass('nav_sub-menu--interno')) {

                if ($(this).next('.nav_sub-menu--interno').hasClass('active')) {

                    $(this).next('.nav_sub-menu--interno').removeClass('active');

                    setTimeout(function () {
                        $(that).next('.nav_sub-menu--interno').removeAttr('style');
                    }, 400);

                } else {

                    $(this).next('.nav_sub-menu--interno').css('display', 'block');

                    setTimeout(function () {
                        $(that).next('.nav_sub-menu--interno').addClass('active');
                    }, 100);

                }

            } else {
                $(this).next('.nav_sub-menu').slideToggle();

                if ($(this).next('.nav_sub-menu').find('.nav_sub-menu--interno').length > 0 && $(this).next('.nav_sub-menu').find('.nav_sub-menu--interno').hasClass('active')) {

                    $(this).next('.nav_sub-menu').find('.nav-dropdown').removeClass('active');
                    $(this).next('.nav_sub-menu').find('.nav_sub-menu--interno').removeClass('active');
                    setTimeout(function () {
                        $(that).next('.nav_sub-menu').find('.nav_sub-menu--interno').removeAttr('style');
                    }, 400);

                }
            }

        } else {
            var link = $(this).attr('href'),
                before = $('.nav--mobile .header_nav_cont.center');

            if ($(this).parent().hasClass('header_nav_breadcrumb')) {
                before.removeClass('center').addClass('right');
                $(link).removeClass('left').addClass('center');
            } else {
                before.removeClass('center').addClass('left');
                $(link).removeClass('right').addClass('center');
            }
        }

    });

    $(document).on('click', function (e) {

        if ($('.nav-dropdown.active').length > 0 && !$(e.target).hasClass('nav-dropdown') && $('.header_nav').find(e.target).length === 0) {

            $('.nav-dropdown.active').each(function () {
                var that = this;
                if ($(this).next().hasClass('active')) {
                    $(this).next().removeClass('active');
                    setTimeout(function () {
                        $(that).next().removeAttr('style');
                    }, 400);
                } else {
                    $(this).next().slideUp('fast');
                }

                $(this).removeClass('active');
            });

        }

    });

    /*if ($('.carrossel_principal').length > 0) {
        $('.carrossel_principal').slick({
            slidesToScroll: 1,
            slidesToShow: 1,
            infinite: true,
            speed: 1000,
            dots: true,
            loop: true,
            arrows: true,
            prevArrow: '<button class="btn btn--icon btn-prev"><span class="icon icon-chevron-left"></span></button>',
            nextArrow: '<button class="btn btn--icon btn-right"><span class="icon icon-chevron-right"></span></button>'
        })
    }*/

    $('.btn-menu').click(function () {
        $('.nav--mobile').addClass('active');

        invocarOverlay('.header');
    });

    $('.dropdown_user .dropdown-toggle').click(function () {
        var link = $(this).data('href');
        $('.' + link).addClass('active');

        invocarOverlay('.header');
    });

    $('.dropdown_cart .dropdown-toggle').click(function () {
        $('.cart').addClass('active');

        invocarOverlay('.header');
    });

    $(document).on('click', '.overlay, .btn-close', function () {

        if ($('.nav--mobile').hasClass('active')) {
            $('.nav--mobile').removeClass('active');
            setTimeout(function () {
                if ($('.nav--mobile #menuPrincipal').hasClass('left')) {
                    $('.nav--mobile .sub-menu.left').removeClass('left').addClass('right');
                    $('.nav--mobile .sub-menu.center').removeClass('center').addClass('right');
                    $('.nav--mobile #menuPrincipal').removeClass('left').addClass('center');
                }
            }, 400);
        }

        if ($('.login').hasClass('active')) {
            $('.login').removeClass('active');
        }

        if ($('.logado').hasClass('active')) {
            $('.logado').removeClass('active');
        }

        if ($('.cart').hasClass('active')) {
            $('.cart').removeClass('active');
        }

        $('.overlay').removeClass('active');
        setTimeout(function () {
            $('.overlay').remove();
        }, 400);

        $('body').removeAttr('style');
    });

    $(document).on('click', '.btn-user', function (e) {

        if (!$('.login').hasClass('active')) {
            if ($('.nav--mobile').hasClass('active')) {
                $('.nav--mobile').removeClass('active');
                setTimeout(function () {
                    if ($('.nav--mobile #menuPrincipal').hasClass('left')) {
                        $('.nav--mobile .sub-menu.left').removeClass('left').addClass('right');
                        $('.nav--mobile .sub-menu.center').removeClass('center').addClass('right');
                        $('.nav--mobile #menuPrincipal').removeClass('left').addClass('center');
                    }
                }, 400);
            }

            if ($('.cart').hasClass('active')) {
                $('.cart').removeClass('active');
            }

            $('.overlay').remove();
            $('body').removeAttr('style');

            $('.dropdown_user .dropdown-toggle[data-href]').click();
        } else {
            e.preventDefault();
        }

    });

    // if($('.fornecedores-select').length > 0){
    //     $('.fornecedores-select').select2();
    // }

    if ($('.input-password').length > 0) {
        $('.input-password .btn').click(function (e) {
            e.preventDefault();

            if ($(this).parent().find('.form-control').attr('type') === 'text') {
                $(this).parent().find('.form-control').attr('type', 'password');
            } else {
                $(this).parent().find('.form-control').attr('type', 'text');
            }
        });
    }

    $('.dropdown_user, .dropdown_cart').on('hide.bs.dropdown', function () {
        $('.overlay').remove();
        $('body').removeAttr('style');
    });

    // $('.btn-add').click(function (e) {
    //     e.preventDefault();

    //     var that = this;

    //     $(this).addClass('active');
    //     $(this).html('');
    //     $(this).attr('disabled', 'disabled');
    //     $(this).append('<div class="spinner-border spinner-border-sm text-light"></div>');

    //     setTimeout(function () {
    //         $(that).find('.spinner-border').fadeOut(300);
    //     }, 1500);

    //     setTimeout(function () {
    //         $(that).append('<span style="display: none;">Adicionado</span>');
    //         $(that).find('span').fadeIn(300)
    //     }, 1800);

    //    setTimeout(function () {
    //        $(that).find('.loader').remove();
    //        $(that).removeClass('active');
    //        $(that).text('Adicionar');
    //    }, 2800);

    //     setTimeout(function () {
    //         $(that).removeAttr('disabled');
    //     }, 3000);

    // });

    $('.btn-verMais').click(function (e) {
        e.preventDefault();

        var holder = $(this).parent().parent().find('.form-filtro-holder');

        if ($(this).text() === 'Ver mais') {
            $(this).text('Ver menos');
        } else {
            $(this).text('Ver mais');
        }

        holder.find('.form-group.hidden').fadeToggle();
    });
});