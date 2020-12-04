
var globalCache = {};

var foodHadnler = (function () {

    var placeSearch = undefined;

    var init = function () {
        //getGps(loadgspcallback);
        getAMapGeo(loadgspcallback);



        AMap.service('AMap.PlaceSearch', function () {//回调函数
            placeSearch = new AMap.PlaceSearch({ //构造地点查询类
                pageSize: 100,
                pageIndex: 1
            });
        });

        var route = new iPathRoute();
        route.Init();
        route.AddListener('/Food/Index', function (pathname, eventState) {
            globalCache.restaurantList.Show();
            iPath.ChangeTitle($('#hospitalname').text());

        });
        route.AddListener('/Food', function (pathname, eventState) {
            globalCache.restaurantList.Show();
            iPath.ChangeTitle($('#hospitalname').text());

        });
        route.AddListener('/Food/List', function (pathname, eventState) {
            if (eventState.restaurant_name == document.title) {
                globalCache.foodlist.Show();
            } else {
                globalCache.foodlist.Show(eventState.restaurantId, eventState.agent_fee, eventState.restaurant_name, eventState.deliver_amount, eventState.image_url);
                iPath.ChangeTitle(eventState.restaurant_name);
            }
        });
        route.AddListener('/Food/Cart', function (pathname, eventState) {
            globalCache.submitorder.Show();
        });
        globalCache.route = route;
    };

    // 对话框对象初始化
    var initdlghandler = function () {
        // 餐厅列表
        globalCache.restaurantList = new RestaurantList();
        globalCache.restaurantList.Init();

        // 地址选择
        globalCache.addresslist = new AddressList();
        globalCache.addresslist.Init();

        // 餐品列表
        globalCache.foodlist = new FoodList();
        globalCache.foodlist.Init();

        // 确认下单
        globalCache.submitorder = new Submitorder();
        globalCache.submitorder.Init();
    };

    // 获取当前GPS坐标回调函数
    var loadgspcallback = function (data) {
        
        if (data.state == 1) {
            $('.loadgeo').remove();

            globalCache.geo = {
                lon: data.lon,
                lat: data.lat
            };

            if (data.amap.state == 1) {
                globalCache._geo = {
                    _lon: data.amap.lon,
                    _lat: data.amap.lat
                };
                globalCache.currentCity = data.amap.currentCity;
                globalCache.currentCityCode = data.amap.currentCityCode;
                globalCache.formattedAddress = data.amap.formattedAddress;
                firstLoadGeoSuccess();
            } else {

            }
        } else {
            $('.loadgeo .loading').html('获取GPS坐标失败！');
            $('.loadgeo .loading').click(function () {
                $('.loadgeo').remove();
            });
        }
    };

    var isFirstLoadGeoSuccess = true;
    var firstLoadGeoSuccess = function () {
        if (!isFirstLoadGeoSuccess) {
            return;
        }
        isFirstLoadGeoSuccess = false;
        initdlghandler();
        showpanel('restaurantlist');
    };

    //var printGlobalCache = function () {
    //    alert(JSONUtil.encode(globalCache));
        
    //};

    // 切换前台显示内容
    var showpanel = function (domid) {
        $('#container > div').appendTo($('#hidecontainer'));
        $('#' + domid).appendTo($('#container'));
    }
    
    // 关键字查询检索地点
    function searchPoint(city, key, fn) {
        placeSearch.setCity(city);
        placeSearch.search(key, function (status, result) {
            fn(status, result);
        });
    }

    return {
        init: init,
        showpanel: showpanel,
        searchPoint: searchPoint
    };
})();

function toTowFloat(value) {
    var num = '' + value.toFixed(2);
    for (var i = 0; i < 2; i++) {
        if (num.charAt(num.length - 1) == '0') {
            num = num.substring(0, num.length - 1);
        }
    }
    if (num.charAt(num.length - 1) == '.') {
        num = num.substring(0, num.length - 1);
    }
    return num;
}



