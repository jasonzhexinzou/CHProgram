var dataState = "";
// 订单数据渲染完毕事件
onOrderLoadSuccess = function () {
    showFootBar();
}

function showFootBar() {
    window.sessionStorage.setItem("State", orderInfo.State);
    dataState = window.sessionStorage.getItem("State");
    $('#action_buttons p').addClass('weui-tabbar__label_disabled');
    if (fromuri != '1') {
        $('#action_buttons a').hide();
    }
    if (orderInfo.State == 2) {
        showChangeOrder();
    }
    else if (orderInfo.State == 3 && orderInfo.IsChange != 1) {
        showCancelOrder();
    }
    else if (orderInfo.State == 6 || orderInfo.State == 7) {
        showFootButton(4);
    }
    else if (orderInfo.State == 4) {
        //for (var i = 0; i < 4; i++) {
        //    showFootButton(i);
        //}
        showFood();
        showChangeOrder();
        showCancelOrder();
        showFoodLost();

    } else if (orderInfo.State == 12) {
        showFood();
        //showFoodLost();
    }


    $('#action_buttons').show();

    if (fromuri != '1') {
        $('#action_buttons a:hidden').remove();
    }

    console.log(orderInfo);
    $('#action_buttons .weui-tabbar__label_disabled').unbind('click');
    $('#action_buttons .weui-tabbar__label_disabled').attr('onclick', '');
}


// 显示确认收餐按钮
function showFood() {

    var nowTime = getTimeNow().getTime();
    var deliverTime = orderInfo.DeliverTime.getTime();

    if (nowTime <= deliverTime) {
        // 非可以点击确认收餐时间
        return;
    }

    showFootButton(0);
}

// 显示改单按钮
function showChangeOrder() {
    var nowTime = getTimeNow().getTime();
    var deliverTime = orderInfo.DeliverTime.getTime() - oneHours;

    if (!isInTimespan(getTimeNow(), timeConfig._workBegin, timeConfig._workEnd)) {
        // 非工作时间段内不能改单
        return;
    }

    if (nowTime >= deliverTime) {
        // 不可改单
        return;
    }

    if (preApproval.State != "5" && preApproval.State != "6" && preApproval.State != "9") {
        return;
    }


    if (orderInfo.IsRetuen == 1) {
        //申请退订状态不可改单
        return;
    }


    showFootButton(1);
}

// 显示退单按钮
function showCancelOrder() {
    //var deliverTime = orderInfo.DeliverTime.toString('HH:mm:ss');

    if (!isInTimespan(getTimeNow(), timeConfig._workBegin, timeConfig._workEnd)) {
        // 非工作时间段内不能退单
        return;
    }

    var nowTime = getTimeNow().getTime();
    var deliverTime = orderInfo.DeliverTime.getTime() + 24 * oneHours;


    if (nowTime >= deliverTime) {
        // 非可以退单时间
        return;
    }

    //if (orderInfo.IsChange == 1) {
    //    //申请改单状态不可退单
    //    return;
    //}

    showFootButton(2);
}

// 显示未收餐
function showFoodLost() {

    var nowTime = getTimeNow().getTime();
    var deliverTime = orderInfo.DeliverTime.getTime() + oneHours;

    if (nowTime <= deliverTime) {
        // 非可以点击未收餐时间
        return;
    }

    showFootButton(3);
}

function showFootButton(i) {
    $('#action_buttons>a:eq(' + i + ') p').removeClass('weui-tabbar__label_disabled');
    $('#action_buttons>a:eq(' + i + ')').show();
}


function footbar1() {
    var price;
    if (orderInfo.XmsTotalPrice > -1 && orderInfo.XmsTotalPrice != orderInfo.Detail.foods.allPrice) {
        price = orderInfo.XmsTotalPrice;
    } else {
        price = orderInfo.Detail.foods.allPrice;
    }
    var count = orderInfo.Detail.details.attendCount;
    var htCode = orderInfo.CN;
    // 确认收餐
    showConfim('确认收餐', '您确认已经收到餐品了吗？', function () {
        location.href = _contextUri + '/P/Order/Confirm/' + orderId + '?supplier=' + supplier + '&price=' + price + '&count=' + count + '&htCode=' + htCode;

    });
}
function footbar2() {
    // 修改订单
    var nowTime = getTimeNow().getTime();
    var deliverTime = orderInfo.DeliverTime.getTime();
    if (nowTime >= (deliverTime - oneHours)) {
        // 不可改单
        showDlg('距离送餐时间已经不足一小时，本单已经无法修改', '确认', function () {
            location.reload();
        });
        return;
    }

    var createDay = orderInfo.CreateDate.pattern('yyyy-MM-dd');
    var deliverDay = orderInfo.DeliverTime.pattern('yyyy-MM-dd');

    if (createDay == deliverDay) {
        $('#changeOrder1').show();
        $('#changeOrder2').show();
        $('#changeOrder3').show();
        $('#changeOrder5').show();
    }
    else {
        // 根据时间判断可以修改的项目
        if (nowTime < (deliverTime - 4 * oneHours)) {
            $('#changeOrder1').show();
            $('#changeOrder2').show();
            $('#changeOrder3').show();
            $('#changeOrder5').show();
        } else {
            $('#changeOrder2').show();
            $('#changeOrder3').show();
            $('#changeOrder5').show();
        }

    }
    //if (orderInfo.State == 2) {
    //    $('#changeOrder4').show();
    //}

    $('#changeStep').show();
}
function footbar3() {
    //退单
    showConfim('确认退单', '您确认取消该笔订单吗？', function () {
        post('/P/Order/Refunds', { id: orderId, supplier: supplier },
            function (d) {
                showDlg('您的订单退单请求已提交成功，正在等待餐厅确认。', '确定', function () {
                    location.reload();
                }, 'success');
            }, 'json');
    });
}
function footbar4() {
    // 未送达
    location.href = _contextUri + '/P/Order/Undelivered/' + orderId + '?resId=' + orderInfo.RestaurantId + '&supplier=' + supplier + '&htCode=' + orderInfo.CN;
}
function footbar5() {
    // 评价订单
    location.href = _contextUri + '/P/Order/Evaluate/' + orderId + '?resId=' + orderInfo.RestaurantId;
}


function beginChangeOrder(fn) {
    post('/P/Order/BeginChangeOrder', { id: orderId },
        function (d) {
            fn();
        });
}
function changeOrder1() {
    beginChangeOrder(function () {
        location.href = contextUri + '/P/Restaurant/List?formenu=1' + '&supplier=' + supplier + '&changeOrder=1';
    });
}
function changeOrder2() {
    beginChangeOrder(function () {
        location.href = contextUri + '/P/Restaurant/Menu?supplier=' + supplier + "&State=" + dataState;
    });
}
function changeOrder3() {
    beginChangeOrder(function () {
        location.href = contextUri + '/P/Food/Order?supplier=' + supplier + "&state=" + dataState;
    });
}
function changeOrder4() {
    beginChangeOrder(function () {
        location.href = contextUri + '/P/Food/MMCoE?supplier=' + supplier;
    });
}
function changeOrder5() {
    beginChangeOrder(function () {
        location.href = contextUri + '/P/Restaurant/SendTime?supplier=' + supplier + '&changeSendTime=1&formenu=1&orderCreateDate=' + orderInfo.CreateDate;
    });
}