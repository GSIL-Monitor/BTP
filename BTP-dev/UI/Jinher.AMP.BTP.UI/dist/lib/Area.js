var ChineseAreas = new Array();

ChineseAreas = ChineseAreas.concat(AreaFirst, AreaSecond, AreaThree);

function checkAreaCode(areaCode) {
    if (!areaCode) return false;
    return true;
}

function AreaModel(areaCode, name, level) {
    this.AreaCode = areaCode;
    this.Name = name;
    this.Level = level;
}

function buildAreaModel(obj) {
    return new AreaModel(obj.A, obj.N, obj.L);
}

//根据地区code获取地区信息
//return AreaCode、Name、Level、SpellCode
function getAreaInfoByCode(areaCode) {
    var result = {};
    if (!checkAreaCode(areaCode)) {
        return result;
    }
    for (var i = 0; i < ChineseAreas.length; i++) {
        if (areaCode == ChineseAreas[i].A) {
            return buildAreaModel(ChineseAreas[i]);
        }
    }
    return result;
}

//获取所有省级信息
//return AreaCode、Name、Level、SpellCode
function getAllProvinces() {
    var arr = [];
    for (var i = 0; i < ChineseAreas.length; i++) {
        if (ChineseAreas[i].L == 1) {
            arr.push(buildAreaModel(ChineseAreas[i]));
        }
    }
    return arr;
}

//根据省代码获取辖区内所有城市信息
//return AreaCode、Name、Level、SpellCode
function getAddressInfo(Code, level) {
    var cities = [];
    if (!checkAreaCode(Code)) {
        return cities;
    }
    for (var i = 0; i < ChineseAreas.length; i++) {
        if (ChineseAreas[i].L == level && ChineseAreas[i].P == Code) {
            cities.push(buildAreaModel(ChineseAreas[i]));
        }
    }
    return cities;
}


//判断code是否存在
function IsCheckCode(Code) {
    var falg = false;
    for (var i = 0; i < ChineseAreas.length; i++) {
        if (ChineseAreas[i].A == Code) {
            falg = true;
        }
    }
    return falg;
}