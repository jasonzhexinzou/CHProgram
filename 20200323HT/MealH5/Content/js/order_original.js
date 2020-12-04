
// 按原订单配送
function postFood() {
    post('/P/Order/OriginalOrderSend', { id: orderId, supplier: supplier },
        function () {
            showDlg(MSG_ORIGINALORDERSENDSUCCESS, undefined, function () {
                WeixinJSBridge.call('closeWindow');
            }, 'success');
        }, 'json');
}


// 关闭当前页面
function closepage() {
    WeixinJSBridge.call('closeWindow');
}


