getOrder({
    userId: sessionStorage.userId,
    pageIndex: sessionStorage.page,
    pageSize: pageSize,
    orderState: sessionStorage.OrderState,
    callback: function (data) {
        //sessionStorage.data = JSON.stringify(data);
        sessionStorage.OrderState = getQueryString('orderState');

        var prevlocalData = JSON.parse(sessionStorage.orderList);
        if (prevlocalData != false && prevlocalData.length > 0) {
            sessionStorage.orderList = JSON.stringify(JSON.parse(sessionStorage.orderList).concat(data));
        } else {
            sessionStorage.orderList = JSON.stringify(data);
        }

        var items = $('#orderItems');

        var html = getOrderListHtml(data);
        items.append(html);
        registOrderEvent();

        showLazyLoadImg();
        if (data.length >= 10) {
            $('#footer_loading').show().find('span').text('正在获取数据...');
        } else {
            $('#footer_loading').show().find('span').text('暂无更多订单数据!');
        }
        setTimeout(function () {
            loading = false;
        }, 1000);
        isOrderDataEnd = data.length < 10;

    }
});