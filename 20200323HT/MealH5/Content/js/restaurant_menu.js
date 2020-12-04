
var renderMenu;
var renderFood;
var renderCart;
var renderCheckedFood;
var fastSelectId;
var source;
var dataState = "";
// 载入餐厅餐品
function loadMenu(restaurantId) {
    post('/P/Restaurant/LoadMenu', {
        restaurantId: restaurantId,
        supplier: supplier,
        hospitalId: hospitalId,
        sendTime: sendTime
    },
        function (d) {
            if (d.data.errorCode == "8011" || d.data.errorCode == "8012") {
                var reName = restaurantName != "" ? restaurantName : nowOrderInfo.foods.resName;
                if (changeSendTime == 1) {
                    showDlg("您选择的餐厅" + reName + "在此送餐时间不可配送，请重新选择其他餐厅，或取消修改，原订单仍可正常配送。", '确定', function () {
                        WeixinJSBridge.call('closeWindow');
                    }, 'info');
                } else {
                    showDlg("您选择的餐厅" + reName + "已下线，请重新选择其他餐厅，或取消修改，原订单仍可正常配送。", '确定', function () {
                        WeixinJSBridge.call('closeWindow');
                    }, 'info');
                }
                return false;
            }
            iPath.ChangeTitle(d.data.resName);
            resName = d.data.resName;

            var htmlMenu = renderMenu(d.data);
            var htmlFood = renderFood(d.data);

            $("#foodtype").html(htmlMenu);
            $("#foodlist").html(htmlFood);

            $('#foodlist .infopanel .name').click(function () {
                // TODO
                var dis = $(this).next();
                if (dis.is(":hidden")) {
                    dis.show();
                } else {
                    dis.hide();
                }

            });

            $('#foodtype>li').click(function () {
                var typeId = $(this).attr('target');
                showFoodType(typeId);
                $('#foodtype>li').removeClass('checked');
                $(this).addClass('checked');
            });
            $('#foodtype>li:eq(0)').click();

            $("#foodlist .btntools>span:nth-child(1)").click(function () {
                var foodId = $(this).closest('li').attr('_id');
                chooseFood(foodId, -1);
            });

            $("#foodlist .btntools>span:nth-child(4)").click(function () {
                fastSelectId = $(this).closest('li').attr('_id');
                $("#dvFastSelect").show();
                source = "foodlist";
            });

            $("#foodlist .btntools span:nth-child(3)").click(function () {
                var foodId = $(this).closest('li').attr('_id');
                chooseFood(foodId, 1);
            });

            // 如果从前选过餐，显示之前选的餐
            if (nowFoods != undefined && nowFoods.foods != undefined && nowFoods.foods != null && nowFoods.foods.length > 0) {
                nowFoods.foods.forEach(function (item) {
                    chooseFood(item.foodId, item.count * 1, false);
                });
                drawFootBar();
            }

            //餐品下架提示
            var allFoodList = new Array();//当前时间所有餐品
            var flag = 0;
            var removeFoodsName = new Array();
            for (var i = 0; i < d.data.listMenu.length; i++) {
                for (var j = 0; j < d.data.listMenu[i].listFood.length; j++) {
                    allFoodList.push(d.data.listMenu[i].listFood[j].foodId);
                }
            }
            if (nowFoods != null) {
                for (var k = 0; k < nowFoods.foods.length; k++) {
                    if (allFoodList.indexOf(nowFoods.foods[k].foodId) < 0) {
                        flag = 1;
                        removeFoodsName.push(nowFoods.foods[k].foodName);
                    }
                }
            }
            if (flag != 0) {
                if (changeSendTime == 1) {
                    showDlg("您选择的餐品" + removeFoodsName.join('，') + "在此送餐时间不可供应，请重新选择其他餐品，或取消修改，原订单仍可正常配送。", '确定', function () {
                    }, 'info');
                } else {
                    showDlg("您选择的餐品" + removeFoodsName.join('，') + "已下架，请重新选择其他餐品，或取消修改，原订单仍可正常配送。", '确定', function () {
                    }, 'info');
                }
            }

        }, 'json');
}

// 切换餐品种类
function showFoodType(typeId) {
    $('#foodlist>ul').hide();
    $('#' + typeId).show();
    $('#foodlist')[0].scrollTop = 0;

}

// 增减餐品数量
function chooseFood(foodId, count, flagDraw) {
    var oldCount = $('#foodlist li[_id="' + foodId + '"] .btntools>span:eq(1)').html() * 1;
    var newCount = oldCount + count;
    if (newCount < 0) {
        newCount = 0;
    }
    $('#foodlist li[_id="' + foodId + '"] .btntools>span:eq(1)').html(newCount);
    if (flagDraw != false) {
        drawFootBar();
    }

}

function chooseFastFood(foodId, count, flagDraw) {
    var oldCount = $('#foodlist li[_id="' + foodId + '"] .btntools>span:eq(1)').html() * 1;
    var newCount = count;
    if (newCount < 0) {
        newCount = 0;
    }
    $('#foodlist li[_id="' + foodId + '"] .btntools>span:eq(1)').html(newCount);
    if (flagDraw != false) {
        drawFootBar();
    }

}

// 增减购物车餐品数量
function chooseCartFood(foodId, count) {
    var oldCount = $('#shoppingcart li[_id="' + foodId + '"] .btntools>span:eq(1)').html() * 1;
    var newCount = oldCount + count;
    if (newCount < 0) {
        newCount = 0;
    }

    $('#shoppingcart li[_id="' + foodId + '"] .btntools>span:eq(1)').html(newCount);
    chooseFood(foodId, count);

    if (newCount == 0) {
        $('#shoppingcart li[_id="' + foodId + '"]').remove();
    }

    var $price = $('#shoppingcart li[_id="' + foodId + '"] .money');
    var price = $price.attr('price') * 1;
    var sum = (price * 100) * newCount / 100;
    sum = toTowFloat(sum);
    $price.html(sum);

}

// 查询当前餐厅点餐数量
function getCheckedFood() {
    var list = new Array();
    $('#foodlist li').each(function () {
        var $li = $(this);
        var _count = $li.find('.btntools>span:eq(1)').html() * 1;
        if (_count > 0) {
            var food = {
                price: $li.find('.price').attr('price') * 1,
                foodId: $li.attr('_id'),
                count: _count,
                foodName: $li.find('.name').html(),
                describe: $li.find('.description').html()
            };
            list.push(food);
        }
    });

    return list;
}

// 刷新并且显示购物车
function showShopCart() {
    var listCheckedFood = getCheckedFood();
    var html = renderCart({ listFood: listCheckedFood });
    $('#shoppingcart ul').html(html);

    $('#shoppingcart .btntools>span:nth-child(1)').click(function () {
        var foodId = $(this).closest('li').attr('_id');
        chooseCartFood(foodId, -1);
    });

    $('#shoppingcart .btntools>span:nth-child(3)').click(function () {
        var foodId = $(this).closest('li').attr('_id');
        chooseCartFood(foodId, 1);
    });

    $("#shoppingcart .btntools>span:nth-child(4)").click(function () {
        fastSelectId = $(this).closest('li').attr('_id');
        source = "shoppingcart";
        $("#dvFastSelect").show();
    });

}

// 刷新当前餐厅订单统计条
function drawFootBar() {
    var listCheckedFood = getCheckedFood();

    if (listCheckedFood.length == 0) {
        $('#menu_page .tip li>label:nth-child(2)').html('0');
        return;
    }

    var count = iPath.Sum(listCheckedFood, function (a) {
        return a.count;
    });
    var sum = iPath.Sum(listCheckedFood, function (a) {
        var res = a.count * (a.price * 100) / 100;
        res = toTowFloat(res);
        return res;
    });
    sum = toTowFloat(sum);
    $('#menu_page .tip li:eq(0) label:eq(1)').html(count);
    $('#menu_page .tip li:eq(1) label:eq(1)').html(sum);

}

// 检查订单量
function checkNowFoods() {
    var listCheckedFood = getCheckedFood();
    var foods = iPath.Select(listCheckedFood, function (a) {
        return { foodId: a.foodId, foodName: a.foodName, count: a.count, describe: a.describe };
    });
    post('/P/Restaurant/CalculateFee',
        {
            hospitalId: hospitalId,
            resId: restaurantId,
            foods: foods,
            dataState: dataState,
            supplier: supplier,
            sendTime: sendTime
        },
        function (d) {
            $('#checked_page .tip li:eq(2) label:eq(1)').html(d.data.sendFee);
            $('#checked_page .tip li:eq(3) label:eq(1)').html(d.data.packageFee);
            $('#checked_page .tip li:eq(4) label:eq(1)').html(d.data.allPrice);

            iRoute.Navigation(contextUri + '/P/Restaurant/Menu#details', {}, '菜单详情');

        }, 'json');
}

function FastSelectCancel() {
    $("#dvFastSelect").hide();
}

function FastSelect(num, flagDraw) {
    var foodId = fastSelectId;
    if (source == "foodlist") {
        var oldCount = $('#foodlist li[_id="' + foodId + '"] .btntools>span:eq(1)').html() * 1;
        var newCount = num * 1;
        if (newCount < 0) {
            newCount = 0;
        }
        $('#foodlist li[_id="' + foodId + '"] .btntools>span:eq(1)').html(newCount);
        if (flagDraw != false) {
            drawFootBar();
        }
    }
    else {
        var oldCount = $('#shoppingcart li[_id="' + foodId + '"] .btntools>span:eq(1)').html() * 1;
        var newCount = num * 1;
        if (newCount < 0) {
            newCount = 0;
        }

        $('#shoppingcart li[_id="' + foodId + '"] .btntools>span:eq(1)').html(newCount);
        chooseFastFood(foodId, newCount);

        if (newCount == 0) {
            $('#shoppingcart li[_id="' + foodId + '"]').remove();
        }

        var $price = $('#shoppingcart li[_id="' + foodId + '"] .money');
        var price = $price.attr('price') * 1;
        var sum = (price * 100) * newCount / 100;
        sum = toTowFloat(sum);
        $price.html(sum);
    }
    FastSelectCancel();
}

var nowOrderInfo;
var nowFoods;
var resName;


var iRoute;

$(function () {
    dataState = window.sessionStorage.getItem("State");
    iRoute = new iPathRoute();
    iRoute.Init();
    iRoute.AddListener(contextUri + '/P/Restaurant/Menu', function () {
        $('#menu_page').show();
        $('#checked_page').hide();
        iPath.ChangeTitle(resName);
    });

    iRoute.AddListener(contextUri + '/P/Restaurant/Menu#details', function () {
        $('#menu_page').hide();

        var listCheckedFood = getCheckedFood();

        var count = iPath.Sum(listCheckedFood, function (a) {
            return a.count;
        });
        var sum = iPath.Sum(listCheckedFood, function (a) {
            return toTowFloat(a.count * (a.price * 100) / 100);
        });
        sum = toTowFloat(sum);

        $('#checked_page .tip li:eq(0) label:eq(1)').html(count);
        $('#checked_page .tip li:eq(1) label:eq(1)').html(sum);

        var foods = iPath.Select(listCheckedFood, function (a) {

            return { foodId: a.foodId, count: a.count, foodName: a.foodName, price: a.price, describe: a.describe };
        });

        var _f = {
            resId: restaurantId,
            resName: resName,
            foods: foods,
            totalFoodFee: $('#checked_page .tip li:eq(1) label:eq(1)').html(),
            sendFee: $('#checked_page .tip li:eq(2) label:eq(1)').html(),
            packageFee: $('#checked_page .tip li:eq(3) label:eq(1)').html(),
            allPrice: $('#checked_page .tip li:eq(4) label:eq(1)').html(),
            totalCount: $('#checked_page .tip li:eq(0) label:eq(1)').html()
        };

        var html = renderCheckedFood({ foods: _f });

        $('.foodinfo').html(html);

        iPath.ChangeTitle('菜单详情');
        $('#checked_page').show();

    });


    renderMenu = template('menu_lists');
    renderFood = template('menu_foods');
    renderCart = template('menu_shoppingcart');
    renderCheckedFood = template('order_foods');

    post('/P/PreApproval/NowOrder', {}, function (d) {
        nowOrderInfo = d.data;
        if (sendTime == "") {
            if (d.data.details != null) {
                if (d.data.details.deliverTime != null)
                    sendTime = getDateByDotNet(d.data.details.deliverTime).pattern('yyyy-MM-dd HH:mm:ss');
            } else {
                //新下单
                sendTime = window.sessionStorage.getItem("CacheSendTime");
            }
        }
        if (nowOrderInfo != null
            && nowOrderInfo != undefined
            && nowOrderInfo.foods != null
            && nowOrderInfo.foods != undefined
            && nowOrderInfo.hospital != null
            && nowOrderInfo.hospital != undefined) {

            if (restaurantId == '' || restaurantId == undefined || restaurantId == null) {
                restaurantId = nowOrderInfo.foods.resId;
                if (supplier == '' || supplier == undefined || supplier == null) {
                    supplier = nowOrderInfo.supplier;
                }

            }

            if (restaurantId == nowOrderInfo.foods.resId) {
                hospitalId = nowOrderInfo.preApproval.HospitalAddressCode;
                nowFoods = nowOrderInfo.foods;
            }

        }

        if (restaurantId == '' || restaurantId == undefined || restaurantId == null) {
            showTopMsg('流程错误，请先完成前置步骤！');
        }

        loadMenu(restaurantId);

    }, 'json');

    $('#btnClear, .shoppingcart .carpanel .head>span:eq(1)').click(function () {
        $("#foodlist .btntools>span:nth-child(2)").html('0');
        $('#shoppingcart ul').html('');
        drawFootBar();
    });

    $('#btnShopCart').click(function () {
        showShopCart();

        if ($('.shoppingcart').is(":hidden")) {
            $('.shoppingcart').show();
        } else {
            $('.shoppingcart').hide();
        }
    });

    $('#btnNext').click(function () {
        var listCheckedFood = getCheckedFood();
        if (listCheckedFood.length < 1) {
            showTopMsg(MSG_NEEDCHOOSEFOOD);
            return;
        }

        checkNowFoods();
    });

    $('#btnTrueNext').click(function () {
        var listCheckedFood = getCheckedFood();
        var allPrice = $('#checked_page .tip li:eq(4) label:eq(1)').html() * 1;
        var foods = iPath.Select(listCheckedFood, function (a) {
            return { foodId: a.foodId, count: a.count, foodName: a.foodName, price: a.price, describe: a.describe };
        });
        if (nowOrderInfo.preApproval.BudgetTotal < allPrice) {
            showDlg(MSG_TOTALVOERPROOF, '确定', function () { }, 'cancel')
            return;
        }

        post('/P/Restaurant/FindRestaurant',
            {
                hospitalId: hospitalId,
                restaurantId: restaurantId,
                dataState: dataState,
                supplier: supplier,
                sendTime: sendTime
            },
            function (da) {
                if (da.data.minAmount > allPrice) {
                    showDlg(MSG_TOTALTOOLOWER, '确定', function () { }, 'cancel')
                    return;
                }

                // 通过校验后提交
                post('/P/Restaurant/SaveFood',
                    {
                        resId: restaurantId,
                        resName: resName,
                        foods: foods,
                        dataState: dataState,
                        supplier: supplier,
                        hospitalId: hospitalId,
                        sendTime: sendTime
                    },
                    function (d) {
                        //$('#form0').submit();
                        location.href = contextUri + '/P/Food/Order?supplier=' + supplier + "&state=" + dataState + "&sendTime=" + sendTime;
                    }, 'json');
            }, 'json');


    });


});

