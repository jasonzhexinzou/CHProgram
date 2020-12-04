function Submitorder() { }
Submitorder.fn = Submitorder.prototype;
Submitorder.fn.$orderdetails = undefined;
Submitorder.fn.$orderother = undefined;
Submitorder.fn.cart = undefined;
Submitorder.fn.Init = function () {
    var that = this;
    that.$orderdetails = $('.foodinfo .details').iPathDataBind();
    that.$orderother = $('.foodinfo .other').iPathDataBind();

    $('#btnSubmit').click(function () {
        that.ClickSubmit($(this));
    });

    $('#username').text('申请人：' + globalCache.currentuser.name);
    $('#phone').val(globalCache.currentuser.phone);

    $('#_description').click(function () {
        that.ShowInputDialog('请输入备注', function () {
            var des = $('#_description').text();
            if (des != '(选填)可输入特殊要求') {
                $('#description').val(des);
            }
        }, function () {
            var des = $('#description').val();
            if (des != '') {
                $('#_description').text(des);
            } else {
                $('#_description').text('(选填)可输入特殊要求');
            }
        });
    });

    $('#eat_description').click(function () {
        that.ShowInputDialog('请输入用餐说明', function () {
            var des = $('#eat_description').text();
            if (des != '(选填)') {
                $('#description').val(des);
            }
        }, function () {
            var des = $('#description').val();
            if (des != '') {
                $('#eat_description').text(des);
            } else {
                $('#eat_description').text('(选填)');
            }
        });
    });

}
Submitorder.fn.Show = function () {
    var that = this;
    var data = globalCache.foodlist.GetSelectFoods();
    var food = iPath.Select(data, function (a) {
        return { id: a.id.substr(4), quantity: a.count }
    });
    var postdata = {
        phone: $('#phone').val(),
        longitude: globalCache._geo._lon,
        latitude: globalCache._geo._lat,
        food: food
    };
    iPath.Post(contextUri + '/Food/CreateCart',
        mvcParamMatch(postdata),
        function (d) {
            if (d.state != 1) {
                alert(d.txt);
                return;
            }
            that.$orderdetails.DataBind(d.data.foods);
            that.$orderother.DataBind(d.data.extras);
            var total = d.data.total;

            $('#confirmtotalmoney').text('￥' + total);


            foodHadnler.showpanel('submitorder');
            that.cart = d.data;
        }, 'json');

}
// 显示输入用对话框
Submitorder.fn.ShowInputDialog = function (title, onshow_fn, onhide_fn) {
    $('#inputdialog>div:first').text(title);
    $('#description').val('');
    onshow_fn();

    $('#inputdialog').show();
    $('#description').focus();
    $('#description').selectRange($('#description').val().length, $('#description').val().length);

    $('#inputdialog>div:last').unbind('click');
    $('#inputdialog>div:last').bind('click', function () {
        onhide_fn();
        $('#inputdialog').hide();
    });
}

// 确认并提交订单
Submitorder.fn.ClickSubmit = function (jDom) {
    var that = this;

    var data = globalCache.foodlist.GetSelectFoods();
    var food = iPath.Select(data, function (a) {
        return { id: a.id.substr(4), quantity: a.count }
    });

    var postdata = {
        phones: $('#phone').val(),
        consignee: globalCache.currentuser.name,
        address: $('#hospitalname').text() + $('#address').val(),
        cart_id: that.cart.id,
        total: that.cart.total,
        longitude: globalCache._geo._lon,
        latitude: globalCache._geo._lat,
        ip: '127.0.0.1',
        description: $('#_description').text(),
        invoice: '上海途径信息技术有限公司',
        costcenter: $('#costcenter').val(),
        meetingcenter: $('#meetingcenter').val(),
        eat_description: $('#eat_description').text(),
        restaurant_image_url: $('#restaurant>li.selected').attr('_image_url'),
        deliver_times: $('#deliver_times>select').val(),
        food: food
    };
    if (postdata.description.indexOf('(选填)可输入特殊要求') == 0) {
        postdata.description = '';
    }
    if (postdata.eat_description.indexOf('(选填)') == 0) {
        postdata.eat_description = '';
    }

    iPath.Post(contextUri + '/Food/Order', mvcParamMatch(postdata), function (d) {
        if (d.state != 1) {
            if (d.txt == undefined || d.txt == '' || d.txt == null) {
                alert('下单接口错误');
            }
            else {
                alert(d.txt);
            }
            return;
        }

        if (d.data.error_code == 0) {
            alert('下单成功');
            WeixinJSBridge.call('closeWindow');
        } else {
            alert(d.data.error_msg);
        }

    }, 'json');


}
