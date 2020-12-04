function RestaurantList() { };
RestaurantList.fn = RestaurantList.prototype;
RestaurantList.fn.$restaurant = undefined;
RestaurantList.fn.Init = function () {
    var that = this;
    that.$restaurant = $('#restaurant').iPathDataBind();

    $('#btnSearch').click(function () {
        that.ClickSearch($(this));
    });

    $('#querykey').next().click(function () {
        globalCache.addresslist.Show();
    });

    /* 筛选条件相关 BEGIN */
    $('.filtertools a').click(function () {
        var _popup = $(this).attr('_popup');
        $('.popup_categories, .popup_order, .popup_filter').hide();
        $('.' + _popup).show();
    });
    $('.popup_categories li').click(function () {
        $('.filtertools a:eq(0)').attr('_data', $(this).attr('_data'));
        $('.popup_categories, .popup_order, .popup_filter').hide();
        $('.popup_categories li').removeClass('checked');
        $(this).addClass('checked');
        $('#btnSearch').click();

    });
    $('.popup_order li').click(function () {
        $('.filtertools a:eq(1)').attr('_data', $(this).attr('_data'));
        $('.popup_categories, .popup_order, .popup_filter').hide();
        $('.popup_order li').removeClass('checked');
        $(this).addClass('checked');
        $('#btnSearch').click();
    });
    $('.popup_filter li').click(function () {
        $('.filtertools a:eq(2)').attr('_data', $(this).attr('_data'));
        $('.popup_categories, .popup_order, .popup_filter').hide();
        $('.popup_filter li').removeClass('checked');
        $(this).addClass('checked');
        $('#btnSearch').click();
    });
    /* 筛选条件相关 END */
}
RestaurantList.fn.Show = function () {
    //$('body>div').hide();
    //$('#restaurantlist').show();
    foodHadnler.showpanel('restaurantlist');
    $('.popup_categories, .popup_order, .popup_filter').hide();
}
// 搜索按钮
RestaurantList.fn.ClickSearch = function (jDom) {
    var that = this;
    var keyword = $('#querykey').val();
    var geo = $('#querykey').attr('geo');
    var category_id = $('.filtertools a:eq(0)').attr('_data');
    var order_by = $('.filtertools a:eq(1)').attr('_data');
    var new_restaurant = $('.filtertools a:eq(2)').attr('_data');

    iPath.Post(contextUri + '/Food/LoadRestaurant',
        {
            keyword: keyword,
            geo: geo,
            category_id: category_id,
            order_by: order_by,
            new_restaurant: new_restaurant
        },
        function (d) {
            if (d.state != 1) {
                return;
            }
            var data = d.rows;
            //data[0].restaurant_id = '1290494'; // 这个id是测试的商店 买多少东西 都不会送货的
            that.$restaurant.DataBind(data);
            $('#restaurant>li').click(function () {
                $('#restaurant>li').removeClass('selected');
                $(this).addClass('selected');
                that.ClickRestaurant($(this));
            });
            // 星标
            iPath.Star();

        },
        'json');
}
// 点击餐厅
RestaurantList.fn.ClickRestaurant = function (jDom) {
    var restaurantId = jDom.attr('_id');
    var agent_fee = jDom.attr('_agent_fee');
    var deliver_amount = jDom.attr('_deliver_amount');
    var restaurant_name = jDom.attr('_restaurant_name');
    var image_url = jDom.attr('_image_url');
    globalCache.route.Navigation('/Food/List',
        {
            restaurantId: restaurantId,
            agent_fee: agent_fee,
            restaurant_name: restaurant_name,
            deliver_amount: deliver_amount,
            image_url: image_url
        }, '');
}