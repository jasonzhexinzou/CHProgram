var render;
var renderMMCoE;
var orderInfo;
var preApproval;
var _contextUri = contextUri;
var evaluate;

$(function () {
    render = template('tmpl_order');
    renderMMCoE = template('tmpl_images');

    post('/P/Order/LoadOrderInfo', { id: orderId },
        function (d) {
            orderInfo = d.data;
            orderInfo.VeevaMeetingID = d.preApproval.VeevaMeetingID;
            orderInfo.DeliverTime = getDateByDotNet(orderInfo.DeliverTime);
            orderInfo.CreateDate = getDateByDotNet(orderInfo.CreateDate);
            orderInfo.MMCoE = getMMCoEState(orderInfo.MMCoEApproveState);
            preApproval = d.preApproval;
            var json = d.data.Detail;
            if (d.data.IsChange == 1) {
                json = d.data.ChangeDetail;
            }
            var _orderInfo = JSON.parse(json);

            //_orderInfo.foods.allPrice = outputdollars(_orderInfo.foods.allPrice.toString());
            _orderInfo.foods.allPrice = format(_orderInfo.foods.allPrice);

            _orderInfo.details.deliverTime = _orderInfo.details.deliverTime.toString().replace(/T/g, ' ').substring(0, 16);

            orderInfo.Detail = _orderInfo;
            var _channel = orderInfo.Channel;
            if (_channel == 'xms') {
                _channel = 'XMS';
            } else {
                _channel = 'BDS';
            }
            orderInfo.Channel = _channel;
            var stateName = '';
            switch (orderInfo.State) {
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
                    stateName = '订单确认收餐';
                    break;
                case 7:
                    stateName = '订单系统收餐';
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
            orderInfo.StateCN = stateName;
            switch (orderInfo.IsSpecialOrder) {
                case 1: orderInfo.IsSpecialOrder = "呼叫中心操作退单"; break;
                case 2: orderInfo.IsSpecialOrder = "会议支持文件丢失"; break;
            }
            switch (orderInfo.IsRetuen) {
                case 1: orderInfo.IsRetuen = "订单申请退单"; break;
                case 2: orderInfo.IsRetuen = "订单退单成功"; break;
                case 3: orderInfo.IsRetuen = "订单退单失败"; break;
                case 4: orderInfo.IsRetuen = "订单退单失败"; break;
                case 5: orderInfo.IsRetuen = "订单退单失败"; break;
                case 6: orderInfo.IsRetuen = "订单退单失败"; break;
            }
            var html = render(orderInfo);
            $('#orderinfo').html(html);

            if (orderInfo.MMCoEImage != null && orderInfo.MMCoEImage != undefined && orderInfo.MMCoEImage != '') {
                var images = orderInfo.MMCoEImage.split(',');
                images.forEach(function (v, i) {
                    images[i] = awsUrl + images[i];
                });

                var html = $(renderMMCoE({ items: images }));
                var imageUrls = new Array();
                var http = location.protocol + '//' + location.host;
                for (var i in images) {
                    var _url = images[i];
                    imageUrls.push(_url);
                }
                $('#mmCoEImages').html(html);
                $('#mmCoEImages img').each(function (i, e) {
                    $(this).click(function () {
                        WeixinJSBridge.invoke('imagePreview', {
                            'current': imageUrls[i],
                            'urls': imageUrls
                        });
                    });
                });
            }

            if (orderInfo.State == 9 || orderInfo.State == 8) {
                post('/P/Order/LoadEvaluate', { id: orderId },
                    function (d) {
                        evaluate = d.data;
                        if (orderInfo.State == 9) {
                            var render9 = template('tmpl_evaluate');
                            evaluate.OnTimeName = getState1(evaluate.OnTime);
                            evaluate.IsSafe = getState2(evaluate.IsSafe);
                            evaluate.Health = getState3(evaluate.Health);
                            evaluate.Pack = getState3(evaluate.Pack);
                            evaluate.CostEffective = getState3(evaluate.CostEffective);
                           
                            var html = render9(evaluate);
                            $('#evaluateinfo').html(html);
                            iPath.Star();
                            if (evaluate.SafeImage != null && evaluate.SafeImage != undefined && evaluate.SafeImage != '') {
                                var images = evaluate.SafeImage.split(',');
                                images.forEach(function (v, i) {
                                    images[i] = awsUrl + images[i];
                                });
                                var html = $(renderMMCoE({ items: images }));
                                var safeImageUrls = new Array();
                                var http = location.protocol + '//' + location.host;
                                for (var i in images) {
                                    var _safeImageUrl = images[i];
                                    safeImageUrls.push(_safeImageUrl);
                                }
                                $('#safeImage').html(html);
                                $('#safeImage img').each(function (i, e) {
                                    $(this).click(function () {
                                        WeixinJSBridge.invoke('imagePreview', {
                                            'current': safeImageUrls[i],
                                            'urls': safeImageUrls
                                        });
                                    });
                                });
                            }

                            if (evaluate.HealthImage != null && evaluate.HealthImage != undefined && evaluate.HealthImage != '') {
                                var images = evaluate.HealthImage.split(',');
                                images.forEach(function (v, i) {
                                    images[i] = awsUrl + images[i];
                                });
                                var html = $(renderMMCoE({ items: images }));
                                var healthImageUrls = new Array();
                                var http = location.protocol + '//' + location.host;
                                for (var i in images) {
                                    var _url = images[i];
                                    healthImageUrls.push(_url);
                                }
                                $('#healthImage').html(html);
                                $('#healthImage img').each(function (i, e) {
                                    $(this).click(function () {
                                        WeixinJSBridge.invoke('imagePreview', {
                                            'current': healthImageUrls[i],
                                            'urls': healthImageUrls
                                        });
                                    });
                                });
                            }

                            if (evaluate.PackImage != null && evaluate.PackImage != undefined && evaluate.PackImage != '') {
                                var images = evaluate.PackImage.split(',');
                                images.forEach(function (v, i) {
                                    images[i] = awsUrl + images[i];
                                });
                                var html = $(renderMMCoE({ items: images }));
                                var packImageUrls = new Array();
                                var http = location.protocol + '//' + location.host;
                                for (var i in images) {
                                    var _url = images[i];
                                    packImageUrls.push(_url);
                                }
                                $('#packImage').html(html);
                                $('#packImage img').each(function (i, e) {
                                    $(this).click(function () {
                                        WeixinJSBridge.invoke('imagePreview', {
                                            'current': packImageUrls[i],
                                            'urls': packImageUrls
                                        });
                                    });
                                });
                            }

                            if (evaluate.CostEffectiveImage != null && evaluate.CostEffectiveImage != undefined && evaluate.CostEffectiveImage != '') {
                                var images = evaluate.CostEffectiveImage.split(',');
                                images.forEach(function (v, i) {
                                    images[i] = awsUrl + images[i];
                                });
                                var html = $(renderMMCoE({ items: images }));
                                var costEffectiveImageUrls = new Array();
                                var http = location.protocol + '//' + location.host;
                                for (var i in images) {
                                    var _url = images[i];
                                    costEffectiveImageUrls.push(_url);
                                }
                                $('#costEffectiveImage').html(html);
                                $('#costEffectiveImage img').each(function (i, e) {
                                    $(this).click(function () {
                                        WeixinJSBridge.invoke('imagePreview', {
                                            'current': costEffectiveImageUrls[i],
                                            'urls': costEffectiveImageUrls
                                        });
                                    });
                                });
                            }
                        } else {
                            var render8 = template('tmpl_undelivered');
                            var html = render8(evaluate);
                            $('#evaluateinfo').html(html);
                        }
                    }, 'json');
            }

            // 控制底部按钮显隐可用状态
            onOrderLoadSuccess();

        }, 'json');

});

function getMMCoEState(state) {
    var res;
    switch (state) {
        case 0:
            res = '不需要审批';
            break;
        case 1:
            res = '等待审批';
            break;
        case 2:
            res = '审批拒绝';
            break;
        case 3:
            res = '审批通过';
            break;
    }
    return res;
}

function getState1(state) {
    var res;
    switch (state) {
        
        case 1:
            res = '迟到60分钟及以上';
            break;
        case 2:
            res = '迟到60分钟以内';
            break;
        case 3:
            res = '早到';
            break;
        case 4:
            res = '准点';
            break;
        case 5:
            res = '迟到60分及以上';
            break;
        case 6:
            res = '迟到30-60分钟';
            break;
        case 7:
            res = '迟到30分钟以内';
            break;
        case 8:
            res = '早到30分钟及以上';
            break;
        case 9:
            res = '早到30分钟以内';
            break;
        case 10:
            res = '准点';
            break;
        default:
            res = '';
            break;
    }
    return res;
}

function getState2(state) {
    var res;
    if (state == 1) {
        res = '有问题';
    }
    else if (state == 2) {
        res = '无问题';
    }
    else {
        res = '';
    }
    return res;
}

function getState3(state) {
    var res;
    switch (state) {
        case 1:
            res = '好';
            break;
        case 2:
            res = '中';
            break;
        case 3:
            res = '差';
            break;
        default:
            res = ''
            break;
    }
    return res;
}

//格式化金额
function outputdollars(number) {
    if (number.length <= 3) {
        return (number == '' ? '0' : number + '.00');
    } 
    else {
        var mod = number.length % 3;
        var output = (mod == 0 ? '' : (number.substring(0, mod)));
        for (i = 0; i < Math.floor(number.length / 3) ; i++) {
            if ((mod == 0) && (i == 0))
                output += number.substring(mod + 3 * i, mod + 3 * i + 3);
            else
                output += ',' + number.substring(mod + 3 * i, mod + 3 * i + 3);
        }
        output+= '.00';
        return (output);
    }
}

onOrderLoadSuccess = function () { };



