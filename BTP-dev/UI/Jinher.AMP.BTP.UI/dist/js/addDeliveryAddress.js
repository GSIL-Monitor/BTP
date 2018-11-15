new Vue({
    el: '#app',
    mounted: function () {
        this.$nextTick(function () {
            saveContextDTOByUrl();
            if (!JsVilaDataNull(sessionStorage.userLoginOut)) {
                saveContextDTOByUrl();
            }
            this.provinces = getAllProvinces();
            if (getQueryString('opa') == 'edit') {
                this.addressInfo = JSON.parse(addressJsonString);
                this.initData(this.addressInfo);
            }
            this.protype = getQueryString('producttype');
        });
    },
    data: function () {
        return {
            addressInfo: {}, //地址的所有信息（编辑状态需要获取之前的信息）
            IsDefault: false,
            chooseOpen: false,
            provinces: [], //省
            cityLists: [], //市
            districtList: [], //区
            streetList: [], //街道
            selectProvince: '请选择', //选择的省
            selectProvinceCode: null,
            selectCity: '', //选择的市
            selectCityCode: null,
            selectDistrict: '', //选择的区
            selectDistrictCode: null,
            selectStreet: '', //选择的街道
            selectStreetCode: null,
            selectedIndex: 0,
            userName: '', //收货人姓名
            phone: '', //手机号码
            zipCode: '', //邮政编码
            addressLabel: '', //地址
            addressWhere: '', //详细地址
            transformX: '0px',
            protype: '',
            isedit: true,
            select: []
        }
    },
    methods: {
        initData: function (info) {
            this.userName = info.ReceiptUserName;
            this.phone = info.ReceiptPhone;
            this.zipCode = info.RecipientsZipCode;

            if (IsCheckCode(info.ProvinceCode)) {
                this.selectProvince = (info.Province == null ? "" : info.Province);
                this.selectProvinceCode = info.ProvinceCode;
            }
            else {
                this.isedit = false;
            }
            if (IsCheckCode(info.CityCode)) {
                this.selectCity = (info.City == null ? "" : info.City);
                this.selectCityCode = info.CityCode;
            }
            else {
                this.isedit = false;
            }
            if (IsCheckCode(info.DistrictCode)) {
                this.selectDistrict = (info.District == null ? "" : info.District);
                this.selectDistrictCode = info.DistrictCode;
            }
            else {
                this.isedit = false;
            }

            if (IsCheckCode(info.StreetCode)) {
                this.selectStreet = (info.Street == null ? "" : info.Street); //街道
                this.selectStreetCode = info.StreetCode; //街道code
            }

            if (this.isedit == true) {
                this.addressLabel = this.selectProvince + this.selectCity + this.selectDistrict + (this.selectStreet == null ? "" : this.selectStreet);
            }
            else {
                this.addressLabel = "";
            }
            this.addressWhere = info.ReceiptAddress;
            this.IsDefault = info.IsDefault;
        },
        initAddressLabel: function () {
            this.addressLabel = this.selectProvince + this.selectCity + this.selectDistrict + this.selectStreet;
        },
        defaultAddress: function () {
            var self = this;
            setTimeout(function () {
                self.IsDefault = self.select.length ? true : false;
            })
        },
        /**
        * 点击已经选择的地址页签title
        */
        addressClick: function (index) {
            this.transformX = (index * -25 + '%');
            this.selectedIndex = index;
        },
        /**
        * 切换了省份
        */
        provinceChange: function (code, name) {
            this.cityLists = getAddressInfo(code, 2);
            this.transformX = '-25%';
            this.selectedIndex = 1;
            this.selectCity = '请选择';
            this.selectCityCode = null;
            this.selectDistrict = '';
            this.selectDistrictCode = null;
            this.selectStreet = '';
            this.selectStreetCode = null;
            this.selectProvince = name;
            this.selectProvinceCode = code;
        },
        /**
        * 切换了市，获取所在的区
        */
        cityChange: function (code, name) {
            this.selectCity = name;
            this.selectCityCode = code;
            this.selectDistrict = '';
            this.selectDistrictCode = null;
            this.selectStreet = '';
            this.selectStreetCode = null;
            this.districtList = getAddressInfo(code, 3);
            this.transformX = '-50%';
        },
        /**
        * 切换了区,获取所在的街道
        */
        districtChange: function (code, name) {
            this.streetList = getAddressInfo(code, 4);
            this.selectedIndex = 3;
            this.selectDistrict = name;
            this.selectDistrictCode = code;
            this.selectStreet = '';
            this.selectStreetCode = null;
            if (this.streetList.length) {
                this.transformX = '-75%';
            } else {
                this.chooseOpen = false;
                this.initAddressLabel();
            }
        },
        /**
        * 点击街道列表元素
        */
        selectedStreet: function (code, name) {
            this.streetChange(code, name);
            this.chooseOpen = false;
        },
        /**
        * 切换了街道
        */
        streetChange: function (code, name) {
            this.selectStreet = name;
            this.selectStreetCode = code;
            this.selectedIndex = 3;
            this.initAddressLabel();
        },
        chooseAddress: function (provinceCode, CityCode, DistrictCode) {
            this.$loading.open();
            if (this.provinces.length) {
                if (this.addressLabel) {//已选择
                    this.cityLists = getAddressInfo(provinceCode, 2);
                    this.districtList = getAddressInfo(CityCode, 3);
                    this.streetList = getAddressInfo(DistrictCode, 4);
                    this.$loading.close();
                    this.chooseOpen = true;
                } else { //未选择
                    this.$loading.close();
                    this.selectProvince = '请选择';
                    this.selectedIndex = 0;
                    this.transformX = '0%';
                    this.selectCity = this.selectDistrict = this.selectStreet = '';
                    this.selectProvinceCode = this.selectCityCode = this.selectDistrictCode = this.selectStreetCode = null;
                    this.cityLists = this.districtList = this.streetList = [];
                    var _this = this;
                    setTimeout(function () {
                        _this.chooseOpen = true;
                    }, 100);
                }
            }
        },
        check: function () {
            if (this.userName.trim() == '') {
                this.$toast("收货人姓名不能为空!");
                return false;
            }
            if (this.phone.trim() == '') {
                this.$toast("手机号不能为空!");
                return false;
            } else if (!this.phone.trim().match(/^1/) || this.phone.length != 11) {
                this.$toast("请填写正确手机号!");
                return false;
            }
            if (this.addressLabel == '') {
                this.$toast("请选择地区!");
                return false;
            }
            if (this.addressWhere.trim() == '') {
                this.$toast("详细地址不能为空!");
                return false;
            }
            if (this.zipCode == '') {
                this.$toast("邮政编码不能为空!");
                return false;
            }
            var re = /^[[0-9]{6}$/;
            if (!re.test(this.zipCode)) {
                this.$toast("邮政编码不正确!");
                return false;
            }
            return true;
        },
        /**
        * 保存地址
        */
        save: function () {
            if (!isLogin()) {
                this.$toast("请先登录！");
                return;
            }
            var flag = this.check();
            if (flag) {
                this.$loading.open('保存中...');
                this.addressInfo.UserId = getUserId(); //userId
                this.addressInfo.ReceiptUserName = this.userName; //用户名
                this.addressInfo.ReceiptPhone = this.phone; //手机
                this.addressInfo.RecipientsZipCode = this.zipCode; //邮政编码
                this.addressInfo.Province = this.selectProvince; // 省
                this.addressInfo.ProvinceCode = this.selectProvinceCode; //省 code
                this.addressInfo.City = this.selectCity; //市
                this.addressInfo.CityCode = this.selectCityCode; //市code
                this.addressInfo.District = this.selectDistrict; //区
                this.addressInfo.DistrictCode = this.selectDistrictCode; //区code
                this.addressInfo.Street = this.selectStreet; //街道
                this.addressInfo.StreetCode = this.selectStreetCode; //街道code
                this.addressInfo.ReceiptAddress = this.addressWhere; //详细地址
                this.addressInfo.IsDefault = this.IsDefault; //是否设置默认
                this.addressInfo.IsSelect = false;
                if (getQueryString('opa') == "add") {
                    this.addressInfo.AppId = getEsAppId();
                }
                this.$http.post('/Mobile/SaveDeliveryAddress', this.addressInfo, { emulateJSON: true }).then(function (response) {
                    if (response.data.ResultCode == 0) {
                        this.$loading.close();
                        var catering = getQueryString('catering');
                        var par = "&type=" + sessionStorage.type + "&price=" + getQueryString('price') + "&producttype=" + this.protype + "&co=" + getQueryString('co');
                        if (JsVilaDataNull(catering) && catering == "1") {
                            par = par + "&catering=1";
                        }
                        if (getQueryString("setMealId") != null) {
                            par = par + "&setMealId=" + getQueryString("setMealId");
                        }
                        if (getQueryString("jcActivityId") != null) {
                            par = par + "&jcActivityId=" + getQueryString("jcActivityId");
                        }
                        if (document.referrer.indexOf("CreateOrder") >= 0) {
                            var gourl = JsVilaDataNull(catering) && catering == "1" ? "/Mobile/CYCreateOrder" : "/Mobile/CreateOrder";
                            window.location.href = urlAppendCommonParams(gourl + "?price=" + getQueryString('price') + "&type=" + sessionStorage.type + "&setMealId=" + getQueryString("setMealId") + "&jcActivityId=" + getQueryString("jcActivityId"));
                        } else {
                            window.location.href = urlAppendCommonParams("/Mobile/DeliveryAddressList?opa=add&addressid=" + par);
                        }
                    } else {
                        this.addressLabel = "";
                        this.$toast('请重新选择所在地区！');
                        this.$loading.close();
                    }
                }, function (err) {
                    this.$toast('保存失败！');
                    this.$loading.close();
                });

            }
        }
    }
});
