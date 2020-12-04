var lastTime = getTimeNow().pattern('yyyy-MM-dd HH:mm:ss.999') + oneDay;
var noMore = false;

// 载入当前登录人上传文件列表数据
function loadPreApprovalList(callback) {
    post('/P/Upload/LoadMyAutoTransfer',
        {
            end: lastTime,
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
                    item.DeliverTime = getDateByDotNet(item.DeliverTime).pattern('yyyy-MM-dd HH:mm');
                }
                var html = $(render(d));
                html.click(function () {
                    location.href = contextUri + '/P/Upload/AutoTransferOrderDetails' + '?HTCode=' + item.HTCode;
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

var render;
var renderApprove;
$(function () {
    render = template('tmpl_myApproval');
    loadApproval();
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