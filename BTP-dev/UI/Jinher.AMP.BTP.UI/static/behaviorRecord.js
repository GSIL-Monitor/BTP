//sys_name | service_type | oper_type | item_id | desc_info
function logBTP(srcTypeId, service_type, oper_type, item_id, desc_info) {
    try {
        if (sessionStorage.speader && sessionStorage.speader != '') {
            desc_info = (desc_info || '') + "spreadCode:" + (sessionStorage.speader || '') + "|";
        }
        log(getSysName(srcTypeId), service_type, oper_type, item_id, desc_info);
    } catch (e) {
    }
}
function getSysName(srcTypeId) {
    if (!srcTypeId || srcTypeId == "null" || srcTypeId == '' || srcTypeId == "undefined") {
        srcTypeId = 34;
    }
    var sys_name = "";
    var tmp = parseInt(srcTypeId).toString(16);
    if (tmp.length == 1) {
        sys_name = "0x000" + tmp;
    } else {
        sys_name = "0x00" + tmp;
    }
    return sys_name;
}
window.onload = function(){
    var descinfo = '';
    if ("undefined" != typeof (logOrderId) && logOrderId && logOrderId != '') {
        descinfo += logOrderId;
    }
    var pageSysname = '';
    if ("undefined" != typeof (sysname) && sysname && sysname != '') {
        pageSysname = sysname;

    }
    logBTP(pageSysname || sessionStorage.SrcType, service_type || '', "0x0001", getQueryString('commodityId') || '', descinfo);
};
