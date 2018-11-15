var vm = new Vue({
    el: '#app',
    data:{
        navList:['可用优惠券','不可用优惠券'],
        lists:[],
        selected: '',
        available: null
    },
    mounted:function(){
        this.$nextTick(function(){
            this.$loading.open('加载中');
            this.available = true;
            this.loadDate();
        });
    },
    methods:{
        loadDate: function(){
            this.$http({
                url:'/coupon',
                method:'GET',
                param:{}
            }).then(function(response){

            },function(err){
                this.$loading.close();
                var data = {
                    count: 2,
                    data: [
                        {
                            id: '1',
                            name:'河套酒',
                            price: 100,
                            available: 500,
                            date: '1502992740000'
                        },
                        {
                            id: '2',
                            name:'河套酒',
                            price: 100,
                            available: 500,
                            date: '1502992740000'
                        }
                    ],
                    selected: '1'
                };
                this.navList = ['可用优惠券 (' + data.count +')','不可用优惠券'],
                this.lists = data.data;
                this.selected = data.selected;
            });
        },
        itemClick: function(id){
            this.selected = id;
        },
        navChangeFun: function(val){
            this.available = val ? false : true;
        }
    },
    filters:{
        textFilter: function(value){
            return '满' + value + '可用';
        },
        dateFilter: function(value){
            var date = new Date(Number(value));
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var day = date.getDate();
            return '有效期至' + year + '年' + month + '月' + day + '日'
        }
    }
});