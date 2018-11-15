var app = new Vue({
    el: '#app',
    mounted: function () {
        this.$nextTick(function () {
            document.domain = "iuoooo.com";
            this.AppId = GetQueryString("appId");
            //获取快递公司
            this.getExpCompany();
            //获取所有模板
            this.initData();
            var _this = this;
            window.addEventListener('mousemove', function (e) {
                if (_this.allowDrag) {
                    var posX = Number(_this.selectItem.Left) + (Math.ceil(e.pageX / 3.78) - _this.lastPosX);
                    var posY = Number(_this.selectItem.Top) + (Math.ceil(e.pageY / 3.78) - _this.lastPosY);
                    if (posX <= 0) {
                        _this.selectItem.Left = 0;
                    } else if (posX + Number(_this.selectItem.Width) >= Number(_this.curSelect.Width)) {
                        _this.selectItem.Left = _this.curSelect.Width - _this.selectItem.Width;
                    } else {
                        _this.selectItem.Left = posX;
                    }
                    if (posY <= 0) {
                        _this.selectItem.Top = 0;
                    } else if (posY + Number(_this.selectItem.Height) >= _this.curSelect.Height) {
                        _this.selectItem.Top = _this.curSelect.Height - _this.selectItem.Height;
                    } else {
                        _this.selectItem.Top = posY;
                    }
                    _this.lastPosX = Math.ceil(e.pageX / 3.78);
                    _this.lastPosY = Math.ceil(e.pageY / 3.78);
                }
                if (_this.isResize) {
                    switch (_this.resizeState) {
                        case 'left':
                            _this.selectItem.Width = parseInt(_this.selectItem.Width - parseInt(Math.ceil(e.pageX / 3.78) - _this.lastPosX));
                            _this.selectItem.Left = parseInt(Number(_this.selectItem.Left) + parseInt(Math.ceil(e.pageX / 3.78) - _this.lastPosX));
                            _this.lastPosX = Math.ceil(e.pageX / 3.78);
                            break;
                        case 'right':
                            _this.selectItem.Width = parseInt(Number(_this.selectItem.Width) + parseInt(Math.ceil(e.pageX / 3.78) - _this.lastPosX));
                            _this.lastPosX = Math.ceil(e.pageX / 3.78);
                            break;
                        case 'top':
                            _this.selectItem.Height = parseInt(_this.selectItem.Height - parseInt(Math.ceil(e.pageY / 3.78) - _this.lastPosY));
                            _this.selectItem.Top = parseInt(Number(_this.selectItem.Top) + parseInt(Math.ceil(e.pageY / 3.78) - _this.lastPosY));
                            _this.lastPosY = Math.ceil(e.pageY / 3.78);
                            break;
                        case 'bottom':
                            _this.selectItem.Height = parseInt(Number(_this.selectItem.Height) + parseInt(Math.ceil(e.pageY / 3.78) - _this.lastPosY));
                            _this.lastPosY = Math.ceil(e.pageY / 3.78);
                            break;
                    }
                }
            }, false);
            window.addEventListener('mouseup', function () {
                _this.isResize = false;
                _this.allowDrag = false;
                _this.lastPosX = 0;
                _this.lastPosY = 0;
            }, false);
        });
    },
    data: {
        maskShow: false,
        PropertyLists: [//设置打印内容列表
            {
            Name: '收件人信息',
            Code: '',
            Content: [
                    {
                        Name: '姓名', //打印属性名
                        Id: 'ReceiptUserName',
                        Default: true, //打印属性是否是默认
                        Checked: true //是否选中
                    }, {
                        Name: '电话', //打印属性名
                        Id: 'ReceiptPhone',
                        Default: true, //打印属性是否是默认
                        Checked: true //是否选中
                    }, {
                        Name: '邮编', //打印属性名
                        Id: 'RecipientsZipCode',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '收件人地址', //打印属性名
                        Id: 'ReceiptAddress',
                        Default: true, //打印属性是否是默认
                        Checked: true //是否选中
                    }
                ]
        }, {
            Name: '发件人信息',
            Content: [
                    {
                        Name: '姓名', //打印属性名
                        Id: 'SenderName',
                        Default: true, //打印属性是否是默认
                        Checked: true //是否选中
                    }, {
                        Name: '电话', //打印属性名
                        Id: 'SenderPhone',
                        Default: true, //打印属性是否是默认
                        Checked: true //是否选中
                    }, {
                        Name: '邮编', //打印属性名
                        Id: 'SendPostCode',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '单位名称', //打印属性名
                        Id: 'SenderCompany',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '发件人地址', //打印属性名
                        Id: 'SenderAddress',
                        Default: true, //打印属性是否是默认
                        Checked: true //是否选中
                    }
                ]
        }, {
            Name: '商品信息',
            Content: [
                    {
                        Name: '商品名称', //打印属性名
                        Id: 'CommodityName',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '商品编码', //打印属性名
                        Id: 'CommodityCode',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '商品属性（仅显示值）', //打印属性名
                        Id: 'CommodityAttributes',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '购买数量', //打印属性名
                        Id: 'Number',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }
                ]
        }, {
            Name: '订单信息',
            Content: [
                    {
                        Name: '订单编号', //打印属性名
                        Id: 'OrderCode',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '买家留言', //打印属性名
                        Id: 'BuyersRemark',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }, {
                        Name: '卖家留言', //打印属性名
                        Id: 'SellersRemark',
                        Default: false, //打印属性是否是默认
                        Checked: false //是否选中
                    }
                ]
        }
        ],
        printTips: [ //直接打印列表
            {
            Name: '直接打印1', //打印属性名
            Id: 'DirectPrint1',
            Context: "谢谢您的选择，回馈送好礼",
            Checked: false //是否选中true
        }, {
            Name: '直接打印2', //打印属性名
            Id: 'DirectPrint2',
            Context: "",
            Checked: false //是否选中true
        }, {
            Name: '直接打印3', //打印属性名
            Id: 'DirectPrint3',
            Context: "",
            Checked: false //是否选中true
        }
        ],
        DefaultPoperty: [//打印内容单元
                            {
                            PropertyName: 'SenderName',
                            Width: 18, //宽
                            Height: 6, //高
                            Top: 41, //上边距
                            Left: 94 //左边距
                        },
                            {
                                PropertyName: 'SenderPhone',
                                Width: 47, //宽
                                Height: 5, //高
                                Top: 53, //上边距
                                Left: 30 //左边距
                            },
                            {
                                PropertyName: 'SenderAddress',
                                Width: 82, //宽
                                Height: 6, //高
                                Top: 46, //上边距
                                Left: 30 //左边距
                            },
                            {
                                PropertyName: 'ReceiptUserName', //收件人名，
                                Width: 18, //宽
                                Height: 5, //高
                                Top: 58, //上边距
                                Left: 94 //左边距
                            },
                            {
                                PropertyName: 'ReceiptPhone', //收件人电话，
                                Width: 82, //宽
                                Height: 6, //高
                                Top: 76, //上边距
                                Left: 30 //左边距
                            },
                            {
                                PropertyName: 'ReceiptAddress', //收件人地址，
                                Width: 82, //宽
                                Height: 5, //高
                                Top: 64, //上边距
                                Left: 30 //左边距
                            }
                        ],
        AppId: 'a7ca8d4d-3685-426d-b78e-e035798e6424', //应用ID
        lists: [], //快递单模板列表
        expCompany: [], //快递公司
        checkPropertyNames: [],
        curSelectIndex: 0, //当前选中的模板索引值（默认为第一个）
        curSelect: {}, //当前选中的模板
        selectProperty: [], //当前选中模板的打印属性列表
        createTemName: '', //新建模板的名称
        selectExpCode: '', //已选择的快递公司编码
        createTemExpPic: '', //新建模板的背景图
        createTemExpPicFile: '', //新建模板的背景图文件对象
        createTemWidth: 0, //新建模板的宽度（单位毫米）
        createTemHeight: 0, //新建模板的高度（单位毫米）
        lastPosX: 0,
        lastPosY: 0,
        selectItem: null,
        selectItemIndex: null,
        allowDrag: false,
        sysTemplate: true, //是否是系统模板
        isResize: false,
        resizeState: '',
        addModal: false,
        propertyModal: false,
        commonTemplatesModal: false,
        commonTemplatesLists: [], //常用模板列表
        selectComTempIndex: 0, //常用模板选中的索引值
        selectTemplatesLists: [], //可选模板列表
        selectTempIndex: 0, //可选模板选中的索引值
        dialogShow: false,
        showMask: false,
        isEditTemplate: false //判断是添加还是编辑模板
    },
    methods: {
        initData: function () {
            var _this = this;
            this.expressOrderTemplate(function (data) {
                if (data.isSuccess) {
                    if (data.Data.length) {
                        _this.lists = data.Data;
                        _this.curSelect = _this.lists[0];
                        _this.selectProperty = [];
                        for (var i = 0; i < _this.curSelect.Property.length; i++) {
                            var item = _this.curSelect.Property[i];
                            _this.selectProperty.push(_this.clone(item));
                        }
                        _this.sysTemplate = (_this.curSelect.TemplateType == 1) ? false : true;
                    }
                } else {
                    var message = data.Message || '获取快递模板失败!';
                    toast(message);
                }
            }, function (err) {
                toast('获取快递模板失败,请稍候再试！');
            });
        },
        //获取所有快递模板
        expressOrderTemplate: function (successCallback, errCallback) {
            //调用接口
            this.$http({
                url: '/ExpressOrderTemplate/GetAll',
                method: 'POST',
                params: { appId: this.AppId }
            }).then(function (response) {//成功回调
                if (typeof successCallback == 'function') {
                    successCallback(response.data);
                }
            }, function (err) {//失败回调
                if (typeof errCallback == 'function') {
                    errCallback();
                }
            });
        },
        /*获取快递公司*/
        getExpCompany: function () {
            this.$http({
                url: '/Express/GetAll',
                method: 'POST'
            }).then(function (response) {
                if (response.data.isSuccess) {
                    if (response.data.Data.length) {
                        this.expCompany = response.data.Data;
                    }
                } else {
                    var message = response.data.Message || '获取快递公司失败!';
                    toast(message);
                }
            }, function (err) {
                toast('获取快递公司失败,请稍后从试！');

                //测试代码,开发时需删掉
                this.expCompany = [{ ExpCode: '1', ExpCompanyName: 'EMS' }, { ExpCode: '2', ExpCompanyName: '天天快递' }, { ExpCode: '3', ExpCompanyName: '顺丰快递'}];
            });
        },
        /*新建模板*/
        addTemplate: function () {
            //this.close();//关闭弹窗
            var formData = new FormData();
            formData.append('file', this.createTemExpPicFile);
            formData.append('AppId', this.AppId);
            formData.append('TemplateName', this.createTemName);
            formData.append('Width', ((this.createTemWidth / 25.4) * 96).toFixed(0));
            formData.append('Height', ((this.createTemHeight / 25.4) * 96).toFixed(0));
            formData.append('ExpressCode', this.selectExpCode);

            this.$http.post('/ExpressOrderTemplate/Save', formData).then(function (response) {
                if (response.data.isSuccess) {
                    toast('新建模板成功');
                    this.selectProperty = [];
                    //                    for (var i = 0; i < this.DefaultPoperty.length; i++) {
                    //                        var item = this.DefaultPoperty[i];
                    //                        this.selectProperty.push(this.clone(item));
                    //                    }

                    this.lists.unshift(response.data.Data);
                    this.curSelect = this.lists[0];
                    this.curSelectIndex = 0;
                    this.close();

                } else {
                    var message = response.data.Message || '新建模板失败!';
                    toast(message);
                }
            }, function (err) {
                toast('新建模板失败，请稍后从试！');
            });
        },
        /*删除模板*/
        cancelTemplate: function () {
            if (this.curSelect && this.curSelect.Id) {
                this.dialogShow = false;
                this.$http.post('/ExpressOrderTemplate/Remove', { Id: this.curSelect.Id, AppId: this.AppId }).then(function (response) {
                    if (response.data.isSuccess) {
                        toast('删除成功!');
                        this.lists.splice(this.curSelectIndex, 1);
                        this.curSelect = this.lists[0];
                        // this.selectProperty = this.curSelect.Property;
                        this.curSelectIndex = 0;
                    } else {
                        var message = response.data.Message || '删除失败!';
                        toast(message);
                    }
                }, function (err) {
                    toast('删除失败!');
                });
            }
        },
        /*编辑模板*/
        editTemplate: function () {
            if (this.curSelect && this.curSelect.Id) {
                var formData = new FormData();
                if (this.createTemExpPicFile && this.createTemExpPicFile != '') {
                    formData.append('file', this.createTemExpPicFile);
                }
                formData.append('Id', this.curSelect.Id);
                formData.append('AppId', this.AppId);
                formData.append('TemplateName', this.createTemName);
                formData.append('Width', ((this.createTemWidth / 25.4) * 96).toFixed(0));
                formData.append('Height', ((this.createTemHeight / 25.4) * 96).toFixed(0));
                formData.append('ExpressCode', this.selectExpCode);

                this.$http.post('/ExpressOrderTemplate/Save', formData).then(function (response) {
                    this.close(); //关闭弹窗
                    if (response.data.isSuccess) {
                        this.lists[this.curSelectIndex].Id = response.data.Data.Id;
                        this.lists[this.curSelectIndex].TemplateName = response.data.Data.TemplateName;
                        this.lists[this.curSelectIndex].ExpressImage = response.data.Data.ExpressImage;
                        this.lists[this.curSelectIndex].Width = response.data.Data.Width;
                        this.lists[this.curSelectIndex].Height = response.data.Data.Height;
                        this.lists[this.curSelectIndex].ExpCode = response.data.Data.ExpressCode;
                    } else {
                        var message = response.data.Message || '修改模板失败!';
                        toast(message);
                    }
                }, function (err) {
                    toast('修改模板失败!');
                });
            }
        },
        //点击模板列表
        listClick: function (index) {
            if (index !== this.curSelectIndex) {
                this.curSelectIndex = index;
                this.curSelect = this.lists[index];
                this.selectProperty = [];
                if (this.curSelect.Property) {
                    for (var i = 0; i < this.curSelect.Property.length; i++) {
                        var item = this.curSelect.Property[i];
                        this.selectProperty.push(this.clone(item));
                    }
                }

                this.sysTemplate = (this.curSelect.TemplateType == 1) ? false : true;
            }
        },
        //点击新建按钮
        createTemplate: function () {
            this.addModal = true;
            this.isEditTemplate = false;
            //情空上传数据
            this.$refs.inputFile.value = '';
            this.createTemExpPicFile = '';
        },
        //关闭新建/编辑 弹窗
        close: function () {
            this.createTemName = '';
            this.selectExpCode = '';
            this.createTemExpPic = '';
            this.createTemWidth = 0;
            this.createTemHeight = 0;
            this.addModal = false;
        },
        //点击编辑模板
        editTemplateClick: function () {
            if (this.sysTemplate) {
                return;
            }

            //情空上传数据
            this.$refs.inputFile.value = '';
            this.createTemExpPicFile = '';
            this.createTemName = this.curSelect.TemplateName;
            this.selectExpCode = this.curSelect.ExpCode;
            this.createTemExpPic = this.curSelect.ExpressImage;
            this.createTemWidth = ((this.curSelect.Width / 96) * 25.4).toFixed(0);
            this.createTemHeight = ((this.curSelect.Height / 96) * 25.4).toFixed(0);
            this.addModal = true;
            this.isEditTemplate = true;
        },
        //删除模板
        cancelTemplateClick: function () {
            if (this.sysTemplate) {
                return;
            }
            this.dialogShow = true;
        },
        //点击创建模(编辑)板保存按钮
        createBtnClick: function () {
            if (this.createTemName == '') {
                toast('请输入模板名称');
                return;
            }
            if (this.createTemName.length > 30) {
                toast('模板名称最多30个字');
                return;
            }
            if (this.selectExpCode == '') {
                toast('请选择快递公司');
                return;
            }
            if (this.createTemExpPic == '') {
                toast('请上传快递单背景图');
                return;
            }
            if (!this.createTemWidth) {
                toast('请输入模板宽度');
                return;
            }
            if (!this.createTemHeight) {
                toast('请输入模板高度');
                return;
            }
            this.isEditTemplate ? this.editTemplate() : this.addTemplate();
        },
        //点击设置打印内容按钮
        openPropertyModal: function () {
            if (this.sysTemplate) {
                return;
            }

            this.checkPropertyNames = [];
            if (this.selectProperty.length) {
                for (var i = 0; i < this.selectProperty.length; i++) {
                    this.checkPropertyNames.push(this.selectProperty[i].PropertyName)
                }
            }
            else {
                this.checkPropertyNames.push("ReceiptUserName");
                this.checkPropertyNames.push("ReceiptPhone");
                this.checkPropertyNames.push("ReceiptAddress");
                this.checkPropertyNames.push("SenderName");
                this.checkPropertyNames.push("SenderPhone");
                this.checkPropertyNames.push("SenderAddress");
            }

            this.propertyModal = true;
        },
        //设置打印弹窗确定按钮
        propertySubmit: function () {
            var arr = [];
            for (var i = 0; i < this.checkPropertyNames.length; i++) {
                var value = this.checkPropertyNames[i];
                var flag = false;
                for (var j = 0; j < this.selectProperty.length; j++) {
                    if (this.selectProperty[j].PropertyName == value) {
                        arr.push(this.selectProperty[j]);
                        flag = true;
                        break;
                    }
                }

                if (!flag) {
                    for (var j = 0; j < this.printTips.length; j++) {
                        if (this.printTips[j].Id == value) {
                            if (this.printTips[j].Context == "") {
                                toast("请设置直接打印内容");
                                throw "---";
                            }
                            arr.push({ PropertyName: value, PropertyText: this.printTips[j].Context, Width: 31, Height: 6, Top: 0, Left: 0 });
                            flag = true;
                            break;
                        }
                    }
                }

                if (!flag) {
                    arr.push({ PropertyName: value, PropertyText: "", Width: 31, Height: 6, Top: 0, Left: 0 })
                    //  arr.push({ PropertyName: value, Width: 31, Height: 6, Top: 0, Left: 0 })
                }
            }
            this.selectProperty = arr;
            this.propertyModal = false;

        },
        CloseDialog: function () {
            window.parent.CloseExpressTemplateSet();
        },
        //保存打印内容
        saveProperty: function () {
            this.maskShow = true;
            var param = {
                TemplateId: this.curSelect.Id, //模板id
                Property: this.selectProperty
            };
            //            var _curSelect = this.curSelect;
            //            var _selectProperty = this.selectProperty;

            this.$http.post('/ExpressOrderTemplate/SaveProperty', param).then(function (response) {
                this.maskShow = false;
                if (response.data.isSuccess) {
                    toast('保存成功');
                    if (!this.curSelect.Property) this.curSelect.Property = [];
                    for (var i = 0; i < this.selectProperty.length; i++) {
                        var item = this.selectProperty[i];
                        this.curSelect.Property.push(this.clone(item));
                    }
                } else {
                    var message = response.data.Message || '保存失败!';
                    toast(message);
                }
            }, function (err) {
                toast('保存失败，请稍后重试！');
                this.maskShow = false;
            });
        },
        //点击设置常用模板按钮
        setCommonTemplates: function () {
            this.$http({
                url: '/ExpressOrderTemplate/GetUsed',
                method: 'POST',
                params: { AppId: this.AppId },
                before: function () {
                    this.maskShow = true;
                }
            }).then(function (response) {
                this.maskShow = false;
                if (response.data.isSuccess) {
                    this.maskShow = false;
                    this.commonTemplatesModal = true;
                    this.commonTemplatesLists = [];
                    this.selectTemplatesLists = [];
                    if (response.data.Data.length) {
                        for (var i = 0; i < this.lists.length; i++) {
                            var id = this.lists[i].Id;
                            var flag = false;
                            for (var j = 0; j < response.data.Data.length; j++) {
                                if (response.data.Data[j] == id) {
                                    this.commonTemplatesLists.push({ Id: id, TemplateName: this.lists[i].TemplateName, TemplateType: this.lists[i].TemplateType });
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag) {
                                this.selectTemplatesLists.push({ Id: id, TemplateName: this.lists[i].TemplateName, TemplateType: this.lists[i].TemplateType });
                            }
                        };
                    } else {
                        for (var i = 0; i < this.lists.length; i++) {
                            this.selectTemplatesLists.push({ Id: this.lists[i].Id, TemplateName: this.lists[i].TemplateName, TemplateType: this.lists[i].TemplateType });
                        };
                    }
                    if (this.selectTemplatesLists.length) {
                        this.selectComTempIndex = 0;
                    } else {
                        this.selectTemplatesLists = null;
                    }
                } else {
                    var message = response.data.Message || '获取常用模板失败';
                    toast(message);
                }
            }, function (err) {
                toast('获取常用模板失败，请稍候再试！');
            });
        },
        changeComTemplate: function (state) {
            if (state == "right") {
                if (this.selectComTempIndex == null) { return; }
                this.selectTemplatesLists.push(this.commonTemplatesLists[this.selectComTempIndex]);
                this.commonTemplatesLists.splice(this.selectComTempIndex, 1);
                if (this.selectTempIndex == null) {
                    this.selectTempIndex = 0;
                }
                if (this.commonTemplatesLists.length && (this.commonTemplatesLists.length <= this.selectComTempIndex)) {
                    this.selectComTempIndex = this.commonTemplatesLists.length - 1;
                } else if (this.commonTemplatesLists.length <= 0) {
                    this.selectComTempIndex = null;
                }
            }
            if (state == "left") {
                if (this.selectTempIndex == null) { return; }
                this.commonTemplatesLists.push(this.selectTemplatesLists[this.selectTempIndex]);
                this.selectTemplatesLists.splice(this.selectTempIndex, 1);
                if (this.selectComTempIndex == null) {
                    this.selectComTempIndex = 0;
                }
                if (this.selectTemplatesLists.length && (this.selectTemplatesLists.length <= this.selectComTempIndex)) {
                    this.selectTempIndex = this.selectTemplatesLists.length - 1;
                } else if (this.selectTemplatesLists.length <= 0) {
                    this.selectTempIndex = null;
                }
            }
        },
        chooseTemplate: function (state, index) {
            if (state == 'left') {
                this.selectTempIndex = index;
            } else if (state == 'right') {
                this.selectComTempIndex = index;
                console.log(this.selectComTempIndex, 'this.selectComTempIndex')
            } else if (state == 'up') {
                if (this.selectComTempIndex - 1 <= 0) {
                    this.selectComTempIndex = 0;
                } else {
                    this.selectComTempIndex--;
                }
            } else if (state == 'down') {
                if (this.selectComTempIndex + 1 >= this.commonTemplatesLists.length) {
                    this.selectComTempIndex = this.commonTemplatesLists.length - 1;
                } else {
                    this.selectComTempIndex++;
                }
            }
        },
        commonTemplatesSubmit: function () {
            var ids = [];
            for (var i = 0; i < this.commonTemplatesLists.length; i++) {
                ids.push(this.commonTemplatesLists[i].Id);
            }
            this.maskShow = true;
            this.$http.post('/ExpressOrderTemplate/SaveUsed', {
                AppId: this.AppId,
                Ids: ids
            }).then(function (response) {
                this.maskShow = false;
                if (response.data.isSuccess) {
                    toast('保存常用模板成功');
                    this.commonTemplatesModal = false;
                } else {
                    var message = response.data.Message || '保存常用模板失败';
                    toast(message);
                }
            }, function (err) {
                toast('保存常用模板失败,请稍候再试！');
                this.maskShow = false;
            });
        },
        //choose img
        onFileChange: function (e) {
            this.createTemExpPic = e.target.value;
            this.createTemExpPicFile = e.target.files[0];
        },
        //drag start
        dragstart: function (e, index, element) {
            if (this.sysTemplate) {
                return;
            }
            this.selectItem = element;
            this.selectItemIndex = index;
            if (this.selectItemIndex == index) {
                this.allowDrag = true;
                this.lastPosX = Math.ceil(e.pageX / 3.78);
                this.lastPosY = Math.ceil(e.pageY / 3.78);
            }
        },
        // resize start
        resizeLeftStart: function (e, index, state) {
            if (this.sysTemplate) {
                return;
            }
            this.isResize = true;
            this.resizeState = state;
            this.lastPosX = Math.ceil(e.pageX);
            this.lastPosY = Math.ceil(e.pageY);
        },
        inputChange: function (state) {
            if (state == 'width') {
                var value = parseInt(this.createTemWidth) || 0;
                if (value >= 1000000) {
                    this.createTemWidth = 999999;
                    toast('宽度最多输入6位数字');
                } else {
                    this.createTemWidth = value;
                }
            } else if (state == 'height') {
                var value = parseInt(this.createTemHeight) || 0;
                if (value >= 1000000) {
                    this.createTemHeight = 999999;
                    toast('高度最多输入6位数字');
                } else {
                    this.createTemHeight = value;
                }
            }
        },
        changePosX: function (e) {
            var posX = parseInt(this.selectItem.Left) || 0;
            if (posX < 0) {
                this.selectItem.Left = 0;
                toast('不能移出快递单区域');
            } else if (posX + Number(this.selectItem.Width) > Number(this.curSelect.Width)) {
                this.selectItem.Left = this.curSelect.Width - this.selectItem.Width;
                toast('不能移出快递单区域');
            } else {
                this.selectItem.Left = posX;
            }
        },
        changePosY: function (e) {
            var posY = parseInt(this.selectItem.Top) || 0;
            if (posY < 0) {
                this.selectItem.Top = 0;
                toast('不能移出快递单区域');
            } else if (posY + Number(this.selectItem.Height) > Number(this.curSelect.Height)) {
                this.selectItem.Top = this.curSelect.Height - this.selectItem.Height;
                toast('不能移出快递单区域');
            } else {
                this.selectItem.Top = posY;
            }
        },
        changeWidth: function (e) {
            var width = parseInt(this.selectItem.Width) || 0;
            if (width < 0) {
                this.selectItem.Width = 0;
                toast('宽度不能小于零');
            } else if (width + Number(this.selectItem.Left) > Number(this.curSelect.Width)) {
                this.selectItem.Width = this.curSelect.Width - this.selectItem.Left;
                toast('不能超出快递单区域');
            } else {
                this.selectItem.Width = width;
            }
        },
        changeHeight: function (e) {
            var height = parseInt(this.selectItem.Height) || 0;
            if (height < 0) {
                this.selectItem.Height = 0;
                toast('高度不能小于零');
            } else if (height + Number(this.selectItem.Top) > Number(this.curSelect.Height)) {
                this.selectItem.Height = this.curSelect.Height - this.selectItem.Top;
                toast('不能超出快递单区域');
            } else {
                this.selectItem.Height = height;
            }
        },
        expCompanyFilter: function (value) {
            for (var i = 0; i < this.expCompany.length; i++) {
                if (this.expCompany[i].ExpCode == value) {
                    return '快递公司: ' + this.expCompany[i].ExpCompanyName;
                }
            }
        },
        clone: function (obj) {
            var o;
            if (typeof obj == 'object') {
                if (obj === null) {
                    o = null;
                } else {
                    if (o instanceof Array) {
                        o = [];
                        for (var i = 0; i < obj.length; i++) {
                            o.push(obj[i]);
                        }
                    } else {
                        o = {};
                        for (var key in obj) {
                            o[key] = obj[key];
                        }
                    }
                }
            } else {
                o = obj;
            };
            return o;
        }
    },
    filters: {
        propertyNameFilter: function (value) {
            switch (value) {
                case 'SenderName':
                    return '#发件人#';
                    break;
                case 'SenderPhone':
                    return '#发件人电话#';
                    break;
                case 'SenderAddress':
                    return '#发件人地址#';
                    break;
                case 'SendPostCode':
                    return '#发件人邮编#';
                    break;
                case 'SenderCompany':
                    return '#发件人公司名称#';
                    break;
                case 'ReceiptUserName':
                    return '#收件人#';
                    break;
                case 'ReceiptCompany':
                    return '#收件人公司名称#';
                    break;
                case 'ReceiptPhone':
                    return '#收件人电话#';
                    break;
                case 'ReceiptAddress':
                    return '#收件人地址#';
                    break;
                case 'RecipientsZipCode':
                    return '#收件人邮编#';
                    break;
                case 'CommodityName':
                    return '#商品名称#';
                    break;
                case 'CommodityCode':
                    return '#商品编码#';
                    break;
                case 'CommodityAttributes':
                    return '#商品属性#';
                    break;
                case 'Number':
                    return '#购买数量#';
                    break;
                case 'OrderCode':
                    return '#订单编号#';
                    break;
                case 'BuyersRemark':
                    return '#买家留言#';
                    break;
                case 'SellersRemark':
                    return '#卖家留言#';
                    break;
                case 'DirectPrint1':
                    return '#直接打印1#';
                    break;
                case 'DirectPrint2':
                    return '#直接打印2#';
                    break;
                case 'DirectPrint3':
                    return '#直接打印3#';
                    break;
            }
        }
    }
});