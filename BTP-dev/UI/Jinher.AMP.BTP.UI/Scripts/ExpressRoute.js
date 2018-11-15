
//物流信息。
var ExpressRoute = (function () {
    var ExpressCompany = function (kd100Type, expName) {
        var a = new Object();
        a.expName = expName;
        a.kd100Type = kd100Type;
        return a;
    }

    var allExpress = new Array();
    allExpress.push(new ExpressCompany("aae", "aae全球专递"));
    allExpress.push(new ExpressCompany("anjie", "安捷快递"));
    allExpress.push(new ExpressCompany("anxindakuaixi", "安信达快递"));
    allExpress.push(new ExpressCompany("biaojikuaidi", "彪记快递"));
    allExpress.push(new ExpressCompany("bht", "bht"));
    allExpress.push(new ExpressCompany("baifudongfang", "百福东方国际物流"));
    allExpress.push(new ExpressCompany("coe", "中国东方（COE）"));
    allExpress.push(new ExpressCompany("changyuwuliu", "长宇物流"));
    allExpress.push(new ExpressCompany("datianwuliu", "大田物流"));
    allExpress.push(new ExpressCompany("debangwuliu", "德邦物流"));
    allExpress.push(new ExpressCompany("dhl", "DHL"));
    allExpress.push(new ExpressCompany("dpex", "dpex"));
    allExpress.push(new ExpressCompany("dsukuaidi", "d速快递"));
    allExpress.push(new ExpressCompany("disifang", "递四方"));
    allExpress.push(new ExpressCompany("ems", "EMS快递"));
    allExpress.push(new ExpressCompany("fedex", "fedex（国外）"));
    allExpress.push(new ExpressCompany("feikangda", "飞康达物流"));
    allExpress.push(new ExpressCompany("fenghuangkuaidi", "凤凰快递"));
    allExpress.push(new ExpressCompany("feikuaida", "飞快达"));
    allExpress.push(new ExpressCompany("guotongkuaidi", "国通快递"));
    allExpress.push(new ExpressCompany("ganzhongnengda", "港中能达物流"));
    allExpress.push(new ExpressCompany("guangdongyouzhengwuliu", "广东邮政物流"));
    allExpress.push(new ExpressCompany("gongsuda", "共速达"));
    allExpress.push(new ExpressCompany("huitongkuaidi", "汇通快运"));
    allExpress.push(new ExpressCompany("hengluwuliu", "恒路物流"));
    allExpress.push(new ExpressCompany("huaxialongwuliu", "华夏龙物流"));
    allExpress.push(new ExpressCompany("haihongwangsong", "海红"));
    allExpress.push(new ExpressCompany("haiwaihuanqiu", "海外环球"));
    allExpress.push(new ExpressCompany("jiayiwuliu", "佳怡物流"));
    allExpress.push(new ExpressCompany("jinguangsudikuaijian", "京广速递"));
    allExpress.push(new ExpressCompany("jixianda", "急先达"));
    allExpress.push(new ExpressCompany("jjwl", "佳吉物流"));
    allExpress.push(new ExpressCompany("jymwl", "加运美物流"));
    allExpress.push(new ExpressCompany("jindawuliu", "金大物流"));
    allExpress.push(new ExpressCompany("jialidatong", "嘉里大通"));
    allExpress.push(new ExpressCompany("jykd", "晋越快递"));
    allExpress.push(new ExpressCompany("kuaijiesudi", "快捷速递"));
    allExpress.push(new ExpressCompany("lianb", "联邦快递（国内）"));
    allExpress.push(new ExpressCompany("lianhaowuliu", "联昊通物流"));
    allExpress.push(new ExpressCompany("longbanwuliu", "龙邦物流"));
    allExpress.push(new ExpressCompany("lijisong", "立即送"));
    allExpress.push(new ExpressCompany("lejiedi", "乐捷递"));
    allExpress.push(new ExpressCompany("minghangkuaidi", "民航快递"));
    allExpress.push(new ExpressCompany("meiguokuaidi", "美国快递"));
    allExpress.push(new ExpressCompany("menduimen", "门对门"));
    allExpress.push(new ExpressCompany("ocs", "OCS"));
    allExpress.push(new ExpressCompany("peisihuoyunkuaidi", "配思货运"));
    allExpress.push(new ExpressCompany("quanchenkuaidi", "全晨快递"));
    allExpress.push(new ExpressCompany("quanfengkuaidi", "全峰快递"));
    allExpress.push(new ExpressCompany("quanjitong", "全际通物流"));
    allExpress.push(new ExpressCompany("quanritongkuaidi", "全日通快递"));
    allExpress.push(new ExpressCompany("quanyikuaidi", "全一快递"));
    allExpress.push(new ExpressCompany("rufengda", "如风达"));
    allExpress.push(new ExpressCompany("santaisudi", "三态速递"));
    allExpress.push(new ExpressCompany("shenghuiwuliu", "盛辉物流"));
    allExpress.push(new ExpressCompany("shentong", "申通快递"));
    allExpress.push(new ExpressCompany("shunfeng", "顺丰速运"));
    allExpress.push(new ExpressCompany("sue", "速尔物流"));
    allExpress.push(new ExpressCompany("shengfeng", "盛丰物流"));
    allExpress.push(new ExpressCompany("saiaodi", "赛澳递"));
    allExpress.push(new ExpressCompany("tiandihuayu", "天地华宇"));
    allExpress.push(new ExpressCompany("tiantian", "天天快递"));
    allExpress.push(new ExpressCompany("tnt", "TNT"));
    allExpress.push(new ExpressCompany("ups", "UPS"));
    allExpress.push(new ExpressCompany("wanjiawuliu", "万家物流"));
    allExpress.push(new ExpressCompany("wenjiesudi", "文捷航空速递"));
    allExpress.push(new ExpressCompany("wuyuan", "伍圆"));
    allExpress.push(new ExpressCompany("wxwl", "万象物流"));
    allExpress.push(new ExpressCompany("xinbangwuliu", "新邦物流"));
    allExpress.push(new ExpressCompany("xinfengwuliu", "信丰物流"));
    allExpress.push(new ExpressCompany("yafengsudi", "亚风速递"));
    allExpress.push(new ExpressCompany("yibangwuliu", "一邦速递"));
    allExpress.push(new ExpressCompany("youshuwuliu", "优速物流"));
    allExpress.push(new ExpressCompany("youzhengguonei", "邮政包裹挂号信"));
    allExpress.push(new ExpressCompany("youzhengguoji", "邮政国际包裹挂号信"));
    allExpress.push(new ExpressCompany("yuanchengwuliu", "远成物流"));
    allExpress.push(new ExpressCompany("yuantong", "圆通速递"));
    allExpress.push(new ExpressCompany("yuanweifeng", "源伟丰快递"));
    allExpress.push(new ExpressCompany("yuanzhijiecheng", "元智捷诚快递"));
    allExpress.push(new ExpressCompany("yunda", "韵达快运"));
    allExpress.push(new ExpressCompany("yuntongkuaidi", "运通快递"));
    allExpress.push(new ExpressCompany("yuefengwuliu", "越丰物流"));
    allExpress.push(new ExpressCompany("yad", "源安达"));
    allExpress.push(new ExpressCompany("yinjiesudi", "银捷速递"));
    allExpress.push(new ExpressCompany("zhaijisong", "宅急送"));
    allExpress.push(new ExpressCompany("zhongtiekuaiyun", "中铁快运"));
    allExpress.push(new ExpressCompany("zhongtong", "中通速递"));
    allExpress.push(new ExpressCompany("zhongyouwuliu", "中邮物流"));
    allExpress.push(new ExpressCompany("zhongxinda", "忠信达"));
    allExpress.push(new ExpressCompany("zhimakaimen", "芝麻开门"));
    allExpress.push(new ExpressCompany("jingdongkuaidi", "京东快递"));

    function getKd100Type(expCompany) {
        if (!expCompany) return "";
        expCompany = expCompany.toLowerCase().split("快递")[0].split("速递")[0].split("物流")[0].replace(" ", '');
        for (var i = 0; i < allExpress.length; i++) {
            if (allExpress[i].expName.toLowerCase().indexOf(expCompany) > -1) {
                return allExpress[i].kd100Type;
            }
        }
        return "";
    }

    var expRoute = {};
    expRoute.getKd100Type = getKd100Type;
    expRoute.getKd100Url = function (expCompany, expOrderNo) {
        var type = getKd100Type(expCompany);
        var url = "http://m.kuaidi100.com/index_all.html?type=" + type + "&postid=" + expOrderNo + "&callbackurl=" + encodeURIComponent(document.location.href);
        return url;
    }
    return expRoute;
} ());