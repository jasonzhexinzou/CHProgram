var cacheTA = new Array();
var cacheProvince = new Array();
var cacheCity = new Array();
var cacheHospital = new Array();
var cacheCostCenter = new Array();
var renderdelivertime;
var isBindRes = true;
var cacheHospitalShow = new Array();

// 保存缓存
function setCache(list, cache) {
    for (var i in list) {
        var item = list[i];
        cache[item.ID] = item;
    }
}

//保存TA缓存
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
    if (IsOutSideHT) {
        // 允许使用院外会，开放全部数据
        _list = list;
    } else {
        // 不允许使用院外会，屏蔽院外会数据
        _list = iPath.Where(list, function (a) {
            if (a.External == 0) {
                return a;
            }
            return undefined;
        });
    }

    setCache(_list, cacheHospital);
    cacheHospitalShow = _list;
}

//保存成本中心缓存
function setCacheCostCenter(list) {
    //var i = 0;
    //for (var t in list) {
    //    var item = list[t];
    //    cacheCostCenter[i] = item;
    //    i++;
    //}
    for (var i = 0; i < list.length; i++) {
        var item = list[i];
        cacheCostCenter[i] = item;
        if (i == list.length) {
            i--;
        }
    }
}

//搜索成本中心
function searchCostCenter(keyword, callback) {
    var list = iPath.Where(cacheCostCenter, function (a) {
        if (a.Name != undefined && a.Name.indexOf(keyword) != -1) {
            return a;
        }
        return undefined;
    });
    callback(list);
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
        if (a.Name != undefined && (a.Name.indexOf(keyword) != -1
            || a.PinYin.indexOf(';' + keyword) != -1)) {
            return a;
        }
        return undefined;
    });
    callback(list);
}

// 搜索市
function searchCity(keyword, callback) {
    var list = iPath.Where(cacheCity, function (a) {
        if (a.Name != undefined && (a.Name.indexOf(keyword) != -1
            || a.PinYin.indexOf(keyword) != -1)) {
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
    post('/P/PreApproval/SearchHospital', {
        keyword: keyword,
        city: cityid,
        province: province,
        market: market,
        userid: currentUserId,
        TA: TA
    },
        function (d) {
            if (d.rows != undefined && d.rows != null && d.rows.length > 0) {
                if (IsOutSideHT) {
                //    // 可以使用院外会，开放全部数据
                } else {
                    // 不可以使用院外会，过滤院外会数据
                    var _rows = iPath.Where(d.rows, function (a) {
                        if (a.External == 0) {
                            return a;
                        }
                        return undefined;
                    });
                    d.rows = _rows;
                }

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

// 根据医院编码搜索医院
function searchHospitalByCode(cityid, keyword, callback) {
    province = $('#ProvinceId').val();
    if (province == '') {
        province = -1;
    }
    var market = $('#Market').val();
    var TA = $('#TA').val();
    post('/P/PreApproval/SearchHospitalByCode', {
        keyword: keyword,
        city: cityid,
        province: province,
        market: market,
        userid: currentUserId,
        TA: TA
    },
        function (d) {
            if (d.rows != undefined && d.rows != null && d.rows.length > 0) {
                if (IsOutSideHT) {
                //    // 可以使用院外会，开放全部数据
                } else {
                    // 不可以使用院外会，过滤院外会数据
                    var _rows = iPath.Where(d.rows, function (a) {
                        if (a.External == 0) {
                            return a;
                        }
                        return undefined;
                    });
                    d.rows = _rows;
                }

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

//显示会议时间
function showDelivertime(deliverTime) {
    //年份列表
    var listyear = [];
    //月份列表
    var listmonth = [];
    //当前年份
    var yearNow = new Date().getFullYear();
    //当前月份
    var monthNow = new Date().getMonth() + 1;
    //默认选择当前年
    for (var i = yearNow; i <= yearNow + 1; i++) {
        listyear.push(i);
    }
    //默认选择当前月
    for (var i = monthNow; i <= 12; i++) {
        listmonth.push(i);
    }
    deliverTimePicker.init('chooseDeliverTime', deliverTime, listyear, listmonth);
}

// 从这里开始，是选择会议时间
var deliverTimePicker = (function () {
    var _pickerDomId;
    var init = function (pickerDomId, deliverTime, listyear, listmonth, yearNow, monthNow) {
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

        var html = renderdelivertime({ delivertime: days, years: listyear, months: listmonth, yearNow: yearNow, monthNow: monthNow });
        $('#chooseDeliverTime').html(html);

        _pickerDomId = pickerDomId;

        var dyaswidth = $('#' + _pickerDomId + ' .days>.day').length * 90;
        $('#' + _pickerDomId + ' .days').css('width', dyaswidth + 'px');
        $('#' + _pickerDomId + ' .days>.day').click(function () {
            $('#' + _pickerDomId + ' .days>.day').removeClass('checked');
            $(this).addClass('checked');
            var checkedDay = $(this).attr('_val');
            $('#' + _pickerDomId + ' .hour_body>div').hide();
            $('#' + _pickerDomId + ' .choosehour').removeClass('checked');
            $('#' + _pickerDomId + ' .time_body>div').hide();
            $('#' + _pickerDomId + ' .hour_body>div[_val="' + checkedDay + '"]').show();
            //$('#' + _pickerDomId + ' .hour_body>div[_val="' + checkedDay + '"]>div:eq(0)').click();
            $('#' + _pickerDomId + ' .hour_body')[0].scrollTop = 0;
        });

        $('#' + _pickerDomId + ' .choosehour').click(function () {
            var checkedDay = $(this).attr('_val');
            $('#' + _pickerDomId + ' .time_body>div').hide();
            $('#' + _pickerDomId + ' div[_val="' + checkedDay + '"]').show();
            $('#' + _pickerDomId + ' .time_body')[0].scrollTop = 0;

            $('#' + _pickerDomId + ' .choosehour').removeClass('checked');
            $(this).addClass('checked');
        });

        $('#' + _pickerDomId + ' .years select').change(function () {
            var selectedYear = $(this).val();
            var listmonth = [];
            $('#' + _pickerDomId + ' .months select').html("");
            //当前年
            if (selectedYear == yearNow) {
                for (var i = monthNow; i <= 12; i++) {
                    listmonth.push(i);
                }
            }
            else {
                for (var i = 1; i <= 12; i++) {
                    listmonth.push(i);
                }
            }
            $.each(listmonth, function (i, n) {
                var _content = "<option value='" + n + "'>" + n + "月</option>";
                $('#' + _pickerDomId + ' .months select').append(_content);
            });
            $('#' + _pickerDomId + ' .months').change();
        });

        $('#' + _pickerDomId + ' .months').change(function () {
            var dateNow = new Date();
            var selectedYear = $('#' + _pickerDomId + ' .years select').val();
            var selectedMonth = $('#' + _pickerDomId + ' .months select').val();
            //年份列表
            var listyear = [];
            //月份列表
            var listmonth = [];
            //当前年份
            var yearNow = new Date().getFullYear();
            //当前月份
            var monthNow = new Date().getMonth() + 1;
            //默认选择当前年
            for (var i = yearNow; i <= yearNow + 1; i++) {
                listyear.push(i);
            }
            if (selectedYear == yearNow) {
                for (var i = monthNow; i <= 12; i++) {
                    listmonth.push(i);
                }
            }
            else {
                for (var i = 1; i <= 12; i++) {
                    listmonth.push(i);
                }
            }
            post('/P/Food/LoadNextHoliday', {},
                function (d) {
                    var minDate;
                    var currentStartDate;
                    var holiday = d.data.holiday;
                    holiday.StartDay = getDateByDotNet(holiday.StartDay);
                    holiday.EndDay = getDateByDotNet(holiday.EndDay);
                    var startDay = getDateByDotNet(d.data.now);
                    var endDay;

                    // 判断今天在那个区间1.非最后一个工作日 2.最后一个工作日 3.休息日
                    // 规则1.可以定到下一天 2、3.可以订到休假日后第二个工作日
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
                    var deliverTime = new Array();
                    //if (rangeType == 1) {
                    //    if (isInTimespan(getTimeNow(), timeConfig.preNextBegin, timeConfig.preNextEnd)) {
                    //        minDate = new Date(startDay.getTime() + oneDay).pattern('yyyy-MM-dd');
                    //    }
                    //    else {
                    //        minDate = new Date(startDay.getTime() + 2 * oneDay).pattern('yyyy-MM-dd');
                    //    }
                    //}
                    //else if (rangeType == 2) {
                    //    if (isInTimespan(getTimeNow(), timeConfig.preNextBegin, timeConfig.preNextEnd)) {
                    //        minDate = new Date(startDay.getTime() + oneDay).pattern('yyyy-MM-dd');
                    //    }
                    //    else {
                    //        for (var i = startDay.getTime(); i <= (holiday.EndDay.getTime() + oneDay); i += oneDay) {
                    //            minDate = new Date(holiday.EndDay.getTime() + oneDay * 2).pattern('yyyy-MM-dd');
                    //        }
                    //    }
                    //}
                    //else {
                    //    minDate = new Date(holiday.EndDay.getTime() + oneDay * 2).pattern('yyyy-MM-dd');
                    //}
                    if (selectedMonth < 10) {
                        currentStartDate = new Date(selectedYear + "-0" + selectedMonth + "-01").pattern('yyyy-MM-dd')
                    }
                    else {
                        currentStartDate = new Date(selectedYear + "-" + selectedMonth + "-01").pattern('yyyy-MM-dd')
                    }
                    if (new Date(minDate).pattern('yyyy-MM-dd') < currentStartDate) {
                        minDate = currentStartDate;
                    }
                    var i = minDate;
                    while (new Date(i).getMonth() == new Date(minDate).getMonth()) {
                        deliverRange.push(i);
                        i = new Date(new Date(i).getTime() + oneDay).pattern('yyyy-MM-dd');
                    }

                    for (var i = 0; i < deliverRange.length; i++) {
                        var _time = timeConfig.preApprovalBegin;
                        var _weekday = new Date(deliverRange[i].replace(/-/g, '/')).pattern('E');
                        for (; isTimeInTimespan(_time, timeConfig.preApprovalBegin, timeConfig.preApprovalEnd);) {
                            deliverTime.push({
                                time: deliverRange[i] + ' ' + _time,
                                weekday: _weekday
                            });
                            _time = timeAdd(_time, timeConfig.cachetime);
                        }
                    }
                    deliverTimePicker.init('chooseDeliverTime', deliverTime, listyear, listmonth, selectedYear, selectedMonth);
                });
        });

        $('#' + _pickerDomId + ' .body>div .chooseTime').click(function () {
            var time = $(this).attr('_val');
            $('#MeetingTime').val(time.substr(0, time.length - 3));
            $('#MeetingTime').attr('_val', time);
            // 清除红色叹号样式
            hidecell_warn('deliverTime');

            //$('#page_order').show();
            $('#chooseDeliverTime').hide();
        });

        //$('#' + _pickerDomId + ' .body>div').hide();

        //$('#page_order').hide();
        $('#' + _pickerDomId).show();

        //$('#' + _pickerDomId + ' .day:eq(0)').click();
        $('#chooseDeliverTime .close').click(function () {
            $('#chooseDeliverTime').hide();
        });
    };

    return {
        init: init
    }
})();

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

// 校验页面表单
function checkData() {
    var ta = $('#TA').val();  //TA
    if (ta == undefined || ta == '') {
        $('#TAPanel').removeClass('weui-cell_select');
        $('#TAPanel .weui-cell__ft').show();
        return;
    } else {
        $('#TAPanel').addClass('weui-cell_select');
        $('#TAPanel .weui-cell__ft').hide();
    }

    var provinceName = $('#ProvinceName').val();  //省份
    if (provinceName == undefined || provinceName == '') {
        $('#ProvincePanel').removeClass('weui-cell_select');
        $('#ProvincePanel .weui-cell__ft').show();
        return;
    } else {
        $('#ProvincePanel').addClass('weui-cell_select');
        $('#ProvincePanel .weui-cell__ft').hide();
    }

    var cityName = $('#CityName').val();  //城市
    if (cityName == undefined || cityName == '') {
        $('#CityPanel').removeClass('weui-cell_select');
        $('#CityPanel .weui-cell__ft').show();
        return;
    } else {
        $('#CityPanel').addClass('weui-cell_select');
        $('#CityPanel .weui-cell__ft').hide();
    }

    var hospitalId = $('#HospitalId').val();
    if (hospitalId == undefined || hospitalId == '') {
        $('#HospitalPanel').removeClass('weui-cell_select');
        $('#HospitalPanel .weui-cell__ft').show();
        return;
    } else {
        $('#HospitalPanel').addClass('weui-cell_select');
        $('#HospitalPanel .weui-cell__ft').hide();
    }

    var meetingTitle = $('#MeetingTitle').val();  //会议名称
    if (meetingTitle == undefined || meetingTitle == '') {
        $('#MeetingTitlePanel').removeClass('weui-cell_select');
        $('#MeetingTitlePanel .weui-cell__ft').show();
        return;
    } else {
        //$('#MeetingTitlePanel').addClass('weui-cell_select');
        $('#MeetingTitlePanel .weui-cell__ft').hide();
    }

    var meetingDate = $('#MeetingDate').val();  //会议日期
    if (meetingDate == undefined || meetingDate == '') {
        $('#MeetingDatePanel').removeClass('weui-cell_select');
        $('#MeetingDatePanel .weui-cell__ft').show();
        return;
    } else {
        //$('#MeetingDatePanel').addClass('weui-cell_select');
        $('#MeetingDatePanel .weui-cell__ft').hide();
    }

    var meetingTime = $('#MeetingTime').val();  //会议时间
    if (meetingTime == undefined || meetingTime == '') {
        $('#MeetingTimePanel').removeClass('weui-cell_select');
        $('#MeetingTimePanel .weui-cell__ft').show();
        return;
    } else {
        //$('#MeetingTimePanel').addClass('weui-cell_select');
        $('#MeetingTimePanel .weui-cell__ft').hide();
    }

    var attendance = $('#Attendance').val();  //参会人数
    if (attendance == undefined || meetingTime == '') {
        $('#AttendancePanel').removeClass('weui-cell_select');
        $('#AttendancePanel .weui-cell__ft').show();
        return;
    } else {
        //$('#AttendancePanel').addClass('weui-cell_select');
        $('#AttendancePanel .weui-cell__ft').hide();
    }
    //20190115
    var veevaMeetingId = $('#VeevaMeetingID').val();  //VeevaMeetingID
    if (veevaMeetingId == undefined || veevaMeetingId == '') {
        $('#VeevaMeetingPanel').removeClass('weui-cell_select');
        $('#VeevaMeetingPanel .weui-cell__ft').show();
        return;
    } else {
        //$('#AttendancePanel').addClass('weui-cell_select');
        $('#VeevaMeetingPanel .weui-cell__ft').hide();
    }
    var costCenter = $('#CostCenter').val();  //成本中心
    if (costCenter == undefined || costCenter == '') {
        $('#CostCenterPanel').removeClass('weui-cell_select');
        $('#CostCenterPanel .weui-cell__ft').show();
        return;
    } else {
        $('#CostCenterPanel').addClass('weui-cell_select');
        $('#CostCenterPanel .weui-cell__ft').hide();
    }

    var budget = delcommafy($('#Budget').val());  //预算金额
    if (budget == undefined || budget == '') {
        $('#BudgetPanel').removeClass('weui-cell_select');
        $('#BudgetPanel .weui-cell__ft').show();
        return;
    } else {
        //$('#BudgetPanel').addClass('weui-cell_select');
        $('#BudgetPanel .weui-cell__ft').hide();
    }

    var followVisit = $('#FollowVisit').val();  //直线经理是否随访
    if (followVisit == undefined || followVisit == '') {
        $('#FollowVisitPanel').removeClass('weui-cell_select');
        $('#FollowVisitPanel .weui-cell__ft').show();
        return;
    } else {
        //$('#FollowVisitPanel').addClass('weui-cell_select');
        $('#FollowVisitPanel .weui-cell__ft').hide();
    }
}




// 校验页面表单
function submitCheckData(state) {
    var market = $('#Market').val();
    var ta = $('#TA').val();  //TA
    var veevaMeetingId = $('#VeevaMeetingID').val()//VeevaMeetingID
    var provinceName = $('#ProvinceName').val();  //省份
    var cityName = $('#CityName').val();  //城市
    var hospitalId = $('#HospitalId').val();
    var hospitalName = $('#HospitalName').val();  //医院名称
    var hospitalCodee = $('#HospitalCode').val();  //医院编码
    var meetingTitle = $('#MeetingTitle').val();  //会议名称
    var meetingTime = $('#MeetingTime').val();  //会议时间
    var attendance = $('#Attendance').val();  //参会人数
    var costCenter = $('#CostCenter').val();  //成本中心
    var budget = delcommafy($('#Budget').val());  //预算金额
    var followVisit = $('#FollowVisit').val();  //直线经理是否随访
    var visitOne = $('#VisitOne').val();  //直线经理是否随访
    var visitTwo = $('#VisitTwo').val();  //直线经理是否随访
    var _images = new Array();
    var _images2 = new Array();
    isBindRes = true; //IsBindRes

    var hospitalAddress = $('#Address').val();   //医院地址

    $('#chooseUploadServiceImage img').each(function () {
        var src = $(this).attr('_src');
        _images.push(src);
    });
    $('#chooseUploadBenefitImage img').each(function () {
        var src = $(this).attr('_src');
        _images2.push(src);
    });

    //是否在工作时间
    //if (state == 1) {
    //    if (!isInTimespan(getTimeNow(), timeConfig.PreWorkTimeSpanBegin, timeAdd(timeConfig.PreWorkTimeSpanEnd, timeConfig.cachetime))) {
    //        showDlg(MSG_NOPREAPPROVALWORKINGTIME);
    //        return false;
    //    }
    //}

    //TA
    if (ta == undefined || ta == '') {
        showTopMsg(MSG_TA);
        return false;
    }
    //20190115
    if (market == 'Rx' || market == 'Vx') {
        //VeevaMeetingID
        if (veevaMeetingId == undefined || veevaMeetingId == '') {
            showTopMsg(MSG_VEEVAMEETINGIDNULL);

            return false;
        }
        else if (veevaMeetingId.length != 8) {
            showTopMsg(MSG_VEEVAMEETINGID);

            return false;
        }
    }
    //省份
    if (provinceName == undefined || provinceName == '') {
        showTopMsg(MSG_PROVINCE);
        return false;
    }

    //城市
    if (cityName == undefined || cityName == '') {
        showTopMsg(MSG_CITY);
        return false;
    }

    //医院名称
    if (hospitalName == undefined || hospitalName == '') {
        showTopMsg(MSG_HOSPITALNAME);
        return false;
    }

    if (hospitalId == undefined || hospitalId == '') {
        showTopMsg(MSG_NEEDCHOOSEHOSPITAL);
        return false;
    }

    //医院地址
    if (hospitalAddress == undefined || hospitalAddress == '') {
        var list = iPath.Where(cacheHospital, function (a) {
            if (a.GskHospital == hospitalCodee) {
                return a;
            }
            return undefined;
        });

        if (list[0].Address != undefined && list[0].Address != "") {
            showTopMsg(MSG_NEEDCHOOSEHOSPITALADDRESS);
            return false;
        } else if (parseFloat(budget) > 0) {
            showTopMsg(MSG_NEEDCHOOSEHOSPITALADDRESS);
            return false;
        } else {
            $("#AddressCode").val($("#HospitalCode").val());
        }
    }

    //会议名称
    if (meetingTitle == undefined || meetingTitle == '') {
        showTopMsg(MSG_CONFERENCETITLE);
        return false;
    }

    if (meetingTitle.length > 50) {
        showTopMsg(MSG_CONFERENCETITLELENGTH);
        return false;
    }

    //会议时间
    if (meetingTime == undefined || meetingTime == '') {
        showTopMsg(MSG_CONFERENCETIME);
        return false;
    }

    //参会人数
    if (attendance == undefined || attendance == '' || attendance <= 0) {
        showTopMsg(MSG_ATTENDANCE);
        return false;
    }

    var reg = /^[0-9]\d*$/;
    if (reg.test(attendance) == false) {
        showTopMsg(MSG_ATTENDANCETYPE);
        return false;
    }

    //成本中心
    if (costCenter == undefined || costCenter == '') {
        showTopMsg(MSG_COSTCENTER);
        return false;
    }

    //预算金额
    if (budget == undefined || budget == '' || budget < 0) {
        showTopMsg(MSG_BUDGET);
        return false;
    } else if (budget > 0) {
        $.ajax({
            async: false,
            type: "post",
            url: contextUri + '/P/Food/IsBindRes',
            data: {
                hospitalId: hospitalId
            },
            datatype: 'json',
            success: function (data) {
                if (data.data == false) {
                    isBindRes = false;
                }
            }
        });
    }

    //DM是否随访
    var infos = $("input:radio[name=FollowVisit]:checked").val();
    if (infos == undefined || infos == '') {
        showTopMsg(MSG_FOLLOWVISIT);
        return false;
    }

    //预申请是否超出人均60元标准
    //if (budget / attendance > 60) {
    //    showTopMsg(MSG_BUDGETINFO);
    //    return false;
    //}

    if ($("#rbSpeaker1").is(":checked") == false && $("#rbSpeaker2").is(":checked") == false) {
        showTopMsg(MSG_NOTSELECTSPEAKERFILE);
        return false;
    }

    //免费讲者
    if ($("#rbSpeaker1").is(":checked") == true) {
        if (_images == "" || _images == null || _images == undefined || _images2 == "" || _images2 == null || _images2 == undefined) {
            showTopMsg(MSG_NOTUPLOADSPEAKERFILE);
            return false;
        }
    }

    //预申请是否超出人均标准
    var _isSubmit = true;
    var showMess = '';
    $.ajax({
        async: false,
        type: "post",
        url: contextUri + '/P/Food/IsSubmit',
        data: {
            hospitalId: hospitalId,
            budget: budget,
            attendance: attendance,
            state: 1
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

    return {
        market: $('#Market').val(),
        ta: $('#TA').val(),
        veevaMeetingId: $('#VeevaMeetingID').val(),
        province: $('#ProvinceId').val(),
        provinceName: $('#ProvinceName').val(),
        city: $('#CityId').val(),
        cityName: $('#CityName').val(),
        hospital: $('#HospitalId').val(),
        hospitalName: $('#HospitalName').val(),
        hospitalCode: $('#HospitalCode').val(),
        meetingTitle: $('#MeetingTitle').val(),
        meetingTime: $('#MeetingTime').val(),
        attendance: $('#Attendance').val(),
        costCenter: $('#CostCenter').val(),
        budget: delcommafy($('#Budget').val()),
        followVisit: infos,
        visitOne: $('#VisitOne').val(),
        visitTwo: $('#VisitTwo').val(),
        SpeakerServiceImage: _images.toString(),
        SpeakerBenefitImage: _images2.toString(),
        IsFreeSpeaker: $("input:radio[name=Speaker]:checked").val(),
        address: $('#Address').val(),
        addressCode: $('#AddressCode').val()
    };

}
//20190115
function VeevaMeetingPanelShowOrHide(market) {

    //$('#Market').val(_Market);
    if (market == 'Rx' || market == 'Vx') {
        $("#VeevaMeetingPanel").show()
    }
    else {
        $("#VeevaMeetingPanel").hide()
    }
}
var nowOrderInfo;
var orderSuccess = false;
$(function () {
    renderdelivertime = template('tmpl_delivertime');
    if (_Market != '') {
        //20190115
        VeevaMeetingPanelShowOrHide(_Market)
        //20190115
        //$('#Market').val(_Market);
        $('#TA').val('');
        post('/P/PreApproval/LoadTA', { market: _Market }, function (d) {
            $.each(d.rows, function (i, n) {
                var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                $('#TA').append(_content);

                $('#TA').val(n.Name);
            });
            if (GroupType != 1) {
                $('#TA').val("");
            }
            cacheTA = new Array();
            setCacheTA(d.rows);
        }, 'json');
    }

    if (_Market != '') {
        $('#CostCenter').val('');
        var TA = $('#TA').val();
        post('/P/PreApproval/LoadCostCenter', { market: _Market, TA: TA }, function (d) {
            $.each(d.rows, function (i, n) {
                var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                $('#CostCenter').append(_content);

                $('#CostCenter').val(n.Name);
            });
            cacheCostCenter = new Array();
            setCacheCostCenter(d.rows);
        }, 'json');
    }

    $('#Market').change(function () {
        var market = $('#Market').val();
        //20190115
        VeevaMeetingPanelShowOrHide(market);
        $('#TA').val('');
        post('/P/PreApproval/LoadTA', { market: market }, function (d) {
            $.each(d.rows, function (i, n) {
                var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                $('#TA').append(_content);
            });
            cacheTA = new Array();
            setCacheTA(d.rows);
        }, 'json');

        post('/P/PreApproval/LoadProvince', { Type: $('#Market').val() }, function (d) {
            setCacheProvince(d.rows);
        }, 'json');

        var cityId = $('#CityId').val();

        if (cityId == '') {
            return;
        }

        $('#HospitalName').val('');
        $('#HospitalCode').val('');
        $('#HospitalId').val('');
        $('#Address').val('');




        post('/P/PreApproval/LoadHospital', { cityId: cityId, market: market, userid: currentUserId, TA: $('#TA').val() }, function (d) {
            cacheHospital = new Array();
            setCacheHospital(d.rows);
        }, 'json');

    });

    function GetCostCenterListByTACode(ta) {
        var market = $('#Market').val();
        post('/P/PreApproval/LoadCostCenter', { market: market, ta: ta }, function (d) {
            $.each(d.rows, function (i, n) {
                var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                $('#CostCenter').append(_content);
            });
            cacheCostCenter = new Array();
            setCacheCostCenter(d.rows);
        }, 'json');
    }

    $("#TA").change(function () {
        var market = $('#Market').val();
        var ta = $('#TA').val();
        $('#CostCenter').val('');
        post('/P/PreApproval/LoadCostCenter', { market: market, ta: ta }, function (d) {
            $.each(d.rows, function (i, n) {
                var _content = "<option value='" + n.Name + "'>" + n.Name + "</option>";
                $('#CostCenter').append(_content);
            });
            cacheCostCenter = new Array();
            setCacheCostCenter(d.rows);
        }, 'json');
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
                $('#CostCenter').val('');
                GetCostCenterListByTACode(opt.val);
                var TA = opt.val;

                post('/P/PreApproval/LoadProvince', { Type: market, TA: TA }, function (d) {
                    cacheProvince = new Array();
                    setCacheProvince(d.rows);
                }, 'json');

                var cityId = $('#CityId').val();

                if (cityId == '') {
                    return;
                }

                post('/P/PreApproval/LoadHospital', {
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

    //成本中心点击事件
    $('#CostCenter').click(function () {
        var ta = $('#TA').val();
        if (ta == -1) {
            showTopMsg(MSG_COSTCENTER);
            return;
        }

        chooser.Show(
            function (keyword, callback) {
                searchCostCenter(keyword, function (list) {
                    var res = iPath.Select(list, function (a) {
                        return { val: a.Name };
                    });
                    callback(res);
                })
            },
            function (opt) {
                $('#CostCenter').val(opt.val);
            });
    });

    var first = $('#ProvinceId').val();
    if (first == '') {
        if (_Market != '') {
            $('#Market').val(_Market);
        } else {
            showDlg(MSG_MarketNone, '返回', function () {
                WeixinJSBridge.call('closeWindow');
            }, 'info');
        }
    }

    //if (!isInTimespan(getTimeNow(), timeConfig.PreWorkTimeSpanBegin, timeConfig.PreWorkTimeSpanEnd)) {
    //    showDlg(MSG_NOPREAPPROVALWORKINGTIME, '返回', function () {
    //        WeixinJSBridge.call('closeWindow');
    //    }, 'info')
    //}

    post('/P/PreApproval/LoadProvince', { Type: $('#Market').val(), TA: "" }, function (d) {
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
                $('#HospitalCode').val('');
                $('#HospitalId').val('');
                $('#Address').val('');
                $('#AddressCode').val('');

                post('/P/PreApproval/LoadCity', { provinceId: opt.key, Type: market, TA:TA }, function (d) {
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
                $('#HospitalCode').val('');
                $('#HospitalId').val('');
                $('#Address').val('');
                $('#AddressCode').val('');

                var market = $('#Market').val();
                var TA = $('#TA').val();
                post('/P/PreApproval/LoadHospital', { cityId: opt.key, market: market, TA: TA }, function (d) {
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
                //$("HospitalName").height($("HospitalName")[0].scrollHeight);
                $('#HospitalCode').val(opt.hco);
                $('#HospitalId').val(opt.hco);
                $('#Address').val('');
                $('#AddressCode').val('');
                var _cityId = cacheHospital[opt.key].CityId;
                post('/P/PreApproval/FindCity', { cityId: _cityId }, function (d) {
                    $('#CityId').val(d.data.ID);
                    $('#CityName').val(d.data.Name);
                    var province = cacheProvince[d.data.ProvinceId];
                    $('#ProvinceId').val(province.ID);
                    $('#ProvinceName').val(province.Name);
                }, 'json');
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
                post('/P/PreApproval/FindCity', { cityId: _cityId }, function (d) {
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
        var ta = $('#TA').val();
        if (ta == '' || ta == null || ta == undefined) {
            showTopMsg('请选择TA');
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
                        return {
                            key: a.ID, val: a.GskHospital == a.HospitalCode ? a.Address : a.MainAddress + ':' + a.Address, ext: a.GskHospital, hco: a.Name,
                            ta: $('#TA').val(), market: $('#Market').val(), lat: a.latitude,lng :a.longitude, del:a.IsDelete
                        };
                    });
                    
                    if (res.length < 1) {
                        document.getElementById("newBtnDiv").style.display = "none";
                    } else {
                        for (var i = 0; i < res.length; i++) {
                            if (res[i].val != "" && res[i].val != "N/A" && res[i].val != null && res[i].lat != "" && res[i].lng != "" && res[i].del !=1 && res[i].del != 2 && GroupType != 2) {
                                document.getElementById("newBtnDiv").style.display = "block";
                            } else {
                                document.getElementById("newBtnDiv").style.display = "none";
                                break;
                            }
                        }
                    }
                    document.getElementById("chooserSearchInput").style.display = "none";
                    callback(res);
                } else {
                    searchHospitalByCode(cityId, keyword, function (list) {
                        var res = iPath.Select(list, function (a) {
                            return {
                                key: a.ID, val: a.GskHospital == a.HospitalCode ? a.Address : a.MainAddress + ':' + a.Address, ext: a.GskHospital, hco: a.Name,
                                ta: $('#TA').val(), market: $('#Market').val(), lat: a.latitude, lng: a.longitude, del: a.IsDelete };
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
                post('/P/PreApproval/FindCity', { cityId: _cityId }, function (d) {
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

    if (GroupType == 2) {
        $(".Group2").attr("readonly", "readonly");
        //不能修改的单选框不能选择
        $(".Group2").attr("disabled", "disabled");

        $(".Group2").css("color", "rgba(0, 0, 0, 0.3)");

    } else if (GroupType == 1) {
        $(".Group1").attr("readonly", "readonly");
        //不能修改的单选框不能选择
        $(".Group1").unbind();
    }


    //提交预申请
    //$('#btnSubmitApplication').click(function () {
    //    showLoadingToast();
    //    var p = submitCheckData(1);
    //    console.log(p);
    //    hideLoadingToast();
    //    if (p.attendance < 3) {
    //        if (isBindRes) {
    //            showConfim('您申请的会议少于3人参会', '', function () {
    //                if (p == false) {
    //                    return;
    //                }
    //                if (orderSuccess) {
    //                    return;
    //                }
    //                orderSuccess = true;
    //                $('#btnSubmitApplication').unbind('click');
    //                post('/P/PreApproval/SaveSession', p, function (d) {
    //                    //if (p.attendance >= 60) {
    //                    //    $('#form1').submit();
    //                    //}
    //                    //else {
    //                    post('/P/PreApproval/_Submit', {},
    //                        function (d) {
    //                            var _msg = MSG_PREAPPROVALSUBMITSUCCESS;
    //                            if (p.budget >= 1500) {
    //                                _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITBUHEAD;
    //                            }
    //                            if (p.budget >= 1200 && p.budget < 1500) {
    //                                _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITSECOND;
    //                            }
    //                            showDlg(_msg, undefined, function () {
    //                                WeixinJSBridge.call('closeWindow');
    //                            }, 'success')
    //                        }, 'json');
    //                    //}
    //                })
    //            }, '确认', '放弃', function () {
    //            }, 'info');
    //        } else {
    //            showConfimSub('该目标医院暂不支持送餐服务，是否继续？', '', function () {
    //                showConfim('您申请的会议少于3人参会', '', function () {
    //                    if (p == false) {
    //                        return;
    //                    }
    //                    if (orderSuccess) {
    //                        return;
    //                    }
    //                    orderSuccess = true;
    //                    $('#btnSubmitApplication').unbind('click');
    //                    post('/P/PreApproval/SaveSession', p, function (d) {
    //                        //if (p.attendance >= 60) {
    //                        //    $('#form1').submit();
    //                        //}
    //                        //else {
    //                        post('/P/PreApproval/_Submit', {},
    //                            function (d) {
    //                                var _msg = MSG_PREAPPROVALSUBMITSUCCESS;
    //                                if (p.budget >= 1500) {
    //                                    _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITBUHEAD;
    //                                }
    //                                if (p.budget >= 1200 && p.budget < 1500) {
    //                                    _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITSECOND;
    //                                }
    //                                showDlg(_msg, undefined, function () {
    //                                    WeixinJSBridge.call('closeWindow');
    //                                }, 'success')
    //                            }, 'json');
    //                        //}
    //                    })
    //                }, '确认', '放弃', function () {
    //                }, 'info');
    //            }, '是', '否', function () { }, 'info');
    //        }
    //    }
    //    else {
    //        if (p == false) {
    //            return;
    //        }
    //        if (orderSuccess) {
    //            return;
    //        }
    //        if (isBindRes) {
    //            orderSuccess = true;
    //            $('#btnSubmitApplication').unbind('click');
    //            post('/P/PreApproval/SaveSession', p, function (d) {
    //                //if (p.attendance >= 60) {
    //                //    $('#form1').submit();
    //                //}
    //                //else {
    //                post('/P/PreApproval/_Submit', {},
    //                    function (d) {
    //                        var _msg = MSG_PREAPPROVALSUBMITSUCCESS;
    //                        if (p.budget >= 1500) {
    //                            _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITBUHEAD;
    //                        }
    //                        if (p.budget >= 1200 && p.budget < 1500) {
    //                            _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITSECOND;
    //                        }
    //                        showDlg(_msg, undefined, function () {
    //                            WeixinJSBridge.call('closeWindow');
    //                        }, 'success')
    //                    }, 'json');
    //                //}
    //            })
    //        } else {
    //            showConfimSub('该目标医院暂不支持送餐服务，是否继续？', '', function () {
    //                orderSuccess = true;
    //                $('#btnSubmitApplication').unbind('click');
    //                post('/P/PreApproval/SaveSession', p, function (d) {
    //                    //if (p.attendance >= 60) {
    //                    //    $('#form1').submit();
    //                    //}
    //                    //else {
    //                    post('/P/PreApproval/_Submit', {},
    //                        function (d) {
    //                            var _msg = MSG_PREAPPROVALSUBMITSUCCESS;
    //                            if (p.budget >= 1500) {
    //                                _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITBUHEAD;
    //                            }
    //                            if (p.budget >= 1200 && p.budget < 1500) {
    //                                _msg = MSG_PREAPPROVALSUBMITSUCCESSWAITSECOND;
    //                            }
    //                            showDlg(_msg, undefined, function () {
    //                                WeixinJSBridge.call('closeWindow');
    //                            }, 'success')
    //                        }, 'json');
    //                    //}
    //                })
    //            }, '是', '否', function () { }, 'info');
    //        }
    //    }
    //});

    //$('#MeetingTime').click(function () {
    //    post('/P/Food/LoadNextHoliday', {},
    //        function (d) {
    //            var minDate;
    //            var holiday = d.data.holiday;
    //            holiday.StartDay = getDateByDotNet(holiday.StartDay);
    //            holiday.EndDay = getDateByDotNet(holiday.EndDay);
    //            var startDay = getDateByDotNet(d.data.now);
    //            var endDay;

    //            // 判断今天在那个区间1.非最后一个工作日 2.最后一个工作日 3.休息日
    //            // 规则1.可以定到下一天 2、3.可以订到休假日后第二个工作日
    //            var rangeType = 0;

    //            if (holiday.StartDay.getTime() <= startDay.getTime() && startDay.getTime() <= holiday.EndDay.getTime()) {
    //                // 今天是休假日 判定为区间3
    //                rangeType = 3;
    //            } else {
    //                if ((startDay.getTime() + 24 * 60 * 60 * 1000) < holiday.StartDay.getTime()) {
    //                    // 明天也是工作日 判定为区间1
    //                    rangeType = 1;
    //                } else {
    //                    // 明天是休息日，今天是最后一个工作日 判定为区间2
    //                    rangeType = 2;
    //                }
    //            }
    //            var deliverRange = new Array();
    //            var deliverTime = new Array();
    //            if (rangeType == 1) {
    //                if (isInTimespan(getTimeNow(), timeConfig.preNextBegin, timeConfig.preNextEnd)) {
    //                    minDate = new Date(startDay.getTime() + oneDay).pattern('yyyy-MM-dd');
    //                }
    //                else {
    //                    minDate = new Date(startDay.getTime() + 2 * oneDay).pattern('yyyy-MM-dd');
    //                }
    //            }
    //            else if (rangeType == 2) {
    //                if (isInTimespan(getTimeNow(), timeConfig.preNextBegin, timeConfig.preNextEnd)) {
    //                    minDate = new Date(startDay.getTime() + oneDay).pattern('yyyy-MM-dd');
    //                }
    //                else {
    //                    for (var i = startDay.getTime(); i <= (holiday.EndDay.getTime() + oneDay); i += oneDay) {
    //                        minDate = new Date(holiday.EndDay.getTime() + oneDay * 2).pattern('yyyy-MM-dd');
    //                    }
    //                }
    //            }
    //            else {
    //                minDate = new Date(holiday.EndDay.getTime() + oneDay * 2).pattern('yyyy-MM-dd');
    //            }
    //            var i = minDate;
    //            while (new Date(i).getMonth() == new Date(minDate).getMonth()) {
    //                deliverRange.push(i);
    //                i = new Date(new Date(i).getTime() + oneDay).pattern('yyyy-MM-dd');
    //            }

    //            for (var i = 0; i < deliverRange.length; i++) {
    //                var _time = timeConfig.preApprovalBegin;
    //                var _weekday = new Date(deliverRange[i].replace(/-/g, '/')).pattern('E');
    //                for (; isTimeInTimespan(_time, timeConfig.preApprovalBegin, timeConfig.preApprovalEnd);) {
    //                    deliverTime.push({
    //                        time: deliverRange[i] + ' ' + _time,
    //                        weekday: _weekday
    //                    });
    //                    _time = timeAdd(_time, timeConfig.cachetime);
    //                }
    //            }
    //            showDelivertime(deliverTime);
    //        })
    //});

    //$('#Attendance').bind('input propertychange', function () {
    //    var _attendCount = $(this).val();
    //    _attendCount = parseInt(_attendCount);
    //    _attendCount = isNaN(_attendCount) ? 0 : _attendCount;
    //    $(this).val(_attendCount);
    //});

    //$("#rbSpeaker1").click(function () {
    //    $("#dvUploadSpeakerFile").show();
    //})

    //$("#dvSpeakerPanel").click(function () {
    //    if ($("#rbSpeaker1").is(":checked") == true) {
    //        $("#dvUploadSpeakerFile").show();
    //    }
    //})

    //$('#dvUploadSpeakerFile .close').click(function () {
    //    $('#dvUploadSpeakerFile').hide();
    //});

    //$("#btnSaveSpeakFile").click(function () {
    //    var files1 = new Array();
    //    var files2 = new Array();
    //    $('#chooseUploadServiceImage img').each(function () {
    //        var src = $(this).attr('_src');
    //        files1.push(src);
    //    });
    //    $('#chooseUploadBenefitImage img').each(function () {
    //        var src = $(this).attr('_src');
    //        files2.push(src);
    //    });
    //    if (files1.length == 0) {
    //        showTopMsg(MSG_NOTUPLOADSPEAKERSERVICE);
    //        return;
    //    }
    //    if (files2.length == 0) {
    //        showTopMsg(MSG_NOTUPLOADSPEAKERBENEFIT);
    //        return;
    //    }
    //    $('#dvUploadSpeakerFile').hide();
    //})
});

function delcommafy(num) {
    num = num.replace(/,/gi, '');
    return num;
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


//添加演讲服务协议
function UploadServiceImage() {
    var index = 0;
    wx.chooseImage({
        count: 1,
        sizeType: ['original'],
        sourceType: ['album'],
        success: function (res) {
            var localId = res.localIds[0];
            wx.uploadImage({
                localId: localId,
                isShowProgressTips: 1,
                success: function (res) {
                    var serverId = res.serverId;
                    index++;
                    post('/FileService/Save', { id: serverId }, function (d) {
                        var html = '<div class="my_weui_uploader_input_wrp icon_sp_area" style="margin-top: 5px;margin-right: 20px;" id="' + index + '">'
                            + '<img _src="' + d.data + '" src="' + awsUrl + d.data + '" style="width:100%; height:100%;margin-top: 5px;"/>'
                            + '<i class="weui-icon-cancel" style="float:right; position: relative; top: -89px; right: -16px;" onclick="removeUploadServiceImage(this)"></i>'
                            + '</div>';
                        $('#dvUploaderServiceImage').before(html);
                        var imageUrls = new Array();
                        $('#chooseUploadServiceImage img').each(function (i, e) {
                            imageUrls.push($(e).attr("src"));
                            $(this).unbind("click");
                            $(this).click(function () {
                                WeixinJSBridge.invoke('imagePreview', {
                                    'current': imageUrls[i],
                                    'urls': imageUrls
                                });
                            });
                        });
                        showAddChooseUploadServiceImage();
                    }, 'json');
                },
                fail: function (res) { showDlg(MSG_UPLOADIMAGE); }
            });
        }
    });
}

//添加利益冲突声明
function UploadBenefitImage() {
    var index = 0;
    wx.chooseImage({
        count: 1,
        sizeType: ['original'],
        sourceType: ['album'],
        success: function (res) {
            var localId = res.localIds[0];
            wx.uploadImage({
                localId: localId,
                isShowProgressTips: 1,
                success: function (res) {
                    var serverId = res.serverId;
                    index++;
                    post('/FileService/Save', { id: serverId }, function (d) {
                        var html = '<div class="my_weui_uploader_input_wrp icon_sp_area" style="margin-top: 5px;margin-right: 20px;" id="' + index + '">'
                            + '<img _src="' + d.data + '" src="' + awsUrl + d.data + '" style="width:100%; height:100%;margin-top: 5px;"/>'
                            + '<i class="weui-icon-cancel" style="float:right; position: relative; top: -89px; right: -16px;" onclick="removeUploadBenefitImage(this)"></i>'
                            + '</div>';
                        $('#dvUploaderBenefitImage').before(html);
                        var imageUrls = new Array();
                        $('#chooseUploadBenefitImage img').each(function (i, e) {
                            imageUrls.push($(e).attr("src"));
                            $(this).unbind("click");
                            $(this).click(function () {
                                WeixinJSBridge.invoke('imagePreview', {
                                    'current': imageUrls[i],
                                    'urls': imageUrls
                                });
                            });
                        });
                        showAddChooseUploadBenefitImage();
                    }, 'json');
                },
                fail: function (res) { showDlg(MSG_UPLOADIMAGE); }
            });

        }
    });
}

//演讲服务协议文件数量最大值
function showAddChooseUploadServiceImage() {
    if ($('#chooseUploadServiceImage>div').length > 9) {
        $('#dvUploaderServiceImage').hide();
    } else {
        $('#dvUploaderServiceImage').show();
    }
}

//利益冲突声明文件数量最大值
function showAddChooseUploadBenefitImage() {
    if ($('#chooseUploadBenefitImage>div').length > 9) {
        $('#dvUploaderBenefitImage').hide();
    } else {
        $('#dvUploaderBenefitImage').show();
    }
}

//删除图片（演讲服务协议）
function removeUploadServiceImage(dom) {
    $(dom).parent().remove();
    showAddChooseUploadServiceImage();
}

//删除图片（利益冲突声明）
function removeUploadBenefitImage(dom) {
    $(dom).parent().remove();
    showAddChooseUploadBenefitImage();
}

Array.prototype.contains = function (needle) {
    for (i in this) {
        if (this[i] == needle) return true;
    }
    return false;
}

