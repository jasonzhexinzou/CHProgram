var render;
var renderdelivertime;
var nowOrderInfo;
var restaurant;
var isChangeOrder = true;

// 获取订单详细信息
function getOrderDetail() {
    return {
        attendCount: $('#attendCount').val(),
        deliveryAddress: $('#deliveryAddress').val(),
        consignee: $('#consignee').val(),
        phone: $('#phone').val(),
        deliverTime: $('#deliverTime').val(),
        remark: $('#remark').val()
    }
}

// 隐藏表单警告消息
function hidecell_warn(inputId) {
    $('#' + inputId).closest('.weui-cell').removeClass('weui-cell_warn');
    $('#' + inputId).closest('.weui-cell').find('weui-cell__ft').hide();
}

// 显示表单警告消息
function showcell_warn(inputId) {
    $('#' + inputId).closest('.weui-cell').addClass('weui-cell_warn');
    $('#' + inputId).closest('.weui-cell').find('.weui-cell__ft').show();
}

// 校验订单数据
function checkOrderDetail(orderDetail) {
    var addressInfo = $('#deliveryAddress').val();  //送餐详细地址
    if (addressInfo == undefined || addressInfo == '') {
        showTopMsg(MSG_DELIVERYADDRESS);
        return false;
    }

    hidecell_warn('attendCount');
    if (orderDetail.attendCount * 1 < 1) {
        showcell_warn('attendCount');
        showTopMsg(MSG_NEEDATTENDCOUNT);
        return;
    }
    if (orderDetail.attendCount * 1 > nowOrderInfo.preApproval.AttendCount * 1) {
        showcell_warn('attendCount');
        showTopMsg(MSG_OUTATTENDCOUNT);
        return;
    }

    hidecell_warn('consignee');
    if (orderDetail.consignee == '') {
        showcell_warn('consignee');
        showTopMsg(MSG_NEEDCONSIGNEE);
        return;
    }

    hidecell_warn('phone');
    if (orderDetail.phone == '' || orderDetail.phone.length != 11) {
        showcell_warn('phone');
        showTopMsg(MSG_NEEDPHONE);
        return;
    }

    hidecell_warn('deliverTime');
    if (orderDetail.deliverTime == '' || orderDetail.phone.deliverTime < 11) {
        showcell_warn('deliverTime');
        showTopMsg(MSG_NEEDDELIVERTIME);
        return;
    }

    var deliverRange = getDateByDotNet(nowOrderInfo.preApproval.MeetingDate).pattern('yyyy-MM-dd');
    var timeNow = getTimeNow();
    var dateNow = timeNow.pattern('yyyy-MM-dd');

    if (isChangeOrder) {
        if (!isInTimespan(getTimeNow(), timeConfig._workBegin, timeAdd(timeConfig._workEnd, timeConfig.cachetime))) {
            showDlg(MSG_NOORDERTIME);
            return false;
        }

        ////会议当天下单
        //if (dateNow == deliverRange) {
        //    if (!isInTimespan(getTimeNow(), timeConfig.todayWorkBegin, timeAdd(timeConfig.todayWorkEnd, timeConfig.cachetime))) {
        //        showDlg(MSG_ORDERTIMEFAIL);
        //        return false;
        //    }
        //}
        //else {
        //    if (!isInTimespan(getTimeNow(), timeConfig._workBegin, timeAdd(timeConfig._workEnd, timeConfig.cachetime))) {
        //        showDlg(MSG_NOORDERTIME);
        //        return false;
        //    }
        //}
    }

    if (!isChangeOrder) {
        //会议当天下单
        if (dateNow == deliverRange) {
            if (!isInTimespan(getTimeNow(), timeConfig.todayWorkBegin, timeAdd(timeConfig.todayWorkEnd, timeConfig.cachetime))) {
                showDlg(MSG_ORDERTIMEFAIL);
                return false;
            }
        }
        else {
            if (!isInTimespan(getTimeNow(), timeConfig.workBegin, timeAdd(timeConfig.workEnd, timeConfig.cachetime))) {
                showDlg(MSG_NOWORKINGTIME);
                return false;
            }
        }
    }

    orderDetail.attendCount = orderDetail.attendCount * 1;
    //if ((nowOrderInfo.foods.allPrice / orderDetail.attendCount) > 60) {
    //    showTopMsg(MSG_MONEYOVERPROOF);
    //    return false;
    //}

    //是否超出人均标准
    var _isSubmit = true;
    var showMess = '';
    $.ajax({
        async: false,
        type: "post",
        url: contextUri + '/P/Food/IsSubmit',
        data: {
            hospitalId: nowOrderInfo.preApproval.HospitalCode,
            budget: nowOrderInfo.foods.allPrice,
            attendance: orderDetail.attendCount,
            state: 2
        },
        datatype: 'json',
        success: function (data) {
            if (data.data == false) {
                _isSubmit = false;
                showMess = data.txt
            }
        }
    });

    if (!_isSubmit) {
        showTopMsg(showMess);
        return false;
    }



    if (orderDetail.attendCount >= 300) {
        showTopMsg("您的订单用餐人数>=300人，请修改订单");
        return false;
    }

    //var _isSubmit = IsSubmit(nowOrderInfo.preApproval.HospitalCode, nowOrderInfo.foods.allPrice, orderDetail.attendCount);
    //if (_isSubmit == false) {
    //    showTopMsg(MSG_MONEYOVERPROOF);
    //    return false;
    //}

    if (nowOrderInfo.details == undefined || nowOrderInfo.details == null) {
        if (!checkDeliverTime(orderDetail.deliverTime, timeConfig.todayWorkBegin, timeConfig.todayWorkEnd)) {
            showDlg(MSG_ORDERTIMEFAIL);
            return false;
        }
    }
    return true;

}

// 校验配送时间
function checkDeliverTime(deliverTime, todayWorkBegin, todayWorkEnd) {
    var today = getTimeNow().pattern('yyyy-MM-dd');
    var targetday = deliverTime.substring(0, 10);
    if (today == targetday) {
        // 目标订餐在今天
        if (isInTimespan(getTimeNow(), todayWorkBegin, timeAdd(todayWorkEnd, timeConfig.cachetime))) {
            // 点击时候时间在上午
            return true;
        } else {
            return false;
        }

    } else {
        return true;
    }
}

// 判断是否在餐厅工作时间
function isInResWoringTime(time) {
    if (restaurant.businessHour != undefined) {
        if (isTimeInTimespan(time, restaurant.businessHour.start, restaurant.businessHour.end)) {
            return true;
        }
    } else {
        // 全天候配送
        return true;
    }
    if (restaurant.eveningHour != undefined) {
        if (isTimeInTimespan(time, restaurant.eveningHour.start, restaurant.eveningHour.end)) {
            return true;
        }
    }
    return false;
}



// 显示可选订餐时间
function showDelivertime() {

    var deliverTime = new Array();

    var deliverRange = getDateByDotNet(nowOrderInfo.preApproval.MeetingDate).pattern('yyyy-MM-dd');

    var timeNow = getTimeNow();

    var dateNow = timeNow.pattern('yyyy-MM-dd');

    //会议当天进行订单操作
    if (dateNow == deliverRange) {
        if (nowOrderInfo.details != undefined && nowOrderInfo.details != null) {
            var create_time = $('#createTime').val();
            console.log(create_time);
            var _create_time = create_time.substring(0, 10);
            var _newDate = getTimeNow();

            var HH = _newDate.getHours() + 1;

            var mm = _newDate.getMinutes();
            if (mm > 45) {
                mm = 0;
                HH = HH + 1;
            }
            else if (mm > 30) {
                mm = 45;
            }
            else if (mm > 15) {
                mm = 30;
            } else {
                mm = 15;
            }

            var _time = HH + ':' + mm + ':' + '00';

            if (mm == 0) {
                _time = HH + ':' + mm + '0:' + '00';
            }
            var _weekday = new Date(deliverRange.replace(/-/g, '/')).pattern('E');
            if (_create_time == dateNow) {
                timeConfig.sendBegin = timeConfig.todaySendBegin;
                timeConfig.sendEnd = timeConfig.todaySendEnd;

                if (HH < 12) {
                    _time = timeConfig.todaySendBegin;
                }
                else if (HH == 12) {
                    if (mm < 30) {
                        _time = timeConfig.todaySendBegin;
                    }
                }
            }

            for (; isTimeInTimespan(_time, timeConfig.sendBegin, timeConfig.sendEnd) ;) {
                if (isInResWoringTime(_time)) {
                    deliverTime.push({
                        time: deliverRange + ' ' + _time,
                        weekday: _weekday
                    });
                }
                _time = timeAdd(_time, timeConfig.cachetime);
            }
        }
        else {
            var _time = timeConfig.todaySendBegin;
            var _weekday = getTimeNow().pattern('E');
            for (; isTimeInTimespan(_time, timeConfig.todaySendBegin, timeConfig.todaySendEnd) ;) {
                if (isInResWoringTime(_time)) {
                    deliverTime.push({
                        time: deliverRange + ' ' + _time,
                        weekday: _weekday
                    });
                }
                _time = timeAdd(_time, timeConfig.cachetime);
            }
        }
    }
    else {
        var _time = timeConfig.sendBegin;
        var _weekday = new Date(deliverRange.replace(/-/g, '/')).pattern('E');
        for (; isTimeInTimespan(_time, timeConfig.sendBegin, timeConfig.sendEnd) ;) {
            if (isInResWoringTime(_time)) {
                deliverTime.push({
                    time: deliverRange + ' ' + _time,
                    weekday: _weekday
                });
            }

            _time = timeAdd(_time, timeConfig.cachetime);
        }
    }

    if (nowOrderInfo.details != undefined && nowOrderInfo.details != null) {
        isChangeOrder = true;
    } else {
        isChangeOrder = false;
    }

    deliverTimePicker.init('chooseDeliverTime', deliverTime);

}



// 显示可选订餐时间范围
function showDeliverTime() {
    // 调用后台接口获取可订餐时间范围

    post('/P/Food/LoadNextHoliday', {},
        function (d) {
            var holiday = d.data.holiday;
            holiday.StartDay = getDateByDotNet(holiday.StartDay);
            holiday.EndDay = getDateByDotNet(holiday.EndDay);

            var startDay = getDateByDotNet(d.data.now);
            var endDay;
            // 判断今天在那个区间1.非最后一个工作日 2.最后一个工作日 3.休息日
            // 规则1.可以定到下一天 2、3.可以订到休假日后第一个工作日
            var rangeType = 0;
            if (holiday.StartDay.getTime() <= startDay.getTime() && startDay.getTime() <= holiday.EndDay.getTime()) {
                // 今天是休假日 判定为区间3
                rangeType = 3;
            } else {
                if ((startDay.getTime() + 24 * 60 * 60 * 1000) < holiday.StartDay.getTime()) {
                    // 明天也是工作日 判定为区间1
                    rangeType = 1;
                } else {
                    // 明天是休息日，今天是最后一个工作日 判定为区间2
                    rangeType = 2;
                }
            }

            var deliverRange = new Array();

            if (rangeType == 1) {
                deliverRange.push(startDay.pattern('yyyy-MM-dd'));
                deliverRange.push(new Date(startDay.getTime() + oneDay).pattern('yyyy-MM-dd'));
            } else {
                for (var i = startDay.getTime() ; i <= (holiday.EndDay.getTime() + oneDay) ; i += oneDay) {
                    deliverRange.push(new Date(i).pattern('yyyy-MM-dd'));
                }
            }

            var deliverTime = new Array();
            // 构造可选送餐时间点
            //修改订单
            if (nowOrderInfo.details != undefined && nowOrderInfo.details != null) {
                isChangeOrder = true;
                var create_time = $('#createTime').val();
                var deliver_time = $('#deliverTime').val();
                if (deliver_time == "") {
                    deliver_time = getDateByDotNet(nowOrderInfo.details.oldDeliverTime).pattern('yyyy-MM-dd HH:mm:ss');
                    create_time = getDateByDotNet(nowOrderInfo.details.createTime).pattern('yyyy-MM-dd HH:mm');
                }
                var _deliver_time = deliver_time.substring(0, 10);
                var _create_time = create_time.substring(0, 10);
                var today = getTimeNow().pattern('yyyy-MM-dd');
                //非送餐日修改
                if (_deliver_time != today) {
                    var _time = timeConfig.sendBegin;
                    var _weekday = new Date(_deliver_time.replace(/-/g, '/')).pattern('E');
                    for (; isTimeInTimespan(_time, timeConfig.sendBegin, timeConfig.sendEnd) ;) {
                        if (isInResWoringTime(_time)) {
                            deliverTime.push({
                                time: _deliver_time + ' ' + _time,
                                weekday: _weekday
                            });
                        }
                        _time = timeAdd(_time, timeConfig.cachetime);
                    }
                }
                    //送餐当天修改 
                else {
                    var _newDate = getTimeNow();

                    var HH = _newDate.getHours() + 1;

                    var mm = _newDate.getMinutes();
                    if (mm > 45) {
                        mm = 0;
                        HH = HH + 1;
                    }
                    else if (mm > 30) {
                        mm = 45;
                    }
                    else if (mm > 15) {
                        mm = 30;
                    } else {
                        mm = 15;
                    }

                    var _time = HH + ':' + mm + ':' + '00';

                    if (mm == 0) {
                        _time = HH + ':' + mm + '0:' + '00';
                    }
                    var _weekday = new Date(_deliver_time.replace(/-/g, '/')).pattern('E');
                    if (_create_time == today) {
                        timeConfig.sendBegin = timeConfig.todaySendBegin;
                        timeConfig.sendEnd = timeConfig.todaySendEnd;

                        if (HH < 12) {
                            _time = timeConfig.todaySendBegin;
                        }
                        else if (HH == 12) {
                            if (mm < 30) {
                                _time = timeConfig.todaySendBegin;
                            }
                        }
                    }

                    for (; isTimeInTimespan(_time, timeConfig.sendBegin, timeConfig.sendEnd) ;) {
                        if (isInResWoringTime(_time)) {
                            deliverTime.push({
                                time: _deliver_time + ' ' + _time,
                                weekday: _weekday
                            });
                        }
                        _time = timeAdd(_time, timeConfig.cachetime);
                    }
                }
            }

                //下新单
            else {
                isChangeOrder = false;
                // 规则 如果上午截止时间的缓冲期之前下单，仍然可以定当天下午的餐
                if (isInTimespan(getTimeNow(), timeConfig.todayWorkBegin, timeAdd(timeConfig.todayWorkEnd, timeConfig.cachetime))) {
                    // 现在是上午,可以定今天下午的餐
                    var _time = timeConfig.todaySendBegin;
                    var _weekday = getTimeNow().pattern('E');

                    for (; isTimeInTimespan(_time, timeConfig.todaySendBegin, timeConfig.todaySendEnd) ;) {
                        if (isInResWoringTime(_time)) {
                            deliverTime.push({
                                time: deliverRange[0] + ' ' + _time,
                                weekday: _weekday
                            });
                        }
                        _time = timeAdd(_time, timeConfig.cachetime);
                    }

                }

                // 任何时间都能定明天和更晚的餐
                for (var i = 1; i < deliverRange.length; i++) {
                    var _time = timeConfig.sendBegin;
                    var _weekday = new Date(deliverRange[i].replace(/-/g, '/')).pattern('E');
                    for (; isTimeInTimespan(_time, timeConfig.sendBegin, timeConfig.sendEnd) ;) {
                        if (isInResWoringTime(_time)) {
                            deliverTime.push({
                                time: deliverRange[i] + ' ' + _time,
                                weekday: _weekday
                            });
                        }

                        _time = timeAdd(_time, timeConfig.cachetime);
                    }
                }
            }

            deliverTimePicker.init('chooseDeliverTime', deliverTime);
        }, 'json');


    //$('#page_order').hide();
    //$('#chooseDeliverTime').hide();
}

var orderSuccess = false;



$(function () {
    dataState = window.sessionStorage.getItem("State");
    render = template('order_foods');
    renderdelivertime = template('tmpl_delivertime');

    post('/P/PreApproval/NowOrder', {}, function (d) {
        nowOrderInfo = d.data;
        if (d.data.details != null && sendTime == "") {
            sendTime = getDateByDotNet(d.data.details.deliverTime).pattern('yyyy-MM-dd HH:mm:ss');
        }
        if (nowOrderInfo.details != undefined && nowOrderInfo.details != null) {
            isChangeOrder = true;
        } else {
            isChangeOrder = false;
        }
        if (nowOrderInfo != null) {
            // 加载餐厅信息
            post('/P/Restaurant/FindRestaurant',
                {
                    hospitalId: nowOrderInfo.preApproval.HospitalAddressCode,
                    restaurantId: nowOrderInfo.foods.resId,
                    supplier: supplier,
                    dataState: dataState,
                    sendTime: sendTime
                }, function (d) {
                    restaurant = d.data;
                    if (restaurant.businessHourStart != 0) {
                        restaurant.businessHour = {
                            start: new Date(restaurant.businessHourStart - eightHours).pattern('HH:mm:ss'),
                            end: new Date(restaurant.businessHourEnd - eightHours).pattern('HH:mm:ss')
                        };
                    }

                    if (restaurant.eveningHourStart != 0 && restaurant.businessHourStart != restaurant.eveningHourStart) {
                        restaurant.eveningHour = {
                            start: new Date(restaurant.eveningHourStart - eightHours).pattern('HH:mm:ss'),
                            end: new Date(restaurant.eveningHourEnd - eightHours).pattern('HH:mm:ss')
                        };
                    }

                    //餐厅下架提示
                    if (d.data.errorCode == "8011" || d.data.errorCode == "8012") {
                        showDlg("您选择的餐厅" + nowOrderInfo.foods.resName + "已下线，请重新选择其他餐厅，或取消修改，原订单仍可正常配送。", '确定', function () {
                            WeixinJSBridge.call('closeWindow');
                        }, 'info');
                        return false;
                    }

                    //餐品下架提示
                    var nowFoods = new Array();
                    var allFoodList = new Array();
                    var removeFoodsName = new Array();
                    var flag = 0;
                    for (var i = 0; i < d.data.listMenu.length; i++) {
                        for (var j = 0; j < d.data.listMenu[i].listFood.length; j++) {
                            allFoodList.push(d.data.listMenu[i].listFood[j].foodId);
                        }
                    }

                    for (var k = 0; k < nowOrderInfo.foods.foods.length; k++) {
                        if (allFoodList.indexOf(nowOrderInfo.foods.foods[k].foodId) < 0) {
                            flag = 1;
                            removeFoodsName.push(nowOrderInfo.foods.foods[k].foodName);
                        } else {
                            nowFoods.push(nowOrderInfo.foods.foods[k]);
                        }
                    }
                    nowOrderInfo.foods.foods = nowFoods;
                    //餐品全部下架提示
                    if (nowOrderInfo.foods.foods.length == 0) {
                        var msg = '你选择的餐品全部下架，请联系订餐供应商。';
                        msg += '<br\>BDS：400-6868-912';
                        msg += '<br\>XMS：400-820-5577';
                        showDlg(msg, '确定', function () {
                            WeixinJSBridge.call('closeWindow');
                        }, 'info');
                        return false;
                    }
                    if (flag != 0) {
                        showDlg("您选择的餐品" + removeFoodsName.join(',') + "已下架，请重新选择其他餐品，或取消修改，原订单仍可正常配送。", '确定', function () {
                            // 重新计算费用
                            post('/P/Restaurant/CalculateFee', {
                                hospitalId: nowOrderInfo.preApproval.HospitalCode,
                                resId: nowOrderInfo.foods.resId,
                                foods: nowOrderInfo.foods.foods,
                                supplier: supplier,
                                dataState: dataState,
                                sendTime: sendTime
                            }, function (feeData) {
                                nowOrderInfo.foods.allPrice = feeData.data.allPrice;
                                nowOrderInfo.foods.foodFee = feeData.data.foodFee;
                                nowOrderInfo.foods.packageFee = feeData.data.packageFee;
                                nowOrderInfo.foods.sendFee = feeData.data.sendFee;
                                post('/P/Restaurant/SaveFood',
                                    {
                                        resId: nowOrderInfo.foods.resId,
                                        resName: nowOrderInfo.foods.resName,
                                        foods: nowOrderInfo.foods.foods,
                                        supplier: supplier,
                                        hospitalId: nowOrderInfo.preApproval.HospitalCode,
                                        sendTime: sendTime
                                    },
                                    function (d) {
                                    }, 'json');

                                var html = render(nowOrderInfo);
                                $('.foodinfo').html(html);
                                $('#attendCount').val(nowOrderInfo.preApproval.AttendCount);
                                $('#address').html(nowOrderInfo.preApproval.HospitalAddress);
                                $('#invoiceTitle').html(nowOrderInfo.invoiceTitle);
                                $('#dutyParagraph').html(nowOrderInfo.dutyParagraph);
                                $('#deliverTime').val(sendTime);
                                if (nowOrderInfo.details != undefined && nowOrderInfo.details != null) {
                                    $('#attendCount').val(nowOrderInfo.details.attendCount);
                                    $('#deliveryAddress').val(nowOrderInfo.details.deliveryAddress);
                                    $('#consignee').val(nowOrderInfo.details.consignee);
                                    $('#phone').val(nowOrderInfo.details.phone);
                                    if (sendTime == "")
                                        $('#deliverTime').val(getDateByDotNet(nowOrderInfo.details.deliverTime).pattern('yyyy-MM-dd HH:mm:ss'));
                                    $('#remark').val(nowOrderInfo.details.remark);
                                    $('#createTime').val(getDateByDotNet(nowOrderInfo.details.createTime).pattern('yyyy-MM-dd HH:mm'));
                                }
                            }, 'json');
                        }, 'info');
                    } else {
                        // 重新计算费用
                        post('/P/Restaurant/CalculateFee', {
                            hospitalId: nowOrderInfo.preApproval.HospitalCode,
                            resId: nowOrderInfo.foods.resId,
                            foods: nowOrderInfo.foods.foods,
                            supplier: supplier,
                            dataState: dataState,
                            sendTime: sendTime
                        }, function (feeData) {
                            nowOrderInfo.foods.allPrice = feeData.data.allPrice;
                            nowOrderInfo.foods.foodFee = feeData.data.foodFee;
                            nowOrderInfo.foods.packageFee = feeData.data.packageFee;
                            nowOrderInfo.foods.sendFee = feeData.data.sendFee;
                            post('/P/Restaurant/SaveFood',
                                {
                                    resId: nowOrderInfo.foods.resId,
                                    resName: nowOrderInfo.foods.resName,
                                    foods: nowOrderInfo.foods.foods,
                                    supplier: supplier,
                                    hospitalId: nowOrderInfo.preApproval.HospitalCode,
                                    sendTime: sendTime
                                },
                                function (d) {
                                }, 'json');

                            var html = render(nowOrderInfo);
                            $('.foodinfo').html(html);
                            $('#attendCount').val(nowOrderInfo.preApproval.AttendCount);
                            $('#address').html(nowOrderInfo.preApproval.HospitalAddress);
                            $('#invoiceTitle').html(nowOrderInfo.invoiceTitle);
                            $('#dutyParagraph').html(nowOrderInfo.dutyParagraph);
                            $('#deliverTime').val(sendTime);
                            if (nowOrderInfo.details != undefined && nowOrderInfo.details != null) {
                                $('#attendCount').val(nowOrderInfo.details.attendCount);
                                $('#deliveryAddress').val(nowOrderInfo.details.deliveryAddress);
                                $('#consignee').val(nowOrderInfo.details.consignee);
                                $('#phone').val(nowOrderInfo.details.phone);
                                if (sendTime == "")
                                    $('#deliverTime').val(getDateByDotNet(nowOrderInfo.details.deliverTime).pattern('yyyy-MM-dd HH:mm:ss'));
                                $('#remark').val(nowOrderInfo.details.remark);
                                $('#createTime').val(getDateByDotNet(nowOrderInfo.details.createTime).pattern('yyyy-MM-dd HH:mm'));
                            }
                        }, 'json');
                    }


                }, 'json');
        }

    }, 'json');

    $('#attendCount').bind('input propertychange', function () {
        var _attendCount = $(this).val();
        _attendCount = parseInt(_attendCount);
        _attendCount = isNaN(_attendCount) ? 0 : _attendCount;
        $(this).val(_attendCount);
        hidecell_warn('attendCount');
    });

    $('#phone').bind('input propertychange', function () {
        var phone = $(this).val();
        if (phone != '' && phone.length == 11) {
            hidecell_warn('phone');
        }


    });

    //$('#deliverTime').click(function () {
    //    showDelivertime();
    //});

    $('#btnSubmitOrder').click(function () {
        //判断餐厅是否被拉黑
        post('/P/Restaurant/IsLahei',
            {
                resId: nowOrderInfo.foods.resId,
                supplier: supplier,
                dataState: dataState
            },
            function (d) {
                if (d.state == 1) {
                    var postdata = getOrderDetail();
                    if (checkOrderDetail(postdata)) {
                        var budget = nowOrderInfo.foods.allPrice;
                        var attendance = postdata.attendCount;

                        if (attendance >= 60) {
                            showConfim('您的订单用餐人数>=60人', '', function () {
                                if (budget / attendance <= 10) {
                                    showConfim('您的订单人均<=10元', '', function () {
                                        if (isChangeOrder == true) {
                                            post('/P/Order/GetOrderInfoByHTCode',
                                                {
                                                    htcode: nowOrderInfo.preApproval.HTCode
                                                }, function (d) {
                                                    if (d.data != null) {
                                                        var oldOrderInfo = d.data;
                                                        var deliverDate = getDateByDotNet(oldOrderInfo.DeliverTime).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                        var createDate = getDateByDotNet(oldOrderInfo.CreateDate).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                        var deliverTime = getDateByDotNet(oldOrderInfo.DeliverTime).getTime();
                                                        var currentTimeNow = getTimeNow().getTime();
                                                        //修改餐品、详情
                                                        if (oldOrderInfo.RestaurantId == nowOrderInfo.foods.resId) {
                                                            if (currentTimeNow > deliverTime - oneHours) {
                                                                showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                                return
                                                            }
                                                        }
                                                        else {
                                                            //当日单
                                                            if (deliverDate == createDate) {
                                                                if (currentTimeNow > deliverTime - oneHours) {
                                                                    showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                                    return
                                                                }
                                                            }
                                                            else {
                                                                if (currentTimeNow > deliverTime - oneHours * 4) {
                                                                    showDlg("距离送餐时间已经不足四小时，本单已经无法修改");
                                                                    return
                                                                }
                                                            }
                                                        }
                                                        if (orderSuccess) {
                                                            return;
                                                        }
                                                        orderSuccess = true;
                                                        $('#btnSubmitOrder').unbind('click');
                                                        // 校验成功，发出请求
                                                        post('/P/PreApproval/Details', postdata, function (d) {
                                                            postdata.attendCount = postdata.attendCount * 1;
                                                            post('/P/PreApproval/Order', {},
                                                                function (d) {
                                                                    var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                                    showDlg(_msg, undefined, function () {
                                                                        WeixinJSBridge.call('closeWindow');
                                                                    }, 'success')
                                                                }, 'json');
                                                        }, 'json');
                                                    }
                                                }, 'json');
                                        }
                                        else {
                                            if (orderSuccess) {
                                                return;
                                            }
                                            orderSuccess = true;
                                            $('#btnSubmitOrder').unbind('click');
                                            // 校验成功，发出请求
                                            post('/P/PreApproval/Details', postdata, function (d) {
                                                postdata.attendCount = postdata.attendCount * 1;
                                                post('/P/PreApproval/Order', {},
                                                    function (d) {
                                                        var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                        showDlg(_msg, undefined, function () {
                                                            WeixinJSBridge.call('closeWindow');
                                                        }, 'success')
                                                    }, 'json');
                                            }, 'json');
                                        }
                                    }, '确认', '放弃', function () {
                                    }, 'info');
                                } else {
                                    if (isChangeOrder == true) {
                                        post('/P/Order/GetOrderInfoByHTCode',
                                            {
                                                htcode: nowOrderInfo.preApproval.HTCode
                                            }, function (d) {
                                                if (d.data != null) {
                                                    var oldOrderInfo = d.data;
                                                    var deliverDate = getDateByDotNet(oldOrderInfo.DeliverTime).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                    var createDate = getDateByDotNet(oldOrderInfo.CreateDate).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                    var deliverTime = getDateByDotNet(oldOrderInfo.DeliverTime).getTime();
                                                    var currentTimeNow = getTimeNow().getTime();
                                                    //修改餐品、详情
                                                    if (oldOrderInfo.RestaurantId == nowOrderInfo.foods.resId) {
                                                        if (currentTimeNow > deliverTime - oneHours) {
                                                            showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                            return
                                                        }
                                                    }
                                                    else {
                                                        //当日单
                                                        if (deliverDate == createDate) {
                                                            if (currentTimeNow > deliverTime - oneHours) {
                                                                showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                                return
                                                            }
                                                        }
                                                        else {
                                                            if (currentTimeNow > deliverTime - oneHours * 4) {
                                                                showDlg("距离送餐时间已经不足四小时，本单已经无法修改");
                                                                return
                                                            }
                                                        }
                                                    }
                                                    if (orderSuccess) {
                                                        return;
                                                    }
                                                    orderSuccess = true;
                                                    $('#btnSubmitOrder').unbind('click');
                                                    // 校验成功，发出请求
                                                    post('/P/PreApproval/Details', postdata, function (d) {
                                                        postdata.attendCount = postdata.attendCount * 1;
                                                        post('/P/PreApproval/Order', {},
                                                            function (d) {
                                                                var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                                showDlg(_msg, undefined, function () {
                                                                    WeixinJSBridge.call('closeWindow');
                                                                }, 'success')
                                                            }, 'json');
                                                    }, 'json');
                                                }
                                            }, 'json');
                                    }
                                    else {
                                        if (orderSuccess) {
                                            return;
                                        }
                                        orderSuccess = true;
                                        $('#btnSubmitOrder').unbind('click');
                                        // 校验成功，发出请求
                                        post('/P/PreApproval/Details', postdata, function (d) {
                                            postdata.attendCount = postdata.attendCount * 1;
                                            post('/P/PreApproval/Order', {},
                                                function (d) {
                                                    var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                    showDlg(_msg, undefined, function () {
                                                        WeixinJSBridge.call('closeWindow');
                                                    }, 'success')
                                                }, 'json');
                                        }, 'json');
                                    }
                                }
                            }, '确认', '放弃', function () {
                            }, 'info');
                        } else {
                            if (budget / attendance <= 10) {
                                showConfim('您的订单人均<=10元', '', function () {
                                    if (isChangeOrder == true) {
                                        post('/P/Order/GetOrderInfoByHTCode',
                                            {
                                                htcode: nowOrderInfo.preApproval.HTCode
                                            }, function (d) {
                                                if (d.data != null) {
                                                    var oldOrderInfo = d.data;
                                                    var deliverDate = getDateByDotNet(oldOrderInfo.DeliverTime).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                    var createDate = getDateByDotNet(oldOrderInfo.CreateDate).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                    var deliverTime = getDateByDotNet(oldOrderInfo.DeliverTime).getTime();
                                                    var currentTimeNow = getTimeNow().getTime();
                                                    //修改餐品、详情
                                                    if (oldOrderInfo.RestaurantId == nowOrderInfo.foods.resId) {
                                                        if (currentTimeNow > deliverTime - oneHours) {
                                                            showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                            return
                                                        }
                                                    }
                                                    else {
                                                        //当日单
                                                        if (deliverDate == createDate) {
                                                            if (currentTimeNow > deliverTime - oneHours) {
                                                                showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                                return
                                                            }
                                                        }
                                                        else {
                                                            if (currentTimeNow > deliverTime - oneHours * 4) {
                                                                showDlg("距离送餐时间已经不足四小时，本单已经无法修改");
                                                                return
                                                            }
                                                        }
                                                    }
                                                    if (orderSuccess) {
                                                        return;
                                                    }
                                                    orderSuccess = true;
                                                    $('#btnSubmitOrder').unbind('click');
                                                    // 校验成功，发出请求
                                                    post('/P/PreApproval/Details', postdata, function (d) {
                                                        postdata.attendCount = postdata.attendCount * 1;
                                                        post('/P/PreApproval/Order', {},
                                                            function (d) {
                                                                var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                                showDlg(_msg, undefined, function () {
                                                                    WeixinJSBridge.call('closeWindow');
                                                                }, 'success')
                                                            }, 'json');
                                                    }, 'json');
                                                }
                                            }, 'json');
                                    }
                                    else {
                                        if (orderSuccess) {
                                            return;
                                        }
                                        orderSuccess = true;
                                        $('#btnSubmitOrder').unbind('click');
                                        // 校验成功，发出请求
                                        post('/P/PreApproval/Details', postdata, function (d) {
                                            postdata.attendCount = postdata.attendCount * 1;
                                            post('/P/PreApproval/Order', {},
                                                function (d) {
                                                    var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                    showDlg(_msg, undefined, function () {
                                                        WeixinJSBridge.call('closeWindow');
                                                    }, 'success')
                                                }, 'json');
                                        }, 'json');
                                    }
                                }, '确认', '放弃', function () {
                                }, 'info');
                            } else {
                                if (isChangeOrder == true) {
                                    post('/P/Order/GetOrderInfoByHTCode',
                                        {
                                            htcode: nowOrderInfo.preApproval.HTCode
                                        }, function (d) {
                                            if (d.data != null) {
                                                var oldOrderInfo = d.data;
                                                var deliverDate = getDateByDotNet(oldOrderInfo.DeliverTime).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                var createDate = getDateByDotNet(oldOrderInfo.CreateDate).pattern('yyyy-MM-dd').replace(/-/g, '/');
                                                var deliverTime = getDateByDotNet(oldOrderInfo.DeliverTime).getTime();
                                                var currentTimeNow = getTimeNow().getTime();
                                                //修改餐品、详情
                                                if (oldOrderInfo.RestaurantId == nowOrderInfo.foods.resId) {
                                                    if (currentTimeNow > deliverTime - oneHours) {
                                                        showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                        return
                                                    }
                                                }
                                                else {
                                                    //当日单
                                                    if (deliverDate == createDate) {
                                                        if (currentTimeNow > deliverTime - oneHours) {
                                                            showDlg("距离送餐时间已经不足一小时，本单已经无法修改");
                                                            return
                                                        }
                                                    }
                                                    else {
                                                        if (currentTimeNow > deliverTime - oneHours * 4) {
                                                            showDlg("距离送餐时间已经不足四小时，本单已经无法修改");
                                                            return
                                                        }
                                                    }
                                                }
                                                if (orderSuccess) {
                                                    return;
                                                }
                                                orderSuccess = true;
                                                $('#btnSubmitOrder').unbind('click');
                                                // 校验成功，发出请求
                                                post('/P/PreApproval/Details', postdata, function (d) {
                                                    postdata.attendCount = postdata.attendCount * 1;
                                                    post('/P/PreApproval/Order', {},
                                                        function (d) {
                                                            var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                            showDlg(_msg, undefined, function () {
                                                                WeixinJSBridge.call('closeWindow');
                                                            }, 'success')
                                                        }, 'json');
                                                }, 'json');
                                            }
                                        }, 'json');
                                }
                                else {
                                    if (orderSuccess) {
                                        return;
                                    }
                                    orderSuccess = true;
                                    $('#btnSubmitOrder').unbind('click');
                                    // 校验成功，发出请求
                                    post('/P/PreApproval/Details', postdata, function (d) {
                                        postdata.attendCount = postdata.attendCount * 1;
                                        post('/P/PreApproval/Order', {},
                                            function (d) {
                                                var _msg = d.isChange == 0 ? MSG_SUBMITORDERSUCCESS : MSG_ORDERCHANGE;
                                                showDlg(_msg, undefined, function () {
                                                    WeixinJSBridge.call('closeWindow');
                                                }, 'success')
                                            }, 'json');
                                    }, 'json');
                                }
                            }
                        }
                    }
                }
                else {
                }
            }, 'json');

    });
});

// 从这里开始，是选择送餐时间
var deliverTimePicker = (function () {
    var _pickerDomId;
    var init = function (pickerDomId, deliverTime) {

        // 过滤出日期
        var _days = new Array();
        var days = new Array();
        deliverTime.forEach(function (v, i) {
            var day = v.time.substr(0, 10);
            if (!_days.contains(day)) {
                _days.push(day);
                var _d = {
                    time: day,
                    weekday: v.weekday,
                    day: day.substr(8, 2) * 1,
                    month: day.substr(5, 2) * 1
                };
                days.push(_d);
            }
        });

        // 过滤每个日期下的整小时
        var _hour = new Array();
        for (var i in days) {
            var day = days[i];
            _hours = new Array();
            day.ondayTimes = new Array();
            // 得到每天的时间
            deliverTime.forEach(function (v) {
                if (v.time.indexOf(day.time) == 0) {
                    day.ondayTimes.push(v);
                }
            });

            // 得到每天的小时
            day.hours = new Array();
            day.ondayTimes.forEach(function (v) {
                var hour = v.time.substr(0, 13);
                if (!_hours.contains(hour)) {
                    _hours.push(hour);
                    var _h = {
                        hour: hour
                    };
                    day.hours.push(_h);
                }
            });

            // 得到每个小时下时间
            for (var j in day.hours) {
                var hour = day.hours[j];
                hour.times = new Array();
                day.ondayTimes.forEach(function (v) {
                    if (v.time.indexOf(hour.hour) == 0) {
                        hour.times.push(v);
                    }

                });

            }
        }

        var html = renderdelivertime({ delivertime: days });
        $('#chooseDeliverTime').html(html);

        _pickerDomId = pickerDomId;

        var dyaswidth = $('#' + _pickerDomId + ' .days>.day').length * 90;
        $('#' + _pickerDomId + ' .days').css('width', dyaswidth + 'px');

        $('#' + _pickerDomId + ' .days>.day').addClass('checked');
        $('#' + _pickerDomId + ' .hour_body>div').hide();
        $('#' + _pickerDomId + ' .choosehour').removeClass('checked');
        $('#' + _pickerDomId + ' .time_body>div').hide();
        $('#' + _pickerDomId + ' .hour_body>div[_val="' + getDateByDotNet(nowOrderInfo.preApproval.MeetingDate).pattern('yyyy-MM-dd') + '"]').show();
        //$('#' + _pickerDomId + ' .hour_body>div[_val="' + checkedDay + '"]>div:eq(0)').click();
        $('#' + _pickerDomId + ' .hour_body')[0].scrollTop = 0;

        //$('#' + _pickerDomId + ' .days>.day').click(function () {
        //    $('#' + _pickerDomId + ' .days>.day').removeClass('checked');
        //    $(this).addClass('checked');
        //    var checkedDay = $(this).attr('_val');
        //    $('#' + _pickerDomId + ' .hour_body>div').hide();
        //    $('#' + _pickerDomId + ' .choosehour').removeClass('checked');
        //    $('#' + _pickerDomId + ' .time_body>div').hide();
        //    $('#' + _pickerDomId + ' .hour_body>div[_val="' + checkedDay + '"]').show();
        //    //$('#' + _pickerDomId + ' .hour_body>div[_val="' + checkedDay + '"]>div:eq(0)').click();
        //    $('#' + _pickerDomId + ' .hour_body')[0].scrollTop = 0;
        //});

        $('#' + _pickerDomId + ' .choosehour').click(function () {
            var checkedDay = $(this).attr('_val');
            $('#' + _pickerDomId + ' .time_body>div').hide();
            $('#' + _pickerDomId + ' div[_val="' + checkedDay + '"]').show();
            $('#' + _pickerDomId + ' .time_body')[0].scrollTop = 0;

            $('#' + _pickerDomId + ' .choosehour').removeClass('checked');
            $(this).addClass('checked');
        });

        $('#' + _pickerDomId + ' .body>div .chooseTime').click(function () {
            var time = $(this).attr('_val');
            $('#deliverTime').val(time.substr(0, time.length - 3));
            $('#deliverTime').attr('_val', time);
            // 清除红色叹号样式
            hidecell_warn('deliverTime');

            //$('#page_order').show();
            $('#chooseDeliverTime').hide();
        });

        //$('#' + _pickerDomId + ' .body>div').hide();

        //$('#page_order').hide();
        $('#' + _pickerDomId).show();

        $('#' + _pickerDomId + ' .day:eq(0)').click();
        $('#chooseDeliverTime .close').click(function () {
            $('#chooseDeliverTime').hide();
        });
    };

    return {
        init: init
    }
})();


Array.prototype.contains = function (needle) {
    for (i in this) {
        if (this[i] == needle) return true;
    }
    return false;
}
