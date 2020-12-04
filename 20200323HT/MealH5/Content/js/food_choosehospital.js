var cacheTA = new Array();
var cacheProvince = new Array();
var cacheCity = new Array();
var cacheHospital = new Array();
var cacheHospitalShow = new Array();

// 保存缓存
function setCache(list, cache) {
    for (var i in list) {
        var item = list[i];
        cache[item.ID] = item;
    }
}

function setCacheTA(list) {
    //var i = 0;
    //for (var t in list) {
    //    var item = list[t];
    //    cacheTA[i] = item;
    //    i++;
    //}
    for (var i = 0; i < list.length; i++) {
        var item = list[i];
        cacheTA[i] = item;
        if (i == list.length) {
            i--;
        }
    }
}

// 保存省份缓存
function setCacheProvince(list) {
    setCache(list, cacheProvince);
}

// 保存城市缓存
function setCacheCity(list) {
    setCache(list, cacheCity);
}

// 保存医院缓存
function setCacheHospital(list) {
    var _list;
    //if (IsOutSideHT) {
        // 允许使用院外会，开放全部数据
        _list = list;
    //} else {
    //    // 不允许使用院外会，屏蔽院外会数据
    //    _list = iPath.Where(list, function (a) {
    //        if (a.External == 0) {
    //            return a;
    //        }
    //        return undefined;
    //    });
    //}

    setCache(_list, cacheHospital);
    cacheHospitalShow = _list;
}

//搜索TA
function searchTA(keyword, callback) {
    var list = iPath.Where(cacheTA, function (a) {
        if (a.Name != undefined && a.Name.indexOf(keyword) != -1) {
            return a;
        }
    });
    callback(list);
}

// 搜索省
function searchProvince(keyword, callback) {
    var list = iPath.Where(cacheProvince, function (a) {
        if (a.Name.indexOf(keyword) != -1
            || a.PinYin.indexOf(';' + keyword) != -1) {
            return a;
        }
        return undefined;
    });
    callback(list);
}

// 搜索市
function searchCity(keyword, callback) {
    var list = iPath.Where(cacheCity, function (a) {
        if (a.Name.indexOf(keyword) != -1
            || a.PinYin.indexOf(keyword) != -1) {
            return a;
        }
        return undefined;
    });
    callback(list);
}

// 搜索医院
function searchHospital(cityid, keyword, callback) {
    province = $('#ProvinceId').val();
    if (province == '') {
        province = -1;
    }
    var market = $('#Market').val();
    var TA = $('#TA').val();
    post('/P/Food/SearchHospital', {
        keyword: keyword,
        city: cityid,
        province: province,
        market: market,
        userid: currentUserId,
        TA: TA
    },
        function (d) {
            if (d.rows != undefined && d.rows != null && d.rows.length > 0) {
                //if (IsOutSideHT) {
                //    // 可以使用院外会，开放全部数据
                //} else {
                //    // 不可以使用院外会，过滤院外会数据
                //    var _rows = iPath.Where(d.rows, function (a) {
                //        if (a.External == 0) {
                //            return a;
                //        }
                //        return undefined;
                //    });
                //    d.rows = _rows;
                //}

                setCacheHospital(d.rows);
            }

            if (cityid != -1) {
                var list = iPath.Where(d.rows, function (a) {
                    if (a.CityId == cityid) {
                        return a;
                    }
                    return undefined;
                });
                var _list = iPath.Where(d.rows, function (a) {
                    if (a.CityId != cityid) {
                        return a;
                    }
                    return undefined;
                });

                list = list.concat(_list);
                callback(list);
            } else {
                callback(d.rows);
            }

        }, 'json');
}


function searchHospitalByCode(cityid, keyword, callback) {
    province = $('#ProvinceId').val();
    if (province == '') {
        province = -1;
    }
    var market = $('#Market').val();
    var TA = $('#TA').val();
    post('/P/Food/SearchHospitalByCode', {
        keyword: keyword,
        city: cityid,
        province: province,
        market: market,
        userid: currentUserId,
        TA: TA
    },
        function (d) {
            if (d.rows != undefined && d.rows != null && d.rows.length > 0) {
                //if (IsOutSideHT) {
                //    // 可以使用院外会，开放全部数据
                //} else {
                //    // 不可以使用院外会，过滤院外会数据
                //    var _rows = iPath.Where(d.rows, function (a) {
                //        if (a.External == 0) {
                //            return a;
                //        }
                //        return undefined;
                //    });
                //    d.rows = _rows;
                //}

                setCacheHospital(d.rows);
            }

            if (cityid != -1) {
                var list = iPath.Where(d.rows, function (a) {
                    if (a.CityId == cityid) {
                        return a;
                    }
                    return undefined;
                });
                var _list = iPath.Where(d.rows, function (a) {
                    if (a.CityId != cityid) {
                        return a;
                    }
                    return undefined;
                });

                list = list.concat(_list);
                callback(list);
            } else {
                callback(d.rows);
            }

        }, 'json');
}

// 校验页面表单
function checkData() {
    var hospitalId = $('#HospitalId').val();

    if (hospitalId == undefined || hospitalId == '') {
        $('#HospitalPanel').removeClass('weui-cell_select');
        $('#HospitalPanel .weui-cell__ft').show();
    } else {
        $('#HospitalPanel').addClass('weui-cell_select');
        $('#HospitalPanel .weui-cell__ft').hide();
    }
}

// 校验页面表单
function submitCheckData(state) {
    var hospitalId = $('#HospitalId').val();
    var hospitalAddress = $('#Address').val();   //医院地址

    checkData();

    if (hospitalId == undefined || hospitalId == '') {
        showTopMsg(MSG_NEEDCHOOSEHOSPITAL);
        return false;
    }

    //医院地址
    if (hospitalAddress == undefined || hospitalAddress == '') {
        showTopMsg(MSG_NEEDCHOOSEHOSPITALADDRESS);
        return false;
    }

    //if (IsServPause) {
    //    showDlg(MSG_USERINBLACKNAMELIST);
    //    return false;
    //}

    if (state == 1) {
        if (!isInTimespan(getTimeNow(), timeConfig.workBegin, timeConfig.workEnd)) {
            showDlg(MSG_NOWORKINGTIME);
            return false;
        }
    }

    return {
        market: $('#Market').val(),
        province: $('#ProvinceId').val(),
        provinceName: $('#ProvinceName').val(),
        city: $('#CityId').val(),
        cityName: $('#CityName').val(),
        hospital: $('#HospitalId').val(),
        hospitalName: $('#HospitalName').val(),
        address: $('#Address').val(),
        addressCode: $('#AddressCode').val(),
        code: $('#HospitalCode').val()
    };
}

// 判断医院是否上线
function checkHospital(successfn) {
    var hospitalId = $('#AddressCode').val();
    post('/P/Restaurant/LoadRestaurant', { hospitalId: hospitalId }, function (d) {
        if (d.rows == undefined || d.rows == null || d.rows.length < 1) {
            showDlg(MSG_NORESTAURANT, '确定', function () {
            }, 'info');
        } else {
            successfn();

        }

    }, 'json');
}

function unique1(list) {
    var r = [];
    for (var i = 0, l = list.length; i < l; i++) {
        var flag = true;
        for (var j = i + 1; j < l; j++) {
            if (list[i].GskHospital == list[j].GskHospital) {
                //j == ++i;
                flag = false;
            }
        }
        if (flag) {
            r.push(list[i]);
        }

    }
    return r;
}


var nowOrderInfo;

$(function () {

    var first = $('#ProvinceId').val();
    if (first == '') {
        if (_Market != '') {
            $('#Market').val(_Market);
            $('#TA').val('');
            post('/P/Food/LoadTA', { market: _Market }, function (d) {
                $.each(d.rows, function (i, n) {
                    var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                    $('#TA').append(_content);

                    //$('#TA').val(n.Name);
                });
                cacheTA = new Array();
                setCacheTA(d.rows);
            }, 'json');
        } else {
            showDlg(MSG_MarketNone);
            return;
        }
    } else {
        if (_Market != '') {
            //$('#TA').val('');
            post('/P/Food/LoadTA', { market: _Market }, function (d) {
                $.each(d.rows, function (i, n) {
                    var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                    $('#TA').append(_content);

                    //$('#TA').val(n.Name);
                });
                cacheTA = new Array();
                setCacheTA(d.rows);
            }, 'json');
        }
    }

    $('#Market').change(function () {
        $('#TA').val('');
        post('/P/Food/LoadTA', { market: market }, function (d) {
            $.each(d.rows, function (i, n) {
                var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                $('#TA').append(_content);
            });
            cacheTA = new Array();
            setCacheTA(d.rows);
        }, 'json');

        var cityId = $('#CityId').val();

        if (cityId == '') {
            return;
        }

        $('#HospitalName').val('');
        $('#HospitalId').val('');
        $('#Address').val('');
        $('#AddressCode').val('');
        $('#HospitalCode').val('');

        var market = $('#Market').val();
        post('/P/Food/LoadHospital', { cityId: cityId, market: market, TA: $('#TA').val() }, function (d) {
            cacheHospital = new Array();
            setCacheHospital(d.rows);
        }, 'json');

    });

    $('#TA').change(function () {
        var market = $('#Market').val();
        var ta = $('#TA').val();
        var hosCode = $('#HospitalCode').val();

        if (hosCode != "") {
            post('/P/PreApproval/LoadHospital', {
                cityId: -1, market: market, userid: currentUserId, TA: ta
            }, function (d) {
                var isHos = false;

                $.each(d.rows, function (i, n) {
                    if (n.HospitalCode == hosCode) {
                        isHos = true;
                    }
                });

                if (!isHos) {
                    $('#ProvinceId').val('');
                    $('#ProvinceName').val('');
                    $('#CityId').val('');
                    $('#CityName').val('');
                    $('#HospitalName').val('');
                    $('#HospitalId').val('');
                    $('#Address').val('');
                    $('#AddressCode').val('');
                    $('#HospitalCode').val('');
                }
            }, 'json');
        }

    });

    $('#TA').click(function () {
        var market = $('#Market').val();
        var hosCode = $('#HospitalCode').val();
        if (market == -1) {
            showTopMsg(MSG_Market);
            return;
        }

        chooser.Show(
            function (keyword, callback) {
                searchTA(keyword, function (list) {
                    var res = iPath.Select(list, function (a) {
                        return { val: a.Name };
                    });
                    callback(res);
                })
            },
            function (opt) {
                $('#TA').val(opt.val);

                var TA = opt.val;

                post('/P/Food/LoadProvince', { Type: market, TA: TA }, function (d) {
                    cacheProvince = new Array();
                    setCacheProvince(d.rows);
                }, 'json');

                var cityId = $('#CityId').val();

                if (cityId == '') {
                    return;
                }

                post('/P/Food/LoadHospital', {
                    cityId: cityId, market: market, userid: currentUserId, TA: TA
                }, function (d) {
                    var isHos = false;
                    //var isCity = false;
                    //var isProvince = false;
                    var hosCode = $('#HospitalCode').val();
                    //var provinceId = $('#ProvinceId').val();
                    //var cityId = $('#CityId').val();

                    $.each(d.rows, function (i, n) {
                        if (n.HospitalCode == hosCode) {
                            isHos = true;
                        }
                        //if (n.CityId == cityId) {
                        //    isCity = true;
                        //}
                        //if (n.ProvinceId == provinceId) {
                        //    isProvince = true;
                        //}
                    });

                    if (!isHos || hosCode == "") {
                        $('#HospitalName').val('');
                        $('#HospitalId').val('');
                        $('#Address').val('');
                        $('#AddressCode').val('');
                        $('#HospitalCode').val('');
                        $('#CityId').val('');
                        $('#CityName').val('');
                        $('#ProvinceId').val('');
                        $('#ProvinceName').val('');
                    }

                    //if (!isCity) {
                    //    $('#CityId').val('');
                    //    $('#CityName').val('');
                    //} 

                    //if (!isProvince) {
                    //    $('#ProvinceId').val('');
                    //    $('#ProvinceName').val('');
                    //} else {
                    //    post('/P/PreApproval/LoadCity', { provinceId: $('#ProvinceId').val(), Type: market, TA: opt.val }, function (d) {
                    //        cacheCity = new Array();
                    //        setCacheCity(d.rows);
                    //    }, 'json');
                    //}
                }, 'json');
            });
    });

    post('/P/Food/NowOrderInfo', {}, function (d) {
        nowOrderInfo = d.data;
        if (nowOrderInfo != undefined && nowOrderInfo != null && nowOrderInfo.hospital != undefined && nowOrderInfo.hospital != null) {
            var hospital = nowOrderInfo.hospital;
            $('#ProvinceId').val(hospital.province);
            $('#ProvinceName').val(hospital.provinceName);
            $('#CityId').val(hospital.city);
            $('#CityName').val(hospital.cityName);
            $('#HospitalName').val(hospital.hospitalName);
            $('#HospitalId').val(hospital.hospital);
            $('#Address').val(hospital.address);
            $('#HospitalCode').val(hospital.code);

            $('#Market').val(hospital.market);
            post('/P/Food/LoadCity', { provinceId: hospital.province, Type: hospital.market }, function (d) {
                setCacheCity(d.rows);
            }, 'json');
            post('/P/Food/LoadHospital', { cityId: hospital.city, market: hospital.market }, function (d) {
                setCacheHospital(d.rows);
            }, 'json');
        }

    }, 'json');

    post('/P/Food/LoadProvince', { Type: $('#Market').val(), TA: $('#TA').val() }, function (d) {
        setCacheProvince(d.rows);
    }, 'json');

    $('#ProvinceName').click(function () {
        var market = $('#Market').val();
        var TA = $('#TA').val();
        if (market == -1) {
            showTopMsg(MSG_Market);
            return;
        }

        chooser.Show(
            function (keyword, callback) {
                searchProvince(keyword, function (list) {
                    var res = iPath.Select(list, function (a) {
                        return { key: a.ID, val: a.Name, ext: a.PinYin };
                    });
                    callback(res);
                })
            },
            function (opt) {
                $('#ProvinceId').val(opt.key);
                $('#ProvinceName').val(opt.val);

                $('#CityId').val('');
                $('#CityName').val('');
                $('#HospitalName').val('');
                $('#HospitalId').val('');
                $('#Address').val('');
                $('#AddressCode').val('');
                $('#HospitalCode').val('');

                post('/P/Food/LoadCity', { provinceId: opt.key, Type: market,TA:TA }, function (d) {
                    setCacheCity(d.rows);
                }, 'json');

            });
    });

    $('#CityName').click(function () {
        var provinceId = $('#ProvinceId').val();
        if (provinceId == '') {
            showTopMsg(MSG_NEEDCHOOSEPROVINCE);
            return;
        }

        chooser.Show(
            function (keyword, callback) {
                searchCity(keyword, function (list) {
                    var _list = iPath.Where(list, function (a) {
                        if (a.ProvinceId == provinceId) {
                            return a;
                        }
                        return undefined;
                    });
                    var res = iPath.Select(_list, function (a) {
                        return { key: a.ID, val: a.Name, ext: a.PinYin };
                    });
                    callback(res);
                });
            },
            function (opt) {
                $('#CityId').val(opt.key);
                $('#CityName').val(opt.val);

                $('#HospitalName').val('');
                $('#HospitalId').val('');
                $('#Address').val('');
                $('#AddressCode').val('');
                $('#HospitalCode').val('');

                var market = $('#Market').val();
                var TA = $('#TA').val();
                post('/P/Food/LoadHospital', { cityId: opt.key, market: market, userid: currentUserId, TA: TA }, function (d) {
                    cacheHospital = new Array();
                    setCacheHospital(d.rows);
                }, 'json');
            });

    });

    $('#HospitalName').click(function () {
        var market = $('#Market').val();
        if (market == -1) {
            showTopMsg(MSG_Market);
            return;
        }

        //var cityName = $('#CityName').val();
        //if (cityName == '') {
        //    showTopMsg(MSG_CityName);
        //    return;
        //}

        var cityId = $('#CityId').val();
        cityId = cityId == '' ? -1 : cityId * 1;
        chooser.Show(
            function (keyword, callback) {
                if (keyword == '' || keyword == undefined || keyword == null) {
                    var list = iPath.Where(cacheHospital, function (a) {
                        if (a.CityId == cityId) {
                            return a;
                        }
                        return undefined;
                    });
                    if (list.length > 1) {
                        list = unique1(list);
                    }
                    var res = iPath.Select(list, function (a) {
                        return { key: a.ID, val: a.Name, ext: a.Address, hco: a.GskHospital };
                    });
                    callback(res);
                } else {
                    searchHospital(cityId, keyword, function (list) {
                        if (list.length > 1) {
                            list = unique1(list);
                        }
                        var res = iPath.Select(list, function (a) {
                            return { key: a.ID, val: a.Name, ext: a.Address, hco: a.GskHospital };
                        });
                        callback(res);
                    });
                }

            },
            function (opt) {
                $('#HospitalName').val(opt.val);
                $('#HospitalId').val(opt.hco);
                $('#Address').val('');
                $('#AddressCode').val('');
                $('#HospitalCode').val(opt.hco);

                var _cityId = cacheHospital[opt.key].CityId;
                post('/P/Food/FindCity', { cityId: _cityId }, function (d) {

                    $('#CityId').val(d.data.ID);
                    $('#CityName').val(d.data.Name);
                    var province = cacheProvince[d.data.ProvinceId];
                    $('#ProvinceId').val(province.ID);
                    $('#ProvinceName').val(province.Name);

                }, 'json');
                checkData();
            });
    });

    $('#HospitalCode').click(function () {
        var market = $('#Market').val();
        if (market == -1) {
            showTopMsg(MSG_Market);
            return;
        }
        var cityId = $('#CityId').val();
        cityId = cityId == '' ? -1 : cityId * 1;
        chooser.Show(
            function (keyword, callback) {
                if (keyword == '' || keyword == undefined || keyword == null) {
                    var list = iPath.Where(cacheHospital, function (a) {
                        if (a.CityId == cityId) {
                            return a;
                        }
                        return undefined;
                    });
                    if (list.length > 1) {
                        list = unique1(list);
                    }
                    var res = iPath.Select(list, function (a) {
                        return { key: a.ID, val: a.GskHospital, ext: a.Address, hco: a.Name };
                    });
                    callback(res);
                } else {
                    searchHospitalByCode(cityId, keyword, function (list) {
                        if (list.length > 1) {
                            list = unique1(list);
                        }
                        var res = iPath.Select(list, function (a) {
                            return { key: a.ID, val: a.GskHospital, ext: a.Address, hco: a.Name };
                        });
                        callback(res);
                    });
                }
            },
            function (opt) {
                $('#HospitalName').val(opt.hco);
                //$("HospitalName").height($("HospitalName")[0].scrollHeight);
                $('#HospitalCode').val(opt.val);
                $('#HospitalId').val(opt.val);
                $('#Address').val('');
                $('#AddressCode').val('');
                var _cityId = cacheHospital[opt.key].CityId;
                post('/P/Food/FindCity', { cityId: _cityId }, function (d) {
                    $('#CityId').val(d.data.ID);
                    $('#CityName').val(d.data.Name);
                    var province = cacheProvince[d.data.ProvinceId];
                    $('#ProvinceId').val(province.ID);
                    $('#ProvinceName').val(province.Name);
                }, 'json');
            });
    });

    $('#Address').click(function () {
        var hospitalCode = $('#HospitalCode').val();
        if (hospitalCode == '' || hospitalCode == null || hospitalCode == undefined) {
            showTopMsg('请选择医院编码');
            return;
        }

        chooser.Show(
            function (keyword, callback) {
                if (keyword == '' || keyword == undefined || keyword == null) {
                    var list = iPath.Where(cacheHospitalShow, function (a) {
                        if (a.GskHospital == hospitalCode) {
                            return a;
                        }
                        return undefined;
                    });


                    var res = iPath.Select(list, function (a) {
                        return { key: a.ID, val: a.GskHospital == a.HospitalCode ? a.Address : a.MainAddress + ':' + a.Address, ext: a.GskHospital, hco: a.Name, del: a.IsDelete };
                    });
                    callback(res);
                } else {
                    searchHospitalByCode(cityId, keyword, function (list) {
                        var res = iPath.Select(list, function (a) {
                            return { key: a.ID, val: a.GskHospital == a.HospitalCode ? a.Address : a.MainAddress + ':' + a.Address, ext: a.GskHospital, hco: a.Name, del: a.IsDelete };
                        });
                        callback(res);
                    });
                }
            },
            function (opt) {
                $('#HospitalName').val(opt.hco);
                //$("HospitalName").height($("HospitalName")[0].scrollHeight);
                $('#HospitalCode').val(opt.ext);
                $('#HospitalId').val(opt.ext);
                $('#Address').val(opt.val);
                var _cityId = cacheHospital[opt.key].CityId;
                post('/P/Food/FindCity', { cityId: _cityId }, function (d) {
                    $('#CityId').val(d.data.ID);
                    $('#CityName').val(d.data.Name);
                    var province = cacheProvince[d.data.ProvinceId];
                    $('#ProvinceId').val(province.ID);
                    $('#ProvinceName').val(province.Name);
                }, 'json');

                var _hospitalCode = cacheHospital[opt.key].HospitalCode;
                $('#AddressCode').val(_hospitalCode);
            });

    });

    // 我要订餐
    $('#btnChooseCN').click(function () {
        var p = submitCheckData(1);
        if (p == false) {
            return;
        }

        // 判断
        checkHospital(function () {
            post('/P/Food/ChooseHospital', p,
            function (d) {
                $('#form0').attr('action', $('#form0').attr('_action1'));
                $('#form0').submit();
            }, 'json');
        });

    });

    // 浏览餐厅
    $('#btnRestaurant').click(function () {
        var p = submitCheckData(2);
        if (p == false) {
            return;
        }
        checkHospital(function () {
            post('/P/Food/ChooseHospital', p,
                function (d) {
                    $('#form0').attr('action', $('#form0').attr('_action2'));
                    $('#form0').submit();
                }, 'json');
        });

    });


});

