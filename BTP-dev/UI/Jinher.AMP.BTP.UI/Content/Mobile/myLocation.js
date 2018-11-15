 //A->AreaCode:地区代码
//N->Name:地区名称
//S->SpellCode:拼音
//L->Level:级别1省，2市，3，区
var ChineseAreas = [];

function checkAreaCode(areaCode) {
    var testReg = /^\d{6}$/;
    if (testReg.test(areaCode)) return true;
    return false;
}
function buildAreaModel(obj) {
    var result = {};
    result.AreaCode = obj.A;
    result.Name = obj.N;
    result.Level = obj.L;
    result.SpellCode = obj.S;
    return result;
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
//获取所有城市信息
//return AreaCode、Name、Level、SpellCode
function getAllCities() {
    var arr = [];
    for (var i = 0; i < ChineseAreas.length; i++) {
        if (ChineseAreas[i].L == 2) {
            arr.push(buildAreaModel(ChineseAreas[i]));
        }
    }
    return arr;
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

//根据城市代码获取所在省信息
//return AreaCode、Name、Level、SpellCode
function getProvinceByCityCode(areaCode) {
    var result = {};
    if (!checkAreaCode(areaCode)) {
        return result;
    }
    var provinceCode = areaCode.substr(0, 2) + "0000";
    for (var i = 0; i < ChineseAreas.length; i++) {
        if (ChineseAreas[i].A == provinceCode) {
            return buildAreaModel(ChineseAreas[i]);
        }
    }
    return result;
}

//根据省代码获取辖区内所有城市信息
//return AreaCode、Name、Level、SpellCode
function getProvinceCities(provinceCode) {
    var cities = [];
    if (!checkAreaCode(provinceCode)) {
        return cities;
    }
    var provinceCodeSub = provinceCode.substr(0, 2);
    for (var i = 0; i < ChineseAreas.length; i++) {
        if (ChineseAreas[i].L == 2 && ChineseAreas[i].A.substr(0, 2) == provinceCodeSub) {
            cities.push(buildAreaModel(ChineseAreas[i]));
        }
    }
    return cities;
}
function getCityCode(cityName) {
    var cityCode='';
    var allCities = [];
    allCities = getAllCities();
    for (var i = 0, n = allCities.length; i < n; i++) {
        if (cityName == allCities[i].Name) {
            cityCode = allCities[i].AreaCode;
        }
    }
    return cityCode;
}