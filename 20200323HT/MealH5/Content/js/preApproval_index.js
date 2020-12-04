var lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999') + oneDay;
var noMore = false;

var approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
var approveNoMore = false;

var cbClick = false;

// 载入当前登录人预申请列表数据
function loadPreApprovalList(callback) {
    var year = $("#years").val();
    var month = $("#months").val();
    var state = $("#dpState").val();
    var budget = $("#dpBudget").val();
    post('/P/PreApprovalState/LoadMyPreApproval',
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
                    showTopMsg('已无更多数据！');
                }
            }
            else {
                for (var i in d.rows) {
                    var item = d.rows[i];
                    item.MeetingDate = getDateByDotNet(item.MeetingDate).pattern('yyyy-MM-dd HH:mm');
                    switch (item.State) {
                        case "0": item.State = "预申请待审批"; break;
                        case "1": item.State = "预申请待审批"; break;
                        case "2": item.State = "预申请审批被驳回"; break;
                        case "3": item.State = "预申请待审批"; break;
                        case "4": item.State = "预申请审批被驳回"; break;
                        case "5": item.State = "预申请审批通过"; break;
                        case "6": item.State = "预申请审批通过"; break;
                        case "7": item.State = "预申请待审批"; break;
                        case "8": item.State = "预申请审批被驳回"; break;
                        case "9": item.State = "预申请审批通过"; break;
                        case "10": item.State = "预申请已取消"; break;
                    }
                    switch (item.OrderState) {
                        case 3: item.OrderState = "订单提交成功"; break;
                        case 4: item.OrderState = "订单预定成功"; break;
                        case 5: item.OrderState = "订单预定失败"; break;
                        case 6: item.OrderState = "订单已确认收餐"; break;
                        case 7: item.OrderState = "订单已自动收餐"; break;
                        case 8: item.OrderState = "订单未送达"; break;
                        case 9: item.OrderState = "订单已评价"; break;
                        case 10: item.OrderState = "订单申请退单"; break;
                        case 12: item.OrderState = "订单退单失败"; break;
                    }
                }
                var html = $(render(d));
                html.click(function () {
                    var id = $(this).attr('_id');
                    location.href = contextUri + '/P/PreApproval/Details/' + id;
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
                lastTime = getDateByDotNet(lastTime).pattern('yyyy-MM-dd HH:mm:ss');
            }
            callback(d.rows.length);
        }, 'json');
}

// 载入当前等路人审批预申请列表数据
function loadApproveList(callback) {
    var year = $("#approveYears").val();
    var month = $("#approveMonths").val();
    var state = $("#dpApproveState").val();
    var applicant = $("#txtApplicant").val();
    post('/P/PreApprovalState/LoadMyApprove',
        {
            end: approveLastTime,
            state: state,
            applicant,
            year: year,
            month: month == "All" ? 0 : month
        },
        function (d) {
            if (d.rows.length == 0) {
                if (approveNoMore) {
                    showTopMsg('已无更多数据！');
                }
            }
            else {
                for (var i in d.rows) {
                    var item = d.rows[i];
                    item.MeetingDate = getDateByDotNet(item.MeetingDate).pattern('yyyy-MM-dd HH:mm');
                    switch (item.State) {
                        case "0": item.State = "预申请待审批"; break;
                        case "2": item.State = "预申请审批驳回"; break;
                        case "3": item.State = "预申请待审批"; break;
                        case "4": item.State = "预申请审批驳回"; break;
                        case "5": item.State = "预申请审批通过"; break;
                        case "6": item.State = "预申请审批通过"; break;
                        case "7": item.State = "预申请待审批"; break;
                        case "8": item.State = "预申请审批驳回"; break;
                        case "9": item.State = "预申请审批通过"; break;
                        case "10": item.State = "预申请已取消"; break;
                    }
                }
                var html = $(renderApprove(d));
                $('#myApproveList').append(html);
                $("#myApproveList .pInfo").click(function () {
                    var id = $(this).attr('_val');
                    location.href = contextUri + '/P/PreApproval/Approval?id=' + id+'&from=1';
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
                approveLastTime = getDateByDotNet(approveLastTime).pattern('yyyy-MM-dd HH:mm:ss');
            }
            callback(d.rows.length);
        }, 'json');
}

var render;
var renderApprove;
$(function () {
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
        if ($(this).val() == "'5','6'" || $(this).val() == "'2','4'") {
            $("#btnApproveSelected").hide();
            $("#btnApproveAll").hide();
        }
    });

    $("#txtApplicant").change(function () {
        document.getElementById("myApproveList").innerHTML = "";
        approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApprove();
    });

    $("#btnApproveAll").click(approveAll);

    $("#btnApproveSelected").click(approveSelected);

    $("#dpBudget").change(function () {
        document.getElementById("myPreApprovalList").innerHTML = "";
        lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApproval();
    });
});

function loadApproval() {
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
    location.href = contextUri + '/P/PreApproval/Approval?id=' + id + '&from=1';
}

function approve(id, meetingDate) {
    var timeNow = getTimeNow().getTime();
    var meetingTime = new Date(meetingDate.replace(/-/g, '/')).getTime();
    if (timeNow > meetingTime) {
        showDlg('该预申请已失效', '确认', function () {
            WeixinJSBridge.call('closeWindow');
        }, 'success');
    }
    else {
        post('/P/PreApprovalState/PreApprovalApprove',
        {
            id: id,
            state: 3,
            reason: ''
        },
        function (d) {
            if (d.state == 1) {
                showDlg(d.txt, '返回', function () {
                    //WeixinJSBridge.call('closeWindow');
                    document.getElementById("myApproveList").innerHTML = "";
                    approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm');
                    loadApprove();
                }, 'success');
            } else {
                showDlg(d.txt, '返回', function () {
                    //WeixinJSBridge.call('closeWindow');
                }, 'success');
            }
        }, 'json');
    }
}

function approveAll() {
    var year = $("#approveYears").val();
    var month = $("#approveMonths").val();
    var state = $("#dpApproveState").val();
    var applicant = $("#txtApplicant").val();
    post('/P/PreApprovalState/ApproveAll',
        {
            end: approveLastTime,
            state: state,
            applicant,
            year: year,
            month: month == "All" ? 0 : month
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
        post('/P/PreApprovalState/ApproveSelected',
            {
                ids: ids
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
        showDlg("请选择至少1条待审批预申请", '返回', function () {
        }, 'info');
    }
}