var RN = {
    mallTypeOptions: [{
        value: '0',
        label: '自营他配'
    }, {
        value: '1',
        label: '第三方'
    }, {
        value: '2',
        label: '自营自配自采'
    },{
        value: '3',
        label: '自营自配统采'
    }], 
    getMallType: function (type) {
        if (type == null) {
            return "第三方";
        } else if (type == 0) {
            return "自营他配";
        } else if (type == 1) {
            return "第三方";
        } else if (type == 2) {
            return "自营自配自采";
        } else if(type == 3){
            return "自营自配统采";
        }else {
            return "未知类型";
        }
    },
    shipperTypeOptions: [
    {
        value: '0',
        label: '商家自选物流'
    }, {
        value: '1',
        label: '京东发货'
    }],
    getShipperType: function (type) {
        if (type == null) {
            return "商家自选物流";
        } else if (type == 0) {
            return "商家自选物流";
        } else if (type == 1) {
            return "京东发货";
        } else {
            return "未知类型";
        }
    },
};