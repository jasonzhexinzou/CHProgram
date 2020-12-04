function AddressList() { }
AddressList.fn = AddressList.prototype;
AddressList.fn.$nearaddresspanel = undefined;
AddressList.fn.$autocompletion = undefined;
AddressList.fn.Init = function () {
    var that = this;

    that.$nearaddresspanel = $('.nearaddresspanel').iPathDataBind();
    that.$autocompletion = $('.autocompletion').iPathDataBind();

    $('#chooseHospital').show();
    $('#chooseCity').hide();

    // 初始化城市列表
    $('#chooseCity .quickchoose').iPathDataBind().DataBind(citys);
    $('#chooseCity ul').iPathDataBind().DataBind(citys);
    $('#chooseCity .quickchoose>div').click(function () {
        // 滚动到城市分组
        var target = $(this).attr('target');
        var offset = $('#citygroupA').offset().top - $('#' + target).offset().top;
        offset = Math.abs(offset);
        $("#addresslist .body").animate({ scrollTop: offset }, 500);
    });
    $('#chooseCity li').click(function () {
        var $li = $(this);
        that.ChooseCity($li.attr('district'), $li.text());
    });

    // 城市搜索
    $('#citykeyword').bind('input propertychange', function () {
        if ($('#chooseCity:visible').length > 0) {
            that.Search($(this));
        } else {
            that.SearchAMap($(this));
        }
    });
    $('#citykeyword').bind('click', function () {
        var district = $('#addresslist .headtools .city').attr('district');
        if (district == undefined || district == '') {
            $('#addresslist .headtools .city').click();
        }

    });

    // 取消搜索
    $('#citykeyword').next().click(function () {
        that.CancelSearch();
    });

    // 切换到城市选择区
    $('#addresslist .city').click(function () {
        $('#chooseHospital').hide();
        $('#chooseCity').show();
    });

    // 刷新当前位置方法
    $('#refresh').click(function () {
        that.RefreshLoaction();
    });
    $('#refresh').click();

    // 当前位置医院点击事件
    $('#addresslist .line0').click(function () {
        var $line = $(this);
        var geo = $line.attr('geo');
        var name = $line.attr('name');

        if (geo == undefined || geo == '') {
            return;
        }

        iPath.ChangeTitle(name);
        $('#querykey').val('');
        $('#hospitalname').text(name);
        $('#querykey').attr('geo', geo);
        var _geo_arry = geo.split(',');
        globalCache._geo._lon = _geo_arry[0];
        globalCache._geo._lat = _geo_arry[1];

        globalCache.restaurantList.Show();
        $('#btnSearch').click();
    });

    // 加载常用地址
    iPath.Post(contextUri + '/Food/LoadFrequentAddress', { district: '' }, function (d) {
        that.$nearaddresspanel.DataBind(d.rows);
        $('#addresslist .line1').click(function () {
            var $line = $(this);
            var geo = $line.attr('geo');
            var name = $line.attr('name');
            var phone = $line.attr('phone');

            iPath.ChangeTitle(name);
            $('#querykey').val('');
            $('#hospitalname').text(name);
            $('#querykey').attr('geo', geo);
            var _geo_arry = geo.split(',');
            _lon = _geo_arry[0];
            _lat = _geo_arry[1];
            $('#phone').val(phone);

            globalCache.restaurantList.Show();
            $('#btnSearch').click();
        });


    }, 'json');
}
AddressList.fn.Show = function () {
    var that = this;
    //$('body>div').hide();
    //$('#addresslist').show();
    foodHadnler.showpanel('addresslist');
    that.CancelSearch();
}
AddressList.fn.Hide = function () {
    var that = this;
    $('#addresslist').Hide();
}
// 搜索框
AddressList.fn.Search = function (jDom) {
    var key = jDom.val().trim();
    if (key == '') {
        this.CancelSearch();
    }
    $('#chooseCity li').hide();
    $('#chooseCity li').each(function () {
        var $li = $(this);
        var pinyin = $li.attr("pinyin");
        var jianpin = $li.attr("jianpin");
        if (pinyin == undefined) {
            return;
        }
        if (pinyin.indexOf(key) >= 0
            || jianpin.indexOf(key) >= 0
            || $li.text().indexOf(key) >= 0) {
            $li.show();
        }
    });

    $('.nearaddresspanel .line1').hide();
    $('.nearaddresspanel .line1').each(function () {
        var $li = $(this);
        var pinyin = $li.attr("pinyin");
        var jianpin = $li.attr("jianpin");
        if (pinyin == undefined) {
            return;
        }
        if (pinyin.indexOf(key) >= 0
            || jianpin.indexOf(key) >= 0) {
            $li.show();
        }
    });

}
// 搜索框
AddressList.fn.SearchAMap = function (jDom) {
    var that = this;
    var key = jDom.val().trim();
    if (key == '') {
        this.CancelSearch();
    }
    if (key.indexOf("'") != -1) {
        return;
    }
    var district = $('#addresslist .city').attr('district');
    foodHadnler.searchPoint(district, key, function (status, result) {
        if ('complete' == status && result.info == 'OK' && result.poiList.pois.length > 0) {
            that.$autocompletion.DataBind(result.poiList.pois);
            that.$autocompletion.jq_obj.show();
            that.$autocompletion.jq_obj.find('li').click(function () {
                var $this = $(this);
                var name = $this.attr('_name');
                var address = $this.attr('_address');
                var geo = $this.attr('_geo');

                var $line0 = $('#chooseHospital .line0');
                $line0.text(name);
                $line0.attr('name', name + ' ' + address);
                $line0.attr('geo', geo);
                that.$autocompletion.jq_obj.hide();
                $line0.click();
            });
        }
    });

}
// 取消搜索
AddressList.fn.CancelSearch = function () {
    $('#citykeyword').val('');
    $('#chooseCity li').show();
    $('.nearaddresspanel .line1').show();
    this.$autocompletion.jq_obj.hide();
}
AddressList.fn.IsPageInit = true;
// 重新获取当前所在地址
AddressList.fn.RefreshLoaction = function () {
    var that = this;

    getAMapGeo(function (amapdata) {
        that.ChooseCity(globalCache.currentCityCode, globalCache.currentCity);
        var $line0 = $('#chooseHospital .line0');
        $line0.text(amapdata.amap.formattedAddress);
        $line0.attr('name', amapdata.amap.formattedAddress);
        $line0.attr('geo', amapdata.amap.lon + ',' + amapdata.amap.lat);
        if (that.IsPageInit) {
            setTimeout(function () { $line0.click(); }, 100);

            that.IsPageInit = false;
        }
    });

}
// 选择了城市
AddressList.fn.ChooseCity = function (district, name) {
    var that = this;
    that.CancelSearch();

    $('#addresslist .city').text(name);
    $('#addresslist .city').attr('district', district);

    // 切换到市内选择区
    $('#chooseHospital').show();
    $('#chooseCity').hide();
}