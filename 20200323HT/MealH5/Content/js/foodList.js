function FoodList() { }
FoodList.fn = FoodList.prototype;
FoodList.fn.$foodtype = undefined;
FoodList.fn.$foodlist = undefined;
FoodList.fn.$hoppingcart = undefined;
FoodList.fn.$deliver_times = undefined;
FoodList.fn.$restaurant_info = undefined;
FoodList.fn.$restaurant_level_panel = undefined;
FoodList.fn.Init = function () {
    var that = this;
    that.$foodtype = $('#foodtype').iPathDataBind();
    that.$foodlist = $('.foodlist').iPathDataBind();
    that.$hoppingcart = $('#shoppingcart>ul').iPathDataBind();
    that.$deliver_times = $('#deliver_times>select').iPathDataBind();
    that.$restaurant_info = $('#sheet3').iPathDataBind();
    that.$restaurant_level_panel = $('.restaurant_level_panel').iPathDataBind();

    $('.headtools>ul').iPathTabSheet();
    $('#btnChoosed').click(function () {
        that.ClickChoosed($(this));
    });
    $('#ordercount').parent().click(function () {
        that.ClickShoppingCart();
    });
}
FoodList.fn.Show = function (restaurant_id, agent_fee, restaurant_name, deliver_amount, image_url) {
    var that = this;

    //$('body>div').hide();
    //$('#foodlist').show();
    foodHadnler.showpanel('foodlist');

    if (restaurant_id == undefined) {
        return;
    }

    that.$foodtype.Clear();
    that.$foodlist.Clear();

    $('#foodlist .foottools .orders>span:eq(1)').text('配送费' + agent_fee + '元');
    $('#foodlist .foottools .orders>span:eq(1)').attr('agent_fee', agent_fee);
    $('#foodlist .foottools .orders>span:eq(1)').attr('deliver_amount', deliver_amount);
    $('#submitorder .body .foodinfo .supplier>span:eq(0)').text(restaurant_name);

    // 刷新餐厅食物
    iPath.Post(contextUri + '/Food/LoadRestaurantMenu',
        {
            restaurant_id: restaurant_id
        }, function (d) {
            if (d.state == 1) {
                var data = d.rows;
                that.$foodtype.DataBind(data);
                that.$foodlist.DataBind(data);
                $('.foodgroup>ul').iPathTabSheet();

                $('.foodlist>ul').each(function () {
                    var uHtml = $(this).html();
                    for (; uHtml.indexOf('#{') > -1 ;) {
                        uHtml = uHtml.replace('#{', '${').replace('}#', '}$');
                    }
                    $(this).html(uHtml);
                });
                for (var i in data) {
                    var item = data[i];
                    $('#hot' + item.id).iPathDataBind().DataBind(item.foods);
                }

                that.ListenFoodItemAddBtn();
            }
        }, 'json');

    // 获取餐厅详细信息
    iPath.Post(contextUri + '/Food/FindRestaurantInfo', { restaurant_id: restaurant_id }, function (d) {
        if (d.state != 1) {
            alert('出错了');
        }
        // 送达时间
        var deliver_times = Array();
        deliver_times.push({ id: '', name: '立即送达' });
        if (d.data.is_bookable == 1) {
            // 支持预定
            for (var i in d.data.deliver_times) {
                var dt = d.data.deliver_times[i];
                deliver_times.push({ id: dt, name: dt });
            }
        }
        that.$deliver_times.DataBind(deliver_times);

        if (d.data.promotion_info == undefined || d.data.promotion_info == '') {
            d.data.promotion_info = '餐厅暂无公告';
        }

        var restaurant_info_datas = new Array();
        restaurant_info_datas.push(d.data);
        that.$restaurant_info.DataBind(restaurant_info_datas);
        that.$restaurant_level_panel.DataBind(restaurant_info_datas);
        iPath.Star();

    }, 'json');

    // 获取餐厅评价
    iPath.Post(contextUri + '/Food/LoadRestaurantRating', { restaurant_id: restaurant_id }, function (d) {
        if (d.state != 1) {
            alert('出错了');
        }

        //iPath.Star();

    }, 'json');
}
// 监听增减餐品按钮
FoodList.fn.ListenFoodItemAddBtn = function () {
    var that = this;
    $('.price .btntools>span:nth-child(1)').bind('click', function () {
        var _count = Number($(this).next().text());
        if (_count < 1) {
            return;
        }
        _count--;
        $(this).next().text(_count);
        $(this).next().click();
    });
    $('.price .btntools>span:nth-child(2)').bind('click', function () {
        var _count = $(this).text();
        if (_count < 1) {
            _count = 0;
            $(this).hide();
            $(this).prev().hide();
        } else {
            $(this).show();
            $(this).prev().show();
        }
        that.SumFoodMoney();
    });
    $('.price .btntools>span:nth-child(2)').click();
    $('.price .btntools>span:nth-child(3)').bind('click', function () {
        var _count = Number($(this).prev().text());
        _count++;
        $(this).prev().text(_count);
        $(this).prev().click();
    });
}
FoodList.fn.SumFoodMoney = function () {
    var totalmoney = 0;
    var countorder = 0;
    $('.price .btntools').each(function () {
        var _count = Number($(this).find('span:eq(1)').text());
        if (_count > 0) {
            var price = $(this).parent().attr('price');
            price = Number(price) * 100;
            totalmoney += _count * price;
            countorder += _count;
        }

    });
    $('#totalmoney').text('￥' + toTowFloat(totalmoney / 100));
    $('#ordercount').text(countorder);
    if (countorder > 0) {
        $('#ordercount').show();
    } else {
        $('#ordercount').hide();
    }
}
// 获取当前店面订餐数据
FoodList.fn.GetSelectFoods = function () {
    var data = new Array();
    $('.price .btntools>span:nth-child(2)').each(function () {
        var $span = $(this);
        var food = {};
        food.count = Number($span.text());
        if (food.count > 0) {
            food.price = $span.parent().parent().attr('price');
            food.name = $span.parent().parent().parent().children(":first").text();
            food.id = $span.parents('li:eq(0)').attr('_id');
            data.push(food);
        }
    });
    return data;
}
// 点击购物车
FoodList.fn.ClickShoppingCart = function (jDom) {
    var that = this;
    if ($('.shoppingcart').is(':visible')) {
        // 当前显示，应隐藏
        $('.shoppingcart').hide();
    } else {
        // 当前隐藏，应显示
        var data = that.GetSelectFoods();
        that.$hoppingcart.DataBind(data);
        if (data.length < 1) {
            return;
        }
        if (data.length > 2) {
            $('#foodlist .shoppingcart .carpanel').css('height', '200px');
        } else {
            $('#foodlist .shoppingcart .carpanel').css('height', (data.length + 1) * 50 + 'px');
        }

        that.ListenShoppingCart();
        $('.shoppingcart').show();
    }
}
// 监听购物车按钮事件
FoodList.fn.ListenShoppingCart = function () {
    var that = this;
    $('.shoppingcart .btntools>span:nth-child(1)').bind('click', function () {
        var _count = Number($(this).next().text());
        if (_count < 1) {
            return;
        }
        _count--;
        $(this).next().text(_count);
        $(this).next().click();
    });
    $('.shoppingcart .btntools>span:nth-child(2)').bind('click', function () {
        var _count = $(this).text();
        var id = $(this).parents('li:eq(0)').attr('_id');
        var _$span = $('.foodlist li[_id="' + id + '"] .price .btntools>span:nth-child(2)');
        _$span.text(_count);
        _$span.click();

        var price = $(this).parent().prev().attr('price') * 100;
        $(this).parent().prev().text('￥' + toTowFloat((_count * price) / 100));

    });
    $('.shoppingcart .btntools>span:nth-child(3)').bind('click', function () {
        var _count = Number($(this).prev().text());
        _count++;
        $(this).prev().text(_count);
        $(this).prev().click();
    });
    $('#shoppingcart').parent().children(':first').find('span:eq(1)').unbind('click');
    $('#shoppingcart').parent().children(':first').find('span:eq(1)').bind('click', function () {
        if (confirm('确认清空吗')) {
            $('.price .btntools>span:nth-child(2)').text(0);
            $('.price .btntools>span:nth-child(2)').click();
            $('.shoppingcart').hide();
        }
    });
}
// 选好了按钮被点击
FoodList.fn.ClickChoosed = function (jDom) {
    var data = globalCache.foodlist.GetSelectFoods();
    if (data == undefined || data == null
        || data.length < 1) {
        alert('请先选择餐品');
        return;
    }

    var deliver_amount = $('#foodlist .foottools .orders>span:eq(1)').attr('deliver_amount');
    deliver_amount = Number(deliver_amount);

    var totalmoney = $('#totalmoney').text().substr(1);
    totalmoney = Number(totalmoney);

    if (deliver_amount > totalmoney) {
        alert('您选择的餐品总价低于起送价[' + deliver_amount + '元]不能下单,请继续选择');
        return;
    }

    globalCache.route.Navigation('/Food/Cart', {}, '');
}