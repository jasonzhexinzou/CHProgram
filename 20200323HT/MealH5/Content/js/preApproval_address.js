var lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999') + oneDay;
var noMore = false;

var approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
var approveNoMore = false;

var cbClick = false;

// 载入当前登录人地址申请列表数据
function loadPreApprovalList(callback) {
    var year = $("#years").val();
    var month = $("#months").val();
    var state = $("#dpState").val();
    var budget = '';
    post('/P/PreApprovalState/LoadMyAddressApproval',
        {
            end: lastTime,
            state: state,
            year: year,
            month: month == "All" ? 0 : month,
            budget: budget
        },
        function (d) {
            if (d.rows.length == 0) {
                if (noMore) {
                    //showTopMsg('已无更多数据！');
                }
            }
            else {
                for (var i in d.rows) {
                    var item = d.rows[i];
                    item.CreateDate = getDateByDotNet(item.CreateDate).pattern('yyyy-MM-dd HH:mm:ss');
                    switch (item.ApprovalStatus) {
                        case 0: item.ApprovalStatus = "地址申请待审批"; break;
                        case 9: item.ApprovalStatus = "地址申请待审批"; break;
                        case 10: item.ApprovalStatus = "地址申请待审批"; break;
                        case 1: item.ApprovalStatus = "地址申请审批通过"; break;
                        case 5: item.ApprovalStatus = "地址申请审批通过"; break;
                        case 7: item.ApprovalStatus = "地址申请审批通过"; break;
                        case 2: item.ApprovalStatus = "地址申请审批驳回"; break;
                        case 6: item.ApprovalStatus = "地址申请审批驳回"; break;
                        case 8: item.ApprovalStatus = "地址申请审批驳回"; break;
                        case 3: item.ApprovalStatus = "地址申请已失效"; break;
                        case 4: item.ApprovalStatus = "地址申请已取消"; break;
                    }
                }
                var html = $(render(d));
                html.click(function () {
                    var id = $(this).attr('_id');
                    location.href = contextUri + '/P/PreApproval/AddressDetail?id=' + id + "&from=1";
                });
                $('#myPreApprovalList').append(html);
                var height = window.screen.height;
                var _height = document.getElementById("myPreApprovalList").offsetHeight;
                if (_height < height) {
                    noMore = false;
                }
                else {
                    noMore = true;
                }
                lastTime = d.rows[d.rows.length - 1].CreateDate;
                //lastTime = getDateByDotNet(lastTime).pattern('yyyy-MM-dd HH:mm:ss');
            }
            callback(d.rows.length);
        }, 'json');
}

// 载入当前登录人待审批地址申请列表数据
function loadApproveList(callback) {
    var year = $("#approveYears").val();
    var month = $("#approveMonths").val();
    var state = $("#dpApproveState").val();
    var applicant = $("#txtApplicant").val();
    post('/P/PreApprovalState/LoadMyAddressApprove',
        {
            end: approveLastTime,
            state: state,
            applicant,
            year: year,
            month: month == "All" ? 0 : month
        },
        function (d) {
            if (d.rows.length == 0) {
                if (!approveNoMore) {
                    //showTopMsg('已无更多数据！');
                    //document.getElementById('btnApprove').style.display = 'none';
                }
                //else
                //    document.getElementById('btnApprove').style.display = 'bolck';
            }
            else {
                for (var i in d.rows) {
                    var item = d.rows[i];
                    item.CreateDate = getDateByDotNet(item.CreateDate).pattern('yyyy-MM-dd HH:mm:ss');
                    switch (item.ApprovalStatus) {
                        case 0: item.ApprovalStatus = "地址申请待审批"; break;
                        case 9: item.ApprovalStatus = "地址申请待审批"; break;
                        case 10: item.ApprovalStatus = "地址申请待审批"; break;
                        case 1: item.ApprovalStatus = "地址申请审批通过"; break;
                        case 5: item.ApprovalStatus = "地址申请审批通过"; break;
                        case 7: item.ApprovalStatus = "地址申请审批通过"; break;
                        case 2: item.ApprovalStatus = "地址申请审批驳回"; break;
                        case 6: item.ApprovalStatus = "地址申请审批驳回"; break;
                        case 8: item.ApprovalStatus = "地址申请审批驳回"; break;
                        case 3: item.ApprovalStatus = "地址申请已失效"; break;
                        case 4: item.ApprovalStatus = "地址申请已取消"; break;
                    }
                }
                var html = $(renderApprove(d));
                $('#myApproveList').append(html);
                $("#myApproveList .pInfo").click(function () {
                    var id = $(this).attr('_val');
                    location.href = contextUri + '/P/PreApproval/AddressApprove?id=' + id +'&from=1';
                })
                $("#myApproveList .tdCheck").click(function () {
                    if (cbClick) {
                        cbClick = false;
                    }
                    else {
                        var cb = $(this).children();
                        if ($(cb).is(":checked") == true) {
                            $(cb).prop("checked", false);
                        }
                        else {
                            $(cb).prop("checked", true);
                        }
                    }
                })
                $("#myApproveList .pcheck").click(function () {
                    cbClick = true;
                })
                var height = window.screen.height;
                var _height = document.getElementById("dvApproveContent").offsetHeight;
                if (_height < height) {
                    approveNoMore = false;
                }
                else {
                    approveNoMore = true;
                }
                approveLastTime = d.rows[d.rows.length - 1].CreateDate;
                //approveLastTime = getDateByDotNet(approveLastTime).pattern('yyyy-MM-dd HH:mm:ss');
            }
            callback(d.rows.length);
        }, 'json');
}

function loadApproveListCount() {
    var year = $("#approveYears").val();
    var month = $("#approveMonths").val();
    var state = $("#dpApproveState").val();
    var applicant = $("#txtApplicant").val();
    post('/P/PreApprovalState/LoadMyAddressApproveCount',
        {
            end: approveLastTime,
            state: state,
            applicant,
            year: year,
            month: month == "All" ? 0 : month
        },
        function (d) {
            if (d.totalCount == 0)
                document.getElementById('btnApprove').style.display = 'none';
            else
                document.getElementById('btnApprove').style.display = 'bolck';
        }, 'json');
}

var render;
var renderApprove;
$(function () {
    //if (!isInTimespan(getTimeNow(), timeConfig.AddAddressTimeSpanBegin, timeAdd(timeConfig.AddAddressTimeSpanEnd, timeConfig.cachetime))) {
    //    $("#btnEdit").attr("disabled", "true");
    //    $('#btnEdit').css("background-color", "rgba(0, 0, 0, 0.3)");
    //}
    if (!isInTimespan(getTimeNow(), timeConfig.AddAddressTimeSpanBegin, timeConfig.AddAddressTimeSpanEnd)) {
        $("#btnEdit").attr("disabled", "true");
        $('#btnEdit').css("background-color", "rgba(0, 0, 0, 0.3)");
    }
    //loadApproveListCount();
    render = template('tmpl_myApproval');
    renderApprove = template('tmpl_myApprove');
    $("#content div").hide(); // Initially hide all content
    if (hasRights) {
        $("li").eq(1).attr("id", "current"); // Activate this
        $("#MyApprove").show(); // Show content for current tab
        $("#MyApprove").find('*').show();
        $("#dvNoRights").hide();
        loadApprove();
    }
    else {
        $("#tabs li:first").attr("id", "current"); // Activate first tab
        $("#content div:first").show(); // Show first tab content
        $("#content div:first *").show(); // Show first tab content
        loadApproval();
    }
    $('#tabs a').click(function (e) {
        e.preventDefault();
        $("#content div").hide(); //Hide all content
        $("#tabs li").attr("id", ""); //Reset id's
        $(this).parent().attr("id", "current"); // Activate this
        $('#' + $(this).attr('title')).show(); // Show content for current tab
        $('#' + $(this).attr('title')).find('*').show();
        if (hasRights) {
            $("#dvApproveContent").show();
            $("#btnApproveAll").show();
            $("#btnApproveSelected").show();
            $("#dvNoRights").hide();
        }
        else {
            $("#dvApproveContent").hide();
            $("#btnApproveAll").hide();
            $("#btnApproveSelected").hide();
            $("#dvNoRights").show();
            $("#dvNoRightsBg").height(window.screen.height);
        }
        if ($(this).attr('title') == "MyApprove") {
            loadApprove();
        }
        else {
            loadApproval();
        }
    });

    $("#years").change(function () {
        var year = $("#years").val();
        if (year == yearNow) {
            document.getElementById("months").innerHTML = "";
            $.each(listmonth, function (i, n) {
                if (n == "All") {
                    var _content = "<option value='" + n + "'>" + n + "</option>";
                    $("#months").append(_content);
                }
                else {
                    var _content = "<option value='" + n + "'>" + n + "月</option>";
                    $("#months").append(_content);
                }
            });
            document.getElementById("months").value = "All";
        }
        else {
            document.getElementById("months").innerHTML = "";
            $.each(months, function (i, n) {
                if (n == "All") {
                    var _content = "<option value='" + n + "'>" + n + "</option>";
                    $("#months").append(_content);
                }
                else {
                    var _content = "<option value='" + n + "'>" + n + "月</option>";
                    $("#months").append(_content);
                }
            });
            document.getElementById("months").value = "All";
        }
        document.getElementById("myPreApprovalList").innerHTML = "";
        lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApproval();
    });

    $("#months").change(function () {
        document.getElementById("myPreApprovalList").innerHTML = "";
        lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApproval();
    });

    $("#dpState").change(function () {
        document.getElementById("myPreApprovalList").innerHTML = "";
        lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApproval();
    });

    $("#approveYears").change(function () {
        var year = $("#approveYears").val();
        if (year == yearNow) {
            document.getElementById("approveMonths").innerHTML = "";
            $.each(listmonth, function (i, n) {
                if (n == "All") {
                    var _content = "<option value='" + n + "'>" + n + "</option>";
                    $("#approveMonths").append(_content);
                }
                else {
                    var _content = "<option value='" + n + "'>" + n + "月</option>";
                    $("#approveMonths").append(_content);
                }
            });
            document.getElementById("approveMonths").value = "All";
        }
        else {
            document.getElementById("approveMonths").innerHTML = "";
            $.each(months, function (i, n) {
                if (n == "All") {
                    var _content = "<option value='" + n + "'>" + n + "</option>";
                    $("#approveMonths").append(_content);
                }
                else {
                    var _content = "<option value='" + n + "'>" + n + "月</option>";
                    $("#approveMonths").append(_content);
                }
            });
            document.getElementById("approveMonths").value = "All";
        }
        document.getElementById("myApproveList").innerHTML = "";
        approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApprove();
    });

    $("#approveMonths").change(function () {
        document.getElementById("myApproveList").innerHTML = "";
        approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApprove();
    });

    $("#dpApproveState").change(function () {
        document.getElementById("myApproveList").innerHTML = "";
        approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApprove();
        if ($(this).val() == "'1','5','7'" || $(this).val() == "'2','6','8'") {
            $("#btnApproveSelected").hide();
            $("#btnApproveAll").hide();
        } else {
            $("#btnApproveSelected").show();
            $("#btnApproveAll").show();
        }
    });

    $("#txtApplicant").change(function () {
        document.getElementById("myApproveList").innerHTML = "";
        approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApprove();
    });

    $("#btnApproveAll").click(approveAll);

    $("#btnApproveSelected").click(approveSelected);

    //$("#dpBudget").change(function () {
    //    document.getElementById("myPreApprovalList").innerHTML = "";
    //    lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
    //    loadApproval();
    //});
});

function loadApproval() {
    $("#btnApply").show();
    $("#btnApprove").hide();
    $('#MyApproval').dropload({
        scrollArea: window,
        loadDownFn: function (me) {
            showLoadingToast();
            loadPreApprovalList(function (loadCount) {
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

function loadApprove() {
    $("#btnApprove").show();
    $("#btnApply").hide();
    $('#MyApprove').dropload({
        scrollArea: window,
        loadDownFn: function (me) {
            showLoadingToast();
            loadApproveList(function (loadCount) {
                if (loadCount > 0) {
                    me.resetload();
                } else {
                    me.lock();
                    me.noData();
                }
                if ($("#dpApproveState").val() == "'3'") {
                    if ($("#myApproveList li").length > 0) {
                        $("#btnApproveSelected").show();
                        $("#btnApproveAll").show();
                    }
                    else {
                        $("#btnApproveSelected").hide();
                        $("#btnApproveAll").hide();
                    }
                }
                else if ($("#dpApproveState").val() == "'3','4','5','6'") {
                    if ($("#myApproveList li").length > 0) {
                        $("#btnApproveSelected").show();
                        $("#btnApproveAll").show();
                    }
                }
            });
        }
    });
}

function reject(id) {
    location.href = contextUri + '/P/PreApproval/AddressApprove?id=' + id + '&from=1';
}

function approve(id) {
    location.href = contextUri + '/P/PreApproval/AddressApprove?id=' + id + '&from=1';
    //post('/P/PreApproval/LoadAddressApprovalInfo',
    //    {
    //        id: id
    //    },
    //    function (d) {
    //        if (d.state == 1) {
    //            if (d.data.ApprovalStatus == 3) {
    //                showDlg(d.data.DACode + '该地址申请已失效', '确定', function () {
    //                    document.getElementById("myApproveList").innerHTML = "";
    //                    approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
    //                    loadApprove();
    //                }, 'info');
    //            }
    //            else if (d.data.ApprovalStatus == 4) {
    //                //取消
    //                showDlg(d.data.DACode + '该地址申请已取消', '确定', function () {
    //                    document.getElementById("myApproveList").innerHTML = "";
    //                    approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
    //                    loadApprove();
    //                }, 'info');
    //            } else {
    //                post('/P/PreApprovalState/AddressApprove',
    //                    {
    //                        id: id,
    //                        action: 1,
    //                        reason: ''
    //                    },
    //                    function (d) {
    //                        if (d.state == 1) {
    //                            showDlg(d.txt, '返回', function () {
    //                                //WeixinJSBridge.call('closeWindow');
    //                                document.getElementById("myApproveList").innerHTML = "";
    //                                approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
    //                                loadApprove();
    //                            }, 'success');
    //                        } else {
    //                            showDlg(d.txt, '返回', function () {
    //                                //WeixinJSBridge.call('closeWindow');
    //                            }, 'success');
    //                        }
    //                    }, 'json');
    //            }
                
    //        } 
    //    }, 'json');
}

function approveAll() {
    var _applicant = $("#txtApplicant").val();
    post('/P/PreApprovalState/AddressApproveAll',
        {
            action: 1,
            reason: '',
            applicant: _applicant
        },
        function (d) {
            if (d.state == 1) {
                showDlg(d.txt, '返回', function () {
                    document.getElementById("myApproveList").innerHTML = "";
                    approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
                    loadApprove();
                }, 'success');
            }
        }, 'json');
}

function approveSelected() {
    var ids = new Array();
    $.each($("#myApproveList .pcheck"), function (index, item) {
        if ($(item).is(":checked")) {
            ids.push($(item).attr("_val"));
        }
    })
    if (ids.length > 0) {
        post('/P/PreApprovalState/AddressApproveSelected',
            {
                Ids: ids
            },
            function (d) {
                if (d.state == 1) {
                    showDlg(d.txt, '返回', function () {
                        document.getElementById("myApproveList").innerHTML = "";
                        approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
                        loadApprove();
                    }, 'success');
                }
            }, 'json');
    }
    else {
        showDlg("请选择至少1条待审批地址申请", '返回', function () {
        }, 'info');
    }
}