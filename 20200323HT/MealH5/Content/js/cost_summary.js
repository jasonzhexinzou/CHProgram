$(document).ready(function () {
    var calendar = new LCalendar();
    calendar.init({
        'trigger': '#startDate', //标签id
        'type': 'date', //date 调出日期选择 datetime 调出日期时间选择 time 调出时间选择 ym 调出年月选择,
        'minDate': (new Date().getFullYear() - 3) + '-' + 1 + '-' + 1, //最小日期
        'maxDate': (new Date().getFullYear() + 3) + '-' + 12 + '-' + 31 //最大日期
    });

    var calendar = new LCalendar();
    calendar.init({
        'trigger': '#endDate', //标签id
        'type': 'date', //date 调出日期选择 datetime 调出日期时间选择 time 调出时间选择 ym 调出年月选择,
        'minDate': (new Date().getFullYear() - 3) + '-' + 1 + '-' + 1, //最小日期
        'maxDate': (new Date().getFullYear() + 3) + '-' + 12 + '-' + 31 //最大日期
    });

    $('#startDate').val(std);
    $('#endDate').val(ed);


    $("#btnRefresh").click(function () {

        var StartDate = $('#startDate').val();
        var EndDate = $('#endDate').val();

        post('/P/CostAnalysis/LoadSummaryData', {
            StartDate: StartDate,
            EndDate: EndDate
        },
            function (d) {
                var data = d.data;
                $('#PreApprovalCount').val(data.PreApprovalCount);
                $('#BudgetTotal').val("￥" + data.BudgetTotal);
                $('#OrderCount').val(data.OrderCount);
                $('#RealPrice').val("￥" + data.RealPrice);
                $('#SpecialOrderApplierCount').val(data.SpecialOrderApplierCount);
                $('#SpecialOrderCount').val(data.SpecialOrderCount);
                $('#UnfinishedOrderApplierCount').val(data.UnfinishedOrderApplierCount);
                $('#UnfinishedOrderCount').val(data.UnfinishedOrderCount);
            }, 'json');

    })

    $('#tip1').tipso({
        useTitle: false,
        titleContent: "预算金额",
        content: "统计审批通过的预申请",
        onShow: function () {
            $('#tip1').click(function () {
                $('#tip1').tipso('hide');
            });
        },
        onHide: function () {
            $('#tip1').click(function () {
                $('#tip1').tipso('show');
            });
        }
    });


    $('#tip2').tipso({
        useTitle: false,
        titleContent: "实际金额",
        content: "统计有效订单（除退单成功和预定失败）",
        onShow: function () {
            $('#tip2').click(function () {
                $('#tip2').tipso('hide');
            });
        },
        onHide: function () {
            $('#tip2').click(function () {
                $('#tip2').tipso('show');
            });
        }
    });


    $('#tip3').tipso({
        useTitle: false,
        titleContent: "特殊订单",
        content: "当日多场/文件丢失/退单失败",
        onShow: function () {
            $('#tip3').click(function () {
                $('#tip3').tipso('hide');
            });
        },
        onHide: function () {
            $('#tip3').click(function () {
                $('#tip3').tipso('show');
            });
        }
    });


    $('#tip4').tipso({
        useTitle: false,
        titleContent: "未完成订单",
        content: "未完成上传文件审批",
        onShow: function () {
            $('#tip4').click(function () {
                $('#tip4').tipso('hide');
            });
        },
        onHide: function () {
            $('#tip4').click(function () {
                $('#tip4').tipso('show');
            });
        }
    });
});




function tipShow(id) {
    $("#" + id).tipso('show');
}

function LoadOrderUp(Role) {
    var StartDate = $('#startDate').val();
    var EndDate = $('#endDate').val();

    window.location.href = contextUri + '/P/CostAnalysis/OrderAnalysisUp?Role=' + Role + '&StartDate=' + StartDate + '&EndDate=' + EndDate;
}

function LoadOrderDown(Role) {
    var StartDate = $('#startDate').val();
    var EndDate = $('#endDate').val();

    window.location.href = contextUri + '/P/CostAnalysis/OrderAnalysis?Role=' + Role + '&StartDate=' + StartDate + '&EndDate=' + EndDate;
}

function LoadPreUp(Role) {
    var StartDate = $('#startDate').val();
    var EndDate = $('#endDate').val();

    window.location.href = contextUri + '/P/CostAnalysis/PreApprovalAnalysisUp?Role=' + Role + '&StartDate=' + StartDate + '&EndDate=' + EndDate;
}

function LoadPreDown(Role) {
    var StartDate = $('#startDate').val();
    var EndDate = $('#endDate').val();

    window.location.href = contextUri + '/P/CostAnalysis/PreApprovalAnalysis?Role=' + Role + '&StartDate=' + StartDate + '&EndDate=' + EndDate;
}

