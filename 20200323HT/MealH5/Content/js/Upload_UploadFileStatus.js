var lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999') + oneDay;
var noMore = false;

var approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
var approveNoMore = false;

// 载入当前登录人上传文件列表数据
function loadPreApprovalList(callback) {
    var year = $("#years").val();
    var month = $("#months").val();
    var state = $("#dpState").val();
    var budget = $("#dpBudget").val();
    post('/P/Upload/LoadMyUploadOrder',
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
                        case "1": item.State = "上传文件待审批"; break;
                        case "2": item.State = "上传文件审批被驳回"; break;
                        case "3": item.State = "上传文件审批被驳回"; break;
                        case "4": item.State = "上传文件审批通过"; break;
                    }
                }
                var html = $(render(d));
                html.click(function () {
                    var id = $(this).attr('_id');
                    location.href = contextUri + '/P/Upload/Details/' + id;
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

// 载入当前等路人审批上传文件列表数据
function loadApproveList(callback) {
    var year = $("#approveYears").val();
    var month = $("#approveMonths").val();
    var state = $("#dpApproveState").val();
    var applicant = $("#txtApplicant").val();
    post('/P/Upload/LoadMyApprove',
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
                        case "1": item.State = "上传文件待审批"; break;
                        case "2": item.State = "上传文件审批驳回"; break;
                        case "3": item.State = "上传文件财务审批驳回"; break;
                        case "4": item.State = "上传文件审批通过"; break;
                    }
                }
                var html = $(renderApprove(d));
                $('#myApproveList').append(html);
                $("#myApproveList").on("click", ".pInfo", function () {
                    var id = $(this).attr('_val');
                    location.href = contextUri + '/P/Upload/Approval?id=' + id+'&from=1';
                });
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
    });

    $("#txtApplicant").change(function () {
        document.getElementById("myApproveList").innerHTML = "";
        approveLastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999');
        loadApprove();
    });

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
            });
        }
    });
}