if (iPath == undefined) {
    var iPath = {};
}

iPath.Star = function (domId) {
    var $Star = undefined;
    if (domId != undefined) {
        $Star = $('#' + domId);
    } else {
        $Star = $('.star_panel');
    }
    $Star.each(function () {
        var $sp = $(this);
        var data = $sp.attr('data');
        data = Number(data);
        var starIndex = 0;
        for (; starIndex < Math.floor(data) ; starIndex++) {
            $sp.find('.star:eq(' + starIndex + ')>div:eq(1)').css('width', '100%');
        }
        var remainder = data % 1;
        remainder = remainder * 100;
        remainder = remainder.toFixed(0) + '%';
        $sp.find('.star:eq(' + starIndex + ')>div:eq(1)').css('width', remainder);
    });
    
}


iPath.ClickStar = function () {
    $('.clickstar').click(function () {
        var index = $(this).index();
        index++;
        $(this).parent().find('.clickstar').removeClass('clickstar_checked');
        $(this).parent().find('.clickstar:lt(' + index + ')').addClass('clickstar_checked');

    });
}

