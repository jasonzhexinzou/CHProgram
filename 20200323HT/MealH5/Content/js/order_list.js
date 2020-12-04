
var lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
var noMore = false;

//function get_url() {
//    var src = window.location.href;
//    var index = src.indexOf("?state=") + 7;
//    var str = src.substring(index);
//    return str;
//}

// 载入当前登录人订单列表数据
function loadOrderList(callback) {

    var year = $("#years").val();
    var month = $("#months").val();

    // 当前登录页需可以看到的订单状态
    var state = (function () {
        if (fromuri == '1') {
            return "1,2,3,4,5,6,7,8,9,10,11,12";
        }
        if (fromuri == '2') {
            return "4,12";
        }
        if (fromuri == '3') {
            return "6,7,8,9";
        }
        if (fromuri == '4') {
            return "6,7";
        }
    })();

    post('/P/Order/Load',
        {
            end: lastTime,
            state: state,
            year: year,
            month: month == "All" ? 0 : month
        },
        function (d) {
            if (d.rows.length == 0) {
                if (noMore) {
                    showTopMsg('已无更多数据！');
                }

            }
            else {
                for (var i in d.rows) {
                    var item = d.rows[i];
                    var nowTime = getTimeNow().getTime();
                    var deliverTime = Number(item.DeliverTime.replace(/\/Date\((\d+)\)\//, "$1"));
                    if ((deliverTime + oneHours) <= nowTime) {
                        item.FoodLost = 1;
                    }
                    else {
                        item.FoodLost = 0;
                    }
                    if (deliverTime <= nowTime) {
                        item.FoodConfirm = 1;
                    }
                    else {
                        item.FoodConfirm = 0;
                    }

                    item.DeliverTime = getDateByDotNet(item.DeliverTime).pattern('yyyy-MM-dd HH:mm');
                    item.StateName = getStateName(item);
                }

                var html = $(render(d));
                html.click(function () {
                    var id = $(this).attr('_id');
                    var supplier = $(this).attr('_supplier');
                    var code = $(this).attr('_code');
                    var _code = code.substring(0, 2);
                    if (_code == 'HT') {
                        location.href = contextUri + '/P/Order/Details/' + id + '?fromuri=' + fromuri + '&supplier=' + supplier;
                    }
                    else {
                        location.href = contextUri + '/P/Order/OrderDetails/' + id + '?fromuri=' + fromuri + '&supplier=' + supplier;
                    }
                });
                $('#orderlist').append(html);
                var height = window.screen.height;
                var _height = document.getElementById("orderlist").offsetHeight;
                if (_height < height) {
                    noMore = false;
                }
                else {
                    noMore = true;
                }

                lastTime = d.rows[d.rows.length - 1].CreateDate;
                lastTime = getDateByDotNet(lastTime).pattern('yyyy-MM-dd HH:mm:ss');
            }
            callback(d.rows.length);
        }, 'json');
}

// 取得订单状态的中文表示法
function getStateName(order) {
    var stateName = '';
    switch (order.State) {
        case 1:
            stateName = '订单待审批';
            break;
        case 2:
            stateName = '订单审批被驳回';
            break;
        case 3:
            stateName = '订单提交成功';
            break;
        case 4:
            stateName = '订单预订成功';
            break;
        case 5:
            stateName = '订单预订失败';
            break;
        case 6:
            stateName = '订单已收餐';
            break;
        case 7:
            stateName = '订单系统已收餐';
            break;
        case 8:
            stateName = '订单未送达';
            break;
        case 9:
            stateName = '订单已评价';
            break;
        case 10:
            stateName = '订单申请退单';
            break;
        case 11:
            stateName = '订单退单成功';
            break;
        case 12:
            stateName = '订单退单失败';
            break;
    }

    //if (order.IsChange == 1) {
    //    stateName += '(改单中)';
    //}
    return stateName;
}

var render;
$(function () {
    render = template('tmpl_orderlist');
    $('#orderlist2').dropload({
        scrollArea: window,
        loadDownFn: function (me) {
            showLoadingToast();
            loadOrderList(function (loadCount) {
                if (loadCount > 0) {
                    me.resetload();

                } else {
                    me.lock();
                    me.noData();
                }
            });
        }
    });

    $("#years").change(function () {
        var year = $("#years").val();
        if (year == yearNow) {
            document.getElementById("months").innerHTML = "";
            $.each(listmonth, function (i, n) {
                var _content = "<option value='" + n + "'>" + n + "</option>";
                $("#months").append(_content);
            });
            document.getElementById("months").value = 1;
        }
        else {
            document.getElementById("months").innerHTML = "";
            $.each(months, function (i, n) {
                var _content = "<option value='" + n + "'>" + n + "</option>";
                $("#months").append(_content);
            });
            document.getElementById("months").value = 1;
        }

        document.getElementById("orderlist").innerHTML = "";
        lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
        loadOrders();
    });

    $("#months").change(function () {
        document.getElementById("orderlist").innerHTML = "";
        lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
        loadOrders();
    });

});

function loadOrders() {
    $('#orderlist2').dropload({
        scrollArea: window,
        loadDownFn: function (me) {
            showLoadingToast();
            loadOrderList(function (loadCount) {
                console.log(loadCount);
                if (loadCount > 0) {
                    me.resetload();

                } else {
                    me.lock();
                    me.noData();
                }
            });
        }
    });
}

function btntool2(orderId, resId, htCode, supplier) {
    // 未送达
    location.href = contextUri + '/P/Order/Undelivered/' + orderId + '?resId=' + resId + '&supplier=' + supplier + '&htCode=' + htCode;
}
function btntool4(orderId, resId) {
    // 评价订单
    location.href = contextUri + '/P/Order/Evaluate/' + orderId + '?resId=' + resId;
}

function footbar1(orderId, htCode, supplier, totalPrice) {

    event.stopPropagation();
    // 确认收餐
    //showConfim('确认收餐', '您确定已经收到餐品了吗？', function () {
    //    post('/P/Order/ConfirmOrder', { id: orderId, supplier: supplier },
    //        function (d) {
    //            showConfim('收餐成功', '', function () {
    //                location.href = contextUri + '/P/Order/Evaluate/' + orderId + '?resId=' + resId + '&supplier=' + supplier;
    //            }, '评价投诉', '返回', function () {
    //                location.reload();
    //            });
    //        }, 'json');
    //});
    post('/P/Order/LoadOrderInfo', { id: orderId },
        function (d) {
            orderInfo = d.data;
            var json = d.data.Detail;
            var _orderInfo = JSON.parse(json);
            var price;
            if (totalPrice > -1 && totalPrice != _orderInfo.foods.allPrice) {
                price = totalPrice;
            } else {
                price = _orderInfo.foods.allPrice;
            }
            var count = _orderInfo.details.attendCount;
            // 确认收餐
            showConfim('确认收餐', '您确认已经收到餐品了吗？', function () {
                location.href = contextUri + '/P/Order/Confirm/' + orderId + '?supplier=' + supplier + '&price=' + price + '&count=' + count + '&htCode=' + htCode;
            });
        });
}